/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    internal class PropertyProcessContext : ProcessContext<PropertyDeclarationModel>
    {
        public override void EnterAccessor(IAccessorDeclarationNode accessorDeclarationNode)
        {
            if (this.IsValid() == false)
            {
                return;
            }
            if (accessorDeclarationNode == null)
            {
                return;
            }

            var parent = this.BlockModelsStack.Peek();

            var model = new AccessorDeclarationModel(this.AnalyzeUnit, accessorDeclarationNode);
            model.ParentBlock = parent;
            this.Model.Accessors.Add(model);
            this.BlockModelsStack.Push(model);
        }

        public override void LeaveAccessor()
        {
            this.BlockModelsStack.Pop();
        }
    }
}