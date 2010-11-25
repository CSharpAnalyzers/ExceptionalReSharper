// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    public class ExceptionalProcessor : IRecursiveElementProcessor
    {
        private readonly CSharpDaemonStageProcessBase _daemonProcess;
        private readonly IDaemonProcess _process;
        private IProcessContext _currentContext;

        public ExceptionalProcessor(CSharpDaemonStageProcessBase daemonProcess, IDaemonProcess process)
        {
            this._daemonProcess = daemonProcess;
            this._process = process;
            this._currentContext = new NullProcessContext();
        }

        public bool InteriorShouldBeProcessed(IElement element)
        {
            return true;
        }

        public void ProcessBeforeInterior(IElement element)
        {
            if (element is IThrowStatement)
            {
                this._currentContext.Process(element as IThrowStatementNode);
            }

            if (element is ICatchVariableDeclaration)
            {
                this._currentContext.Process(element as ICatchVariableDeclarationNode);
            }

            if (element is IReferenceExpression)
            {
                this._currentContext.Process(element as IReferenceExpressionNode);
            }

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
                this._currentContext.EnterAccessor(element as IAccessorDeclarationNode);
            }
            else if (element is IDocCommentBlockNode)
            {
                this._currentContext.Process(element as IDocCommentBlockNode);
            }
            else if (element is ITryStatementNode)
            {
                this._currentContext.EnterTryBlock(element as ITryStatementNode);
            }
            else if (element is ICatchClauseNode)
            {
                this._currentContext.EnterCatchClause(element as ICatchClauseNode);
            }
        }

        public void ProcessAfterInterior(IElement element)
        {
            if (element is IMethodDeclarationNode)
            {
                this._currentContext.EndProcess(this._daemonProcess);
            }
            else if (element is IPropertyDeclarationNode)
            {
                this._currentContext.EndProcess(this._daemonProcess);
            }
            else if (element is IAccessorDeclarationNode)
            {
                this._currentContext.LeaveAccessor();
            }
            else if (element is ITryStatementNode)
            {
                this._currentContext.LeaveTryBlock();
            }
            else if (element is ICatchClauseNode)
            {
                this._currentContext.LeaveCatchClause();
            }
        }

        public bool ProcessingIsFinished
        {
            get { return this._process.InterruptFlag; }
        }

        private static bool ShouldProcessProperty(IPropertyDeclarationNode propertyDeclarationNode)
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
    }
}
