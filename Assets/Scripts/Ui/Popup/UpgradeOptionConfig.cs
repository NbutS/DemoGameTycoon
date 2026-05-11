using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    [CreateAssetMenu(menuName = "IdleFarm/UpgradeOptionConfig")]
    public class UpgradeOptionConfig : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private string _description;
        [SerializeField] private double _cost;
        [SerializeField] private ScriptableObject _actionAsset;

        public string DisplayName => _displayName;
        public string Description => _description;
        public double Cost => _cost;

        public IUpgradeAction GetAction() => _actionAsset as IUpgradeAction;
    }
    public interface IUpgradeAction
    {
        void Execute(GameController gameController);
    }
}