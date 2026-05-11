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

        private SlotData _data;

        public void Setup(SlotData data)
        {
            _data = data;
            uiRoot.SetActive(false);
        }

        public void Show()
        {
            uiRoot.SetActive(true);
            Refresh(_data);
        }

        public void Refresh(SlotData data)
        {
            if (data == null) return;
            if (icon != null && data.Config.Icon != null)
                icon.sprite = data.Config.Icon;
            profitText.text = data.Profit.ToString();
        }
    }
}