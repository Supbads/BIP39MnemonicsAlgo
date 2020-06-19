using System;
using System.Collections.Generic;
using System.IO;

namespace MnemonicBruteforcer.Algorithm
{
    public class FileLogger : ILogger
    {
        private const string outputFile = "output.txt";
        private string _loggingDirectory = string.Empty;

        public FileLogger()
        {
            var currentDir = Directory.GetCurrentDirectory();
            this._loggingDirectory = $"{currentDir}\\{outputFile}";
        }

        public void Log(string output)
        {
            File.AppendAllText(this._loggingDirectory, $"{output} {Environment.NewLine}");
        }

        public void LogBatch(IEnumerable<string> output)
        {
            File.AppendAllLines(this._loggingDirectory, output);
        }
    }
}