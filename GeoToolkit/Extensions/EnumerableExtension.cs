using System.Collections;
using System.Linq;
using System.Text;

namespace GeoToolkit.Extensions
{
	public static class EnumerableExtension
	{
		public static string ToDelimitedString(this IEnumerable list, char delimiter)
		{
			var builder = new StringBuilder();
		    foreach (object obj in list.Cast<object>().Where(obj => obj != null))
		        builder.Append(obj).Append(delimiter);

		    return builder.ToString().TrimEnd(delimiter);
		}

		public static string ToDelimitedString(this IEnumerable list)
		{
			return ToDelimitedString(list, ';');
		}
	}
}