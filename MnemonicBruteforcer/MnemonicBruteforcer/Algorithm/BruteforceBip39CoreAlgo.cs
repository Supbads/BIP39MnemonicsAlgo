using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Bitcoin.BIP39;
using Newtonsoft.Json;

namespace MnemonicBruteforcer.Algorithm
{
    public class BruteforceBip39CoreAlgo
    {
        const string bip39wordsFile = "bip39words.txt";
        const string seedFile = "seed.json";
        const int TotalWordsCount = 12;

        protected static string[] WordsRaw = new string[] { };
        protected static string SeedRaw = string.Empty;
        protected int TotalMatches = 0;
        Dictionary<int, int> seedWordsIndices = new Dictionary<int, int>();
        private readonly InputArguments _inputArguments;
        private readonly ILogger _logger;
        List<string> solutionSeeds = new List<string>(1024);

        public BruteforceBip39CoreAlgo(InputArguments inputArguments, ILogger logger)
        {
            this._inputArguments = inputArguments;
            this._logger = logger;
        }

        public void Start()
        {
            this._logger.Log($"Starting bruteforce {DateTime.Now.ToLongTimeString()}");

            var bip39 = new BIP39();
            ReadBip39WordsList();

            //// Fill words maps
            var wordsRes = ReadWordsMap();
            var wordsByIndex = wordsRes.Item1;
            var indexByWords = wordsRes.Item2;

            //// Input Current Words either from seed or through input
            Stopwatch sw = null;
            foreach (var currentWords in ReadCurrentWords())
            {
                sw = Stopwatch.StartNew();
                int currentWordsLength = currentWords.Length;
                int wordsToFillCount = TotalWordsCount - currentWordsLength;

                var allWordsIndices = new List<int>(TotalWordsCount);
                for (int i = 0; i < currentWordsLength; i++)
                {
                    if (!indexByWords.ContainsKey(currentWords[i]))
                    {
                        throw new Exception($"The provided word: {currentWords[i]} was not present within the original mnemonic words");
                    }

                    var currentWordIndex = indexByWords[currentWords[i]];
                    allWordsIndices.Add(currentWordIndex);
                }
                
                int[] lookupIndices = GetLookupIndices();
                foreach (var indicesCombination in CombinationsWithRepetition(lookupIndices, wordsToFillCount))
                {
                    foreach (var index in indicesCombination)
                    {
                        allWordsIndices.Add(index);
                    }

                    var res = bip39.CheckSum(allWordsIndices);

                    if (res)
                    {
                        TotalMatches++;
                        OnMatch(wordsByIndex, wordsToFillCount, allWordsIndices);
                    }

                    allWordsIndices.RemoveRange(TotalWordsCount - wordsToFillCount, wordsToFillCount);
                }

                // Log here
                if (this._inputArguments.ShouldLog && this.solutionSeeds.Any())
                {
                    this._logger.LogBatch(this.solutionSeeds);
                    this.solutionSeeds.Clear();
                }
            }

            sw.Stop();
            this._logger.Log($"Algo Done Elapsed={sw.Elapsed}");
            this._logger.Log($"total matches {TotalMatches}");
        }

        private IEnumerable<string[]> ReadCurrentWords()
        {
            if (this._inputArguments.UsingFileSeed)
            {
                var seedWords = ReadSeedWords();
                InitSeedWordsIndices();
                int totalcombinations = GetTotalCombinationsFromSeed(seedWords);

                List<string> currentWords = new List<string>(TotalWordsCount);
                for (int i = 0; i < totalcombinations; i++)
                {
                    for (int j = 0; j < TotalWordsCount; j++)
                    {
                        var currentWordIndex = Math.Min(seedWordsIndices[j], seedWords.Words[j].Length - 1);
                        var word = seedWords.Words[j][currentWordIndex];
                        if (!string.IsNullOrWhiteSpace(word))
                            currentWords.Add(word);
                    }

                    yield return currentWords.ToArray();

                    //try decrement last valid index
                    DecrementLastValidSeedIndex(seedWords);

                    currentWords.Clear();
                }
            }
            else
            {
                var currentWordsInput = Console.ReadLine().Split(' ');
                yield return currentWordsInput;
            }
        }

        private void DecrementLastValidSeedIndex(KnownWordsSeed seedWords)
        {
            for (int j = TotalWordsCount - 1; j >= 0; j--)
            {
                var currentWordIndex = seedWordsIndices[j];
                if (seedWordsIndices[j] + 1 >= seedWords.Words[j].Length)
                {
                    continue;
                }

                var word = seedWords.Words[j][currentWordIndex + 1];
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }

                seedWordsIndices[j] = ++seedWordsIndices[j];
                for (int k = j + 1; k < TotalWordsCount; k++)
                {
                    seedWordsIndices[k] = 0;
                }

                break;
            }
        }

        private void InitSeedWordsIndices()
        {
            for (int i = 0; i < TotalWordsCount; i++)
            {
                seedWordsIndices.Add(i, 0);
            }
        }

        private static int GetTotalCombinationsFromSeed(KnownWordsSeed seedWords)
        {
            return seedWords.Words.Aggregate(1, (totalCount, words) =>
            {
                var count = words.Count(x => !string.IsNullOrWhiteSpace(x));
                if (count == 0)
                    return totalCount;

                return totalCount * count;
            });
        }

        protected (Dictionary<int, string>, Dictionary<string, int>) ReadWordsMap()
        {
            var wordsByIndex = new Dictionary<int, string>();
            var indexByWords = new Dictionary<string, int>();
            for (int i = 0; i < WordsRaw.Length; i++)
            {
                var word = WordsRaw[i];
                wordsByIndex.Add(i, word);
                indexByWords.Add(word, i);
            }

            return (wordsByIndex, indexByWords);
        }

        protected IEnumerable<List<int>> CombinationsWithRepetition(IEnumerable<int> input, int length)
        {
            if (length <= 0)
                yield return new List<int>();
            else
            {
                foreach (var i in input)
                    foreach (var list in CombinationsWithRepetition(input, length - 1))
                    {
                        list.Add(i);
                        yield return list;
                    }
            }
        }

        protected virtual int[] GetLookupIndices()
        {
            if (_inputArguments.IsReversed)
            {
                return Enumerable.Range(0, 2048).Reverse().ToArray();
            }

            return Enumerable.Range(0, 2048).ToArray();
        }

        protected virtual void OnMatch(Dictionary<int, string> wordsByIndex, int wordsToFillCount, List<int> allWordsIndices)
        {
            if (_inputArguments.ShouldLog)
            {
                string currentSeed = "";
                for (int i = 0; i < TotalWordsCount; i++)
                {
                    currentSeed += $"{wordsByIndex[allWordsIndices[i]]} ";
                }

                this.solutionSeeds.Add(currentSeed.Trim());
            }
        }

        private static void ReadBip39WordsList()
        {
            var currentDir = Directory.GetCurrentDirectory();
            WordsRaw = InputReader.TryReadFileLines(currentDir, bip39wordsFile);
        }

        private static KnownWordsSeed ReadSeedWords()
        {
            var currentDir = Directory.GetCurrentDirectory();
            SeedRaw = InputReader.TryReadFileText(currentDir, seedFile);
            return JsonConvert.DeserializeObject<KnownWordsSeed>(SeedRaw);
        }
    }
}

