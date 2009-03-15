using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;

namespace CodeGears.ReSharper.Exceptional
{
    //internal static class TypesFactory
    //{
    //    public static IDeclaredType CreateDeclaredType(ISolution solution, string typeName)
    //    {
    //        var cache = PsiManager.GetInstance(solution).GetDeclarationsCache(DeclarationsCacheScope.SolutionScope(solution, true), true);
    //        var typeElement = cache.GetTypeElementByCLRName(typeName);
    //        if (typeElement == null) return null;

    //        return TypeFactory.CreateType(typeElement);
    //    }
    //}
}