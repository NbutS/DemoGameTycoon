// using Assembly_CSharp.Assets.Scripts.Manager;
using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    [CreateAssetMenu(menuName = "IdleFarm/X2SlotProfit")]
    public class X2SlotProfitAction : ScriptableObject, IUpgradeAction
    {
        [SerializeField] private int slotIndex;

        public void Execute(GameController gc)
        {
            Debug.Log($"x2 value: {slotIndex}");
            gc.SlotController.X2SlotProfit(slotIndex);
            
        }
    }
}