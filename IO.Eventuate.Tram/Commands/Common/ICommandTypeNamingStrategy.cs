using System;

namespace IO.Eventuate.Tram.Commands.Common
{
	/// <summary>
	/// Strategy for determining the command type name to use in the message header for a particular type of command
	/// </summary>
	public interface ICommandTypeNamingStrategy
	{
		/// <summary>
		/// Determine the command type to use in the message header for a particular type of command.
		/// </summary>
		/// <param name="commandType">Type of command.</param>
		/// <returns>Command type name for specified command type.</returns>
		string GetCommandTypeName(Type commandType);
	}
}