using CodeStack.Tools.Sw.SketchPlusPlus.Exceptions;
using CodeStack.Tools.Sw.SketchPlusPlus.Utils;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Commands
{
    public class OffsetBuilder : IDisposable
    {
        private ISldWorks m_App;
        private IModelDoc2 m_Model;
        private IModeler m_Modeler;
        private IMathUtility m_MathUtils;

        private IBody2 m_PreviewOffsetFilletBody;

        public OffsetBuilder(ISldWorks app, IModelDoc2 model)
        {
            m_App = app;
            m_Model = model;
            m_Modeler = app.IGetModeler();
            m_MathUtils = m_App.IGetMathUtility();
        }

        public void PreviewOffset(IEnumerable<ISketchSegment> segs, double offset,
            double innerRadius, double outerRadius, bool reverse)
        {
            HidePreview();
            m_PreviewOffsetFilletBody = CreateOffset(segs, offset, innerRadius, outerRadius, reverse);

            m_PreviewOffsetFilletBody.Display3(m_Model, ConvertColor(KnownColor.Yellow),
                (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
        }

        public Body2 CreateOffset(IEnumerable<ISketchSegment> segs, double offset,
            double innerRadius, double outerRadius, bool reverse)
        {
            if (!segs.Any())
            {
                throw new UserException("No segments provided");
            }

            var curves = new List<ICurve>();

            var transform = segs.First().GetSketch().ModelToSketchTransform.IInverse();

            foreach (var skSeg in segs)
            {
                if (!skSeg.ConstructionGeometry)
                {
                    var curve = skSeg.GetTrimmedCurve();
                    curve.ApplyTransform(transform);
                    curves.Add(curve);
                }
            }

            var wireBody = m_Modeler.CreateWireBody(curves.ToArray(),
                (int)swCreateWireBodyOptions_e.swCreateWireBodyByDefault);

            if (wireBody == null)
            {
                throw new UserException("Segments must be connected and form chain");
            }

            var offsetNormal = m_MathUtils.CreateVector(new double[] { 0, 0, reverse ? -1 : 1 }) as MathVector;

            offsetNormal = offsetNormal.MultiplyTransform(transform) as MathVector;

            var offsetBody = wireBody.OffsetPlanarWireBody(offset, offsetNormal,
                (int)swOffsetPlanarWireBodyOptions_e.swOffsetPlanarWireBodyOptions_GapFillRound);

            if (offsetBody == null)
            {
                throw new UserException("Failed to offset the body. Try change the parameters");
            }

            var offsetCurves = new List<ICurve>();

            var linearEdges = (offsetBody.GetEdges() as object[]).Cast<IEdge>()
                .Where(e => e.IGetCurve().IsLine()).ToArray();

            var findFarMostPointFunc = new Func<IEdge, double[], double[]>(
                (edge, startPt) =>
                {
                    var startVert = edge.IGetStartVertex().GetPoint() as double[];
                    var endVert = edge.IGetEndVertex().GetPoint() as double[];

                    var startDist = MathUtils.GetDistanceBetweenPoints(startVert, startPt);
                    var endDist = MathUtils.GetDistanceBetweenPoints(endVert, startPt);

                    if (startDist > endDist)
                    {
                        return startVert;
                    }
                    else
                    {
                        return endVert;
                    }
                });

            double[] prevEndPt = null;

            for (int i = 0; i < linearEdges.Length - 1; i++)
            {
                var firstEdge = linearEdges[i];
                var secondEdge = linearEdges[i + 1];

                var interPt = firstEdge.IGetCurve().GetIntersectionWithLinearCurve(secondEdge.IGetCurve());

                //TODO: check parallel curves

                var firstStartPt = findFarMostPointFunc.Invoke(firstEdge, interPt);
                var secondStartPt = findFarMostPointFunc.Invoke(secondEdge, interPt);

                var firstDir = new double[]
                {
                    firstStartPt[0] - interPt[0],
                    firstStartPt[1] - interPt[1],
                    firstStartPt[2] - interPt[2]
                };

                var secondDir = new double[]
                {
                    secondStartPt[0] - interPt[0],
                    secondStartPt[1] - interPt[1],
                    secondStartPt[2] - interPt[2]
                };

                var radius = outerRadius;

                if (i % 2 == 0)
                {
                    radius = innerRadius;
                }

                var firstVec = m_MathUtils.CreateVector(firstDir) as IMathVector;
                var secondVec = m_MathUtils.CreateVector(secondDir) as IMathVector;

                var ang = firstVec.GetAngle(secondVec);

                var dist = radius / Math.Sin(ang / 2);

                var bisecVec = firstVec.Normalise().Add(secondVec.Normalise()) as IMathVector;

                var centerPt = m_MathUtils.CreatePoint(interPt) as IMathPoint;
                centerPt = centerPt.MoveAlongVector(bisecVec, dist);

                var centrPtCoord = centerPt.ArrayData as double[];

                var createLineFunc = new Func<double[], double[], Curve>(
                    (s, e) =>
                    {
                        var line = m_Modeler.CreateLine(s,
                            new double[] { e[0] - s[0], e[1] - s[1], e[2] - s[2] }) as Curve;
                        line = line.CreateTrimmedCurve2(s[0], s[1], s[2], e[0], e[1], e[2]);
                        return line;
                    });

                var firstCurve = firstEdge.IGetCurve().GetBaseCurve();

                var firstEndPt = firstCurve.GetClosestPointOn(
                    centrPtCoord[0], centrPtCoord[1], centrPtCoord[2]) as double[];

                firstEndPt = new double[] { firstEndPt[0], firstEndPt[1], firstEndPt[2] };

                if (prevEndPt != null)
                {
                    firstStartPt = prevEndPt;
                }

                firstCurve = createLineFunc.Invoke(firstStartPt, firstEndPt);

                offsetCurves.Add(firstCurve);

                var secondCurve = secondEdge.IGetCurve().GetBaseCurve();
                var secondEndPt = secondCurve.GetClosestPointOn(centrPtCoord[0], centrPtCoord[1], centrPtCoord[2]) as double[];
                secondEndPt = new double[] { secondEndPt[0], secondEndPt[1], secondEndPt[2] };

                var isLast = i == linearEdges.Length - 2;

                if (isLast)
                {
                    secondCurve = createLineFunc.Invoke(secondStartPt, secondEndPt);

                    offsetCurves.Add(secondCurve);
                }

                ICurve filletCurve = null;

                var arcDir = (secondVec.Cross(firstVec) as IMathVector).ArrayData as double[];

                filletCurve = m_Modeler.CreateArc(centrPtCoord, arcDir,
                        radius, firstEndPt, secondEndPt) as ICurve;

                filletCurve = filletCurve.CreateTrimmedCurve2(firstEndPt[0], firstEndPt[1], firstEndPt[2],
                        secondEndPt[0], secondEndPt[1], secondEndPt[2]);

                prevEndPt = secondEndPt;

                offsetCurves.Add(filletCurve);
            }

            var offsetFilletBody = m_Modeler.CreateWireBody(offsetCurves.ToArray(),
                (int)swCreateWireBodyOptions_e.swCreateWireBodyByDefault);

            if (offsetFilletBody == null)
            {
                throw new UserException("Failed to generate the chain from offset result");
            }

            offsetBody = null;
            GC.Collect();

            return offsetFilletBody;
        }

        public void ConvertToSketchSegments(IModelDoc2 model, IBody2 body)
        {
            var transform = model.SketchManager.ActiveSketch.ModelToSketchTransform;

            model.SketchManager.AddToDB = true;

            foreach (var edge in (body.GetEdges() as object[]).Cast<IEdge>())
            {
                var curve = edge.IGetCurve();
                curve.ApplyTransform(transform);

                double start;
                double end;
                bool isClosed;
                bool isPeriodic;
                curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);

                var startPt = curve.Evaluate2(start, 0) as double[];
                var endPt = curve.Evaluate2(end, 0) as double[];

                if (curve.IsLine())
                {
                    model.SketchManager.CreateLine(startPt[0], startPt[1], startPt[2],
                        endPt[0], endPt[1], endPt[2]);
                }
                else if (curve.IsCircle())
                {
                    var circParam = curve.CircleParams as double[];
                    model.SketchManager.CreateArc(circParam[0], circParam[1], circParam[2],
                        startPt[0], startPt[1], startPt[2], endPt[0], endPt[1], endPt[2], (short)circParam[5]);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            model.SketchManager.AddToDB = false;
        }

        private int ConvertColor(KnownColor knownColor)
        {
            var color = Color.FromKnownColor(knownColor);

            return (color.R << 0) | (color.G << 8) | (color.B << 16);
        }

        public void HidePreview()
        {
            m_PreviewOffsetFilletBody = null;
            GC.Collect();
            m_Model.GraphicsRedraw2();
        }

        public void Dispose()
        {
            HidePreview();
        }
    }
}
