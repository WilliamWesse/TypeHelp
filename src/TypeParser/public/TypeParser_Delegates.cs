// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// TypeParser_Delegates.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Numerics;

namespace TypeHelp
{
   public partial class TypeParser
   {
      #region Set (flags by integer type) methods

      public static byte Set(bool set, byte value, byte bits)
      {
         return (byte)(set ? value | bits : value & ~bits);
      }

      public static int Set(bool set, int value, int bits)
      {
         return set ? value | bits : value & ~bits;
      }

      public static IntPtr Set(bool set, IntPtr value, IntPtr bits)
      {
         long raw = set
            ? (long)value | (long)bits
            : (long)value & ~(long)bits;
         return RunProps.Features.Is64BitApp
            ? new IntPtr(raw)
            : new IntPtr((int)raw);
      }

      // TODO 32-bit test app: Set(bool set, IntPtr HasValue, IntPtr HasCoex, bool? forceAltSize)
      public static IntPtr Set(
         bool set,
         IntPtr value,
         TypeCoex typeCoex,
         bool? forceAltSize)
      {
         if (null == forceAltSize) {
            forceAltSize = RunProps.Features.PreferInt32;
         }
         long raw = set
            ? (long)value | (long)typeCoex
            : (long)value & ~(long)typeCoex;
         return RunProps.Features.Is32BitSys || (bool)forceAltSize
            ? new IntPtr((int)raw)
            : new IntPtr(raw);
      }

      public static UIntPtr Set(
         bool set,
         UIntPtr value,
         TypeCoex typeCoex,
         bool? forceAltSize)
      {
         if (null == forceAltSize) {
            forceAltSize = RunProps.Features.PreferInt32;
         }
         ulong raw = set
            ? (ulong)value |  (ulong)typeCoex
            : (ulong)value & ~(ulong)typeCoex;
         return RunProps.Features.Is32BitSys || (bool)forceAltSize
            ? new UIntPtr((uint)raw)
            : new UIntPtr(raw);
      }

      public static long Set(bool set, long value, long bits)
      {
         return set ? value | bits : value & ~bits;
      }

      public static sbyte Set(bool set, sbyte value, sbyte bits)
      {
         return (sbyte)(set ? value | bits : value & ~bits);
      }

      public static short Set(bool set, short value, short bits)
      {
         return (short)(set ? value | bits : value & ~bits);
      }

      public static uint Set(bool set, uint value, uint bits)
      {
         return set ? value | bits : value & ~bits;
      }

      public static ulong Set(bool set, ulong value, ulong bits)
      {
         return set ? value | bits : value & ~bits;
      }

      public static ushort Set(bool set, ushort value, ushort bits)
      {
         return (ushort)(set ? value | bits : value & ~bits);
      }

      public static BigInteger Set(bool set, BigInteger value, BigInteger bits)
      {
         return set ? value | bits : value & ~bits;
      }

      public static UIntPtr Set(bool set, UIntPtr value, UIntPtr bits)
      {
         ulong raw = set
            ? (ulong)value |  (ulong)bits
            : (ulong)value & ~(ulong)bits;
         return RunProps.Features.Is64BitApp
            ? new UIntPtr(raw)
            : new UIntPtr((uint)raw);
      }

      public static TCF Set(bool set, TCF value, TCF flags)
      {
         return (TCF)TypeParser.Set(set, (ulong)value, (ulong)flags);
      }

      #endregion Set (flags by integer type) methods

      #region public static GetTypeData, GetWellKnownTypes methods


      public static TypeInfo GetTypeInfo(object? obj)
      {
         TypeCoex typeCoex = TypeParser.GetTypeCoex(obj);
         TCF typeflag = TypeParser.GetCoexFlag(typeCoex);
         return TypeParser.GetTypeInfo(true, true, typeflag);
      }

      public static TypeInfo GetTypeInfo(TypeCode typeCode)
      {
         TCF typeflag = TypeParser.GetCodeFlag(typeCode);
         return TypeParser.GetTypeInfo(true, true, typeflag);
      }

      public static TypeInfo GetTypeInfo(TypeCoex typeCoex)
      {
         TCF typeflag = TypeParser.GetCoexFlag(typeCoex);
         return TypeParser.GetTypeInfo(true, true, typeflag);
      }

      public static TypeInfo GetTypeInfo(
         bool valid,
         bool parsed,
         TCF  typeFlag)
      {
         TypeInfo typeInfo = TypeParser.GetTypeInfo(typeFlag);
         if (typeInfo.CoexFlag == TCF.None) {
            parsed = false;
         }
         TCF result = TCF.Processed | typeInfo.CoexFlag;
         result |= (valid && parsed
            ? TCF.Success
            : (parsed ? TCF.Nullity : TCF.Empty) |
               (valid ? TCF.Error   : TCF.Fatal));
         typeInfo.CoexStat |= result;
         return typeInfo;
      }

      public static TypeInfo GetTypeInfo(TCF typeFlag)
      {
         ulong bits = (ulong)TypeParser.GetWellKnownTypes(typeFlag);
         if (bits != 0) {
            while ((bits & 1) == 0) bits >>= 1;
         }
         return new TypeInfo((TCF)bits);
      }

      public static TCF GetWellKnownTypes(TCF value)
      {
         ulong bits = (ulong)value & (ulong)TCF.WellKnowns;
         return (TCF)bits;
      }

      #endregion public static GetTypeData, GetWellKnownTypes methods

      #region static delegate lambda methods

      // ParseAuto

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, long>> TCF_Template =
         (TCF flags, string s) => {
            long value  = 0;
            bool valid  = TypeParser.IsValid(false, false, s);
            bool parsed = valid && long.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Int64);
            Tuple<TCF, TypeCoex, long> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, bool>> ParseToBool =
         (TCF flags, string s) => {
            bool value = false;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && bool.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Boolean);
            Tuple<TCF, TypeCoex, bool> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, sbyte>> ParseToSByte =
         (TCF flags, string s) => {
            sbyte value = 0;
            bool  valid = TypeParser.IsValid(false, false, s);
            bool  parsed = valid && sbyte.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.SByte);
            Tuple<TCF, TypeCoex, sbyte> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, byte>> ParseToByte =
         (TCF flags, string s) => {
            byte value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && byte.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Byte);
            Tuple<TCF, TypeCoex, byte> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, short>> ParseToShort =
         (TCF flags, string s) => {
            short value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && short.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Int16);
            Tuple<TCF, TypeCoex, short> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, Int32>> ParseToInt =
         (TCF flags, string s) => {
            int value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && int.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Int32);
            Tuple<TCF, TypeCoex, Int32> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, long>> ParseToLong =
         (TCF flags, string s) => {
            long value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && long.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Int64);
            Tuple<TCF, TypeCoex, long> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, Single>> ParseToSingle =
         (TCF flags, string s) => {
            float value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && Single.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Single);
            Tuple<TCF, TypeCoex, Single> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, Double>> ParseToDouble =
         (TCF flags, string s) => {
            double value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && Double.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Double);
            Tuple<TCF, TypeCoex, Double> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, Decimal>> ParseToDecimal =
         (TCF flags, string s) => {
            decimal value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && Decimal.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.Decimal);
            Tuple<TCF, TypeCoex, Decimal> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, DateTime>> ParseToDateTime =
         (TCF flags, string s) => {
            DateTime value = DateTime.MinValue;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && DateTime.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.DateTime);
            if (!parsed) {
               value = flags.HasFlag(TCF.FailToEmpty)
                  ? DateTime.MinValue
                  : DateTime.MaxValue;
            }
            Tuple<TCF, TypeCoex, DateTime> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, UInt16>> ParseToUShort =
         (TCF flags, string s) => {
            ushort value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && UInt16.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.UInt16);
            Tuple<TCF, TypeCoex, UInt16> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, UInt32>> ParseToUInt =
         (TCF flags, string s) => {
            uint value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && uint.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.UInt32);
            Tuple<TCF, TypeCoex, UInt32> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, ulong>> ParseToULong =
         (TCF flags, string s) => {
            ulong value = 0;
            bool valid = TypeParser.IsValid(false, false, s);
            bool parsed = valid && ulong.TryParse(s, out value);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, parsed, TCF.UInt64);
            Tuple<TCF, TypeCoex, ulong> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, value);
            return result;
         };

      private static Func<TCF, string,
         Tuple<TCF, TypeCoex, string>> ParseToString =
         (TCF flags, string s) => {
            if (null == s) {
               s = string.Empty;
            }
            bool valid = TypeParser.IsValid(false, false, s);
            TypeInfo typeInfo =
               TypeParser.GetTypeInfo(valid, valid, TCF.String);
            Tuple<TCF, TypeCoex, string> result =
               Tuple.Create(typeInfo.CoexFlag, typeInfo.TypeCoex, s);
            return result;
         };

      #endregion static delegate lambda methods

      #region TODO static delegate lambda methods
      // Base64
      // Guid
      // SID
      // Uri
      // UUID
      // Enum
      // IntPtr
      // UIntPtr
      // BigInteger
      // Complex
      // Tuple
      // ValueTuple


      #endregion TODO static delegate lambda methods
   }
}
