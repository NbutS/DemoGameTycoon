namespace Assembly_CSharp.Assets.Scripts.Currency
{
    using System;
    using Assembly_CSharp.Assets.Scripts.Ui.Popup;
    using UnityEngine;

    public class CurrencyManager : MonoBehaviour
    {
        [SerializeField ] TopUIController topUIController;
        public static CurrencyManager Instance { get; private set; }

        private BigNumber _coins = new BigNumber(10, 6);
        public BigNumber Coins => _coins;

        public event Action<BigNumber> OnCoinChanged;

        private void Awake()
        {
            Instance = this;
            topUIController.OnInit();
        }

        public bool CanAfford(BigNumber amount) => _coins >= amount;

        public void AddCoins(BigNumber amount)
        {
            _coins = _coins + amount;
            OnCoinChanged?.Invoke(_coins);
        }

        public bool SpendCoins(BigNumber amount)
        {
            if (!CanAfford(amount)) return false;
            _coins = _coins - amount;
            OnCoinChanged?.Invoke(_coins);
            return true;
        }
    }
}