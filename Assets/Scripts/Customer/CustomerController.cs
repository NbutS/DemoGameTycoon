using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Customer
{
    public class CustomerController : MonoBehaviour
    {
        [SerializeField] private List<BaseCustomer> customers;
        [SerializeField] private CounterSlotManager counterSlotManager;
        [SerializeField] private BaseCustomer customerPrefab;
        [SerializeField] private Transform customerParent;

        public event Action<BaseCustomer> OnCustomerWaiting;

        public void OnInit()
        {
            counterSlotManager.OnInit();
            for (int i = 0; i < customers.Count; i++)
                InitCustomer(customers[i], i);
        }

        public void OnUpdate()
        {
            foreach (var customer in customers)
                customer.OnUpdate();
        }

        private void InitCustomer(BaseCustomer customer, int index)
        {
            Vector3 spawnPos = counterSlotManager.GetCustomerSpawnPoint(index);
            Vector3 endPos = counterSlotManager.GetCustomerEndPoint(index);

            customer.WarpTo(spawnPos);
            customer.OnInit(counterSlotManager, spawnPos, endPos);
            customer.OnBecameWaiting += OnCustomerBecameWaiting;
        }

        private void OnCustomerBecameWaiting(BaseCustomer customer)
        {
            OnCustomerWaiting?.Invoke(customer);
        }

        public BaseCustomer GetWaitingCustomer() =>
            customers.Find(c => c.State == CustomerStateType.Waiting && !c.IsTargeted);

        public void SendAllIdleToCounter()
        {
            foreach (var customer in customers)
                customer.MoveToCounter();
        }

        public void AddCustomer()
        {
            if (customerPrefab == null || customerParent == null) return;
            if (customers.Count >= 4) return;

            int index = customers.Count;

            Vector3 offscreen = new Vector3(9999f, 0f, 9999f);
            var newCustomer = Instantiate(customerPrefab, offscreen, Quaternion.identity, customerParent);

            InitCustomer(newCustomer, index);
            customers.Add(newCustomer);
            newCustomer.MoveToCounter();
        }

        public CounterSlotManager GetCounterSlotManager() => counterSlotManager;
    }
}