using System;

namespace IO.Eventuate.Tram.Commands.Common
{
	/// <summary>
	/// Allows overriding the command type used in the header of produced command messages
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class CommandTypeAttribute : Attribute
	{
		/// <summary>
		/// Command type name to use
		/// </summary>
		public string CommandType { get; }

		/// <summary>
		/// Overrides the command type name used in the header of produced command messages
		/// </summary>
		/// <param name="commandType">Command type name to use</param>
		public CommandTypeAttribute(string commandType)
		{
			CommandType = commandType;
		}
	}
}