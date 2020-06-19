using MnemonicBruteforcer.Algorithm;

namespace MnemonicBruteforcer
{
    class Program
    {
        static void Main()
        {
            var inputArgs = new InputArguments().ReadInputArguments();
            ILogger logger = SetupLogger(inputArgs);

            var bip39BruteForce = new BruteforceBip39CoreAlgo(inputArgs, logger);
            bip39BruteForce.Start();
        }

        private static ILogger SetupLogger(InputArguments inputArgs)
        {
            ILogger logger;
            if (inputArgs.FileLogging)
            {
                logger = new FileLogger();
            }
            else
            {
                logger = new ConsoleLoger();
            }

            return logger;
        }
    }
}