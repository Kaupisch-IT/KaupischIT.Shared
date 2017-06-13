using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die XmlDocument-Klasse bereit
	/// </summary>
	public static class XmlDocumentExtensions
	{
		/// <summary>
		/// Wählt den ersten XmlNode aus, der mit dem XPath-Ausdruck übereinstimmt.
		/// Falls der XmlNode noch nicht existiert, wird er angelegt.
		/// </summary>
		public static XmlNode SelectOrAddSingleNode(this XmlDocument xmlDocument,string xpath)
		{
			return (xmlDocument as XmlNode).SelectOrAddSingleNode(xpath);
		}

		/// <summary>
		/// Wählt den ersten XmlNode aus, der mit dem XPath-Ausdruck übereinstimmt.
		/// Falls der XmlNode noch nicht existiert, wird er angelegt.
		/// </summary>
		public static XmlNode SelectOrAddSingleNode(this XmlNode parentNode,string xpath)
		{
			string[] xpathParts = xpath.Trim('/').Split('/');
			string nextNode = xpathParts.First();

			if (string.IsNullOrEmpty(nextNode))
				return parentNode;

			XmlNode xmlNode = parentNode.SelectSingleNode(nextNode);
			if (xmlNode==null)
			{
				// Element mit Attribut-Filter
				Match match = Regex.Match(nextNode,@"^(?<elementName>\w+)\[@(?<attributeName>\w+)='?(?<attributeValue>[^\]]+?)'?\]$");
				if (match.Success)
				{
					xmlNode = parentNode.AppendChild(parentNode.OwnerDocument.CreateElement(match.Groups["elementName"].Value));
					xmlNode.Attributes.Append(parentNode.OwnerDocument.CreateAttribute(match.Groups["attributeName"].Value)).Value = match.Groups["attributeValue"].Value;
				}
				// Attribut
				else if (nextNode.StartsWith("@"))
					xmlNode = parentNode.Attributes.Append(parentNode.OwnerDocument.CreateAttribute(nextNode.Substring(1)));
				// normales Element
				else
					xmlNode = parentNode.AppendChild(parentNode.OwnerDocument.CreateElement(nextNode));
			}

			string rest = String.Join("/",xpathParts.Skip(1).ToArray());
			return xmlNode.SelectOrAddSingleNode(rest);
		}
	}
}
