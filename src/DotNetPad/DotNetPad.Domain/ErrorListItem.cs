namespace Waf.DotNetPad.Domain
{
    public class ErrorListItem
    {
        private readonly ErrorSeverity errorSeverity;
        private readonly string description;
        private readonly int startLine;
        private readonly int startColumn;
        private readonly int endLine;
        private readonly int endColumn;


        public ErrorListItem(ErrorSeverity errorSeverity, string description, int startLine, int startColumn, int endLine, int endColumn)
        {
            this.errorSeverity = errorSeverity;
            this.description = description;
            this.startLine = startLine;
            this.startColumn = startColumn;
            this.endLine = endLine;
            this.endColumn = endColumn;
        }


        public ErrorSeverity ErrorSeverity { get { return errorSeverity; } }

        public string Description { get { return description; } }

        public int StartLine { get { return startLine; } }

        public int StartColumn { get { return startColumn; } }

        public int EndLine { get { return endLine; } }

        public int EndColumn { get { return endColumn; } }
    }
}
