using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Injector.Tree
{
    public class TreeJoint<T>
    {
        private readonly List<TreeJoint<T>> _children;

        public T JointPayload
        {
            get;
        }

        public TreeJoint<T>? Parent
        {
            get;
        }

        public List<TreeJoint<T>> Children => _children;

        public bool IsRoot => Parent is null;

        public bool IsEndJoint => _children.Count == 0;

        public TreeJoint(
            TreeJoint<T>? parent,
            T jointPayload
            )
        {
            if (jointPayload is null)
            {
                throw new ArgumentNullException(nameof(jointPayload));
            }

            Parent = parent;
            JointPayload = jointPayload;
            _children = new List<TreeJoint<T>>();
        }

        public bool TryGetParent<TParent>(
            [NotNullWhen(true)] out TParent? parent
            )
            where TParent : TreeJoint<T>
        {
            parent = (TParent?)Parent;
            return !(parent is null);
        }

        public void AddChild(
            TreeJoint<T> childJoint
            )
        {
            if (childJoint is null)
            {
                throw new ArgumentNullException(nameof(childJoint));
            }

            _children.Add(childJoint);
        }

        public T3 ConvertTo2<T3, T2>(
            Func<T3?, TreeJoint<T>, T3> converter
            )
            where T3 : notnull, TreeJoint<T2>
        {
            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (!IsRoot)
            {
                throw new InvalidOperationException();
            }

            var result = ConvertTo2<T3, T2>(
                null,
                converter
                );

            return result;
        }

        private T3 ConvertTo2<T3, T2>(
            T3? convertedParent,
            Func<T3?, TreeJoint<T>, T3> converter
            )
            where T3 : notnull, TreeJoint<T2>
        {
            var converted = converter(convertedParent, this);

            foreach (var child in _children)
            {
                var convertedChild = child.ConvertTo2<T3, T2>(
                    converted,
                    converter
                    );
                converted.AddChild(convertedChild);
            }

            return converted;
        }


        internal void Apply(
            Action<TreeJoint<T>> action
            )
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(this);

            foreach (var child in Children)
            {
                child.Apply(action);
            }
        }

        internal void Apply(
            Action<T> action
            )
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(JointPayload);

            foreach (var child in Children)
            {
                child.Apply(action);
            }
        }

        public bool TryFindInThisAndItsParents(
            Func<T, bool> predicate,
            [NotNullWhen(true)] out TreeJoint<T>? foundJoint
            )
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (predicate(JointPayload))
            {
                foundJoint = this;
                return true;
            }

            return TryFindInItsParents(predicate, out foundJoint);
        }

        public bool TryFindInItsParents(
            Func<T, bool> predicate,
            [NotNullWhen(true)] out TreeJoint<T>? foundJoint
            )
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (Parent is null)
            {
                foundJoint = null;
                return false;
            }

            return Parent.TryFindInThisAndItsParents(predicate, out foundJoint);
        }
    }
}
