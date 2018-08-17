using CodeStack.Dev.Sw.AddIn.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStack.Dev.Sw.AddIn.Enums;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Attributes
{
    public class SketchCommandItemInfoAttribute : CommandItemInfoAttribute
    {
        public Type BaseHandlerType { get; private set; }
        public Type HandlerType { get; private set; }

        public SketchCommandItemInfoAttribute(bool hasMenu, bool hasToolbar, 
            swWorkspaceTypes_e suppWorkspaces, Type baseHandlerType, Type handlerType)
            : base(hasMenu, hasToolbar, suppWorkspaces)
        {
            BaseHandlerType = baseHandlerType;
            HandlerType = handlerType;
        }
    }
}
