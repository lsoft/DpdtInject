using System;
using DpdtInject.Injector.Excp;

namespace DpdtInject.Generator.Core.Producer.Product
{
    public class PropertyProduct
    {
        public string Modifiers
        {
            get;
        }

        public string Type
        {
            get;
        }

        public string? ExplicitType
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

        public PropertyProduct(
            string modifiers,
            string type,
            string? explicitType,
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
            ExplicitType = explicitType;
            Name = name;
            DefaultValue = defaultValue;

            if(!(string.IsNullOrEmpty(explicitType) ^ string.IsNullOrEmpty(modifiers)))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, "Only one must be set");
            }
        }

        public string GetText()
        {
            if (string.IsNullOrEmpty(ExplicitType))
            {
                return $"{Modifiers} {Type} {Name} {{ get; }} = {DefaultValue};";
            }
            else
            {
                return $"{Type} {ExplicitType}.{Name} {{ get; }} = {DefaultValue};";
            }
        }
    }

}