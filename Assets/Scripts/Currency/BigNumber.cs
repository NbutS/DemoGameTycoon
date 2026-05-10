using System;

namespace Assembly_CSharp.Assets.Scripts.Currency
{
    [Serializable]
    public struct BigNumber
    {
        private double _mantissa;   
        private int _exponent;  

        public static readonly BigNumber Zero = new BigNumber(0, 0);

        public BigNumber(double mantissa, int exponent)
        {
            _mantissa = mantissa;
            _exponent = exponent;
            Normalize();
        }

        // Tạo từ double thông thường
        public static BigNumber FromDouble(double value)
        {
            if (value == 0) return Zero;
            int exp = (int)Math.Floor(Math.Log10(Math.Abs(value)));
            return new BigNumber(value / Math.Pow(10, exp), exp);
        }

        private void Normalize()
        {
            if (_mantissa == 0) { _exponent = 0; return; }

            while (Math.Abs(_mantissa) >= 10)
            {
                _mantissa /= 10;
                _exponent++;
            }
            while (Math.Abs(_mantissa) < 1 && _mantissa != 0)
            {
                _mantissa *= 10;
                _exponent--;
            }
        }

        // ─── Operators ────────────────────────────────────────────────────────────
        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            if (a._exponent > b._exponent)
            {
                double bAdj = b._mantissa * Math.Pow(10, b._exponent - a._exponent);
                return new BigNumber(a._mantissa + bAdj, a._exponent);
            }
            else
            {
                double aAdj = a._mantissa * Math.Pow(10, a._exponent - b._exponent);
                return new BigNumber(aAdj + b._mantissa, b._exponent);
            }
        }

        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            if (a._exponent > b._exponent)
            {
                double bAdj = b._mantissa * Math.Pow(10, b._exponent - a._exponent);
                return new BigNumber(a._mantissa - bAdj, a._exponent);
            }
            else
            {
                double aAdj = a._mantissa * Math.Pow(10, a._exponent - b._exponent);
                return new BigNumber(aAdj - b._mantissa, b._exponent);
            }
        }

        public static BigNumber operator *(BigNumber a, double multiplier)
        {
            return new BigNumber(a._mantissa * multiplier, a._exponent);
        }

        public static BigNumber operator *(BigNumber a, BigNumber b)
        {
            return new BigNumber(a._mantissa * b._mantissa, a._exponent + b._exponent);
        }

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

        public static implicit operator BigNumber(double value) => FromDouble(value);
        public static implicit operator BigNumber(int value) => FromDouble(value);

        // ─── Format ───────────────────────────────────────────────────────────────
        public override string ToString()
        {
            if (_exponent < 3) return $"{_mantissa * Math.Pow(10, _exponent):0}";
            if (_exponent < 6) return $"{_mantissa * Math.Pow(10, _exponent - 3):0.#}k";
            if (_exponent < 9) return $"{_mantissa * Math.Pow(10, _exponent - 6):0.#}M";
            if (_exponent < 12) return $"{_mantissa * Math.Pow(10, _exponent - 9):0.#}B";
            if (_exponent < 15) return $"{_mantissa * Math.Pow(10, _exponent - 12):0.#}T";

            // Số rất lớn dùng ký hiệu khoa học: 1.23e15
            return $"{_mantissa:0.##}e{_exponent}";
        }

        public double ToDouble() => _mantissa * Math.Pow(10, _exponent);
    }
}