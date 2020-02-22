using System;
using System.Linq;
using JetBrains.Application.Shell;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Exceptional.Utilities
{
    /// <summary>Aids in creating names for code elements.</summary>
    public static class NameFactory
    {
        public static string CatchVariableName(ITreeNode treeNode, IDeclaredType exceptionType)
        {
            var namingPolicyManager = new NamingPolicyManager(LanguageManager.Instance, treeNode.GetSolution());
            var nameParser = new NameParser(treeNode.GetSolution(), namingPolicyManager, new HostCulture());
            var nameSuggestionManager = new NameSuggestionManager(treeNode.GetSolution(), nameParser, namingPolicyManager);
            var policy = namingPolicyManager.GetPolicy(NamedElementKinds.Locals, treeNode.Language, treeNode.GetSourceFile());

            var namesCollection = nameSuggestionManager.CreateEmptyCollection(
                PluralityKinds.Single, treeNode.Language, true, treeNode.GetSourceFile());

            var entryOptions = new EntryOptions
            {
                PluralityKind = PluralityKinds.Single,
                PredefinedPrefixPolicy = PredefinedPrefixPolicy.Preserve,
                Emphasis = Emphasis.Good,
                SubrootPolicy = SubrootPolicy.Decompose
            };

            namesCollection.Add(exceptionType, entryOptions);
            namesCollection.Prepare(policy.NamingRule, ScopeKind.Common, new SuggestionOptions());

            try
            {
                return namesCollection.GetRoots().FirstOrDefault()?.GetFinalPresentation() ?? String.Empty;
            }
            catch (ArgumentNullException)
            {
                return String.Empty;
            }
        }
    }
}