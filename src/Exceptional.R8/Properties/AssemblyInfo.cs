using System.Reflection;
using System.Runtime.InteropServices;
#if R8
using JetBrains.Application.PluginSupport;
#endif

[assembly: AssemblyTitle("Exceptional for ReSharper")]
[assembly: AssemblyDescription("Analyzes thrown and documented exceptions and suggests improvements. ")]
[assembly: AssemblyProduct("Exceptional")]
[assembly: AssemblyCompany("ExceptionalDevs")]
[assembly: AssemblyCopyright("Copyright © 2015 ExceptionalDevs")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.7.8.0")]
[assembly: ComVisible(false)]
[assembly: Guid("3628d589-e118-4c2c-bd8e-fdef6b6ed07c")]

#if R8
[assembly: PluginTitle("Exceptional")]
[assembly: PluginDescription("Analyzes exception usage and suggests improvements.")]
[assembly: PluginVendor("ExceptionalDevs")]
#endif