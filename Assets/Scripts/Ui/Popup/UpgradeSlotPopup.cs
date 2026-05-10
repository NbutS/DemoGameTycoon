using Assembly_CSharp.Assets.Scripts.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class UpgradeSlotPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI currentProfitText;
        [SerializeField] private TextMeshProUGUI nextProfitText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Vector2 offset = new Vector2(0, 150);
        // [SerializeField] private Image icon;
        private BaseSlot _slot;

        public override void OnInit()
        {
            upgradeButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() => OnUpgradeClicked());
            closeButton.onClick.AddListener(() => PopupManager.Instance.HideCurrent());
        }

        public void Setup(BaseSlot slot)
        {
            _slot = slot;
            // if (icon != null && slot.Config.icon != null)
            //     icon.sprite = slot.Config.icon;
            Refresh();
            ShowAtWorldPosition(slot.transform.position, offset);
        }

        private void Refresh()
        {
            levelText.text = $"Level {_slot.Level}";
            currentProfitText.text = _slot.Profit.ToString();
            nextProfitText.text = _slot.GetNextProfit().ToString();
            upgradeCostText.text = _slot.GetUpgradeCost().ToString();
        }

        private void OnUpgradeClicked()
        {
            if (_slot.TryUpgrade())
                Refresh();
        }

        private string FormatNumber(double n)
        {
            if (n >= 1_000_000_000) return $"{n / 1_000_000_000:0.#}B";
            if (n >= 1_000_000) return $"{n / 1_000_000:0.#}M";
            if (n >= 1_000) return $"{n / 1_000:0.#}k";
            return n.ToString("0");
        }
    }
}