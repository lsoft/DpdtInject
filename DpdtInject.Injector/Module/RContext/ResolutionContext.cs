using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Module.RContext
{
    public class ResolutionContext : IResolutionContext
    {
        public static readonly ResolutionContext EmptyContext = new ResolutionContext();
        private readonly List<ResolutionFrame> _frames;

        public IReadOnlyList<IResolutionFrame> Frames => _frames;

        public bool IsRoot => _frames.Count == 1;

        public IResolutionFrame RootFrame => _frames[0];

        public IResolutionFrame CurrentFrame => _frames[_frames.Count - 1];

        public ResolutionContext()
        {
            _frames = new List<ResolutionFrame>();
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
