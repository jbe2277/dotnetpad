using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;
using Waf.DotNetPad.Domain;

namespace Waf.DotNetPad.Applications.Services
{
    [Export(typeof(IDocumentService)), Export(typeof(IFileService)), Export]
    internal class FileService : Model, IFileService
    {
        private readonly ObservableCollection<DocumentFile> documentFiles;
        private readonly ReadOnlyObservableList<DocumentFile> readOnlyDocumentFiles;
        private DocumentFile activeDocumentFile;
        private DocumentFile lockedDocumentFile;
        private ICommand newCSharpCommand;
        private ICommand newVisualBasicCommand;
        private DelegateCommand newCSharpFromClipboardCommand;
        private DelegateCommand newVisualBasicFromClipboardCommand;
        private ICommand openCommand;
        private ICommand closeCommand;
        private ICommand closeAllCommand;
        private ICommand saveCommand;
        private ICommand saveAsCommand;


        [ImportingConstructor]
        public FileService()
        {
            this.documentFiles = new ObservableCollection<DocumentFile>();
            this.readOnlyDocumentFiles = new ReadOnlyObservableList<DocumentFile>(documentFiles);
        }


        public IReadOnlyObservableList<DocumentFile> DocumentFiles { get { return readOnlyDocumentFiles; } }

        public DocumentFile ActiveDocumentFile
        {
            get { return activeDocumentFile; }
            set
            {
                if (activeDocumentFile != value)
                {
                    if (value != null && !documentFiles.Contains(value))
                    {
                        throw new ArgumentException("value is not an item of the Documents collection.");
                    }
                    activeDocumentFile = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DocumentFile LockedDocumentFile
        {
            get { return lockedDocumentFile; }
            set { SetProperty(ref lockedDocumentFile, value); }
        }

        public ICommand NewCSharpCommand
        {
            get { return newCSharpCommand; }
            set { SetProperty(ref newCSharpCommand, value); }
        }

        public ICommand NewVisualBasicCommand
        {
            get { return newVisualBasicCommand; }
            set { SetProperty(ref newVisualBasicCommand, value); }
        }

        public DelegateCommand NewCSharpFromClipboardCommand
        {
            get { return newCSharpFromClipboardCommand; }
            set { SetProperty(ref newCSharpFromClipboardCommand, value); }
        }

        public DelegateCommand NewVisualBasicFromClipboardCommand
        {
            get { return newVisualBasicFromClipboardCommand; }
            set { SetProperty(ref newVisualBasicFromClipboardCommand, value); }
        }

        public ICommand OpenCommand
        {
            get { return openCommand; }
            set { SetProperty(ref openCommand, value); }
        }

        public ICommand CloseCommand
        {
            get { return closeCommand; }
            set { SetProperty(ref closeCommand, value); }
        }

        public ICommand CloseAllCommand
        {
            get { return closeAllCommand; }
            set { SetProperty(ref closeAllCommand, value); }
        }

        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set { SetProperty(ref saveCommand, value); }
        }

        public ICommand SaveAsCommand
        {
            get { return saveAsCommand; }
            set { SetProperty(ref saveAsCommand, value); }
        }


        public void AddDocument(DocumentFile document)
        {
            documentFiles.Add(document);
        }

        public void RemoveDocument(DocumentFile document)
        {
            documentFiles.Remove(document);
        }

        public void ClearDocuments()
        {
            documentFiles.Clear();
        }
    }
}
