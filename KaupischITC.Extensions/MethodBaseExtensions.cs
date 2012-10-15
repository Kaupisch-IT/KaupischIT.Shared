using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die MethodBase-Klasse bereit
	/// </summary>
	public static class MethodBaseExtensions
	{
		private static readonly OpCode[] oneByteOpCodes = new OpCode[0x100]; // enthält die 1 Byte großen OpCodes
		private static readonly OpCode[] twoByteOpCodes = new OpCode[0x100]; // enthält die 2 Byte großen OpCodes

		/// <summary>
		/// Statischer Konstruktor
		/// </summary>
		static MethodBaseExtensions()
		{
			// OpCodes "sortieren"
			foreach (FieldInfo fieldInfo in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static))
			{
				OpCode opCode = (OpCode)fieldInfo.GetValue(null);
				UInt16 value = (UInt16)opCode.Value;
				if (value < 0x100)
					oneByteOpCodes[value] = opCode;
				else if ((value & 0xff00) == 0xfe00)
					twoByteOpCodes[value & 0xff] = opCode;
			}
		}


		/// <summary>
		/// Ermittelt alle Methodenaufrufe innerhalb der übergebenen Methode
		/// </summary>
		/// <param name="methodInfo">die Methode, deren Methodenaufrufe ermittelt werden solle</param>
		/// <returns>eine Auflistung aller Methoden, die in der übergebene Methoden aufgerufen wurde</returns>
		public static IEnumerable<MethodBase> GetCalls(this MethodBase methodInfo)
		{
			MethodBody methodBody = methodInfo.GetMethodBody();
			if (methodBody!=null)
			{
				byte[] ilBytes = methodBody.GetILAsByteArray();
				for (int position=0;position<ilBytes.Length;position++)
				{
					OpCode opCode = (ilBytes[position]!=0xFE) ? oneByteOpCodes[ilBytes[position]] : twoByteOpCodes[ilBytes[++position]];

					if (opCode==OpCodes.Call || opCode==OpCodes.Callvirt)
					{
						Type[] genericTypeArguments = methodInfo.DeclaringType.GetGenericArguments();
						Type[] genericMethodArguments = methodInfo.GetGenericArguments();
						yield return methodInfo.Module.ResolveMethod(BitConverter.ToInt32(ilBytes,position+1),genericTypeArguments,genericMethodArguments);
					}

					switch (opCode.OperandType)
					{
						case OperandType.InlineNone:
							break;
						case OperandType.ShortInlineBrTarget:
							position += sizeof(SByte);
							break;
						case OperandType.ShortInlineI:
						case OperandType.ShortInlineVar:
							position += sizeof(Byte);
							break;
						case OperandType.InlineI8:
							position += sizeof(Int64);
							break;
						case OperandType.ShortInlineR:
							position += sizeof(Single);
							break;
						case OperandType.InlineR:
							position += sizeof(Double);
							break;
						case OperandType.InlineVar:
							position += sizeof(UInt16);
							break;
						case OperandType.InlineBrTarget:
						case OperandType.InlineField:
						case OperandType.InlineI:
						case OperandType.InlineMethod:
						case OperandType.InlineSig:
						case OperandType.InlineString:
						case OperandType.InlineType:
							position += sizeof(Int32);
							break;
						case OperandType.InlineSwitch:
							position += sizeof(Int32) + sizeof(Int32)*BitConverter.ToInt32(ilBytes,position);
							break;
						case OperandType.InlineTok:
							position += sizeof(Int32);
							break;
						default:
							throw new NotSupportedException("Unerwarteter Operandentyp "+opCode.OperandType);
					}
				}
			}
		}
	}
}
