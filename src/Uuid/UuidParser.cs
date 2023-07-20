// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// UuidParser.cs
// Copyright © William Edward Wesse
//
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TypeHelp
{
   [StructLayout(LayoutKind.Sequential)]
   public readonly struct FieldInfo
   {
      /// <summary>
      /// List&lt;byte&gt; Bytes
      /// </summary>
      public readonly List<byte> Bytes;
      /// <summary>
      /// UuidField Field
      /// </summary>
      public readonly UuidField Field;
      /// <summary>
      /// long HasValue
      /// </summary>
      public readonly long Value;

      /// <summary>
      /// FieldInfo
      /// </summary>
      /// <param name="field"></param>
      /// <param name="bytes"></param>
      /// <param name="value"></param>
      public FieldInfo(
         UuidField field,
         List<byte> bytes,
         long value)
      {
         Bytes = bytes;
         Field = field;
         Value = value;
      }
   }

   /// <summary>
   /// UuidParser
   /// </summary>
   public class UuidParser
   {
      #region public const and static fields


      public const bool   DefaultStrict = true;
      public const int    SIZEOF_UUID = 16;
      /// <summary>
      /// UuidFormatChars
      /// </summary>
      public const string UuidFormatChars = "DNBPXdb";
      public const string UuidMaxDecimalText =
         "340282366920938463463374607431768211455";
      public static readonly BigInteger UuidMaxValue =
         BigInteger.Parse(UuidMaxDecimalText);

      #endregion public const and static fields

      #region public static methods

      public static FieldInfo ExtractField(byte[] data, UuidField field)
      {
         List<byte> bytes;
         BigInteger value = new BigInteger(ulong.MaxValue) + 1;
         UuidVariant variant = ExtractVariant(data);
         UuidVersion version = ExtractVersion(data);
         switch (field) {
            case UuidField.ClockSequence: // V 1-6
               Tuple<ulong, List<byte>> cSeq =
                  ExtractClockSequence(variant, data);
               value = cSeq.Item1;
               bytes = cSeq.Item2;
               break;
            case UuidField.Node: // V 1-6
               bytes = ExtractBytes(data, 10, 6, 0xFF);
               break;
            case UuidField.CommonTime: // V 1-6
               Tuple<ulong, List<byte>> time = ExtractTime(version, data);
               value = time.Item1;
               bytes = time.Item2;
               break;
            case UuidField.SequenceSize: // V 1-6
               value = 15;
               switch (version) {
                  case UuidVersion.NameBasedMD5:  value = 14; break;
                  case UuidVersion.PseudoRandom:  value = 14; break;
                  case UuidVersion.NameBasedSHA1: value = 14; break;
               }
               switch (variant) {
                  case UuidVariant.BackComp_MSFT: value = 13; break;
                  case UuidVariant.RFC_Reserved:  value = 13; break;
               }
               bytes = new List<byte>(1) { (byte)value };
               break;
            case UuidField.Variant: // V 1-8
               byte octet = (byte)(data[8] & 0xE0);
               value = (ulong)variant;
               bytes = new List<byte>(1) { octet };
               break;
            case UuidField.Version: // V 1-8
               value = (ulong)ExtractVersion(data);
               bytes = new List<byte>(1) { (byte)value };
               break;
            case UuidField.PosixTime: // V 7
               bytes = ExtractBytes(data, 0, 6, 0xFF);
               break;
            case UuidField.RandA: // V 7
               bytes = ExtractBytes(data, 6, 2, 0x0F);
               break;
            case UuidField.RandB: // V 7
               bytes = ExtractBytes(data, 8, 8, 0x1F);
               break;
            case UuidField.CustomA: // V 8
               bytes = ExtractBytes(data, 0, 6, 0xFF);
               break;
            case UuidField.CustomB: // V 8
               bytes = ExtractBytes(data, 6, 2, 0x0F);
               break;
            case UuidField.CustomC: // V 8
               bytes = ExtractBytes(data, 8, 8, 0x1F);
               break;
            default:
               bytes = new List<byte>(0);
               value = 0;
               break;
         }
         if (value > ulong.MaxValue) {
            value = ExtractULong(bytes);
         }
         FieldInfo fInfo = new (field, bytes, (long)value);
         return fInfo;
      }

      public static string InvalidSizeMessage(bool strict, byte[] data)
      {
         int i = data.Length - UuidParser.SIZEOF_UUID;
         if (i == 0) {
            return string.Empty;
         }
         string label = i > 0 ? "extra" : "missing";
         i = Math.Abs(i);
         return string.Format(
            "strict = {0}, byte[{1}] Data - parameter is {2} {3} byte{4}",
            strict,
            data.Length, label,
            i, i == 1 ? string.Empty : "s");
      }

      /// <summary>
      /// InvalidVersionMessage
      /// </summary>
      /// <param name="version">the version to check</param>
      /// <param name="expected">the expected version</param>
      /// <returns>formatted error Text.</returns>
      public static string InvalidVersionMessage(
         long version,
         UuidVersion expected)
      {
         UuidVersion checkedVersion =
            Enum.IsDefined(typeof(UuidVersion), (int)version)
               ? (UuidVersion)(int)version
               : UuidVersion.Unknown;
         return string.Format(
            "Invalid version: {0} ({1}); expected {2} ({3})",
            version, checkedVersion,
            (int)expected, expected);
      }

      public static bool IsSafeSize(bool strict, byte[] data)
      {
         return IsSafeSize(strict, data, 0);
      }

      public static bool IsSafeSize(bool strict, byte[] data, int offset)
      {
         return strict
            ? (data.Length - offset) == SIZEOF_UUID
            : (data.Length - offset) >= SIZEOF_UUID;
      }

      #endregion public static methods

      #region protected static methods

      protected static List<byte> ExtractBytes(
         byte[] data,
         int index,
         int count,
         byte hiMask)
      {
         List<byte> bytes = new (count--) {
            (byte)(data[index++] & hiMask)
         };
         while (count > 0) {
            bytes.Add(data[index++]);
            count--;
         }
         return bytes;
      }

      protected static Tuple<ulong, List<byte>> ExtractClockSequence(
         UuidVariant variant,
         byte[] data)
      {
         byte mask = 0x1F;
         switch (variant) {
            case UuidVariant.BackComp_NCS: mask = 0x7F; break;
            case UuidVariant.RFC_Variant: mask = 0x3F; break;
         }
         List<byte> bytes = ExtractBytes(data, 8, 2, mask);
         return Tuple.Create(ExtractULong(bytes), bytes);
      }

      protected static Tuple<ulong, List<byte>> ExtractTime(
         UuidVersion version,
         byte[] data)
      {
         ulong value = 0;
         switch (version) {
            case UuidVersion.Unknown:       break; // 0 Unused
            case UuidVersion.TimeGregorian: break; // 1 Gregorian 100-nanosec time-based
            case UuidVersion.DCESecurity:   break; // 2 DCE Security, w/embedded Posix UIDs
            case UuidVersion.NameBasedMD5:  break; // 3 name-based using MD5 hashing
            case UuidVersion.PseudoRandom:  break; // 4 The [pseudo]randomly generated
            case UuidVersion.NameBasedSHA1: break; // 5 name-based using SHA-1 hashing
            case UuidVersion.TimeReordered:  value = (ulong)version; break; // 6 Reordered V1
            case UuidVersion.TimePosixEpoch: value = (ulong)version; break; // 7 Posix Epoch time-based
            case UuidVersion.CustomFormats:  value = (ulong)version; break; // 8 Reserved for custom formats
         }
         List<byte> bytes;
         byte b;
         switch (value) {
            case 0:
               bytes = new List<byte>(8);
               foreach (byte h in ExtractBytes(data, 6, 2, 0x0F)) bytes.Add(h);
               foreach (byte m in ExtractBytes(data, 4, 2, 0xFF)) bytes.Add(m);
               foreach (byte l in ExtractBytes(data, 0, 4, 0xFF)) bytes.Add(l);
               value = ExtractULong(bytes);
               break;
            case 6:
               // https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-format-04.html#name-uuid-version-6
               // 5.1. UUID Version 6
               bytes = new List<byte>(8);
               List<byte> extracted = ExtractBytes(data, 0, 8, 0xFF);
               b = (byte)((extracted[0] >> 4) & 0x0F);
               bytes.Add(b);
               b = (byte)((extracted[0] << 4) & 0xF0);
               for (int i = 0; i < 6; i++) {
                  bytes.Add((byte)(b | (byte)((extracted[i] >> 4) & 0x0F)));
                  b = (byte)((extracted[i] << 4) & 0xF0);
               }
               b |= (byte)((extracted[6] >> 4) & 0x0F);
               bytes.Add(b);
               bytes.Add(extracted[7]);
               value = ExtractULong(bytes);
               break;
            case 7:
               // https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-format-04.html#name-uuid-version-7
               // 5.2. UUID Version 7
               // unix_ts_ms: 48 bit Big-Endian unsigned number of Posix epoch timestamp
               // 
               bytes = ExtractBytes(data, 0, 6, 0xFF);
               value = ExtractULong(bytes);
               break;
            //case 8:
            // https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-format-04.html#name-uuid-version-8
            // 5.3. UUID Version 8
            default:
               bytes = new List<byte>(0);
               value = 0;
               break;
         }
         return Tuple.Create(value, bytes);
      }

      protected static ulong ExtractULong(List<byte> bytes)
      {
         ulong u = 0;
         for (int i = 0; i < Math.Min(8, bytes.Count); i++) {
            u = (ulong)((u << 8) | bytes[i]);
         }
         return u;
      }

      protected static UuidVariant ExtractVariant(byte[] data)
      {
         // Msb0 Msb1 Msb2 Description
         //  0    x    x   RFC_Reserved, NCS backward compatibility.
         //  1    0    x   The variant specified in this document.
         //  1    1    0   RFC_Reserved, Microsoft Corporation backward compatibility
         //  1    1    1   RFC_Reserved for future definition.
         byte octet = (byte)(data[8] & 0xE0);
         UuidVariant variant;
         if ((octet & 0x80) == 0) {
            variant = UuidVariant.BackComp_NCS;
         } else if ((octet & 0xC0) == 0x80) {
            variant = UuidVariant.RFC_Variant;
         } else if ((octet & 0xE0) == 0xC0) {
            variant = UuidVariant.BackComp_MSFT;
         } else {
            variant = UuidVariant.RFC_Reserved;
         }
         return variant;
      }

      protected static UuidVersion ExtractVersion(byte[] data)
      {
         int candidate = (byte)(data[6] >> 4) & 0x0F;
         if (Enum.IsDefined(typeof(UuidVersion), candidate)) {
            return (UuidVersion)candidate;
         }
         return UuidVersion.Unknown;
      }

      #endregion public static methods

      #region protected const and static fields

      protected const RegexOptions REOptions =
         RegexOptions.Compiled |
         RegexOptions.CultureInvariant |
         RegexOptions.Singleline;

      protected const string UidCharsPattern = @"(?<b>[0-9A-Fa-f])";
      protected const string UidDecPattern = @"[0-9]+";
      protected const string UidRFCPattern =
         @"\k<b>{8}(-\k<b>{4}){3}-\k<b>{12}";
      protected const string UidPrefixedPattern =
         @"\{0x\k<b>{8},(0x\k<b>{4},){2}" +
         @"\{(0x\k<b>{2},){7}0x\k<b>{2}}}";

      protected static readonly Regex[] UidREs = new Regex[] {
         new Regex( // Format: "D", RFC4122 dashed hex
            UidCharsPattern + UidRFCPattern, REOptions),
         new Regex( // Format: "N", PackedHex hex
            UidCharsPattern + @"\k<b>{32}", REOptions),
         new Regex( // Format: "B", RFC4122 dashed hex in braces
            UidCharsPattern + @"\{" + UidRFCPattern + "}",
            REOptions),
         new Regex( // Format: "P", RFC4122 dashed hex in parentheses
            UidCharsPattern + @"\(" + UidRFCPattern + @"/)",
            REOptions),
         new Regex( // Format: "X", "0x" prefixed dashed hex in parentheses
            UidCharsPattern + UidPrefixedPattern, REOptions),
         new Regex("[0-9]+", REOptions), // Format: "d", RFC4122
         new Regex( // Format: "b", Binary (format defined here only)
            @"^(?<b>[01]{8})( ?(\f<b>)){16}",
            REOptions),
      };

      #endregion protected const and static fields
   }
}
