using System;
using System.Reflection;

namespace IO.Eventuate.Tram.Commands.Common
{
	/// <summary>
	/// Uses the CommandTypeAttribute if present to determine the command type name to use in the message header
	/// for a particular type of command. Otherwise, falls back to use the the full name of the type.
	/// </summary>
	public class AttributeCommandTypeNamingStrategy : ICommandTypeNamingStrategy
	{
		/// <inheritdoc />
		public string GetCommandTypeName(Type commandType)
		{
			// Get command type attribute
			var commandTypeAttribute = commandType.GetCustomAttribute<CommandTypeAttribute>();

			// Use command type from attribute, if it exists; otherwise, use type full name
			return commandTypeAttribute != null ? commandTypeAttribute.CommandType : commandType.FullName;
		}
	}
}