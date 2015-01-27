using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    internal class AccessorOwnerDeclarationModel : AnalyzeUnitModelBase<IAccessorOwnerDeclaration>
    {
        public AccessorOwnerDeclarationModel(IAccessorOwnerDeclaration node)
            : base(null, node)
        {
            Accessors = new List<AccessorDeclarationModel>();
        }

        public List<AccessorDeclarationModel> Accessors { get; private set; }

        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var accessor in Accessors)
                accessor.Accept(analyzer);

            base.Accept(analyzer);
        }

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