using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    internal class PropertyDeclarationModel : AnalyzeUnitModelBase<IPropertyDeclaration>
    {
        public List<AccessorDeclarationModel> Accessors { get; private set; }

        public PropertyDeclarationModel(IPropertyDeclaration node, ExceptionalSettings settings)
            : base(null, node, settings)
        {
            Accessors = new List<AccessorDeclarationModel>();
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var accessorDeclarationModel in Accessors)
                accessorDeclarationModel.Accept(analyzerBase);

            base.Accept(analyzerBase);
        }

        public override IEnumerable<ThrownExceptionModel> NotCaughtThrownExceptions
        {
            get { return Accessors.SelectMany(m => m.NotCaughtThrownExceptions); }
        }

        public override IBlock Contents
        {
            get { return null; }
        }
    }
}