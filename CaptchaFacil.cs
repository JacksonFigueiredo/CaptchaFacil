using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CaptchaFacil
{
    public class CaptchaResult
    {
        public byte[] ImageBytes { get; set; }
        public string CaptchaText { get; set; }
    }

    public class CaptchaImage
    {
        public CaptchaResult GenerateImage()
        {
            string captchaText = GenerateRandomText(6);

            using (Bitmap bitmap = new Bitmap(200, 50))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    g.DrawString(captchaText, new Font("Arial", 25, FontStyle.Bold), Brushes.Gray, new PointF(10, 10));

                    AddNoise(bitmap, bitmap.Width, bitmap.Height);
                }

                using (Bitmap deformedBitmap = DeformImage(bitmap))
                {
                    using (var ms = new MemoryStream())
                    {
                        deformedBitmap.Save(ms, ImageFormat.Png);
                        byte[] imageBytes = ms.ToArray();

                        return new CaptchaResult { ImageBytes = imageBytes, CaptchaText = captchaText };
                    }
                }
            }
        }

        public bool ValidateCaptcha(string userInput, string captchaText)
        {
            return string.Equals(userInput, captchaText, StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateRandomText(int length)
        {
            var random = new Random();
            var text = new string[6];
            for (int i = 0; i < length; i++)
            {
                text[i] = ((char)(random.Next(26) + 65)).ToString();
            }
            return string.Concat(text);
        }

        private void AddNoise(Bitmap bitmap, int width, int height)
        {
            var random = new Random();

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < 10; i++)
                {
                    int x1 = random.Next(width);
                    int y1 = random.Next(height);
                    int x2 = random.Next(width);
                    int y2 = random.Next(height);
                    g.DrawLine(new Pen(Color.Gray, 2), x1, y1, x2, y2);
                }

                for (int i = 0; i < 1000; i++)
                {
                    int x = random.Next(width);
                    int y = random.Next(height);
                    bitmap.SetPixel(x, y, Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));
                }
            }
        }

        private Bitmap DeformImage(Bitmap originalImage)
        {
            Bitmap deformedImage = new Bitmap(originalImage.Width, originalImage.Height);

            using (Graphics g = Graphics.FromImage(deformedImage))
            {
                var matrix = new System.Drawing.Drawing2D.Matrix();
                var random = new Random();
                float skewAmount = random.Next(-10, 10) / 10.0f;
                matrix.Shear(skewAmount, 0);
                g.Transform = matrix;

                g.DrawImage(originalImage, new Point(0, 0));
            }

            return deformedImage;
        }
    }
}
