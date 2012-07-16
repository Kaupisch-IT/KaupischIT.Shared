using System;

namespace KaupischITC.Extensions
{
	public static class EventHandlerExtensions
	{
		public static void Raise(this EventHandler eventHandler,object sender,EventArgs eventArgs)
		{
			if (eventHandler!=null)
				eventHandler(sender,eventArgs);
		}

		public static void Raise<T>(this EventHandler<T> eventHandler,object sender,T eventArgs) where T : EventArgs
		{
			if (eventHandler!=null)
				eventHandler(sender,eventArgs);
		}
	}
}
