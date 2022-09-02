using Luval.FileSync.Core.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
        /// <summary>
        /// Creates the hash for the image
        /// </summary>
        /// <param name="fileName">The name of the file to process</param>
        /// <param name="preProcessImage">Indicates of edge detection is added to create the hash</param>
        /// <returns>An object with the hash values</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static ImageHashResult FromFile(string fileName, bool preProcessImage = false)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("fileName");
            if (!File.Exists(fileName)) throw new FileNotFoundException("Invalid file path provided", fileName);
            return FromImage(Image.Load<Rgba32>(fileName), preProcessImage);
        }


        /// <summary>
        /// Creates the hash for the image
        /// </summary>
        /// <param name="image">The image object</param>
        /// <param name="preProcessImage">Indicates of edge detection is added to create the hash</param>
        /// <returns>An object with the hash values</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ImageHashResult FromImage(Image<Rgba32> image, bool preProcessImage = false)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (preProcessImage)
            {
                image = EdgeDetector(image);
            }
            var per = new PerceptualHashProvider().FromImage(image);
            //var diff = new DifferenceHashProvider().FromImage(image);
            //var average = new AverageHashProvider().FromImage(image);
            return new ImageHashResult()
            {
                PerceptualHash = per
            };
        }

        public static ulong FromStream(Stream stream, bool preProcessImage = false)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            Image<Rgba32> image;
            stream.Position = 0;
            if (preProcessImage)
                image = EdgeDetector(stream);
            else
                image = Image.Load<Rgba32>(stream);
            var per = new PerceptualHashProvider().FromImage(image);
            return per;
        }

        public static Image<Rgba32> EdgeDetector(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var i = Image.Load<Rgba32>(fileName);
            var img = EdgeDetector(i);
            var newFileName = Path.Combine(fileInfo.DirectoryName, String.Format("NEW-{0}.png", fileInfo.Name));
            i.SaveAsPng(newFileName);
            return img;
        }

        public static Image<Rgba32> EdgeDetector(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var i = Image.Load<Rgba32>(memoryStream.ToArray());
            return EdgeDetector(i);
        }

        public static Image<Rgba32> EdgeDetector(Image<Rgba32> image)
        {
            var detector = new EdgeDetector2DProcessor(KnownEdgeDetectorKernels.Sobel, true);
            var procesor = detector.CreatePixelSpecificProcessor(Configuration.Default, image, image.Bounds());
            procesor.Execute();
            return image;
        }

        public static string MD5FromStream(Stream stream)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                stream.Position = 0;
                byte[] hashBytes = md5.ComputeHash(stream);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string MD5FromFile(string fileName)
        {
            using (var fs = new FileInfo(fileName).OpenRead())
            {
                return MD5FromStream(fs);
            }
        }
    }
}
