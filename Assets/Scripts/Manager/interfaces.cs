using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Worker;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Manager
{
    public interface IInitializable
    {
        void OnInit();
    }

    public interface IUpdatable
    {
        void OnUpdate();
    }

    public interface IStateful<T>
    {
        T    GetState();
        void ChangeState(T newState);
    }

    public interface IHarvestable
    {
        BigNumber OnHarvest();
        bool      IsReady { get; }
    }

    public interface IUnlockable
    {
        bool TryUnlock();
        bool IsUnlocked { get; }
    }

    public interface IUpgradeable
    {
        bool      TryUpgrade();
        BigNumber GetUpgradeCost();
        BigNumber GetNextProfit();
        int       Level { get; }
    }

    public interface IBuffable
    {
        void ApplyModifier(StatModifier modifier);
    }

    public interface IMovable
    {
        void    WarpTo(Vector3 position);
        Vector3 GetStandPoint();
    }

    public interface IDispatchable
    {
        void        OnGenerateOrder(Assembly_CSharp.Assets.Scripts.Slot.BaseSlot slot,
                                    Assembly_CSharp.Assets.Scripts.Customer.BaseCustomer customer,
                                    Vector3 deliveryPoint);
        WorkerState GetWorkerState();
    }
}
