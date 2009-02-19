using System;
using System.Xml;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    public sealed class ExceptionsAnalyzer
    {
        private readonly CSharpErrorStageProcessBase _process;
        private MethodExceptionData _methodExceptionData;
        
        public ExceptionsAnalyzer(CSharpErrorStageProcessBase process)
        {
            this._process = process;
        }

        public void Process(IMethodDeclaration methodDeclaration)
        {
            var xmlNode = methodDeclaration.GetXMLDoc(false);
            if (xmlNode == null) return;
            
            var exceptionNodes = xmlNode.SelectNodes("exception");
            if(exceptionNodes == null || exceptionNodes.Count == 0) return;

            foreach (XmlNode exceptionNode in exceptionNodes)
            {
                var model = ExceptionDocCommentModel.Create(exceptionNode, methodDeclaration);
                if (model.IsValid == false) continue;

                this._methodExceptionData.AddDocumentedException(model);
            }
        }

        public void Process(IThrowStatement throwStatement)
        {
            var model = ThrowStatementModel.Create(throwStatement);
            if(model.IsValid == false) return;

            this._methodExceptionData.AddThrownException(model);
        }

        public void Process(ICatchVariableDeclaration catchVariableDeclaration)
        {
            
        }

        public void Process(ITryStatement tryStatement)
        {
        }

        public void Process(ISpecificCatchClause catchClause)
        {
            if (catchClause.ExceptionType == null || catchClause.ExceptionType.GetCLRName().Equals("System.Exception"))
            {
                var model = CatchAllClauseModel.Create(catchClause);
                this._methodExceptionData.AddModel(model);
            }


        }

        public void Process(IGeneralCatchClause catchClause)
        {
            if (catchClause.ExceptionType == null || catchClause.ExceptionType.GetCLRName().Equals("System.Exception"))
            {
                var model = CatchAllClauseModel.Create(catchClause);
                this._methodExceptionData.AddModel(model);
            }
        }

        public void BeginProcess(IMethodDeclaration methodDeclaration)
        {
            if(this._methodExceptionData != null)
                throw new InvalidOperationException("The analyzed method has not been computed.");

            this._methodExceptionData = new MethodExceptionData(methodDeclaration);
        }

        public void Compute(IMethodDeclaration methodDeclaration)
        {
            if (this._methodExceptionData == null)
                throw new InvalidOperationException("You have to first begin the process.");
            if (this._methodExceptionData.IsDefinedFor(methodDeclaration) == false)
                throw new InvalidOperationException("The given method does not match processed method.");

            foreach (var model in this._methodExceptionData.Result)
            {
                model.AssignHighlights(this._process);
            }

            this._methodExceptionData = null;
        }

        public void EnterTryBlock(ITryStatement tryStatement)
        {
            this._methodExceptionData.EnterTryBlock(tryStatement);
        }

        public void LeaveTryBlock()
        {
            this._methodExceptionData.LeaveTryBlock();
        }

        public void EnterCatchClause(ICatchClause catchClause)
        {
            this._methodExceptionData.EnterCatchClause(catchClause);
        }

        public void LeaveCatchClause()
        {
            this._methodExceptionData.LeaveCatchClause();
        }
    }
}