using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine.Utilities;

namespace PixelEngine
{
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Point position, int size)
        {
            X = position.X;
            Y = position.Y;
            Width = size;
            Height = size;
        }

        public Rectangle(Point position, Point size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;
        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public Point Size {
            get => new Point(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}}}";
        }

        public override bool Equals(object obj)
        {
            return (obj is Rectangle r) ? this == r : false;
        }

        public override int GetHashCode()
        {
            return (Y ^ X) + (Width ^ Height);
        }

        public bool Contains(Vector point)
        {
            return !((point.X < Left || point.X > Right) || (point.Y < Top || point.Y > Bottom));
        }

        public bool Intersects(Rectangle other)
        {
            return !((other.Right < Left || other.Left >= Right) || (other.Bottom < Top || other.Top >= Bottom));
        }

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Position == b.Position && a.Size == b.Size;
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !(a == b);
        }
    }
}
