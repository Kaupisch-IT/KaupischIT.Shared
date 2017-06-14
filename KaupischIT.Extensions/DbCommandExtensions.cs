using System.Data;
using System.Reflection;

namespace KaupischIT.Extensions
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
			IDataParameter result = (dbCommand.Parameters.Contains(name)) ? (IDataParameter)dbCommand.Parameters[name] : dbCommand.CreateParameter();
			result.ParameterName = name;
			result.Value = value;
			dbCommand.Parameters.Add(result);
			return result;
		}


		/// <summary>
		/// Fügt die Werte der Eigenschaften des übergebenen Objekts als Parameter der IDbParameterCollection hinzu
		/// </summary>
		/// <param name="dbCommand">der IDbCommand, zu dessen IDbParameterCollection Parameterwerte hinzugefügt werden sollen</param>
		/// <param name="value">das Objekt, dessen Eigenschaftswerte der IDbParameterCollection hinzugefügt werden sollen</param>
		/// <returns>das IDbCommand-Objekt</returns>
		public static IDbCommand AddParamtersWithPropertiesOf(this IDbCommand dbCommand,object value)
		{
			if (value!=null)
				foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
					dbCommand.AddParameterWithValue(propertyInfo.Name,propertyInfo.GetValue(value,null));
			return dbCommand;
		}


		/// <summary>
		/// Führt eine SQL-Anweisung für das Connection-Objekt eines .NET Framework-Datenproviders aus und gibt die Anzahl der betroffenen Zeilen zurück.
		/// </summary>
		/// <param name="dbCommand">die Abfrage, die ausgeführt werden soll</param>
		/// <param name="parameterObject">das Objekt, dessen Eigenschaftswerte der IDbParameterCollection hinzugefügt werden sollen</param>
		/// <returns>die Anzahl der betroffenen Zeilen</returns>
		public static int ExecuteNonQuery(this IDbCommand dbCommand,object parameterObject) => dbCommand.AddParamtersWithPropertiesOf(parameterObject).ExecuteNonQuery();


		/// <summary>
		/// Führt die Abfrage aus und gibt die erste Spalte der ersten Zeile im der Ergebnismenge zurück, das durch die Abfrage zurückgegeben wird. Zusätzliche Spalten oder Zeilen werden ignoriert
		/// </summary>
		/// <typeparam name="T">der Datentyp der ersten Spalte der ersten Zeile in der Ergebnismenge</typeparam>
		/// <param name="dbCommand">die Abfrage, die ausgeführt werden soll</param>
		/// <param name="parameterObject">das Objekt, dessen Eigenschaftswerte der IDbParameterCollection hinzugefügt werden sollen</param>
		/// <returns>die erste Spalte der ersten Zeile in der Ergebnismenge</returns>
		public static T ExecuteScalar<T>(this IDbCommand dbCommand,object parameterObject) => (T)dbCommand.AddParamtersWithPropertiesOf(parameterObject).ExecuteScalar();


		/// <summary>
		/// Führt System.Data.IDbCommand.CommandText für die System.Data.IDbCommand.Connection aus und erstellt einen System.Data.IDataReader.
		/// </summary>
		/// <param name="dbCommand">die Abfrage, die ausgeführt werden soll</param>
		/// <param name="parameterObject">das Objekt, dessen Eigenschaftswerte der IDbParameterCollection hinzugefügt werden sollen</param>
		/// <returns>ein System.Data.IDataReader-Objekt</returns>
		public static IDataReader ExecuteReader(this IDbCommand dbCommand,object parameterObject) => dbCommand.AddParamtersWithPropertiesOf(parameterObject).ExecuteReader();
	}
}
