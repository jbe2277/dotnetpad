using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using Waf.DotNetPad.Applications.CodeAnalysis;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeCompletionData : ICompletionData
    {
        private readonly Lazy<CodeCompletionDescription> description;
        private readonly Lazy<ImageSource> image;
        private readonly IReadOnlyList<ISymbol> symbols;


        public CodeCompletionData(string text, IReadOnlyList<ISymbol> symbols)
        {
            this.Text = text;
            this.image = new Lazy<ImageSource>(GetImage);
            this.description = new Lazy<CodeCompletionDescription>(CreateDescription);
            this.symbols = symbols ?? new ISymbol[0];
        }


        public double Priority => 0;

        public string Text { get; }

        public object Description => description.Value;

        public object Content => Text;

        public ImageSource Image => image.Value;


        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        private CodeCompletionDescription CreateDescription()
        {
            string overloads = string.Join(Environment.NewLine, symbols.Select(x => x.ToDisplayString()));
            var result = new CodeCompletionDescription(GetSummaryAsync(), overloads);
            return result;
        }

        private Task<string> GetSummaryAsync()
        {
            return Task.Run(() =>
            {
                string summary = "";
                if (symbols.Any())
                {
                    string documentation = symbols.First().GetDocumentationCommentXml();
                    if (!string.IsNullOrEmpty(documentation))
                    {
                        if (documentation.Contains("<summary>"))
                        {
                            try
                            {
                                summary = (from item in XDocument.Parse("<r>" + documentation + "</r>").Descendants("summary")
                                            select GetValue(item)
                                          ).First().Trim();
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            summary = documentation.Trim();
                        }
                    }
                }
                return summary;
            });
        }

        private static string GetValue(XElement element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var node in element.Nodes())
            {
                var textNode = node as XText;
                if (textNode != null)
                {
                    stringBuilder.Append(textNode.Value);
                }
                var elementNode = node as XElement;
                if (elementNode != null && elementNode.Name == "see")
                {
                    var crefAttribute = elementNode.Attributes("cref").FirstOrDefault();
                    if (crefAttribute != null)
                    {
                        stringBuilder.Append(crefAttribute.Value.Split('.').Last());
                    }
                }
            }
            return stringBuilder.ToString();
        }
        
        private ImageSource GetImage()
        {
            var glyph = symbols.FirstOrDefault()?.GetGlyph();
            if (glyph == null) { return null; }
            
            switch (glyph.Value)
            {
                case Glyph.Class:
                    return GetImage("ClassImageSource");
                case Glyph.Constant:
                    return GetImage("ConstantImageSource");
                case Glyph.Delegate:
                    return GetImage("DelegateImageSource");
                case Glyph.Enum:
                    return GetImage("EnumImageSource");
                case Glyph.EnumMember:
                    return GetImage("EnumItemImageSource");
                case Glyph.Event:
                    return GetImage("EventImageSource");    
                case Glyph.ExtensionMethod:
                    return GetImage("ExtensionMethodImageSource");
                case Glyph.Field:
                    return GetImage("FieldImageSource");
                case Glyph.Interface:
                    return GetImage("InterfaceImageSource");
                case Glyph.Keyword:
                    return GetImage("KeywordImageSource");
                case Glyph.Method:
                    return GetImage("MethodImageSource");
                case Glyph.Module:
                    return GetImage("ModuleImageSource");
                case Glyph.Namespace:
                    return GetImage("NamespaceImageSource");
                case Glyph.Property:
                    return GetImage("PropertyImageSource");
                case Glyph.Structure:
                    return GetImage("StructureImageSource");
            }
            return null;
        }

        private static ImageSource GetImage(string resourceKey)
        {
            return (ImageSource)Application.Current.Resources[resourceKey];
        }
    }
}
