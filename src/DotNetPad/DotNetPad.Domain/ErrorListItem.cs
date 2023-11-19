namespace Waf.DotNetPad.Domain;

public record ErrorListItem(ErrorSeverity ErrorSeverity, string Description, int StartLine, int StartColumn, int EndLine, int EndColumn);