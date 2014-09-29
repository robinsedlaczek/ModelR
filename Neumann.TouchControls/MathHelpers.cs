using System;
using System.Windows;

namespace Neumann.TouchControls
{
    public static class MathHelpers
    {
        public static double AngleFromPoint(Point origin, Point pos)
        {
            var angle = ((Math.Atan2(origin.Y - pos.Y, pos.X - origin.X) * (180.0 / Math.PI)) - 90) * -1;
            if (angle < 0)
                angle = 360 + angle;
            return angle;
        }
    }
}
