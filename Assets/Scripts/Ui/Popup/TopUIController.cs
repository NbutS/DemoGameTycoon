using Assembly_CSharp.Assets.Scripts.Currency;
using TMPro;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class TopUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        private void OnEnable()
        {

        }
        public void OnInit()
        {
            CurrencyManager.Instance.OnCoinChanged += UpdateCoinText;
            UpdateCoinText(CurrencyManager.Instance.Coins);
        }

        private void OnDisable()
        {
            CurrencyManager.Instance.OnCoinChanged -= UpdateCoinText;
        }

        private void UpdateCoinText(BigNumber amount)
        {
            coinText.text = amount.ToString();
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