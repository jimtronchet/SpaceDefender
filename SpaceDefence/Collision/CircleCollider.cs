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
            // Edge case 1: Cirkel staat naast (boven/onder of links/rechts van) de rechthoek
            // if cy < Ab && cy > At => cx + r > Al && cx - r < Ar
            if (Center.Y < other.shape.Bottom && Center.Y > other.shape.Top)
                if (Center.X + Radius > other.shape.Left && Center.X - Radius < other.shape.Right)
                    return true;

            // if cx < Ar && cx > Al => cy + r > At && cy - r < Ab
            if (Center.X < other.shape.Right && Center.X > other.shape.Left)
                if (Center.Y + Radius > other.shape.Top && Center.Y - Radius < other.shape.Bottom)
                    return true;

            // Edge case 2: Cirkel staat diagonaal
            // kijk of de hoeken van rectangle in de cirkel vallen
            Vector2[] corners = new Vector2[]
            {
                new Vector2(other.shape.Left,  other.shape.Top),
                new Vector2(other.shape.Right, other.shape.Top),
                new Vector2(other.shape.Left,  other.shape.Bottom),
                new Vector2(other.shape.Right, other.shape.Bottom),
            };

            foreach (Vector2 corner in corners)
                if ((corner - Center).Length() < Radius)
                    return true; // als een hoek in de cirkel valt, dan is er een overlap

            return false;
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
