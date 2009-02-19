using System.Diagnostics;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>This process is executed by the ReSharper's Daemon</summary>
    /// <remarks>The instance of this class is constructed each time the daemon
    /// needs to rehighlight a given file. This object is short-lived. It executes
    /// the target highlighting logic.</remarks>
    public class ExceptionalDaemonStageProcess : CSharpErrorStageProcessBase
    {
        private readonly ExceptionsAnalyzer _exceptionsAnalyzer;
        private ICSharpFile _file;

        public ExceptionalDaemonStageProcess(IDaemonProcess process) : base(process)
        {
            this._exceptionsAnalyzer = new ExceptionsAnalyzer(this);
        }

        public override void ProcessFile(ICSharpFile file)
        {
            this._file = file;

            if (this.DaemonProcess.FullRehighlightingRequired)
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
                this._exceptionsAnalyzer.BeginProcess(method);
            }
            else if (element is ITryStatement)
            {
                this._exceptionsAnalyzer.BeginTry(element as ITryStatement);
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
                this._exceptionsAnalyzer.Compute(method);
            }
            else if (element is ITryStatement)
            {
                this._exceptionsAnalyzer.EndTry(element as ITryStatement);
            }
        }

        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclarationParam)
        {
            this._exceptionsAnalyzer.Process(methodDeclarationParam);
        }

        public override void VisitThrowStatement(IThrowStatement throwStatementParam)
        {
            this._exceptionsAnalyzer.Process(throwStatementParam);
        }

        public override void VisitCatchVariableDeclaration(ICatchVariableDeclaration catchVariableDeclarationParam)
        {
            this._exceptionsAnalyzer.Process(catchVariableDeclarationParam);
        }

        public override void VisitSpecificCatchClause(ISpecificCatchClause specificCatchClauseParam)
        {
            this._exceptionsAnalyzer.Process(specificCatchClauseParam);
        }

        public override void VisitGeneralCatchClause(IGeneralCatchClause generalCatchClauseParam)
        {
            this._exceptionsAnalyzer.Process(generalCatchClauseParam);
        }

        public override void VisitTryStatement(ITryStatement tryStatementParam)
        {
            this._exceptionsAnalyzer.Process(tryStatementParam);
        }
    }
}