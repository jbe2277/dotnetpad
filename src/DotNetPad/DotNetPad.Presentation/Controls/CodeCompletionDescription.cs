using System.Threading.Tasks;
using System.Waf.Foundation;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeCompletionDescription : Model
    {
        private string summary;
        

        public CodeCompletionDescription(Task<string> lazySummary, string overloads)
        {
            Overloads = overloads;
            UpdateSummary(lazySummary);
        }


        public string Overloads { get; }

        public string Summary
        {
            get { return summary; }
            set { SetProperty(ref summary, value); }
        }


        private async void UpdateSummary(Task<string> lazySummary)
        {
            Summary = await lazySummary;
        }
    }
}
