using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GeoToolkit.Serialization
{
	public class XmlSerializer
	{
		private static readonly ConcurrentDictionary<Encoding, XmlSerializer> _serializers =
            new ConcurrentDictionary<Encoding, XmlSerializer>();

		private Encoding _encoding;

		public string EncodingName
		{
			get { return _encoding.EncodingName; }
			set { _encoding = Encoding.GetEncoding(value); }
		}

		private XmlSerializer(Encoding encoding)
		{
			_encoding = encoding;
		}

		/// <summary>
		/// Returns serializer which uses default UTF8 encoding
		/// </summary>
		public static XmlSerializer Default
		{
			get { return GetSerializer(Encoding.UTF8); }
		}

		public static XmlSerializer ISO_8859_1
		{
			get { return GetSerializer(Encoding.GetEncoding("ISO-8859-1")); }
		}

		/// <summary>
		/// Return serializer which uses specified encoding
		/// </summary>
		/// <param name="encodingName">Encoding name. Must be supported by <see cref="Encoding"/> class</param>
		/// <returns></returns>
		public static XmlSerializer GetSerializer(string encodingName)
		{
			return GetSerializer(Encoding.GetEncoding(encodingName));
		}

		public static XmlSerializer GetSerializer(Encoding encoding)
		{
		    return _serializers.GetOrAdd(encoding, enc => new XmlSerializer(enc));
		}

		/// <summary>
		/// Method to convert a custom Object to XML string
		/// </summary>
		/// <param name="obj">Object that is to be serialized to XML</param>
		/// <returns>XML string</returns>
		public string Serialize<T>(T obj)
		{
			return RemoveHeader(ByteArrayToString(SerializeToBytes(obj)));
		}

		private static string RemoveHeader(string xml)
		{
			return xml.Substring(xml.IndexOf(">") + 1);
		}

		/// <summary>
		/// Method to convert a custom Object to XML string
		/// </summary>
		/// <param name="obj">Object that is to be serialized to XML</param>
		/// <returns>XML string</returns>
		public byte[] SerializeToBytes<T>(T obj)
		{
			using (var memoryStream = new MemoryStream())
			{
			    var xs = XmlSerializerCache.GetXmlSerializer(typeof(T));
			    using (var xmlTextWriter = new XmlTextWriter(memoryStream, _encoding))
			    {
			        xs.Serialize(xmlTextWriter, obj);

			        return memoryStream.ToArray();
			    }
			}
		}

		public T Deserialize<T>(string xml)
		{
		    return xml != string.Empty ? Deserialize<T>(StringToByteArray(xml)) : default(T);
		}

	    /// <summary>
		/// Method to reconstruct an Object from XML byte array
		/// </summary>
		/// <param name="byteArray"></param>
		/// <returns></returns>
		public static T Deserialize<T>(byte[] byteArray)
	    {
	        if (byteArray == null || byteArray.Length <= 0)
	            return default(T);

	        var xs = XmlSerializerCache.GetXmlSerializer(typeof(T));

	        using (var memoryStream = new MemoryStream(byteArray))
                return (T)xs.Deserialize(memoryStream);
	    }

	    /// <summary>
		/// Method to convert a custom Object to XML file
		/// </summary>
		/// <param name="obj">Object that is to be serialized to XML</param>
		/// <param name="fileName">Name of file to serialize in</param>
		/// <returns>XML string</returns>
		public void SerializeToFile<T>(T obj, string fileName)
		{
			var serializer = XmlSerializerCache.GetXmlSerializer(typeof(T));

			using (Stream fs = new FileStream(fileName, FileMode.Create))
			using (XmlWriter writer = new XmlTextWriter(fs, _encoding))
                serializer.Serialize(writer, obj);
		}

		/// <summary>
		/// Method to reconstruct an Object from XML file
		/// </summary>
		/// <param name="fileName">File to read</param>
		/// <returns></returns>
		public T DeserializeFromFile<T>(string fileName)
		{
			var serializer = XmlSerializerCache.GetXmlSerializer(typeof(T));

		    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
		    using (XmlReader reader = new XmlTextReader(fs))
		        return (T) serializer.Deserialize(reader);
		}

		private static class XmlSerializerCache
		{
			private static readonly ConcurrentDictionary<string, System.Xml.Serialization.XmlSerializer> _serializerCache =
                new ConcurrentDictionary<string, System.Xml.Serialization.XmlSerializer>();

			public static System.Xml.Serialization.XmlSerializer GetXmlSerializer(Type type)
			{
				return GetXmlSerializer(type, string.Empty, s => new System.Xml.Serialization.XmlSerializer(type));
			}

		    private static System.Xml.Serialization.XmlSerializer GetXmlSerializer(Type type, string customKey, Func<string, System.Xml.Serialization.XmlSerializer> factory)
			{
				string key = string.Concat(customKey, type.AssemblyQualifiedName);

			    var serializer =
			        _serializerCache.GetOrAdd(key,
			                                  s =>
			                                      {
			                                          var res = factory(s);

			                                          res.UnknownElement += SerializerUnknownElement;
			                                          res.UnknownAttribute += SerializerUnknownAttribute;
			                                          res.UnknownNode += SerializerUnknownNode;
			                                          res.UnreferencedObject += SerializerUnreferencedObject;

			                                          return res;
			                                      });

				return serializer;
			}

			static void SerializerUnreferencedObject(object sender, UnreferencedObjectEventArgs e)
			{
				Debug.WriteLine(e.UnreferencedId);
			}

			static void SerializerUnknownNode(object sender, XmlNodeEventArgs e)
			{
				Debug.WriteLine(e.Text);
			}

			static void SerializerUnknownAttribute(object sender, XmlAttributeEventArgs e)
			{
				Debug.WriteLine(e.Attr.OuterXml);
			}

			static void SerializerUnknownElement(object sender, XmlElementEventArgs e)
			{
				Debug.WriteLine(e.Element.OuterXml);
			}
		}

		/// <summary>
		/// Converts the String to UTF8 Byte array and is used in Deserialization
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		private byte[] StringToByteArray(string xml)
		{
		    return string.IsNullOrEmpty(xml) ? new byte[0] : _encoding.GetBytes(xml);
		}

	    /// <summary>
		/// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
		/// </summary>
		/// <param name="characters">Unicode Byte Array to be converted to String</param>
		/// <returns>String converted from Unicode Byte Array</returns>
		private string ByteArrayToString(byte[] characters)
	    {
	        return characters != null ? (_encoding.GetString(characters)) : string.Empty;
	    }
	}
}