using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Streams
{
	public static class StreamTask
	{
		/// <summary>
		/// Parses Resources\Planets.xlsx file and returns the planet data: 
		///   Jupiter     69911.00
		///   Saturn      58232.00
		///   Uranus      25362.00
		///    ...
		/// See Resources\Planets.xlsx for details
		/// </summary>
		/// <param name="xlsxFileName">Source file name.</param>
		/// <returns>Sequence of PlanetInfo</returns>
		public static IEnumerable<PlanetInfo> ReadPlanetInfoFromXlsx(string xlsxFileName)
		{
			// TODO : Implement ReadPlanetInfoFromXlsx method using System.IO.Packaging + Linq-2-Xml

			// HINT : Please be as simple & clear as possible.
			//        No complex and common use cases, just this specified file.
			//        Required data are stored in Planets.xlsx archive in 2 files:
			//         /xl/sharedStrings.xml      - dictionary of all string values
			//         /xl/worksheets/sheet1.xml  - main worksheet

            XmlReader stringReader = null;
            XmlReader mainWorkSheetReader = null;
            StreamWriter sr = new StreamWriter(new FileStream("output.txt", FileMode.OpenOrCreate));
            using (ZipArchive planets = ZipFile.Open(xlsxFileName, ZipArchiveMode.Read))
            {
                foreach (var entry in planets.Entries)
                {
                    if (entry.Name == "sharedStrings.xml")
                    {
                        Stream stringStream = entry.Open();
                        stringReader = XmlReader.Create(stringStream);
                    }

                    if (entry.Name == "sheet1.xml")
                    {
                        Stream stringStream = entry.Open();
                        mainWorkSheetReader =  XmlReader.Create(stringStream);
                    }
                }
            }

            int i = 0;
            int count = 0;
            while (stringReader.Name != "sst")
            {
                stringReader.Read();
            }
            count = Int32.Parse(stringReader.GetAttribute("count")) - 2;
            while (i < count)
            {
                while (stringReader.Read())
                {
                    if (stringReader.LocalName == "t" && stringReader.NodeType != XmlNodeType.EndElement)
                    {
                        while (mainWorkSheetReader.Read())
                        {
                            if (mainWorkSheetReader.Name == "c" && mainWorkSheetReader.HasAttributes &&
                                Int32.Parse(mainWorkSheetReader.GetAttribute("s")) >= 8)
                            {
                                mainWorkSheetReader.Read();
                                yield return new PlanetInfo()
                                {
                                    Name = stringReader.ReadElementContentAsString(),
                                    MeanRadius = mainWorkSheetReader.ReadElementContentAsDouble()
                                };
                                break;
                            }
                        }
                        break;
                    }
                }

                i++;
            }

            stringReader.Close();
            mainWorkSheetReader.Close();
        }

		/// <summary>
		/// Calculates hash of stream using specified algorithm.
		/// </summary>
		/// <param name="stream">Source stream</param>
		/// <param name="hashAlgorithmName">
		///     Hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET).
		/// </param>
		/// <returns></returns>
		public static string CalculateHash(this Stream stream, string hashAlgorithmName)
        {
            HashAlgorithm hashAlgorithm;
            if ((hashAlgorithm = HashAlgorithm.Create(hashAlgorithmName)) is null)
            {
                throw new ArgumentException($"{hashAlgorithmName} is incorrect hash algorithm.");
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashAlgorithm.ComputeHash(stream))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

		/// <summary>
		/// Returns decompressed stream from file. 
		/// </summary>
		/// <param name="fileName">Source file.</param>
		/// <param name="method">Method used for compression (none, deflate, gzip).</param>
		/// <returns>output stream</returns>
		public static Stream DecompressStream(string fileName, DecompressionMethods method)
        {
            MemoryStream output = new MemoryStream();
            // TODO : Implement DecompressStream method
            switch (method)
            {
                case DecompressionMethods.None:
                    return new MemoryStream(File.ReadAllBytes(fileName));
                case DecompressionMethods.GZip:
                    using (GZipStream gZipStream = new GZipStream(new FileStream(fileName, FileMode.Open),
                        CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(output);
                    }

                    output.Position = 0;
                    return output;
                case DecompressionMethods.Deflate:
                    using (DeflateStream deflateStream = new DeflateStream(new FileStream(fileName, FileMode.Open), CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(output);
                    }

                    output.Position = 0;
                    return output;
                default:
                    throw new ArgumentException($"{method} is incorrect.");
            }
        }

		/// <summary>
		/// Reads file content encoded with non Unicode encoding
		/// </summary>
		/// <param name="fileName">Source file name</param>
		/// <param name="encoding">Encoding name</param>
		/// <returns>Unicoded file content</returns>
		public static string ReadEncodedText(string fileName, string encoding)
        {
            using StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open), Encoding.GetEncoding(encoding));
            return sr.ReadToEnd();
		}
	}
}