using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Slot;
using Scripts.Worker;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Ui.Popup;
namespace Assembly_CSharp.Assets.Scripts.Manager
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [Header("Controllers")]
        [SerializeField] private WorkerController workerController;
        [SerializeField] private SlotController slotController;
        [SerializeField] private CustomerController customerController;

        [Header("Managers")]
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private PopupManager popupManager;

        [Header("UI")]
        [SerializeField] private TopUIController topUIController;

        public WorkerController WorkerController => workerController;
        public SlotController SlotController => slotController;
        public CustomerController CustomerController => customerController;
        public CurrencyManager CurrencyManager => currencyManager;
        public PopupManager PopupManager => popupManager;

        private void Awake()
        {
            Instance = this;
            popupManager.OnInit();
        }

        private void Start()
        {
            slotController.OnInit(currencyManager, popupManager);
            customerController.OnInit();
            workerController.OnInit(customerController.GetCounterSlotManager(), currencyManager);
            topUIController.OnInit(currencyManager);

            slotController.OnSlotReady += OnSlotReady;
            customerController.OnCustomerWaiting += OnCustomerWaiting;
            workerController.OnWorkerIdle += OnWorkerIdle;

            customerController.SendAllIdleToCounter();
        }

        private void Update()
        {
            workerController.OnUpdate();
            slotController.OnUpdate();
            customerController.OnUpdate();
        }

        private void OnDestroy()
        {
            slotController.OnSlotReady -= OnSlotReady;
            customerController.OnCustomerWaiting -= OnCustomerWaiting;
            workerController.OnWorkerIdle -= OnWorkerIdle;
        }

        private void OnSlotReady(BaseSlot slot)
        {
            TryDispatchWorker(slot);
        }

        private void OnCustomerWaiting(BaseCustomer customer)
        {
            if (customer.SlotIndex < 0) return;
            var slot = slotController.GetAvailableSlot();
            if (slot == null) return;
            TryDispatchWorker(slot);
        }

        private void OnWorkerIdle(BaseWorker worker)
        {
            var slot = slotController.GetAvailableSlot();
            if (slot == null) return;
            TryDispatchWorker(slot);
        }

        private void TryDispatchWorker(BaseSlot slot)
        {
            var customer = customerController.GetWaitingCustomer();
            if (customer == null || customer.SlotIndex < 0 || customer.IsTargeted) return;
            workerController.TryDispatch(slot, customer);
        }
        public void OpenGlobalUpgradePopup()
        {
            PopupManager.Get<GlobalUpgradePopup>(PopupType.GlobalUpgrade).Setup();
        }
    }
}
