using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class CatchVariableModel : ModelBase
    {
        public ICatchVariableDeclaration CatchVariableDeclaration { get; set; }

        private ICatchVariableDeclarationNode CatchVariableDeclarationNode
        {
            get { return this.CatchVariableDeclaration as ICatchVariableDeclarationNode; }
        }

        public ICSharpIdentifierNode VariableName
        {
            get { return this.CatchVariableDeclarationNode.Name; }
        }

        public CatchVariableModel(ICatchVariableDeclaration catchVariableDeclaration)
        {
            CatchVariableDeclaration = catchVariableDeclaration;
        }
    }
}