// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// TypeParser_Public_Static.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TypeHelp
{
    public partial class TypeParser
    {
        #region properties

        public static readonly RuntimeProps RunProps = new (true);

        #endregion properties

        #region methods

        #region Get[Type,Code,Coex] methods

        public static TCF GetCodeFlag(TypeCode typeCode)
        {
            TCF flags = (TCF)(1 << (int)typeCode);
            return flags;
        }

        public static TCF GetCoexFlag(TypeCoex typeCoex)
        {
            TCF flags = (TCF)(1 << (int)typeCoex);
            return flags;
        }

        public static Type? GetType(object? item)
        {
            if (null != item) {
                try {
                    if (IsType(item)) {
                        return (Type)item;
                    }
                    return item.GetType();
                }
                catch { }
            }
            return null;
        }

        public static Type? GetType(TypeCode typeCode)
        {
            Type? type = IsValid(typeCode)
               ? GetType((TypeCoex)typeCode)
               : null;
            return type;
        }

        public static bool GetType(TypeCode typeCode, out Type? type)
        {
            return GetType((TypeCoex)typeCode, out type);
        }

        public static Type? GetType(TypeCoex typeCoex)
        {
            Type? type = IsValid(typeCoex)
               ? KnownTypeList[(int)typeCoex]
               : null;
            return type;
        }

        public static bool GetType(TypeCoex typeCoex, out Type? type)
        {
            type = GetType((TypeCoex)typeCoex);
            return type != null;
        }

        public static TypeCode GetTypeCode(TypeCoex typeCoex)
        {
            if (IsValid((TypeCode)typeCoex)) {
                return (TypeCode)typeCoex;
            }
            return Enum.IsDefined(typeof(TypeCode), typeCoex)
               ? (TypeCode)typeCoex
               : TypeCode.Empty;
        }

        public static TypeCode GetTypeCode(object? obj)
        {
            TypeCode result = TypeCode.Empty;
            if (null != obj) {
                try {
                    Type type = obj.GetType();
                    result = Type.GetTypeCode(type);
                }
                catch { }
            }
            return result;
        }

        public static TypeCode GetTypeCode(TCF flags)
        {
            ulong bits = (ulong)(flags & TCF.TypeCodes);
            int result = bits == 0 ? 0 : 1;

            while (bits != 0) {
                if ((bits & 1) == 1) {
                    break;
                }
                bits >>= 1;
                result++;
            }
            return (TypeCode)result;
        }

        public static TypeCoex GetTypeCoex(TypeCode value)
        {
            return Enum.IsDefined(typeof(TypeCoex), value)
               ? (TypeCoex)value
               : TypeCoex.Empty;
        }

        /// <summary>
        /// Get the <c>TypeCoex</c> for an <c>object</c>.
        /// </summary>
        /// <param name="value">The <c>object</c> to test, which may be null. <see cref="TypeCode.Empty"/>.<para><seealso cref="TypeCoex.Empty"/></para>.</param>
        /// <returns></returns>
        public static TypeCoex GetTypeCoex(object? value)
        {
            TypeCoex result = TypeCoex.Empty;
            if (null != value) {
                try {
                    result = GetTypeCoex(value.GetType());
                }
                catch {
                    result = TypeCoex.Nullity;
                }
            }
            return result;
        }

        public static TypeCoex GetTypeCoex(TCF value)
        {
            ulong coex = (ulong)(value & TCF.TypeCoexs);
            if (coex != 0) {
                TypeCoex result = TypeCoex.Empty;
                ulong match = (int)TCF.Empty;

                while (match <= (ulong)TCF.MaxCoex) {
                    if ((match & coex) != 0) {
                        return result;
                    }
                    match <<= 1;
                    result++;
                }
            }
            return TypeCoex.Nullity;
        }

        public static TypeCoex GetTypeCoex(Type? type)
        {
            TypeCoex result = (TypeCoex)Type.GetTypeCode(type);
            if (null != type &&
               result == TypeCoex.Object &&
               Enum.IsDefined(typeof(TypeCoex), type.Name)) {
                try {
                    result = (TypeCoex)Enum.Parse(typeof(TypeCoex), type.Name);
                }
                catch {
                    result = TypeCoex.Nullity;
                }
            }
            return result;
        }

        #endregion Get[Type,Code,Coex] methods

        #region Is* methods

        public static bool IsEnumType(object? obj)
        {
            bool result = false;
            try {
                result = IsType(obj) && ((Type)obj!).IsEnum;
            }
            catch { }
            return result;
        }

        public static bool IsInteger(TypeCode typeCode)
        {
            return IsInteger((TypeCoex)typeCode);
        }

        public static bool IsInteger(TypeCoex typeCoex)
        {
            TCF tcf = TypeParser.GetCoexFlag(typeCoex);
            bool result = TCF.Integers.HasFlag(tcf);
            return result;
        }

        public static bool IsIntegerOrExt(TypeCoex typeCoex)
        {
            TCF tcf = TypeParser.GetCoexFlag(typeCoex);
            bool result = TCF.IntegersExt.HasFlag(tcf);
            return result;
        }

        public static bool IsNatural(TypeCode typeCode)
        {
            return IsNatural((TypeCoex)typeCode);
        }

        public static bool IsNatural(TypeCoex typeCoex)
        {
            TCF tcf = TypeParser.GetCoexFlag(typeCoex);
            bool result = TCF.Naturals.HasFlag(tcf);
            return result;
        }

        public static bool IsType(object? obj)
        {
            try {
                return null != obj && obj is System.Type;
            }
            catch { }
            return false;
        }

        public static bool IsNumeric(string text)
        {
            TypeCode typeCode = ParseToTypeCode(text);
            return typeCode != TypeCode.Empty;
        }

        public static bool IsNumeric(bool allowPrecision, string text)
        {
            object? value = ParseToValue(text);
            if (null == value) {
                return false;
            }
            Type? type = GetType(value);
            if (null == type) {
                return false;
            }
            TypeCoex typeCoex = GetTypeCoex(type);
            return IsNumeric(allowPrecision, typeCoex);
        }

        public static bool IsNumeric(TypeCode typeCode, string text)
        {
            if (typeCode == TypeCode.Empty) {
                return false;
            }
            TypeCode parsedCode = ParseToTypeCode(text);
            return IsValid(parsedCode) && parsedCode == typeCode;
        }

        public static bool IsNumeric(TypeCode typeCode)
        {
            return IsNumeric((TypeCoex)typeCode);
        }

        public static bool IsNumeric(bool allowPrecision, TypeCode typeCode)
        {
            return IsNumeric(allowPrecision, (TypeCoex)typeCode);
        }

        public static bool IsNumeric(TypeCoex typeCoex)
        {
            bool result = IsNumeric(true, typeCoex);
            return result;
        }

        public static bool IsNumeric(bool allowPrecision, TypeCoex typeCoex)
        {
            bool result = (TCF.Numerics & GetCoexFlag(typeCoex)) != 0;
            switch (typeCoex) {
                case TypeCoex.Single: break;
                case TypeCoex.Double: break;
                case TypeCoex.Decimal: break;
                case TypeCoex.Complex: break;
                default: return result;
            }
            return result && allowPrecision;
        }

        public static bool IsPrecision(TypeCoex typeCoex)
        {
            TCF coexflag = TypeParser.GetCoexFlag(typeCoex);
            bool result = TCF.Precisions.HasFlag(coexflag);
            return result;
        }

        public static bool IsValid(string text)
        {
            return IsValid(false, false, text);
        }

        public static bool IsValid(object? value)
        {
            return null != value &&
               IsValid(GetType(value));
        }

        public static bool IsValid(RTProp prop)
        {
            return Enum.IsDefined(typeof(RTProp), prop);
        }

        public static bool IsValid(Type? type)
        {
            return IsValid(GetTypeCoex(type));
        }

        public static bool IsValid(Type enumType, int value)
        {
            return null != enumType && enumType.IsEnum &&
               Enum.IsDefined(enumType, value);
        }

        public static bool IsValid(TypeCode typeCode)
        {
            return Enum.IsDefined(typeof(TypeCode), typeCode);
        }

        public static bool IsValid(TypeCode typeCode, object? item)
        {
            return typeCode == GetTypeCode(item);
        }

        public static bool IsValid(bool isNullish, TypeCode typeCode)
        {
            return IsValid(typeCode) &&
               isNullish == (typeCode == TypeCode.Empty);
        }

        public static bool IsValid(TypeCoex typeCoex)
        {
            return Enum.IsDefined(typeof(TypeCoex), typeCoex);
        }

        public static bool IsValid(TypeCoex typeCoex, object? item)
        {
            return typeCoex == GetTypeCoex(item);
        }

        public static bool IsValid(bool isNullish, TypeCoex typeCoex)
        {
            return IsValid(typeCoex) && isNullish == (
               typeCoex == TypeCoex.Empty ||
               typeCoex == TypeCoex.Nullity);
        }

        public static bool IsValid(bool allowWhitespace, string text)
        {
            return IsValid(false, allowWhitespace, text);
        }

        public static bool IsValid(
           bool allowEmpty,
           bool allowWhitespace,
           string text)
        {
            if (null == text) return false;
            if (text.Length == 0) return allowEmpty;
            return !string.IsNullOrWhiteSpace(text) || allowWhitespace;
        }

        #endregion Is* methods

        #region Pad, Parse and Pointer methods

        #region Pad methods

        public static long PadMod(int value, int modulus)
        {
            return PadMod((long)value, modulus);
        }

        public static long PadMod(long value, int modulus)
        {
            if (modulus == 0 || value == 0) {
                return value;
            }
            bool negate = value < 0;

            if (negate) {
                value = Math.Abs(value);
            }
            if (modulus < 0) {
                modulus = Math.Abs(modulus);
            }
            long result = value;

            if (result > modulus) {
                long pad = modulus - (modulus - (value % modulus));
                result = (long)Math.Min(long.MaxValue, (decimal)value + pad);
            }
            return negate ? 0 - result : result;
        }

        #endregion Pad methods

        #region Parse methods

        public static Complex? ParseToComplex(bool allowEngJsuffix, string text)
        {
            try {
                int count = 0;
                text = text.Trim();
                if (text.IndexOf('(') == 0) {
                    count++;
                    text = text[1..];
                }
                if (text.IndexOf(')') == text.Length - 1) {
                    count++;
                    text = text[..^1];
                }
                int i = count > 0 && text.IndexOfAny(Splits[1]) > 0
                   ? 1
                   : 0;
                string[] tokens = text.Split(Splits[i],
                   SplitOptContentfulOnly);

                if (tokens.Length == 1 && i > 0) {
                    tokens = text.Split(Splits[0],
                       SplitOptContentfulOnly);
                }
                double[] doubles = new double[2] {
               double.NaN, double.NaN
            };
                for (i = 0; i < Math.Min(tokens.Length, 2); i++) {
                    string token = tokens[i].Replace(" ", "");
                    char ch = token[^1];
                    bool im = ch == 'i' || (allowEngJsuffix && ch == 'j');
                    if (im) {
                        token = token.Remove(token.Length - 1);
                    }
                    if (!double.TryParse(token, out doubles[im ? 1 : 0])) {
                        return null;
                    }
                    count++;
                }
                if (count > 0 && count < doubles.Length) {
                    for (i = 0; i < doubles.Length; i++) {
                        if (double.IsNaN(doubles[i])) {
                            doubles[i] = 0.0;
                        }
                    }
                    return new Complex(doubles[0], doubles[1]);
                }
            }
            catch { }
            return null;
        }

        public static Type? ParseToType(string text)
        {
            object? value = ParseToValue(text);
            Type? type = value?.GetType();
            return type;
        }

        public static TypeCode ParseToTypeCode(string text)
        {
            Type? type = ParseToType(text);
            TypeCode result = Type.GetTypeCode(type);
            return result;
        }

        public static object? ParseToValue(string text)
        {
            return ParseToValue(TCF.Everything, text);
        }

        public static object? ParseToValue(TCF filter, string text)
        {
            if (!IsValid(text)) {
                return null;
            }
            List<TypeCoex> list = filter == TCF.None
               ? TypeCoexNumericPriority
               : new List<TypeCoex>();
            if (list.Count == 0) {
                foreach (TypeCoex tc in Enum.GetValues(typeof(TypeCoex))) {
                    list.Add(tc);
                }
            }
            bool keep = !filter.HasFlag(TCF.MatchDiscard);
            for (int i = 0; i < list.Count; i++) {
                TypeCoex typeCoex = list[i];
                TCF flags = (TCF)(1 << (int)typeCoex);
                if (keep != filter.HasFlag(flags)) {
                    continue;
                }
                try {
                    object? value = null;
                    switch (list[i]) {
                        case TypeCoex.Empty: value = null; break;
                        case TypeCoex.Byte: value = byte.Parse(text); break;
                        case TypeCoex.SByte: value = sbyte.Parse(text); break;
                        case TypeCoex.Int16: value = short.Parse(text); break;
                        case TypeCoex.UInt16: value = ushort.Parse(text); break;
                        case TypeCoex.IntPtr:
                            value = RunProps.Features.Is64BitSys
                               ? long.Parse(text)
                               : value = int.Parse(text);
                            break;
                        case TypeCoex.UIntPtr:
                            value = RunProps.Features.Is64BitSys
                               ? ulong.Parse(text)
                               : uint.Parse(text);
                            break;
                        case TypeCoex.Int32: value = int.Parse(text); break;
                        case TypeCoex.UInt32: value = uint.Parse(text); break;
                        case TypeCoex.Int64: value = long.Parse(text); break;
                        case TypeCoex.UInt64: value = ulong.Parse(text); break;
                        case TypeCoex.DateTime: value = DateTime.Parse(text); break;
                        case TypeCoex.BigInteger:
                            if (BigInteger.TryParse(text, out BigInteger bigText)) {
                                value = bigText;
                            }
                            break;
                        case TypeCoex.Single: value = float.Parse(text); break;
                        case TypeCoex.Double: value = double.Parse(text); break;
                        case TypeCoex.Decimal: value = decimal.Parse(text); break;
                        case TypeCoex.Complex:
                            value = ParseToComplex(
                            filter.HasFlag(TCF.MatchAltFormats), text);
                            break;
                        case TypeCoex.Object:
                            if (BigInteger.TryParse(text, out BigInteger bigObject)) {
                                value = bigObject;
                            } else {
                                value = ParseToComplex(true, text);
                            }
                            break;
                        case TypeCoex.String: value = text; break;
                        case TypeCoex.Boolean: value = bool.Parse(text); break;
                        case TypeCoex.Char:
                            value = text.Length < 2 ? text : text[0].ToString();
                            break;
                        case TypeCoex.DBNull: value = DBNull.Value; break;
                    }
                    if (null != value) {
                        return value;
                    }
                }
                catch { }
            }
            return null;
        }

        public static object? ParseUsing(Type type, string text)
        {
            if (type == typeof(IntPtr)) {
                type = Environment.Is64BitProcess
                   ? typeof(long)
                   : typeof(int);
            } else if (type == typeof(UIntPtr)) {
                type = Environment.Is64BitProcess
                   ? typeof(ulong)
                   : typeof(uint);
            }
            return ParseUsing(Type.GetTypeCode(type), text);
        }

        public static object? ParseUsing(TypeCode typeCode, string text)
        {
            object? result = null;
            if (!(IsValid(typeCode) && IsValid(true, true, text))) {
                return result;
            }
            text = text.Trim();
            if (text.Length == 0) {
                return typeCode == TypeCode.String
                   ? text
                   : result;
            }
            try {
                switch (typeCode) {
                    case TypeCode.Int32: result = int.Parse(text); break;
                    case TypeCode.Int64: result = long.Parse(text); break;
                    case TypeCode.UInt32: result = uint.Parse(text); break;
                    case TypeCode.UInt64: result = ulong.Parse(text); break;
                    case TypeCode.Double: result = double.Parse(text); break;
                    case TypeCode.Byte: result = byte.Parse(text); break;
                    case TypeCode.Int16: result = short.Parse(text); break;
                    case TypeCode.SByte: result = sbyte.Parse(text); break;
                    case TypeCode.Single: result = float.Parse(text); break;
                    case TypeCode.UInt16: result = ushort.Parse(text); break;
                    case TypeCode.Decimal: result = decimal.Parse(text); break;
                    case TypeCode.DateTime: result = DateTime.Parse(text); break;
                    case TypeCode.Boolean: result = bool.Parse(text); break;
                    case TypeCode.Char: result = char.Parse(text); break;
                    case TypeCode.String: result = text; break;
                }
            }
            catch {
                if (typeCode == TypeCode.Char) {
                    result = text[0];
                }
            }
            return result;
        }

        #endregion Parse methods

        #region Pointer methods

        public static int PointerBaseSize(bool? signed)
        {
            Type type = PointerBaseType(signed);
            int size = SizeOf(type);
            return size;
        }

        public static Type PointerBaseType(bool? signed)
        {
            return PointerBaseType(signed, false);
        }

        public static Type PointerBaseType(bool? signed, bool? system)
        {
            bool is64 = null != system && (bool)system
               ? Environment.Is64BitOperatingSystem
               : Environment.Is64BitProcess;
            return null != signed && (bool)signed
               ? is64 ? typeof(long) : typeof(int)
               : is64 ? typeof(ulong) : typeof(uint);
        }

        #endregion Pointer methods

        #endregion Pad, Parse and Pointer methods

        #region SizeOf methods

        public static int SizeOf(object? value)
        {
            return SizeOf(value, null);
        }

        public static int SizeOf(object? item, int? failValue)
        {
            // TODO ITypeInfo Interface ???
            // https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.comtypes.itypeinfo?view=netframework-4.8.1
            if (null == item) { // TypeCode.IsEmpty: 0 Bytes
                return 0;
            }
            object obj = (object)item;
            int size = 0;
            try {
                Type? t = GetType(obj);
                if (null == t) {
                    return size;
                }
                Type type = (Type)t;
                size = SizeOf(type, null);
                if (!Enum.IsDefined(typeof(TCSizes), size)) {
                    return 0;
                }
                if (size >= 0) {
                    // TCSizes.IsEmpty, Octet, Word, DWord, QWord, Para
                    return size;
                }
                bool isValue = !IsType(obj);
                TCSizes more = (TCSizes)size;
                bool asStr = false;
                bool tuple = false;
                switch (more) {
                    case TCSizes.BigInteger:
                        size = ((BigInteger)obj).ToByteArray().Length;
                        break;
                    case TCSizes.Pointer: return RunProps.Properties.SizeOfPointer;
                    case TCSizes.IntPtr: return RunProps.Properties.SizeOfPointer;
                    case TCSizes.UIntPtr: return RunProps.Properties.SizeOfPointer;
                    //
                    case TCSizes.String: asStr = true; break;
                    case TCSizes.SID: asStr = true; break;
                    case TCSizes.Uri: asStr = true; break;
                    case TCSizes.Base64: asStr = true; break;
                    case TCSizes.Guid: return RunProps.Properties.SizeOfGuid;
                    case TCSizes.UUID: return RunProps.Properties.SizeOfGuid;
                    case TCSizes.Enum:
                        return SizeOf(type.GetEnumUnderlyingType(), null);
                    case TCSizes.TimeSpan:
                        return RunProps.SizeOfTimeSpan;
                    case TCSizes.BitArray:
                        if (isValue) {
                            size = ((BitArray)obj).Length;
                            size += 8 - (8 - (size % 8));
                            size >>= 3;
                        } else {
                            size = sizeof(bool);
                        }
                        break;
                    case TCSizes.Tuple: tuple = true; break;
                    case TCSizes.ValueTuple: tuple = true; break;

                    case TCSizes.Object: break;
                    case TCSizes.Multiple: break;
                    case TCSizes.Unknown: size = 0; break;
                    default:
                        throw new NotImplementedException(more.ToString()); // TODO deprecate
                }
                if (tuple) {
                    size = SizeOfTuple(obj);
                } else if (asStr) {
                    size = isValue
                       ? ((string)obj).Length * sizeof(char)
                       : sizeof(char);
                }
            }
            catch {
                size = (int)TCSizes.Unknown;
            }
            if (size < 0 && null != failValue) {
                size = (int)failValue;
            }
            return size < 0 ? 0 : size;
        }

        public static int SizeOf(Type type)
        {
            return SizeOf(type, null);
        }

        public static int SizeOf(Type type, int? failValue)
        {
            TypeCoex typeCoex = GetTypeCoex(type);
            int size = SizeOf(typeCoex, failValue);
            if (size >= 0) {
                return size;
            }
            if (size < 0 && null != failValue) {
                size = failValue.Value;
            }
            return size;
        }

        public static int SizeOf(bool checkEnum, Type type, int? failValue)
        {
            TypeCode typeCode = Type.GetTypeCode(type);
            int size = SizeOf(typeCode, failValue);
            if (size >= 0) {
                return size;
            }
            if (checkEnum && type.IsEnum) {
                return SizeOf(type.UnderlyingSystemType, failValue);
            }
            if (size < 0 && null != failValue) {
                size = failValue.Value;
            }
            return size;
        }

        public static int SizeOf(TypeCode typeCode)
        {
            return SizeOf(typeCode, null);
        }

        public static int SizeOf(TypeCode typeCode, int? failValue)
        {
            return SizeOf((TypeCoex)typeCode, failValue);
        }

        public static int SizeOf(TypeCoex typeCoex)
        {
            return SizeOf(typeCoex, null);
        }

        public static int SizeOf(TypeCoex typeCoex, int? failValue)
        {
            int size = Enum.IsDefined(typeof(TypeCoex), typeCoex)
               ? TypeCoexSizes[(int)typeCoex]
               : int.MinValue;
            if (size < 0 && null != failValue) {
                size = failValue.Value;
            }
            return size;
        }

        public static int SizeOfTuple(object? tupleObj)
        {
            if (null == tupleObj || tupleObj is not ITuple) {
                return 0;
            }
            Type? t = GetType(tupleObj);
            if (null == t) {
                return 0;
            }
            bool isType = IsType(tupleObj);
            Type type = (Type)t;
            int size = 0;
            int length = t.GenericTypeArguments.Length;
            int i = 0;
            if (isType) {
                while (i < length) {
                    size += SizeOf(type.GenericTypeArguments[i++]);
                }
            } else {
                ITuple iTuple = (tupleObj as ITuple)!;
                while (i < length) {
                    size += SizeOf(iTuple[i++]);
                }
            }
            return size;
        }

        #endregion SizeOf methods

        #region TupleElementCount

        public static int TupleElementCount(object tupleObj)
        {
            int count = 0;
            if (null == tupleObj || tupleObj is not ITuple) {
                return count;
            }
            Type? type = GetType(tupleObj);
            if (null != type) {
                if (IsType(tupleObj)) {
                    count = type.GenericTypeArguments.Length;
                } else {
                    string[] parts = type.Name.Split(
                       new char[] { '`' },
                       StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1) {
                        if (!int.TryParse(parts[1], out count)) {
                            count = -1;
                        }

                    }
                }
            }
            return count;
        }

        #endregion TupleElementCount

        #endregion methods
    }
}
