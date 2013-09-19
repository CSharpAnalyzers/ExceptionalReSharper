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
            _daemonProcess = daemonProcess;
            _process = process;
            _currentContext = new NullProcessContext();
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            if (element is IThrowStatement)
            {
                _currentContext.Process(element as IThrowStatement);
            }

            if (element is ICatchVariableDeclaration)
            {
                _currentContext.Process(element as ICatchVariableDeclaration);
            }

            if (element is IReferenceExpression)
            {
                _currentContext.Process(element as IReferenceExpression);
            }

            if (element is IMethodDeclaration)
            {
                var methodDeclaration = element as IMethodDeclaration;
                if (ShouldProcessMethod(methodDeclaration))
                {
                    _currentContext = new MethodProcessContext();
                    _currentContext.StartProcess(new MethodDeclarationModel(methodDeclaration));
                }
            }
            else if (element is IPropertyDeclaration)
            {
                var propertyDeclaration = element as IPropertyDeclaration;
                if (ShouldProcessProperty(propertyDeclaration))
                {
                    _currentContext = new PropertyProcessContext();
                    _currentContext.StartProcess(new PropertyDeclarationModel(propertyDeclaration));
                }
            }
            else if (element is IAccessorDeclaration)
            {
                _currentContext.EnterAccessor(element as IAccessorDeclaration);
            }
            else if (element is IDocCommentBlockNode)
            {
                _currentContext.Process(element as IDocCommentBlockNode);
            }
            else if (element is ITryStatement)
            {
                _currentContext.EnterTryBlock(element as ITryStatement);
            }
            else if (element is ICatchClause)
            {
                _currentContext.EnterCatchClause(element as ICatchClause);
            }
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
            if (element is IMethodDeclaration)
            {
                _currentContext.EndProcess(_daemonProcess);
            }
            else if (element is IPropertyDeclaration)
            {
                _currentContext.EndProcess(_daemonProcess);
            }
            else if (element is IAccessorDeclaration)
            {
                _currentContext.LeaveAccessor();
            }
            else if (element is ITryStatement)
            {
                _currentContext.LeaveTryBlock();
            }
            else if (element is ICatchClause)
            {
                _currentContext.LeaveCatchClause();
            }
        }

        public bool ProcessingIsFinished
        {
            get { return _process.InterruptFlag; }
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
