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

        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var accessorDeclarationModel in Accessors)
                accessorDeclarationModel.Accept(analyzer);

            base.Accept(analyzer);
        }

        public override IEnumerable<ThrownExceptionModel> UncaughtThrownExceptions
        {
            get { return Accessors.SelectMany(m => m.UncaughtThrownExceptions); }
        }

        public override IBlock Contents
        {
            get { return null; }
        }
    }
}