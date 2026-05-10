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
            _currentCost = BigNumber.FromDouble(config.cost);
            nameText.text = config.displayName;
            descText.text = config.description;
            RefreshCost();
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }

        private void OnBuyClicked()
        {
            if (!CurrencyManager.Instance.SpendCoins(_currentCost))
            {
                PopupManager.Instance.Get<NotEnoughCoinPopup>(PopupType.NotEnoughCoin).Setup();
                return;
            }
            Execute();
            _currentCost = _currentCost * 2.0;
            RefreshCost();
        }

        private void Execute()
        {
            switch (config.upgradeType)
            {
                case UpgradeOptionType.AddCustomer:
                    GameController.Instance.CustomerController.AddCustomer();
                    break;
                case UpgradeOptionType.AddWorker:
                    GameController.Instance.WorkerController.AddWorker();
                    break;
                case UpgradeOptionType.X2SlotProfit:
                    GameController.Instance.SlotController.X2SlotProfit(config.slotIndex);
                    break;
                case UpgradeOptionType.X2GlobalProfit:
                    GameController.Instance.SlotController.X2GlobalProfit();
                    break;
            }
        }

        private void RefreshCost() => costText.text = _currentCost.ToString();

        private string FormatNumber(double n)
        {
            if (n >= 1_000_000_000) return $"{n / 1_000_000_000:0.#}B";
            if (n >= 1_000_000) return $"{n / 1_000_000:0.#}M";
            if (n >= 1_000) return $"{n / 1_000:0.#}k";
            return n.ToString("0");
        }
    }
    public enum UpgradeOptionType
    {
        AddCustomer,
        AddWorker,
        X2SlotProfit,
        X2GlobalProfit,
    }
}