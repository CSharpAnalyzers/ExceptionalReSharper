using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    internal abstract class AnalyzeUnitModelBase<T> : BlockModelBase<T>, IAnalyzeUnit where T : ITreeNode
    {
        private readonly ExceptionalSettings _settings;

        protected AnalyzeUnitModelBase(IAnalyzeUnit analyzeUnit, T node, ExceptionalSettings settings)
            : base(analyzeUnit, node)
        {
            _settings = settings;

            DocumentationBlock = new DocCommentBlockModel(this, null);
        }

        public DocCommentBlockModel DocumentationBlock { get; set; }

        ITreeNode IAnalyzeUnit.Node
        {
            get { return Node; }
        }

        public bool IsInspected
        {
            get
            {
                var accessRightsOwner = Node as IAccessRightsOwner;
                if (accessRightsOwner == null)
                    return false;

                var inspectPublicMethods = _settings.InspectPublicMethods;
                var inspectInternalMethods = _settings.InspectInternalMethods;
                var inspectProtectedMethods = _settings.InspectProtectedMethods;
                var inspectPrivateMethods = _settings.InspectPrivateMethods;

                var rights = accessRightsOwner.GetAccessRights();
                return (rights == AccessRights.PUBLIC && inspectPublicMethods) ||
                       (rights == AccessRights.INTERNAL && inspectInternalMethods) ||
                       (rights == AccessRights.PROTECTED && inspectProtectedMethods) ||
                       (rights == AccessRights.PRIVATE && inspectPrivateMethods);
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            if (DocumentationBlock != null)
                DocumentationBlock.Accept(analyzerBase);

            base.Accept(analyzerBase);
        }

        public IPsiModule GetPsiModule()
        {
            return Node.GetPsiModule();
        }
    }
}