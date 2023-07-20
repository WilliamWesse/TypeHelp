// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// Nullity.cs
// Portions Copyright © William Edward Wesse
#region Assembly mscorlib, Version=4.0.0.0, Culture=neutral
// Portions Copyright © Microsoft Corporation
// Adapted from: DBNull.cs
// Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\mscorlib.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System;
using System.Security;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;

namespace TypeHelp
{
    /// <summary>
    /// Represents a nulled value. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public sealed class Nullity : IConvertible, ISerializable
    {
        /// <summary>
        /// Represents the sole instance of the TypeHelp.Nullity class.
        /// </summary>
        public static readonly Nullity Value = new ();

        private Nullity()
        {
        }

        private static string get_resource_string()
        {
            return get_resource_string("InvalidCast_FromNullity");
        }

        private static string get_resource_string(string resourceName)
        {
            return TypeParser.RunProps.ResourceString(resourceName);
        }

        private Nullity(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException(get_resource_string("NotSupported_NullitySerial"));
        }

        /// <summary>
        /// Implements the System.Runtime.Serialization.ISerializable interface and returns the data needed to serialize the TypeHelp.Nullity object.
        /// </summary>
        /// <param name="info">A System.Runtime.Serialization.SerializationInfo object containing information required to serialize the TypeHelp.Nullity object.</param>
        /// <param name="context">A System.Runtime.Serialization.StreamingContext object containing the source and destination of the serialized stream associated with the TypeHelp.Nullity object.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <para>info is null.</para>
        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            NullitySerializationHolder.LoadObjectData(info, context);
        }

        /// <summary>
        /// Returns an empty string (System.String.Empty).
        /// </summary>
        /// <returns>
        /// System.String.Empty
        /// </returns>
        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns an empty string using the specified System.IFormatProvider.
        /// </summary>
        /// <param name="provider">The System.IFormatProvider to be used to format the return value. -or- null to obtain the format information from the current locale setting of the operating system.</param>
        /// <returns>
        /// System.String.Empty
        /// </returns>
        public string ToString(IFormatProvider? provider)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the System.TypeCode value for TypeHelp.Nullity.
        /// </summary>
        /// <returns>The System.TypeCode value for TypeHelp.Nullity, which is System.TypeCode.Object.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        /// <summary>
        /// Gets the System.TypeCoex value for TypeHelp.Nullity.
        /// </summary>
        /// <returns>
        /// The TypeHelp.TypeCoex value for TypeHelp.Nullity, which is TypeCoex.Nullity.
        /// </returns>
        public TypeCoex GetTypeCoex()
        {
            return TypeCoex.Nullity;
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        bool IConvertible.ToBoolean(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        char IConvertible.ToChar(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        sbyte IConvertible.ToSByte(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        byte IConvertible.ToByte(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        short IConvertible.ToInt16(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        ushort IConvertible.ToUInt16(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        int IConvertible.ToInt32(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        uint IConvertible.ToUInt32(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        long IConvertible.ToInt64(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        ulong IConvertible.ToUInt64(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        float IConvertible.ToSingle(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        double IConvertible.ToDouble(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        decimal IConvertible.ToDecimal(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// This conversion is not supported. Attempting to make this conversion throws an <see cref="System.InvalidCastException"/>.
        /// </summary>
        /// <param name="provider">An object that implements the System.IFormatProvider interface. (This parameter is not used; specify null.)</param>
        /// <returns>None. The return value for this member is not used.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        DateTime IConvertible.ToDateTime(IFormatProvider? provider)
        {
            throw new InvalidCastException(get_resource_string());
        }

        /// <summary>
        /// Converts the current TypeHelp.Nullity object to the specified type.
        /// </summary>
        /// <param name="type">The type to convert the current TypeHelp.Nullity object to.</param>
        /// <param name="provider">An object that implements the System.IFormatProvider interface and is used to augment the conversion. If null is specified, format information is obtained from the current culture.</param>
        /// <returns>The boxed equivalent of the current TypeHelp.Nullity object, if that conversion is supported; otherwise, an exception is thrown and no value is returned.</returns>
        /// <exception cref="System.FormatException"></exception>
        /// <para>TypeHelp.Nullity.Value is not in a format for the conversionType recognized by the provider.</para>
        /// <exception cref="System.InvalidCastException"></exception>
        /// <para>This conversion is not supported for the TypeHelp.Nullity type.</para>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <para><paramref name="type"/> is null.</para>
        object IConvertible.ToType(Type type, IFormatProvider? provider)
        {
            if (null == provider) {
                provider = CultureInfo.CurrentCulture;
            }
            return Convert.ChangeType(this, type, provider);
        }
    }

    internal sealed class NullityConverter : IFormatterConverter
    {
        #region IFormatterConverter interface implementation

        /// <summary>
        /// Converts the specified value to a given <c>Type</c>.
        /// <para>The following conversions are not supported. Attempting to make any will result in throwing a System.InvalidCastException.</para>
        /// <code>
        ///    ToBoolean, ToByte,   ToChar,   ToDateTime,
        ///    ToDouble,  ToInt16,  ToInt32,  ToInt64,    ToSByte,
        ///    ToSingle,  ToUInt16, ToUInt32, ToUInt64
        /// </code>
        /// </summary>
        /// <param name="value">An object that implements the System.IFormatProvider interface. (This parameteris not used; specify null.)</param>
        /// <param name="type">The type to convert the value to.</param>
        /// <returns>The converted object.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        object IFormatterConverter.Convert(object value, Type type)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        object IFormatterConverter.Convert(object value, TypeCode typeCode)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        bool IFormatterConverter.ToBoolean(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        byte IFormatterConverter.ToByte(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        char IFormatterConverter.ToChar(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        DateTime IFormatterConverter.ToDateTime(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        decimal IFormatterConverter.ToDecimal(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        double IFormatterConverter.ToDouble(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        short IFormatterConverter.ToInt16(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        int IFormatterConverter.ToInt32(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        long IFormatterConverter.ToInt64(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        sbyte IFormatterConverter.ToSByte(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        float IFormatterConverter.ToSingle(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        ushort IFormatterConverter.ToUInt16(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        uint IFormatterConverter.ToUInt32(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        ulong IFormatterConverter.ToUInt64(object value)
        {
            throw new InvalidCastException(BadCastResourceString);
        }

        #region ToString methods

        /// <summary>
        /// Returns an empty string (System.String.Empty).
        /// </summary>
        /// <returns>
        /// System.String.Empty
        /// </returns>
        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns an empty string (System.String.Empty).
        /// </summary>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>
        /// System.String.Empty
        /// </returns>
        public string ToString(object value)
        {
            return string.Empty;
        }

        #endregion ToString methods

        #endregion IFormatterConverter interface implementation

        #region protected fields and properties

        private const string invalid_cast = "InvalidCast_FromNullity";

        private static string BadCastResourceString {
            get {
                return TypeParser.RunProps.ResourceString(invalid_cast);
            }
        }

        #endregion protected fields and properties
    }

    [Serializable]
    internal sealed class NullitySerializationHolder : ISerializable, IObjectReference
    {
        #region constructors

        public NullitySerializationHolder()
        {
            this.serializationInfo = new SerializationInfo(
               typeof(Nullity), new NullityConverter());
            this.streamingContext = GetStreamingContext();
            LoadObjectData(serializationInfo, streamingContext);
        }

        public NullitySerializationHolder(SerializationInfo serializationInfo)
        {
            this.serializationInfo = serializationInfo;
            this.streamingContext = GetStreamingContext();
            LoadObjectData(this.serializationInfo, streamingContext);
        }

        public NullitySerializationHolder(StreamingContext context)
        {
            this.serializationInfo = new SerializationInfo(
               typeof(Nullity), new NullityConverter());
            this.streamingContext = context;
            LoadObjectData(serializationInfo, streamingContext);
        }

        public NullitySerializationHolder(
           SerializationInfo serializationInfo,
           StreamingContext context)
        {
            this.serializationInfo = serializationInfo;
            this.streamingContext = context;
            LoadObjectData(this.serializationInfo, streamingContext);
        }

        #endregion constructors

        #region public properties

        public SerializationInfo SerializationInfo {
            get { return serializationInfo; }
        }

        public StreamingContext StreamingContext {
            get { return streamingContext; }
        }

        #endregion public properties

        #region public methods

        /// <summary>
        /// Implements the System.Runtime.Serialization.ISerializable interface and returns the Data needed to serialize the TypeHelp.Nullity object.
        /// </summary>
        /// <param name="serializationInfo">A System.Runtime.Serialization.SerializationInfo object containing information required to serialize the TypeHelp.Nullity object.</param>
        /// <param name="context">A System.Runtime.Serialization.StreamingContext object containing the source and destination of the serialized stream associated with the TypeHelp.Nullity object.</param>
        /// <exception cref="System.ArgumentNullException"> serializationInfo is null.</exception>
        /// <para>serializationInfo is null.</para>
        [SecurityCritical]
        public void GetObjectData(
           SerializationInfo serializationInfo,
           StreamingContext context)
        {
            LoadObjectData(serializationInfo, context);
        }

        /// <summary>
        /// Gets the real object.
        /// </summary>
        /// <param name="context">A System.Runtime.Serialization.StreamingContext object containing the source and destination of the serialized stream associated with the TypeHelp.Nullity object.</param>
        /// <returns>An object</returns>
        [SecurityCritical]
        public object GetRealObject(StreamingContext context)
        {
            return context.Context ?? Nullity.Value;
        }

        #endregion public methods

        #region public static methods

        /// <summary>
        /// Gets the streaming context.
        /// </summary>
        /// <returns></returns>
        [SecurityCritical]
        public static StreamingContext GetStreamingContext()
        {
            StreamingContext context = new (
               StreamingContextStates.All,
               Nullity.Value);
            return context;
        }

        #endregion public static methods

        #region private methods

        /// <summary>
        /// Loads the object data.
        /// </summary>
        /// <param name="serializationInfo">A System.Runtime.Serialization.SerializationInfo object containing information required to serialize the TypeHelp.Nullity object.</param>
        /// <param name="context">A System.Runtime.Serialization.StreamingContext object containing the source and destination of the serialized stream associated with the TypeHelp.Nullity object.</param>
        [SecurityCritical]
        public static void LoadObjectData(
           SerializationInfo serializationInfo,
           StreamingContext context)
        {
            Type type = typeof(Nullity);
            Assembly? assembly = Assembly.GetAssembly(type);

            serializationInfo.SetType(type);
            serializationInfo.AddValue("FullName", type.FullName, typeof(string));
            serializationInfo.AddValue("Name", type.Name, typeof(string));
            serializationInfo.AddValue("Value", context.Context, type);
            if (null != assembly && null != assembly.FullName) {
                serializationInfo.AddValue("AssemblyName", assembly.FullName, typeof(string));
            }
        }

        #endregion private methods

        #region private fields

        private readonly SerializationInfo serializationInfo;
        private readonly StreamingContext streamingContext;

        #endregion private fields
    }
}
