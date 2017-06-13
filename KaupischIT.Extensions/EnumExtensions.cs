using System;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Enum-Klasse bereit
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// Bestimmt, ob ein oder mehrere Bitfelder in der aktuellen Instanz festgelegt werden.
		/// </summary>
		/// <param name="variable">die aktuelle Instanz</param>
		/// <param name="flag">ein Enumerationswert.</param>
		/// <returns>true, wenn das in flag festgelegte Bitfeld bzw. die Bitfelder auch in der aktuellen Instanz festgelegt werden, andernfalls false.</returns>
		public static bool HasFlag(this Enum variable,Enum flag)
		{
			if (variable==null)
				return false;
			if (flag==null)
				throw new ArgumentNullException("value");

			if (!Enum.IsDefined(variable.GetType(),flag))
				throw new ArgumentException(string.Format("Enumeration type mismatch. The flag is of type '{0}', was expecting '{1}'.",flag.GetType(),variable.GetType()));

			ulong num = Convert.ToUInt64(flag);
			return ((Convert.ToUInt64(variable) & num) == num);
		}
	}
}
