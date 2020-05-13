using System;
using PixelEngine.Utilities;

namespace PixelEngine
{
	public struct Point
	{
        private static Point zero = new Point(0, 0);
        private static Point one = new Point(1, 1);
        private static Point left = new Point(-1, 0);
        private static Point right = new Point(1, 0);
        private static Point up = new Point(0, -1);
        private static Point down = new Point(0, 1);

        public static readonly Point Zero = zero;
		public static readonly Point One = one;
		public static readonly Point Left = left;
		public static readonly Point Right = right;
		public static readonly Point Up = up;
		public static readonly Point Down = down;

        public int X { get; set; }
		public int Y { get; set; }

		public Point(int xy) : this(xy, xy)
		{
		}

		public Point(int x, int y) : this()
		{
			this.X = x;
			this.Y = y;
		}

		public Point Abs()
		{
			return new Point(Math.Abs(X), Math.Abs(Y));
		}

		public Vector ToVector()
		{
			return this;
		}

		public override string ToString()
		{
			return $"{{X: {X}, Y: {Y}}}";
		}

		public override bool Equals(object obj) => obj is Point p ? p == this : false;

		public override int GetHashCode()
		{
			int hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + X.GetHashCode();
			hashCode = hashCode * -1521134295 + Y.GetHashCode();
			return hashCode;
		}

		public static implicit operator Vector(Point p)
		{
			return new Vector(p.X, p.Y, 0);
		}

		public static Point operator +(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		public static Point operator +(Point a, int b)
		{
			return new Point(a.X + b, a.Y + b);
		}

		public static Point operator -(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}
		public static Point operator -(Point a, int b)
		{
			return new Point(a.X - b, a.Y - b);
		}

		public static Point operator *(Point a, int b)
		{
			return new Point(a.X * b, a.Y * b);
		}
		public static Point operator *(Point a, Point b)
		{
			return new Point(a.X * b.X, a.Y * b.Y);
		}

		public static Point operator /(Point a, Point b)
		{
			return new Point(a.X / b.X, a.Y / b.Y);
		}
		public static Point operator /(Point a, int b)
		{
			return new Point(a.X / b, a.Y / b);
		}

		public static bool operator ==(Point a, Point b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}
	}
}
