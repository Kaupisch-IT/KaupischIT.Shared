using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaupischITC.Shared
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Flatten<T>(this T value,Func<T,IEnumerable<T>> childSelector)
		{
			yield return value;
			foreach (T flattendChild in childSelector(value).SelectMany(child => child.Flatten(childSelector)))
				yield return flattendChild;
		}
	}
}
