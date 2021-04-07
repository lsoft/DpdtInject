using System;
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

}