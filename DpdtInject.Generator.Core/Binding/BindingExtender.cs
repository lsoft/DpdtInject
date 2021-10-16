using System;

namespace DpdtInject.Generator.Core.Binding
{
    /// <summary>
    /// Binding container extender with additional flags.
    /// </summary>
    public class BindingExtender

    {
        public IBindingContainer BindingContainer
        {
            get;
        }

        /// <summary>
        /// Do this binding needs in resolution context?
        /// </summary>
        public bool NeedToProcessResolutionContext => 
            ItselfOrAnyChildNeedForContext | ItselfOrAnyChildIsConditional | HasUnresolvedChildren;

        /// <summary>
        /// Do this binding or any of its children need in context because of Custom scope?
        /// </summary>
        public bool ItselfOrAnyChildNeedForContext
        {
            get;
            set;
        }

        /// <summary>
        /// Is this binding or any of its children is conditional?
        /// </summary>
        public bool ItselfOrAnyChildIsConditional
        {
            get;
            set;
        }

        /// <summary>
        /// Do this binding or any of its children has a unresolved children?
        /// (if so, we need to push resolution request in other cluster).
        /// </summary>
        public bool HasUnresolvedChildren
        {
            get;
            set;
        }

        public BindingExtender(
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
