using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    internal class ScriptingWorkspace : Workspace
    {
        private static readonly Assembly[] defaultReferences =
            {
                typeof(Int32).Assembly,                                 // mscorelib
                typeof(Uri).Assembly,                                   // System
                typeof(Enumerable).Assembly,                            // System.Core
                typeof(XmlReader).Assembly,                             // System.Xml
                typeof(XDocument).Assembly,                             // System.Xml.Linq
                typeof(DataContractSerializer).Assembly,                // System.Runtime.Serialization
                typeof(ImmutableArray).Assembly,                        // System.Collections.Immutable
            };

        private readonly ConcurrentDictionary<string, DocumentationProvider> documentationProviders;


        public ScriptingWorkspace() : base(MefHostServices.DefaultHost, WorkspaceKind.Host)
        {
            documentationProviders = new ConcurrentDictionary<string, DocumentationProvider>();
        }


        public DocumentId AddProjectWithDocument(string documentFileName, string text)
        {
            var fileName = Path.GetFileName(documentFileName);
            var name = Path.GetFileNameWithoutExtension(documentFileName);
            var language = Path.GetExtension(documentFileName) == ".vb" ? LanguageNames.VisualBasic : LanguageNames.CSharp;

            var projectId = ProjectId.CreateNewId();
            
            var references = defaultReferences.Select(CreateReference).ToList();
            if (language == LanguageNames.VisualBasic) { references.Add(CreateReference(typeof(VBMath).Assembly)); }
            else if (language == LanguageNames.CSharp) { references.Add(CreateReference(typeof(RuntimeBinderException).Assembly)); }

            var projectInfo = ProjectInfo.Create(projectId, VersionStamp.Default, name, name + ".dll", language, metadataReferences: references);
            OnProjectAdded(projectInfo);

            var documentId = DocumentId.CreateNewId(projectId);
            var documentInfo = DocumentInfo.Create(documentId, fileName, 
                                loader: TextLoader.From(TextAndVersion.Create(SourceText.From(text, Encoding.UTF8), VersionStamp.Create())));
            OnDocumentAdded(documentInfo);
            return documentId;
        }

        public void RemoveProject(DocumentId id)
        {
            OnProjectRemoved(id.ProjectId);
        }

        public void UpdateText(DocumentId documentId, string text)
        {
            OnDocumentTextChanged(documentId, SourceText.From(text, Encoding.UTF8), PreservationMode.PreserveValue);
        }

        public async Task<IReadOnlyList<ISymbol>> GetRecommendedSymbolsAsync(DocumentId documentId, int position, CancellationToken cancellationToken)
        {
            var document = CurrentSolution.GetDocument(documentId);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var symbols = await Recommender.GetRecommendedSymbolsAtPositionAsync(semanticModel, position, this, null, cancellationToken).ConfigureAwait(false);
            return symbols.ToArray();
        }

        public Task<IReadOnlyList<Diagnostic>> GetDiagnosticsAsync(DocumentId documentId, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var project = CurrentSolution.GetProject(documentId.ProjectId);
                var compilation = await project.GetCompilationAsync(cancellationToken);
                return (IReadOnlyList<Diagnostic>)compilation.GetDiagnostics(cancellationToken);
            }, cancellationToken);
        }

        public Task<BuildResult> BuildAsync(DocumentId documentId, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var project = CurrentSolution.GetProject(documentId.ProjectId);
                var compilation = await project.GetCompilationAsync(cancellationToken);
            
                using (var peStream = new MemoryStream())
                using (var pdbStream = new MemoryStream())
                {
                    var result = compilation.Emit(peStream, pdbStream);
                    var inMemoryAssembly = result.Success ? peStream.ToArray() : null;
                    var inMemorySymbolStore = result.Success ? pdbStream.ToArray() : null;
                    return new BuildResult(result.Diagnostics, inMemoryAssembly, inMemorySymbolStore);
                }
            }, cancellationToken);
        }
        
        private MetadataReference CreateReference(Assembly assembly)
        {
            string assemblyPath = assembly.Location;
            string documentationPath = Path.ChangeExtension(assemblyPath, "xml");
            var provider = documentationProviders.GetOrAdd(documentationPath, path => new FileBasedXmlDocumentationProvider(path));
            return MetadataReference.CreateFromFile(assemblyPath, new MetadataReferenceProperties(), provider);
        }
    }
}
