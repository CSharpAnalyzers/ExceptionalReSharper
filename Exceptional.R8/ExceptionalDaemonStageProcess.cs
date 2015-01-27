using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Settings;

#if R9
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

namespace ReSharper.Exceptional
{
    /// <summary>This process is executed by the ReSharper's Daemon</summary>
    /// <remarks>The instance of this class is constructed each time the daemon needs to re highlight a given file. 
    /// This object is short-lived. It executes the target highlighting logic.</remarks>
    public class ExceptionalDaemonStageProcess : CSharpDaemonStageProcessBase
    {
        private readonly List<HighlightingInfo> _hightlightings = new List<HighlightingInfo>();

        public ExceptionalDaemonStageProcess(ICSharpFile file)
            : base(ServiceLocator.Process, file)
        {
        }

        public List<HighlightingInfo> Hightlightings
        {
            get { return _hightlightings; }
        }

        public override void Execute(Action<DaemonStageResult> commiter)
        {
            var file = ServiceLocator.Process.SourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) as ICSharpFile;
            if (file == null)
                return;

            var elementProcessor = new ExceptionalRecursiveElementProcessor(this);
            file.ProcessDescendants(elementProcessor);

            if (ServiceLocator.Process.InterruptFlag)
                throw new ProcessCancelledException();

            commiter(new DaemonStageResult(Hightlightings));
        }
    }
}