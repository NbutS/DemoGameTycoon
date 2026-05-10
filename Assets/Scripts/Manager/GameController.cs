using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Slot;
using Scripts.Worker;
namespace Assembly_CSharp.Assets.Scripts.Manager
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [SerializeField] private WorkerController workerController;
        [SerializeField] private SlotController slotController;
        [SerializeField] private CustomerController customerController;

        public WorkerController WorkerController => workerController;
        public SlotController SlotController => slotController;
        public CustomerController CustomerController => customerController;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            slotController.OnInit();
            customerController.OnInit();
            workerController.OnInit(customerController.GetCounterSlotManager());

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
        private void OnWorkerIdle(BaseWorker worker)
        {
            var slot = slotController.GetAvailableSlot();
            if (slot == null) return;
            TryDispatchWorker(slot);
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

        private void TryDispatchWorker(BaseSlot slot)
        {
            var customer = customerController.GetWaitingCustomer();
            if (customer == null || customer.SlotIndex < 0 || customer.IsTargeted) return;
            workerController.TryDispatch(slot, customer);
        }
    }
}
