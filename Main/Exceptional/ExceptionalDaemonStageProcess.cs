/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

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
    public class ExceptionalDaemonStageProcess : CSharpDaemonStageProcessBase
    {
        private IProcessContext _currentContext;

        public ExceptionalDaemonStageProcess(IDaemonProcess process) : base(process)
        {
        }

        public override void Execute(Action<DaemonStageResult> commiter)
        {
            HighlightInFile(file => file.ProcessDescendants(this), commiter);
        }

        public override void ProcessBeforeInterior(IElement element)
        {
            if (element is IMethodDeclarationNode)
            {
                var methodDeclaration = element as IMethodDeclarationNode;
                if (ShouldProcessMethod(methodDeclaration))
                {
                    this._currentContext = new MethodProcessContext();
                    this._currentContext.StartProcess(new MethodDeclarationModel(methodDeclaration));
                }
            }
            else if (element is IPropertyDeclarationNode)
            {
                var propertyDeclaration = element as IPropertyDeclarationNode;
                if (ShouldProcessProperty(propertyDeclaration))
                {
                    this._currentContext = new PropertyProcessContext();
                    this._currentContext.StartProcess(new PropertyDeclarationModel(propertyDeclaration));
                }
            }
            else if (element is IAccessorDeclarationNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.EnterAccessor(element as IAccessorDeclarationNode);
                }
            }
            else if (element is IDocCommentBlockNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.Process(element as IDocCommentBlockNode);
                }
            }
            else if (element is ITryStatementNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.EnterTryBlock(element as ITryStatementNode);
                }
            }
            else if (element is ICatchClauseNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.EnterCatchClause(element as ICatchClauseNode);
                }
            }
        }

        private bool ShouldProcessProperty(IPropertyDeclarationNode propertyDeclarationNode)
        {
            foreach (var accessorDeclarationNode in propertyDeclarationNode.AccessorDeclarationsNode)
            {
                if (accessorDeclarationNode.Body != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ShouldProcessMethod(IMethodDeclaration methodDeclaration)
        {
            return methodDeclaration.Body != null;
        }

        /// <summary>This is executed after processing the contents of a given element.</summary>
        public override void ProcessAfterInterior(IElement element)
        {
            //This call triggers visiting so it must be called first.
            base.ProcessAfterInterior(element);

            if (element is IMethodDeclarationNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.EndProcess(this);
                }
            }
            else if (element is IPropertyDeclarationNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.EndProcess(this);
                }
            }
            else if (element is IAccessorDeclarationNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.LeaveAccessor();
                }
            }
            else if (element is ITryStatementNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.LeaveTryBlock();
                }
            }
            else if (element is ICatchClauseNode)
            {
                if (this._currentContext != null)
                {
                    this._currentContext.LeaveCatchClause();
                }
            }
        }

        public override void VisitThrowStatement(IThrowStatement throwStatement)
        {
            if (this._currentContext != null)
            {
                this._currentContext.Process(throwStatement as IThrowStatementNode);
            }
        }

        public override void VisitCatchVariableDeclaration(ICatchVariableDeclaration catchVariableDeclaration)
        {
            if (this._currentContext != null)
            {
                this._currentContext.Process(catchVariableDeclaration as ICatchVariableDeclarationNode);
            }
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpression)
        {
            if (this._currentContext != null)
            {
                this._currentContext.Process(referenceExpression as IReferenceExpressionNode);
            }
        }
    }
}