using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    // Thêm worker
    [CreateAssetMenu(menuName = "IdleFarm/AddWorker")]
    public class AddWorkerAction : ScriptableObject, IUpgradeAction
    {
        public void Execute(GameController gc) =>
            gc.WorkerController.AddWorker();
    }
}