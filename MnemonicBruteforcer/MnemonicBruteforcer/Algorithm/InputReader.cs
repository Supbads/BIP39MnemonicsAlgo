using System.IO;

namespace MnemonicBruteforcer.Algorithm
{
    public static class InputReader
    {
        const string fourDirsUp = "\\..\\..\\..\\..\\";

        public static string[] TryReadFileLines(string currentDir, string fileName)
        {
            if (File.Exists($"{currentDir}{fourDirsUp}{fileName}"))
            {
                return File.ReadAllLines($"{currentDir}{fourDirsUp}{fileName}");
            }
            else if (File.Exists($"{currentDir}\\{fileName}"))
            {
                return File.ReadAllLines($"{currentDir}\\{fileName}");
            }

            throw Throw(currentDir, fileName);
        }

        public static string TryReadFileText(string currentDir, string fileName)
        {
            if (File.Exists($"{currentDir}{fourDirsUp}{fileName}"))
            {
                return File.ReadAllText($"{currentDir}{fourDirsUp}{fileName}");
            }
            else if (File.Exists($"{currentDir}\\{fileName}"))
            {
                return File.ReadAllText($"{currentDir}\\{fileName}");
            }

            throw Throw(currentDir, fileName);
        }

        private static FileNotFoundException Throw(string currentDir, string fileName)
        {
            return new FileNotFoundException($"Unable to find file {fileName} in directory: {currentDir}");
        }
    }
}
