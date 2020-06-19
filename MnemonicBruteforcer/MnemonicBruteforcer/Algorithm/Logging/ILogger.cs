using System.Collections.Generic;

namespace MnemonicBruteforcer.Algorithm
{
    public interface ILogger
    {
        void Log(string output);
        void LogBatch(IEnumerable<string> output);
    }
}
