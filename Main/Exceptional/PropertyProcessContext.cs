// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    internal class PropertyProcessContext : ProcessContext<PropertyDeclarationModel>
    {
        public override void EnterAccessor(IAccessorDeclaration accessorDeclarationNode)
        {
            if (IsValid() == false) return;
            if (accessorDeclarationNode == null) return;

            var parent = BlockModelsStack.Peek();

            var model = new AccessorDeclarationModel(AnalyzeUnit, accessorDeclarationNode);
            model.ParentBlock = parent;
            Model.Accessors.Add(model);
            BlockModelsStack.Push(model);
        }

        public override void LeaveAccessor()
        {
            BlockModelsStack.Pop();
        }
    }
}