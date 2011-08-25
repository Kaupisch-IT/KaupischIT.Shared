using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace KaupischITC.Extensions
{
	public static class ExceptionExtensions
	{
		public static void RethrowWithNoStackTraceLoss(this Exception exception)
		{
			FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString",  BindingFlags.Instance | BindingFlags.NonPublic);
			remoteStackTraceString.SetValue(exception,exception.StackTrace);
			throw exception;
		}
	}
}
