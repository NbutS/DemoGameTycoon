using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class UpgradeOptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button buyButton;
        [SerializeField] private UpgradeOptionConfig config;

        private BigNumber _currentCost;

        public void OnInit()
        {
            _currentCost = BigNumber.FromDouble(config.Cost);
            nameText.text = config.DisplayName;
            descText.text = config.Description;
            RefreshCost();
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }

        private void OnBuyClicked()
        {
            if (!GameController.Instance.CurrencyManager.SpendCoins(_currentCost))
            {
                GameController.Instance.PopupManager
                    .Get<NotEnoughCoinPopup>(PopupType.NotEnoughCoin)
                    .Setup();
                return;
            }

            config.GetAction()?.Execute(GameController.Instance);
            _currentCost = _currentCost * 2.0;
            RefreshCost();
        }

        private void RefreshCost() => costText.text = _currentCost.ToString();
    }
    public enum UpgradeOptionType
    {
        AddCustomer,
        AddWorker,
        X2SlotProfit,
        X2GlobalProfit,
    }
}