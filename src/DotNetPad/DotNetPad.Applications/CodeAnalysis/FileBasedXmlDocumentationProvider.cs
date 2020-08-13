using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    internal class FileBasedXmlDocumentationProvider : DocumentationProvider
    {
        private readonly string filePath;
        private readonly Lazy<Dictionary<string, string>> docComments;

        public FileBasedXmlDocumentationProvider(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            docComments = new Lazy<Dictionary<string, string>>(CreateDocComments, isThreadSafe: true);
        }

        public override bool Equals(object? obj)
        {
            return obj is FileBasedXmlDocumentationProvider other && filePath == other.filePath;
        }

        public override int GetHashCode()
        {
            return filePath.GetHashCode();
        }
        
        protected override string GetDocumentationForSymbol(string documentationMemberID, CultureInfo preferredCulture, CancellationToken cancellationToken = default)
        {
            return docComments.Value.TryGetValue(documentationMemberID, out var docComment) ? docComment : "";
        }

        private Dictionary<string, string> CreateDocComments()
        {
            var commentsDictionary = new Dictionary<string, string>();
            try
            {
                var foundPath = GetDocumentationFilePath(filePath);
                if (!string.IsNullOrEmpty(foundPath))
                {
                    var document = XDocument.Load(foundPath);
                    foreach (var element in document.Descendants("member"))
                    {
                        var nameAttribute = element.Attribute("name");
                        if (nameAttribute != null)
                        {
                            commentsDictionary[nameAttribute.Value] = string.Concat(element.Nodes());
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }
            return commentsDictionary;
        }

        private static string? GetDocumentationFilePath(string originalPath)
        {
            if (File.Exists(originalPath)) { return originalPath; }
            
            var fileName = Path.GetFileName(originalPath);
            string? path = null;
            foreach (var version in new[] { @"v4.X", @"v4.7.1", @"v4.7", @"v4.6.1", @"v4.6", @"v4.5.2", @"v4.5.1", @"v4.5" })
            {
                path = GetNetFrameworkPathOrNull(fileName, version);
                if (path != null)
                {
                    break;
                }
            }
            
            return path;
        }

        private static string? GetNetFrameworkPathOrNull(string fileName, string version)
        {
            const string netFrameworkPathPart = @"Reference Assemblies\Microsoft\Framework\.NETFramework";
            var newPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), netFrameworkPathPart, version, fileName);
            return File.Exists(newPath) ? newPath : null;
        }
    }
}
