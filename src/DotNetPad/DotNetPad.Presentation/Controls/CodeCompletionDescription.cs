using System.Threading.Tasks;
using System.Waf.Foundation;

namespace Waf.DotNetPad.Presentation.Controls
{
    public class CodeCompletionDescription : Model
    {
        private string summary;
        

        public CodeCompletionDescription(Task<string> lazySummary)
        {
            UpdateSummary(lazySummary);
        }


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
