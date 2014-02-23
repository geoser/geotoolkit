using System;
using System.IO;
using System.IO.Compression;
using System.Text;
#if DEBUG
using NUnit.Framework;
#endif

namespace GeoToolkit.Compression
{
    public static class GZipHelper
    {
        public static byte[] CompressToBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                return new byte[0];

            MemoryStream outputStream;
            
            using (var inputStream = new MemoryStream(data))
            using (outputStream = new MemoryStream())
            using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress, false))
                inputStream.CopyTo(gZipStream);

            return outputStream.ToArray();
        }

        public static byte[] CompressToBytes(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data == string.Empty)
                return new byte[0];

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            return CompressToBytes(bytes);
        }

        public static byte[] DecompressToBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                return new byte[0];

            MemoryStream outputStream;
            
            using (var inputStream = new MemoryStream(data))
            using (outputStream = new MemoryStream())
            using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress, false))
                gZipStream.CopyTo(outputStream);

            return outputStream.ToArray();
        }

        public static string DecompressToString(byte[] data)
        {
            var bytes = DecompressToBytes(data);

            return Encoding.UTF8.GetString(bytes);
        }
    }

#if DEBUG
    [TestFixture]
    public class CompressorTest
    {
        [Test]
        public void CompressTest()
        {
            var str = "Тестовая строка для тестирования компрессии/декомпрессии ждыво ащф ыуждфыоважщфш оывдафывждаоф ыжщуаофж ываф оруафдытадфытвжфжаф рыуарфжажф щшвжфыражфыружафры жларфжвлф ывр фыражфщыруажщ фрыж вало вало валова лвоал влаоваловаловалвоаловаловаловаловаловаловаловаловаловало";

            var compressedBytes = GZipHelper.CompressToBytes(str);

            Assert.NotNull(compressedBytes);
            Assert.Greater(compressedBytes.Length, 0);

            var decompressedStr = GZipHelper.DecompressToString(compressedBytes);

            Assert.IsNotNull(decompressedStr);
            Assert.AreEqual(str, decompressedStr);
        }
    }
#endif
}