﻿// ======================================================== OracleEngine.cs
namespace Kerosene.ORM.Direct.Concrete
{
	using Kerosene.Tools;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data.Common;
	using System.Linq;
	using System.Text;

	// ==================================================== 
	/// <summary>
	/// Represents a generic ORACLE database in a direct connection scenario.
	/// </summary>
	public class OracleEngine : Core.Concrete.OracleEngine, IDataEngine
	{
		/// <summary>
		/// Initializes a new engine.
		/// </summary>
		public OracleEngine() : base() { }

		/// <summary>
		/// Invoked to obtain a string with identification of this string for representation
		/// purposes.
		/// </summary>
		protected override string ToStringType()
		{
			return "Direct" + base.ToStringType();
		}

		/// <summary>
		/// Returns a new instance that is a copy of the original one.
		/// </summary>
		/// <returns>A new instance.</returns>
		public new OracleEngine Clone()
		{
			var cloned = new OracleEngine();
			OnClone(cloned, null); return cloned;
		}
		IDataEngine IDataEngine.Clone()
		{
			return this.Clone();
		}
		Core.IDataEngine Core.IDataEngine.Clone()
		{
			return this.Clone();
		}
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Returns a new instance that is a copy of the original one.
		/// </summary>
		/// <param name="settings">A dictionary containing the names of the properties whose
		/// values are to be changed with respect to the original instance, or null to not
		/// modify any of those.</param>
		/// <returns>A new instance.</returns>
		public new OracleEngine Clone(IDictionary<string, object> settings)
		{
			var cloned = new OracleEngine();
			OnClone(cloned, settings); return cloned;
		}
		IDataEngine IDataEngine.Clone(IDictionary<string, object> settings)
		{
			return this.Clone(settings);
		}
		Core.IDataEngine Core.IDataEngine.Clone(IDictionary<string, object> settings)
		{
			return this.Clone(settings);
		}

		/// <summary>
		/// Gets the provider factory associated with this engine.
		/// </summary>
		public DbProviderFactory ProviderFactory
		{
			get { return DbProviderFactories.GetFactory(InvariantName); }
		}
	}
}
// ======================================================== 
