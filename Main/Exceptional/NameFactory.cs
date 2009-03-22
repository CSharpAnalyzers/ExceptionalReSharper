/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>Aids in creating names for code elements.</summary>
    public static class NameFactory
    {
        public static string CatchVariableName(ITreeNode treeNode, IDeclaredType exceptionType)
        {
            var namesCollection = NameSuggestionUtil.CreateEmptyCollection(treeNode.Language, PluralityKinds.Single, true, treeNode.GetSolution());
            namesCollection.Add(exceptionType, PluralityKinds.Single);
            var roots = new List<NameRoot>(namesCollection.GetRoots("e", PredefinedPrefixPolicy.Preserve));

            var namingRule = NamingManager.GetRulesProvider(treeNode.Language)
                .GetPolicy(NamedElementKinds.Locals, treeNode.GetSolution()).NamingRule;
            return NamingManager.GetUniqueShortName(roots, treeNode, ScopeKind.LocalSelfScoped, namingRule);
        }
    }
}