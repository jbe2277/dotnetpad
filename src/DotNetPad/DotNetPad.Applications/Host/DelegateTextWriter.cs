using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Waf.DotNetPad.Applications.Host
{
    public class DelegateTextWriter : TextWriter
    {
        private readonly Action<string> appendTextAction;


        public DelegateTextWriter(Action<string> appendTextAction) : base(CultureInfo.CurrentCulture)
        {
            this.appendTextAction = appendTextAction;
        }


        public override Encoding Encoding => Encoding.UTF8;
        
        public override void Write(char value)
        {
            appendTextAction(value.ToString());
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (index != 0 || count != buffer.Length)
            {
                buffer = buffer.Skip(index).Take(count).ToArray();
            }
            appendTextAction(new string(buffer));
        }

        public override void Write(string value)
        {
            appendTextAction(value);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
