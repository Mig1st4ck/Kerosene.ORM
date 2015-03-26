﻿// ======================================================== ChangedException.cs
namespace Kerosene.ORM.Maps
{
	using Kerosene.Tools;
	using System;

	// ==================================================== 
	/// <summary>
	/// Represents an attempt to update or delete a record that has been changed in the database
	/// per the optimistic concurrency mode.
	/// </summary>
	[Serializable]
	public class ChangedException : Exception
	{
		public ChangedException() { }
		public ChangedException(string message) : base(message) { }
		public ChangedException(string message, Exception inner) : base(message, inner) { }
	}
}
// ======================================================== 
