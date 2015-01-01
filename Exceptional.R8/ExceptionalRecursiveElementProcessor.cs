using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Contexts;
using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

#if R9
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

namespace ReSharper.Exceptional
{
    public class ExceptionalRecursiveElementProcessor : IRecursiveElementProcessor
    {
        private readonly CSharpDaemonStageProcessBase _daemonProcess;
        private readonly IDaemonProcess _process;
        private readonly ExceptionalSettings _settings;
#if R8
        private readonly List<IDocCommentBlockNode> _eventComments = new List<IDocCommentBlockNode>();
#endif
#if R9
        private readonly List<IDocCommentBlock> _eventComments = new List<IDocCommentBlock>();
#endif

        private IProcessContext _currentContext;

        public ExceptionalRecursiveElementProcessor(CSharpDaemonStageProcessBase daemonProcess, IDaemonProcess process, ExceptionalSettings settings)
        {
            _settings = settings;
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
                _currentContext.Process(element as IThrowStatement);
            else if (element is ICatchVariableDeclaration)
                _currentContext.Process(element as ICatchVariableDeclaration);
            else if (element is IReferenceExpression)
                _currentContext.Process(element as IReferenceExpression);
            else if (element is IObjectCreationExpression)
                _currentContext.Process(element as IObjectCreationExpression);
            if (element is IMethodDeclaration)
            {
                var methodDeclaration = element as IMethodDeclaration;
                _currentContext = new MethodProcessContext();
                _currentContext.StartProcess(new MethodDeclarationModel(methodDeclaration, _settings));
            }
            else if (element is IConstructorDeclaration)
            {
                var constructorDeclaration = element as IConstructorDeclaration;
                _currentContext = new ConstructorProcessContext();
                _currentContext.StartProcess(new ConstructorDeclarationModel(constructorDeclaration, _settings));
            }
            else if (element is IPropertyDeclaration || element is IIndexerDeclaration)
            {
                var accessorOwnerDeclaration = element as IAccessorOwnerDeclaration;
                _currentContext = new AccessorOwnerProcessContext();
                _currentContext.StartProcess(new AccessorOwnerDeclarationModel(accessorOwnerDeclaration, _settings));
            }
            else if (element is IAccessorDeclaration && !(_currentContext is AccessorOwnerProcessContext)) // already in accessor block (e.g. property)
                _currentContext.EnterAccessor(element as IAccessorDeclaration);
            else if (element is IEventDeclaration)
            {
                var eventDeclaration = element as IEventDeclaration;
                _currentContext = new EventProcessContext();
                _currentContext.StartProcess(new EventDeclarationModel(eventDeclaration, _settings));

                foreach (var doc in _eventComments)
                    _currentContext.Process(doc);
                _eventComments.Clear();
            }
#if R8
            else if (element is IDocCommentBlockNode)
            {
                if (_currentContext.Model == null || _currentContext.Model.Node == element.Parent)
                    _currentContext.Process(element as IDocCommentBlockNode);
                else
                {
                    _eventComments.Add((IDocCommentBlockNode)element);
                    // HACK: Event documentation blocks are processed before event declaration, 
                    // other documentation blocks are processed after the associated element declaration
                }
            }
#endif
#if R9
            else if (element is IDocCommentBlock)
            {
                if (_currentContext.Model == null || _currentContext.Model.Node == element.Parent)
                    _currentContext.Process(element as IDocCommentBlock);
                else
                {
                    _eventComments.Add((IDocCommentBlock)element);
                    // HACK: Event documentation blocks are processed before event declaration, 
                    // other documentation blocks are processed after the associated element declaration
                }
            }
#endif
            else if (element is ITryStatement)
                _currentContext.EnterTryBlock(element as ITryStatement);
            else if (element is ICatchClause)
                _currentContext.EnterCatchClause(element as ICatchClause);
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
            if (element is IMethodDeclaration)
                _currentContext.EndProcess(_daemonProcess, _settings);
            else if (element is IPropertyDeclaration || element is IIndexerDeclaration)
                _currentContext.EndProcess(_daemonProcess, _settings);
            else if (element is IAccessorDeclaration && !(_currentContext is AccessorOwnerProcessContext)) // already in accessor block (e.g. property)
            {
                _currentContext.EndProcess(_daemonProcess, _settings);
                _currentContext.LeaveAccessor();
            }
            else if (element is IEventDeclaration)
                _currentContext.EndProcess(_daemonProcess, _settings);
            else if (element is IConstructorDeclaration)
                _currentContext.EndProcess(_daemonProcess, _settings);
            else if (element is ITryStatement)
                _currentContext.LeaveTryBlock();
            else if (element is ICatchClause)
                _currentContext.LeaveCatchClause();
        }

        public bool ProcessingIsFinished
        {
            get { return _process.InterruptFlag; }
        }
    }
}
