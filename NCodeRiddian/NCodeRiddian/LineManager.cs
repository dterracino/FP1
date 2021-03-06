﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace NCodeRiddian
{
    public abstract class LineManager
    {
        public static ColisionInfo LinesIntersect(Vector2 [] A, Vector2[] B)
        {
            return LinesIntersect(A[0], A[1], B[0], B[1]);
        }

        private static bool PointOnLineSSI(Vector2 A, Vector2 B, Vector2 point)
        {
            bool onX = (point.X >= Math.Min(A.X, B.X) && point.X <= Math.Max(A.X, B.X)) || FloatHelp.ApproxEquals(point.X, A.X, .001f) || FloatHelp.ApproxEquals(point.X, B.X, .001f);
            bool onY = (point.Y >= Math.Min(A.Y, B.Y) && point.Y <= Math.Max(A.Y, B.Y)) || FloatHelp.ApproxEquals(point.Y, A.Y, .001f) || FloatHelp.ApproxEquals(point.Y, B.Y, .001f);
            return onX && onY;
        }
        private static bool PointOnLineSSI(DBLV A, DBLV B, DBLV point)
        {
            bool onX = (point.X >= Math.Min(A.X, B.X) && point.X <= Math.Max(A.X, B.X)) || FloatHelp.ApproxEquals(point.X, A.X, .001) || FloatHelp.ApproxEquals(point.X, B.X, .001);
            bool onY = (point.Y >= Math.Min(A.Y, B.Y) && point.Y <= Math.Max(A.Y, B.Y)) || FloatHelp.ApproxEquals(point.Y, A.Y, .001) || FloatHelp.ApproxEquals(point.Y, B.Y, .001);
            return onX && onY;
        }
        public static Vector2 Scale(Vector2 start, Vector2 currentEnd, float perc)
        {
            return new Vector2(start.X + perc * (currentEnd.X - start.X), start.Y + perc * (currentEnd.Y - start.Y));
        }
        public static Vector2 StaticScale(Vector2 start, Vector2 currentEnd, float Amt)
        {
            Velocity temp = new Velocity(currentEnd.X - start.X, currentEnd.Y - start.Y, false);
            temp.M = Amt;
            return temp.Move(start);
        }

        public static ColisionInfo LinesIntersect(Vector2 Atmp, Vector2 Btmp, Vector2 Ctmp, Vector2 Dtmp)
        {
            DBLV A = new DBLV(Atmp), B = new DBLV(Btmp), C = new DBLV(Ctmp), D = new DBLV(Dtmp);

            DBLV line1 = ConvertToSI(A, B);
            DBLV line2 = ConvertToSI(C, D);

            // Coterminal?
            if(line1.Equals( line2 ))
            {
                if(PointOnLineSSI(A, B, C) || PointOnLineSSI(A, B, D))
                {
                    // C -> D -> B
                    DBLV Colision1 = (PointOnLineSSI(A, B, C) ? C : (PointOnLineSSI(A, B, D) ? D : B));
                    // A -> B -> D
                    DBLV Colision2 = (PointOnLineSSI(C, D, A) ? A : (PointOnLineSSI(C, D, B) ? B : D));
                    
                    return new ColisionInfo(true, 2, Colision1, Colision2);
                }
                return new ColisionInfo(false, 0, null);
            }

            DBLV pointOfIntersection = new DBLV();
            if (!double.IsInfinity(line1.X) && !double.IsInfinity(line2.X))
            {
                pointOfIntersection.X = (line1.Y - line2.Y) / (line2.X - line1.X);
                pointOfIntersection.Y = (line1.X * pointOfIntersection.X) + line1.Y;
            }
            else if (!double.IsInfinity(line2.X))
            {
                pointOfIntersection.X = A.X;
                pointOfIntersection.Y =(line2.X * pointOfIntersection.X) + line2.Y;
            }
            else if (!double.IsInfinity(line1.X))
            {
                pointOfIntersection.X = C.X;
                pointOfIntersection.Y =(line1.X * pointOfIntersection.X) + line1.Y;
            }
            
            if(PointOnLineSSI(A, B, pointOfIntersection) && PointOnLineSSI(C, D, pointOfIntersection))
            {
                if (pointOfIntersection.ApproxEquals( A, .001 ) || pointOfIntersection.ApproxEquals( B, .001 ) || pointOfIntersection.ApproxEquals( C, .001 ) || pointOfIntersection.ApproxEquals( D, .001 ))
                {
                    return new ColisionInfo(true, 1, pointOfIntersection);
                }
                return new ColisionInfo(true, 3, pointOfIntersection);
            }
            return new ColisionInfo(false, 0, null);
        }

        public static Vector2 ConvertToSI(Vector2 A, Vector2 B)
        {
            float slope = (B.Y - A.Y) / (B.X - A.X);
            float intersect = A.Y + (-A.X * slope);

            return new Vector2(slope, intersect);
        }
        private static DBLV ConvertToSI(DBLV A, DBLV B)
        {
            double slope = (B.Y - A.Y) / (B.X - A.X);
            double intersect = A.Y + (-A.X * slope);

            return new DBLV(slope, intersect);
        }

        public static Vector2 GetClosestPointOnLine(Vector2 point, Vector2 LineA, Vector2 LineB)
        {
            float A = point.X - LineA.X;
            float B = point.Y - LineA.Y;
            float C = LineB.X - LineA.X;
            float D = LineB.Y - LineA.Y;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            Vector2 tgt;

            if (param < 0)
            {
                tgt = LineA;
            }
            else if (param > 1)
            {
                tgt = LineB;
            }
            else
            {
                tgt = LineA + (new Vector2(C, D) * param);
            }
            return tgt;
        }
    }

    public struct ColisionInfo
    {
        public bool Intersect;
        // 0: No
        // 1: Single point on line
        // 2: Same SI
        // 3: Full colision
        public byte Type;
        public Vector2? IntersectionPointA;
        public Vector2? IntersectionPointB;

        public ColisionInfo(bool hit, byte type, Vector2? i1, Vector2? i2)
        {
            Intersect = hit;
            Type = type;
            IntersectionPointA = i1;
            IntersectionPointB = i2;
        }
        public ColisionInfo(bool hit, byte type, Vector2? i1)
        {
            Intersect = hit;
            Type = type;
            IntersectionPointA = i1;
            IntersectionPointB = i1;
        }
    }

    public struct DBLV
    {
        public double X;
        public double Y;
        public DBLV(double v)
        {
            X = v;
            Y = v;
        }
        public DBLV(double x, double y)
        {
            X = x;
            Y = y;
        }
        public DBLV(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public override bool Equals(object obj)
        {
            if(obj is DBLV)
            {
                DBLV other = (DBLV)obj;
                return other.X == X && other.Y == Y;
            }
            return false;
        }

        public bool ApproxEquals(DBLV obj, double margin)
        {
            return FloatHelp.ApproxEquals(this, obj, margin);
        }
        public static implicit operator Vector2(DBLV me)
        {
            return new Vector2((float)me.X, (float)me.Y);
        }
    }
    
}
