using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.DatabaseStrategies.Sharding
{
    public class ShardingHashCalculator
    {
        private readonly Dictionary<char, int> LetterDictionary;

        public ShardingHashCalculator(IShardingHashConfig configuration)
        {
            this.LetterDictionary = new Dictionary<char, int>();

            var chars = configuration.GetAvailableCodeCharacters().ToArray();
            for(var i = 0; i < chars.Length; i++)
                LetterDictionary.Add(chars[i], i);
        }

        private int GetLetterIndex(char letter) => LetterDictionary.GetValueOrDefault(letter);

        private int GetModulo(int value, int modulo) => (value >= modulo) ? value % modulo : value;
        private int GetModulo(double value, int modulo) => GetModulo((int)value, modulo);

        public int CalculateHash(string key, int modulo)
        {
            int index = 0;
            var processedLetters = 5;
            var letterLimit = Math.Max(key.Length - processedLetters, 0);

            // Run from end to beginning
            for(var i = key.Length; i > letterLimit; i--)
            {
                var power = key.Length - i;
                var letter = key[i - 1];

                var letterHash = GetModulo(GetLetterIndex(letter), modulo);
                var charHash = GetModulo(Math.Pow(letterHash, power), modulo);
                index = GetModulo(index + charHash, modulo);
            }

            return index;
        }
    }
}