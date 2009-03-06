/// <copyright file="ExceptionalDaemonStageProcess.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

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
                ProcessContext.Instance.StartProcess(element as IMethodDeclaration);
            }
            else if (element is ITryStatement)
            {
                ProcessContext.Instance.EnterTryBlock(element as ITryStatement);
            }
            else if(element is ICatchClause)
            {
                ProcessContext.Instance.EnterCatchClause(element as ICatchClause);
            }
            else if(element is ICSharpCommentNode)
            {
                ProcessContext.Instance.Process(element as ICSharpCommentNode);
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
            ProcessContext.Instance.Process(throwStatement);
        }

        public override void VisitCatchVariableDeclaration(ICatchVariableDeclaration catchVariableDeclaration)
        {
            ProcessContext.Instance.Process(catchVariableDeclaration);
        }
    }
}