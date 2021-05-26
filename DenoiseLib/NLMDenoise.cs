using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Providers.LinearAlgebra;
using System.ComponentModel;
using System.Threading;

namespace DenoiseLib
{
    
    public class NLMDenoise
    {

        string[] denoisingSchemes = { "Non Local Means", "External Denoising" };

        const int patchDim = 5;
        const int patchesWidth = patchDim * patchDim - 1;
        private static Mutex mut = new Mutex();


        public byte[] denoisedImage { get; set; }
        public ImageV2 noisyImage { get; set; }
        private Matrix<double> noisyImageMat;
        private Matrix<double> noisyPatches;

        private Vector<double> rowImageYi;



        private int N;
        private int rows;
        private int cols;


        // statistical properties
        public double std{ get; set; }
        private double h;

        public NLMDenoise(double std, ImageV2 noisyImage)
        {
            this.std = std;
            h = 2 * std;
            this.noisyImage = noisyImage;
            setMatrix(noisyImage);
        }

        public void setMatrix(ImageV2 noisyImage)
        {
            noisyImageMat = CreateMatrix.DenseOfColumnArrays(noisyImage.pixelData);
            N             = (noisyImage.width - 4) * (noisyImage.height - 4);
            rows          = noisyImage.height;
            cols          = noisyImage.width;
            GetImagePatches();
        }

        public void DoNLMDenoise(object sender, EventArgs e)
        {
            double[] DenoisedImage = noisyImageMat.ToColumnMajorArray();

            var index = 0;
            
            for (int row = 2; row < rows - 2; ++row)
            {
                (sender as BackgroundWorker).ReportProgress(row);
                for (int col = 2; col < cols - 2; ++col)
                {
                    var Weights_ = GetWeights(noisyPatches.Row(index)).Normalize(1.0);

                    DenoisedImage[row * cols + col] = Weights_.DotProduct(rowImageYi);
                    ++index;
                }
            }
            denoisedImage = Array.ConvertAll(DenoisedImage, item => (byte)item);
            
            
        }
        
        private void GetImagePatches()
        {
            // Helper constans

            rowImageYi = CreateVector.Dense<double>(N);
            noisyPatches = CreateMatrix.Dense<double>(N, patchesWidth, 0);
            var rowVector = CreateVector.Dense<double>(patchesWidth, 0);
            int rowIndex = 0;

            for (int row = 0; row < rows - 4; ++row)
            {
                for (int col = 0; col < cols - 4; ++col)
                {
                    var window = noisyImageMat.SubMatrix(col, 5, row, 5);
                    double[] windowArr = window.ToColumnMajorArray();
                    var vecWindow = CreateVector.DenseOfArray<double>(windowArr);

                    var vec2 = vecWindow.SubVector(0, 12);

                    rowVector.SetSubVector(0, 12, vec2);
                    var vec1 = vecWindow.SubVector(13, 12);
                    rowVector.SetSubVector(12, 12, vec1);
                    noisyPatches.SetRow(rowIndex, rowVector);
                    rowImageYi[rowIndex] = vecWindow.SubVector(12, 1).ToArray()[0]; 

                    rowIndex++;
                }
            }
        }

        private Vector<double> GetWeights(Vector<double> YN)
        {
            Vector<double> Weights = CreateVector.Dense<double>(N, 0);

            var constDiv = 2 * Math.Pow(h, 2.0);
            for (int i = 0; i < N; ++i)
            {
                var norm = YN.Subtract(noisyPatches.Row(i)).L2Norm();
                Weights[i] = Math.Exp(-Math.Pow(norm, 2.0) / constDiv);
            }
            return Weights;
        }
    }
}
