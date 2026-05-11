using System;
using System.Collections.Generic;
using UnityEngine;
using Assembly_CSharp.Assets.Scripts.Customer;
using Assembly_CSharp.Assets.Scripts.Ui.Popup;
using Assembly_CSharp.Assets.Scripts.Currency;
using Assembly_CSharp.Assets.Scripts.Manager;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class SlotController : MonoBehaviour
    {
        [SerializeField] private List<SlotConfig> slotConfigs;
        [SerializeField] private List<BaseSlot> slots;

        public event Action<BaseSlot> OnSlotReady;

        public void OnInit(CurrencyManager currencyManager, PopupManager popupManager)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                slot.Setup(slotConfigs[i], currencyManager, popupManager);
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
                if (!slot.IsReady || slot.IsTargeted) continue;
                if (best == null || slot.Profit > bestProfit)
                {
                    bestProfit = slot.Profit;
                    best = slot;
                }
            }

            return best;
        }

        public BaseSlot GetSlotByIndex(int index) =>
            slots.Find(s => s.Config != null && s.Config.SlotIndex == index);

        public void X2SlotProfit(int index)
        {
            GetSlotByIndex(index)?.ApplyModifier(
                new StatModifier(ModifierType.Multiply, 2.0, $"x2_slot_{index}_{Guid.NewGuid()}")
            );
        }

        public void X2GlobalProfit()
        {
            string source = $"x2_global_{Guid.NewGuid()}";
            foreach (var slot in slots)
                slot.ApplyModifier(new StatModifier(ModifierType.Multiply, 2.0, source));
        }
    }

}