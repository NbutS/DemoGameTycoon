using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    [CreateAssetMenu(menuName = "IdleFarm/AddCustomer")]
    public class AddCustomerAction : ScriptableObject, IUpgradeAction
    {
        public void Execute(GameController gc) =>
            gc.CustomerController.AddCustomer();
    }
}