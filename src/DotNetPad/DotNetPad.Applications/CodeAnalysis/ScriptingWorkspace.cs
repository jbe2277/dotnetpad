﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    internal class ScriptingWorkspace : Workspace
    {
        private static readonly ImmutableArray<Assembly> defaultReferences = ImmutableArray.CreateRange(new[]
        {
            typeof(object).Assembly,                                // System.Runtime
            typeof(Console).Assembly,                               // System.Console
            typeof(Enumerable).Assembly,                            // System.Link
            typeof(ImmutableArray).Assembly,                        // System.Collections.Immutable
            typeof(INotifyPropertyChanged).Assembly,                // System.ObjectModel
        });

        private static readonly ImmutableArray<string> preprocessorSymbols = ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

        private readonly ConcurrentDictionary<string, DocumentationProvider> documentationProviders;

        public ScriptingWorkspace(HostServices hostServices) : base(hostServices, WorkspaceKind.Host)
        {
            documentationProviders = new ConcurrentDictionary<string, DocumentationProvider>();
        }

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
            return feature == ApplyChangesKind.ChangeDocument || base.CanApplyChange(feature);
        }

        public DocumentId AddProjectWithDocument(string documentFileName, string text)
        {
            var fileName = Path.GetFileName(documentFileName);
            var name = Path.GetFileNameWithoutExtension(documentFileName);
            var language = Path.GetExtension(documentFileName) == ".vb" ? LanguageNames.VisualBasic : LanguageNames.CSharp;

            var projectId = ProjectId.CreateNewId();

            var references = defaultReferences.Distinct().Select(CreateReference).ToList();
            references.Add(CreateReference(Assembly.Load("System.Runtime, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")));
            if (language == LanguageNames.VisualBasic) { references.Add(CreateReference(typeof(VBMath).Assembly)); }
            else if (language == LanguageNames.CSharp) { references.Add(CreateReference(typeof(RuntimeBinderException).Assembly)); }

            var projectInfo = ProjectInfo.Create(projectId, VersionStamp.Default, name, name + ".dll", language, metadataReferences: references,
                parseOptions: language == LanguageNames.CSharp
                    ? new CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp9, preprocessorSymbols: preprocessorSymbols)
                    : new VisualBasicParseOptions(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9));
            OnProjectAdded(projectInfo);

            var documentId = DocumentId.CreateNewId(projectId);
            var documentInfo = DocumentInfo.Create(documentId, fileName, loader: TextLoader.From(TextAndVersion.Create(SourceText.From(text, Encoding.UTF8), VersionStamp.Create())));
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

        public Task<IReadOnlyList<Diagnostic>> GetDiagnosticsAsync(DocumentId documentId, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var project = CurrentSolution.GetProject(documentId.ProjectId);
                var compilation = await project!.GetCompilationAsync(cancellationToken);
                return (IReadOnlyList<Diagnostic>)compilation!.GetDiagnostics(cancellationToken);
            }, cancellationToken);
        }

        public Task<BuildResult> BuildAsync(DocumentId documentId, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var project = CurrentSolution.GetProject(documentId.ProjectId);
                var compilation = await project!.GetCompilationAsync(cancellationToken);

                using var peStream = new MemoryStream();
                using var pdbStream = new MemoryStream();
                var result = compilation!.Emit(peStream, pdbStream);
                var inMemoryAssembly = result.Success ? peStream.ToArray() : null;
                var inMemorySymbolStore = result.Success ? pdbStream.ToArray() : null;
                return new BuildResult(result.Diagnostics, inMemoryAssembly, inMemorySymbolStore);
            }, cancellationToken);
        }

        public Task FormatDocumentAsync(DocumentId documentId)
        {
            return Task.Run(async () =>
            {
                var formattedDocument = await Microsoft.CodeAnalysis.Formatting.Formatter.FormatAsync(
                        CurrentSolution.GetDocument(documentId)!).ConfigureAwait(false);
                TryApplyChanges(formattedDocument.Project.Solution);
            });
        }

        protected override void ApplyDocumentTextChanged(DocumentId documentId, SourceText text)
        {
            OnDocumentTextChanged(documentId, text, PreservationMode.PreserveValue);
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
