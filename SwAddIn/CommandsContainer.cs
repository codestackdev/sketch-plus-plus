using CodeStack.Tools.Sw.SketchPlusPlus.Attributes;
using CodeStack.Tools.Sw.SketchPlusPlus.Commands;
using CodeStack.Tools.Sw.SketchPlusPlus.Commands.Base;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Xarial.AppLaunchKit;
using Xarial.AppLaunchKit.Services.Eula;
using Xarial.AppLaunchKit.Services.Updates;
using Xarial.AppLaunchKit.Services.Auth.Oidc;
using Xarial.AppLaunchKit.Services.UserSettings;
using Xarial.AppLaunchKit.Services.Logger;
using Xarial.AppLaunchKit.Services.About;
using System.Threading;
using Xarial.AppLaunchKit.Services.Updates.Exceptions;
using Xarial.AppLaunchKit.Services.Auth.Oidc.Exceptions;

namespace CodeStack.Tools.Sw.SketchPlusPlus
{
    public class CommandsContainer
    {
        private UnityContainer m_Container;

        public static CommandsContainer Instance
        {
            get;
            private set;
        }

        private Dictionary<Enum, Type> m_Commands;

        private ServicesManager m_Kit;

        public event Action<string> Error;

        public CommandsContainer()
        {
            Instance = this;
        }
        
        internal void Load(ISldWorks app, params Type[] cmdGroupTypes)
        {
            m_Kit = new ServicesManager(this.GetType().Assembly, new IntPtr(app.IFrameObject().GetHWnd()),
                            typeof(EulaService),
                            typeof(UpdatesService),
                            typeof(OpenIdConnectorService),
                            typeof(UserSettingsService),
                            typeof(SystemEventLogService),
                            typeof(AboutApplicationService));

            m_Kit.HandleError += OnHandleError;
            m_Container = new UnityContainer();

            m_Container.RegisterInstance(app);
            m_Container.RegisterInstance(app.IGetMathUtility() as IMathUtility);
            m_Container.RegisterInstance(app.IGetModeler() as IModeler);
            m_Container.RegisterInstance(m_Kit);

            m_Commands = new Dictionary<Enum, Type>();

            foreach (var cmdGrpType in cmdGroupTypes)
            {
                foreach (var cmd in Enum.GetValues(cmdGrpType).Cast<Enum>())
                {
                    Type from;
                    Type to;
                    GetCommandHandler(cmd, out from, out to);
                    m_Commands.Add(cmd, from);
                    m_Container.RegisterType(from, to,
                        new ContainerControlledLifetimeManager());
                }
            }

            m_Kit.StartServices();
        }
        
        private bool OnHandleError(Exception ex)
        {
            var error = "";

            if (ex is UpdatesCheckException)
            {
                error = "Failed to check for updates";
            }
            else if (ex is LoginFailedException)
            {
                error = "Failed to login";
            }
            else
            {
                error = "Generic error";
            }

            Error?.Invoke(error);

            return true;
        }

        internal TService GetService<TService>()
        {
            return m_Container.Resolve<TService>();
        }

        internal ICommand GetCommand(Enum cmdId)
        {
            var cmdType = m_Commands[cmdId];
            return m_Container.Resolve(cmdType) as ICommand;
        }

        private void GetCommandHandler(Enum enumer, out Type baseHandler, out Type handler)
        {
            var enumType = enumer.GetType();
            var enumField = enumType.GetMember(enumer.ToString()).FirstOrDefault();
            var atts = enumField.GetCustomAttributes(typeof(SketchCommandItemInfoAttribute), false);

            if (atts != null && atts.Any())
            {
                var infoAtt = atts.First() as SketchCommandItemInfoAttribute;
                baseHandler = infoAtt.BaseHandlerType;
                handler = infoAtt.HandlerType;
            }
            else
            {
                throw new NullReferenceException($"Attribute of type {typeof(SketchCommandItemInfoAttribute)} is not fond on {enumer}");
            }
        }
    }
}
