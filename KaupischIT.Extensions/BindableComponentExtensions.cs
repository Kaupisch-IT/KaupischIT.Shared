using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die IBindableComponent-Schnittstelle bereit
	/// </summary>
	public static class BindableComponentExtensions
	{
		/// <summary>
		///  Erstellt eine Bindung, die die angegebene Steuerelementeigenschaft an den angegebenen Datenmember der angegebenen Datenquelle bindet, wodurch optional die Formatierung aktiviert wird, Werte basierend auf der angegebenen Aktualisierungseinstellung an die Datenquelle weitergegeben werden und der Auflistung die Bindung hinzugefügt wird.
		/// </summary>
		/// <typeparam name="TBindable">der Typ der Komponente, an die Daten gebunden werden sollen</typeparam>
		/// <typeparam name="TSource">der Typ des Objekts, das als Datenquelle dient</typeparam>
		/// <param name="bindable">die Komponente, an die Daten gebunden werden sollen</param>
		/// <param name="propertySelector">die Steuerelementeigenschaft für die Bindung</param>
		/// <param name="dataSource">ein System.Object, das die Datenquelle darstellt</param>
		/// <param name="dataMemberSelector">die Eigenschaft, an die die Bindung erfolgen soll</param>
		/// <param name="dataSourceUpdateMode"></param>
		/// <returns>das neu erstellte System.Windows.Forms.Binding</returns>
		public static Binding SetBinding<TBindable, TSource>(this TBindable bindable,Expression<Func<TBindable,object>> propertySelector,TSource dataSource,Expression<Func<TSource,object>> dataMemberSelector,DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged) where TBindable : IBindableComponent
		{
			string propertyName = propertySelector.GetMemberName();
			string memberName = dataMemberSelector.GetMemberName();

			bindable.RemoveBinding(propertySelector);
			return bindable.DataBindings.Add(propertyName,dataSource,memberName,true,dataSourceUpdateMode);
		}


		/// <summary>
		///  Entfernt die angegebene Datenbindung
		/// </summary>
		/// <typeparam name="TBindable">der Typ der Komponente, an die Daten gebunden werden sollen</typeparam>
		/// <param name="bindable">die Komponente, an die Daten gebunden werden sollen</param>
		/// <param name="propertySelector">die Steuerelementeigenschaft für die Bindung</param>
		public static void RemoveBinding<TBindable>(this TBindable bindable,Expression<Func<TBindable,object>> propertySelector) where TBindable : IBindableComponent
		{
			string propertyName = propertySelector.GetMemberName();
			Binding oldBinding = bindable.DataBindings[propertyName];
			if (oldBinding!=null)
				bindable.DataBindings.Remove(oldBinding);
		}
	}
}
