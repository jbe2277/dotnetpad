using System;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.DataModels
{
    public class DocumentDataModel
    {
        public DocumentDataModel(DocumentFile documentFile, Lazy<object> lazyCodeEditorView)
        {
            DocumentFile = documentFile;
            LazyCodeEditorView = lazyCodeEditorView;
        }

        public DocumentFile DocumentFile { get; }

        public Lazy<object> LazyCodeEditorView { get; }
    }
}
