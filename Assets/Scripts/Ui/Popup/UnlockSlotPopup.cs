using Assembly_CSharp.Assets.Scripts.Manager;
using Assembly_CSharp.Assets.Scripts.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class UnlockSlotPopup : BasePopup
    {
        [SerializeField] private Image           icon;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button          unlockButton;
        [SerializeField] private Button          closeButton;
        [SerializeField] private Vector2         offset = new Vector2(0, 150);

        private BaseSlot _slot;

        public override void OnInit()
        {
            unlockButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(OnUnlockClicked);
            closeButton.onClick.AddListener(() => GameController.Instance.PopupManager.HideCurrent());
        }

        public void Setup(BaseSlot slot)
        {
            _slot         = slot;
            costText.text = slot.Config.UnlockCostBig.ToString();

            if (icon != null && slot.Config.Icon != null)
                icon.sprite = slot.Config.Icon;

            ShowAtWorldPosition(slot.transform.position, offset);
        }

        private void OnUnlockClicked()
        {
            if (_slot.TryUnlock())
                GameController.Instance.PopupManager.HideCurrent();
        }
    }
}