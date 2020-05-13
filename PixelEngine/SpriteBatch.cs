using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine.Extensions.Transforms;
using PixelEngine.Utilities;

namespace PixelEngine
{
    public class SpriteBatch
    {
		private Sprite pixel;

		private bool hasBegun;

		//private Sprite buffer;
		private SpriteBatchItem[] batchItemList;
		private int batchItemCapacity;
		private int batchItemCount;

		private Game game;

		private PixelMode colorMode;
		private SortMode sortMode;
		private Shader shader;
		private Transform transform;

		public SpriteBatch(Game game, int capacity = 256)
		{
			if (capacity <= 0) capacity = 256;

			this.game = game;

			//buffer = new Sprite(game.ScreenWidth, game.ScreenHeight);
			batchItemCapacity = capacity;
			batchItemList = new SpriteBatchItem[capacity];

			pixel = new Sprite(1, 1, Color.White);
		}

		public void Begin(SortMode sortMode = SortMode.Deferred, PixelMode colorMode = PixelMode.Normal, Shader shader = null, Transform transform = null)
		{
			if (hasBegun)
				throw new Exception("A SpriteBatch has already begun.");

			hasBegun = true;

			this.sortMode = sortMode;
			this.colorMode = colorMode;
			this.shader = shader;
			this.transform = transform == null ? new Transform() : transform;
		}

		public void End()
		{
			hasBegun = false;
			if (batchItemCount == 0) return;

			int indexStart = 0;

			switch (sortMode)
			{
				case SortMode.BackToFront:
					Array.Sort(batchItemList, 0, batchItemCount);
					break;

				case SortMode.FrontToBack:
					Array.Sort(batchItemList, 0, batchItemCount);
					Array.Reverse(batchItemList);
					indexStart = batchItemCapacity - batchItemCount;
					break;
			}

			for (int i = indexStart; i < indexStart + batchItemCount; i++)
			{
				SpriteBatchItem item = batchItemList[i];

				PixelMode oldMode = game.PixelMode;
				game.PixelMode = colorMode;
				Transform newTransform = transform.Copy();
				if (item.Scale.X != 1 && item.Scale.Y != 1) newTransform.Scale(item.Scale.X, item.Scale.Y);
				if (item.Rotation != 0) newTransform.Rotate(item.Rotation);
				newTransform.Translate(item.Position.X, item.Position.Y);
				Transform.DrawSprite(item.Sprite, newTransform, item.Color);
				game.PixelMode = oldMode;
			}


			batchItemCount = 0;
		}

		public void Draw(Vector position, Color color, float layerDepth = 0f)
		{
			CreateBatchItem(position, Vector.Unity2, 0f, pixel, color, layerDepth);
		}

		public void DrawFilledRect(Vector position, Vector size, Color color, float rotation, float layerDepth = 0f)
		{
			CreateBatchItem(position, size, rotation, pixel, color, layerDepth);
		}

		private void CreateBatchItem(Vector position, Vector scale, float rotation, Sprite Sprite, Color color, float SortKey)
		{
			if (batchItemCount == batchItemCapacity) return;

			batchItemList[batchItemCount] = new SpriteBatchItem(position, scale, rotation, Sprite, color,  SortKey);
			batchItemCount++;
		}
	}

	internal class SpriteBatchItem : IComparable<SpriteBatchItem>
	{
		public readonly Vector Position;
		public readonly Vector Scale;
		public readonly float Rotation;
		public readonly Sprite Sprite;
		public readonly Color Color;
		public readonly float SortKey;

		public SpriteBatchItem(Vector position, Vector scale, float rotation, Sprite sprite, Color color, float sortKey)
		{
			Position = position;
			Scale = scale;
			Rotation = rotation;
			Sprite = sprite;
			Color = color;
			SortKey = sortKey;
		}

		public int CompareTo(SpriteBatchItem other)
		{
			return this.SortKey.CompareTo(other.SortKey);
		}
	}
}
