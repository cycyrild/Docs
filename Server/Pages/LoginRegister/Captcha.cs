using SkiaSharp;
namespace DocsWASM.Pages.LoginRegister
{
    public static class Captcha
    {
        private struct Letter
        {
            public int Angle { get; set; }
            public string Value { get; set; }
            public SKColor ForeColor { get; set; }
            public string Family { get; set; }
        }
        public static string GetCaptchaWord(int length)
        {
            var random = new Random(DateTime.Now.Millisecond);

            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjklmnpqrstuvwxyz123456789";
            string cw = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return cw;
        }
        public static void GenerateImage(out string B64img, out string CaptchaWord, int CharNumber = 4, int Height = 40, int Width = 170)
        {
            CaptchaWord = GetCaptchaWord(CharNumber);
            var RandomValue = new Random();

            var _bgColor = new SKColor((byte)RandomValue.Next(90, 130), (byte)RandomValue.Next(90, 130), (byte)RandomValue.Next(90, 130));

            var fontFamilies = new string[] { "Courier", "Arial", "Verdana", "Times New Roman" };

            var Letters = new List<Letter>();

            if (!string.IsNullOrEmpty(CaptchaWord))
            {
                foreach (char c in CaptchaWord)
                {
                    var letter = new Letter
                    {
                        Value = c.ToString(),
                        Angle = RandomValue.Next(-15, 25),
                        ForeColor = new SKColor((byte)RandomValue.Next(256), (byte)RandomValue.Next(256), (byte)RandomValue.Next(256)),
                        Family = fontFamilies[RandomValue.Next(0, fontFamilies.Length)],
                    };

                    Letters.Add(letter);
                }
            }
            SKImageInfo imageInfo = new(Width, Height);
            using (var surface = SKSurface.Create(imageInfo))
            {
                var canvas = surface.Canvas;
                canvas.Clear(_bgColor);

                using (SKPaint paint = new())
                {
                    float x = 10;

                    foreach (Letter l in Letters)
                    {
                        paint.Color = l.ForeColor;
                        paint.Typeface = SKTypeface.FromFamilyName(l.Family);
                        paint.TextAlign = SKTextAlign.Left;
                        paint.TextSize = RandomValue.Next(Height / 2, (Height / 2) + (Height / 4));
                        paint.FakeBoldText = true;
                        paint.IsAntialias = true;

                        SKRect rect = new();
                        float width = paint.MeasureText(l.Value, ref rect);
                        float textWidth = width - 2;// + rect.Right;
                        var y = ((Height - rect.Height) / 2);


                        if (l.Angle < -5)
                        {
                            y = Height - rect.Height;
                        }
                        canvas.Save();
                        canvas.Translate(x, y);
                        canvas.RotateDegrees(l.Angle);
                        canvas.DrawText(l.Value, x, y, paint);
                        canvas.Restore();

                        x += textWidth;
                    }

                    canvas.DrawLine(0, RandomValue.Next(0, Height), Width, RandomValue.Next(0, Height), paint);
                    canvas.DrawLine(0, RandomValue.Next(0, Height), Width, RandomValue.Next(0, Height), paint);
                    paint.Style = SKPaintStyle.Stroke;
                    canvas.DrawOval(RandomValue.Next(-Width, Width), RandomValue.Next(-Height, Height), Width, Height, paint);
                }

                // save the file
                MemoryStream memoryStream = new();
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                    data.SaveTo(memoryStream);
                string imageBase64Data2 = Convert.ToBase64String(memoryStream.ToArray());
                B64img = string.Format("data:image/jpeg;base64,{0}", imageBase64Data2);
            }
        }
    }

}
