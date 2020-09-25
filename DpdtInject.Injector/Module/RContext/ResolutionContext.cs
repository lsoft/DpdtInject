﻿using DpdtInject.Injector.Reinvented;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Module.RContext
{
//#nullable disable

    public interface ICustomScopeObject
    {
        //bool TryGetObject(
        //    Type requestedType,
        //    [NotNullWhen(true)] out object? result
        //    );
    }

    public abstract class BaseScopeObject : ICustomScopeObject, IDisposable
    {
        private FlexibleSizeObjectContainer _dependencyContainer;

        protected BaseScopeObject()
        {
            _dependencyContainer = new FlexibleSizeObjectContainer(
                1
                );
        }

        //public bool TryGetObject(
        //    Type requestedType,
        //    [NotNullWhen(true)] out object? result
        //    )
        //{
        //    return _dependencyContainer.TryGetObject(requestedType, out result);
        //}

        public object GetOrAdd(
            Guid uniqueId,
            Func<object> objectProvider
            )
        {
            var result = _dependencyContainer.GetOrAdd(uniqueId, objectProvider);

            return result;
        }

        public void Dispose()
        {
            try
            {
                DoDispose();
            }
            finally
            {
                _dependencyContainer.Dispose();
            }
        }

        protected abstract void DoDispose();
    }

    public class ResolutionContext : IResolutionContext
    {
        public static readonly ResolutionContext EmptyContext = new ResolutionContext();
        private readonly List<ResolutionFrame> _frames;

        public IReadOnlyList<IResolutionFrame> Frames => _frames;

        public bool IsRoot => _frames.Count == 1;

        public IResolutionFrame RootFrame => _frames[0];

        public IResolutionFrame ParentFrame => _frames[_frames.Count - 2];

        public IResolutionFrame CurrentFrame => _frames[_frames.Count - 1];

        public ICustomScopeObject? CustomScopeObject => ScopeObject;

        public BaseScopeObject? ScopeObject
        {
            get;
        }

        public ResolutionContext()
        {
            _frames = new List<ResolutionFrame>();
            ScopeObject = null;
        }

        public ResolutionContext(BaseScopeObject scopeObject)
        {
            if (scopeObject is null)
            {
                throw new ArgumentNullException(nameof(scopeObject));
            }

            _frames = new List<ResolutionFrame>();
            ScopeObject = scopeObject;
        }

        private ResolutionContext(
            ResolutionContext parentResolutionContext,
            ResolutionFrame newFrame
            )
        {
            _frames = new List<ResolutionFrame>(parentResolutionContext._frames); //clone this
            _frames.Add(newFrame);
        }

        public ResolutionContext AddFrame(ResolutionFrame newFrame)
        {
            if (newFrame is null)
            {
                throw new ArgumentNullException(nameof(newFrame));
            }

            return new ResolutionContext(
                this,
                newFrame
                );
        }
    }
}
