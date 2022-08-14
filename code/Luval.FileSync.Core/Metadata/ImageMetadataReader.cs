using Luval.FileSync.Core.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Metadata
{
    public static class ImageMetadataReader
    {

        public static ImageMetadata FromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath)) throw new FileNotFoundException("Invalid file path provided", filePath);

            return FromImage(Image.FromFile(filePath));

        }

        public static ImageMetadata FromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            return FromImage(Image.FromStream(stream));
        }

        public static ImageMetadata FromImage(Image image)
        {
            if (image == null) throw new ArgumentNullException("image");
            using (image)
            {
                return new ImageMetadata()
                {
                    GeoLocation = GetLocationFromImage(image),
                    UtcDateTaken = GetDateTakenFromImage(image)
                };
            }
        }

        public static ImageGeoLocation GetLocationFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath)) throw new FileNotFoundException("Invalid file path provided", filePath);

            return GetLocationFromImage(Image.FromFile(filePath));
        }

        public static ImageGeoLocation GetLocationFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return GetLocationFromImage(Image.FromStream(stream));
        }

        public static ImageGeoLocation GetLocationFromImage(Image image)
        {
            if (image == null) throw new ArgumentNullException("image");

            var result = new ImageGeoLocation();
            result.Longitude = GetCoordinateDouble(image.PropertyItems.SingleOrDefault(p => p.Id == 4));
            result.Latitude = GetCoordinateDouble(image.PropertyItems.SingleOrDefault(p => p.Id == 2));

            return result;
        }

        public static DateTime? GetDateTakenFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath)) throw new FileNotFoundException("Invalid file path provided", filePath);

            return GetDateTakenFromImage(Image.FromFile(filePath));
        }

        public static DateTime? GetDateTakenFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            return GetDateTakenFromImage(Image.FromStream(stream));
        }

        public static DateTime? GetDateTakenFromImage(Image image)
        {
            if (image == null) throw new ArgumentNullException("image");

            DateTime? result = null;
            var r = new Regex(":");

            var propItem = image.PropertyItems.SingleOrDefault(p => p.Id == 36867);
            if (propItem == null) return result;

            var dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);

            DateTime dt;
            if (DateTime.TryParse(dateTaken, out dt)) result = dt;

            return result;
        }

        private static double? GetCoordinateDouble(PropertyItem propItem)
        {
            if (propItem == null) return null;

            uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            double degrees = degreesNumerator / (double)degreesDenominator;


            uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            double minutes = minutesNumerator / (double)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            double seconds = secondsNumerator / (double)secondsDenominator;

            double coorditate = degrees + (minutes / 60d) + (seconds / 3600d);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItem.Value[0] }); //N, S, E, or W  

            if (gpsRef == "S" || gpsRef == "W")
            {
                coorditate = coorditate * -1;
            }
            return double.NaN.Equals(coorditate) ? null : coorditate;
        }
    }
}
