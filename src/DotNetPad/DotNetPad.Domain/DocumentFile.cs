namespace Waf.DotNetPad.Domain;

public class DocumentFile : Model
{
    private readonly Func<DocumentFile, DocumentContent> contentFactory;
    private readonly Lazy<DocumentContent?> content;

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
        FileName = fileName;
    }

    public DocumentType DocumentType { get; }

    public int StartCaretPosition { get; }

    public bool IsContentLoaded => content.IsValueCreated;

    public Exception? LoadError { get; set => SetProperty(ref field, value); }

    public DocumentContent? Content
    {
        get
        {
            var wasContentLoaded = IsContentLoaded;
            var value = content.Value;
            if (!wasContentLoaded && IsContentLoaded) RaisePropertyChanged(nameof(IsContentLoaded));
            return value;
        }
    }

    public bool Modified { get; set => SetProperty(ref field, value); }

    public string? FileName { get; set => SetProperty(ref field, value); }

    public void ResetModified() => Modified = false;

    private DocumentContent? LoadContent()
    {
        DocumentContent? content = null;
        try
        {
            content = contentFactory(this);
        }
        catch (Exception ex)
        {
            Log.Default.Error("LoadContent error: {0}", ex);
            LoadError = ex;
        }
            
        content?.PropertyChanged += ContentPropertyChanged;
        return content;
    }

    private void ContentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentContent.Code)) Modified = true;
    }
}
