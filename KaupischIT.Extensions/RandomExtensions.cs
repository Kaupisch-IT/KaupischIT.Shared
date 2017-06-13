using System;
using System.Linq;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Random-Klasse bereit
	/// </summary>
	public static class RandomExtensions
	{
		/// <summary>
		/// Gibt eine normalverteilte Zufallszahl im angegebenen Bereich zurück.
		/// </summary>
		/// <param name="random">das Random-Objekt zum Bestimmen von Zufallszahlen</param>
		/// <param name="minValue">die inklusive untere Grenze der zurückgegebenen Zufallszahl</param>
		/// <param name="maxValue">die exklusive obere Grenze der zurückgegebenen Zufallszahl.</param>
		/// <returns>eine normalverteilte Zufallszahl im angegebenen Bereich</returns>
		public static int NextNormal(this Random random,int minValue,int maxValue)
		{
			return (int)((maxValue-minValue) * Enumerable.Range(0,12).Average(_ => random.NextDouble())) + minValue;
		}


		/// <summary>
		/// Gibt eine normalverteilte Zufallszahl zwischen 0,0 und 1,0 zurück.
		/// </summary>
		/// <param name="random">das Random-Objekt zum Bestimmen von Zufallszahlen</param>
		/// <returns>eine normalverteilte Zufallszahl zwischen 0,0 und 1,0</returns>
		public static double NextNormalDouble(this Random random)
		{
			return Enumerable.Range(0,12).Average(_ => random.NextDouble());
		}
	}
}
