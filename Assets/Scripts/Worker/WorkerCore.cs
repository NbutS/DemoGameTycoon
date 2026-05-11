using System;
using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Worker
{
    public enum WorkerState
    {
        Idle,
        MovingToSlot,
        Harvesting,
        MovingToCustomer,
        Delivering,
    }

    public class WorkerStateMachine : IStateful<WorkerState>
    {
        private WorkerState _state = WorkerState.Idle;

        public event Action<WorkerState> OnStateChanged;

        public WorkerState GetState() => _state;

        public void ChangeState(WorkerState newState)
        {
            _state = newState;
            OnStateChanged?.Invoke(_state);
        }
    }

    
    
}