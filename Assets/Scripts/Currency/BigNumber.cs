using System;

namespace Assembly_CSharp.Assets.Scripts.Currency
{
    [Serializable]
    public struct BigNumber
    {
        private double _mantissa;
        private int _exponent;

        public static readonly BigNumber Zero = new BigNumber(0, 0);
        public static readonly BigNumber One = new BigNumber(1, 0);

        public BigNumber(double mantissa, int exponent)
        {
            _mantissa = mantissa;
            _exponent = exponent;
            Normalize();
        }

        public static BigNumber FromDouble(double value)
        {
            if (value == 0) return Zero;
            if (double.IsInfinity(value) || double.IsNaN(value)) return Zero;

            int exp = (int)Math.Floor(Math.Log10(Math.Abs(value)));
            double mantissa = value;

            if (exp > 0)
                for (int i = 0; i < exp; i++) mantissa /= 10.0;
            else
                for (int i = 0; i < -exp; i++) mantissa *= 10.0;

            return new BigNumber(mantissa, exp);
        }

        private void Normalize()
        {
            if (_mantissa == 0) { _exponent = 0; return; }

            while (Math.Abs(_mantissa) >= 10.0)
            {
                _mantissa /= 10.0;
                _exponent++;
            }
            while (Math.Abs(_mantissa) < 1.0 && _mantissa != 0)
            {
                _mantissa *= 10.0;
                _exponent--;
            }
        }

        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            if (a._exponent >= b._exponent)
            {
                int diff = a._exponent - b._exponent;
                double bAdj = b._mantissa;
                for (int i = 0; i < diff; i++) bAdj /= 10.0;
                return new BigNumber(a._mantissa + bAdj, a._exponent);
            }
            else
            {
                int diff = b._exponent - a._exponent;
                double aAdj = a._mantissa;
                for (int i = 0; i < diff; i++) aAdj /= 10.0;
                return new BigNumber(aAdj + b._mantissa, b._exponent);
            }
        }

        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            if (a._exponent >= b._exponent)
            {
                int diff = a._exponent - b._exponent;
                double bAdj = b._mantissa;
                for (int i = 0; i < diff; i++) bAdj /= 10.0;
                return new BigNumber(a._mantissa - bAdj, a._exponent);
            }
            else
            {
                int diff = b._exponent - a._exponent;
                double aAdj = a._mantissa;
                for (int i = 0; i < diff; i++) aAdj /= 10.0;
                return new BigNumber(aAdj - b._mantissa, b._exponent);
            }
        }

        public static BigNumber operator *(BigNumber a, BigNumber b)
            => new BigNumber(a._mantissa * b._mantissa, a._exponent + b._exponent);

        public static BigNumber operator *(BigNumber a, double multiplier)
            => new BigNumber(a._mantissa * multiplier, a._exponent);

        public static BigNumber operator *(double multiplier, BigNumber a)
            => a * multiplier;

        public static BigNumber operator /(BigNumber a, double divisor)
            => new BigNumber(a._mantissa / divisor, a._exponent);

        public static bool operator >=(BigNumber a, BigNumber b)
        {
            if (a._exponent != b._exponent) return a._exponent > b._exponent;
            return a._mantissa >= b._mantissa;
        }

        public static bool operator <=(BigNumber a, BigNumber b) => b >= a;

        public static bool operator >(BigNumber a, BigNumber b)
        {
            if (a._exponent != b._exponent) return a._exponent > b._exponent;
            return a._mantissa > b._mantissa;
        }

        public static bool operator <(BigNumber a, BigNumber b) => b > a;

        public static bool operator ==(BigNumber a, BigNumber b)
            => a._exponent == b._exponent && Math.Abs(a._mantissa - b._mantissa) < 1e-9;

        public static bool operator !=(BigNumber a, BigNumber b) => !(a == b);

        public override bool Equals(object obj) => obj is BigNumber b && this == b;
        public override int GetHashCode() => HashCode.Combine(_mantissa, _exponent);

        public static implicit operator BigNumber(double v) => FromDouble(v);
        public static implicit operator BigNumber(int v) => FromDouble(v);

        public override string ToString() => BigNumberFormatter.Format(this);

        public (double mantissa, int exponent) Raw => (_mantissa, _exponent);
    }
}