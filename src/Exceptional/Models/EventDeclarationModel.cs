using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    internal class EventDeclarationModel : AnalyzeUnitModelBase<IEventDeclaration>
    {
        public EventDeclarationModel(IEventDeclaration node)
            : base(null, node)
        {
            Accessors = new List<AccessorDeclarationModel>();
        }

        public List<AccessorDeclarationModel> Accessors { get; private set; }

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var accessorDeclarationModel in Accessors)
                accessorDeclarationModel.Accept(analyzer);

            base.Accept(analyzer);
        }

        /// <summary>Gets the list of not caught thrown exceptions. </summary>
        public override IEnumerable<ThrownExceptionModel> UncaughtThrownExceptions
        {
            get { return Accessors.SelectMany(m => m.UncaughtThrownExceptions); }
        }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content
        {
            get { return null; }
        }
    }
}