using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis.Completion;
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
            var result = new CodeCompletionDescription(getDescriptionFunc());
            return result;
        }

        private ImageSource GetImage()
        {
            var tag = tags.FirstOrDefault();
            if (tag == null) { return null; }
            
            switch (tag)
            {
                case CompletionTags.Class:
                    return GetImage("ClassImageSource");
                case CompletionTags.Constant:
                    return GetImage("ConstantImageSource");
                case CompletionTags.Delegate:
                    return GetImage("DelegateImageSource");
                case CompletionTags.Enum:
                    return GetImage("EnumImageSource");
                case CompletionTags.EnumMember:
                    return GetImage("EnumItemImageSource");
                case CompletionTags.Event:
                    return GetImage("EventImageSource");    
                case CompletionTags.ExtensionMethod:
                    return GetImage("ExtensionMethodImageSource");
                case CompletionTags.Field:
                    return GetImage("FieldImageSource");
                case CompletionTags.Interface:
                    return GetImage("InterfaceImageSource");
                case CompletionTags.Keyword:
                    return GetImage("KeywordImageSource");
                case CompletionTags.Method:
                    return GetImage("MethodImageSource");
                case CompletionTags.Module:
                    return GetImage("ModuleImageSource");
                case CompletionTags.Namespace:
                    return GetImage("NamespaceImageSource");
                case CompletionTags.Property:
                    return GetImage("PropertyImageSource");
                case CompletionTags.Structure:
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
