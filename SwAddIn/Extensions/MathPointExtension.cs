using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidWorks.Interop.sldworks
{
    public static class MathPointExtension
    {
        public static IMathPoint MoveAlongVector(this IMathPoint pt, IMathVector dir, double dist)
        {
            dir = dir.Normalise().Scale(dist) as IMathVector;

            var centerPt = pt.AddVector(dir) as IMathPoint;

            return centerPt;
        }
    }
}
