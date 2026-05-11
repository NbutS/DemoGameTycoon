using System;
using System.Collections;
using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Ui.Popup;
using UnityEngine.EventSystems;
using Assembly_CSharp.Assets.Scripts.Manager;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class BaseSlot : MonoBehaviour,
        IInitializable,
        IUpdatable,
        IHarvestable,
        IUnlockable,
        IUpgradeable,
        IBuffable
    {
        [Header("References")]
        [SerializeField] private Transform workerStandPoint;
        [SerializeField] private SlotView slotView;

        [Header("Config")]
        [SerializeField] private float openDuration = 1f;

        private SlotData _data;
        private SlotStateMachine _stateMachine;
        private CurrencyManager _currencyManager;
        private PopupManager _popupManager;

        public bool IsTargeted { get; set; } = false;
        public SlotConfig Config => _data?.Config;
        public int Level => _data?.Level ?? 1;
        public BigNumber Profit => _data?.Profit ?? BigNumber.Zero;

        public event Action OnBecameReady;

        public void Setup(SlotConfig config, CurrencyManager currencyManager, PopupManager popupManager)
        {
            _data = new SlotData(config);
            _stateMachine = new SlotStateMachine();
            _currencyManager = currencyManager;
            _popupManager = popupManager;

            _stateMachine.OnStateChanged += OnStateChanged;
            slotView.OnReadyTriggered += () => _stateMachine.ChangeState(SlotStateType.Ready);
            slotView.Init(_data);
        }

        // ─── IInitializable ───────────────────────────────────────────────────
        public void OnInit()
        {
            IsTargeted = false;
            _stateMachine.ChangeState(SlotStateType.Close);
        }

        public void OnUpdate() { }

        // ─── State ────────────────────────────────────────────────────────────
        public SlotStateType GetSlotState() => _stateMachine.GetState();

        private void OnStateChanged(SlotStateType state)
        {
            if (state == SlotStateType.Ready)
            {
                IsTargeted = false;
                OnBecameReady?.Invoke();
            }
            slotView.OnStateChanged(state, _data, _data.Config.CooldownDuration, openDuration, this);
        }

        // ─── IHarvestable ─────────────────────────────────────────────────────
        public bool IsReady => _stateMachine.GetState() == SlotStateType.Ready;

        public BigNumber OnHarvest()
        {
            if (!IsReady) return BigNumber.Zero;
            IsTargeted = true;
            _stateMachine.ChangeState(SlotStateType.NotReady);
            return Profit;
        }

        // ─── IUnlockable ──────────────────────────────────────────────────────
        public bool IsUnlocked => _stateMachine.GetState() != SlotStateType.Close;

        public bool TryUnlock()
        {
            if (IsUnlocked) return false;
            if (!_currencyManager.SpendCoins(_data.Config.UnlockCostBig))
            {
                _popupManager.Get<NotEnoughCoinPopup>(PopupType.NotEnoughCoin).Setup();
                return false;
            }
            _stateMachine.ChangeState(SlotStateType.Opening);
            return true;
        }

        // ─── IUpgradeable ─────────────────────────────────────────────────────
        public bool TryUpgrade()
        {
            if (!_currencyManager.SpendCoins(GetUpgradeCost()))
            {
                _popupManager.Get<NotEnoughCoinPopup>(PopupType.NotEnoughCoin).Setup();
                return false;
            }
            _data.IncrementLevel();
            _data.ProfitStat.AddModifier(new StatModifier(
                ModifierType.Percent,
                _data.Config.UpgradeProfitMultiplier - 1.0,
                $"upgrade_level_{_data.Level}"
            ));
            slotView.RefreshUI(_data);
            return true;
        }

        public BigNumber GetUpgradeCost() => _data.GetUpgradeCost();
        public BigNumber GetNextProfit() => _data.GetNextProfit();

        // ─── IBuffable ────────────────────────────────────────────────────────
        public void ApplyModifier(StatModifier modifier)
        {
            _data.ProfitStat.AddModifier(modifier);
            slotView.RefreshUI(_data);
        }

        // ─── Click ────────────────────────────────────────────────────────────
        private void OnMouseUp()
        {
            if (IsPointerOverUI()) return;
            SlotClickHandler.Handle(this, _stateMachine.GetState(), _popupManager);
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return true;
            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            return false;
        }

        public Vector3 GetStandPoint() => workerStandPoint.position;
    }
}