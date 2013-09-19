// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.

using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>Daemon stage that is responsible for creating daemon stage process.</summary>
    /// <remarks>The daemon stage is needed to plug-in into a ReSharper's highlighting infrastructure.
    /// It is responsible for creating daemon stage process. The <see cref="DaemonStageAttribute"/>
    /// marks this type so that it will be automatically loaded by ReSharper. To work properly the
    /// marked type must implement <see cref="IDaemonStage"/> interface.</remarks>
    [DaemonStage]
    public class ExceptionalDaemonStage : CSharpDaemonStageBase
    {
        public override ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settings)
        {
            //We need a stripe and we're willing to show errors and warnings on it
            return ErrorStripeRequest.STRIPE_AND_ERRORS;
        }

        protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings,
                                                             DaemonProcessKind processKind, ICSharpFile file)
        {
            if (process == null) return null;
            if (IsSupported(process.SourceFile) == false) return null;

            return new ExceptionalDaemonStageProcess(process, file);
        }
    }
}