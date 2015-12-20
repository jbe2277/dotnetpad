using System;
using System.ComponentModel.Composition;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services
{
    [Export(typeof(ICodeEditorService)), Export]
    public class CodeEditorService : ICodeEditorService
    {
        public event EventHandler<SetCaretEventArgs> RequestSetCaret;
        

        public void SetCaret(DocumentFile documentFile, int line, int column)
        {
            OnRequestSetCaret(new SetCaretEventArgs(documentFile, line, column));
        }


        protected virtual void OnRequestSetCaret(SetCaretEventArgs e)
        {
            var handler = RequestSetCaret;
            if (handler != null) { handler(this, e); }
        }
    }


    public class SetCaretEventArgs : EventArgs
    {
        private readonly DocumentFile documentFile;
        private readonly int line;
        private readonly int column;


        public SetCaretEventArgs(DocumentFile documentFile, int line, int column)
        {
            this.documentFile = documentFile;
            this.line = line;
            this.column = column;
        }


        public DocumentFile DocumentFile { get { return documentFile; } }

        public int Line { get { return line; } }

        public int Column { get { return column; } }
    }
}
