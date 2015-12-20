namespace Waf.DotNetPad.Domain
{
    public class ErrorListItem
    {
        public ErrorListItem(ErrorSeverity errorSeverity, string description, int startLine, int startColumn, int endLine, int endColumn)
        {
            ErrorSeverity = errorSeverity;
            Description = description;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }


        public ErrorSeverity ErrorSeverity { get; }

        public string Description { get; }

        public int StartLine { get; }

        public int StartColumn { get; }

        public int EndLine { get; }

        public int EndColumn { get; }
    }
}
