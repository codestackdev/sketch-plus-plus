using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using System.Collections.Generic;
using System.Diagnostics;
using CodeStack.Dev.Sw.AddIn;
using CodeStack.Dev.Sw.AddIn.Attributes;
using CodeStack.Dev.Sw.AddIn.Enums;
using System.ComponentModel;
using CodeStack.Tools.Sw.SketchPlusPlus.Properties;
using CodeStack.Dev.Sw.AddIn.Helpers;
using CodeStack.Tools.Sw.SketchPlusPlus.Attributes;
using CodeStack.Tools.Sw.SketchPlusPlus.Commands.Base;
using CodeStack.Tools.Sw.SketchPlusPlus.Commands;

namespace CodeStack.Tools.Sw.SketchPlusPlus
{
    [Title("Sketch++")]
    [Description("Utilities for complimenting sketch functionality")]
    [Icon(typeof(Resources), nameof(Resources.main_icon))]
    [CommandGroupInfo(0)]
    public enum SketchCommands_e
    {
        [Title("Create Offset Fillet")]
        [Description("Creates Offset Fillet")]
        [Icon(typeof(Resources), nameof(Resources.offset_icon))]
        [SketchCommandItemInfo(true, true, swWorkspaceTypes_e.Part,
            typeof(IOffsetWithFillet), typeof(OffsetWithFillet))]
        CreateSketchOffsetFillet,

        [Title("Select Partial Chain")]
        [Description("Selects the chain between two selected sketch segments")]
        [Icon(typeof(Resources), nameof(Resources.main_icon))]
        [SketchCommandItemInfo(true, true, swWorkspaceTypes_e.All,
            typeof(IPartialChainSelector), typeof(PartialChainSelector))]
        SelectPartialChain,

        [Title("About...")]
        [Description("About Sketch++")]
        [Icon(typeof(Resources), nameof(Resources.about_icon))]
        [SketchCommandItemInfo(true, false, swWorkspaceTypes_e.All,
            typeof(IAbout), typeof(AboutCommand))]
        About
    }
    
    [Guid("26D588DD-D3FD-45EF-8F35-A61656AA83E9"), ComVisible(true)]
    [SwAddin(
        Description = "Sketch++",
        Title = "Sketch++",
        LoadAtStartup = true
        )]
    public class SwAddin : SwAddInEx, IAddCommandGroupWithEnable<SketchCommands_e>
    {
        #region SolidWorks Registration

#if DEBUG
        [ComRegisterFunction]
        public static void RegisterFunction(Type t)
        {
            RegistrationHelper.Register(t);
        }

        [ComUnregisterFunction]
        public static void UnregisterFunction(Type t)
        {
            RegistrationHelper.Unregister(t);
        }
#endif

        #endregion

        private CommandsContainer m_CommandsContainer;

        protected override bool OnConnect()
        {
            m_CommandsContainer = new CommandsContainer();
            m_CommandsContainer.Error += OnError;
            m_CommandsContainer.Load(m_App,
                typeof(SketchCommands_e));

            return true;
        }

        private void OnError(string err)
        {
            m_App.SendMsgToUser2($"Sketch++: {err}", (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
        }

        public void Callback(SketchCommands_e cmd)
        {
            m_CommandsContainer.GetCommand(cmd).Execute();
        }
        
        public void Enable(SketchCommands_e cmd, ref CommandItemEnableState_e state)
        {
            if (!m_CommandsContainer.GetCommand(cmd).CanExecute())
            {
                state = CommandItemEnableState_e.DeselectDisable;
            }
        }
    }
}
