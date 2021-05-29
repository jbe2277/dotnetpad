using System;
using System.ComponentModel.Composition;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services
{
    [Export(typeof(ICodeEditorService)), Export]
    public class CodeEditorService : ICodeEditorService
    {
        public event EventHandler<SetCaretEventArgs>? RequestSetCaret;

        public void SetCaret(DocumentFile documentFile, int line, int column) => OnRequestSetCaret(new SetCaretEventArgs(documentFile, line, column));

        protected virtual void OnRequestSetCaret(SetCaretEventArgs e) => RequestSetCaret?.Invoke(this, e);
    }


    public class SetCaretEventArgs : EventArgs
    {
        public SetCaretEventArgs(DocumentFile documentFile, int line, int column)
        {
            DocumentFile = documentFile;
            Line = line;
            Column = column;
        }

        public DocumentFile DocumentFile { get; }

        public int Line { get; }

        public int Column { get; }
    }
}
