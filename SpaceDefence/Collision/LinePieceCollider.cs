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
        /// The length of the LinePiece, changing the length moves the end vector to adjust the length.
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
        /// The A component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardA
        {
            get
            {
                return End.Y - Start.Y;
            }
        }

        /// <summary>
        /// The B component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardB
        {
            get
            {
                return Start.X - End.X;
            }
        }

        /// <summary>
        /// The C component from the standard line formula Ax + By + C = 0
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
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the the up vector and the direction to the cursor.</returns>
        public static float GetAngle(Vector2 direction)
        {
            return (float)Math.Atan2(direction.X, -direction.Y);
        }


        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
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
        /// Gets whether or not the Line intersects another Line
        /// </summary>
        /// <param name="other">The Line to check for intersection</param>
        /// <returns>true there is any overlap between the Circle and the Line.</returns>
        public override bool Intersects(LinePieceCollider other)
        {
            return SegmentsIntersect(this, other);
        }


        /// <summary>
        /// Gets whether or not the line intersects a Circle.
        /// </summary>
        /// <param name="other">The Circle to check for intersection.</param>
        /// <returns>true there is any overlap between the two Circles.</returns>

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
        /// Gets whether or not the Line intersects the Rectangle.
        /// </summary>
        /// <param name="other">The Rectangle to check for intersection.</param>
        /// <returns>true there is any overlap between the Circle and the Rectangle.</returns>
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
        /// Calculates the intersection point between 2 lines.
        /// </summary>
        /// <param name="Other">The line to intersect with</param>
        /// <returns>A Vector2 with the point of intersection.</returns>
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
        /// Finds the nearest point on a line to a given vector, taking into account if the line is .
        /// </summary>
        /// <param name="other">The Vector you want to find the nearest point to.</param>
        /// <returns>The nearest point on the line.</returns>

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
        /// Returns the enclosing Axis Aligned Bounding Box containing the control points for the line.
        /// As an unbound line has infinite length, the returned bounding box assumes the line to be bound.
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBoundingBox()
        {
            Point topLeft = new Point((int)Math.Min(Start.X, End.X), (int)Math.Min(Start.Y, End.Y));
            Point size = new Point((int)Math.Max(Start.X, End.X), (int)Math.Max(Start.Y, End.Y)) - topLeft;
            return new Rectangle(topLeft, size);
        }


        /// <summary>
        /// Gets whether or not the provided coordinates lie on the line.
        /// </summary>
        /// <param name="coordinates">The coordinates to check.</param>
        /// <returns>true if the coordinates are within the circle.</returns>
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
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public static Vector2 GetDirection(Point point1, Point point2)
        {
            return GetDirection(point1.ToVector2(), point2.ToVector2());
        }


        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public Vector2 GetDirection()
        {
            return GetDirection(Start, End);
        }


        /// <summary>
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the the up vector and the direction to the cursor.</returns>
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
