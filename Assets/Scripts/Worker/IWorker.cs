using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Slot;
using UnityEngine;

public interface IWorker
{
    void OnInit(CounterSlotManager slotController, Vector3 homePos);
    void OnUpdate();
}

// public enum WorkerState
// {
//     None,
//     Idle,
//     MovingToSlot,
//     Harvesting,
//     MovingToCustomer,
//     Delivering,

    
// }