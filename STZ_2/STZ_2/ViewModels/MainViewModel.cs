using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;

namespace STZ_2.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private Bitmap originalImage;
        private string selectedFileName;
        
        public BitmapImage Origin { get; set; }

        public BitmapImage Result { get; set; }

        public BitmapImage ResultOpenCv { get; set; }

        public string FileName { get; set; }

        public string Time { get; set; }

        public string TimeOpenCv { get; set; }

        public void BrowseButton()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Image",
                Filter = "Image File|*.bmp; *.gif; *.jpg; *.jpeg; *.png;"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                InitOriginImage(openFileDialog);
                ImageProcess();
                OpenCvImageProcess();
            }
        }

        private void InitOriginImage(OpenFileDialog openFileDialog)
        {
            selectedFileName = openFileDialog.FileName;
            originalImage = new Bitmap(selectedFileName);

            Origin = BitmapToImageSource(originalImage);
            FileName = selectedFileName;

            NotifyOfPropertyChange(() => Origin);
            NotifyOfPropertyChange(() => FileName);
        }

        private void ImageProcess()
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            var processedImage = Filtration(originalImage);
            startTime.Stop();
            
            processedImage.Save(CreateNewFileName(), ImageFormat.Png);

            Result = BitmapToImageSource(processedImage);
            
            var resultTime = startTime.Elapsed;

            Time =
                $"{resultTime.Hours:00}:" +
                $"{resultTime.Minutes:00}:" +
                $"{resultTime.Seconds:00}." +
                $"{resultTime.Milliseconds:000}";

            NotifyOfPropertyChange(() => Result);
            NotifyOfPropertyChange(() => Time);
        }

        private void OpenCvImageProcess()
        {
            var beforeImage = new Image<Bgr, byte>(originalImage);

            var startTime = System.Diagnostics.Stopwatch.StartNew();
            var processedImage = beforeImage.SmoothMedian(3);
            startTime.Stop();

            processedImage.Bitmap.Save(CreateNewFileName(true), ImageFormat.Png);

            ResultOpenCv = BitmapToImageSource(processedImage.Bitmap);

            var resultTime = startTime.Elapsed;

            TimeOpenCv =
                $"{resultTime.Hours:00}:" +
                $"{resultTime.Minutes:00}:" +
                $"{resultTime.Seconds:00}." +
                $"{resultTime.Milliseconds:000}";

            NotifyOfPropertyChange(() => ResultOpenCv);
            NotifyOfPropertyChange(() => TimeOpenCv);
        }

        private string CreateNewFileName(bool openCv = false)
        {
            var pathList = selectedFileName.Split(new[] {"\\"}, StringSplitOptions.None);
            string result = string.Empty;

            for (int i = 0; i < pathList.Length-1; i++)
            {
                result += pathList[i] + "\\";
            }

            result += openCv
                ? $"resultOpencv_{DateTime.Now:{{d_M_yyyy_HH_mm_ss}}}.png"
                : $"result_{DateTime.Now:{{d_M_yyyy_HH_mm_ss}}}.png";

            return result;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
        
        public Bitmap Filtration(Bitmap origin)
        {
            var resultImage = new Bitmap(origin);

            for (int i = 1; i < origin.Width - 1; i++)
            {
                for (int j = 1; j < origin.Height - 1; j++)
                {
                    resultImage.SetPixel(i, j, FiltrationPixel(origin, i, j));

                }
            }
            return resultImage;
        }

        private Color FiltrationPixel(Bitmap image, int i, int j)
        {
            var pixelBytes = new List<Color>();

            for (int k = i - 1; k < i + 2; k++)
            {
                for (int m = j - 1; m < j + 2; m++)
                {
                    pixelBytes.Add(image.GetPixel(k, m));
                }
            }
            
            var newR = MediumFiltrationColor(pixelBytes.Select(p => p.R).ToList());
            var newG = MediumFiltrationColor(pixelBytes.Select(p => p.G).ToList());
            var newB = MediumFiltrationColor(pixelBytes.Select(p => p.B).ToList());

            return Color.FromArgb(newR, newG, newB);
        }

        private byte MediumFiltrationColor(List<byte> colorBytes)
        {
            colorBytes.Sort();
            return colorBytes[colorBytes.Count / 2];
        }
    }
}
