using System;
using System.Collections;
using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Ui.Popup;
using UnityEngine.EventSystems;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class BaseSlot : MonoBehaviour, ISlot
    {
        [Header("References")]
        [SerializeField] private BoxElement box;
        [SerializeField] private TreeElement tree;
        [SerializeField] private Transform workerStandPoint;

        [Header("Config")]
        [SerializeField] private float openDuration = 1f;
        [SerializeField] private SlotWorldUI slotWorldUI;

        private SlotConfig _config;
        private SlotStateType _state = SlotStateType.Close;
        private int _level = 1;
        private BigNumber _profit;

        public bool IsTargeted { get; set; } = false;
        public SlotConfig Config => _config;
        public int Level => _level;
        public BigNumber Profit => _profit;

        public event Action OnBecameReady;

        public void Setup(SlotConfig config)
        {
            _config = config;
            slotWorldUI.Setup(this);
        }

        public void OnInit()
        {
            _profit = _config.BaseProfitBig;
            box.gameObject.SetActive(true);
            tree.gameObject.SetActive(false);
            IsTargeted = false;
            ChangeState(SlotStateType.Close);
        }

        public void OnUpdate() { }

        public SlotStateType GetSlotState() => _state;

        public void ChangeState(SlotStateType newState)
        {
            _state = newState;

            switch (_state)
            {
                case SlotStateType.Close:
                    break;
                case SlotStateType.Opening:
                    StartCoroutine(OpenRoutine());
                    break;
                case SlotStateType.NotReady:
                    StartCoroutine(RegrowRoutine());
                    break;
                case SlotStateType.Ready:
                    IsTargeted = false;
                    tree.OnSpawnApple();
                    slotWorldUI.Show();
                    slotWorldUI.Refresh();
                    OnBecameReady?.Invoke();
                    break;
            }
        }
        private bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

            return false;
        }

        private void OnMouseUp()
        {
            if (IsPointerOverUI())
                return;
            switch (_state)
            {
                case SlotStateType.Close:
                    PopupManager.Instance
                        .Get<UnlockSlotPopup>(PopupType.UnlockSlot)
                        .Setup(this);
                    break;
                case SlotStateType.Ready:
                case SlotStateType.NotReady:
                    PopupManager.Instance
                        .Get<UpgradeSlotPopup>(PopupType.UpgradeSlot)
                        .Setup(this);
                    break;
            }
        }

        private IEnumerator OpenRoutine()
        {
            box.OnOpen();
            yield return new WaitForSeconds(openDuration);
            box.OnClose();

            tree.gameObject.SetActive(true);
            tree.transform.localScale = Vector3.zero;
            yield return AnimateScale(tree.gameObject, Vector3.zero, Vector3.one, 0.4f);
            tree.OnOpen();

            yield return new WaitForSeconds(_config.cooldownDuration);
            ChangeState(SlotStateType.Ready);
        }

        private IEnumerator RegrowRoutine()
        {
            tree.OnHideApple();
            yield return new WaitForSeconds(_config.cooldownDuration);
            ChangeState(SlotStateType.Ready);
        }

        public BigNumber OnHarvest()
        {
            if (_state != SlotStateType.Ready) return BigNumber.Zero;
            IsTargeted = true;
            ChangeState(SlotStateType.NotReady);
            return _profit;
        }

        public bool TryUnlock()
        {
            if (_state != SlotStateType.Close) return false;
            if (!CurrencyManager.Instance.SpendCoins(_config.UnlockCostBig))
            {
                PopupManager.Instance.Get<NotEnoughCoinPopup>(PopupType.NotEnoughCoin).Setup();
                return false;
            }
            ChangeState(SlotStateType.Opening);
            return true;
        }

        public bool TryUpgrade()
        {
            if (!CurrencyManager.Instance.SpendCoins(GetUpgradeCost()))
            {
                PopupManager.Instance.Get<NotEnoughCoinPopup>(PopupType.NotEnoughCoin).Setup();
                return false;
            }
            _level++;
            _profit = _profit * _config.upgradeProfitMultiplier;
            slotWorldUI.Refresh();
            return true;
        }

        public void X2Profit()
        {
            _profit = _profit * 2.0;
            slotWorldUI.Refresh();
        }
        public BigNumber GetNextProfit() => _profit * _config.upgradeProfitMultiplier;
        public BigNumber GetUpgradeCost() => _config.UpgradeCostBig * _level;
        public Vector3 GetStandPoint() => workerStandPoint.position;

        private IEnumerator AnimateScale(GameObject target, Vector3 from, Vector3 to, float duration)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                target.transform.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }
            target.transform.localScale = to;
        }
    }
}