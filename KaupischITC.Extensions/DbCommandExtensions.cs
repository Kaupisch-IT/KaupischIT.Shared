using System.Data;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die IDbCommand-Schnittstelle bereit
	/// </summary>
	public static class DbCommandExtensions
	{
		/// <summary>
		/// Fügt am Ende der IDbParameterCollection einen Wert hinzu.
		/// </summary>
		/// <param name="dbCommand">der IDbCommand, zu dessen IDbParameterCollection ein Wert hinzugefügt werden soll</param>
		/// <param name="name">der Name des Parameters</param>
		/// <param name="value">der hinzuzufügende Wert</param>
		/// <returns>das hinzugefügte IDataParameter-Objekt</returns>
		public static IDataParameter AddParameterWithValue(this IDbCommand dbCommand,string name,object value)
		{
			IDataParameter result = dbCommand.CreateParameter();
			result.ParameterName = name;
			result.Value = value;
			dbCommand.Parameters.Add(result);
			return result;
		}
	}
}
