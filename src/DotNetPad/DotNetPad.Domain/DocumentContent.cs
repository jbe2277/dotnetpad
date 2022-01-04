namespace Waf.DotNetPad.Domain;

public class DocumentContent : Model
{
    private string code = "";
    private IReadOnlyList<ErrorListItem> errorList = Array.Empty<ErrorListItem>();

    public string Code
    {
        get => code;
        set => SetProperty(ref code, value);
    }

    public IReadOnlyList<ErrorListItem> ErrorList
    {
        get => errorList;
        set => SetProperty(ref errorList, value);
    }
}
