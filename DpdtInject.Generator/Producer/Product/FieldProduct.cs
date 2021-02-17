using System;

namespace DpdtInject.Generator.Producer.Product
{

    public class FieldProduct
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

        public string DefaultValue
        {
            get;
        }

        public FieldProduct(
            string modifiers,
            string type,
            string name,
            string defaultValue
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

            if (defaultValue is null)
            {
                throw new ArgumentNullException(nameof(defaultValue));
            }

            Modifiers = modifiers;
            Type = type;
            Name = name;
            DefaultValue = defaultValue;
        }

        public string GetText()
        {
            return $"{Modifiers} {Type} {Name} = {DefaultValue};";
        }
    }

}