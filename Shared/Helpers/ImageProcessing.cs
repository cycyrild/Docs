using SkiaSharp;

namespace DocsWASM.Shared
{
	public class ImageProcessing
	{


		public static byte[] ImgToWebP(byte[] ImgBin, int SizeRatio, int Quality, SKFilterQuality FilterQuality = SKFilterQuality.Medium)
		{
			SKEncodedOrigin orientation;
			using MemoryStream ms = new MemoryStream(ImgBin);
			using (var inputStream = new SKManagedStream(ms))
			using (var codec = SKCodec.Create(inputStream))
				orientation = codec.EncodedOrigin;
			ms.Position = 0;
			using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

			var res = GetNewResolution(sourceBitmap.Width, sourceBitmap.Height, SizeRatio);
			using SKBitmap scaledBitmap = HandleOrientation(sourceBitmap.Resize(new SKImageInfo(res.width, res.height), FilterQuality), orientation);
			using SKData data = scaledBitmap.Encode(SKEncodedImageFormat.Webp, Quality);
			return data.ToArray();
		}

		public static SKBitmap HandleOrientation(SKBitmap bitmap, SKEncodedOrigin orientation)
		{
			SKBitmap rotated;
			switch (orientation)
			{
				case SKEncodedOrigin.BottomRight:

					using (var surface = new SKCanvas(bitmap))
					{
						surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
						surface.DrawBitmap(bitmap.Copy(), 0, 0);
					}

					return bitmap;

				case SKEncodedOrigin.RightTop:
					rotated = new SKBitmap(bitmap.Height, bitmap.Width);

					using (var surface = new SKCanvas(rotated))
					{
						surface.Translate(rotated.Width, 0);
						surface.RotateDegrees(90);
						surface.DrawBitmap(bitmap, 0, 0);
					}

					return rotated;

				case SKEncodedOrigin.LeftBottom:
					rotated = new SKBitmap(bitmap.Height, bitmap.Width);

					using (var surface = new SKCanvas(rotated))
					{
						surface.Translate(0, rotated.Height);
						surface.RotateDegrees(270);
						surface.DrawBitmap(bitmap, 0, 0);
					}

					return rotated;

				default:
					return bitmap;
			}
		}

		public static (int width, int height) GetNewResolution(int width, int height, int ratio)
		{
			float ratioW_h = (float)width / height;
			int newHeight = ratio;
			int newWidth = (int)(ratioW_h * ratio);

			if (newWidth * newHeight < width * height)
				return (newWidth, newHeight);
			else
				return (width, height);
		}

		public static (int width, int height) GetNewResolutionSVG(int width, int height, int ratio)
		{
			float ratioW_h = (float)width / height;
			int newHeight = ratio;
			int newWidth = (int)(ratioW_h * ratio);
			return (newWidth, newHeight);
		}

		/*public static string RemoveSizeSvg(string svg)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(svg);
			var h1Node = htmlDoc.DocumentNode.SelectSingleNode("//svg");
			h1Node.Attributes.Remove("width");
			h1Node.Attributes.Remove("height");
			return h1Node.ParentNode.InnerHtml;
		}

		public static string RemoveSizeSvg(string svg, ref int height, ref int width)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(svg);
			var h1Node = htmlDoc.DocumentNode.SelectSingleNode("//svg");
			height = int.Parse(h1Node.Attributes["height"].Value);
			width = int.Parse(h1Node.Attributes["width"].Value);
			h1Node.Attributes.Remove("width");
			h1Node.Attributes.Remove("height");
			return h1Node.ParentNode.InnerHtml;
		}

		public static string WhiteBG(string svg)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(svg);
			var h1Node = htmlDoc.DocumentNode.SelectSingleNode("//svg");
			h1Node.Attributes.Add("style", "background: white;");
			return h1Node.ParentNode.InnerHtml;
		}*/


	}
}
