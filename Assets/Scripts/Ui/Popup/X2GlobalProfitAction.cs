using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    [CreateAssetMenu(menuName = "IdleFarm/X2GlobalProfit")]
    public class X2GlobalProfitAction : ScriptableObject, IUpgradeAction
    {
        public void Execute(GameController gc) =>
            gc.SlotController.X2GlobalProfit();
    }
}