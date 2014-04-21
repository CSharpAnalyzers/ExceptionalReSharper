// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.

using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Naming.Interfaces;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>Aids in creating names for code elements.</summary>
    public static class NameFactory
    {
        public static string CatchVariableName(ITreeNode treeNode, IDeclaredType exceptionType)
        {
            NamingPolicyManager namingPolicyManager = new NamingPolicyManager(LanguageManager.Instance, treeNode.GetSolution());
            NameParser nameParser = new NameParser(treeNode.GetSolution(), namingPolicyManager);
            NameSuggestionManager nameSuggestionManager = new NameSuggestionManager(treeNode.GetSolution(), nameParser, namingPolicyManager);
            NamingPolicy policy = namingPolicyManager.GetPolicy(NamedElementKinds.Locals, treeNode.Language, treeNode.GetSourceFile());

            INamesCollection namesCollection = nameSuggestionManager.CreateEmptyCollection(
                PluralityKinds.Single,
                treeNode.Language,
                true,
                treeNode.GetSourceFile());

            EntryOptions entryOptions = new EntryOptions
                {
                    PluralityKind = PluralityKinds.Single,
                    PredefinedPrefixPolicy = PredefinedPrefixPolicy.Preserve,
                    Emphasis = Emphasis.Good,
                    SubrootPolicy = SubrootPolicy.Decompose
                };

            namesCollection.Add(exceptionType, entryOptions);

            namesCollection.Prepare(policy.NamingRule, ScopeKind.Common, new SuggestionOptions());

            return namesCollection.FirstName();
        }
    }

    internal class UnigueNamesService : IUnigueNamesService
    {
        public bool IsUnique(string name, ITreeNode context, ScopeKind kind)
        {
            return GetConflictedElements(name, context, kind).Count == 0;
        }

        public IList<IDeclaredElement> GetConflictedElements(string name, ITreeNode context, ScopeKind kind)
        {
            return NamingManager.GetNamingLanguageService(context.Language).GetConflictedElements(name, context, kind);
        }
    }

}