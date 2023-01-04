using DocsWASM.Shared;
using SkiaSharp;
using Svg.Skia;
using System.IO;

namespace DocsWASM.Server.Helpers
{
	public class SvgProcessing
	{
		public static byte[] SvgToWebP(byte[] ImgBin, int SizeRatio, int Quality, SKFilterQuality FilterQuality = SKFilterQuality.Medium)
		{
			using (var msSvg = new MemoryStream(ImgBin))
			{
				using (var svg = new SKSvg())
				{
					if (svg.Load(msSvg) is { })
					{
						var res = ImageProcessing.GetNewResolutionSVG((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height, SizeRatio);
						using SKImage Image = SKImage.FromPicture(svg.Picture, new() { Height = (int)svg.Picture.CullRect.Height, Width = (int)svg.Picture.CullRect.Width });
						if (Image != null)
						{
							using SKBitmap scaledBitmap = SKBitmap.FromImage(Image).Resize(new SKImageInfo(res.width, res.height), FilterQuality);
							using SKData data = scaledBitmap.Encode(SKEncodedImageFormat.Webp, Quality);
							return data.ToArray();
						}
					}
				}
			}
			return null;
		}

	}
}
