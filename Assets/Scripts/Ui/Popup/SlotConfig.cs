using Assembly_CSharp.Assets.Scripts.Currency;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    [CreateAssetMenu(fileName = "SlotConfig", menuName = "IdleFarm/SlotConfig")]
    public class SlotConfig : ScriptableObject
    {
        public int slotIndex;
        public Sprite icon;
        public double unlockCost;
        public double baseProfit;
        public float cooldownDuration;
        public double upgradeCost;
        public double upgradeProfitMultiplier;

        public BigNumber UnlockCostBig => BigNumber.FromDouble(unlockCost);
        public BigNumber BaseProfitBig => BigNumber.FromDouble(baseProfit);
        public BigNumber UpgradeCostBig => BigNumber.FromDouble(upgradeCost);
    }








}