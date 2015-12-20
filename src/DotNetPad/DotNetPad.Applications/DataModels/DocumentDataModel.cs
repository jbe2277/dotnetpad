using System;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.DataModels
{
    public class DocumentDataModel
    {
        private readonly DocumentFile documentFile;
        private readonly Lazy<object> lazyCodeEditorView;


        public DocumentDataModel(DocumentFile documentFile, Lazy<object> lazyCodeEditorView)
        {
            this.documentFile = documentFile;
            this.lazyCodeEditorView = lazyCodeEditorView;
        }


        public DocumentFile DocumentFile { get { return documentFile; } }

        public Lazy<object> LazyCodeEditorView { get { return lazyCodeEditorView; } }
    }
}
