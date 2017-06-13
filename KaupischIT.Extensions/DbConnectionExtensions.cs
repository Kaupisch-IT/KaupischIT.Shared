using System.Data;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die IDbConnection-Schnittstelle bereit
	/// </summary>
	public static class DbConnectionExtensions
	{
		/// <summary>
		/// Führt eine SQL-Anweisung für das Connection-Objekt eines .NET Framework-Datenproviders aus und gibt die Anzahl der betroffenen Zeilen zurück.
		/// </summary>
		/// <param name="dbConnection">die Datenbankverbindung, in deren Kontext die Abfrage ausgeführt werden soll</param>
		/// <param name="commandText">der Textbefehl, der für die Datenquelle ausgeführt werden</param>
		/// <param name="parameterObject">das Objekt, dessen Eigenschaftswerte der IDbParameterCollection hinzugefügt werden sollen</param>
		/// <returns>die Anzahl der betroffenen Zeilen</returns>
		public static int ExecuteNonQuery(this IDbConnection dbConnection,string commandText,object parameterObject)
		{
			using (IDbCommand dbCommand = dbConnection.CreateCommand())
			{
				dbCommand.CommandText = commandText;
				return dbCommand.ExecuteNonQuery(parameterObject);
			}
		}


		/// <summary>
		/// Führt die Abfrage aus und gibt die erste Spalte der ersten Zeile in der Ergebnismenge zurück, das durch die Abfrage zurückgegeben wird. Zusätzliche Spalten oder Zeilen werden ignoriert
		/// </summary>
		/// <typeparam name="T">der Datentyp der ersten Spalte der ersten Zeile in der Ergebnismenge</typeparam>
		/// <param name="dbConnection">die Datenbankverbindung, in deren Kontext die Abfrage ausgeführt werden soll</param>
		/// <param name="commandText">der Textbefehl, der für die Datenquelle ausgeführt werden</param>
		/// <param name="parameterObject">das Objekt, dessen Eigenschaftswerte der IDbParameterCollection hinzugefügt werden sollen</param>
		/// <returns>die erste Spalte der ersten Zeile in der Ergebnismenge</returns>
		public static T ExecuteScalar<T>(this IDbConnection dbConnection,string commandText,object parameterObject)
		{
			using (IDbCommand dbCommand = dbConnection.CreateCommand())
			{
				dbCommand.CommandText = commandText;
				return dbCommand.ExecuteScalar<T>(parameterObject);
			}
		}


		/// <summary>
		/// Führt System.Data.IDbCommand.CommandText für die System.Data.IDbCommand.Connection aus und erstellt einen System.Data.IDataReader.
		/// </summary>
		/// <param name="dbConnection">die Datenbankverbindung, in deren Kontext die Abfrage ausgeführt werden soll</param>
		/// <param name="commandText">der Textbefehl, der für die Datenquelle ausgeführt werden</param>
		/// <param name="parameterObject">das Objekt, dessen Eigenschaftswerte der IDbParameterCollection hinzugefügt werden sollen</param>
		/// <returns>ein System.Data.IDataReader-Objekt</returns>
		public static IDataReader ExecuteReader(this IDbConnection dbConnection,string commandText,object parameterObject)
		{
			using (IDbCommand dbCommand = dbConnection.CreateCommand())
			{
				dbCommand.CommandText = commandText;
				return dbCommand.ExecuteReader(parameterObject);
			}
		}
	}
}
