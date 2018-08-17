using CodeStack.Tools.Sw.SketchPlusPlus.Commands.Base;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Commands
{
    public class PartialChainSelector : IPartialChainSelector
    {
        private ISldWorks m_App;

        public PartialChainSelector(ISldWorks app)
        {
            m_App = app;
        }

        public bool CanExecute()
        {
            var model = m_App.IActiveDoc2;

            if (model != null)
            {
                var selMgr = model.ISelectionManager;

                if (selMgr.GetSelectedObjectCount2(-1) == 2)
                {
                    var firstSeg = selMgr.GetSelectedObject6(1, -1) as ISketchSegment;
                    var secondSeg = selMgr.GetSelectedObject6(2, -1) as ISketchSegment;

                    if (firstSeg != null && secondSeg != null)
                    {
                        return firstSeg.GetSketch() == secondSeg.GetSketch();
                    }
                }
            }

            return false;
        }

        public void Execute()
        {
            if (CanExecute())
            {
                var selMgr = m_App.IActiveDoc2.ISelectionManager;

                var firstSeg = selMgr.GetSelectedObject6(1, -1) as ISketchSegment;
                var secondSeg = selMgr.GetSelectedObject6(2, -1) as ISketchSegment;

                var skCont = (firstSeg.GetSketch().GetSketchContours() as object[])
                    .Cast<ISketchContour>().FirstOrDefault(c =>
                    {
                        var segs = (c.GetSketchSegments() as object[]).Cast<ISketchSegment>();
                        return segs.Contains(firstSeg) && segs.Contains(secondSeg);
                    });

                if (skCont != null)
                {
                    var segs = (skCont.GetSketchSegments() as object[]).Cast<ISketchSegment>().ToList();

                    var firstIndex = segs.IndexOf(firstSeg);
                    var secondIndex = segs.IndexOf(secondSeg);

                    var startIndex = Math.Min(firstIndex, secondIndex);
                    var endIndex = Math.Max(firstIndex, secondIndex);

                    var segsToSelect = segs.Skip(startIndex).Take(endIndex - startIndex + 1).ToArray();

                    m_App.IActiveDoc2.Extension.MultiSelect2(segsToSelect, false, null);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
