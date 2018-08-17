using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonSupport
{
    public class SpaceSnap
    {
        const int SnapSpace = 5;

        public SpaceSnap()
        { 
        }

        public PointF RepositionPoint(PointF input)
        {
            input.X = input.X / (float)SnapSpace;
            input.X = (float)System.Math.Round(input.X, MidpointRounding.ToEven);
            input.X *= SnapSpace;

            input.Y = input.Y / (float)SnapSpace;
            input.Y = (float)System.Math.Round(input.Y, MidpointRounding.ToEven);
            input.Y *= SnapSpace;

            return input;
        }
    }
}
