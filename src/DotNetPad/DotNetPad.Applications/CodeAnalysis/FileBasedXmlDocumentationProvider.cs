using Microsoft.CodeAnalysis;
using System.Globalization;
using System.Xml.Linq;

namespace Waf.DotNetPad.Applications.CodeAnalysis;

internal sealed class FileBasedXmlDocumentationProvider : DocumentationProvider
{
    private readonly string filePath;
    private readonly Lazy<Dictionary<string, string>> docComments;

    public FileBasedXmlDocumentationProvider(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        docComments = new(CreateDocComments);
    }

    public override bool Equals(object? obj) => obj is FileBasedXmlDocumentationProvider other && filePath.Equals(other.filePath);

    public override int GetHashCode() => filePath.GetHashCode();

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
        if (File.Exists(originalPath)) return originalPath;
            
        var fileName = Path.GetFileName(originalPath);
        string? path = null;
        // TOOD: Bad design with hard-coded path. Required Roslyn API is internal.
        foreach (var version in Enumerable.Range(0, 19).Select(x => $"8.0.{18-x}\\ref\\net8.0"))
        {
            path = GetNetFrameworkPathOrNull(fileName, version);
            if (path != null) break;
        }
        return path;
    }

    private static string? GetNetFrameworkPathOrNull(string fileName, string version)
    {
        const string netFrameworkPathPart = @"dotnet\packs\Microsoft.NETCore.App.Ref";
        var newPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), netFrameworkPathPart, version, fileName);
        return File.Exists(newPath) ? newPath : null;
    }
}
