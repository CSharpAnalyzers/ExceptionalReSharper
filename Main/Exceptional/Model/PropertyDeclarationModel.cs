/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class PropertyDeclarationModel : AnalyzeUnitModelBase<IPropertyDeclarationNode>
    {
        public List<AccessorDeclarationModel> Accessors { get; private set; }

        public PropertyDeclarationModel(IPropertyDeclarationNode node)
            : base(null, node)
        {
            Accessors = new List<AccessorDeclarationModel>();
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var accessorDeclarationModel in Accessors)
            {
                accessorDeclarationModel.Accept(analyzerBase);
            }

            base.Accept(analyzerBase);
        }

        public override IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched
        {
            get
            {
                foreach (var accessorDeclarationModel in this.Accessors)
                {
                    foreach (var thrownExceptionModel in accessorDeclarationModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return thrownExceptionModel;
                    }
                }
            }
        }
    }
}