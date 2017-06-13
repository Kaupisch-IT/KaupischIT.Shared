using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Exception-Klasse bereit
	/// </summary>
	public static class ExceptionExtensions
	{
		private static readonly Action<Exception> internalPreserveStackTrace; // Delegat auf die interne "InternalPreserveStackTrace"-Methode des Exception-Objekts

		/// <summary>
		/// Statischer Konstruktor
		/// </summary>
		static ExceptionExtensions()
		{
			MethodInfo methodInfo = typeof(Exception).GetMethod("InternalPreserveStackTrace",BindingFlags.Instance | BindingFlags.NonPublic);
			if (methodInfo!=null)
				ExceptionExtensions.internalPreserveStackTrace = (Action<Exception>)Delegate.CreateDelegate(typeof(Action<Exception>),methodInfo);
		}


		/// <summary>
		/// Erzeugt eine Exception, die den ursprünglichen Aufrufstapel beibehält
		/// </summary>
		/// <param name="exception">die Ausnahme, deren ursprünglicher Aufrufstapel beibehalten werden soll</param>
		/// <returns>die übergebene Ausnahme</returns>
		public static Exception PreserveStackTrace(this Exception exception)
		{
			// wenn die interne InternalPreserveStackTrace-Methode gefunden wurde, diese für die Ausnahme aufrufen
			if (ExceptionExtensions.internalPreserveStackTrace!=null)
				ExceptionExtensions.internalPreserveStackTrace(exception);
			// ansonsten die Remoting-Funktionalitäten benutzen, um den ursprünglichen Aufrufstapel zu behalten
			else
			{
				var streamingContext = new StreamingContext(StreamingContextStates.CrossAppDomain);
				var objectManager = new ObjectManager(null,streamingContext);
				var serializationInfo  = new SerializationInfo(exception.GetType(),new FormatterConverter());

				exception.GetObjectData(serializationInfo,streamingContext);
				objectManager.RegisterObject(exception,1,serializationInfo); // prepare for SetObjectData
				objectManager.DoFixups(); // ObjectManager calls SetObjectData
			}
			return exception;
		}
	}
}
