using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Applications.Views;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Presentation.Views;

[Export(typeof(IOutputView))]
public partial class OutputView : IOutputView
{
    private readonly Lazy<OutputViewModel> viewModel;
    private readonly Dictionary<DocumentFile, Paragraph> outputParagraphs;

    public OutputView()
    {
        InitializeComponent();
        viewModel = new(() => this.GetViewModel<OutputViewModel>()!);
        outputParagraphs = [];

        Loaded += FirstTimeLoadedHandler;
        outputBox.TextChanged += OutputBoxTextChanged;
    }

    private OutputViewModel ViewModel => viewModel.Value;

    public void AppendOutputText(DocumentFile document, string text) => outputParagraphs[document].Inlines.Add(text);

    public void AppendErrorText(DocumentFile document, string text) => outputParagraphs[document].Inlines.Add(new Run(text) { Foreground = (Brush)FindResource("ErrorForeground") });

    public void ClearOutput(DocumentFile document) => outputParagraphs[document].Inlines.Clear();

    private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
    {
        Loaded -= FirstTimeLoadedHandler;

        WeakEvent.CollectionChanged.Add(ViewModel.DocumentService.DocumentFiles, DocumentsCollectionChanged);
        foreach (var x in ViewModel.DocumentService.DocumentFiles) outputParagraphs.Add(x, new Paragraph());

        WeakEvent.PropertyChanged.Add(ViewModel.DocumentService, DocumentServicePropertyChanged);
        if (ViewModel.DocumentService.ActiveDocumentFile != null)
        {
            outputDocument.Blocks.Add(outputParagraphs[ViewModel.DocumentService.ActiveDocumentFile]);
        }
    }

    private void DocumentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (DocumentFile x in e.NewItems!) outputParagraphs.Add(x, new Paragraph());
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (DocumentFile x in e.OldItems!) outputParagraphs.Remove(x);
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset && !ViewModel.DocumentService.DocumentFiles.Any())  // Clear
        { 
            outputParagraphs.Clear();
        }
        else throw new NotSupportedException("Collection modification is not supported!");
    }

    private void DocumentServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IDocumentService.ActiveDocumentFile))
        {
            outputDocument.Blocks.Clear();
            if (ViewModel.DocumentService.ActiveDocumentFile != null)
            {
                outputDocument.Blocks.Add(outputParagraphs[ViewModel.DocumentService.ActiveDocumentFile]);
            }
        }
    }

    private void OutputBoxTextChanged(object? sender, TextChangedEventArgs e) => outputBox.ScrollToEnd();
}
