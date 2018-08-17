using System.Runtime.InteropServices;
using CodeStack.Tools.Sw.SketchPlusPlus;
using CodeStack.Tools.Sw.SketchPlusPlus.Auth;
using CodeStack.Tools.Sw.SketchPlusPlus.Properties;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xarial.AppLaunchKit.Attributes;
using Xarial.AppLaunchKit.Services.Attributes;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]

//[assembly: SketchPlusPlusAuth]

[assembly: UpdatesUrl("https://www.codestack.net/labs/solidworks/sketch-plus-plus/version-info.json")]

[assembly: About(typeof(Resources), nameof(Resources.eula),
    nameof(Resources.licenses), nameof(Resources.main_icon))]

[assembly: ApplicationInfo(typeof(Resources), Environment.SpecialFolder.ApplicationData,
    nameof(Resources.WorkDir), nameof(Resources.AppTitle), nameof(Resources.main))]
[assembly: ComVisible(false)]

