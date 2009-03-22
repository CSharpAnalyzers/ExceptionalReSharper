/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class AnalyzeUnitModelBase<T> : BlockModelBase<T>, IAnalyzeUnit where T : ITreeNode
    {
        public DocCommentBlockModel DocCommentBlockModel { get; set; }

        public bool IsPublicOrInternal
        {
            get
            {
                var accessRightsOwner = this.Node as IAccessRightsOwner;
                if (accessRightsOwner == null)
                {
                    return false;
                }

                var rights = accessRightsOwner.GetAccessRights();
                return rights == AccessRights.PUBLIC ||
                       rights == AccessRights.INTERNAL ||
                       rights == AccessRights.PROTECTED;
            }
        }

        protected AnalyzeUnitModelBase(IAnalyzeUnit analyzeUnit, T node)
            : base(analyzeUnit, node)
        {
            DocCommentBlockModel = new DocCommentBlockModel(this);
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            if (this.DocCommentBlockModel != null)
            {
                this.DocCommentBlockModel.Accept(analyzerBase);
            }

            base.Accept(analyzerBase);
        }

        public IDocCommentBlockNode AddDocCommentNode(IDocCommentBlockNode docCommentBlockNode)
        {
            return ModificationUtil.AddChildBefore(this.Node, this.Node.FirstChild, docCommentBlockNode);
        }

        public IPsiModule GetPsiModule()
        {
            return this.Node.GetPsiModule();
        }
    }
}