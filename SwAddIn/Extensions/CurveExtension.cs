using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidWorks.Interop.sldworks
{
    public static class CurveExtension
    {
        public static double[] GetIntersectionWithLinearCurve(this ICurve firstCurve, ICurve secondCurve)
        {
            if (!firstCurve.IsLine() || !secondCurve.IsLine())
            {
                throw new InvalidOperationException("Curves must be linear");
            }

            var getEndPtsFunc = new Func<ICurve, double[][]>(curve =>
            {
                double start;
                double end;
                bool isClosed;
                bool isPeriodic;
                curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);

                var evalStart = curve.Evaluate2(start, 1) as double[];
                var evalEnd = curve.Evaluate2(end, 1) as double[];

                return new double[][]
                {
                    new double[] { evalStart[0], evalStart[1], evalStart[2]},
                    new double[] { evalEnd[0], evalEnd[1], evalEnd[2]}
                };
            });

            firstCurve = firstCurve.GetBaseCurve();
            secondCurve = secondCurve.GetBaseCurve();

            var firstPts = getEndPtsFunc.Invoke(firstCurve);
            var secondPts = getEndPtsFunc.Invoke(secondCurve);

            var pt = firstCurve.IntersectCurve(secondCurve,
                firstPts[0], firstPts[1],
                secondPts[0], secondPts[1]) as double[];

            return new double[] { pt[0], pt[1], pt[2] };
        }
    }
}
