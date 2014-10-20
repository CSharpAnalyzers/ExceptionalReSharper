using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>. </summary>
    internal class DocCommentBlockModel : TreeElementModelBase<IDocCommentBlockNode>
    {
        private List<DocCommentModel> DocCommentModels { get; set; }

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit)
            : this(analyzeUnit, null)
        {
        }

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit, IDocCommentBlockNode docCommentNode)
            : base(analyzeUnit, docCommentNode)
        {
            DocCommentModels = new List<DocCommentModel>();
            References = new List<IReference>();

            Initialize();
        }

        public List<IReference> References { get; private set; }

        private bool IsReal
        {
            get { return Node != null; }
        }

        public IEnumerable<ExceptionDocCommentModel> ExceptionDocCommentModels
        {
            get { return DocCommentModels.OfType<ExceptionDocCommentModel>(); }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var docCommentModel in DocCommentModels)
                docCommentModel.Accept(analyzerBase);
        }

        public ExceptionDocCommentModel AddExceptionDocumentation(IDeclaredType exceptionType, string exceptionDescription, IProgressIndicator progress)
        {
            if (exceptionType == null) 
              return null;

            Shell.Instance.GetComponent<IShellLocks>().AcquireWriteLock();
            try
            {
                var exceptionDocumentation = string.IsNullOrEmpty(exceptionDescription)
                    ? string.Format("<exception cref=\"{0}\">[MARKER]. </exception>{1}", exceptionType.GetClrName().ShortName, Environment.NewLine)
                    : string.Format("<exception cref=\"{0}\">{1}</exception>{2}", exceptionType.GetClrName().ShortName, exceptionDescription, Environment.NewLine);

                ExceptionDocCommentModel result;
                if (IsReal == false)
                {
                    var docCommentNode = GetElementFactory().CreateDocCommentBlock(exceptionDocumentation);

                    docCommentNode = AnalyzeUnit.AddDocCommentNode(docCommentNode);

                    Node = docCommentNode;
                    Initialize();

                    Node.FormatNode(progress);

                    result = DocCommentModels[1] as ExceptionDocCommentModel;
                }
                else
                {
                    var docCommentBlockNode = GetElementFactory().CreateDocComment(" " + exceptionDocumentation);
                    var commentNode = docCommentBlockNode;

                    var newLineNode = GetElementFactory().CreateWhitespaces(Environment.NewLine);
                    if (Node.LastChild != null)
                        LowLevelModificationUtil.AddChildAfter(Node.LastChild, newLineNode[0], commentNode);
                    else
                        LowLevelModificationUtil.AddChild(Node, newLineNode[0], commentNode);

                    Node.FormatNode(progress);

                    result = new ExceptionDocCommentModel(this);
                    result.TreeNodes.Add(commentNode);
                    result.Initialize();

                    DocCommentModels.Add(result);
                }
                return result;
            }
            finally
            {
                Shell.Instance.GetComponent<IShellLocks>().ReleaseWriteLock();
            }
        }

        public void RemoveExceptionDocumentation(ExceptionDocCommentModel exceptionDocCommentModel, IProgressIndicator progress)
        {
            if (exceptionDocCommentModel == null)
                return;

            if (exceptionDocCommentModel.DocCommentNodes.Count == 0) 
                return;

            var firstNode = exceptionDocCommentModel.TreeNodes[0];
            var lastNode = exceptionDocCommentModel.TreeNodes[exceptionDocCommentModel.TreeNodes.Count - 1];

            LowLevelModificationUtil.DeleteChildRange(firstNode, lastNode);
        }

        private void Initialize()
        {
            if (Node == null) 
                return;

            References.AddRange(Node.GetFirstClassReferences());
            DocCommentModels = DocCommentReader.Read(Node, this);
        }
    }
}