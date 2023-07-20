// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// TypeParser_Public.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Numerics;

namespace TypeHelp
{
   public partial class TypeParser
   {
      #region properties

      public bool IsBigInteger {
         get { return this.Parse(typeof(BigInteger)) != null; }
      }

      public bool IsComplex {
         get { return this.Parse(typeof(Complex)) != null; }
      }

      public bool IsDateTime {
         get { return this.Parse(typeof(DateTime)) != null; }
      }

      public bool IsDecimal {
         get { return this.Parse(typeof(Decimal)) != null; }
      }

      public bool IsDouble {
         get { return this.Parse(typeof(double)) != null; }
      }

      public bool IsEmpty {
         get { return this.text.Length == 0; }
      }

      public bool IsEmptyOrWhiteSpace {
         get { return this.IsEmpty || this.IsWhiteSpace; }
      }

      public bool IsInt {
         get { return this.Parse(typeof(int)) != null; }
      }

      public bool IsLong {
         get { return Parse(typeof(long)) != null; }
      }

      public bool IsNumber {
         get { return TypeParser.IsNumeric(this.text); }
      }

      public bool IsWhiteSpace {
         get { return string.IsNullOrWhiteSpace(this.text); }
      }

      public int Length {
         get { return this.text.Length; }
      }

      public string Text {
         get { return this.text; }
         set { this.text = value; }
      }

      #endregion properties

      #region public methods

      public bool CanParse(Type type)
      {
         return this.Parse(type) != null;
      }

      public bool CanParse(TypeCode typeCode)
      {
         return this.Parse(typeCode) != null;
      }

      public BigInteger GetBigInteger(BigInteger defaultValue)
      {
         object? result = this.Parse(typeof(BigInteger));
         return null != result ? (BigInteger)result:  defaultValue;
      }

      public bool GetBigInteger(out BigInteger? value)
      {
         value = (BigInteger?)this.Parse(typeof(BigInteger));
         return null != value;
      }

      public bool GetBigInteger(BigInteger defaultValue, out BigInteger value)
      {
         object? result = this.Parse(typeof(BigInteger));
         value = null != result ? (BigInteger)result:  defaultValue;
         return null != result;
      }

      public bool GetBool(bool defaultValue)
      {
         object? result = this.Parse(typeof(bool));
         return null != result ? (bool)result:  defaultValue;
      }

      public bool GetBool(out bool? value)
      {
         value = (bool?)this.Parse(typeof(bool));
         return null != value;
      }

      public bool GetBool(bool defaultValue, out bool value)
      {
         object? result = this.Parse(typeof(bool));
         value = null != result ? (bool)result:  defaultValue;
         return null != result;
      }

      public byte GetByte(byte defaultValue)
      {
         object? result = this.Parse(typeof(byte));
         return null != result ? (byte)result:  defaultValue;
      }

      public bool GetByte(out byte? value)
      {
         value = (byte?)this.Parse(typeof(byte));
         return null != value;
      }

      public bool GetByte(byte defaultValue, out byte? value)
      {
         object? result = this.Parse(typeof(byte));
         value = null != result ? (byte)result:  defaultValue;
         return null != result;
      }

      public Complex GetComplex(Complex defaultValue)
      {
         object? result = this.Parse(typeof(Complex));
         return null != result ? (Complex)result:  defaultValue;
      }

      public bool GetComplex(out Complex? value)
      {
         value = (Complex?)this.Parse(typeof(Complex));
         return null != value;
      }

      public bool GetComplex(Complex defaultValue, out Complex value)
      {
         object? result = this.Parse(typeof(Complex));
         value = null != result ? (Complex)result:  defaultValue;
         return null != result;
      }

      public DateTime GetDateTime(DateTime defaultValue)
      {
         object? result = this.Parse(typeof(DateTime));
         return null != result ? (DateTime)result:  defaultValue;
      }

      public bool GetDateTime(out DateTime? value)
      {
         value = (DateTime?)this.Parse(typeof(DateTime));
         return null != value;
      }

      public bool GetDateTime(DateTime defaultValue, out DateTime value)
      {
         object? result = this.Parse(typeof(DateTime));
         value = null != result ? (DateTime)result:  defaultValue;
         return null != result;
      }

      public decimal GetDecimal(decimal defaultValue)
      {
         object? result = this.Parse(typeof(decimal));
         return null != result ? (decimal)result:  defaultValue;
      }

      public bool GetDecimal(out decimal? value)
      {
         value = (decimal?)this.Parse(typeof(decimal));
         return null != value;
      }

      public bool GetDecimal(decimal defaultValue, out decimal? value)
      {
         object? result = this.Parse(typeof(decimal));
         value = null != result ? (decimal)result:  defaultValue;
         return null != result;
      }

      public double GetDouble(double defaultValue)
      {
         object? result = this.Parse(typeof(double));
         return null != result ? (double)result:  defaultValue;
      }

      public bool GetDouble(out double? value)
      {
         value = (double?)this.Parse(typeof(double));
         return null != value;
      }

      public bool GetDouble(double defaultValue, out double value)
      {
         object? result = this.Parse(typeof(double));
         value = null != result ? (double)result:  defaultValue;
         return null != result;
      }

      public float GetFloat(float defaultValue)
      {
         object? result = this.Parse(typeof(float));
         return null != result ? (float)result:  defaultValue;
      }

      public bool GetFloat(out float? value)
      {
         value = (float?)this.Parse(typeof(float));
         return null != value;
      }

      public bool GetFloat(float defaultValue, out float value)
      {
         object? result = this.Parse(typeof(float));
         value = null != result ? (float)result:  defaultValue;
         return null != result;
      }

      public int GetInt(int defaultValue)
      {
         object? result = this.Parse(typeof(int));
         return null != result ? (int)result:  defaultValue;
      }

      public bool GetInt(out int? value)
      {
         value = (int?)this.Parse(typeof(int));
         return null != value;
      }

      public bool GetInt(int defaultValue, out int? value)
      {
         object? result = this.Parse(typeof(int));
         value = null != result ? (int)result:  defaultValue;
         return null != result;
      }

      public long GetLong(long defaultValue)
      {
         object? result = this.Parse(typeof(long));
         return null != result ? (long)result:  defaultValue;
      }

      public bool GetLong(out long? value)
      {
         value = (long?)this.Parse(typeof(long));
         return null != value;
      }

      public bool GetLong(long defaultValue, out long? value)
      {
         object? result = this.Parse(typeof(long));
         value = null != result ? (long)result:  defaultValue;
         return null != result;
      }

      public sbyte GetSByte(sbyte defaultValue)
      {
         object? result = this.Parse(typeof(sbyte));
         return null != result ? (sbyte)result:  defaultValue;
      }

      public bool GetSByte(out sbyte? value)
      {
         value = (sbyte?)this.Parse(typeof(sbyte));
         return null != value;
      }

      public bool GetSByte(sbyte defaultValue, out sbyte? value)
      {
         object? result = this.Parse(typeof(sbyte));
         value = null != result ? (sbyte)result:  defaultValue;
         return null != result;
      }

      public short GetShort(short defaultValue)
      {
         object? result = this.Parse(typeof(short));
         return null != result ? (short)result:  defaultValue;
      }

      public bool GetShort(out short? value)
      {
         value = (short?)this.Parse(typeof(short));
         return null != value;
      }

      public bool GetShort(short defaultValue, out short? value)
      {
         object? result = this.Parse(typeof(short));
         value = null != result ? (short)result:  defaultValue;
         return null != result;
      }

      public uint GetUInt(uint defaultValue)
      {
         object? result = this.Parse(typeof(uint));
         return null != result ? (uint)result:  defaultValue;
      }

      public bool GetUInt(out uint? value)
      {
         value = (uint?)this.Parse(typeof(uint));
         return null != value;
      }

      public bool GetUInt(uint defaultValue, out uint? value)
      {
         object? result = this.Parse(typeof(uint));
         value = null != result ? (uint)result:  defaultValue;
         return null != result;
      }

      public ulong GetULong(ulong defaultValue)
      {
         object? result = this.Parse(typeof(ulong));
         return null != result ? (ulong)result:  defaultValue;
      }

      public bool GetULong(out ulong? value)
      {
         value = (ulong?)this.Parse(typeof(ulong));
         return null != value;
      }

      public bool GetULong(ulong defaultValue, out ulong? value)
      {
         object? result = this.Parse(typeof(ulong));
         value = null != result ? (ulong)result:  defaultValue;
         return null != result;
      }

      public short GetUShort(short defaultValue)
      {
         object? result = this.Parse(typeof(short));
         return null != result ? (short)result:  defaultValue;
      }

      public bool GetUShort(out short? value)
      {
         value = (short?)this.Parse(typeof(short));
         return null != value;
      }

      public bool GetUShort(ushort defaultValue, out ushort? value)
      {
         object? result = this.Parse(typeof(ushort));
         value = null != result ? (ushort)result:  defaultValue;
         return null != result;
      }

      public object? Parse(Type type)
      {
         return TypeParser.ParseUsing(type, this.text);
      }

      public bool Parse(Type type, out object? result)
      {
         result = this.Parse(type);
         return null != result;
      }

      public object? Parse(TypeCode typeCode)
      {
         return TypeParser.ParseUsing(typeCode, this.text);
      }

      public bool Parse(TypeCode typeCode, out object? result)
      {
         result = this.Parse(typeCode);
         return null != result;
      }

      #endregion public methods
   }
}
