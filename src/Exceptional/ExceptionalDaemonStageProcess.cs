using JetBrains.DocumentModel;
using JetBrains.Application.Settings;
using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Settings;

#if R9 || R10
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

namespace ReSharper.Exceptional
{
    /// <summary>This process is executed by the ReSharper's Daemon</summary>
    /// <remarks>The instance of this class is constructed each time the daemon needs to re highlight a given file. 
    /// This object is short-lived. It executes the target highlighting logic.</remarks>
    public class ExceptionalDaemonStageProcess : CSharpDaemonStageProcessBase
    {
        private readonly IContextBoundSettingsStore _settings;
        private readonly IHighlightingConsumer _consumer;

        public ExceptionalDaemonStageProcess(ICSharpFile file, IContextBoundSettingsStore settings)
            : base(ServiceLocator.Process, file)
        {
            _settings = settings;

#if R2016_1 || R2016_2
            _consumer = new FilteringHighlightingConsumer(this, _settings, file);
#else
            _consumer = new DefaultHighlightingConsumer(this, _settings);
#endif
        }

        public void AddHighlighting(IHighlighting highlighting, DocumentRange range)
        {
            _consumer.AddHighlighting(highlighting, range);
        }

        /// <exception cref="ProcessCancelledException">The process has been cancelled. </exception>
        public override void Execute(Action<DaemonStageResult> commiter)
        {
            var file = ServiceLocator.Process.SourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) as ICSharpFile;
            if (file == null)
                return;

            var elementProcessor = new ExceptionalRecursiveElementProcessor(this);
            file.ProcessDescendants(elementProcessor);

            if (ServiceLocator.Process.InterruptFlag)
                throw new ProcessCancelledException();

            commiter(new DaemonStageResult(_consumer.Highlightings));
        }
    }
}