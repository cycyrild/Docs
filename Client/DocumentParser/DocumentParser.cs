using static DocsWASM.Shared.UploadModels;
using DocsWASM.Shared;
using SkiaSharp;
using static DocsWASM.Client.DocumentParser.DocumentParser;
using System.Net.Mime;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using DocsWASM.Shared.Helpers;
using Microsoft.JSInterop;
using PdfToSvg;

namespace DocsWASM.Client.DocumentParser
{
	public class DocumentParser
	{
		public DocumentParser()
		{
		}
		public List<PageModel> Parser(byte[] bin, string filename)
		{
			switch((dataBinTypesEnum)dataBinTypes[Path.GetExtension(filename)])
			{
				case dataBinTypesEnum.pdf:
					return ParsePdfAsync(bin, filename);

				case dataBinTypesEnum.png:
					return new () { new(0, ImageProcessing.ImgToWebP(bin, 1200, 90) , filename, dataBinTypesEnum.webp, false, "") };

				case dataBinTypesEnum.jpg:
					return new() { new(0, ImageProcessing.ImgToWebP(bin, 1200, 90), filename, dataBinTypesEnum.webp, false, "") };
			}
			return null;
		}

		private List<PageModel> ParsePdfAsync(byte[] pdf, string filename)
		{
			/*
			var items = await JS.InvokeAsync<List<string[]>>("convert", pdf);
			var elements = new List<PageModel>();
			var pageNo = 0;

			foreach (var item in items)
			{
				elements.Add(new(pageNo++, Encoding.UTF8.GetBytes(XMLMinifier.XMLMinifier.Clean(item[0])), filename, dataBinTypesEnum.svg, false, item[1]));
			}
			return elements;
			*/

			using (var pdfMs = new MemoryStream(pdf))
			{
				var elements = new List<PageModel>();

				using (var doc = PdfToSvg.PdfDocument.Open(pdfMs))
				{
					var pageNo = 0;

					foreach (var page in doc.Pages)
					{
						var svg = XMLHelper.XMLHelper.Clean(page.ToSvgString(new SvgConversionOptions() { ImageResolver = ImageResolver2.DataUrl }));
						var txt = DocsWASM.Shared.Helpers.Text.CleanString(XMLHelper.XMLHelper.ExtractTextStringsFromSvg(svg));
						elements.Add(new(pageNo + 1, Encoding.UTF8.GetBytes(svg), filename, dataBinTypesEnum.svg, false, txt));
					}
				}
				return elements;
			}
		}
	}

    public abstract class ImageResolver2
    {
        public static ImageResolver DataUrl { get; } = new DataUrlImageResolver2();
        public static ImageResolver Default { get; } = DataUrl;
        public abstract string ResolveImageUrl(PdfToSvg.Image image, CancellationToken cancellationToken);
    }

    internal class DataUrlImageResolver2 : ImageResolver
    {
        public override string ResolveImageUrl(PdfToSvg.Image image, CancellationToken cancellationToken)
        {
			byte[] img = image.GetContent(cancellationToken);
			byte[] compressedImg = ImageProcessing.ImgToWebP(img, 3508, 50, SKFilterQuality.None);
            return $"data:image/webp;base64, {Convert.ToBase64String(compressedImg)}";
        }
    }
}
