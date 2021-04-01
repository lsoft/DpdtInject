using System;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Injector;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer.Product
{
    public class EventProduct : IWritable
    {
        public string Modifiers
        {
            get;
        }
        public IEventSymbol Event
        {
            get;
        }
        public string Type => Event.Type.ToGlobalDisplayString();

        public string Name => Event.Name;

        public string? AddBody
        {
            get;
        }

        public string? RemoveBody
        {
            get;
        }

        public EventProduct(
            string modifiers,
            IEventSymbol @event,
            string? addBody,
            string? removeBody
            )
        {
            if (modifiers is null)
            {
                throw new ArgumentNullException(nameof(modifiers));
            }

            if (addBody is null && removeBody is null)
            {
                throw new ArgumentOutOfRangeException("addBody && removeBody are both null");
            }


            Modifiers = modifiers;
            Event = @event;
            AddBody = addBody;
            RemoveBody = removeBody;
        }

        public string GetText()
        {
            var add = string.Empty;
            if (AddBody != null)
            {
                add = $@"
            add
            {{
                {AddBody}
            }}
";
            }
            var remove = string.Empty;
            if (RemoveBody != null)
            {
                remove = $@"
            remove
            {{
                {RemoveBody}
            }}
";
            }

            return $@"
        {Modifiers} event {Type} {Event.Name}
        {{
            {add}
            {remove}
        }}
";
        }

        public void Write(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.WriteLine2(GetText());
        }
    }

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