using System;
using System.Diagnostics;

namespace KaupischITC.Shared
{
	/// <summary>
	/// Unterstützt die verzögerte Initialisierung.
	/// </summary>
	/// <typeparam name="T">Gibt den Objekttyp an, der verzögert initialisiert wird.</typeparam>
	public class Lazy<T>
	{
		private Func<T> valueFactory;	// der zum Erstellen des verzögert initialisierten Werts bei Bedarf aufgerufene Delegat
		private object boxed;			// Containerobjekt für das verzögert initialisierte Objekt


		/// <summary>
		/// Containerobjekt für bereits geladene Objekte
		/// </summary>
		private class Boxed   
		{
			public T Value { get; set; }  
		}


		/// <summary>
		/// Initialisiert ein neues Exemplar der Lazy-Klasse.Wenn eine verzögerte Initialisierung auftritt, wird die angegebene Initialisierungsfunktion verwendet.
		/// </summary>
		public Lazy()
		{ }



		/// <summary>
		/// Initialisiert ein neues Exemplar der Lazy-Klasse.Wenn eine verzögerte Initialisierung auftritt, wird die angegebene Initialisierungsfunktion verwendet.
		/// </summary>
		/// <param name="valueFactory">Der zum Erstellen des verzögert initialisierten Werts bei Bedarf aufgerufene Delegat.</param>
		public Lazy(Func<T> valueFactory)
		{
			if (valueFactory == null)
				throw new ArgumentNullException("valueFactory");
			this.valueFactory = valueFactory;
		}
		


		/// <summary>
		/// Ruft einen Wert ab, der angibt, ob ein Wert für diese Lazy-Instanz erstellt wurde.
		/// </summary>
		public bool IsValueCreated
		{
			get { return (this.boxed!=null && (this.boxed is Boxed)); }
		}


		/// <summary>
		/// Ruft den verzögert initialisierten Wert des aktuellen Lazy-Exemplars ab.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public T Value
		{
			get
			{
				// der Wert muss geladen werden
				if (this.boxed==null)
				{
					// entweder den Delegat zur Objekterzeugung verwenden (falls vorhanden), ansonsten parameterlosen Standardkonstruktor aufrufen
					T value = (this.valueFactory!=null) ? this.valueFactory() : (T)Activator.CreateInstance(typeof(T));
					Boxed result = new Boxed { Value = value };
					this.boxed = result;
					return result.Value;
				}
				// der Wert wurde bereits geladen
				else
				{					
					Boxed result = this.boxed as Boxed;
					if (result!=null) // das verzögerte Laden hatte geklappt
						return result.Value;
					else // das verzögerte laden hatte eine Ausnahme verursacht
						throw (Exception)this.boxed;
				}
			}
		}


		/// <summary>
		/// Erstellt eine Zeichenfolgendarstellung der Lazy.Value-Eigenschaft für diese Instanz und gibt sie zurück. (Überschreibt Object.ToString().)
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return (this.IsValueCreated) ? this.Value.ToString() : "Der verzögert geladene Wert wurde noch nicht geladen.";
		}		
	}
}
