// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>
    /// Stores data about processed <see cref="IDocCommentBlockNode"/>.
    /// </summary>
    internal class DocCommentBlockModel : TreeElementModelBase<IDocCommentBlockNode>
    {
        private List<DocCommentModel> DocCommentModels { get; set; }
        public List<IReference> References { get; private set; }

        private bool IsReal
        {
            get { return Node != null; }
        }

        public IEnumerable<ExceptionDocCommentModel> ExceptionDocCommentModels
        {
            get
            {
                foreach (var docCommentModel in DocCommentModels)
                {
                    if (docCommentModel is ExceptionDocCommentModel)
                    {
                        yield return docCommentModel as ExceptionDocCommentModel;
                    }
                }
            }
        }

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit)
            : this(analyzeUnit, null)
        {
        }

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit, IDocCommentBlockNode docCommentNode = null)
            : base(analyzeUnit, docCommentNode)
        {
            DocCommentModels = new List<DocCommentModel>();
            References = new List<IReference>();

            Preprocess();
        }

        private void Preprocess()
        {
            if (Node == null) return;

            References.AddRange(Node.GetFirstClassReferences());
            DocCommentModels = DocCommentReader.Read(Node, this);
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var docCommentModel in DocCommentModels)
            {
                docCommentModel.Accept(analyzerBase);
            }
        }

        public ExceptionDocCommentModel AddExceptionDocumentation(IDeclaredType exceptionType)
        {
            if (exceptionType == null) return null;

            Shell.Instance.GetComponent<IShellLocks>().AcquireWriteLock();

            ExceptionDocCommentModel result;

            var exceptionDocumentation = string.Format(" <exception cref=\"{1}\">[MARKER]</exception>{0}",
                                                       Environment.NewLine, exceptionType.GetClrName().ShortName);

            if (IsReal == false)
            {
                var docCommentNode = GetElementFactory().CreateDocCommentBlock(exceptionDocumentation);
                docCommentNode = AnalyzeUnit.AddDocCommentNode(docCommentNode);
                Node = docCommentNode;
                Preprocess();
                //TODO: japf warning: i don't know how to use the CSharpCodeFormatter
                //CSharpCodeFormatter.Instance.Format(Node, CodeFormatProfile.INDENT);

                result = DocCommentModels[1] as ExceptionDocCommentModel;
            }
            else
            {
                //exceptionDocumentation += exceptionDocumentation;
                var docCommentBlockNode = GetElementFactory().CreateDocComment(exceptionDocumentation);

                var commentNode = docCommentBlockNode;
                //var commentNode = docCommentBlockNode.FirstChild as IDocCommentNode;

                //if (commentNode == null) return null;

                var spaces1 = GetElementFactory().CreateWhitespaces(Environment.NewLine);
                var spaces2 = GetElementFactory().CreateWhitespaces(Environment.NewLine);

                if (Node.LastChild != null)
                {
                    LowLevelModificationUtil.AddChildAfter(Node.LastChild, spaces1[0], commentNode, spaces2[0]);
                }
                else
                {
                    LowLevelModificationUtil.AddChild(Node, spaces2[1], commentNode, spaces2[0]);
                }

                //TODO: japf warning: i don't know how to use the CSharpCodeFormatter
                //LanguageService.GetInstance(AnalyzeUnit.GetPsiModule().PsiLanguage).CodeFormatter.Format(Node, CodeFormatProfile.INDENT);
                //CSharpCodeFormatter.Instance.Format(Node, CodeFormatProfile.INDENT);

                result = new ExceptionDocCommentModel(this);
                result.TreeNodes.Add(commentNode);
                result.Initialize();
                DocCommentModels.Add(result);
            }

            Shell.Instance.GetComponent<IShellLocks>().ReleaseWriteLock();

            return result;
        }

        public void RemoveExceptionDocumentation(ExceptionDocCommentModel exceptionDocCommentModel)
        {
            if (exceptionDocCommentModel == null) return;
            if (exceptionDocCommentModel.DocCommentNodes.Count == 0) return;

            var firstNode = exceptionDocCommentModel.TreeNodes[0];
            var lastNode = exceptionDocCommentModel.TreeNodes[exceptionDocCommentModel.TreeNodes.Count - 1];

            LowLevelModificationUtil.DeleteChildRange(firstNode, lastNode);
            //TODO: japf warning: i don't know how to use the CSharpCodeFormatter
            //CSharpCodeFormatter.Instance.Format(Node, CodeFormatProfile.SOFT);
        }
    }
}