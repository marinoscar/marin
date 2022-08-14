// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;

// ReSharper disable RedundantCaseLabel

namespace MetadataExtractor
{
    /// <summary>Reads metadata from any supported file format.</summary>
    /// <remarks>
    /// This class a lightweight wrapper around other, specific metadata processors.
    /// During extraction, the file type is determined from the first few bytes of the file.
    /// Parsing is then delegated to one of:
    ///
    /// <list type="bullet">
    ///   <item><see cref="JpegMetadataReader"/> for JPEG files</item>
    ///   <item><see cref="TiffMetadataReader"/> for TIFF and (most) RAW files</item>
    ///   <item><see cref="PsdMetadataReader"/> for Photoshop files</item>
    ///   <item><see cref="PngMetadataReader"/> for PNG files</item>
    ///   <item><see cref="BmpMetadataReader"/> for BMP files</item>
    ///   <item><see cref="GifMetadataReader"/> for GIF files</item>
    ///   <item><see cref="IcoMetadataReader"/> for ICO files</item>
    ///   <item><see cref="NetpbmMetadataReader"/> for Netpbm files (PPM, PGM, PBM, PPM)</item>
    ///   <item><see cref="PcxMetadataReader"/> for PCX files</item>
    ///   <item><see cref="WebPMetadataReader"/> for WebP files</item>
    ///   <item><see cref="RafMetadataReader"/> for RAF files</item>
    ///   <item><see cref="QuickTimeMetadataReader"/> for QuickTime files</item>
    /// </list>
    ///
    /// If you know the file type you're working with, you may use one of the above processors directly.
    /// For most scenarios it is simpler, more convenient and more robust to use this class.
    /// <para />
    /// <see cref="FileTypeDetector"/> is used to determine the provided image's file type, and therefore
    /// the appropriate metadata reader to use.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class ImageMetadataReader
    {
        /// <summary>Reads metadata from an <see cref="Stream"/>.</summary>
        /// <param name="stream">A stream from which the file data may be read.  The stream must be positioned at the beginning of the file's data.</param>
        /// <returns>A list of <see cref="Directory"/> instances containing the various types of metadata found within the file's data.</returns>
        /// <exception cref="ImageProcessingException">The file type is unknown, or processing errors occurred.</exception>
        /// <exception cref="IOException"/>
        public static DirectoryList ReadMetadata(Stream stream)
        {

            var directories = new List<Directory>();
            return directories;
        }

        /// <summary>Reads metadata from a file.</summary>
        /// <remarks>Unlike <see cref="ReadMetadata(Stream)"/>, this overload includes a <see cref="FileMetadataDirectory"/> in the output.</remarks>
        /// <param name="filePath">Location of a file from which data should be read.</param>
        /// <returns>A list of <see cref="Directory"/> instances containing the various types of metadata found within the file's data.</returns>
        /// <exception cref="ImageProcessingException">The file type is unknown, or processing errors occurred.</exception>
        /// <exception cref="IOException"/>
        public static DirectoryList ReadMetadata(string filePath)
        {
            var directories = new List<Directory>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                directories.AddRange(ReadMetadata(stream));

            //directories.Add(new FileMetadataReader().Read(filePath));

            return directories;
        }
    }
}
