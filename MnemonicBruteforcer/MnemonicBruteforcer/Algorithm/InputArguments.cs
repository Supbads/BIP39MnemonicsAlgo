using System;

namespace MnemonicBruteforcer.Algorithm
{
    public class InputArguments
    {
        public bool IsReversed { get; set; }

        public bool ShouldLog { get; set; }

        public bool UsingFileSeed { get; set; }

        public bool FileLogging { get; set; }

        public InputArguments ReadInputArguments()
        {
            Console.WriteLine("Use seed.json file? (y/n) ");
            if (this.IsYesInput(Console.ReadLine().ToLower()))
            {
                this.UsingFileSeed = true;
            }

            Console.WriteLine("Run in reverse? (y/n) ");
            if (this.IsYesInput(Console.ReadLine().ToLower()))
                this.IsReversed = true;

            Console.WriteLine("Enable Logging? (y/n)");
            if (this.IsYesInput(Console.ReadLine().ToLower()))
                this.ShouldLog = true;

            if (this.ShouldLog)
            {
                Console.WriteLine("Log to File? (y/n)");
                if (this.IsYesInput(Console.ReadLine().ToLower()))
                {
                    this.FileLogging = true;
                }
            }

            return this;
        }

        private bool IsYesInput(string input)
        {
            return input == "y" || input == "yes";
        }
    }
}