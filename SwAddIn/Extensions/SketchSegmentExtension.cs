using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidWorks.Interop.sldworks
{
    public static class SketchSegmentExtension
    {
        public static Curve GetTrimmedCurve(this ISketchSegment skSeg)
        {
            var curve = skSeg.IGetCurve();

            ISketchPoint startPt = null;
            ISketchPoint endPt = null;

            if (skSeg is ISketchLine)
            {
                startPt = (skSeg as ISketchLine).IGetStartPoint2();
                endPt = (skSeg as ISketchLine).IGetEndPoint2();
            }
            else if (skSeg is ISketchArc)
            {
                startPt = (skSeg as ISketchArc).IGetStartPoint2();
                endPt = (skSeg as ISketchArc).IGetEndPoint2();
            }

            curve = curve.CreateTrimmedCurve2(startPt.X, startPt.Y, startPt.Z,
                endPt.X, endPt.Y, endPt.Z);

            return curve;
        }
    }
}
