﻿// ======================================================== IDataLink.cs
namespace Kerosene.ORM.Direct
{
	using Kerosene.Tools;
	using System;
	using System.Data;

	// ==================================================== 
	/// <summary>
	/// Represents an abstract direct connection against an underlying database-alike service.
	/// </summary>
	public interface IDataLink : Core.IDataLink
	{
		/// <summary>
		/// Returns a new instance that is a copy of the original one.
		/// </summary>
		/// <returns>A new instance.</returns>
		new IDataLink Clone();

		/// <summary>
		/// The engine this link is associated with.
		/// </summary>
		new IDataEngine Engine { get; }

		/// <summary>
		/// The abstract nestable transaction this instance maintains. If the reference this
		/// property maintains is null, or if it is is disposed, the getter generates a new
		/// instance on demand. This property can return null if the link is disposed.
		/// </summary>
		new INestableTransaction Transaction { get; }

		/// <summary>
		/// Gets or sets the connection string this instance is currently using.
		/// <para>The setter accepts:</para>
		/// <para>- Null to use the default connection string entry from the configuration files.</para>
		/// <para>- The name of one connection string entry in the configuration files.</para>
		/// <para>- The actual contents of the connection string.</para>
		/// </summary>
		string ConnectionString { get; set; }

		/// <summary>
		/// The server this link is connected to, or null if this information is not available.
		/// </summary>
		string Server { get; }

		/// <summary>
		/// The database this link is connected to, or null if this information is not available.
		/// </summary>
		string Database { get; }

		/// <summary>
		/// The actual connection this instance is using, or null if it is not connected.
		/// </summary>
		IDbConnection DbConnection { get; }

		/// <summary>
		/// Factory method invoked to create an enumerator to execute the given enumerable
		/// command.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>An enumerator able to execute de command.</returns>
		new IEnumerableExecutor CreateEnumerableExecutor(Core.IEnumerableCommand command);

		/// <summary>
		/// Factory method invoked to create an executor to execute the given scalar command.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>An executor able to execute de command.</returns>
		new IScalarExecutor CreateScalarExecutor(Core.IScalarCommand command);
	}
}
// ======================================================== 
