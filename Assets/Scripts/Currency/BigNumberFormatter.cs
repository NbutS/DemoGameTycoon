namespace Assembly_CSharp.Assets.Scripts.Currency
{
    public static class BigNumberFormatter
    {
        private static readonly (int exponent, string suffix)[] Suffixes =
        {
        (15, "Q"),
        (12, "T"),
        (9,  "B"),
        (6,  "M"),
        (3,  "k"),
    };

        public static string Format(BigNumber number)
        {
            var (mantissa, exponent) = number.Raw;

            if (exponent < 3)
            {
                double val = mantissa;
                for (int i = 0; i < exponent; i++) val *= 10.0;
                return $"{val:0}";
            }

            foreach (var (exp, suffix) in Suffixes)
            {
                if (exponent >= exp)
                {
                    double val = mantissa;
                    int diff = exponent - exp;
                    for (int i = 0; i < diff; i++) val *= 10.0;
                    return $"{val:0.#}{suffix}";
                }
            }

            return $"{mantissa:0.##}e{exponent}";
        }
    }

}