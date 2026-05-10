using System;
using System.Collections.Generic;
using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Ui.Popup;
using Assembly_CSharp.Assets.Scripts.Currency;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class SlotController : MonoBehaviour
    {
        [SerializeField] private List<SlotConfig> slotConfigs;
        [SerializeField] private List<BaseSlot> slots;

        public event Action<BaseSlot> OnSlotReady;

        public void OnInit()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                slot.Setup(slotConfigs[i]);
                slot.OnInit();
                slot.OnBecameReady += () => OnSlotReady?.Invoke(slot);
            }
        }

        public void OnUpdate()
        {
            foreach (var slot in slots)
                slot.OnUpdate();
        }

        public BaseSlot GetAvailableSlot()
        {
            BaseSlot best = null;
            BigNumber bestProfit = BigNumber.Zero;

            foreach (var slot in slots)
            {
                if (slot.GetSlotState() != SlotStateType.Ready) continue;
                if (slot.IsTargeted) continue;

                if (best == null || slot.Profit > bestProfit)
                {
                    bestProfit = slot.Profit;
                    best = slot;
                }
            }

            return best;
        }
        public void X2GlobalProfit()
        {
            foreach (var slot in slots)
                slot.X2Profit();
        }

        public BaseSlot GetSlotByIndex(int index) =>
            slots.Find(s => s.Config.slotIndex == index);

        public void X2SlotProfit(int index) =>
            GetSlotByIndex(index)?.X2Profit();
    }
}