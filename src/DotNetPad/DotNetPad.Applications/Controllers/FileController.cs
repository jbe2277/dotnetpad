﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Waf.Foundation;
using Waf.DotNetPad.Applications.CodeAnalysis;
using Waf.DotNetPad.Applications.Properties;
using Waf.DotNetPad.Applications.Services;
using Waf.DotNetPad.Applications.ViewModels;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Controllers
{
    /// <summary>
    /// Responsible for the file related commands.
    /// </summary>
    [Export]
    internal class FileController
    {
        private readonly IMessageService messageService;
        private readonly IFileDialogService fileDialogService;
        private readonly IShellService shellService;
        private readonly IEnvironmentService environmentService;
        private readonly IClipboardService clipboardService;
        private readonly FileService fileService;
        private readonly ExportFactory<SaveChangesViewModel> saveChangesViewModelFactory;
        private readonly DelegateCommand newCSharpCommand;
        private readonly DelegateCommand newVisualBasicCommand;
        private readonly DelegateCommand newCSharpFromClipboardCommand;
        private readonly DelegateCommand newVisualBasicFromClipboardCommand;
        private readonly DelegateCommand openCommand;
        private readonly DelegateCommand closeCommand;
        private readonly DelegateCommand closeAllCommand;
        private readonly DelegateCommand saveCommand;
        private readonly DelegateCommand saveAsCommand;
        private readonly FileType cSharpFileType;
        private readonly FileType visualBasicFileType;
        private readonly FileType allFilesType;
        private readonly List<DocumentFile> observedDocumentFiles;
        private DocumentFile lastActiveDocumentFile;
        private int documentCounter;


        [ImportingConstructor]
        public FileController(IMessageService messageService, IFileDialogService fileDialogService, IShellService shellService, IEnvironmentService environmentService, 
            IClipboardService clipboardService, FileService fileService, ExportFactory<SaveChangesViewModel> saveChangesViewModelFactory)
        {
            this.messageService = messageService;
            this.fileDialogService = fileDialogService;
            this.shellService = shellService;
            this.environmentService = environmentService;
            this.clipboardService = clipboardService;
            this.fileService = fileService;
            this.saveChangesViewModelFactory = saveChangesViewModelFactory;
            this.newCSharpCommand = new DelegateCommand(NewCSharpFile);
            this.newVisualBasicCommand = new DelegateCommand(NewVisualBasicFile);
            this.newCSharpFromClipboardCommand = new DelegateCommand(NewCSharpFromClipboard, CanNewFromClipboard);
            this.newVisualBasicFromClipboardCommand = new DelegateCommand(NewVisualBasicFromClipboard, CanNewFromClipboard);
            this.openCommand = new DelegateCommand(OpenFile);
            this.closeCommand = new DelegateCommand(CloseFile, CanCloseFile);
            this.closeAllCommand = new DelegateCommand(CloseAll, CanCloseAll);
            this.saveCommand = new DelegateCommand(SaveFile, CanSaveFile);
            this.saveAsCommand = new DelegateCommand(SaveAsFile, CanSaveAsFile);

            this.fileService.NewCSharpCommand = newCSharpCommand;
            this.fileService.NewVisualBasicCommand = newVisualBasicCommand;
            this.fileService.NewCSharpFromClipboardCommand = newCSharpFromClipboardCommand;
            this.fileService.NewVisualBasicFromClipboardCommand = newVisualBasicFromClipboardCommand;
            this.fileService.OpenCommand = openCommand;
            this.fileService.CloseCommand = closeCommand;
            this.fileService.CloseAllCommand = closeAllCommand;
            this.fileService.SaveCommand = saveCommand;
            this.fileService.SaveAsCommand = saveAsCommand;

            this.cSharpFileType = new FileType(Resources.CSharpFile, ".cs");
            this.visualBasicFileType = new FileType(Resources.VisualBasicFile, ".vb");
            this.allFilesType = new FileType(Resources.CodeFile, ".cs;*.vb");
            this.observedDocumentFiles = new List<DocumentFile>();
            PropertyChangedEventManager.AddHandler(fileService, FileServicePropertyChanged, "");
            shellService.Closing += ShellServiceClosing;
        }


        private DocumentFile ActiveDocumentFile
        {
            get { return fileService.ActiveDocumentFile; }
            set { fileService.ActiveDocumentFile = value; }
        }

        private DocumentFile LockedDocumentFile => fileService.LockedDocumentFile;

        public void Initialize()
        {
            fileService.DocumentFiles.CollectionChanged += FileServiceDocumentsCollectionChanged;
        }

        public async void Run()
        {
            await Task.Yield();  // Ensure that the ShellView is shown before loading files
            foreach (var fileToLoad in shellService.Settings.LastOpenedFiles)
            {
                OpenCore(fileToLoad, setActiveDocument: false);
            }
            foreach (var fileToLoad in environmentService.FilesToLoad)
            {
                OpenCore(fileToLoad);
            }
            if (!environmentService.FilesToLoad.Any())
            {
                NewCSharpFile();
            }
        }

        private void NewCSharpFile(object commandParameter = null) 
        {
            NewCore(DocumentType.CSharp, TryGetCode(commandParameter));
        }

        private void NewVisualBasicFile(object commandParameter = null)
        {
            NewCore(DocumentType.VisualBasic, TryGetCode(commandParameter));
        }

        private static string TryGetCode(object commandParameter)
        {
            string code = null;
            var lazyParameter = commandParameter as Lazy<string>;
            if (lazyParameter != null)
            {
                code = lazyParameter.Value;
            }
            else
            {
                code = commandParameter as string;
            }
            return code;
        }

        private bool CanNewFromClipboard()
        {
            return clipboardService.ContainsText();
        }

        private void NewCSharpFromClipboard()
        {
            string code = clipboardService.GetText();
            if (!string.IsNullOrEmpty(code))
            {
                NewCore(DocumentType.CSharp, code);
            }
        }

        private void NewVisualBasicFromClipboard()
        {
            string code = clipboardService.GetText();
            if (!string.IsNullOrEmpty(code))
            {
                NewCore(DocumentType.VisualBasic, code);
            }
        }

        private void OpenFile()
        {
            OpenCore();
        }

        private bool CanCloseFile() { return ActiveDocumentFile != null && ActiveDocumentFile != LockedDocumentFile; }

        private void CloseFile() { CloseCore(ActiveDocumentFile); }

        private bool CanCloseAll() { return ActiveDocumentFile != null && LockedDocumentFile == null; }

        private bool CanSaveFile() { return ActiveDocumentFile != null && ActiveDocumentFile.Modified; }

        private void SaveFile() { Save(ActiveDocumentFile); }

        private bool CanSaveAsFile() { return ActiveDocumentFile != null; }

        private void SaveAsFile() { SaveAs(ActiveDocumentFile); }

        private void Save(DocumentFile document)
        {
            if (Path.IsPathRooted(document.FileName))
            {
                SaveCore(document, document.FileName);
            }
            else
            {
                SaveAs(document);
            }
        }

        private void SaveAs(DocumentFile document)
        {
            FileType fileType = document.DocumentType == DocumentType.CSharp ? cSharpFileType : visualBasicFileType;
            string fileName = Path.GetFileNameWithoutExtension(document.FileName);

            FileDialogResult result = fileDialogService.ShowSaveFileDialog(shellService.ShellView, fileType, fileName);
            if (result.IsValid)
            {
                SaveCore(document, result.FileName);
            }
        }

        private void NewCore(DocumentType documentType, string code = null)
        {
            int startCaretPosition = 0;
            if (string.IsNullOrEmpty(code))
            {
                code = documentType == DocumentType.CSharp ? TemplateCode.InitialCSharpCode : TemplateCode.InitialVisualBasicCode;
                startCaretPosition = documentType == DocumentType.CSharp ? TemplateCode.StartCaretPositionCSharp : TemplateCode.StartCaretPositionVisualBasic;
            }
            string fileName = "Script" + (documentCounter + 1) + (documentType == DocumentType.CSharp ? ".cs" : ".vb");
            var document = new DocumentFile(documentType, fileName, code, startCaretPosition);
            document.ResetModified();
            fileService.AddDocument(document);
            ActiveDocumentFile = document;
            documentCounter++;
        }

        private bool PrepareToClose(IEnumerable<DocumentFile> documentsToClose)
        {
            var modifiedDocuments = documentsToClose.Where(d => d.Modified).ToArray();
            if (!modifiedDocuments.Any()) { return true; }

            SaveChangesViewModel saveChangesViewModel = saveChangesViewModelFactory.CreateExport().Value;
            saveChangesViewModel.DocumentFiles = modifiedDocuments;
            bool? dialogResult = saveChangesViewModel.ShowDialog(shellService.ShellView);
            if (dialogResult == true)
            {
                foreach (var document in modifiedDocuments)
                {
                    Save(document);
                }
            }

            return dialogResult != null;
        }

        private void CloseCore(DocumentFile document)
        {
            if (!PrepareToClose(new[] { document })) { return; }

            if (ActiveDocumentFile == document)
            {
                var nextDocument = CollectionHelper.GetNextElementOrDefault(fileService.DocumentFiles, ActiveDocumentFile)
                    ?? fileService.DocumentFiles.Take(fileService.DocumentFiles.Count - 1).LastOrDefault();
                
                ActiveDocumentFile = nextDocument;
            }
            fileService.RemoveDocument(document);
        }

        private void CloseAll()
        {
            if (!PrepareToClose(fileService.DocumentFiles)) { return; }

            ActiveDocumentFile = null;
            fileService.ClearDocuments();
        }
        
        private DocumentFile OpenCore(string fileName = null, bool setActiveDocument = true)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                FileDialogResult result = fileDialogService.ShowOpenFileDialog(shellService.ShellView, allFilesType);
                if (result.IsValid)
                {
                    fileName = result.FileName;
                }
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            // Check if document is already opened
            DocumentFile document = fileService.DocumentFiles.SingleOrDefault(d => d.FileName == fileName);
            if (document == null)
            {
                string fileExtension = Path.GetExtension(fileName);
                if (!new[] { ".cs", ".vb" }.Contains(fileExtension))
                {
                    Trace.TraceError(string.Format(CultureInfo.InvariantCulture,
                        "The extension of the file '{0}' is not supported.", fileName));
                    messageService.ShowError(shellService.ShellView, string.Format(CultureInfo.CurrentCulture, Resources.OpenFileUnsupportedExtension, fileName));
                    return null;
                }

                var documentType = fileExtension == ".cs" ? DocumentType.CSharp : DocumentType.VisualBasic;
                document = new DocumentFile(documentType, LoadDocumentContent)
                {
                    FileName = fileName
                };

                fileService.AddDocument(document);
                documentCounter++;
            }

            if (setActiveDocument) { ActiveDocumentFile = document; }
            return document;
        }

        private DocumentContent LoadDocumentContent(DocumentFile documentFile)
        {            
            Trace.WriteLine(">> Load document content: " + documentFile.FileName);
            using (var stream = new FileStream(documentFile.FileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var documentContent = new DocumentContent()
                    {
                        Code = reader.ReadToEnd()
                    };
                    documentFile.ResetModified();
                    return documentContent;
                }
            }
        }

        private void SaveCore(DocumentFile document, string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.Write(document.Content.Code);
                    }
                }
                document.FileName = fileName;
                document.ResetModified();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                messageService.ShowError(shellService.ShellView, string.Format(CultureInfo.CurrentCulture, Resources.SaveFileError, fileName));
            }
        }

        private void FileServiceDocumentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DocumentFile documentFile in e.NewItems)
                {
                    documentFile.PropertyChanged += DocumentFilePropertyChanged;
                    observedDocumentFiles.Add(documentFile);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DocumentFile documentFile in e.OldItems)
                {
                    documentFile.PropertyChanged -= DocumentFilePropertyChanged;
                    observedDocumentFiles.Remove(documentFile);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset && !fileService.DocumentFiles.Any())
            {
                foreach (DocumentFile documentFile in observedDocumentFiles)
                {
                    documentFile.PropertyChanged -= DocumentFilePropertyChanged;
                }
                observedDocumentFiles.Clear();
            }
            else
            {
                throw new NotSupportedException("The CollectionChangedAction '" + e.Action + "' is not supported.");
            }
        }

        private async void DocumentFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DocumentFile.LoadError))
            {
                await Task.Yield();
                var document = ((DocumentFile)sender);
                CloseCore(document);
                messageService.ShowError(shellService.ShellView, string.Format(CultureInfo.CurrentCulture, Resources.OpenFileError, document.FileName));
            }
        }

        private void FileServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IFileService.ActiveDocumentFile))
            {
                if (lastActiveDocumentFile != null) { PropertyChangedEventManager.RemoveHandler(lastActiveDocumentFile, ActiveDocumentPropertyChanged, ""); }

                lastActiveDocumentFile = ActiveDocumentFile;

                if (lastActiveDocumentFile != null) { PropertyChangedEventManager.AddHandler(lastActiveDocumentFile, ActiveDocumentPropertyChanged, ""); }

                UpdateCommands();
            }
            else if (e.PropertyName == nameof(IFileService.LockedDocumentFile))
            {
                UpdateCommands();
            }
        }

        private void ActiveDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DocumentFile.Modified))
            {
                UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            closeCommand.RaiseCanExecuteChanged();
            closeAllCommand.RaiseCanExecuteChanged();
            saveCommand.RaiseCanExecuteChanged();
            saveAsCommand.RaiseCanExecuteChanged();
        }

        private void ShellServiceClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !PrepareToClose(fileService.DocumentFiles);
            if (!e.Cancel)
            {
                shellService.Settings.LastOpenedFiles = fileService.DocumentFiles.Select(x => x.FileName).Where(Path.IsPathRooted).ToArray();
            }
        }
    }
}
