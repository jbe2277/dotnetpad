using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeCompletionData : ICompletionData
    {
        private readonly Lazy<CodeCompletionDescription> description;
        private readonly Func<Task<string>> getDescriptionFunc;
        private readonly ImmutableArray<string> tags;
        private readonly Lazy<ImageSource> image;
        

        public CodeCompletionData(string text, Func<Task<string>> getDescriptionFunc, ImmutableArray<string> tags)
        {
            this.Text = text;
            this.description = new Lazy<CodeCompletionDescription>(CreateDescription);
            this.getDescriptionFunc = getDescriptionFunc;
            this.tags = tags;
            this.image = new Lazy<ImageSource>(GetImage);
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
            var result = new CodeCompletionDescription(getDescriptionFunc(), "");  // TODO: Remove overloads
            return result;
        }

        private ImageSource GetImage()
        {
            var tag = tags.FirstOrDefault();
            if (tag == null) { return null; }
            
            switch (tag)
            {
                case "Class":
                    return GetImage("ClassImageSource");
                case "Constant":
                    return GetImage("ConstantImageSource");
                case "Delegate":
                    return GetImage("DelegateImageSource");
                case "Enum":
                    return GetImage("EnumImageSource");
                case "EnumMember":
                    return GetImage("EnumItemImageSource");
                case "Event":
                    return GetImage("EventImageSource");    
                case "ExtensionMethod":
                    return GetImage("ExtensionMethodImageSource");
                case "Field":
                    return GetImage("FieldImageSource");
                case "Interface":
                    return GetImage("InterfaceImageSource");
                case "Keyword":
                    return GetImage("KeywordImageSource");
                case "Method":
                    return GetImage("MethodImageSource");
                case "Module":
                    return GetImage("ModuleImageSource");
                case "Namespace":
                    return GetImage("NamespaceImageSource");
                case "Property":
                    return GetImage("PropertyImageSource");
                case "Structure":
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
