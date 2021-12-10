using System;
using System.Collections.Generic;
using System.Windows;

namespace ArcToCubicBezierSample
{
    public static class ArcToCubicBezierHelper
    {
        const double TAU = Math.PI * 2;

        private static (double x, double y) MapToEllipse(double x, double y, double rx, double ry, double cosPhi, double sinPhi, double centerX, double centerY)
        {
            x *= rx;
            y *= ry;

            var xp = cosPhi * x - sinPhi * y;
            var yp = sinPhi * x + cosPhi * y;

            return (xp + centerX, yp + centerY);
        }

        private static (double centeRx, double centeRy, double ang1, double ang2) GetArcCenter
            (double px, double py, double cx, double cy, double rx, double ry, bool largeArcFlag, bool sweepFlag, double sinphi, double cosphi, double pxp, double pyp)
        {
            var rxsq = Math.Pow(rx, 2);
            var rysq = Math.Pow(ry, 2);
            var pxpsq = Math.Pow(pxp, 2);
            var pypsq = Math.Pow(pyp, 2);

            var radicant = (rxsq * rysq) - (rxsq * pypsq) - (rysq * pxpsq);
            if (radicant < 0)
            {
                radicant = 0;
            }

            radicant /= (rxsq * pypsq) + (rysq * pxpsq);
            radicant = Math.Sqrt(radicant) * (largeArcFlag == sweepFlag ? -1 : 1);

            var centerxp = radicant * rx / ry * pyp;
            var centeryp = radicant * -ry / rx * pxp;

            var centeRx = cosphi * centerxp - sinphi * centeryp + (px + cx) / 2;
            var centeRy = sinphi * centerxp + cosphi * centeryp + (py + cy) / 2;

            var vx1 = (pxp - centerxp) / rx;
            var vy1 = (pyp - centeryp) / ry;
            var vx2 = (-pxp - centerxp) / rx;
            var vy2 = (-pyp - centeryp) / ry;

            var ang1 = VectorAngle(1, 0, vx1, vy1);
            var ang2 = VectorAngle(vx1, vy1, vx2, vy2);

            if (!sweepFlag && ang2 > 0)
            {
                ang2 -= TAU;
            }

            if (sweepFlag && ang2 < 0)
            {
                ang2 += TAU;
            }

            return (centeRx, centeRy, ang1, ang2);

        }


        public static (double x1, double y1, double x2, double y2, double x, double y) ArcToBezier(double px, double py, double cx, double cy, double rx, double ry, double xAxisRotation = 0, bool largeArcFlag = false, bool sweepFlag = false)
        {

            if (rx == 0 || ry == 0)
            {
                return (0, 0, 0, 0, 0, 0);
            }

            var sinphi = Math.Sin(xAxisRotation * TAU / 360);
            var cosphi = Math.Cos(xAxisRotation * TAU / 360);

            var pxp = cosphi * (px - cx) / 2 + sinphi * (py - cy) / 2;
            var pyp = -sinphi * (px - cx) / 2 + cosphi * (py - cy) / 2;

            if (pxp == 0 && pyp == 0)
            {
                return (0, 0, 0, 0, 0, 0);
            }

            rx = Math.Abs(rx);
            ry = Math.Abs(ry);

            var lambda = Math.Pow(pxp, 2) / Math.Pow(rx, 2) + Math.Pow(pyp, 2) / Math.Pow(ry, 2);

            if (lambda > 1)
            {
                rx *= Math.Sqrt(lambda);
                ry *= Math.Sqrt(lambda);
            }

            var (centerRx, centerRy, ang1, ang2) = GetArcCenter(px, py, cx, cy, rx, ry, largeArcFlag, sweepFlag, sinphi, cosphi, pxp, pyp);

            // If 'ang2' == 90.0000000001, then `ratio` will evaluate to
            // 1.0000000001. This causes `segments` to be greater than one, which is an
            // unecessary split, and adds extra points to the bezier curve. To alleviate
            // this issue, we round to 1.0 when the ratio is close to 1.0.
            var ratio = Math.Abs(ang2) / (TAU / 4);
            if (Math.Abs(1.0 - ratio) < 0.0000001)
            {
                ratio = 1.0;
            }

            var segments = Math.Max(Math.Ceiling(ratio), 1);

            ang2 /= segments;

            var points = new List<Point>();

            for (var i = 0; i <= segments; i++)
            {
                var (_, _, _, _, x, y) = ApproxUnitArc(ang1, ang2);
                var (ellipseX, ellipseY) = MapToEllipse(x, y, rx, ry, cosphi, sinphi, centerRx, centerRy);
                points.Add(new Point(ellipseX, ellipseY));
                ang1 += ang2;
            };

            //var (x1, y1) = MapToEllipse(rx, ry, cosphi, sinphi, centerRx, centerRy);
            //var (x2, y2) = MapToEllipse(rx, ry, cosphi, sinphi, centerRx, centerRy);
            //var (x, y) = MapToEllipse(rx, ry, cosphi, sinphi, centerRx, centerRy);

            return (points[0].X, points[0].Y, points[1].X, points[1].Y, points[2].X, points[2].Y);
        }


        static (double x1, double y1, double x2, double y2, double x, double y) ApproxUnitArc(double ang1, double ang2)
        {
            // If 90 degree circular arc, use a constant
            // as derived from http://spencermortensen.com/articles/bezier-circle
            var a = ang2 == 1.5707963267948966 ? 0.551915024494 : ang2 == -1.5707963267948966 ? -0.551915024494 : 4 / 3 * Math.Tan(ang2 / 4);

            var x1 = Math.Cos(ang1);
            var y1 = Math.Sin(ang1);
            var x2 = Math.Cos(ang1 + ang2);
            var y2 = Math.Sin(ang1 + ang2);

            return (x1 - y1 * a, y1 + x1 * a, x2 + y2 * a, y2 - x2 * a, x2, y2);
        }


        static double VectorAngle(double ux, double uy, double vx, double vy)
        {
            var sign = (ux * vy - uy * vx < 0) ? -1 : 1;

            var dot = ux * vx + uy * vy;

            if (dot > 1)
            {
                dot = 1;
            }

            if (dot < -1)
            {
                dot = -1;
            }

            return sign * Math.Acos(dot);
        }
    }
}
