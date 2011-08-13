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

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            if (element is IThrowStatement)
            {
                this._currentContext.Process(element as IThrowStatement);
            }

            if (element is ICatchVariableDeclaration)
            {
                this._currentContext.Process(element as ICatchVariableDeclaration);
            }

            if (element is IReferenceExpression)
            {
                this._currentContext.Process(element as IReferenceExpression);
            }

            if (element is IMethodDeclaration)
            {
                var methodDeclaration = element as IMethodDeclaration;
                if (ShouldProcessMethod(methodDeclaration))
                {
                    this._currentContext = new MethodProcessContext();
                    this._currentContext.StartProcess(new MethodDeclarationModel(methodDeclaration));
                }
            }
            else if (element is IPropertyDeclaration)
            {
                var propertyDeclaration = element as IPropertyDeclaration;
                if (ShouldProcessProperty(propertyDeclaration))
                {
                    this._currentContext = new PropertyProcessContext();
                    this._currentContext.StartProcess(new PropertyDeclarationModel(propertyDeclaration));
                }
            }
            else if (element is IAccessorDeclaration)
            {
                this._currentContext.EnterAccessor(element as IAccessorDeclaration);
            }
            else if (element is IDocCommentBlockNode)
            {
                this._currentContext.Process(element as IDocCommentBlockNode);
            }
            else if (element is ITryStatement)
            {
                this._currentContext.EnterTryBlock(element as ITryStatement);
            }
            else if (element is ICatchClause)
            {
                this._currentContext.EnterCatchClause(element as ICatchClause);
            }
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
            if (element is IMethodDeclaration)
            {
                this._currentContext.EndProcess(this._daemonProcess);
            }
            else if (element is IPropertyDeclaration)
            {
                this._currentContext.EndProcess(this._daemonProcess);
            }
            else if (element is IAccessorDeclaration)
            {
                this._currentContext.LeaveAccessor();
            }
            else if (element is ITryStatement)
            {
                this._currentContext.LeaveTryBlock();
            }
            else if (element is ICatchClause)
            {
                this._currentContext.LeaveCatchClause();
            }
        }

        public bool ProcessingIsFinished
        {
            get { return this._process.InterruptFlag; }
        }

        private static bool ShouldProcessProperty(IPropertyDeclaration propertyDeclarationNode)
        {
            foreach (var accessorDeclarationNode in propertyDeclarationNode.AccessorDeclarations)
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
