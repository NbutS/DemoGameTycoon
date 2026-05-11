using Assembly_CSharp.Assets.Scripts.Manager;
using Assembly_CSharp.Assets.Scripts.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class UpgradeSlotPopup : BasePopup
    {
        [SerializeField] private Image           icon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI currentProfitText;
        [SerializeField] private TextMeshProUGUI nextProfitText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button          upgradeButton;
        [SerializeField] private Button          closeButton;
        [SerializeField] private Vector2         offset = new Vector2(0, 150);

        private BaseSlot _slot;

        public override void OnInit()
        {
            upgradeButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
            closeButton.onClick.AddListener(() => GameController.Instance.PopupManager.HideCurrent());
        }

        public void Setup(BaseSlot slot)
        {
            _slot = slot;

            if (icon != null && slot.Config.Icon != null)
                icon.sprite = slot.Config.Icon;

            Refresh();
            ShowAtWorldPosition(slot.transform.position, offset);
        }

        private void Refresh()
        {
            levelText.text         = $"Level {_slot.Level}";
            currentProfitText.text = _slot.Profit.ToString();
            nextProfitText.text    = _slot.GetNextProfit().ToString();
            upgradeCostText.text   = _slot.GetUpgradeCost().ToString();
        }

        private void OnUpgradeClicked()
        {
            if (_slot.TryUpgrade())
                Refresh();
        }
    }
}