namespace WhatIfF1.Util.Extensions
{
    public static class NumberExtensions
    {
        public static double InterpolateLinear(this double x, double x0, double x1, double y0, double y1)
        {
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}
