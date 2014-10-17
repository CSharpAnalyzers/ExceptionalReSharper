using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Naming.Interfaces;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Exceptional.Utilities
{
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