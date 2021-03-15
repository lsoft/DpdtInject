using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector
{
    public class IndentedTextWriter2
    {
        private readonly StringBuilder _sb;
        private readonly IndentedTextWriter _writer;
        private readonly bool _doBeautify;

        public int Indent
        {
            get => _writer.Indent;
            set => _writer.Indent = value;
        }

        public IndentedTextWriter2(int indend, bool doBeautify)
        {
            _sb = new StringBuilder();
            _writer = new IndentedTextWriter(new StringWriter(_sb), IndentedTextWriter.DefaultTabString);
            _writer.Indent = indend;
            _doBeautify = doBeautify;
        }

        public string GetResultString()
        {
            _writer.Flush();
            return _sb.ToString();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string s) => _writer.Write(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLine() => _writer.WriteLineNoTabs(string.Empty);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLine(string s) => _writer.WriteLine(s);

        public void WriteLine2(string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (!_doBeautify)
            {
                _writer.WriteLine(s);
                return;
            }

            foreach (var par in s.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (string.IsNullOrEmpty(par))
                {
                    _writer.WriteLineNoTabs(string.Empty);
                }
                else
                {
                    _writer.WriteLine(par);
                }
            }
        }
    }
}
