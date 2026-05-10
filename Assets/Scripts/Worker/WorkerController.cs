using System.Collections.Generic;
using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Slot;
using Assembly_CSharp.Assets.Scripts.Manager;
using System;

namespace Scripts.Worker
{
    public class WorkerController : MonoBehaviour
    {
        [SerializeField] private List<BaseWorker> workers;
        [SerializeField] private BaseWorker workerPrefab;
        [SerializeField] private Transform workerParent;

        private CounterSlotManager _counterSlotManager;
        public event Action<BaseWorker> OnWorkerIdle;

        public void OnInit(CounterSlotManager counterSlotManager)
        {
            _counterSlotManager = counterSlotManager;
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
            var homePos = _counterSlotManager.GetWorkerHomePoint(index);
            worker.WarpTo(homePos);
            worker.OnInit(_counterSlotManager, homePos);
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

            InitWorker(newWorker, index);
            workers.Add(newWorker);
        }
    }
}