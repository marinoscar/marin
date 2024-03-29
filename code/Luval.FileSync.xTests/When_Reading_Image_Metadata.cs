﻿using Luval.FileSync.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Luval.FileSync.xTests
{
    public class When_Reading_Image_Metadata
    {
        [Fact]
        public void It_Should_Read_The_Date_Taken_Properly()
        {
            //var validFile = Environment.CurrentDirectory + @"\resources\images\metadata.jpg";
            //var inValidFile = Environment.CurrentDirectory + @"\resources\images\jpeg-no-metadata.jpg";

            //var valid = ImageMetadataReader.GetDateTakenFromFile(validFile);
            //var inValid = ImageMetadataReader.GetDateTakenFromFile(inValidFile);

            //Assert.NotNull(valid);
            //Assert.Null(inValid);

        }

        [Fact]
        public void It_Should_Read_The_Location_Properly()
        {
            //var validFile = Environment.CurrentDirectory + @"\resources\images\metadata.jpg";
            //var inValidFile = Environment.CurrentDirectory + @"\resources\images\jpeg-no-metadata.jpg";
            //var noGpsFile = Environment.CurrentDirectory + @"\resources\images\no-gps.jpg";

            //var valid = ImageMetadataReader.GetLocationFromFile(validFile);
            //var inValid = ImageMetadataReader.GetLocationFromFile(inValidFile);
            //var noGps = ImageMetadataReader.GetLocationFromFile(noGpsFile);

            //Assert.NotNull(valid);
            //Assert.NotNull(valid.Longitude);
            //Assert.NotNull(valid.Latitude);
            //Assert.NotNull(inValid);
            //Assert.Null(inValid.Longitude);
            //Assert.Null(inValid.Latitude);
            //Assert.NotNull(noGps);
            //Assert.Null(noGps.Longitude);
            //Assert.Null(noGps.Latitude);

        }

        [Fact]
        public void It_Should_Read_The_Metadata_Properly()
        {
            var validFile = Environment.CurrentDirectory + @"\resources\images\metadata.jpg";
            var inValidFile = Environment.CurrentDirectory + @"\resources\images\jpeg-no-metadata.jpg";
            var noGpsFile = Environment.CurrentDirectory + @"\resources\images\no-gps.jpg";
            var videoFile = Environment.CurrentDirectory + @"\resources\images\video.mp4";

            var video = MediaMetadataReader.FromFile2(videoFile);
            var valid = MediaMetadataReader.FromFile2(validFile);
            var inValid = MediaMetadataReader.FromFile2(inValidFile);
            var noGps = MediaMetadataReader.FromFile2(noGpsFile);

            Assert.NotNull(valid);
            Assert.NotNull(inValid);
            Assert.NotNull(noGps);


        }
    }
}
