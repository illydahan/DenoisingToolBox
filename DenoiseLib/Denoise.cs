using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace DenoiseLib
{
    public class ImageV2
    {
        public double[][] pixelData { get; set; }
        public int width { get; }
        public int height { get; }


        public ImageV2(double[][] pixelData, int width, int height)
        { 
            this.pixelData = pixelData;
            this.width = width;
            this.height = height;
        }

        public ImageV2 MakeNoisyImage(double std)
        {
            double[][] noisyImageData = new double[height][];
            Array.ForEach<double[]>(noisyImageData, item => item = new double[width]);

            ImageV2 noisyImage = this;
            Normal normal = new Normal(0, std);
            noisyImage
                .pixelData
                .AsParallel()
                .ForAll((double[] rowPix) =>
                {
                    for (int i = 0; i < rowPix.Length; ++i)
                    {
                        rowPix[i] += normal.Sample();
                    }
                });
            return noisyImage;
        }
    }                                     
}
