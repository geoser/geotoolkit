using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace GeoToolkit.Net
{
    public static class HttpConnector
    {
        public static XmlDocument LoadXml(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            var req = WebRequest.Create(uri) as HttpWebRequest;

            if (req == null)
                throw new ArgumentException("uri must be of http type", "uri");

            using (var response = req.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                    throw new InvalidOperationException("Cannot get response stream");

                var doc = new XmlDocument();

                doc.Load(stream);

                return doc;
            }
        }

        public static XmlDocument LoadXml(string uriString)
        {
            if (string.IsNullOrEmpty(uriString))
                throw new ArgumentNullException("uriString");

            return LoadXml(new Uri(uriString));
        }

        public static string LoadText(Uri uri, Encoding encoding = null)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            var req = WebRequest.Create(uri) as HttpWebRequest;

            if (req == null)
                throw new ArgumentException("uri must be of http type", "uri");

            using (var response = req.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                    throw new InvalidOperationException("Cannot get response stream");
                
                using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }

        public static string LoadText(string uriString, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(uriString))
                throw new ArgumentNullException("uriString");

            return LoadText(new Uri(uriString), encoding);
        }
    }
}