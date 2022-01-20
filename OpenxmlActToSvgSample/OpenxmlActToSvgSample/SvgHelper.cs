using System;

namespace OpenxmlActToSvgSample
{
    public static class SvgHelper
    {
        public static double Radian(double ux, double uy, double vx, double vy)
        {
            var dot = ux * vx + uy * vy;
            var mod = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            var rad = Math.Acos(dot / mod);
            if (ux * vy - uy * vx < 0.0)
            {
                rad = -rad;
            }
            return rad;
        }

        // svg : [A | a] (rx ry x-axis-rotation large-arc-flag sweep-flag x y)+
        // x1 y1 rx ry φ fA fS x2 y2
        // sample :  svgArcToCenterParam(200,200,50,50,0,1,1,300,200)
        public static (double cx, double cy, double startAngle, double deltaAngle, double endAngle, bool clockwise) SvgArcToCenterParam(double x1, double y1, double rx, double ry, double phi, double fA, double fS, double x2, double y2)
        {
            double cx, cy, startAngle, deltaAngle, endAngle;
            var PIx2 = Math.PI * 2.0;

            if (rx < 0)
            {
                rx = -rx;
            }
            if (ry < 0)
            {
                ry = -ry;
            }
            if (rx == 0.0 || ry == 0.0)
            { // invalid arguments
                throw new Exception("rx and ry can not be 0");
            }

            var s_phi = Math.Sin(phi);
            var c_phi = Math.Cos(phi);
            var hd_x = (x1 - x2) / 2.0; // half diff of x
            var hd_y = (y1 - y2) / 2.0; // half diff of y
            var hs_x = (x1 + x2) / 2.0; // half sum of x
            var hs_y = (y1 + y2) / 2.0; // half sum of y

            // F6.5.1
            var x1_ = c_phi * hd_x + s_phi * hd_y;
            var y1_ = c_phi * hd_y - s_phi * hd_x;

            // F.6.6 Correction of out-of-range radii
            //   Step 3: Ensure radii are large enough
            var lambda = (x1_ * x1_) / (rx * rx) + (y1_ * y1_) / (ry * ry);
            if (lambda > 1)
            {
                rx = rx * Math.Sqrt(lambda);
                ry = ry * Math.Sqrt(lambda);
            }

            var rxry = rx * ry;
            var rxy1_ = rx * y1_;
            var ryx1_ = ry * x1_;
            var sum_of_sq = rxy1_ * rxy1_ + ryx1_ * ryx1_; // sum of square
            if (sum_of_sq == 0)
            {
                throw new Exception("start point can not be same as end point");
            }
            var coe = Math.Sqrt(Math.Abs((rxry * rxry - sum_of_sq) / sum_of_sq));
            if (fA == fS) { coe = -coe; }

            // F6.5.2
            var cx_ = coe * rxy1_ / ry;
            var cy_ = -coe * ryx1_ / rx;

            // F6.5.3
            cx = c_phi * cx_ - s_phi * cy_ + hs_x;
            cy = s_phi * cx_ + c_phi * cy_ + hs_y;

            var xcr1 = (x1_ - cx_) / rx;
            var xcr2 = (x1_ + cx_) / rx;
            var ycr1 = (y1_ - cy_) / ry;
            var ycr2 = (y1_ + cy_) / ry;

            // F6.5.5
            startAngle = Radian(1.0, 0.0, xcr1, ycr1);

            // F6.5.6
            deltaAngle = Radian(xcr1, ycr1, -xcr2, -ycr2);
            while (deltaAngle > PIx2) { deltaAngle -= PIx2; }
            while (deltaAngle < 0.0) { deltaAngle += PIx2; }
            if (fS == 0) { deltaAngle -= PIx2; }
            endAngle = startAngle + deltaAngle;
            while (endAngle > PIx2) { endAngle -= PIx2; }
            while (endAngle < 0.0) { endAngle += PIx2; }

            return (cx, cy, startAngle, deltaAngle, endAngle, fS == 1);
        }
    }
}
