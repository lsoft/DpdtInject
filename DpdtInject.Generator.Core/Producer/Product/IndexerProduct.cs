using System;
using DpdtInject.Generator.Core.Helpers;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer.Product
{

    public class IndexerProduct : IWritable
    {
        public string Modifiers
        {
            get;
        }
        public IPropertySymbol Property
        {
            get;
        }
        public string Type => Property.Type.ToGlobalDisplayString();

        public string Name => Property.Name;

        public string? GetBody
        {
            get;
        }

        public string? SetBody
        {
            get;
        }

        public IndexerProduct(
            string modifiers,
            IPropertySymbol property,
            string? getBody,
            string? setBody
            )
        {
            if (modifiers is null)
            {
                throw new ArgumentNullException(nameof(modifiers));
            }

            if (getBody is null && setBody is null)
            {
                throw new ArgumentOutOfRangeException("getBody && setBody are both null");
            }


            Modifiers = modifiers;
            Property = property;
            GetBody = getBody;
            SetBody = setBody;
        }

        public string GetText()
        {
            var parameters = Property.GetJoinedParametersNameAndType();

            var get = string.Empty;
            if (GetBody != null)
            {
                get = $@"
            get
            {{
                {GetBody}
            }}
";
            }
            var set = string.Empty;
            if (SetBody != null)
            {
                set = $@"
            set
            {{
                {SetBody}
            }}
";
            }

            return $@"
        {Modifiers} {Type} this[{parameters}]
        {{
            {get}
            {set}
        }}
";
        }

        public void Write(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.WriteLine2(GetText());
        }
    }

}