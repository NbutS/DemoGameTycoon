using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class SlotWorldUI : MonoBehaviour
    {
        [SerializeField] private GameObject uiRoot;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI profitText;

        private BaseSlot _slot;

        public void Setup(BaseSlot slot)
        {
            _slot = slot;
            uiRoot.SetActive(false);
        }

        public void Show()
        {
            uiRoot.SetActive(true);
            Refresh();
        }

        public void Refresh()
        {
            if (_slot == null) return;
            if (_slot.Config.icon != null)
                icon.sprite = _slot.Config.icon;
            profitText.text = _slot.Profit.ToString();
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