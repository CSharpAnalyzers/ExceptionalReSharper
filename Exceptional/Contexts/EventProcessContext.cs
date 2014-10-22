using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Contexts
{
    internal class EventProcessContext : ProcessContext<EventDeclarationModel>
    {
        public override void EnterAccessor(IAccessorDeclaration accessorDeclarationNode)
        {
            if (IsValid() == false)
                return;

            if (accessorDeclarationNode == null)
                return;

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