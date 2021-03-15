using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector
{
    public class IndentedTextWriter2
    {
        private readonly IndentedTextWriter _writer;

        public IndentedTextWriter2(IndentedTextWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            _writer = writer;
        }

        public void Write(string s) => _writer.Write(s);

        public void WriteLine() => _writer.WriteLine();

        public void WriteLine(string s) => _writer.WriteLine(s);

        public void WriteLine2(string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var index = 0;
            while(true)
            {
                var newIndex = s.IndexOf('.', index);
                if (newIndex < 0)
                {
                    //last piece
                    _writer.WriteLine(s.Substring(index + 1));
                    return;
                }

                _writer.WriteLine(s.Substring(index, newIndex - index));
                index = newIndex;
            }
        }

    }

    //public class Indended
    //{
    //    private readonly StringBuilder? _sb = null;

    //    public IndentedTextWriter Writer
    //    {
    //        get;
    //    }

    //    public Indended()
    //    {
    //        _sb = new StringBuilder();
    //        Writer = new IndentedTextWriter(
    //            new StringWriter(
    //                _sb
    //                )
    //            );
    //    }

    //    public Indended(
    //        Indended parent
    //        )
    //    {
    //        if (parent is null)
    //        {
    //            throw new ArgumentNullException(nameof(parent));
    //        }

    //        _sb = null;
    //        Writer = new IndentedTextWriter(parent.Writer);
    //        Writer.Indent += 1;
    //    }
    //}
}
