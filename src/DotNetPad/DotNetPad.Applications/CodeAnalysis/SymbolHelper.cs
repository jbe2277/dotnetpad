using Microsoft.CodeAnalysis;
using System;

namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    public static class SymbolHelper
    {
        public static Glyph? GetGlyph(this ISymbol symbol)
        {
            Glyph glyph;
 
            switch (symbol.Kind)
            {
                case SymbolKind.Alias:
                    return ((IAliasSymbol)symbol).Target.GetGlyph();
 
                case SymbolKind.ArrayType:
                    return ((IArrayTypeSymbol)symbol).ElementType.GetGlyph();
 
                case SymbolKind.DynamicType:
                    return Glyph.Class;
 
                case SymbolKind.Event:
                    glyph = Glyph.Event;
                    break;
 
                case SymbolKind.Field:
                    var containingType = symbol.ContainingType;
                    if (containingType != null && containingType.TypeKind == TypeKind.Enum)
                    {
                        return Glyph.EnumMember;
                    }
 
                    glyph = ((IFieldSymbol)symbol).IsConst ? Glyph.Constant : Glyph.Field;
                    break;
 
                case SymbolKind.NamedType:
                case SymbolKind.ErrorType:
                    {
                        switch (((INamedTypeSymbol)symbol).TypeKind)
                        {
                            case TypeKind.Class:
                                glyph = Glyph.Class;
                                break;
 
                            case TypeKind.Delegate:
                                glyph = Glyph.Delegate;
                                break;
 
                            case TypeKind.Enum:
                                glyph = Glyph.Enum;
                                break;
 
                            case TypeKind.Interface:
                                glyph = Glyph.Interface;
                                break;

                            case TypeKind.Module:
                                glyph = Glyph.Module;
                                break;
 
                            case TypeKind.Struct:
                                glyph = Glyph.Structure;
                                break;
 
                            default:
                                return null;
                        }
 
                        break;
                    }
 
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;
 
                        if (methodSymbol.IsExtensionMethod || methodSymbol.MethodKind == MethodKind.ReducedExtension)
                        {
                            glyph = Glyph.ExtensionMethod;
                        }
                        else
                        {
                            glyph = Glyph.Method;
                        }
                    }
 
                    break;
 
                case SymbolKind.Namespace:
                    return Glyph.Namespace;
 
                case SymbolKind.Parameter:
                    return Glyph.Keyword;
 
                case SymbolKind.PointerType:
                    return ((IPointerTypeSymbol)symbol).PointedAtType.GetGlyph();
 
                case SymbolKind.Property:
                    {
                        var propertySymbol = (IPropertySymbol)symbol;
 
                        if (propertySymbol.IsWithEvents)
                        {
                            glyph = Glyph.Field;
                        }
                        else
                        {
                            glyph = Glyph.Property;
                        }
                    }
 
                    break;
 
                default:
                    return null;
            }
 
            return glyph;
        }
    }
}
