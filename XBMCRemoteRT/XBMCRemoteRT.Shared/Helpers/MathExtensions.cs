using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XBMCRemoteRT.Helpers
{
    public class MathExtension
    {
        public static int CurrentStep(double d, int stepSize)
        {
            return (int)(Math.Floor(d / stepSize)) * stepSize;
        }

        public static int UpperStep(double d, int stepSize)
        {
            return CurrentStep(d, stepSize) + stepSize;
        }

        public static int LowerStep(double d, int stepSize)
        {
            return CurrentStep(d, stepSize) - stepSize;
        }      
    }
}
