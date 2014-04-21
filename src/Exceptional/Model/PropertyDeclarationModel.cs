// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class PropertyDeclarationModel : AnalyzeUnitModelBase<IPropertyDeclaration>
    {
        public List<AccessorDeclarationModel> Accessors { get; private set; }

        public PropertyDeclarationModel(IPropertyDeclaration node)
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
                foreach (var accessorDeclarationModel in Accessors)
                {
                    foreach (var thrownExceptionModel in accessorDeclarationModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return thrownExceptionModel;
                    }
                }
            }
        }

        public override IBlock Contents
        {
            get { return null; }
        }
    }
}