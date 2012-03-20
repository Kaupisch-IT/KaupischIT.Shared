using System;

namespace KaupischITC.Extensions
{
	public static class StructExtensions
	{
		public static Nullable<T> AsNullable<T>(T value) where T : struct
		{
			return new Nullable<T>(value);
		}
	}
}
