// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// Slice.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Text;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;

namespace TypeHelp
{
   #region enumerations

   // <list type="table">
   // <listheader><term>Field</term><term>Value</term><description>Description</description></listheader>
   // <item><term>None</term><term>0</term><description>Default: No options. Offset may limit length (locked offset) and data is assumed to be in network byte order (NBO), a.k.a., big-endian.</description></item>
   // <item><term>LittleEndian</term><term>1</term><description>Extract the data with the first as the least-significant byte (little-endian byte order).</description></item>
   // <item><term>LockedLength</term><term>2</term><description>Decrement the offset on length overruns; otherwise, truncate processing (locked offset).</description></item>
   // <item><term>LockUnsigned</term><term>4</term><description>Extract unsigned types from arrays with the hi-order bit set in the most significant byte.</description></item>
   // <item><term>StrictLength</term><term>8</term><description>If length is outside of array bounds, the <see cref="Slice"/> constructor will fail as noted below.</description></item>
   // <item><term>StrictOffset</term><term>16</term><description>If offset is outside of array bounds, the <see cref="Slice"/> constructor will fail as noted below.</description></item>
   // </list>

   /// <summary>
   /// Specifies flag options for <see cref="Slice"/> class constructors to control instance byte access length, position, sign and error processing.
   /// <para>Constructor parameter array boundary violations in conjunction with the <c>StrictLength</c> and <c>StrictOffset</c> options MAY raise a <exception cref="TypeInitializationException"/> with an inner <exception cref="InvalidOperationException"/>.</para>
   /// </summary>
   [Flags, DefaultValue(0)]
   public enum SliceOpts
   {
      /// <summary>
      /// <b>Default</b>: No options. Offset MAY limit length (locked offset) and data is assumed to be in network byte order (NBO), a.k.a., big-endian.
      /// </summary>
      None = 0,
      /// <summary>
      /// Extract the data with the first as least-significant byte (little-endian byte order).
      /// </summary>
      LittleEndian = 1,
      /// <summary>
      /// Decrement the offset on length overruns; otherwise, truncate processing (locked offset).
      /// </summary>
      LockedLength = 2,
      /// <summary>
      /// Extract unsigned types from arrays having the hi-order bit set in the most significant byte.
      /// </summary>
      LockUnsigned = 4,
      /// <summary>
      /// If length is outside of array bounds, the <see cref="Slice"/> constructor will fail as noted in <see cref="SliceOpts"/>.
      /// </summary>
      StrictLength = 8,
      /// <summary>
      /// If offset is outside of array bounds, the <see cref="Slice"/> constructor will fail as noted in <see cref="SliceOpts"/>.
      /// </summary>
      StrictOffset = 16,
   }

   #endregion enumerations

   #region class Slice

   /// <summary>
   /// A class that will extract one of the following data types from an array of bytes:
   /// <code>
   ///  System.Boolean   System.Byte    System.Char
   ///  System.DateTime  System.DBNull  System.Decimal
   ///  System.Double    System.Int16   System.Int32
   ///  System.Int64     System.SByte   System.Single
   ///  System.UInt16    System.UInt32  System.UInt64
   ///  System.Numerics.BigInteger
   ///  System.Numerics.Complex
   ///  TypeHelp.Nullity&#160;
   /// </code>
   /// </summary>
   /// <remarks>Constructor parameter array boundary violations in conjunction with the <see cref="SliceOpts.StrictLength"/> and <see cref="SliceOpts.StrictOffset"/> options MAY raise a <exception cref="TypeInitializationException"/> with an inner <exception cref="InvalidOperationException"/>.</remarks>
   public class Slice
   {
      #region constructors

      /// <summary>
      /// Create a <c>Slice</c> using length, offset and the default options:
      /// <list type="bullet">
      /// <item><description><paramref name="data"/> is in Network Byte Order (NBO), a.k.a., big-endian.</description></item>
      /// <item><description><paramref name="offset"/> is locked, data MAY be truncated.</description></item>
      /// </list>
      /// </summary>
      /// <param name="data">An array of bytes.</param>
      /// <param name="length">The length of the value to extract.</param>
      /// <param name="offset">The position of the value.</param>
      /// <remarks>
      /// If the <see cref="SliceOpts.StrictLength"/> flag is set and the <paramref name="length"/> parameter is out of bounds, or the <see cref="SliceOpts.StrictOffset"/> flag is set and the <paramref name="offset"/> parameter is out of bounds, a <exception cref="TypeInitializationException"/> with an inner <exception cref="InvalidOperationException"/> will be thrown.
      /// </remarks>
      public Slice(byte[] data, int length, int offset)
      {
         this.length = length;
         this.offset = offset;
         Exception? exception = Initialize(data, length, offset);
         if (null != exception) {
            throw exception;
         }
      }

      /// <summary>
      /// Create a <c>Slice</c> using length, offset and the selected <c>SliceOpts</c>.
      /// </summary>
      /// <param name="data">An array of bytes.</param>
      /// <param name="length">The length of the value to extract.</param>
      /// <param name="offset">The position of the value.</param>
      /// <param name="options">A <c>SliceOpts</c> enumeration value.
      /// <para>If the value is <c>null</c>, <see cref="SliceOpts.None"/> will be substituted, resulting in the default options:</para>
      /// <list type="bullet">
      /// <item><description><paramref name="data"/> is in Network Byte Order (NBO), a.k.a., big-endian.</description></item>
      /// <item><description><paramref name="offset"/> is locked, data MAY be truncated.</description></item>
      /// </list>
      /// </param>
      /// <exception cref="InvalidOperationException"></exception>
      /// <remarks>If the <see cref="SliceOpts.StrictLength"/> flag is set and the <paramref name="length"/> parameter is out of bounds, or the <see cref="SliceOpts.StrictOffset"/> flag is set and the <paramref name="offset"/> parameter is out of bounds, a <exception cref="TypeInitializationException"/> with an inner <exception cref="InvalidOperationException"/> will be thrown.</remarks>
      public Slice(byte[] data, int length, int offset, SliceOpts? options)
      {
         this.length = length;
         this.offset = offset;
         this.options = null != options ? (SliceOpts)options : Defaults;
         Exception? exception = Initialize(data, length, offset);
         if (null != exception) {
            throw exception;
         }
      }

      /// <summary>
      /// Create a <c>Slice</c> using the selected <see cref="System.TypeCode"/> <paramref name="typeCode"/>, <paramref name="offset"/> and selected <see cref="SliceOpts"/>.
      /// <para>If the <paramref name="typeCode"/> is <see cref="TypeCode.Object"/>, a <see cref="System.Numerics.BigInteger"/> will be assumed, using the equivalent (<seealso cref="TypeCoex.BigInteger"/>).</para>
      /// </summary>
      /// <param name="data"></param>
      /// <param name="typeCode">For allowable values, see the <see cref="TypeCode"/> Property.</param>
      /// <param name="offset">The position of the value.</param>
      /// <param name="options">A <c>SliceOpts</c> enumeration value.
      /// <para>If the value is <c>null</c>, <see cref="SliceOpts.None"/> will be substituted, resulting in the default options:</para>
      /// <list type="bullet">
      /// <item><description><paramref name="data"/> is in Network Byte Order (NBO), a.k.a., big-endian.</description></item>
      /// <item><description><paramref name="offset"/> is locked, data MAY be truncated.</description></item>
      /// </list>
      /// </param>
      /// <exception cref="InvalidOperationException"></exception>
      /// <remarks>
      /// If the <see cref="SliceOpts.StrictLength"/> flag is set and the <paramref name="typeCode"/> parameter is undefined, the <see cref="TypeCoex"/> Property will be <see cref="TypeCoex.Nullity"/>.
      /// <para>
      /// If the <see cref="SliceOpts.StrictOffset"/> flag is set and the <paramref name="offset"/> parameter is out of bounds, a <exception cref="TypeInitializationException"/> with an inner <exception cref="InvalidOperationException"/> will be thrown.
      /// </para>
      /// </remarks>
      public Slice(
         byte[] data,
         TypeCode typeCode,
         int offset,
         SliceOpts? options)
      {
         length = InitializeLength(data.Length, (int)typeCode);
         this.offset = offset;
         this.options = null != options ? (SliceOpts)options : Defaults;
         Exception? exception = Initialize(data, length, offset);
         if (null != exception) {
            throw exception;
         }
      }

      /// <summary>
      /// Create a <c>Slice</c> using the selected <c>TypeCoex</c>, offset and selected <c>SliceOpts</c>.
      /// </summary>
      /// <param name="data"></param>
      /// <param name="typeCoex">For allowable values, see the <see cref="TypeCoex"/> Property.</param>
      /// <param name="offset">The position of the value.</param>
      /// <param name="options">A <c>SliceOpts</c> enumeration value.
      /// <para>If the value is <c>null</c>, <see cref="SliceOpts.None"/> will be substituted, resulting in the default options:</para>
      /// <list type="bullet">
      /// <item><description><paramref name="data"/> is in Network Byte Order (NBO), a.k.a., big-endian.</description></item>
      /// <item><description><paramref name="offset"/> is locked, data MAY be truncated.</description></item>
      /// </list>
      /// </param>
      /// <exception cref="InvalidOperationException"></exception>
      /// <remarks>If the <see cref="SliceOpts.StrictLength"/> flag is set and the length parameter is out of bounds, or the <see cref="SliceOpts.StrictOffset"/> flag is set and the offset parameter is out of bounds, a <exception cref="TypeInitializationException"/> with an inner <exception cref="InvalidOperationException"/> will be thrown.</remarks>
      public Slice(
         byte[] data,
         TypeCoex typeCoex,
         int offset,
         SliceOpts? options)
      {
         length = InitializeLength(data.Length, (int)typeCoex);
         this.offset = offset;
         this.options = null != options ? (SliceOpts)options : Defaults;
         Exception? exception = Initialize(data, length, offset);
         if (null != exception) {
            throw exception;
         }
      }

      #endregion constructors

      #region public consts

      /// <summary>
      /// The <see cref="SliceOpts"/> <see cref="SliceOpts.None"/> value (0).
      /// </summary>
      public const SliceOpts Defaults = SliceOpts.None;

      /// <summary>
      /// A bitwise OR of the <see cref="SliceOpts"/> <see cref="SliceOpts.LockedLength"/> and <see cref="SliceOpts.LockUnsigned"/> flags.
      /// </summary>
      public const SliceOpts Locked =
         SliceOpts.LockedLength | SliceOpts.LockUnsigned;

      /// <summary>
      /// A bitwise OR of the <see cref="SliceOpts"/> <see cref="SliceOpts.LittleEndian"/>, <see cref="SliceOpts.LockedLength"/> and <see cref="SliceOpts.LockUnsigned"/> flags.
      /// </summary>
      public const SliceOpts LockLittleEndian = SliceOpts.LittleEndian |
         SliceOpts.LockedLength | SliceOpts.LockUnsigned;

      /// <summary>
      /// A bitwise OR of the <see cref="SliceOpts"/> <see cref="SliceOpts.StrictLength"/> and <see cref="SliceOpts.StrictOffset"/> flags.
      /// </summary>
      public const SliceOpts Strict =
         SliceOpts.StrictLength | SliceOpts.StrictOffset;

      #endregion public consts

      #region public properties

      /// <summary>
      /// <c>true</c> if the <see cref="SliceOpts.LockUnsigned"/> flag is not set, otherwise <c>false</c>.
      /// </summary>
      public bool AllowSigned {
         get { return !HasOptions(SliceOpts.LockUnsigned); }
      }

      /// <summary>
      /// A copy of the extracted byte array in big-endian order.
      /// <para>If the <see cref="AllowSigned"/> Property is <c>false</c> and the <see cref="HighBit"/> Property is <c>true</c>, an additional zero-value byte will be present in the high-order (first) byte of the returned big-endian array.</para>
      /// </summary>
      public byte[] Data {
         get {
            byte[] copy = new byte[data.Length];
            if (data.Length > 0) {
               Array.Copy(data, copy, data.Length);
            }
            return copy;
         }
      }

      /// <summary>
      /// The value is <c>true</c> if the first (big-endian normalized) data byte has the high-order bit set, otherwise <c>false</c>.
      /// </summary>
      public bool HighBit {
         get { return data.Length > 0 && (data[0] & 0x80) != 0; }
      }

      /// <summary>
      /// The value is <c>true</c> if the <see cref="Data"/> Property length is zero, otherwise <c>false</c>.
      /// </summary>
      public bool IsEmpty {
         get { return data.Length == 0; }
      }

      /// <summary>
      /// The value is <c>true</c> if <see cref="Length"/> or <seealso cref="Offset"/> were not modified during initialization, else <c>false</c>.
      /// </summary>
      public bool IsExact {
         get { return lengthDelta == 0 && offsetDelta == 0; }
      }

      /// <summary>
      /// The value is <c>true</c> if the <see cref="SliceOpts.LittleEndian"/> flag is set, indicating the <see cref="Data"/> Property byte order is the inverse of that extracted from the data provided to the constructor, otherwise <c>false</c>.
      /// </summary>
      public bool IsLittleEndian {
         get { return HasOptions(SliceOpts.LittleEndian); }
      }

      /// <summary>
      /// The value is<c>true</c> if the <see cref="HighBit"/> Property is true and the <see cref="SliceOpts.LockUnsigned"/> flag is set, otherwise <c>false</c>.
      /// </summary>
      public bool IsSigned {
         get { return AllowSigned && HighBit; }
      }

      /// <summary>
      /// The value is<c>true</c> if the <see cref="HighBit"/> Property is true and the <see cref="SliceOpts.LockUnsigned"/> flag is set, otherwise <c>false</c>.
      /// </summary>
      public bool IsUnsigned {
         get { return AllowSigned && HighBit; }
      }

      /// <summary>
      /// The length of the byte array.
      /// </summary>
      public int Length {
         get { return length; }
      }

      /// <summary>
      /// The change, if any, made to the <see cref="Length"/> during initialization.
      /// </summary>
      public int LengthDelta {
         get { return lengthDelta; }
      }

      /// <summary>
      /// The position of the data in the input array.
      /// </summary>
      public int Offset {
         get { return offset; }
      }

      /// <summary>
      /// The change, if any, made to the <see cref="Offset"/> during initialization.
      /// </summary>
      public int OffsetDelta {
         get { return offsetDelta; }
      }

      /// <summary>
      /// The <c>SliceOpts</c> provided to the constructor.
      /// </summary>
      public SliceOpts Options {
         get { return options; }
      }

      /// <summary>
      /// The <see cref="System.TypeCode"/> of the extracted data. This will be one of the following:
      /// <code>
      ///  TypeCode.Boolean
      ///  TypeCode.Byte     TypeCode.Char     TypeCode.DateTime
      ///  TypeCode.DBNull   TypeCode.Decimal  TypeCode.Double
      ///  TypeCode.Int16    TypeCode.Int32    TypeCode.Int64
      ///  TypeCode.Object   TypeCode.SByte    TypeCode.Single
      ///  TypeCode.UInt16   TypeCode.UInt32   TypeCode.UInt64&#160;
      /// </code>
      /// </summary>
      /// <remarks>
      /// The value is derived as follows:
      /// <code>
      ///    public TypeCode TypeCode {
      ///       get { return Type.GetTypeCode(this.Value.GetType()); }
      ///    }
      /// </code>
      /// </remarks>
      public TypeCode TypeCode {
         get { return Type.GetTypeCode(this.Value.GetType()); }
      }

      /// <summary>
      /// The <see cref="TypeCoex"/> of the extracted value. This will be one of the following:
      /// <code>
      ///  TypeCoex.BigInteger  TypeCoex.Boolean  TypeCoex.Byte
      ///  TypeCoex.Char        TypeCoex.Complex  TypeCoex.DateTime
      ///  TypeCoex.DBNull      TypeCoex.Decimal  TypeCoex.Double
      ///  TypeCoex.Int16       TypeCoex.Int32    TypeCoex.Int64
      ///  TypeCoex.Object      TypeCoex.SByte    TypeCoex.Single
      ///  TypeCoex.UInt16      TypeCoex.UInt32   TypeCoex.UInt64&#160;
      /// </code>
      /// </summary>
      public TypeCoex TypeCoex {
         get { return typeCoex; }
      }

      /// <summary>
      /// The <see cref="System.Type"/> of the extracted value.
      /// </summary>
      public Type TypeOf {
         get { return valueObj.GetType(); }
      }

      /// <summary>
      /// The value will be one of the following types:
      /// <code>
      ///  System.Boolean   System.Byte    System.Char
      ///  System.DateTime  System.DBNull  System.Decimal
      ///  System.Double    System.Int16   System.Int32
      ///  System.Int64     System.SByte   System.Single
      ///  System.UInt16    System.UInt32  System.UInt64
      ///  System.Numerics.BigInteger
      ///  System.Numerics.Complex
      ///  TypeHelp.Nullity&#160;
      ///  </code>
      /// </summary>
      public object Value {
         get { return valueObj; }
      }

      #endregion public properties

      #region public methods

      /// <summary>
      /// Gets A clone of the extracted byte array in big-endian or as presented to the constructor.
      /// </summary>
      /// <param name="original">Controls the order of the returned data.
      /// <para>If <c>true</c> the data is returned in the original order, which is a reverse of the data if the <see cref="IsLittleEndian"/> Property is true; otherwise the data is in big-endian order.</para>
      /// <para>If <c>false</c>, the <see cref="AllowSigned"/> Property is <c>false</c> and the <see cref="HighBit"/> Property is <c>true</c>, an additional zero-length byte will be present in the high-order (first) byte of the returned byte array.</para>
      /// </param>
      /// <returns></returns>
      public byte[] GetData(bool original)
      {
         byte[] source = original && null != orig ? orig : data;
         byte[] clone = new byte[source.Length];
         Array.Copy(source, clone, source.Length);
         return clone;
      }

      /// <summary>
      /// Test whether or not this instance has the specified <c>TypeCode</c> Property length.
      /// </summary>
      /// <param name="typeCode">The <c>TypeCode</c> to test.</param>
      /// <returns><c>true</c> if the <paramref name="typeCode"/> parameter matches the <see cref="TypeCode"/> Property.</returns>
      public bool IsType(TypeCode typeCode)
      {
         return TypeCode == typeCode;
      }

      /// <summary>
      /// Test whether or not this instance has the specified <c>TypeCoex</c> Property length.
      /// </summary>
      /// <param name="typeCoex">The <c>TypeCoex</c> to test.</param>
      /// <returns><c>true</c> if the <paramref name="typeCoex"/> parameter matches the <see cref="TypeCoex"/> Property.</returns>
      public bool IsType(TypeCoex typeCoex)
      {
         return TypeCoex == typeCoex;
      }
      /// <summary>
      /// Test whether or not this instance has a given <c>SliceOpts</c> flag, or flags set.
      /// </summary>
      /// <param name="option">The option flag or flags to check.</param>
      /// <returns><c>true</c> if any flag is set in both Options and the <paramref name="option"/> argument, otherwise <c>false</c>.</returns>
      public bool HasOptions(SliceOpts option)
      {
         return HasOptions(false, option);
      }

      /// <summary>
      /// Test whether or not this instance has the <c>SliceOpts</c> flag, any flag or all flags set.
      /// </summary>
      /// <param name="exact">Specifies whether or not to check all options.</param>
      /// <param name="option">The option flags to check.</param>
      /// <returns><c>true</c> if the required flag or flags are set in both the <see cref="Options"/> Property and the <c>option</c> argument, otherwise <c>false</c>. If exact is true, all flags in the <c>option</c> argument MUST be set in the <c>Options</c> Property.</returns>
      public bool HasOptions(bool exact, SliceOpts option)
      {
         option = options & option;
         return exact
            ? option == options
            : option != 0;
      }

      #endregion public methods

      #region private methods

      /// <summary>
      /// All parameters MUST be as provided to the constructor.
      /// </summary>
      /// <param name="data">An array of bytes.</param>
      /// <param name="length">The number of bytes to extract from the data.</param>
      /// <param name="offset">The starting offset within the data.</param>
      /// <returns><exception cref="InvalidOperationException"> If either the <see cref="SliceOpts.StrictLength"/> flag is set and the length parameter is out of bounds, or the <see cref="SliceOpts.StrictOffset"/> flag is set and the offset parameter is out of bounds, otherwise <c>null</c>.</exception></returns>
      private Exception? Initialize(byte[] data, int length, int offset)
      {
         this.length = Math.Max(length, data.Length);
         this.offset = Math.Max(0, Math.Max(offset, data.Length - 1));

         if (this.length + this.offset > data.Length) {
            if (HasOptions(SliceOpts.LockedLength)) {
               this.offset = Math.Max(0, data.Length - this.length);
            } else {
               this.length = Math.Max(0, data.Length - this.offset);
            }
         }
         lengthDelta = this.length - length;
         offsetDelta = this.offset - offset;
         IntializeData(data);
         return InitializeResult();
      }

      /// <summary>
      /// Copy the selected byte array elements, testing for the <see cref="SliceOpts.LittleEndian"/> option.
      /// </summary>
      /// <param name="bytes">The source byte array.</param>
      private Exception? IntializeData(byte[] bytes)
      {
         Exception? result = null;
         try {
            Array.Resize(ref data, length);
            Array.Copy(bytes, offset, data, 0, length);
            if (length > 1 && IsLittleEndian) {
               Array.Reverse(data);
            }
            InitializeValue();
         } catch (Exception ex) {
            result = ex;
         }
         return result;
      }

      /// <summary>
      /// Initializes the length, using the data length and data.Length value.
      /// </summary>
      /// <param name="size">MUST be the length of the data provided to the constructor.</param>
      /// <param name="coex">The <c>TypeCode</c> or <c>TypeCoex</c>.</param>
      /// <returns>An integer within [0, max(dataLength, data.Length)]</returns>
      private int InitializeLength(int size, int coex)
      {
         typeCoex = Enum.IsDefined(typeof(TypeCoex), coex)
            ? (TypeCoex)coex
            : TypeCoex.Nullity;
         if (typeCoex == TypeCoex.Object) {
            typeCoex = TypeCoex.BigInteger;
         }
         int length = TypeParser.SizeOf(typeCoex, 0);
         length = length > 0
            ? Math.Max(0, size - offset)
            : 0;
         return length;
      }

      /// <summary>
      /// Obtain an <c>Exception</c>, or <c>null</c> if no errors are found.
      /// </summary>
      /// <returns><exception cref="InvalidOperationException"> If either the <see cref="SliceOpts.StrictLength"/> flag is set and the length parameter is out of bounds, or the <see cref="SliceOpts.StrictOffset"/> flag is set and the offset parameter is out of bounds, otherwise <c>null</c>.</exception></returns>
      private Exception? InitializeResult()
      {
         bool[] errors = new bool[3] { false, false, false };
         int hits = 0, i;
         if (!HasOptions(SliceOpts.LockedLength)) {
            bool strict = options.HasFlag(SliceOpts.StrictLength);
            for (i = 0; i < errors.Length; i++) {
               errors[i] = i switch {
                  0 => data.Length == 0 && strict,
                  1 => lengthDelta != 0 && strict,
                  _ => offsetDelta != 0 &&
                        options.HasFlag(SliceOpts.StrictOffset)
               };
               if (errors[i]) hits++;
            }
         }
         if (hits == 0) {
            return null;
         }
         StringBuilder bldr = new (80);
         string andStr = " and";
         string format = " int {0} ({1}) is out of bounds by {2}";
         string prefix = string.Format("Invalid parameter{0}:",
            hits > 1 ? "s" : string.Empty);
         for (i = 0; i < errors.Length; i++) {
            if (!errors[i]) continue;
            switch (i) {
               case 0:
                  bldr.AppendFormat(" byte[] {0} length is 0", nameof(data));
                  break;
               case 1:
                  if (bldr.Length > 0) bldr.Append(hits > 2 ? "," : andStr);
                  bldr.AppendFormat(format, nameof(length),
                     length + lengthDelta, lengthDelta);
                  break;
               case 2:
                  if (bldr.Length > 0) bldr.Append(andStr);
                  bldr.AppendFormat(format, nameof(offset),
                     offset + offsetDelta, offsetDelta);
                  break;
            }
            hits--;
         }
         return new InvalidOperationException(
            string.Format("{0}: {1}.", prefix, bldr.ToString()));
      }

      /// <summary>
      /// Set the length per provided data and signing options, storing the original data in the private <see cref="orig"/> field.
      /// </summary>
      private void InitializeValue()
      {
         if (data.Length == 0) {
            if (IsType(TypeCoex.Nullity)) {
               valueObj = Nullity.Value;
            } else {
               valueObj = DBNull.Value;
            }
            return;
         }
         if (data.Length > sizeof(ulong)) {
            int start = HighBit && !AllowSigned ? 1 : 0;
            int i, j;
            byte[] bytes = new byte[data.Length + start];
            bytes[0] = 0;
            Array.Copy(data, 0, bytes, start, data.Length);
            if (start == 1 || IsLittleEndian) {
               orig = new byte[data.Length];
               Array.Copy(bytes, start, orig, 0, data.Length);
               if (IsLittleEndian) {
                  Array.Reverse(bytes);
               }
            }
            if (IsType(TypeCoex.Nullity)) {
               typeCoex = TypeCoex.BigInteger;
            }
            switch (typeCoex) {
               case TypeCoex.BigInteger: valueObj = new BigInteger(bytes);
                  break;
               case TypeCoex.Complex:
                  List<double> vals = new (0);
                  for (i = 0; i < data.Length; i++) {
                     ulong u = 0;
                     j = 0;
                     while (j++ < sizeof(double) && i < data.Length) {
                        u = (u << 8) | data[i++];
                     }
                     vals.Add((double)u);
                  }
                  while (vals.Count < 2) {
                     vals.Add(0.0);
                  }
                  valueObj = new Complex(vals[0], vals[1]);
                  break;
               case TypeCoex.Decimal:
                  List<int> bits = new (0);
                  for (i = 0; i < data.Length; i++) {
                     int n = 0;
                     j = 0;
                     while (j++ < sizeof(int) && i < data.Length) {
                        n = (n << 8) | data[i++];
                     }
                     bits.Add(n);
                  }
                  while (bits.Count < 4) {
                     bits.Add(0);
                  }
                  bits.Reverse();
                  valueObj = new Decimal(bits.ToArray());
                  break;
            }
            return;
         }
         bool signed = IsSigned;
         switch (typeCoex) {
            case TypeCoex.Char:    signed = false; break;
            case TypeCoex.Single:  signed = false; break;
            case TypeCoex.Double:  signed = false; break;
            case TypeCoex.Boolean: signed = false; break;
         }
         if (!signed) {
            options |= SliceOpts.LockUnsigned;
         }
         // Sign extend if indicated
         object value = signed ? ulong.MaxValue : ulong.MinValue;
         for (int i = 0; i < data.Length; i++) {
            value = ((ulong)value << 8) | data[i];
         }
         if (signed) {
            valueObj = typeCoex switch {
               TypeCoex.DateTime => new DateTime((long)value),
               _ => data.Length switch {
                  4 => (int)value,   3 => (int)value,
                  2 => (short)value, 1 => (sbyte)value,
                  _ => (long)value
               }
            };
         } else {
            valueObj = typeCoex switch {
               TypeCoex.Char    => (char)value,
               TypeCoex.Double  => (double)(long)value,
               TypeCoex.Single  => (float)(int)value,
               TypeCoex.Boolean => (data[0] != 0),
               _ => data.Length switch {
                  4 => (uint)value,   3 => (uint)value,
                  2 => (ushort)value, 1 => (byte)value,
                  _ => (ulong)value
               }
            };
         }
         if (IsType(TypeCoex.Nullity)) {
            typeCoex = TypeParser.GetTypeCoex(TypeOf);
         }
      }

      #endregion private methods

      #region private fields

      private int length;
      private int offset;
      private int lengthDelta = 0;
      private int offsetDelta = 0;
      private byte[] data = Array.Empty<byte>();
      private byte[]? orig = null;
      private object   valueObj = DBNull.Value;
      private TypeCoex typeCoex = TypeCoex.Nullity;
      private SliceOpts options = Defaults;

      #endregion private fields
   }

   #endregion class Slice
}
