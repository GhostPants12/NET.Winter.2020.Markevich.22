using System;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace StreamDemo
{
    // C# 6.0 in a Nutshell. Joseph Albahari, Ben Albahari. O'Reilly Media. 2015
    // Chapter 15: Streams and I/O
    // Chapter 6: Framework Fundamentals - Text Encodings and Unicode
    // https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding?view=netcore-3.0
    // https://docs.microsoft.com/en-us/dotnet/api/system.io?view=netcore-3.0

    public static class StreamsExtension
    {
        #region Public members

        #region TODO: Implement by byte copy logic using class FileStream as a backing store stream .

        public static int ByByteCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            FileStream fsRead = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            FileStream fsWrite = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write);
            int readByteResult;
            int counter = 0;
            while ((readByteResult = fsRead.ReadByte()) != -1)
            {
                fsWrite.WriteByte((byte)readByteResult);
                counter++;
            }

            fsRead.Close();
            fsWrite.Close();
            return counter;
        }

        #endregion

        #region TODO: Implement by byte copy logic using class MemoryStream as a backing store stream.

        public static int InMemoryByByteCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            // TODO: step 1. Use StreamReader to read entire file in string
            StreamReader sr = new StreamReader(sourcePath);
            string read = sr.ReadToEnd();
            sr.Close();
            // TODO: step 2. Create byte array on base string content - use  System.Text.Encoding class
            byte[] byteArr = Encoding.UTF8.GetBytes(read);
            // TODO: step 3. Use MemoryStream instance to read from byte array (from step 2)
            MemoryStream ms = new MemoryStream();
            int i = 0;
            int counter = 0;
            while (i < byteArr.Length)
            {
                ms.WriteByte(byteArr[i++]);
                counter++;
            }
            // TODO: step 4. Use MemoryStream instance (from step 3) to write it content in new byte array
            byte[] byteArrResult = new byte[ms.Length];
            int readByte;
            counter = 0;
            ms.Seek(0, SeekOrigin.Begin);
            while (counter < ms.Length)
            {
                byteArrResult[counter++] = Convert.ToByte(ms.ReadByte());
            }
            ms.Close();
            // TODO: step 5. Use Encoding class instance (from step 2) to create char array on byte array content
            char[] write = Encoding.UTF8.GetChars(byteArrResult);
            // TODO: step 6. Use StreamWriter here to write char array content in new file
            StreamWriter sw = new StreamWriter(destinationPath);
            sw.Write(write);
            sw.Close();
            return counter;
        }

        #endregion

        #region TODO: Implement by block copy logic using FileStream buffer.
        
        public static int ByBlockCopy(string sourcePath, string destinationPath)
        {
            const int bufferSize = 1024;
            int readBytes, total = 0;
            InputValidation(sourcePath, destinationPath);
            byte[] buf = new byte[bufferSize];
            using (FileStream fsr = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsw = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    do
                    {
                        buf = new byte[fsr.Length];
                        readBytes = fsr.Read(buf, 0, bufferSize);
                        fsw.Write(buf, 0, readBytes);
                        total += readBytes;
                    } while (readBytes == bufferSize);
                }
            }

            return total;
        }

        #endregion

        #region TODO: Implement by block copy logic using MemoryStream.
        
        public static int InMemoryByBlockCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            // TODO: Use InMemoryByByteCopy method's approach
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int readBytes, total=0;
            // TODO: step 1. Use StreamReader to read entire file in string
            StreamReader sr = new StreamReader(sourcePath);
            string read = sr.ReadToEnd();
            sr.Close();
            // TODO: step 2. Create byte array on base string content - use  System.Text.Encoding class
            byte[] byteArr = Encoding.UTF8.GetBytes(read);
            MemoryStream ms = new MemoryStream(byteArr);
            // TODO: step 4. Use MemoryStream instance (from step 3) to write it content in new byte array
            byte[] byteArrResult = new byte[ms.Length];
            int current = 0;
            do
            {
                readBytes = ms.Read(buffer, 0, bufferSize);
                for (int j = 0; j < readBytes; j++)
                {
                    byteArrResult[current] = buffer[j];
                    current++;
                }

                total += readBytes;
            } while (readBytes == bufferSize);
            ms.Close();
            // TODO: step 5. Use Encoding class instance (from step 2) to create char array on byte array content
            string write = Encoding.UTF8.GetString(byteArrResult);
            // TODO: step 6. Use StreamWriter here to write char array content in new file
            StreamWriter sw = new StreamWriter(destinationPath);
            sw.Write(write);
            sw.Close();
            return total;
        }

        #endregion

        #region TODO: Implement by block copy logic using class-decorator BufferedStream.

        public static int BufferedCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            const int bufferSize = 1024;
            int length;
            using (FileStream fsr = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsw = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BufferedStream bs = new BufferedStream(fsr, bufferSize);
                    bs.CopyTo(fsw);
                    length = (int) fsw.Length;
                }
            }

            return length;
        }

        #endregion

        #region TODO: Implement by line copy logic using FileStream and classes text-adapters StreamReader/StreamWriter

        public static int ByLineCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            int lines = 0;
            using (StreamReader sr = new StreamReader(new FileStream(sourcePath, FileMode.Open, FileAccess.Read)))
            {
                using StreamWriter sw =
                    new StreamWriter(new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write));
                while (!sr.EndOfStream)
                {
                    if (lines > 0)
                    {
                        sw.Write("\n");
                    }
                    sw.Write(sr.ReadLine());
                    lines++;
                }
            }

            return lines;
        }

        #endregion

        #endregion

        #region Private members

        #region TODO: Implement validation logic

        private static void InputValidation(string sourcePath, string destinationPath)
        {
            if (sourcePath is null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            if (destinationPath == null)
            {
                throw new ArgumentNullException(nameof(destinationPath));
            }

            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException(
                    $"File '{sourcePath}' not found. Parameter name: {nameof(sourcePath)}.");
            }

            //if (!File.Exists(destinationPath))
            //{
            //    try
            //    {
            //        File.Create(destinationPath);
            //    }
            //    catch
            //    {
            //        throw new FileNotFoundException(
            //            $"File '{destinationPath}' not found and can not be created. Parameter name: {nameof(destinationPath)}.");
            //    }
            //}

            //if (new FileInfo(destinationPath).IsReadOnly)
            //{
            //    throw new FieldAccessException(
            //        $"File '{destinationPath}' is readonly. Parameter name: {nameof(destinationPath)}.");
            //}
        }

        #endregion

        #endregion
    }
}