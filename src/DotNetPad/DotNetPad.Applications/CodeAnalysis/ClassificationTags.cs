using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using System;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    public static class ClassificationTags
    {
        public static string GetClassificationTypeName(string textTag) => textTag switch
        {
            TextTags.Keyword => ClassificationTypeNames.Keyword,
            TextTags.Class => ClassificationTypeNames.ClassName,
            TextTags.Delegate => ClassificationTypeNames.DelegateName,
            TextTags.Enum => ClassificationTypeNames.EnumName,
            TextTags.EnumMember => ClassificationTypeNames.EnumMemberName,
            TextTags.Interface => ClassificationTypeNames.InterfaceName,
            TextTags.Module => ClassificationTypeNames.ModuleName,
            TextTags.Struct => ClassificationTypeNames.StructName,
            TextTags.TypeParameter => ClassificationTypeNames.TypeParameterName,
            TextTags.ExtensionMethod => ClassificationTypeNames.ExtensionMethodName,
            TextTags.NumericLiteral => ClassificationTypeNames.NumericLiteral,
            TextTags.StringLiteral => ClassificationTypeNames.StringLiteral,
            TextTags.Operator => ClassificationTypeNames.Operator,
            TextTags.Punctuation => ClassificationTypeNames.Punctuation,
            TextTags.Constant => ClassificationTypeNames.ConstantName,
            TextTags.Alias or TextTags.Assembly or TextTags.Field or TextTags.ErrorType or TextTags.Event or TextTags.Label or TextTags.Local or TextTags.Method or TextTags.Namespace or TextTags.Parameter or TextTags.Property or TextTags.RangeVariable => ClassificationTypeNames.Identifier,
            TextTags.Space or TextTags.LineBreak => ClassificationTypeNames.WhiteSpace,
            TextTags.AnonymousTypeIndicator or TextTags.Text => ClassificationTypeNames.Text,
            _ => throw new NotSupportedException(textTag),
        };
    }
}