using System;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public enum SlotStateType
    {
        Close,
        Opening,
        NotReady,
        Ready,
    }

    [CreateAssetMenu(fileName = "SlotConfig", menuName = "IdleFarm/SlotConfig")]
    public class SlotConfig : ScriptableObject
    {
        [SerializeField] private int slotIndex;
        [SerializeField] private Sprite icon;
        [SerializeField] private double unlockCost;
        [SerializeField] private double baseProfit;
        [SerializeField] private float cooldownDuration;
        [SerializeField] private double upgradeCost;
        [SerializeField] private double upgradeProfitMultiplier = 1.1;

        public int SlotIndex => slotIndex;
        public Sprite Icon => icon;
        public float CooldownDuration => cooldownDuration;
        public double UpgradeProfitMultiplier => upgradeProfitMultiplier;

        public BigNumber UnlockCostBig => BigNumber.FromDouble(unlockCost);
        public BigNumber BaseProfitBig => BigNumber.FromDouble(baseProfit);
        public BigNumber UpgradeCostBig => BigNumber.FromDouble(upgradeCost);
    }

    public class SlotData
    {
        public SlotConfig Config { get; }
        public int Level { get; private set; } = 1;
        public StatValue ProfitStat { get; }
        public BigNumber Profit => ProfitStat.Calculate();

        public SlotData(SlotConfig config)
        {
            Config = config;
            ProfitStat = new StatValue(config.BaseProfitBig);
        }

        public void IncrementLevel() => Level++;

        public BigNumber GetUpgradeCost() => Config.UpgradeCostBig * Level;

        public BigNumber GetNextProfit()
        {
            var preview = new StatValue(ProfitStat.BaseValue);
            foreach (var m in ProfitStat.Modifiers)
                preview.AddModifier(m);
            preview.AddModifier(new StatModifier(
                ModifierType.Percent,
                Config.UpgradeProfitMultiplier - 1.0,
                "preview"
            ));
            return preview.Calculate();
        }
    }

    public class SlotStateMachine : IStateful<SlotStateType>
    {
        private SlotStateType _state = SlotStateType.Close;

        public event Action<SlotStateType> OnStateChanged;

        public SlotStateType GetState() => _state;

        public void ChangeState(SlotStateType newState)
        {
            if (_state == newState) return;
            _state = newState;
            OnStateChanged?.Invoke(_state);
        }
    }
}