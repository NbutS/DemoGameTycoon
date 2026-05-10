using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public abstract class UpgradeOptionConfig : ScriptableObject
    {
        public string displayName;
        public string description;
        public double cost;
        public UpgradeOptionType upgradeType;
        public int slotIndex;

    }
}