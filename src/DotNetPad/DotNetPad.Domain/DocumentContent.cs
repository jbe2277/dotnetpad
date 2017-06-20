using System.Collections.Generic;
using System.Waf.Foundation;

namespace Waf.DotNetPad.Domain
{
    public class DocumentContent : Model
    {
        private string code;
        private IReadOnlyList<ErrorListItem> errorList;


        public DocumentContent()
        {
            errorList = new ErrorListItem[0];
        }
        

        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        public IReadOnlyList<ErrorListItem> ErrorList
        {
            get { return errorList; }
            set { SetProperty(ref errorList, value); }
        }
    }
}
