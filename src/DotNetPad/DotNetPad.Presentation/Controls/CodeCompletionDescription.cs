using System.Threading.Tasks;
using System.Waf.Foundation;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeCompletionDescription : Model
    {
        private readonly string overloads;
        private string summary;
        

        public CodeCompletionDescription(Task<string> lazySummary, string overloads)
        {
            this.overloads = overloads;
            UpdateSummary(lazySummary);
        }


        public string Overloads { get { return overloads; } }

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
