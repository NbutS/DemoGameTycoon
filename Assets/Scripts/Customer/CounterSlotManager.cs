using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Customer
{
    public class CounterSlotManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> customerPoints;
        [SerializeField] private List<Transform> workerPoints;
        [SerializeField] private List<Transform> customerSpawnPoints;
        [SerializeField] List<Transform> customerEndPoints;
        [SerializeField] private List<Transform> workerHomePoints;
        [SerializeField] private Transform customerEndPointDefault;

        private Dictionary<int, BaseCustomer> _occupiedSlots = new();

        public void OnInit()
        {
            _occupiedSlots.Clear();
            for (int i = 0; i < customerPoints.Count; i++)
                _occupiedSlots[i] = null;
        }

        public Transform GetAvailableCustomerPoint(BaseCustomer customer, out int index)
        {
            for (int i = 0; i < customerPoints.Count; i++)
            {
                if (_occupiedSlots[i] == null)
                {
                    _occupiedSlots[i] = customer;
                    index = i;
                    return customerPoints[i];
                }
            }
            index = -1;
            return null;
        }

        public Transform GetWorkerPointByIndex(int index)
        {
            if (index < 0 || index >= workerPoints.Count) return null;
            return workerPoints[index];
        }

        public void ReleasePoint(BaseCustomer customer)
        {
            foreach (var key in new List<int>(_occupiedSlots.Keys))
            {
                if (_occupiedSlots[key] == customer)
                {
                    _occupiedSlots[key] = null;
                    return;
                }
            }
        }

        public bool HasAvailablePoint() =>
            new List<int>(_occupiedSlots.Keys).Exists(k => _occupiedSlots[k] == null);

        // ─── Spawn / Home points theo index ───────────────────────────────────
        public Vector3 GetCustomerSpawnPoint(int index = 0)
        {
            if (index < customerSpawnPoints.Count)
                return customerSpawnPoints[index].position;
            return customerSpawnPoints[0].position;
        }

        public Vector3 GetCustomerEndPoint(int index = 0)
        {
            if (index < customerEndPoints.Count)
                return customerEndPoints[index].position;
            return customerEndPoints[0].position;
        }

        public Vector3 GetWorkerHomePoint(int index = 0)
        {
            if (index < workerHomePoints.Count)
                return workerHomePoints[index].position;
            return workerHomePoints[0].position;
        }

        public int GetCustomerSpawnCount() => customerSpawnPoints.Count;
        public int GetWorkerHomeCount() => workerHomePoints.Count;
    }
}
