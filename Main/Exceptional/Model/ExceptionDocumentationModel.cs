using System;
using System.Text.RegularExpressions;
using System.Xml;
using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ExceptionDocumentationModel : ModelBase
    {
        private readonly ICSharpCommentNode _;
        public DocCommentBlockModel DocCommentBlockModel { get; private set; }
        public ICSharpCommentNode CommentNode { get; set; }

        /// <summary>Exception documentation xml node.</summary>
        //public XmlNode ExceptionNode { get; private set; }

        /// <summary>Exception type.</summary>
        public IDeclaredType ExceptionType { get; private set; }

        /// <summary>Exception documentation tab <see cref="DocumentRange"/>.</summary>
        public DocumentRange DocumentRange
        {
            get { return this.CommentNode.GetDocumentRange(); }
        }

        /// <summary>States that documented exception is actually thrown from documented method.</summary>
        public bool IsThrown { get; set; }

        public ExceptionDocumentationModel(MethodDeclarationModel methodDeclarationModel, ICSharpCommentNode commentNode)
            : base(methodDeclarationModel)
        {
            DocCommentBlockModel = methodDeclarationModel.DocCommentBlockModel;
            CommentNode = commentNode;
            IsThrown = false;

            InitializeException();
            //InitializeDocumentRange();
        }

//        private void InitializeDocumentRange()
//        {
//            if(this.ExceptionType != null)
//            {
//                this.DocumentRange = XmlDocCommentHelper.FindRangeOfExceptionTag(this.ExceptionNode, this.ExceptionType.GetCLRName(), this.DocCommentBlockModel.DocCommentNode);
//            }
//            else
//            {
//                this.DocumentRange = DocumentRange.InvalidRange;
//            }
//        }

        private void InitializeException()
        {
            var regEx = new Regex("cref=\"(?<exception>[^\"]+)\"");
            var match = regEx.Match(this.CommentNode.CommentText);

            var exceptionType = match.Groups["exception"].Value;

            if (exceptionType.StartsWith("T:") || exceptionType.StartsWith("!:"))
            {
                exceptionType = exceptionType.Substring(2);
            }

            this.ExceptionType = GetType(exceptionType);
        }

        private IDeclaredType GetType(string exceptionType)
        {
            var solution = this.DocCommentBlockModel.DocCommentNode.GetProject().GetSolution();
            return TypesFactory.CreateDeclaredType(solution, exceptionType);
        }

        #region Assings & Accepts

        public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if(this.IsThrown == false)
            {
                process.AddHighlighting(this.DocumentRange, new ExceptionNotThrownHighlighting(this));
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }
        #endregion

        public void Remove()
        {
            this.DocCommentBlockModel.Remove(this);
        }
    }
}