namespace WhatIfF1.Util.Extensions
{
    public static class StringExtensions
    {
        public static void OpenInBrowser(this string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}
