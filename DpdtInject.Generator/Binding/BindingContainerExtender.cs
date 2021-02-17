using System;

namespace DpdtInject.Generator.Binding
{
    public class BindingContainerExtender
    {
        public IBindingContainer BindingContainer
        {
            get;
        }

        public bool NeedToProcessResolutionContext => 
            ItselfOrAnyChildNeedForContext | ItselfOrAnyChildIsConditional | HasUnresolvedChildren;

        public bool ItselfOrAnyChildNeedForContext
        {
            get;
            set;
        }

        public bool ItselfOrAnyChildIsConditional
        {
            get;
            set;
        }

        public bool HasUnresolvedChildren
        {
            get;
            set;
        }

        public BindingContainerExtender(
            IBindingContainer bindingContainer
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            BindingContainer = bindingContainer;
        }

    }

}
