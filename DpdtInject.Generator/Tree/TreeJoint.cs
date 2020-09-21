using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Generator.Tree
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

        private TreeJoint(
            TreeJoint<T> parent,
            T jointPayload
            )
        {
            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            if (jointPayload is null)
            {
                throw new ArgumentNullException(nameof(jointPayload));
            }


            Parent = parent;
            JointPayload = jointPayload;
            _children = new List<TreeJoint<T>>();
        }

        public TreeJoint(
            T jointPayload
            )
        {
            if (jointPayload is null)
            {
                throw new ArgumentNullException(nameof(jointPayload));
            }

            JointPayload = jointPayload;
            Parent = null;
            _children = new List<TreeJoint<T>>();
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

        public T3 ConvertTo<T3, T2>(
            Func<TreeJoint<T>, T3> converter
            )
            where T3 : TreeJoint<T2>
        {
            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            var convertedRoot = converter(this);
            foreach (var child in _children)
            {
                var convertedChild = converter(child);
                convertedRoot.AddChild(convertedChild);
            }

            return convertedRoot;
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

            foreach (var child in this.Children)
            {
                Apply(action);
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

            action(this.JointPayload);

            foreach(var child in this.Children)
            {
                Apply(action);
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
