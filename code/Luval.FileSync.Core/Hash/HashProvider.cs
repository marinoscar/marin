using Luval.FileSync.Core.Entities;
using OpenCvSharp;
using SixLabors.ImageSharp.Processing.Processors.Convolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Hash
{
    public static class HashProvider
    {
        public static ImageHashResult FromFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("fileName");
            if (!File.Exists(fileName)) throw new FileNotFoundException("Invalid file path provided", fileName);

            using (var fs = File.OpenRead(fileName))
            {
                return FromStream(fs);
            }
        }

        public static ImageHashResult FromStream(Stream stream)
        {
            stream.Position = 0;
            var per = new PerceptualHashProvider().FromStream(stream);
            stream.Position = 0;
            var diff = new DifferenceHashProvider().FromStream(stream);
            stream.Position = 0;
            var average = new AverageHashProvider().FromStream(stream);
            return new ImageHashResult()
            {
                AverageHash = average,
                DifferenceHash = diff,
                PerceptualHash = per
            };
        }

        public static byte[] EdgeDetector(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var imageMat = new Mat(fileName);
            var newMat = new Mat();
            Cv2.Canny(imageMat, newMat, 50, 200);
            var lenght = newMat.Rows * newMat.Cols;
            byte[] imgData = new byte[lenght];
            newMat.GetArray(out imgData);
            newMat.SaveImage(Path.Combine(fileInfo.DirectoryName, string.Format("NEW-{0}", fileInfo.Name)));
            return imgData;
        }
    }
}
