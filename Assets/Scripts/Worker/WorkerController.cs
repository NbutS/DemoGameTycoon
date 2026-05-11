using System.Collections.Generic;
using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Slot;
using Assembly_CSharp.Assets.Scripts.Manager;
using System;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Worker;

namespace Scripts.Worker
{
    public class WorkerController : MonoBehaviour
    {
        [SerializeField] private List<BaseWorker> workers;
        [SerializeField] private BaseWorker workerPrefab;
        [SerializeField] private Transform workerParent;

        private CounterSlotManager _counterSlotManager;
        private CurrencyManager _currencyManager;

        public event Action<BaseWorker> OnWorkerIdle;

        public void OnInit(CounterSlotManager counterSlotManager, CurrencyManager currencyManager)
        {
            _counterSlotManager = counterSlotManager;
            _currencyManager = currencyManager;

            for (int i = 0; i < workers.Count; i++)
                InitWorker(workers[i], i);
        }

        public void OnUpdate()
        {
            foreach (var worker in workers)
                worker.OnUpdate();
        }

        private void InitWorker(BaseWorker worker, int index)
        {
            Vector3 homePos = _counterSlotManager.GetWorkerHomePoint(index);
            worker.WarpTo(homePos);
            worker.Init(_counterSlotManager, homePos, _currencyManager);
            worker.OnBecameIdle += w => OnWorkerIdle?.Invoke(w);
        }

        public BaseWorker GetIdleWorker() =>
            workers.Find(w => w.GetWorkerState() == WorkerState.Idle);

        public bool TryDispatch(BaseSlot slot, BaseCustomer customer)
        {
            var worker = GetIdleWorker();
            if (worker == null) return false;

            slot.IsTargeted = true;
            customer.IsTargeted = true;
            worker.OnGenerateOrder(slot, customer, customer.GetStandPoint());
            return true;
        }

        public void AddWorker()
        {
            if (workerPrefab == null || workerParent == null) return;
            if (workers.Count >= 4) return;

            int index = workers.Count;
            Vector3 offscreen = new Vector3(9999f, 0f, 9999f);

            var newWorker = Instantiate(workerPrefab, offscreen, Quaternion.identity, workerParent);
            workers.Add(newWorker);
            InitWorker(newWorker, index);
        }
    }

}