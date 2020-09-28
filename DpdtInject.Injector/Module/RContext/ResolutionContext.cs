using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.CustomScope;
using DpdtInject.Injector.Reinvented;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Module.RContext
{
    public class ResolutionContext : IResolutionContext
    {
        private readonly List<ResolutionFrame> _frames;

        public IReadOnlyList<IResolutionFrame> Frames => _frames;

        public bool IsRoot => _frames.Count == 1;

        public IResolutionFrame RootFrame => _frames[0];

        public IResolutionFrame ParentFrame => _frames[_frames.Count - 2];

        public IResolutionFrame CurrentFrame => _frames[_frames.Count - 1];

        public ICustomScopeObject? CustomScopeObject => ScopeObject;

        public CustomScopeObject? ScopeObject
        {
            get;
        }

        public ResolutionContext(
            ResolutionFrame firstFrame
            )
        {
            if (firstFrame is null)
            {
                throw new ArgumentNullException(nameof(firstFrame));
            }

            _frames = new List<ResolutionFrame>
            {
                firstFrame
            };

            ScopeObject = null;
        }

        private ResolutionContext(
            ResolutionContext sourceResolutionContext,
            CustomScopeObject scopeObject
            )
        {
            if (sourceResolutionContext is null)
            {
                throw new ArgumentNullException(nameof(sourceResolutionContext));
            }

            if (scopeObject is null)
            {
                throw new ArgumentNullException(nameof(scopeObject));
            }

            _frames = new List<ResolutionFrame>(sourceResolutionContext._frames); //clone this
            ScopeObject = scopeObject;
        }

        private ResolutionContext(
            ResolutionContext parentResolutionContext,
            ResolutionFrame newFrame
            )
        {
            if (newFrame is null)
            {
                throw new ArgumentNullException(nameof(newFrame));
            }

            _frames = new List<ResolutionFrame>(parentResolutionContext._frames); //clone this
            ScopeObject = parentResolutionContext.ScopeObject;

            _frames.Add(newFrame);
        }

        public ResolutionContext ModifyWith(CustomScopeObject scope)
        {
            return new ResolutionContext(
                this,
                scope
                );
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
