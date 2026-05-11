namespace Assembly_CSharp.Assets.Scripts.Currency
{
    using System;
    using Assembly_CSharp.Assets.Scripts.Ui.Popup;
    using UnityEngine;

    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        private BigNumber _coins = BigNumber.FromDouble(200000);
        public BigNumber Coins => _coins;

        public event Action<BigNumber> OnCoinChanged;

        private void Awake() => Instance = this;

        public bool CanAfford(BigNumber amount) => _coins >= amount;

        public void AddCoins(BigNumber amount)
        {
            if (amount <= BigNumber.Zero) return;
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