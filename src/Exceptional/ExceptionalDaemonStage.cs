using ReSharper.Exceptional.Settings;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional
{
    [ZoneMarker]
    public class ZoneMarker : IPsiLanguageZone, IRequire<ILanguageCSharpZone>, IRequire<ICodeEditingZone>, IRequire<DaemonZone>, IRequire<NavigationZone>
    {
    }

    internal static class ServiceLocator
    {
        public static IDaemonProcess Process { get; set; }
        public static ExceptionalDaemonStageProcess StageProcess { get; set; }
        public static ExceptionalSettings Settings { get; set; }
    }

    /// <summary>Daemon stage that is responsible for creating daemon stage process.</summary>
    /// <remarks>The daemon stage is needed to plug-in into a ReSharper's highlighting infrastructure.
    /// It is responsible for creating daemon stage process. The <see cref="DaemonStageAttribute"/>
    /// marks this type so that it will be automatically loaded by ReSharper. To work properly the
    /// marked type must implement <see cref="IDaemonStage"/> interface.</remarks>
    [DaemonStage]
    public class ExceptionalDaemonStage : CSharpDaemonStageBase
    {
        protected override IDaemonStageProcess CreateProcess(
            IDaemonProcess process, IContextBoundSettingsStore settings,
            DaemonProcessKind processKind, ICSharpFile file)
        {
            if (IsSupported(process.SourceFile) == false)
                return null;

            var exceptionalSettings = settings.GetKey<ExceptionalSettings>(SettingsOptimization.OptimizeDefault);
            exceptionalSettings.InvalidateCaches();

            ServiceLocator.Process = process;
            ServiceLocator.Settings = exceptionalSettings;
            ServiceLocator.StageProcess = new ExceptionalDaemonStageProcess(file, process.SourceFile, settings);

            return ServiceLocator.StageProcess;
        }
    }
}