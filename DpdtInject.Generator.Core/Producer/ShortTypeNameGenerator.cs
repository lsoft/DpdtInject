using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Text;
using DpdtInject.Generator.Core.Helpers;

namespace DpdtInject.Generator.Core.Producer
{
    public class ShortTypeNameGenerator
    {
        private readonly Dictionary<string, string> _nameDict = new Dictionary<string, string>();
        public ShortTypeNameGenerator()
        {

        }

        public string GetShortName(ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var key = type.ToGlobalDisplayString();

            if (!key.Contains('.'))
            {
                //it's string, int, long etc. 
                //no need to put it into the dict
                return key;
            }

            if (_nameDict.TryGetValue(key, out var shortName))
            {
                return shortName;
            }

            while (true)
            {

                var newShortName = type.GetSpecialName() + "_" + key.GetStringSha256Hash().SafeSubstring(0, 8);

                if (_nameDict.ContainsKey(newShortName))
                {
                    continue;
                }

                _nameDict[key] = newShortName;

                return newShortName;
            }
        }

        public string GetCombinedUsings(
            )
        {
            var sb = new StringBuilder();

            foreach (var pair in _nameDict)
            {
                sb.AppendLine($"using {pair.Value} = {pair.Key};");
            }

            return sb.ToString();
        }

        public void WriteUsings(
            HashSet<string> set
            )
        {
            foreach (var pair in _nameDict)
            {
                set.Add($"using {pair.Value} = {pair.Key};");
            }
        }

    }
}
