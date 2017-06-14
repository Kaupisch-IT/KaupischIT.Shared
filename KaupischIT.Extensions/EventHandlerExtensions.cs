using System;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die EventHandler-Klasse bereit
	/// </summary>
	public static class EventHandlerExtensions
	{
		/// <summary>
		/// Löst ein Ereignis aus
		/// </summary>
		/// <param name="eventHandler">das Ereignis, das ausgelöst werden soll</param>
		/// <param name="sender">die Quelle des Ereignisses</param>
		/// <param name="eventArgs">ein System.EventArgs, das die Ereignisdaten enthält</param>
		public static void Raise(this EventHandler eventHandler,object sender,EventArgs eventArgs) => eventHandler?.Invoke(sender,eventArgs);

		/// <summary>
		/// Löst ein Ereignis aus
		/// </summary>
		/// <typeparam name="T">der Typ der vom Ereignis generierten Ereignisdaten</typeparam>
		/// <param name="eventHandler">das Ereignis, das ausgelöst werden soll</param>
		/// <param name="sender">die Quelle des Ereignisses</param>
		/// <param name="eventArgs">ein System.EventArgs, das die Ereignisdaten enthält</param>
		public static void Raise<T>(this EventHandler<T> eventHandler,object sender,T eventArgs) where T : EventArgs => eventHandler?.Invoke(sender,eventArgs);
	}
}
