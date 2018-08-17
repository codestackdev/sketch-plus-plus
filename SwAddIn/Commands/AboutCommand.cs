using CodeStack.Tools.Sw.SketchPlusPlus.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.AppLaunchKit;
using Xarial.AppLaunchKit.Base.Services;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Commands
{
    public class AboutCommand : IAbout
    {
        private IAboutApplicationService m_Service;

        public AboutCommand(ServicesManager srvMgr)
        {
            m_Service = srvMgr.GetService<IAboutApplicationService>();
        }

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            m_Service.ShowAboutForm();
        }
    }
}
