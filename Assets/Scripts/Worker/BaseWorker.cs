using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assembly_CSharp.Assets.Scripts.Slot;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Worker;
using System;
using Assembly_CSharp.Assets.Scripts.Manager;

namespace Scripts.Worker
{
    public class BaseWorker : MonoBehaviour, IInitializable, IUpdatable, IMovable, IDispatchable
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private WorkerAnimatorController workerAnimator;
        [SerializeField] private List<GameObject> listApples;

        private Vector3 _homePosition;
        private WorkerStateMachine _stateMachine;
        private OrderDat _orderDat;
        private BigNumber _carrying;
        private CurrencyManager _currencyManager;

        public event Action<BaseWorker> OnBecameIdle;

        public void OnInit() { }

        public void Init(CounterSlotManager counterSlotManager, Vector3 homePos, CurrencyManager currencyManager)
        {
            _homePosition = homePos;
            _currencyManager = currencyManager;
            _stateMachine = new WorkerStateMachine();

            _stateMachine.OnStateChanged += OnStateChanged;

            foreach (var apple in listApples)
                apple.SetActive(false);
        }

        public void OnUpdate() { }

        public void WarpTo(Vector3 position) => agent.Warp(position);

        public Vector3 GetStandPoint() => transform.position;

        public void OnGenerateOrder(BaseSlot slot, BaseCustomer customer, Vector3 deliveryPoint)
        {
            _orderDat = new OrderDat
            {
                BaseSlot = slot,
                BaseCustomer = customer,
                DeliveryPoint = deliveryPoint,
            };
            _stateMachine.ChangeState(WorkerState.MovingToSlot);
        }

        public WorkerState GetWorkerState() => _stateMachine.GetState();

        private void OnStateChanged(WorkerState state)
        {
            workerAnimator.OnStateChanged(state);

            switch (state)
            {
                case WorkerState.Idle:
                    _orderDat = null;
                    _carrying = BigNumber.Zero;
                    StartCoroutine(ReturnHomeRoutine());
                    break;
                case WorkerState.MovingToSlot:
                    StartCoroutine(MoveToSlotRoutine());
                    break;
                case WorkerState.Harvesting:
                    StartCoroutine(HarvestRoutine());
                    break;
                case WorkerState.MovingToCustomer:
                    StartCoroutine(MoveToCustomerRoutine());
                    break;
                case WorkerState.Delivering:
                    StartCoroutine(DeliverRoutine());
                    break;
            }
        }

        IEnumerator ReturnHomeRoutine()
        {
            agent.SetDestination(_homePosition);
            yield return new WaitUntil(HasArrived);
            workerAnimator.PlayIdle();
            yield return null;
            OnBecameIdle?.Invoke(this);
        }

        IEnumerator MoveToSlotRoutine()
        {
            agent.SetDestination(_orderDat.BaseSlot.GetStandPoint());
            yield return new WaitUntil(HasArrived);
            _stateMachine.ChangeState(WorkerState.Harvesting);
        }

        IEnumerator HarvestRoutine()
        {
            _carrying = _orderDat.BaseSlot.OnHarvest();
            foreach (var apple in listApples)
            {
                apple.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }
            _stateMachine.ChangeState(WorkerState.MovingToCustomer);
        }

        IEnumerator MoveToCustomerRoutine()
        {
            if (_orderDat.DeliveryPoint == Vector3.zero)
            {
                _stateMachine.ChangeState(WorkerState.Idle);
                yield break;
            }

            agent.SetDestination(_orderDat.DeliveryPoint);

            float elapsed = 0f;
            yield return new WaitUntil(() =>
            {
                elapsed += Time.deltaTime;
                if (elapsed >= 10f) return true;
                return HasArrived();
            });

            _stateMachine.ChangeState(WorkerState.Delivering);
        }

        IEnumerator DeliverRoutine()
        {
            foreach (var apple in listApples)
                apple.SetActive(false);

            _orderDat.BaseCustomer.OnReceiveGoods();
            yield return new WaitForSeconds(0.5f);

            _currencyManager.AddCoins(_carrying);
            _stateMachine.ChangeState(WorkerState.Idle);
        }

        private bool HasArrived()
        {
            if (agent.pathPending) return false;
            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }
}