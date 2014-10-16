using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional.Models
{
    internal class CatchVariableModel : TreeElementModelBase<ICatchVariableDeclaration>
    {
        public ICSharpIdentifier VariableName
        {
            get { return Node.NameIdentifier; }
        }

        public CatchVariableModel(IAnalyzeUnit analyzeUnit, ICatchVariableDeclaration catchVariableDeclaration)
            : base(analyzeUnit, catchVariableDeclaration)
        {
        }
    }
}