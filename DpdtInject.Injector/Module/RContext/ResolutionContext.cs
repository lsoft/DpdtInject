using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Module.RContext
{
    public class ResolutionFrame
    {
    }


    public class ResolutionContext
    {
        public static readonly ResolutionContext EmptyContext = new ResolutionContext();

        public IReadOnlyList<ResolutionFrame> Frames
        {
            get;
        }

        public ResolutionContext()
        {
            Frames = new List<ResolutionFrame>();
        }

        public ResolutionContext AddFrame(ResolutionFrame newFrame)
        {
            throw new NotImplementedException();
        }
    }
}
