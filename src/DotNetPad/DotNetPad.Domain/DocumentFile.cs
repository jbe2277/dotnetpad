using System.Diagnostics;

namespace Waf.DotNetPad.Domain
{
    public class DocumentFile : Model
    {
        private readonly Func<DocumentFile, DocumentContent> contentFactory;
        private readonly Lazy<DocumentContent?> content;
        private Exception? loadError;
        private bool modified;
        private string? fileName;

        public DocumentFile(DocumentType documentType, Func<DocumentFile, DocumentContent> contentFactory, int startCaretPosition = 0)
        {
            DocumentType = documentType;
            this.contentFactory = contentFactory;
            content = new Lazy<DocumentContent?>(LoadContent);
            StartCaretPosition = startCaretPosition;
        }

        public DocumentFile(DocumentType documentType, string fileName, string code, int startCaretPosition = 0)
            : this(documentType, df => new DocumentContent() { Code = code }, startCaretPosition)
        {
            this.fileName = fileName;
        }


        public DocumentType DocumentType { get; }

        public bool IsContentLoaded => content.IsValueCreated;

        public Exception? LoadError
        {
            get => loadError;
            private set => SetProperty(ref loadError, value);
        }

        public DocumentContent? Content 
        { 
            get 
            {
                var wasContentLoaded = IsContentLoaded;
                var value = content.Value;
                if (!wasContentLoaded && IsContentLoaded)
                {
                    RaisePropertyChanged(nameof(IsContentLoaded));
                }
                return value;
            } 
        }

        public int StartCaretPosition { get; }

        public bool Modified
        {
            get => modified;
            private set => SetProperty(ref modified, value);
        }

        public string? FileName
        {
            get => fileName;
            set => SetProperty(ref fileName, value);
        }

        public void ResetModified()
        {
            Modified = false;
        }

        private DocumentContent? LoadContent()
        {
            DocumentContent? content = null;
            try
            {
                content = contentFactory(this);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LoadError = ex;
            }
            
            if (content != null) content.PropertyChanged += ContentPropertyChanged;
            return content;
        }

        private void ContentPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DocumentContent.Code))
            {
                Modified = true;
            }
        }
    }
}
