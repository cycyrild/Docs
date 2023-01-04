using static DocsWASM.Shared.UploadModels;
using DocsWASM.Shared;
using SkiaSharp;
using static DocsWASM.Client.DocumentParser.DocumentParser;
using System.Net.Mime;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using DocsWASM.Shared.Helpers;
using Microsoft.JSInterop;

namespace DocsWASM.Client.DocumentParser
{
	public class DocumentParser
	{
		public IJSRuntime JS { get; set; }
		public DocumentParser(IJSRuntime jsruntime)
		{
			JS = jsruntime;
		}
		public async Task<List<PageModel>> Parser(byte[] bin, string filename)
		{
			switch((dataBinTypesEnum)dataBinTypes[Path.GetExtension(filename)])
			{
				case dataBinTypesEnum.pdf:
					return await ParsePdfAsync(bin, filename);

				case dataBinTypesEnum.png:
					return new () { new(0, ImageProcessing.ImgToWebP(bin, 1200, 90) , filename, dataBinTypesEnum.webp, false, "") };

				case dataBinTypesEnum.jpg:
					return new() { new(0, ImageProcessing.ImgToWebP(bin, 1200, 90), filename, dataBinTypesEnum.webp, false, "") };
			}
			return null;


		}
		private async Task<List<PageModel>> ParsePdfAsync(byte[] pdf, string filename)
		{
			var items = await JS.InvokeAsync<List<string[]>>("convert", pdf);
			var elements = new List<PageModel>();
			var pageNo = 0;

			foreach (var item in items)
			{
				elements.Add(new(pageNo++, Encoding.UTF8.GetBytes(XMLMinifier.XMLMinifier.Clean(item[0])), filename, dataBinTypesEnum.svg, false, item[1]));
			}
			return elements;

			/*using (var pdfMs = new MemoryStream(pdf))
			{
				var elements = new List<PageModel>();

				using (var doc = PdfToSvg.PdfDocument.Open(pdfMs))
				{
					var pageNo = 0;

					foreach (var page in doc.Pages)
					{
						var svg = page.ToSvgString(new SvgConversionOptions() { ImageResolver = ImageResolverWebP.DataUrlWebP});
						elements.Add(new PageModel(pageNo + 1, Encoding.UTF8.GetBytes(svg), filename, dataBinTypesEnum.svg, false));
					}
				}
				return elements;
			}*/
		}
	}
}
