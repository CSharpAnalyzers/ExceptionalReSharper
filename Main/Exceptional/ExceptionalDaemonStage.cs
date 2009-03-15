/// <copyright file="ExceptionalDaemonStage.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

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
        public override IDaemonStageProcess CreateProcess(IDaemonProcess process, DaemonProcessKind processKind)
        {
            if (process == null) return null;
            if (IsSupported(process.ProjectFile) == false) return null;

            return new ExceptionalDaemonStageProcess(process);
        }

        public override ErrorStripeRequest NeedsErrorStripe(IProjectFile projectFile)
        {
            //We need a stripe and we're willing to show errors and warnings on it
            return ErrorStripeRequest.STRIPE_AND_ERRORS;
        }
    }
}