using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.Helpers.RandomCodeGeneratorModule
{
    public class RandomCodeGenerator
    {
        private readonly char[] Letters;
        private readonly Random Randomizer;

        public RandomCodeGenerator(IRandomCodeGeneratorConfig configuration)
        {
            this.Letters = configuration.GetAvailableCodeCharacters().ToArray();
            this.Randomizer = new Random();
        }

        public char GetRandomLetter()
        {
            var index = Randomizer.Next(Letters.Length);
            return Letters[index];
        }

        public string GetRandomCode(int count)
        {
            var letters = Enumerable.Range(0, count).Select(x => GetRandomLetter());
            return string.Join(string.Empty, letters);
        }
    }
}