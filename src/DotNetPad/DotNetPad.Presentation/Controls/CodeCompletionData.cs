using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Tags;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeCompletionData : ICompletionData
    {
        private readonly Lazy<object> description;
        private readonly Func<Task<ImmutableArray<TaggedText>>> getDescriptionFunc;
        private readonly ImmutableArray<string> tags;
        private readonly Lazy<ImageSource?> image;

        public CodeCompletionData(string text, Func<Task<ImmutableArray<TaggedText>>> getDescriptionFunc, ImmutableArray<string> tags)
        {
            Text = text;
            description = new Lazy<object>(CreateDescription);
            this.getDescriptionFunc = getDescriptionFunc;
            this.tags = tags;
            image = new Lazy<ImageSource?>(GetImage);
        }

        public double Priority => 0;

        public string Text { get; }

        public object Description => description.Value;

        public object Content => Text;

        public ImageSource? Image => image.Value;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs) => textArea.Document.Replace(completionSegment, Text);

        private object CreateDescription() => new CodeCompletionDescription(getDescriptionFunc());

        private ImageSource? GetImage()
        {
            var tag = tags.FirstOrDefault();
            return tag switch
            {
                WellKnownTags.Class => GetImage("ClassImageSource"),
                WellKnownTags.Constant => GetImage("ConstantImageSource"),
                WellKnownTags.Delegate => GetImage("DelegateImageSource"),
                WellKnownTags.Enum => GetImage("EnumImageSource"),
                WellKnownTags.EnumMember => GetImage("EnumItemImageSource"),
                WellKnownTags.Event => GetImage("EventImageSource"),
                WellKnownTags.ExtensionMethod => GetImage("ExtensionMethodImageSource"),
                WellKnownTags.Field => GetImage("FieldImageSource"),
                WellKnownTags.Interface => GetImage("InterfaceImageSource"),
                WellKnownTags.Keyword => GetImage("KeywordImageSource"),
                WellKnownTags.Method => GetImage("MethodImageSource"),
                WellKnownTags.Module => GetImage("ModuleImageSource"),
                WellKnownTags.Namespace => GetImage("NamespaceImageSource"),
                WellKnownTags.Property => GetImage("PropertyImageSource"),
                WellKnownTags.Structure => GetImage("StructureImageSource"),
                _ => null,
            };
        }

        private static ImageSource GetImage(string resourceKey) => (ImageSource)Application.Current.Resources[resourceKey];
    }
}
