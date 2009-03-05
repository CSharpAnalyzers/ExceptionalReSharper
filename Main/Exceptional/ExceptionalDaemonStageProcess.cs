/// <copyright file="ExceptionalDaemonStageProcess.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

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
                var methodDeclaration = element as IMethodDeclaration;

                ProcessContext.Instance.MethodDeclarationModel = new MethodDeclarationModel(methodDeclaration);
                ProcessContext.Instance.MethodDeclarationModel.Initialize();
            }
            else if (element is ITryStatement)
            {
                ProcessContext.Instance.EnterTryBlock(element as ITryStatement);
            }
            else if(element is ICatchClause)
            {
                ProcessContext.Instance.EnterCatchClause(element as ICatchClause);
            }
        }

        /// <summary>This is executed after processing the contents of a given element.</summary>
        public override void ProcessAfterInterior(IElement element)
        {
            //This call triggers visiting so it must be called first.
            base.ProcessAfterInterior(element);

            if(element is IMethodDeclaration)
            {
                ProcessContext.Instance.EndProcess(this);
            }
            else if (element is ITryStatement)
            {
                ProcessContext.Instance.LeaveTryBlock();
            }
            else if (element is ICatchClause)
            {
                ProcessContext.Instance.LeaveCatchClause();
            }
        }

        public override void VisitThrowStatement(IThrowStatement throwStatement)
        {
            var model = new ThrowStatementModel(throwStatement);
            ProcessContext.Instance.Process(model);
        }

        public override void VisitCatchVariableDeclaration(ICatchVariableDeclaration catchVariableDeclaration)
        {
            var model = new CatchVariableModel(catchVariableDeclaration);
            ProcessContext.Instance.Process(model);
        }

        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclaration)
        {
        }

        public override void VisitGeneralCatchClause(IGeneralCatchClause generalCatchClause)
        {
        }

        public override void VisitSpecificCatchClause(ISpecificCatchClause specificCatchClause)
        {
        }

        public override void VisitTryStatement(ITryStatement tryStatement)
        {
        }
    }
}