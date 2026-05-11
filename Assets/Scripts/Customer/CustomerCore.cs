using System;
using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Customer
{
    public enum CustomerStateType
    {
        Idle,
        MovingToCounter,
        Waiting,
        ReceiveGood,
        MoveOut,
    }

    public class CustomerData
    {
        public Vector3 SpawnPosition { get; }
        public Vector3 EndPosition { get; }

        public CustomerData(Vector3 spawnPosition, Vector3 endPosition)
        {
            SpawnPosition = spawnPosition;
            EndPosition = endPosition;
        }
    }

    public class CustomerStateMachine : IStateful<CustomerStateType>
    {
        private CustomerStateType _state = CustomerStateType.Idle;

        public event Action<CustomerStateType> OnStateChanged;

        public CustomerStateType GetState() => _state;

        public void ChangeState(CustomerStateType newState)
        {
            if (_state == newState) return;
            _state = newState;
            OnStateChanged?.Invoke(_state);
        }
    }


    
}