using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

[assembly: KaupischITC.Shared.Subversion("$Id$")]
namespace KaupischITC.Shared
{
	/// <summary>
	/// Liefert Informationen aus dem Versionsverwaltungssystem
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Assembly,AllowMultiple=true)]
	[Subversion("$Id$")]
	public class SubversionAttribute : Attribute
	{
		// Regex zum parsen der ID
		private static readonly Regex regex = new Regex(@"\$ id: \s*"
            +@"(?<headUrl>[^" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + @"]+) \s+"
            +@"(?<revision>\d+) \s+"
            +@"(?<date>\d\d\d\d-\d\d-\d\d \s+ \d\d:\d\d:\d\dZ) \s+"
            +@"(?<author>\w+)",
			RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);


		/// <summary>
		/// Gibt die komprimierte Version von Dateiname, Revision, Änderungsdatum und Autor zurück
		/// </summary>
		public string ID { get; private set; }

		/// <summary>
		/// Gibt den Autor der letzten bekannten Übertragung zurück
		/// </summary>
		public string Author { get; private set; }

		/// <summary>
		/// Gibt den vollständigen URL der Datei im Projektarchiv zurück
		/// </summary>
		public string HeadUrl { get; private set; }

		/// <summary>
		/// Gibt die Revisionsnummer der letzten bekannten Übertragung zurück
		/// </summary>
		public long Revision { get; private set; }

		/// <summary>
		/// Datum der letzten bekannten Übertragung. Dieses Datum hängt von der Information ab, die bei der letzten Aktualisierung der Arbeitskopie erhalten wurde. Es wird nicht das Projektarchiv auf neuere Änderungen abgefragt.
		/// </summary>
		public DateTime Date { get; private set; }

		/// <summary>
		/// Gibt an, ob das SVN-Schlüsselwort substituiert wurde.
		/// </summary>
		public bool IsSubstituted { get; private set; }


		/// <summary>
		/// Erstellt ein neues Subverions-Attribut
		/// </summary>
		/// <param name="id">die Subversion-ID</param>
		public SubversionAttribute(string id)
		{
			this.ID = id;
			this.IsSubstituted = (id.Trim().ToLower()!="$id"+"$");
			if (this.IsSubstituted)
			{
				Match match = regex.Match(id);
				this.HeadUrl = match.Groups["headUrl"].Value;
				this.Revision = Int64.Parse(match.Groups["revision"].Value);
				this.Date = DateTime.Parse(match.Groups["date"].Value);
				this.Author = match.Groups["author"].Value;
			}
		}


		/// <summary>
		/// Sucht das SubversionAttribute, das Informationen über die aktuellste Revision enthält.
		/// </summary>
		/// <param name="assembly">die Assembly, die durchsucht werden soll</param>
		/// <returns>das SubversionAttribute, das Informationen über die aktuellste Revision enthält, oder null</returns>
		public static SubversionAttribute FindLatest(Assembly assembly)
		{
			return FindLatest(assembly);
		}


		/// <summary>
		/// Sucht das SubversionAttribute, das Informationen über die aktuellste Revision enthält.
		/// </summary>
		/// <param name="type">der Typ, der durchsucht werden soll</param>
		/// <returns>das SubversionAttribute, das Informationen über die aktuellste Revision enthält, oder null</returns>
		public static SubversionAttribute FindLatest(Type type)
		{
			return FindLatest(type);
		}


		/// <summary>
		/// Sucht das SubversionAttribute, das Informationen über die aktuellste Revision enthält.
		/// </summary>
		/// <param name="customAttributeProvider">der CustomAttributeProvider zum Ermitteln der Attribute</param>
		/// <returns>das SubversionAttribute, das Informationen über die aktuellste Revision enthält, oder null</returns>
		private static SubversionAttribute FindLatest(ICustomAttributeProvider customAttributeProvider)
		{
			SubversionAttribute[] subversionAttributes = SubversionAttribute.findAll(customAttributeProvider).ToArray();
			if (subversionAttributes.Length>0)
			{
				Comparison<SubversionAttribute> comparer = (first,second) => second.Revision.CompareTo(first.Revision);
				Array.Sort(subversionAttributes,comparer);
				return subversionAttributes[0];
			}
			else
				return null;
		}



		/// <summary>
		/// Sucht alle SubversionAttribute, die Informationen über die verwendeten Revisionen erhalten
		/// </summary>
		/// <param name="assembly">die Assembly, die durchsucht werden soll</param>
		/// <returns>alle SubversionAttribute, die Informationen über die verwendeten Revisionen erhalten</returns>
		public static List<SubversionAttribute> FindAll(Assembly assembly)
		{
			return SubversionAttribute.findAll(assembly);
		}


		/// <summary>
		/// Sucht alle SubversionAttribute, die Informationen über die verwendeten Revisionen erhalten
		/// </summary>
		/// <param name="type">der Typ, der durchsucht werden soll</param>
		/// <returns>alle SubversionAttribute, die Informationen über die verwendeten Revisionen erhalten</returns>
		public static List<SubversionAttribute> FindAll(Type type)
		{
			return SubversionAttribute.findAll(type);
		}


		/// <summary>
		/// Sucht alle SubversionAttribute, die Informationen über die verwendeten Revisionen erhalten
		/// </summary>
		/// <param name="customAttributeProvider">der CustomAttributeProvider zum Ermitteln der Attribute</param>
		/// <returns>alle SubversionAttribute, die Informationen über die verwendeten Revisionen erhalten</returns>
		private static List<SubversionAttribute> findAll(ICustomAttributeProvider customAttributeProvider)
		{
			List<SubversionAttribute> result = new List<SubversionAttribute>();
			foreach (SubversionAttribute subversionAttribute in customAttributeProvider.GetCustomAttributes(typeof(SubversionAttribute),false))
				if (subversionAttribute.IsSubstituted)
					result.Add(subversionAttribute);
			return result;
		}



		/// <summary>
		/// Gibt die Zeichenkettenrepräsentation dieses SubversionAttributes zurück
		/// </summary>
		/// <returns>die Zeichenkettenrepräsentation dieses SubversionAttributes</returns>
		public override string ToString()
		{
			return this.ID;
		}
	}
}
