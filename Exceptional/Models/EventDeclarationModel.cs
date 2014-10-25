using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    internal class EventDeclarationModel : AnalyzeUnitModelBase<IEventDeclaration>
    {
        public List<AccessorDeclarationModel> Accessors { get; private set; }

        public EventDeclarationModel(IEventDeclaration node, ExceptionalSettings settings)
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

        public override IBlock Content
        {
            get { return null; }
        }
    }
}