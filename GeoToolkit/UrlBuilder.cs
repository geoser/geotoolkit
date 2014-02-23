using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace GeoToolkit
{
	public class UrlBuilder : UriBuilder
	{
		public readonly StringDictionary QueryString = new StringDictionary();

		public string this[string i]
		{
			get { return QueryString[i]; }
			set { QueryString[i] = value; }
		}

		public string PageName
		{
			get
			{
				string path = Path;
				return path.Substring(path.LastIndexOf("/") + 1);
			}
			set
			{
				string path = Path;
				path = path.Substring(0, path.LastIndexOf("/"));
				Path = string.Concat(path, "/", value);
			}
		}

		#region Constructor overloads

		public UrlBuilder()
		{
		}

		public UrlBuilder(string uri) : base(uri)
		{
			PopulateQueryString();
		}

		public UrlBuilder(Uri uri) : base(uri)
		{
			PopulateQueryString();
		}

		public UrlBuilder(string schemeName, string hostName) : base(schemeName, hostName)
		{
		}

		public UrlBuilder(string scheme, string host, int portNumber) : base(scheme, host, portNumber)
		{
		}

		public UrlBuilder(string scheme, string host, int port, string pathValue) : base(scheme, host, port, pathValue)
		{
		}

		public UrlBuilder(string scheme, string host, int port, string path, string extraValue)
			: base(scheme, host, port, path, extraValue)
		{
		}

		public UrlBuilder(Page page) : base(page.Request.Url.AbsoluteUri)
		{
			PopulateQueryString();
		}

		#endregion

		#region Public methods

		public override string ToString()
		{
			FillQueryString();

			return Uri.AbsoluteUri;
		}

	    public void Navigate(bool endResponse = true)
		{
			HttpContext.Current.Response.Redirect(ToString(), endResponse);
		}

		#endregion

		#region Private methods

		private void PopulateQueryString()
		{
			string query = Query;

			if (string.IsNullOrEmpty(query))
				return;

			QueryString.Clear();

			query = query.Substring(1); //remove the ?

			string[] pairs = query.Split(new[] {'&'});
			foreach (string s in pairs)
			{
				string[] pair = s.Split(new[] {'='});

				QueryString[pair[0]] = (pair.Length > 1) ? pair[1] : string.Empty;
			}
		}

		private void FillQueryString()
		{
			int count = QueryString.Count;

			if (count == 0)
			{
				Query = string.Empty;
				return;
			}

			var keys = new string[count];
			var values = new string[count];
			var pairs = new List<string>(count);
		    
            pairs.AddRange(from string key in QueryString.Keys
		                   where QueryString[key] != null
		                   select string.Concat(key, "=", QueryString[key]));

		    for (var i = 0; i < count; i++)
				if (values[i] != null)
					pairs[i] = string.Concat(keys[i], "=", values[i]);

			Query = string.Join("&", pairs.ToArray());
		}

		#endregion
	}
}