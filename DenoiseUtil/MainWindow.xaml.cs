using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DenoiseLib;
using System.IO;
using System.ComponentModel;

namespace DenoiseUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double std;
        private BitmapImage bitmapOriginalImage;
        private ImageV2 GeneratedNoisyImage;
        private NLMDenoise denoise;
        bool isLoaded = false;


        string[] denoisingScheme = { "Non Local Means", "External Denoising"};

        public MainWindow()
        {
            InitializeComponent();
            isLoaded = true;
            // default value
            std = 10;
        }


        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|*.tif|*.png|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if ((bool)dlg.ShowDialog())
            {
                string selectedFileName = dlg.FileName;
                bitmapOriginalImage = new BitmapImage();

                bitmapOriginalImage.BeginInit();
                bitmapOriginalImage.UriSource = new Uri(selectedFileName);
                bitmapOriginalImage.EndInit();
                originalIamge.Source = bitmapOriginalImage;
                GenerateNoisyImage();
            }
        }

        private void GenerateNoisyImage()
        {
            int width = bitmapOriginalImage.PixelWidth;
            int height = bitmapOriginalImage.PixelHeight;
            byte[] pixelData = BitmapSourceToArray(bitmapOriginalImage);
            
            ImageV2 originalImage = new ImageV2(
                                        byteArrayToMatrix(pixelData, width, height),
                                        width,
                                        height);

            this.GeneratedNoisyImage = originalImage.MakeNoisyImage(std);
            denoise = new NLMDenoise(std, GeneratedNoisyImage);


            byte[] noisyImageArray = MatrixToByteArray(GeneratedNoisyImage.pixelData);

            noisyImage.Source = getBitmap(noisyImageArray, width, height);
        }

        private byte[] BitmapSourceToArray(BitmapSource bitmapSource)
        {
            // Stride = (width) x (bytes per pixel)
            int stride = (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)bitmapSource.PixelHeight * stride];

            bitmapSource.CopyPixels(pixels, stride, 0);

            return pixels;
        }

        private double[][] byteArrayToMatrix(byte[] inImageRow, int width, int height)
        {
            double[][] imageMat = new double[width][];
            for (int i = 0; i< height; ++i)
            {
                imageMat[i] = new double[width];
                for (int j = 0; j < width; ++j)
                {
                    imageMat[i][j] = (byte)inImageRow[i * width + j];
                }
            }
            return imageMat;
        }

        private byte[] MatrixToByteArray(double[][] inImage)
        {
            var size = bitmapOriginalImage.PixelHeight * bitmapOriginalImage.PixelWidth;
            var result = new double[(int)size];

            var cursor = 0;
            foreach (var a in inImage)
            {
                a.CopyTo(result, cursor);
                cursor += a.Length;
            }
            var newResult = Array.ConvertAll(result, item => (byte)item);
            return newResult;
        }

        private BitmapSource getBitmap(byte[] imageArray, int width, int height)
        {
            int stride = width * (bitmapOriginalImage.Format.BitsPerPixel / 8);
            BitmapSource bitmapSource =
                                    BitmapSource.Create(width,
                                    height,
                                    bitmapOriginalImage.DpiX,
                                    bitmapOriginalImage.DpiY,
                                    PixelFormats.Indexed8,
                                    BitmapPalettes.Gray256,
                                    imageArray,
                                    stride);
            
            return bitmapSource;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            std = Math.Sqrt(VarianceSlider.Value);
            if (VarianceSliderLabel != null)
            {
                VarianceSliderLabel.Content = "Noise Variance: " + ((int)VarianceSlider.Value).ToString();
            }            
            if (DenoiseBTN != null && bitmapOriginalImage != null)
            {
                DenoiseBTN.IsEnabled = true;
                GenerateNoisyImage();
            }   
        }

        private void DenoiseBTN_Click(object sender, RoutedEventArgs e)
        {

            int width = bitmapOriginalImage.PixelWidth;
            int height = bitmapOriginalImage.PixelHeight;
            int N = (width - 4) * (height - 4);
            byte[] denoisedImage = new byte[N];

            
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += denoise.DoNLMDenoise;
            
            worker.RunWorkerCompleted += 
                (sender1, e1) => showDenoisedImage(sender1, e1, denoise.denoisedImage);

            worker.ProgressChanged += workerProgressChanged;

            worker.RunWorkerAsync();
            popup1.IsOpen = true;
            DenoiseBTN.IsEnabled = false;
        }

        private void showDenoisedImage(object sender, EventArgs e, byte[] denoisedImage)
        {
            int width = bitmapOriginalImage.PixelWidth;
            int height = bitmapOriginalImage.PixelHeight;
            BitmapSource denoisedImageBM = getBitmap(denoisedImage, width, height);
            calculatedDenoisedImage.Source = denoisedImageBM;
            DenoiseBTN.IsEnabled = true;
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if (progressBar1.Value == progressBar1.Maximum)
                popup1.IsOpen = false;
        }

        private void PopupBTN_Click(object sender, RoutedEventArgs e)
        {
            popup1.IsOpen = false;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // not equal to Non local means scheme.
            if (DenoisingSchemeComboBox.Text.Equals(denoisingScheme[0]) && isLoaded)
            {
                DenoisingSchemeComboBox.Text = denoisingScheme[0];
                MessageBox.Show("This denoising scheme is not availible right now.");
            }
        }
    }
}
