using Assembly_CSharp.Assets.Scripts.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class UnlockSlotPopup : BasePopup
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Vector2 offset = new Vector2(0, 150);

        private BaseSlot _slot;

        public override void OnInit()
        {
            unlockButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(OnUnlockClicked);
            closeButton.onClick.AddListener(() => PopupManager.Instance.HideCurrent());
        }

        public void Setup(BaseSlot slot)
        {
            _slot = slot;
            costText.text = FormatNumber(slot.Config.unlockCost);
            if (icon != null && slot.Config.icon != null)
                icon.sprite = slot.Config.icon;
            ShowAtWorldPosition(slot.transform.position, offset);
            
        }

        private void OnUnlockClicked()
        {
            if (_slot.TryUnlock())
                PopupManager.Instance.HideCurrent();
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