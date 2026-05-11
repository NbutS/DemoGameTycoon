namespace Assembly_CSharp.Assets.Scripts.Manager
{
    using System.Collections.Generic;
    using System.Linq;
    using Assembly_CSharp.Assets.Scripts.Currency;

    public enum ModifierType
    {
        Flat,       // +100
        Percent,    // +10%
        Multiply,   // x2
    }

    public class StatModifier
    {
        public ModifierType Type { get; }
        public double Value { get; }
        public object Source { get; }

        public StatModifier(ModifierType type, double value, object source = null)
        {
            Type = type;
            Value = value;
            Source = source;
        }
    }

    public class StatValue
    {
        private BigNumber _baseValue;
        private List<StatModifier> _modifiers = new();

        public StatValue(BigNumber baseValue)
        {
            _baseValue = baseValue;
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifiersBySource(object source)
        {
            _modifiers.RemoveAll(m => m.Source == source);
        }

        public void ClearModifiers()
        {
            _modifiers.Clear();
        }

        /// <summary>
        /// Tính theo thứ tự: Base + Flat → x(1 + tổng Percent) → x từng Multiply
        /// </summary>
        public BigNumber Calculate()
        {
            BigNumber flat = _modifiers
                .Where(m => m.Type == ModifierType.Flat)
                .Aggregate(BigNumber.Zero, (acc, m) => acc + BigNumber.FromDouble(m.Value));

            BigNumber result = _baseValue + flat;

            double totalPercent = _modifiers
                .Where(m => m.Type == ModifierType.Percent)
                .Sum(m => m.Value);

            result = result * (1.0 + totalPercent);

            foreach (var m in _modifiers.Where(m => m.Type == ModifierType.Multiply))
                result = result * m.Value;

            return result;
        }

        public BigNumber BaseValue => _baseValue;
        public void SetBase(BigNumber v) => _baseValue = v;

        public IReadOnlyList<StatModifier> Modifiers => _modifiers;
    }

}