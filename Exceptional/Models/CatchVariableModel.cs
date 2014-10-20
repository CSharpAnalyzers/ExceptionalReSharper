using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional.Models
{
    internal class CatchVariableModel : TreeElementModelBase<ICatchVariableDeclaration>
    {
        public CatchVariableModel(IAnalyzeUnit analyzeUnit, ICatchVariableDeclaration catchVariableDeclaration)
            : base(analyzeUnit, catchVariableDeclaration)
        {
        }

        public ICSharpIdentifier VariableName
        {
            get { return Node.NameIdentifier; }
        }
    }
}