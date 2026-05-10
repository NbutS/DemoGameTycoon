using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assembly_CSharp.Assets.Scripts.Slot;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Worker;
using System;

namespace Scripts.Worker
{
    public class BaseWorker : MonoBehaviour, IWorker
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private List<GameObject> listApples;

        private Vector3 _homePosition;
        private WorkerState _workerState = WorkerState.Idle;
        private OrderDat _orderDat;
        private BigNumber _carrying;
        public event Action<BaseWorker> OnBecameIdle;


        public void WarpTo(Vector3 position)
        {
            agent.Warp(position);
        }

        public virtual void OnInit(CounterSlotManager counterSlotManager, Vector3 homePos)
        {
            _homePosition = homePos;
            foreach (var apple in listApples)
                apple.SetActive(false);
        }

        public virtual void OnUpdate() { }

        public void OnGenerateOrder(BaseSlot slot, BaseCustomer customer, Vector3 deliveryPoint)
        {
            _orderDat = new OrderDat
            {
                BaseSlot = slot,
                BaseCustomer = customer,
                DeliveryPoint = deliveryPoint,
            };
            SetState(WorkerState.MovingToSlot);
        }

        private void SetState(WorkerState newState)
        {
            _workerState = newState;

            switch (_workerState)
            {
                case WorkerState.Idle:
                    _orderDat = null;
                    _carrying = 0;
                    StartCoroutine(ReturnHomeRoutine());
                    break;
                case WorkerState.MovingToSlot:
                    animator.Play("Move");
                    StartCoroutine(MoveToSlotRoutine());
                    break;
                case WorkerState.Harvesting:
                    animator.Play("CarryIdle");
                    StartCoroutine(HarvestRoutine());
                    break;
                case WorkerState.MovingToCustomer:
                    animator.Play("CarryMove");
                    StartCoroutine(MoveToCustomerRoutine());
                    break;
                case WorkerState.Delivering:
                    StartCoroutine(DeliverRoutine());
                    break;
            }
        }

        IEnumerator ReturnHomeRoutine()
        {
            animator.Play("Move");
            agent.SetDestination(_homePosition);
            yield return new WaitUntil(HasArrived);
            animator.Play("Idle"); 
            yield return null;  
            OnBecameIdle?.Invoke(this);  
        }

        IEnumerator MoveToSlotRoutine()
        {
            agent.SetDestination(_orderDat.BaseSlot.GetStandPoint());
            yield return new WaitUntil(HasArrived);
            SetState(WorkerState.Harvesting);
        }

        IEnumerator HarvestRoutine()
        {
           _carrying = _orderDat.BaseSlot.OnHarvest();

            foreach (var apple in listApples)
            {
                apple.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }

            SetState(WorkerState.MovingToCustomer);
        }

        IEnumerator MoveToCustomerRoutine()
        {
            if (_orderDat.DeliveryPoint == Vector3.zero)
            {
                SetState(WorkerState.Idle);
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

            SetState(WorkerState.Delivering);
        }

        IEnumerator DeliverRoutine()
        {
            foreach (var apple in listApples)
                apple.SetActive(false);

            _orderDat.BaseCustomer.OnReceiveGoods();
            yield return new WaitForSeconds(0.5f);

            CurrencyManager.Instance.AddCoins(_carrying);
            SetState(WorkerState.Idle);
        }

        private bool HasArrived()
        {
            if (agent.pathPending) return false;
            return agent.remainingDistance <= agent.stoppingDistance;
        }

        public WorkerState GetWorkerState() => _workerState;
    }
}