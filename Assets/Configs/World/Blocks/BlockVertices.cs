using System.Collections.Generic;
using UnityEngine;

namespace Configs.World.Blocks
{
    public class BlockVertices
    {
        // F(ront), B(ack), D(own), T(op), L(eft), R(ight)
        public static readonly Vector3 FDL = new (0,0,1); // Front-down-left point
        public static readonly Vector3 FDR = new (1,0,1);
        public static readonly Vector3 FTL = new (0,1,1);
        public static readonly Vector3 FTR = new (1,1,1);
        public static readonly Vector3 BDL = new (0,0,0);
        public static readonly Vector3 BDR = new (1,0,0);
        public static readonly Vector3 BTL = new (0,1,0);
        public static readonly Vector3 BTR = new (1,1,0);

        public static List<Vector3> FRONT => new () 
        {
            FDL,
            FDR,
            FTL,
            FTR,
        };
        public static List<Vector3> BACK => new () 
        {
            BTL,
            BTR,
            BDL,
            BDR,
        };
        public static List<Vector3> LEFT => new () 
        {
            FDL,
            FTL,
            BDL,
            BTL,
        };
        public static List<Vector3> RIGHT => new () 
        {
            BDR,
            BTR,
            FDR,
            FTR,
        };
        public static List<Vector3> TOP => new () 
        {
            BTR,
            BTL,
            FTR,
            FTL,
        };
        public static List<Vector3> DOWN => new () 
        {
            BDL,
            BDR,
            FDL,
            FDR,
        };
    }
}