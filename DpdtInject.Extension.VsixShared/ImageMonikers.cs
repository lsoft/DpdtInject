using Microsoft.VisualStudio.Imaging.Interop;
using System;

namespace DpdtInject.Extension
{
    public static class ImageMonikers
    {
        public static ImageMoniker Logo
        {
            get;
        } = new ImageMoniker
            {
                Guid = new Guid("bbd8a64b-7fd0-47fb-a600-503d90f22239"),
                Id = 0
            };


    }
}
