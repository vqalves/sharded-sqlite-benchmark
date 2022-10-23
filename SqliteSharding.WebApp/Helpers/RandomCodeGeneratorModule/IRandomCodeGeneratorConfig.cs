using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.Helpers.RandomCodeGeneratorModule
{
    public interface IRandomCodeGeneratorConfig
    {
        IEnumerable<char> GetAvailableCodeCharacters();
    }
}