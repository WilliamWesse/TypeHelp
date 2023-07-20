// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// TypeParser_Compare.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace TypeHelp
{
   public partial class TypeParser
   {
      #region public static Compare methods

      public static Cmp Comp(Cmp cmp)
      {
         return cmp;
      }

      public static Cmp CompFromInt(int value)
      {
         if (Enum.IsDefined(typeof(Cmp), value)) {
            return (Cmp)value;
         }
         return value > (int)Cmp.GTR ? Cmp.GTR : Cmp.NUL;
      }

      public static Cmp Compare(int valueA, int valueB)
      {
         Cmp cmp = valueA > valueB
            ? Cmp.GTR
            : valueA < valueB ? Cmp.LSS : Cmp.EQU;
         return cmp;
      }

      public static Cmp Compare(uint valueA, uint valueB)
      {
         Cmp cmp = valueA > valueB
            ? Cmp.GTR
            : valueA < valueB ? Cmp.LSS : Cmp.EQU;
         return cmp;
      }

      public static Cmp Compare(long valueA, long valueB)
      {
         Cmp cmp = valueA > valueB
            ? Cmp.GTR
            : valueA < valueB ? Cmp.LSS : Cmp.EQU;
         return cmp;
      }

      public static Cmp Compare(ulong valueA, ulong valueB)
      {
         Cmp cmp = valueA > valueB
            ? Cmp.GTR
            : valueA < valueB ? Cmp.LSS : Cmp.EQU;
         return cmp;
      }

      public static Cmp Compare(string? valueA, string? valueB)
      {
         if (null == valueA) {
            return (null == valueB) ? Cmp.NUL : Cmp.UNF;
         } else if (null == valueB) {
            return Cmp.OVF;
         }
         int diff = valueA.CompareTo(valueB);
         return diff > 0
            ? Cmp.GTR
            : diff < 0
               ? Cmp.LSS
               : Cmp.EQU;
      }

      public static Cmp Compare(bool byValue, object objA, object objB)
      {
         return byValue
            ? CompareValues(objA, objB)
            : CompareTypes(objA, objB);
      }

      public static Cmp Compare(bool strict, byte[]? bytesA, byte[]? bytesB)
      {
         if (null == bytesA) {
            return (null == bytesB) ? Cmp.NUL : Cmp.UNF;
         } else if (null == bytesB) {
            return Cmp.OVF;
         }
         int iA = bytesA.Length - 1;
         int iB = bytesB.Length - 1;
         if (strict) { // ignore most significant zero Bytes.
            while (iA >= 0 && bytesA[iA] == 0) iA--;
            while (iB >= 0 && bytesB[iB] == 0) iB--;
         }
         if (iA != iB) {
            return iA > iB ? Cmp.GTR : Cmp.LSS;
         }
         while (iA >= 0) {
            if (bytesA[iA] != bytesB[iA]) {
               return bytesA[iA] > bytesB[iA] ? Cmp.GTR : Cmp.LSS;
            }
            iA--;
         }
         return Cmp.EQU;
      }

      public static Cmp CompareEnums(TypeInfo tioA, TypeInfo tioB)
      {
         return tioA.HasValue
            ? CompareEnumValues(tioA, tioB)
            : CompareEnumTypes(tioA, tioB);
      }

      public static Cmp CompareEnumTypes(TypeInfo tioA, TypeInfo tioB)
      {
         if (!tioA.IsEnum) {
            return (!tioB.IsEnum) ? Cmp.NUL : Cmp.UNF;
         } else if (!tioB.IsEnum) {
            return Cmp.OVF;
         }
         if (tioA.TypeCoex == tioB.TypeCoex) {
            return Cmp.EQU;
         }
         return CompareTypeCoexes(tioA.TypeCoex, tioB.TypeCoex);
      }

      public static Cmp CompareEnumValues(TypeInfo tioA, TypeInfo tioB)
      {
         bool haveB = tioB.HasValue && tioB.IsEnum;
         if (!(tioA.HasValue && tioA.IsEnum)) {
            return (!haveB) ? Cmp.NUL : Cmp.UNF;
         } else if (!haveB) {
            return Cmp.OVF;
         }
         BigInteger bigA = (BigInteger)tioA.Value!;
         BigInteger bigB = (BigInteger)tioB.Value!;
         if (bigA == bigB) {
            return Cmp.EQU;
         }
         return bigA > bigB ? Cmp.GTR : Cmp.LSS;
      }

      public static Cmp CompareGuids(bool asString, Guid guidA, Guid guidB)
      {
         int result = asString
            ? guidA.ToString("N").CompareTo(guidB.ToString("N"))
            : guidA.CompareTo(guidB);
         if (result == 0) {
            return Cmp.EQU;
         }
         return result > 0
            ? Cmp.GTR
            : Cmp.LSS;
      }

      public static Cmp CompareGuids(TypeInfo tioA, TypeInfo tioB)
      {
         if (!tioA.HasValue) {
            return (!tioB.HasValue) ? Cmp.NUL : Cmp.UNF;
         } else if (!tioB.HasValue) {
            return Cmp.OVF;
         }
         Guid guidA = Guid.Empty;
         bool haveA = false;
         Guid guidB = Guid.Empty;
         bool haveB = false;
         switch (tioA.TypeCoex) {
            case TypeCoex.Guid: guidA = (Guid)tioA.Value!; haveA = true; break;
            case TypeCoex.UUID: guidA = (Guid)tioA.Value!; haveA = true; break; // TODO
            case TypeCoex.String:
               haveA = Guid.TryParse((string)tioA.Value!, out guidA);
               break;
         }
         switch (tioB.TypeCoex) {
            case TypeCoex.Guid: guidB = (Guid)tioB.Value!; haveB = true; break;
            case TypeCoex.UUID: guidB = (Guid)tioB.Value!; haveB = true; break; // TODO
            case TypeCoex.String:
               haveB = Guid.TryParse((string)tioB.Value!, out guidB);
               break;
         }
         Cmp cmp = Cmp.NUL;
         if (haveA && haveB) {
            int diff = guidA.CompareTo(guidB);
            if (diff == 0) {
               cmp = Cmp.EQU;
            } else {
               cmp = diff > 0 ? Cmp.GTR : Cmp.LSS;
            }
         } else if (haveA) {
            cmp = Cmp.OVF;
         } else if (haveB) {
            cmp = Cmp.UNF;
         }
         return cmp;
      }

      public static Cmp CompareNulls(TypeInfo tioA, TypeInfo tioB)
      {
         if (tioA.TypeCoex != tioB.TypeCoex) {
            return Cmp.NUL;
         }
         Cmp cmp = Cmp.EQU;
         switch (tioA.TypeCoex) {
            case TypeCoex.Object:
               if (!tioA.HasValue) {
                  cmp = tioB.HasValue ? Cmp.GTR : Cmp.NUL;
               } else {
                  cmp = !tioB.HasValue ? Cmp.GTR : Cmp.LSS;
               }
               return cmp;
            case TypeCoex.Empty: break;
            case TypeCoex.DBNull: break;
            case TypeCoex.Nullity: break;
            default: return Cmp.NUL;
         }
         if (!tioA.HasValue) {
            cmp = tioB.HasValue ? Cmp.LSS : Cmp.UNF;
         } else if (!tioB.HasValue) {
            cmp = Cmp.OVF;
         }
         return cmp;
      }

      public static Cmp CompareNumerics(TypeInfo tioA, TypeInfo tioB)
      {
         return CompareNumerics(tioA, tioB, true);
      }

      public static Cmp CompareNumerics(TypeInfo tioA, TypeInfo tioB, bool integersExt)
      {
         if (tioA.IsType || tioB.IsType) {
            return CompareNumericTypes(tioA, tioB);
         } else if (!tioA.IsNumber) {
            return tioB.IsNumber ? Cmp.LSS : Cmp.UNF;
         } else if (!tioB.IsNumber) {
            return Cmp.OVF;
         } else if (!tioA.HasValue) {
            return tioB.IsNumber ? Cmp.LSS : Cmp.UNF;
         } else if (!tioB.HasValue) {
            return Cmp.OVF;
         }
         int cmp;
         if (!integersExt) {
            bool hasA = TCF.IntegersExt.HasFlag(tioA.CoexFlag);
            bool hasB = TCF.IntegersExt.HasFlag(tioB.CoexFlag);
            if (hasA && hasB) {
               return Cmp.NUL;
            }
            if (hasA || hasB) {
               return (!hasB) ? Cmp.NUL : Cmp.UNF;
            }
         }
         try {
            object a = (object)tioA.Value!;
            object b = (object)tioB.Value!;
            cmp = tioA.TypeCoex switch {
               TypeCoex.Int32  => ((int)a).CompareTo((int)b),
               TypeCoex.Int64  => ((long)a).CompareTo((long)b),
               TypeCoex.UInt32 => ((uint)a).CompareTo((uint)b),
               TypeCoex.UInt64 => ((ulong)a).CompareTo((ulong)b),
               TypeCoex.Byte   => ((byte)a).CompareTo((byte)b),
               TypeCoex.Int16  => ((short)a).CompareTo((short)b),
               TypeCoex.UInt16 => ((ushort)a).CompareTo((ushort)b),
               TypeCoex.SByte  => ((sbyte)a).CompareTo((sbyte)b),
               TypeCoex.Single => ((float)a).CompareTo((float)b),
               TypeCoex.Double => ((double)a).CompareTo((double)b),
               TypeCoex.Decimal => ((decimal)a).CompareTo((decimal)b),
               TypeCoex.DateTime => ((DateTime)a).Ticks.CompareTo(((DateTime)b).Ticks),
               TypeCoex.TimeSpan => ((TimeSpan)a).Ticks.CompareTo(((TimeSpan)b).Ticks),
               TypeCoex.IntPtr   => (int)Compare(true, (IntPtr)a, (IntPtr)b),
               TypeCoex.UIntPtr  => (int)Compare(true, (UIntPtr)a, (UIntPtr)b),
               TypeCoex.Complex  => (int)Compare(true, (Complex)a, (Complex)b),
               TypeCoex.BigInteger => ((BigInteger)a).CompareTo((BigInteger)b),
               TypeCoex.Enum => (int)CompareEnums(tioA, tioB),
               // integersExt
               TypeCoex.Char => ((int)a).CompareTo((int)b),
               TypeCoex.String => (int)CompareNumericStrings(tioA, tioB),
               _ => (int)Cmp.OVF,
            };
         } catch {
            cmp = (int)Cmp.OVF;
         }
         return (Cmp)cmp;
      }

      public static Cmp CompareNumericStrings(TypeInfo tioA, TypeInfo tioB)
      {
         bool haveB = tioB.HasValue && tioB.TypeCode == TypeCode.String;
         if (!(tioA.HasValue && tioA.TypeCode == TypeCode.String)) {
            return (!haveB) ? Cmp.NUL : Cmp.UNF;
         } else if (!haveB) {
            return Cmp.OVF;
         }
         bool isBigA = BigInteger.TryParse(
            (string)tioA.Value!, out BigInteger bigA);
         bool isBigB = BigInteger.TryParse(
            (string)tioB.Value!, out BigInteger bigB);
         if (!isBigA) {
            return (!isBigB) ? Cmp.NUL : Cmp.UNF;
         } else if (!isBigB) {
            return Cmp.OVF;
         }
         int diff = bigA.CompareTo(bigB);
         if (diff == 0) {
            return Cmp.EQU;
         }
         return diff > 0 ? Cmp.GTR : Cmp.LSS;
      }

      public static Cmp CompareNumericTypes(TypeInfo tioA, TypeInfo tioB)
      {
         if (tioA.TypeCode == TypeCode.String) {
            return CompareNumericStrings(tioA, tioB);
         }
         if (!tioA.IsNumber) {
            return (!tioB.IsNumber) ? Cmp.NUL : Cmp.UNF;
         } else if (!tioB.IsNumber) {
            return Cmp.OVF;
         }
         return CompareTypeCoexes(tioA.TypeCoex, tioB.TypeCoex);
      }

      public static Cmp CompareTuples(object tupleA, object tupleB)
      {
         Cmp cmp = Cmp.NUL;
         if (null == tupleA) {
            cmp = null != tupleB
               ? tupleB is ITuple ? Cmp.LSS : Cmp.UNF
               : Cmp.EQU;
         } else if (null == tupleB) {
            cmp = tupleA is ITuple ? Cmp.GTR : Cmp.OVF;
         } else if (tupleA is not ITuple) {
            cmp = tupleB is ITuple ? Cmp.LSS : Cmp.UNF;
         } else if (tupleB is not ITuple) {
            cmp = Cmp.OVF;
         }
         if (cmp != Cmp.NUL) {
            return cmp;
         }
         int sizeOfA;
         int sizeOfB = 0;
         try {
            int sA = SizeOfTuple(tupleA);
            int sB = SizeOfTuple(tupleB);
            sizeOfA = sA;
            sizeOfB = sB;
         } catch {
            sizeOfA = -1;
         }
         if (sizeOfA < 0) {
            return Cmp.NUL;
         }
         if (sizeOfA == sizeOfB) {
            return Cmp.EQU;
         }
         return sizeOfA > sizeOfB ? Cmp.GTR : Cmp.LSS;
      }

      public static Cmp CompareTypeCoexes(TypeCoex coexA, TypeCoex coexB)
      {
         int idxA = TypeParser.TypeCoexSizePriority.IndexOf(coexA);
         int idxB = TypeParser.TypeCoexSizePriority.IndexOf(coexB);
         if (idxA == idxB) {
            return Cmp.EQU;
         }
         return idxA > idxB ? Cmp.GTR : Cmp.LSS;
      }

      public static Cmp CompareTypes(object objA, object objB)
      {
         if (!TypeParser.IsValid(objA)) {
            return (!TypeParser.IsValid(objB)) ? Cmp.NUL : Cmp.UNF;
         } else if (!TypeParser.IsValid(objB)) {
            return Cmp.OVF;
         }
         TypeCoex coexA = GetTypeCoex(objA);
         TypeCoex coexB = GetTypeCoex(objB);
         return CompareTypeCoexes(coexA, coexB);
      }

      public static Cmp CompareValues(object objA, object objB)
      {
         if (!TypeParser.IsValid(objA)) {
            return TypeParser.IsValid(objB) ? Cmp.LSS : Cmp.UNF;
         } else if (!TypeParser.IsValid(objB)) {
            return Cmp.OVF;
         }
         TypeInfo tioA = new (objA);
         TypeInfo tioB = new (objB);
         if (tioA.IsType || tioB.IsType ||
            !(tioA.HasValue && tioB.HasValue)) {
            return Cmp.NUL;
         }
         if (tioA.IsNumber) {
            return tioB.IsNumber
               ? CompareNumerics(tioA, tioB)
               : Cmp.UNF;
         }
         if (tioA.TypeCode == TypeCode.String) {
            if (tioB.TypeCode == TypeCode.String) {
               return Compare((string)tioA.Value!, (string)tioB.Value!);
            }
         }
         if (tioA.TypeCoex != tioB.TypeCoex) { // TODO DataType matching?
            return Cmp.NUL;
         }
         int result;
         try {
            object a = (object)tioA.Value!;
            object b = (object)tioB.Value!;
            result = tioA.TypeCoex switch {
               TypeCoex.Boolean => ((bool)a).CompareTo((bool)b),
               TypeCoex.DBNull => (int)CompareNulls(tioA, tioB),
               TypeCoex.Object => (int)CompareNulls(tioA, tioB),
               TypeCoex.Tuple => (int)CompareTuples(a, b),
               TypeCoex.ValueTuple => (int)CompareTuples(a, b),
               TypeCoex.Nullity => (int)CompareNulls(tioA, tioB),
               //
               // TODO disambiguate HasCoex.Base64, SID, Uri
               //
               TypeCoex.UUID => (int)CompareGuids(tioA, tioB),
               TypeCoex.Enum => (int)CompareEnumValues(tioA, tioB),
               TypeCoex.Guid => (int)CompareGuids(tioA, tioB),
               _ => tioA.TypeCode == TypeCode.String
                     ? tioB.TypeCode == TypeCode.String
                        ? (int)Compare((string)a, (string)b)
                        : (int)Cmp.GTR
                     : (int)Cmp.LSS,
            };
         } catch {
            result = (int)Cmp.NUL;
         }
         Cmp cmp = Cmp.NUL;
         if (Enum.IsDefined(typeof(Cmp), result)) {
            cmp = (Cmp)result;
         } else if (result < 0) {
            cmp = result < int.MinValue ? Cmp.UNF : Cmp.LSS;
         } else if (result > 0) {
            cmp = Cmp.GTR;
         }
         return cmp;
      }

      #endregion public static Compare methods
   }
}
