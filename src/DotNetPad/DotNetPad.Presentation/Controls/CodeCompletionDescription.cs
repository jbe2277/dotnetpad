using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Waf.Foundation;
using Microsoft.CodeAnalysis;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeCompletionDescription : Model
    {
        private ImmutableArray<TaggedText> summary;


        public CodeCompletionDescription(Task<ImmutableArray<TaggedText>> lazySummary)
        {
            UpdateSummary(lazySummary);
        }


        public ImmutableArray<TaggedText> Summary
        {
            get { return summary; }
            set { SetProperty(ref summary, value); }
        }


        private async void UpdateSummary(Task<ImmutableArray<TaggedText>> lazySummary)
        {
            Summary = await lazySummary;
        }
    }
}
