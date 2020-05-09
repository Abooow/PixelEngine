using System;
using PixelEngine.Utilities;

namespace PixelEngine
{
	public struct Point
	{
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

		public static Point Zero => new Point(0, 0);
		public static Point One => new Point(1, 1);
		public static Point Left => new Point(-1, 0);
		public static Point Right => new Point(1, 0);
		public static Point Up => new Point(0, -1);
		public static Point Down => new Point(0, 1);

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
