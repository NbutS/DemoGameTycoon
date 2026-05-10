using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assembly_CSharp.Assets.Scripts.Customer
{
    public class BaseCustomer : MonoBehaviour, ICustomer
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private List<GameObject> listApples;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private Animator animator;

        private Vector3 _spawnPosition;
        private Vector3 _endPosition;
        private Transform _assignedPoint;
        private int _slotIndex = -1;
        private CounterSlotManager _counterSlotManager;

        private CustomerStateType _state = CustomerStateType.Idle;
        public CustomerStateType State => _state;
        public int SlotIndex => _slotIndex;
        public bool IsTargeted { get; set; } = false;

        public event Action<BaseCustomer> OnBecameWaiting;

        public void WarpTo(Vector3 position)
        {
            agent.Warp(position);
        }

        public void OnInit(CounterSlotManager counterSlotManager, Vector3 spawnPosition, Vector3 endPosition)
        {
            _counterSlotManager = counterSlotManager;
            _spawnPosition = spawnPosition;
            _endPosition = endPosition;

            foreach (var apple in listApples)
                apple.SetActive(false);

            IsTargeted = false;
            _state = CustomerStateType.Idle;
        }

        public void OnUpdate() { }

        private void SetState(CustomerStateType newState)
        {
            _state = newState;

            switch (_state)
            {
                case CustomerStateType.Idle:
                    animator.Play("Idle");
                    break;
                case CustomerStateType.MovingToCounter:
                    animator.Play("Move");
                    StartCoroutine(MoveToCounterRoutine());
                    break;
                case CustomerStateType.Waiting:
                    animator.Play("CarryIdle");
                    OnBecameWaiting?.Invoke(this);
                    break;
                case CustomerStateType.ReceiveGood:
                    StartCoroutine(ReceiveGoodRoutine());
                    break;
                case CustomerStateType.MoveOut:
                    animator.Play("CarryMove");
                    StartCoroutine(MoveOutRoutine());
                    break;
            }
        }

        IEnumerator MoveToCounterRoutine()
        {
            agent.SetDestination(_spawnPosition);
            yield return new WaitUntil(HasArrived);

            _assignedPoint = _counterSlotManager.GetAvailableCustomerPoint(this, out _slotIndex);
            if (_assignedPoint == null)
            {
                SetState(CustomerStateType.Idle);
                yield break;
            }

            agent.SetDestination(_assignedPoint.position);
            yield return new WaitUntil(HasArrived);
            yield return RotateTo(_assignedPoint.forward);
            SetState(CustomerStateType.Waiting);
        }

        IEnumerator ReceiveGoodRoutine()
        {
            foreach (var apple in listApples)
            {
                apple.SetActive(true);
                yield return new WaitForSeconds(0.15f);
            }
            SetState(CustomerStateType.MoveOut);
        }

        IEnumerator MoveOutRoutine()
        {
            IsTargeted = false;
            _counterSlotManager.ReleasePoint(this);
            _assignedPoint = null;
            _slotIndex = -1;

            agent.SetDestination(_endPosition);
            yield return new WaitUntil(HasArrived);

            foreach (var apple in listApples)
                apple.SetActive(false);

            agent.Warp(_spawnPosition);
            SetState(CustomerStateType.Idle);
            MoveToCounter();
        }

        private IEnumerator RotateTo(Vector3 direction)
        {
            direction.y = 0;
            if (direction == Vector3.zero) yield break;

            Quaternion targetRotation = Quaternion.LookRotation(-direction);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotateSpeed
                );
                yield return null;
            }
            transform.rotation = targetRotation;
        }

        public Vector3 GetStandPoint()
        {
            if (_slotIndex < 0) return transform.position;
            var workerPoint = _counterSlotManager.GetWorkerPointByIndex(_slotIndex);
            return workerPoint != null ? workerPoint.position : transform.position;
        }

        public void MoveToCounter()
        {
            if (_state != CustomerStateType.Idle) return;
            SetState(CustomerStateType.MovingToCounter);
        }

        public void OnReceiveGoods()
        {
            if (_state != CustomerStateType.Waiting) return;
            IsTargeted = false;
            SetState(CustomerStateType.ReceiveGood);
        }

        private bool HasArrived()
        {
            if (agent.pathPending) return false;
            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }
}