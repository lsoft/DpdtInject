using System;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;

namespace DpdtInject.Generator.Core.Producer.Product
{
    public class PropertyProduct : IWritable
    {
        public string Modifiers
        {
            get;
        }

        public string Type
        {
            get;
        }

        public string Name
        {
            get;
        }

        public string? GetBody
        {
            get;
        }
        
        public string? SetBody
        {
            get;
        }

        public PropertyProduct(
            string modifiers,
            string type,
            string name,
            string? getBody,
            string? setBody
            )
        {
            if (modifiers is null)
            {
                throw new ArgumentNullException(nameof(modifiers));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (getBody is null && setBody is null)
            {
                throw new ArgumentOutOfRangeException("getBody && setBody are both null");
            }


            Modifiers = modifiers;
            Type = type;
            Name = name;
            GetBody = getBody;
            SetBody = setBody;
        }

        public string GetText()
        {
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
        {Modifiers} {Type} {Name}
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