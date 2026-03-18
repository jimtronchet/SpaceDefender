using System;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;

namespace SpaceDefence
{
    public class CircleCollider : Collider, IEquatable<CircleCollider>
    {
        public float X;
        public float Y;
        public Vector2 Center
        {
            get
            {
                return new Vector2(X, Y);
            }

            set
            {
                X = value.X; Y = value.Y;
            }
        }
        public float Radius;

        /// <summary>
        /// Creates a circle for collision detection at the given position and size.
        /// </summary>
        /// <param name="x">X position of the circle center</param>
        /// <param name="y">Y position of the circle center</param>
        /// <param name="radius">How big the circle is</param>
        public CircleCollider(float x, float y, float radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }

        /// <summary>
        /// Creates a circle for collision detection at the given position and size.
        /// </summary>
        /// <param name="center">Center position of the circle</param>
        /// <param name="radius">How big the circle is</param>
        public CircleCollider(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }


        /// <summary>
        /// Checks if a point is inside this circle.
        /// </summary>
        /// <param name="coordinates">The point to test</param>
        /// <returns>True if the point is inside the circle</returns>
        public override bool Contains(Vector2 coordinates)
        {
            return (Center - coordinates).Length() < Radius;
        }

        /// <summary>
        /// Checks if this circle overlaps with another circle.
        /// </summary>
        /// <param name="other">The other circle to test</param>
        /// <returns>True if the circles are touching or overlapping</returns>
        public override bool Intersects(CircleCollider other)
        {
            float distance = (Center - other.Center).Length();
            return distance < Radius + other.Radius;
        }



        /// <summary>
        /// Checks if this circle overlaps with a rectangle.
        /// </summary>
        /// <param name="other">The rectangle to test</param>
        /// <returns>True if the circle and rectangle are touching or overlapping</returns>
        public override bool Intersects(RectangleCollider other)
        {
            // Clamp the circle's center to the rectangle's edges to get the nearest point
            float nearestX = Math.Clamp(X, other.shape.Left, other.shape.Right);
            float nearestY = Math.Clamp(Y, other.shape.Top, other.shape.Bottom);

            // If that nearest point is within the radius, we have an intersection
            float distanceX = X - nearestX;
            float distanceY = Y - nearestY;
            float distanceSquared = distanceX * distanceX + distanceY * distanceY;

            return distanceSquared < Radius * Radius;
        }
        /// <summary>
        /// Checks if this circle overlaps with a line.
        /// </summary>
        /// <param name="other">The line to test</param>
        /// <returns>True if the circle and line are touching or overlapping</returns>
        public override bool Intersects(LinePieceCollider other)
        {
            // Implemented in the line code.
            return other.Intersects(this);
        }

        /// <summary>
        /// Gets the rectangular box that fully contains this circle.
        /// </summary>
        /// <returns>The bounding box rectangle</returns>
        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)(X - Radius), (int)(Y - Radius), (int)(2 * Radius), (int)(2 * Radius));
        }

        public bool Equals(CircleCollider other)
        {
            return other.X == X && other.Y == Y && other.Radius == Radius;
        }
    }
}
