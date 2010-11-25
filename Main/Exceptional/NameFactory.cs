// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>Aids in creating names for code elements.</summary>
    public static class NameFactory
    {
        public static string CatchVariableName(ITreeNode treeNode, IDeclaredType exceptionType)
        {
            //TODO: japf warning: this code is not completly ported to R# 5.0...
            //var namesCollection = NameSuggestionUtil.CreateEmptyCollection(treeNode.Language, PluralityKinds.Single, true, treeNode.GetSolution());
            //namesCollection.Add(exceptionType, PluralityKinds.Single);
            //var roots = new List<NameRoot>(namesCollection.GetRoots("e", PredefinedPrefixPolicy.Preserve));

            //var namingRule = NamingManager.GetRulesProvider(treeNode.Language)
            //    .GetPolicy(NamedElementKinds.Locals, treeNode.GetSolution()).NamingRule;
            //return NamingManager.GetUniqueShortName(roots, treeNode, ScopeKind.LocalSelfScoped, namingRule);

                        var nameSuggestionManager = new NameSuggestionManager(treeNode.GetSolution());
            var namingManager = new NamingManager(treeNode.GetSolution());

            var namesCollection = nameSuggestionManager.CreateEmptyCollection(
                PluralityKinds.Single,
                treeNode.Language,
                true);

            var entryOptions = new EntryOptions
                {
                    PluralityKind = PluralityKinds.Single, 
                    PredefinedPrefixPolicy = PredefinedPrefixPolicy.Preserve,
                };

            namesCollection.Add(exceptionType, entryOptions);

            //TODO: japf: i don't find out yet how to endup this method...
            return string.Empty;
        }
    }
}