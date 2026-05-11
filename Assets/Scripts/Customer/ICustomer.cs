using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Customer
{
    public interface ICustomer
    {
        void OnInit(CounterSlotManager counterSlotManager, Vector3 spawnPosition, Vector3 endPosition);
        void OnUpdate();
    }
    // public enum CustomerStateType
    // {
    //     None,
    //     Idle,
    //     MovingToCounter,
    //     Waiting,
    //     ReceiveGood,
    //     MoveOut,
    // }
}