using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Utils
{
    public static class MathUtils
    {
        public static double GetDistanceBetweenPoints(double[] firstPt, double[] secondPt)
        {
            return Math.Sqrt(Math.Pow(firstPt[0] - secondPt[0], 2)
                + Math.Pow(firstPt[1] - secondPt[1], 2)
                + Math.Pow(firstPt[2] - secondPt[2], 2));
        }
    }
}
