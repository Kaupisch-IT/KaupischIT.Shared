using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace KaupischITC.Extensions
{
	public static class ExceptionExtensions
	{
		private static readonly Action<Exception> internalPreserveStackTrace;

		static ExceptionExtensions()
		{
			MethodInfo methodInfo = typeof(Exception).GetMethod("InternalPreserveStackTrace",BindingFlags.Instance | BindingFlags.NonPublic);
			if (methodInfo!=null)
				ExceptionExtensions.internalPreserveStackTrace = (Action<Exception>)Delegate.CreateDelegate(typeof(Action<Exception>),methodInfo);
		}

		public static Exception PreserveStackTrace(this Exception exception)
		{
			if (ExceptionExtensions.internalPreserveStackTrace!=null)
				ExceptionExtensions.internalPreserveStackTrace(exception);
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
