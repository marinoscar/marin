using Luval.FileSync.Core.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            using (var fs = File.OpenRead(filePath))
            {
                return FromStream(fs);
            }

        }

       public static ImageMetadata FromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            stream.Position = 0;
            var info = Image.Identify(stream);
            if(info == null) return null;
            return new ImageMetadata() { 
                GeoLocation = LocationFromMetadata(info.Metadata.ExifProfile.Values),
                UtcDateTaken = GetDate(info.Metadata.ExifProfile.Values)
            };
        }

        private static DateTime? GetDate(IReadOnlyList<IExifValue> values)
        {
            var offset = TimeSpan.FromSeconds(0);
            var dt = values.FirstOrDefault(i => Convert.ToString(i.Tag) == "DateTimeOriginal");
            var os = values.FirstOrDefault(i => Convert.ToString(i.Tag) == "OffsetTimeOriginal");
            if (dt == null) return null;
            var strDate = Convert.ToString(dt.GetValue());
            DateTime value = new DateTime();
            if (!DateTime.TryParseExact(strDate, "yyyy:MM:dd HH:mm:ss",
                CultureInfo.CurrentCulture, DateTimeStyles.None, out value))
            {
                return null;
            }
            if (os != null && TimeSpan.TryParse(Convert.ToString(os.GetValue()), out offset))
            {
                value = value.AddHours(offset.TotalHours);
            }
            return value;
        }

        private static ImageGeoLocation LocationFromMetadata(IReadOnlyList<IExifValue> values)
        {
            var lonRef = values.FirstOrDefault(i => Convert.ToString(i.Tag) == "GPSLongitudeRef");
            var latRef = values.FirstOrDefault(i => Convert.ToString(i.Tag) == "GPSLatitudeRef");
            var lat = values.FirstOrDefault(i => Convert.ToString(i.Tag) == "GPSLatitude");
            var lon = values.FirstOrDefault(i => Convert.ToString(i.Tag) == "GPSLongitude");
            var alt = values.FirstOrDefault(i => Convert.ToString(i.Tag) == "GPSAltitude");
            return new ImageGeoLocation()
            {
                Latitude = GetDouble(lat, latRef),
                Longitude = GetDouble(lon, lonRef),
                Altitude = GetAltitude(alt)
            };
        }

        private static double? GetAltitude(IExifValue? gpsRead)
        {
            var res = 0d;
            if (gpsRead == null) return null;
            var strDouble = Convert.ToString(gpsRead.GetValue());
            if (double.TryParse(strDouble, out res)) return res;
            else return null;
        }

        private static double? GetDouble(IExifValue? gpsRead, IExifValue? gpsRef)
        {
            var gpsRefValue = string.Empty;
            if (gpsRead == null) return null;

            if (gpsRef != null)
            {
                gpsRefValue = Convert.ToString(gpsRef.GetValue());
            }

            var rational = (Rational[])gpsRead.GetValue();
            if (rational.Length < 3) return null;

            uint degreesNumerator = rational[0].Numerator;
            uint degreesDenominator = rational[0].Denominator;
            double degrees = degreesNumerator / (double)degreesDenominator;


            uint minutesNumerator = rational[1].Numerator;
            uint minutesDenominator = rational[1].Denominator;
            double minutes = minutesNumerator / (double)minutesDenominator;

            uint secondsNumerator = rational[2].Numerator;
            uint secondsDenominator = rational[2].Denominator;
            double seconds = secondsNumerator / (double)secondsDenominator;

            double coorditate = degrees + (minutes / 60d) + (seconds / 3600d);

            if (gpsRefValue == "S" || gpsRefValue == "W")
            {
                coorditate = coorditate * -1;
            }
            return double.NaN.Equals(coorditate) ? null : coorditate;
        }

    }
}
