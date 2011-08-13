// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>This process is executed by the ReSharper's Daemon</summary>
    /// <remarks>The instance of this class is constructed each time the daemon
    /// needs to re highlight a given file. This object is short-lived. It executes
    /// the target highlighting logic.</remarks>
    public class ExceptionalDaemonStageProcess : CSharpDaemonStageProcessBase
    {
        private readonly IDaemonProcess _process;
        private readonly List<HighlightingInfo> _hightlightings = new List<HighlightingInfo>();

        public List<HighlightingInfo> Hightlightings
        {
            get { return this._hightlightings; }
        }

        public ExceptionalDaemonStageProcess(IDaemonProcess process) : base(process)
        {
            this._process = process;
        }

        public override void Execute(Action<DaemonStageResult> commiter)
        {
            // Getting PSI (AST) for the file being highlighted
            var manager = PsiManager.GetInstance(_process.Solution);
            var file = manager.GetPsiFile(_process.SourceFile, CSharpLanguage.Instance) as ICSharpFile;
            if (file == null) return;

            // Running visitor against the PSI
            var elementProcessor = new ExceptionalProcessor(this, _process);
            file.ProcessDescendants(elementProcessor);

            // Checking if the daemon is interrupted by user activity);)
            if (_process.InterruptFlag)
                throw new ProcessCancelledException();

            commiter(new DaemonStageResult(this.Hightlightings));
        }
    }
}