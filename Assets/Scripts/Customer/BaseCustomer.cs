using System;
using System.Collections;
using System.Collections.Generic;
using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.AI;

namespace Assembly_CSharp.Assets.Scripts.Customer
{
    public class BaseCustomer : MonoBehaviour, IInitializable, IUpdatable, IMovable
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private List<GameObject> listApples;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private CustomerAnimatorController customerAnimator;

        private CustomerData _data;
        private CustomerStateMachine _stateMachine;
        private Transform _assignedPoint;
        private int _slotIndex = -1;
        private CounterSlotManager _counterSlotManager;

        public CustomerStateType State => _stateMachine?.GetState() ?? CustomerStateType.Idle;
        public int SlotIndex => _slotIndex;
        public bool IsTargeted { get; set; } = false;

        public event Action<BaseCustomer> OnBecameWaiting;

        public void OnInit() { }

        public void Init(CounterSlotManager counterSlotManager, Vector3 spawnPosition, Vector3 endPosition)
        {
            _counterSlotManager = counterSlotManager;
            _data = new CustomerData(spawnPosition, endPosition);
            _stateMachine = new CustomerStateMachine();
            IsTargeted = false;

            _stateMachine.OnStateChanged += OnStateChanged;

            foreach (var apple in listApples)
                apple.SetActive(false);
        }

        public void OnUpdate() { }

        public void WarpTo(Vector3 position) => agent.Warp(position);

        public Vector3 GetStandPoint()
        {
            if (_slotIndex < 0) return transform.position;
            var workerPoint = _counterSlotManager.GetWorkerPointByIndex(_slotIndex);
            return workerPoint != null ? workerPoint.position : transform.position;
        }

        private void OnStateChanged(CustomerStateType state)
        {
            customerAnimator.OnStateChanged(state);

            switch (state)
            {
                case CustomerStateType.MovingToCounter:
                    StartCoroutine(MoveToCounterRoutine());
                    break;
                case CustomerStateType.Waiting:
                    OnBecameWaiting?.Invoke(this);
                    break;
                case CustomerStateType.ReceiveGood:
                    StartCoroutine(ReceiveGoodRoutine());
                    break;
                case CustomerStateType.MoveOut:
                    StartCoroutine(MoveOutRoutine());
                    break;
            }
        }

        IEnumerator MoveToCounterRoutine()
        {
            agent.SetDestination(_data.SpawnPosition);

            // Chờ agent bắt đầu tính path trước khi check HasArrived
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(HasArrived);

            _assignedPoint = _counterSlotManager.GetAvailableCustomerPoint(this, out _slotIndex);
            if (_assignedPoint == null)
            {
                _stateMachine.ChangeState(CustomerStateType.Idle);
                yield break;
            }

            agent.SetDestination(_assignedPoint.position);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(HasArrived);
            yield return RotateTo(_assignedPoint.forward);
            _stateMachine.ChangeState(CustomerStateType.Waiting);
        }

        IEnumerator ReceiveGoodRoutine()
        {
            foreach (var apple in listApples)
            {
                apple.SetActive(true);
                yield return new WaitForSeconds(0.15f);
            }
            _stateMachine.ChangeState(CustomerStateType.MoveOut);
        }

        IEnumerator MoveOutRoutine()
        {
            IsTargeted = false;
            _counterSlotManager.ReleasePoint(this);
            _assignedPoint = null;
            _slotIndex = -1;

            agent.SetDestination(_data.EndPosition);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(HasArrived);

            foreach (var apple in listApples)
                apple.SetActive(false);

            // Warp về spawn rồi chờ agent sync xong
            agent.Warp(_data.SpawnPosition);
            yield return new WaitForSeconds(0.1f);

            _stateMachine.ChangeState(CustomerStateType.Idle);
            yield return null;
            MoveToCounter();
        }

        private IEnumerator RotateTo(Vector3 direction)
        {
            direction.y = 0;
            if (direction == Vector3.zero) yield break;

            Quaternion target = Quaternion.LookRotation(-direction);
            while (Quaternion.Angle(transform.rotation, target) > 0.5f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, target, Time.deltaTime * rotateSpeed);
                yield return null;
            }
            transform.rotation = target;
        }

        public void MoveToCounter()
        {
            if (State != CustomerStateType.Idle) return;
            _stateMachine.ChangeState(CustomerStateType.MovingToCounter);
        }

        public void OnReceiveGoods()
        {
            if (State != CustomerStateType.Waiting) return;
            IsTargeted = false;
            _stateMachine.ChangeState(CustomerStateType.ReceiveGood);
        }

        private bool HasArrived()
        {
            if (agent.pathPending) return false;
            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }
}