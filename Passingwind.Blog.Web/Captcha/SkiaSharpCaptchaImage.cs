using SkiaSharp;
using System;
using System.IO;

namespace Passingwind.Blog.Web.Captcha
{
	/// <summary> 
	/// </summary>
	public class SkiaSharpCaptchaImage
	{
		/// <summary> 
		/// </summary>
		/// <returns></returns>
		public string NewRandomCode(int length = 6)
		{
			string source = "1234567890qwertyuipasdfghjklzxcvbnm";

			Random rand = new Random(Guid.NewGuid().GetHashCode());

			string result = null;

			for (int i = 0; i < length; i++)
			{
				var r = rand.Next(source.Length);

				result = string.Concat(result, source[r]);
			}

			return result;
		}

		/// <summary> 
		/// </summary>
		/// <param name="randomCode"></param>
		/// <returns></returns>
		public byte[] CreateImage(string randomCode)
		{
			Random rand = new Random(Guid.NewGuid().GetHashCode());

			int randAngle = 40;
			int mapWidth = (int)(randomCode.Length * 18);

			using (SKBitmap bitmap = new SKBitmap(mapWidth, 28))
			{
				using (SKCanvas canvas = new SKCanvas(bitmap))
				{
					canvas.Clear(SKColors.AliceBlue);

					var paint = new SKPaint() { Color = SKColors.LightGray, };
					for (int i = 0; i < 50; i++)
					{
						int x = rand.Next(0, bitmap.Width);
						int y = rand.Next(0, bitmap.Height);

						canvas.DrawRect(new SKRect(x, y, x + 1, y + 1), paint);
					}

					char[] chars = randomCode.ToCharArray();

					SKColor[] colors = { SKColors.Black, SKColors.Red, SKColors.DarkBlue, SKColors.Green, SKColors.Orange, SKColors.Brown, SKColors.DarkCyan, SKColors.Purple };

					string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial" };

					canvas.Translate(-4, 0);

					for (int i = 0; i < chars.Length; i++)
					{
						int colorIndex = rand.Next(7);
						int fontIndex = rand.Next(5);

						var fontColor = colors[colorIndex];
						var foneSize = rand.Next(18, 25);
						float angle = rand.Next(-randAngle, randAngle);

						SKPoint point = new SKPoint(16, 28 / 2 + 4);

						canvas.Translate(point);
						canvas.RotateDegrees(angle);

						var textPaint = new SKPaint()
						{
							TextAlign = SKTextAlign.Center,
							Color = fontColor,
							TextSize = foneSize,
							IsVerticalText = true,
							IsAntialias = true,

							//IsAntialias = rand.Next(1) == 1 ? true : false,
							//FakeBoldText = true,
							//FilterQuality = SKFilterQuality.High,
							//HintingLevel = SKPaintHinting.Full,

							//IsEmbeddedBitmapText = true,                    
							//LcdRenderText = true,
							//Style = SKPaintStyle.StrokeAndFill,
							//TextEncoding = SKTextEncoding.Utf8,
						};

						canvas.DrawText(chars[i].ToString(), new SKPoint(0, 0), textPaint);
						canvas.RotateDegrees(-angle);
						canvas.Translate(0, -point.Y);
					}

					using (var image = SKImage.FromBitmap(bitmap))
					{
						using (var ms = new MemoryStream())
						{
							image.Encode(SKEncodedImageFormat.Png, 90).SaveTo(ms);

							return ms.ToArray();
						}
					}
				}
			}
		}
	}
}
