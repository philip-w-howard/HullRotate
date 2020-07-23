using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HullRotate
{
    class Geometry
    {
         public Geometry() { }

        // P1 = bottom of bulkhead
        // P2 = top of bulkhead (upper or outer most point)
        // p3 = top center of bulkhead (imaginary point)
        // Set Z3 = Z2
        // Compute line P2 -> P3
        // Compute line P1 -> perpendicular to P2->P3
        //     This line defines the plane for the bulkhead
        //
        // Update x,y from front view. Recompute Z from plane
        //    Pick which bulkhead, then update x,y
        //    May need to special case if you update the bottom point
        // Update Y from side view
        // Update X from top view
        // Update bulkhead 0 from side view.
        //    Set Y,Z. X is always zero


        // Keep track of centerline vector.
        // Compute Rotate-to-centerline in terms of three rotations: X,Y,Z
    }
}
