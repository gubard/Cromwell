using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cromwell.SourceGenerator;

public static class Extensions
{
    public static TypeSyntax GetAttributeValueType(this AttributeSyntax syntax, int argumentIndex)
    {
        return syntax.ArgumentList?.Arguments[argumentIndex].Expression switch
        {
            TypeOfExpressionSyntax type => type.Type,
            { } e => throw new($"Unknown type {e.GetType()}"),
        };
    }

    public static string GetNamespace<T>(this T syntax) where T : SyntaxNode
    {
        return syntax.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().First().Name.ToString();
    }

    public static string GetName<T>(this T syntax) where T : BaseTypeDeclarationSyntax
    {
        return syntax.Identifier.Text;
    }

    public static string GetFullName<T>(this T syntax) where T : BaseTypeDeclarationSyntax
    {
        return $"{syntax.GetNamespace()}.{syntax.GetName()}";
    }

    public static string GetName(this TypeSyntax syntax)
    {
        return syntax.ToString();
    }

    public static string GetFullName(this TypeSyntax syntax, Compilation compilation)
    {
        return $"{syntax.GetNamespace(compilation)}.{syntax.GetName()}";
    }

    public static string GetNamespace(this TypeSyntax typeSyntax, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(typeSyntax.SyntaxTree);
        var symbolInfo = semanticModel.GetSymbolInfo(typeSyntax);
        var symbol = symbolInfo.Symbol;

        if (symbol == null)
        {
            return string.Empty;
        }

        // For a type symbol, get its containing namespace
        if (symbol is ITypeSymbol typeSymbol)
        {
            return typeSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        }

        // For other symbols (methods, properties, etc.), get the containing type's namespace
        var containingType = symbol.ContainingType;

        if (containingType != null)
        {
            return containingType.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        }

        // For namespace symbols or other symbols without a containing type
        var containingNamespace = symbol.ContainingNamespace;

        return containingNamespace?.ToDisplayString() ?? string.Empty;
    }
}