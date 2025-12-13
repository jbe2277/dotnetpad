namespace Waf.DotNetPad.Domain;

public class DocumentContent : Model
{
    public string Code { get; set => SetProperty(ref field, value); } = "";

    public IReadOnlyList<ErrorListItem> ErrorList { get; set => SetProperty(ref field, value); } = [];
}
