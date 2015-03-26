﻿// ======================================================== DataEngine.cs
namespace Kerosene.ORM.Core.Concrete
{
	using Kerosene.Tools;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	// ==================================================== 
	/// <summary>
	/// Represents an underlying database engine. Maintains its main characteristics and acts
	/// as a factory to create concrete objects adapted to it.
	/// </summary>
	public partial class DataEngine : IDataEngine
	{
		string _InvariantName = Core.DataEngine.ValidateInvariantName("Generic");
		string _ServerVersion = Core.DataEngine.ValidateServerVersion(null);
		bool _CaseSensitiveNames = false;
		string _ParameterPrefix = Core.DataEngine.ValidateParameterPrefix("#");
		bool _PositionalParameters = false;
		bool _SupportsNativeSkipTake = false;

		bool _IsDisposed = false;
		bool _RelaxTransformers = Core.DataEngine.DEFAULT_RELAX_TRANSFORMERS;
		Dictionary<string, Delegate> _Transformers = new Dictionary<string, Delegate>();

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <remarks>Adds the default transformers for the <see cref="CalendarDate"/> and the
		/// <see cref="ClockTime"/> classes for convenience.</remarks>
		public DataEngine()
		{
			// For convenience registering this types from the core kerosene library...
			AddTransformer<CalendarDate>(x => x.ToDateTime());
			AddTransformer<ClockTime>(x => x.ToString());
		}

		/// <summary>
		/// Whether this instance has been disposed or not.
		/// </summary>
		public bool IsDisposed
		{
			get { return _IsDisposed; }
		}

		/// <summary>
		/// Disposes this instance.
		/// </summary>
		public void Dispose()
		{
			if (!IsDisposed) { OnDispose(true); GC.SuppressFinalize(this); }
		}

		~DataEngine()
		{
			if (!IsDisposed) OnDispose(false);
		}

		/// <summary>
		/// Invoked when disposing or finalizing this instance.
		/// </summary>
		/// <param name="disposing">True if the object is being disposed, false otherwise.</param>
		protected virtual void OnDispose(bool disposing)
		{
			if (disposing)
			{
				if (_Transformers != null) _Transformers.Clear();
			}
			_Transformers = null;

			Core.EngineFactory.RemoveEngine(this);

			_IsDisposed = true;
		}

		/// <summary>
		/// Returns the string representation of this instance.
		/// </summary>
		/// <param name="extended">True to obtain the extended string representation instead of
		/// the standard one.</param>
		/// <returns>A string containing the standard representation of this instance.</returns>
		public string ToString(bool extended)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0}({1}", ToStringType(), _InvariantName);
			if (_ServerVersion != null) sb.AppendFormat(", v:{0}", _ServerVersion);

			if (extended)
			{
				sb.AppendFormat(", CaseSensitive:{0}", CaseSensitiveNames);
				sb.AppendFormat(", Prefix:{0}", ParameterPrefix);
				sb.AppendFormat(", Positional:{0}", PositionalParameters);
				sb.AppendFormat(", NativeSkipTake:{0}", SupportsNativeSkipTake);
			}

			sb.Append(")");

			var str = sb.ToString();
			return IsDisposed ? string.Format("disposed::{0}", str) : str;
		}

		/// <summary>
		/// Invoked to obtain a string with identification of this string for representation
		/// purposes.
		/// </summary>
		protected virtual string ToStringType()
		{
			return GetType().EasyName();
		}

		/// <summary>
		/// Returns the string representation of this instance.
		/// </summary>
		/// <returns>A string containing the standard representation of this instance.</returns>
		public override string ToString()
		{
			return ToString(extended: false);
		}

		/// <summary>
		/// Returns a new instance that is a copy of the original one.
		/// </summary>
		/// <returns>A new instance.</returns>
		public DataEngine Clone()
		{
			var cloned = new DataEngine();
			OnClone(cloned, null); return cloned;
		}
		IDataEngine IDataEngine.Clone()
		{
			return this.Clone();
		}
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Invoked when cloning this object to set its state at this point of the inheritance
		/// chain.
		/// </summary>
		/// <param name="cloned">The cloned object.</param>
		protected virtual void OnClone(object cloned, IDictionary<string, object> settings)
		{
			if (IsDisposed) throw new ObjectDisposedException(this.ToString());
			var temp = cloned as DataEngine;
			if (cloned == null) throw new InvalidCastException(
				"Cloned instance '{0}' is not a valid '{1}' one."
				.FormatWith(cloned.Sketch(), typeof(DataEngine).EasyName()));

			temp._InvariantName = _InvariantName;
			temp._ServerVersion = _ServerVersion;
			temp._CaseSensitiveNames = _CaseSensitiveNames;
			temp._ParameterPrefix = _ParameterPrefix;
			temp._PositionalParameters = _PositionalParameters;
			temp._SupportsNativeSkipTake = _SupportsNativeSkipTake;

			if (settings != null)
			{
				foreach (var kvp in settings)
				{
					if (kvp.Key == ElementInfo.ParseName<IDataEngine>(x => x.InvariantName)) { temp._InvariantName = Core.DataEngine.ValidateInvariantName((string)kvp.Value); continue; }
					if (kvp.Key == ElementInfo.ParseName<IDataEngine>(x => x.ServerVersion)) { temp._ServerVersion = Core.DataEngine.ValidateServerVersion((string)kvp.Value); continue; }
					if (kvp.Key == ElementInfo.ParseName<IDataEngine>(x => x.CaseSensitiveNames)) { temp._CaseSensitiveNames = (bool)kvp.Value; continue; }
					if (kvp.Key == ElementInfo.ParseName<IDataEngine>(x => x.ParameterPrefix)) { temp._ParameterPrefix = Core.DataEngine.ValidateParameterPrefix((string)kvp.Value); continue; }
					if (kvp.Key == ElementInfo.ParseName<IDataEngine>(x => x.PositionalParameters)) { temp._PositionalParameters = (bool)kvp.Value; continue; }
					if (kvp.Key == ElementInfo.ParseName<IDataEngine>(x => x.SupportsNativeSkipTake)) { temp._SupportsNativeSkipTake = (bool)kvp.Value; continue; }
				}
			}

			foreach (var kvp in _Transformers)
			{
				if (!temp._Transformers.ContainsKey(kvp.Key)) temp._Transformers.Add(kvp.Key, kvp.Value);
			}
		}

		/// <summary>
		/// Returns a new instance that is a copy of the original one.
		/// </summary>
		/// <param name="settings">A dictionary containing the names and values of the properties
		/// that has to be changed with respect to the original ones, or null if these changes
		/// are not needed.
		/// <returns>A new instance.</returns>
		public DataEngine Clone(IDictionary<string, object> settings)
		{
			var cloned = new DataEngine();
			OnClone(cloned, settings); return cloned;
		}
		IDataEngine IDataEngine.Clone(IDictionary<string, object> settings)
		{
			return this.Clone(settings);
		}

		/// <summary>
		/// The engine invariant name. The value of this property typically corresponds to the
		/// ADO.NET provider's invariant name, if such is used, but this correspondence is not
		/// mandatory.
		/// </summary>
		/// <remarks>There might be several instances registered sharing the same invariant name.
		/// In this case resolution can be forced using the min and max version arguments.</remarks>
		public string InvariantName
		{
			get { return _InvariantName; }
			protected set { _InvariantName = Core.DataEngine.ValidateInvariantName(value); }
		}

		/// <summary>
		/// The version specification the database engine identifies itself to be, or null if
		/// this information is not available.
		/// </summary>
		public string ServerVersion
		{
			get { return _ServerVersion; }
			protected set { _ServerVersion = Core.DataEngine.ValidateServerVersion(value); }
		}

		/// <summary>
		/// Whether the identifiers in the database, as table and column names, are case sensitive
		/// or not.
		/// </summary>
		public bool CaseSensitiveNames
		{
			get { return _CaseSensitiveNames; }
			protected set { _CaseSensitiveNames = value; }
		}

		/// <summary>
		/// The default prefix the database engine uses for the names of the parameters of its
		/// command.
		/// </summary>
		public string ParameterPrefix
		{
			get { return _ParameterPrefix; }
			protected set { _ParameterPrefix = Core.DataEngine.ValidateParameterPrefix(value); }
		}

		/// <summary>
		/// Whether the database engine treats the parameters of a command by position instead
		/// of by name.
		/// </summary>
		public bool PositionalParameters
		{
			get { return _PositionalParameters; }
			protected set { _PositionalParameters = value; }
		}

		/// <summary>
		/// Whether the database engine provides a normalized syntax to implement the skip/take
		/// functionality, or rather it has to be emulated by software.
		/// </summary>
		public bool SupportsNativeSkipTake
		{
			get { return _SupportsNativeSkipTake; }
			protected set { _SupportsNativeSkipTake = value; }
		}

		/// <summary>
		/// Factory method to create a new parser adapted to this instance.
		/// </summary>
		/// <returns>A new parser.</returns>
		public IParser CreateParser()
		{
			return new Parser(this);
		}

		/// <summary>
		/// Factory method to create a new collection of parameters adapted to this instance.
		/// </summary>
		/// <returns>A new collection of parameters.</returns>
		public IParameterCollection CreateParameterCollection()
		{
			return new ParameterCollection(CaseSensitiveNames, ParameterPrefix);
		}

		/// <summary>
		/// Factory method to create a new collection of element aliases adapted to this instance.
		/// </summary>
		/// <returns>A new collection of element aliases.</returns>
		public IElementAliasCollection CreateElementAliasCollection()
		{
			return new ElementAliasCollection(CaseSensitiveNames);
		}

		/// <summary>
		/// Creates a new raw command adapted to this engine.
		/// </summary>
		/// <param name="link">The link associated with the new command.</param>
		/// <returns>The new command.</returns>
		public IRawCommand CreateRawCommand(IDataLink link)
		{
			return new RawCommand(link);
		}

		/// <summary>
		/// Creates a new query command adapted to this engine.
		/// </summary>
		/// <param name="link">The link associated with the new command.</param>
		/// <returns>The new command.</returns>
		public IQueryCommand CreateQueryCommand(IDataLink link)
		{
			return new QueryCommand(link);
		}

		/// <summary>
		/// Creates a new insert command adapted to this engine.
		/// </summary>
		/// <param name="link">The link associated with the new command.</param>
		/// <param name="table">The table affected by this command.</param>
		/// <returns>The new command.</returns>
		public IInsertCommand CreateInsertCommand(IDataLink link, Func<dynamic, object> table)
		{
			return new InsertCommand(link, table);
		}

		/// <summary>
		/// Creates a new delete command adapted to this engine.
		/// </summary>
		/// <param name="link">The link associated with the new command.</param>
		/// <param name="table">The table affected by this command.</param>
		/// <returns>The new command.</returns>
		public IDeleteCommand CreateDeleteCommand(IDataLink link, Func<dynamic, object> table)
		{
			return new DeleteCommand(link, table);
		}

		/// <summary>
		/// Creates a new update command adapted to this engine.
		/// </summary>
		/// <param name="link">The link associated with the new command.</param>
		/// <param name="table">The table affected by this command.</param>
		/// <returns>The new command.</returns>
		public IUpdateCommand CreateUpdateCommand(IDataLink link, Func<dynamic, object> table)
		{
			return new UpdateCommand(link, table);
		}

		/// <summary>
		/// Registers into this instance a new transformer for the given type, that will be
		/// invoked when objects of that type have to be converted into objects that the
		/// underlying database engine can understand.
		/// </summary>
		/// <typeparam name="T">The type of the source values to be converted.</typeparam>
		/// <param name="func">The delegate to invoke to convert the values of its type into
		/// whatever objects the underlying database can understand.</param>
		public void AddTransformer<T>(Func<T, object> func)
		{
			if (IsDisposed) throw new ObjectDisposedException(this.ToString());

			var type = typeof(T);
			var name = type.FullName;

			if (_Transformers.ContainsKey(name)) throw new DuplicateException(
				 "A transformer for type '{0}' is already registered.".FormatWith(type.EasyName()));

			_Transformers.Add(name, func);
		}

		/// <summary>
		/// Removes the transformer that may have been registered into this instance for the
		/// given type. Returns true if it has been removed, or false otherwise.
		/// </summary>
		/// <typeparam name="T">The type of the values of the transformer to remove.</typeparam>
		/// <returns>True if the transformer has been removed, or false otherwise</returns>
		public bool RemoveTransformer<T>()
		{
			if (IsDisposed) throw new ObjectDisposedException(this.ToString());

			var type = typeof(T);
			var name = type.FullName;

			return _Transformers.Remove(name);
		}

		/// <summary>
		/// Clears all the transformers that may have been registered into this instance.
		/// </summary>
		public void ClearTransformers()
		{
			if (IsDisposed) throw new ObjectDisposedException(this.ToString());
			_Transformers.Clear();
		}

		/// <summary>
		/// Gets a collection with the types for which transformers have been registered into
		/// this instance.
		/// </summary>
		/// <returns>A collection with the registered types.</returns>
		public IEnumerable<Type> GetTransformerTypes()
		{
			if (IsDisposed) throw new ObjectDisposedException(this.ToString());

			return _Transformers.Select(x => Type.GetType(x.Key));
		}

		/// <summary>
		/// Gets whether a transformer for the given type of values is already registered into
		/// this instance or not.
		/// </summary>
		/// <param name="type">The type of the values of the transformer.</param>
		/// <returns>True if a transformer is registered, false otherwise.</returns>
		public bool IsTransformerRegistered(Type type)
		{
			if (IsDisposed) throw new ObjectDisposedException(this.ToString());
			if (type == null) throw new ArgumentNullException("type", "Type cannot be null.");

			var name = type.FullName;

			return _Transformers.ContainsKey(name);
		}

		/// <summary>
		/// Gets whether a transformer for the given type of values is already registered into
		/// this instance or not.
		/// </summary>
		/// <typeparam name="T">The type of the values of the transformer.</typeparam>
		/// <returns>True if a transformer is registered, false otherwise.</returns>
		public bool IsTransformerRegistered<T>()
		{
			return IsTransformerRegistered(typeof(T));
		}

		/// <summary>
		/// Whether if a value transformer for a given type is not found a value transformer
		/// for a base type can be used, or not.
		/// </summary>
		public bool RelaxTransformers
		{
			get { return _RelaxTransformers; }
			set { _RelaxTransformers = value; }
		}

		/// <summary>
		/// Returns a value that is either the product of the conversion performed by a registered
		/// transformer for its type, or the original value.
		/// </summary>
		/// <param name="value">The value to tranform.</param>
		/// <returns>The value transformed, or the original one.</returns>
		public object TryTransform(object value)
		{
			if (IsDisposed) throw new ObjectDisposedException(this.ToString());

			if (value == null) return null;

			Delegate func = null;
			Type type = value.GetType();
			do
			{
				var name = type.FullName; if (_Transformers.TryGetValue(name, out func))
				{
					try { value = func.DynamicInvoke(new object[] { value }); }
					catch { }
					break;
				}

				if (!RelaxTransformers) break;
				type = type.BaseType;
			}
			while (type != null);

			return value;
		}
	}
}
// ======================================================== 
