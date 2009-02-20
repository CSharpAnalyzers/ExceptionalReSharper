using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>This process is executed by the ReSharper's Daemon</summary>
    /// <remarks>The instance of this class is constructed each time the daemon
    /// needs to re highlight a given file. This object is short-lived. It executes
    /// the target highlighting logic.</remarks>
    public class ExceptionalDaemonStageProcess : CSharpErrorStageProcessBase
    {
        private MethodContext _methodContext;

        public ExceptionalDaemonStageProcess(IDaemonProcess process) : base(process) { }

        public override void ProcessFile(ICSharpFile file)
        {
            //if (this.DaemonProcess.FullRehighlightingRequired)
            {
                file.ProcessDescendants(this);
                this.FullyRehighlighted = true;
            }
        }

        public override void ProcessBeforeInterior(IElement element)
        {
            if (element is IMethodDeclaration)
            {
                var method = element as IMethodDeclaration;

                if (this._methodContext != null)
                    throw new InvalidOperationException("The analyzed method has not been computed.");

                this._methodContext = new MethodContext(method);
            }
            else if (element is ITryStatement)
            {
                this._methodContext.EnterTryBlock(element as ITryStatement);
            }
            else if(element is ICatchClause)
            {
                this._methodContext.EnterCatchClause(element as ICatchClause);
            }
        }

        /// <summary>This is executed after processing the contents of a given element.</summary>
        public override void ProcessAfterInterior(IElement element)
        {
            //This call triggers visiting so it must be called first.
            base.ProcessAfterInterior(element);

            if(element is IMethodDeclaration)
            {
                var method = element as IMethodDeclaration;

                if (this._methodContext == null)
                    throw new InvalidOperationException("You have to first begin the process.");
                if (this._methodContext.IsDefinedFor(method) == false)
                    throw new InvalidOperationException("The given method does not match processed method.");

                this._methodContext.ComputeResult(this);
                this._methodContext = null;
            }
            else if (element is ITryStatement)
            {
                this._methodContext.LeaveTryBlock();
            }
            else if (element is ICatchClause)
            {
                this._methodContext.LeaveCatchClause();
            }
        }

        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclaration)
        {
            var exceptionsDocumentation = DocumentedExceptionsModel.Create(methodDeclaration);
            this._methodContext.Add(exceptionsDocumentation);
        }

        public override void VisitThrowStatement(IThrowStatement throwStatement)
        {
            var model = ThrowStatementModel.Create(throwStatement);
            this._methodContext.Add(model);
        }

        public override void VisitGeneralCatchClause(IGeneralCatchClause generalCatchClause)
        {
            var model = CatchAllClauseModel.Create(generalCatchClause);
            this._methodContext.Add(model);
        }

        public override void VisitSpecificCatchClause(ISpecificCatchClause specificCatchClause)
        {
            var model = CatchAllClauseModel.Create(specificCatchClause);
            this._methodContext.Add(model);
        }

        public override void VisitCatchVariableDeclaration(ICatchVariableDeclaration catchVariableDeclaration)
        {
        }

        public override void VisitTryStatement(ITryStatement tryStatement)
        {
        }
    }
}