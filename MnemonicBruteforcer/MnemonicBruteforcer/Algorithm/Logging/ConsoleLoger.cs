using System;
using System.Collections.Generic;

namespace MnemonicBruteforcer.Algorithm
{
    class ConsoleLoger : ILogger
    {
        public void Log(string output)
        {
            Console.WriteLine(output);
        }

        public void LogBatch(IEnumerable<string> output)
        {
            foreach (var line in output)
            {
                this.Log(line);
            }
        }
    }
}