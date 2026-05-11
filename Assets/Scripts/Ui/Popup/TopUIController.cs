using Assembly_CSharp.Assets.Scripts.Currency;
using TMPro;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class TopUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        public void OnInit(CurrencyManager currencyManager)
        {
            currencyManager.OnCoinChanged += UpdateCoinText;
            UpdateCoinText(currencyManager.Coins);
        }

        private void UpdateCoinText(BigNumber amount)
        {
            coinText.text = amount.ToString();
        }
    }

}