using System;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;

namespace SpaceDefence
{

    public class LinePieceCollider : Collider, IEquatable<LinePieceCollider>
    {

        public Vector2 Start;
        public Vector2 End;

        /// <summary>
        /// How long this line segment is. Setting this value stretches or shrinks the line from the Start point.
        /// </summary>
        public float Length
        {
            get
            {
                return (End - Start).Length();
            }
            set
            {
                End = Start + GetDirection() * value;
            }
        }

        /// <summary>
        /// The A value in the line equation Ax + By + C = 0 (used for collision math).
        /// </summary>
        public float StandardA
        {
            get
            {
                return End.Y - Start.Y;
            }
        }

        /// <summary>
        /// The B value in the line equation Ax + By + C = 0 (used for collision math).
        /// </summary>
        public float StandardB
        {
            get
            {
                return Start.X - End.X;
            }
        }

        /// <summary>
        /// The C value in the line equation Ax + By + C = 0 (used for collision math).
        /// </summary>
        public float StandardC
        {
            get
            {
                return -(StandardA * Start.X + StandardB * Start.Y);
            }
        }

        public LinePieceCollider(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public LinePieceCollider(Vector2 start, Vector2 direction, float length)
        {
            Start = start;
            End = start + direction * length;
        }

        /// <summary>
        /// Converts a direction vector into an angle (in radians), measured from straight up.
        /// </summary>
        /// <param name="direction">The direction to measure.</param>
        /// <returns>The angle in radians.</returns>
        public static float GetAngle(Vector2 direction)
        {
            return (float)Math.Atan2(direction.X, -direction.Y);
        }


        /// <summary>
        /// Finds the direction from one point to another and normalizes it (makes it length 1).
        /// </summary>
        /// <returns>A normalized vector pointing from point1 toward point2.</returns>
        public static Vector2 GetDirection(Vector2 point1, Vector2 point2)
        {
            // Calculate the normalized vector pointing from point1 to point2
            Vector2 diff = point2 - point1;
            float len = diff.Length();
            if (len <= 0f)
                return -Vector2.UnitY;
            return diff / len;
        }


        /// <summary>
        /// Checks if this line segment overlaps with another line segment.
        /// </summary>
        /// <param name="other">The line segment to check.</param>
        /// <returns>True if they intersect, false otherwise.</returns>
        public override bool Intersects(LinePieceCollider other)
        {
            return SegmentsIntersect(this, other);
        }


        /// <summary>
        /// Checks if this line segment touches or crosses a circle.
        /// </summary>
        /// <param name="other">The circle to check.</param>
        /// <returns>True if they overlap, false otherwise.</returns>

        public override bool Intersects(CircleCollider other)
        {
            // Find the nearest point on this line segment to the circle's center
            Vector2 nearest = NearestPointOnLine(other.Center);

            // If the distance from that point to the circle center is less than
            // the radius, the line is inside or touching the circle
            float distance = (nearest - other.Center).Length();
            return distance <= other.Radius;
        }

        /// <summary>
        /// Checks if this line segment overlaps with a rectangle.
        /// </summary>
        /// <param name="other">The rectangle to check.</param>
        /// <returns>True if they overlap, false otherwise.</returns>
        public override bool Intersects(RectangleCollider other)
        {
            // Quick check: is either endpoint inside the rectangle?
            if (other.shape.Contains(Start.ToPoint()) || other.shape.Contains(End.ToPoint()))
                return true;

            // Build the 4 edges of the rectangle as line segments
            Vector2 topLeft = new Vector2(other.shape.Left, other.shape.Top);
            Vector2 topRight = new Vector2(other.shape.Right, other.shape.Top);
            Vector2 bottomLeft = new Vector2(other.shape.Left, other.shape.Bottom);
            Vector2 bottomRight = new Vector2(other.shape.Right, other.shape.Bottom);

            LinePieceCollider topEdge = new LinePieceCollider(topLeft, topRight);
            LinePieceCollider bottomEdge = new LinePieceCollider(bottomLeft, bottomRight);
            LinePieceCollider leftEdge = new LinePieceCollider(topLeft, bottomLeft);
            LinePieceCollider rightEdge = new LinePieceCollider(topRight, bottomRight);

            // Check our segment against each edge using the proper two-way straddle test
            if (SegmentsIntersect(this, topEdge)) return true;
            if (SegmentsIntersect(this, bottomEdge)) return true;
            if (SegmentsIntersect(this, leftEdge)) return true;
            if (SegmentsIntersect(this, rightEdge)) return true;

            return false;
        }

        /// <summary>
        /// Finds where two infinite lines cross each other (ignoring segment endpoints).
        /// </summary>
        /// <param name="Other">The other line to intersect with.</param>
        /// <returns>The point where they cross, or Vector2.Zero if they're parallel.</returns>
        public Vector2 GetIntersection(LinePieceCollider Other)
        {
            float a1 = StandardA, b1 = StandardB, c1 = StandardC;
            float a2 = Other.StandardA, b2 = Other.StandardB, c2 = Other.StandardC;

            // If denominator is 0 the lines are parallel and never meet
            float denominator = a1 * b2 - a2 * b1;
            if (denominator == 0)
                return Vector2.Zero;

            float x = (b1 * c2 - b2 * c1) / denominator;
            float y = (a2 * c1 - a1 * c2) / denominator;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Finds the closest point on this line segment to a given position.
        /// </summary>
        /// <param name="other">The position to find the nearest point to.</param>
        /// <returns>The closest point on the line segment.</returns>

        public Vector2 NearestPointOnLine(Vector2 other)
        {
            // The vector that runs along the line from Start to End
            Vector2 segment = End - Start;

            // The vector from Start to our query point
            Vector2 toPoint = other - Start;

            // Squared length of the segment (we use squared to avoid an extra sqrt)
            float segLengthSquared = segment.LengthSquared();

            // Degenerate case: Start and End are the same point
            if (segLengthSquared == 0f)
                return Start;

            // t is how far along the segment the nearest point is, as a 0-to-1 fraction.
            // Dot product gives the "shadow" of toPoint onto the segment direction.
            float t = Vector2.Dot(toPoint, segment) / segLengthSquared;

            // Clamp to [0, 1] so the nearest point stays between Start and End
            t = Math.Clamp(t, 0f, 1f);

            // The actual nearest point in world space
            return Start + segment * t;
        }

        /// <summary>
        /// Gets the smallest rectangle (axis-aligned) that completely contains this line segment.
        /// </summary>
        /// <returns>A bounding box rectangle.</returns>
        public override Rectangle GetBoundingBox()
        {
            Point topLeft = new Point((int)Math.Min(Start.X, End.X), (int)Math.Min(Start.Y, End.Y));
            Point size = new Point((int)Math.Max(Start.X, End.X), (int)Math.Max(Start.Y, End.Y)) - topLeft;
            return new Rectangle(topLeft, size);
        }


        /// <summary>
        /// Checks if a point lies on this line segment (with small tolerance for floating point errors).
        /// </summary>
        /// <param name="coordinates">The point to check.</param>
        /// <returns>True if the point is on the line, false otherwise.</returns>
        public override bool Contains(Vector2 coordinates)
        {
            Vector2 nearest = NearestPointOnLine(coordinates);
            return (nearest - coordinates).Length() < 0.01f;
        }

        public bool Equals(LinePieceCollider other)
        {
            return other.Start == this.Start && other.End == this.End;
        }

        /// <summary>
        /// Finds the normalized direction between two points.
        /// </summary>
        /// <returns>A normalized vector pointing from point1 toward point2.</returns>
        public static Vector2 GetDirection(Point point1, Point point2)
        {
            return GetDirection(point1.ToVector2(), point2.ToVector2());
        }


        /// <summary>
        /// Gets the normalized direction this line segment is pointing.
        /// </summary>
        /// <returns>A unit vector pointing from Start toward End.</returns>
        public Vector2 GetDirection()
        {
            return GetDirection(Start, End);
        }


        /// <summary>
        /// Gets the angle this line segment is pointing, measured from straight up.
        /// </summary>
        /// <returns>The angle in radians.</returns>
        public float GetAngle()
        {
            return GetAngle(GetDirection());
        }

        // Private helpers.

        private static float Side(LinePieceCollider line, Vector2 point)
        {
            return line.StandardA * point.X + line.StandardB * point.Y + line.StandardC;
        }

        private static bool SegmentsIntersect(LinePieceCollider a, LinePieceCollider b)
        {
            float sideAStart = Side(b, a.Start);
            float sideAEnd = Side(b, a.End);
            float sideBStart = Side(a, b.Start);
            float sideBEnd = Side(a, b.End);

            // Each pair must have opposite signs (one positive, one negative)
            // meaning they are on opposite sides of the other segment's line
            bool aStraddles = sideAStart * sideAEnd < 0;
            bool bStraddles = sideBStart * sideBEnd < 0;

            return aStraddles && bStraddles;
        }
    }
}
