namespace DiegoG.Utilities
{
    public static class MoreMath
    {
        public static double NthRoot(double A, int N)
        {
            return System.Math.Pow(A, 1.0 / N);
        }
        public static ulong GreatestCommonDivisor(ulong a, ulong b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }
            return a == 0 ? b : a;
        }
    }
}
