using System;

namespace Hanna
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot Hanna = new Bot();
            Hanna.RunAsyn().GetAwaiter().ConfigAwait(false);
        }
    }
}
