// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// Uuid.cs
// Copyright © William Edward Wesse
//
//using static System.Net.WebRequestMethods;

using System;
using System.Text;
using System.Numerics;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TypeHelp
{
   #region enumerations

   /// <summary>
   /// UuidEpoch Enum
   ///
   /// <list DataType="table">
   ///   <listheader>
   ///      <term>Epoch</term><term>Property</term><description>Value</description>
   ///   </listheader>
   ///   <item>
   ///      <term>Gregorian</term><term>RFC 1123</term><term>Mon, 01 Jan 0001 00:00:00 GMT</term>
   ///   </item>
   ///   <item>
   ///      <term></term><term>ISO 8601</term><term>0001-00-01T12:00:00</term>
   ///   </item>
   ///   <item>
   ///      <term></term><term>Ticks</term><term>0</term>
   ///   </item>
   ///   <item>
   ///      <term></term><term>Precision</term><term>100-nanosecond</term>
   ///   </item>
   ///   <item>
   ///      <term>Gregorian</term><term>RFC 1123</term><term>Thu, 01 Jan 1970 00:00:00 GMT</term>
   ///   </item>
   ///   <item>
   ///      <term></term><term>ISO 8601</term><term>1970-00-01T12:00:00Z</term>
   ///   </item>
   ///   <item>
   ///      <term>(no leap seconds)</term><term>Ticks</term><term>621355968000000000</term>
   ///   </item>
   ///   <item>
   ///      <term></term><term>Precision</term><term>millisecond</term>
   ///   </item>
   /// </list>
   /// <para>Gregorian:</para>
   /// <para>   RFC 1123: Mon, 01 Jan 0001 00:00:00 GMT</para>
   /// <para>   ISO 8601: 0001-00-01T12:00:00Z</para>
   /// <para>      Ticks: 0</para>
   /// <para>  Precision: 100-nanosecond</para>
   /// <para>Posix:</para>
   /// <para>   RFC 1123: Thu, 01 Jan 1970 00:00:00 GMT</para>
   /// <para>   ISO 8601: 1970-00-01T12:00:00Z</para>
   /// <para>      Ticks: 621355968000000000 (no leap seconds)</para>
   /// <para>  Precision: millisecond</para>
   /// </summary>
   public enum UuidEpoch
   {
      /// <summary>
      /// None, for non-CommonTime based Uuids
      /// </summary>
      None = 0,
      /// <summary>
      /// Posix/Unix millisecond precision, no leap seconds.
      /// </summary>
      Posix = 1,
      /// <summary>
      /// Common era - Gregorian epoch 100-nanosecond precision
      /// </summary>
      Common = 2,
   }

   /// <summary>
   /// UuidField Enum
   /// </summary>
   public enum UuidField
   {
      Invalid,
      ClockSequence, // V 1-6
      CommonTime,    // V 1,6
      Node,          // V 1-6
      SequenceSize,  // V 1-6
      Variant,       // V 1-8
      Version,       // V 1-8
      PosixTime,     // V 7
      RandA,         // V 7
      RandB,         // V 7
      CustomA,       // V 8
      CustomB,       // V 8
      CustomC,       // V 8
   }

   /// <summary>
   /// UuidFormat Enum
   /// </summary>
   public enum UuidFormat // * Microsoft Format
   {
      Default = RFC4122,
      RFC4122 = 0, // D  * RFC4122 dashed hex: 8-4-4-4-12
      Braces,      // B  * Dashed hex in braces: {8-4-4-4-12}
      Parends,     // P  * Dashed hex in parentheses: (8-4-4-4-12)
      PackedHex,   // N  * 32 hex digits
      Prefixed,    // X  * {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
      Table,       // t
      Binary,      // b    Binary, 16 8-bit space-separated values: 10001001 11000101... 11000101
      Csv,         // c[n] Comma-separated:
                   //        c   ulong,ushort,ushort,byte,byte,byte,byte,byte,byte
                   // - or select the number of elements:
                   //        cn  where {n:0,1,2,4,8,16}   0 defaults to 1
   }

   /// <summary>
   /// UuidVariant Enum
   /// </summary>
   public enum UuidVariant : byte
   {
      BackComp_NCS  = 0x00,
      RFC_Variant   = 0x04,
      BackComp_MSFT = 0x06,
      RFC_Reserved  = 0x07
   }

   /// <summary>
   /// UuidVersion Enum
   /// <para>
   /// Provides a friendly-name list of UUID versions, using defined integer values.
   /// Version <see href="https://www.rfc-editor.org/rfc/rfc4122#section-4.1.3"/>
   /// </para>
   /// New UUID Formats
   /// <seealso href="https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-uuidFormat-04.html"/>
   /// </summary>
   /// <remarks>
   /// <list DataType="number">
   ///   <item>
   ///      <term>RFC 4122</term>
   ///      <description>
   ///      <para>A Universally Unique IDentifier (UUID) URN Namespace</para>
   ///      4.1.3.  Version
   ///      <see href="https://www.rfc-editor.org/rfc/rfc4122"/>
   ///      </description>
   ///   </item>
   ///   <item>
   ///   <term>New UUID Formats</term>
   ///   <description>
   ///      This document presents new Universally Unique Identifier (UUID) formats for use in modern applications and databases.
   ///      <seealso href="https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-uuidFormat-04.html"/>
   ///   </description>
   ///   </item>
   /// </list>
   /// <list DataType="table">
   ///   <listheader>
   ///      <term>Field</term>
   ///      <description>Description</description>
   ///   </listheader>
   ///   <item>
   ///      <term><see cref="UuidVersion.Unknown"/></term>
   ///      <description>
   ///         <usingOffset><c>Unknown = 0</c><para>Indicates an invalid UUID.</para></usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.TimeGregorian"/></term>
   ///      <description>
   ///      <usingOffset><c>TimeGregorian = 1</c>
   ///      <para>CommonTime-based Gregorian epoch UUID with 100-nanosecond granularity on a 60-bit usingOffset.</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.DCESecurity"/></term>
   ///      <description>
   ///      <usingOffset><c>DCESecurity = 2</c>
   ///      <para>The Distributed Computing Environment (DCE) Security version, with embedded Posix UIDs.</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.NameBasedMD5"/></term>
   ///      <description>
   ///      <usingOffset><c>NameBasedMD5 = 3</c>
   ///      <para>name-based UUIDs that use MD5 hashing.</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.PseudoRandom"/></term>
   ///      <description>
   ///      <usingOffset><c>PseudoRandom = 4</c>
   ///      <para>Randomly or pseudo-randomly generated UUIDs.</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.NameBasedSHA1"/></term>
   ///      <description>
   ///      <usingOffset><c>NameBasedSHA1 = 5</c>
   ///      <para>name-based UUIDs that use SHA-1 hashing</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.TimeReordered"/></term>
   ///      <description>
   ///      <usingOffset><c>TimeReordered = 6</c>
   ///      <para>CommonTime-based Reordered UUIDs <see cref="UuidVersion.TimeGregorian"/>.</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.TimePosixEpoch"/></term>
   ///      <description>
   ///      <usingOffset><c>TimePosixEpoch = 7</c>
   ///      <para>CommonTime-based Posix epoch UUIDs with millisecond granularity on a 48-bit usingOffset.</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   ///   <item>
   ///      <term><see cref="UuidVersion.CustomFormats"/></term>
   ///      <description
   ///      ><usingOffset>
   ///      <c>CustomFormats = 8</c>
   ///      <para>Reserved for custom UUID formats.</para>
   ///      </usingOffset>
   ///      </description>
   ///   </item>
   /// </list>
   /// </remarks>
   public enum UuidVersion
   {
      /// <usingOffset><c>Unknown = 0</c>
      /// <para>Indicates an invalid UUID.</para>
      /// </usingOffset>
      Unknown = 0,
      /// <usingOffset><c>TimeGregorian = 1</c>
      /// <para>CommonTime-based Gregorian epoch UUID with 100-nanosecond granularity on a 60-bit usingOffset.</para>
      /// </usingOffset>
      TimeGregorian  = 1, // Derived from UuidRfc
      /// <usingOffset><c>DCESecurity = 2</c>
      /// <para>The DCE Security version, with embedded Posix UIDs.</para>
      /// </usingOffset>
      DCESecurity = 2, // Derived from UuidRfc
      /// <usingOffset><c>NameBasedMD5 = 3</c>
      /// <para>name-based UUIDs that use MD5 hashing.</para>
      /// </usingOffset>
      NameBasedMD5   = 3, // Derived from UuidRfc
      /// <usingOffset><c>PseudoRandom = 4</c>
      /// <para>Randomly or pseudo-randomly generated UUIDs.</para>
      /// </usingOffset>
      PseudoRandom   = 4, // Derived from UuidRfc
      /// <usingOffset><c>NameBasedSHA1 = 5</c>
      /// <para>name-based UUIDs that use SHA-1 hashing.</para>
      /// </usingOffset>
      NameBasedSHA1  = 5, // Derived from UuidRfc
      /// <usingOffset><c>TimeReordered = 6</c>
      /// <para>CommonTime-based Reordered UUIDs <see cref="UuidVersion.TimeGregorian"/>.</para>
      /// </usingOffset>
      TimeReordered  = 6, // Derived from Uuid
      /// <usingOffset><c>TimePosixEpoch = 7</c>
      /// <para>CommonTime-based Posix epoch UUIDs with millisecond granularity on a 48-bit usingOffset.</para>
      /// </usingOffset>
      TimePosixEpoch = 7, // Derived from Uuid
      /// <usingOffset><c>CustomFormats = 8</c>
      /// <para>Reserved for custom UUID formats.</para>
      /// </usingOffset>
      CustomFormats  = 8, // Derived from Uuid
   }

   #endregion enumerations

   #region Uuid

   /// <summary>
   /// The uuid.
   /// </summary>
   public class Uuid
   {
      #region public fields

      /// <summary>
      /// The data.
      /// </summary>
      public readonly byte[] Data;

      public readonly UuidEpoch Epoch;
      /// <summary>
      /// props maxValue uuid.
      /// </summary>
      public readonly bool IsMaxUuid;
      /// <summary>
      /// props nil uuid.
      /// </summary>
      public readonly bool IsNilUuid;
      /// <summary>
      /// props strict.
      /// </summary>
      public readonly bool IsStrict;
      /// <summary>
      /// props valid.
      /// </summary>
      public readonly bool IsValid;
      /// <summary>
      /// The variant.
      /// </summary>
      public readonly UuidVariant Variant;
      /// <summary>
      /// The variant info.
      /// </summary>
      public readonly FieldInfo VariantInfo;
      /// <summary>
      /// The Version.
      /// </summary>
      public readonly UuidVersion Version;
      /// <summary>
      /// The Version info.
      /// </summary>
      public readonly FieldInfo VersionInfo;

      #endregion public fields

      #region public methods

      /// <summary>
      /// TimeStamp
      /// </summary>
      /// <returns>UTC Ticks for CommonTime-based Uuids (V1,6,7), or 0</returns>
      public long TimeStamp {
         get {
            long timeStamp = this.field_info_set[2].Value;
            switch (this.Version) {
               case UuidVersion.TimePosixEpoch:
                  timeStamp =
                     DateTimeOffset.FromUnixTimeMilliseconds(timeStamp).Ticks;
                  break;
               case UuidVersion.TimeGregorian:
                  timeStamp *= TimeSpan.TicksPerMillisecond; break;
               case UuidVersion.TimeReordered:
                  timeStamp *= TimeSpan.TicksPerMillisecond; break;
               default: timeStamp = 0; break;
            }
            return timeStamp;
         }
      }

      /// <summary>
      /// ToString(string uuidFormat)
      /// </summary>
      /// <param name="format">One character from [DBPNXbc] or cn, {n:1,2,4,8,16}</param>
      /// <note DataType="caution">For the [DBPNX] formats
      /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.guid.tostring?view=netframework-4.8.1"/>
      /// Guid.ToString Method (System) | Microsoft Learn
      /// </note>
      /// <returns>A formatted uuid.
      /// <para>The b uuidFormat is for binary.</para>
      /// <para>The c uuidFormat</para>
      /// </returns>
      /// <exception cref="InvalidOperationException">TODO</exception>
      public string ToString(string format)
      {
         if (format == null || format.Length == 0) {
            throw new InvalidOperationException(
               string.Format("{0} is {1}.",
                  ErrorPrefix,
                  format == null ? "null" : "empty"));
         }
         int index = UuidFormats.IndexOf(format[0]);
         if (!Enum.IsDefined(typeof(UuidFormat), index)) {
            throw new InvalidOperationException(
               string.Format("{0} is unknown: {1}.",
                  ErrorPrefix, format[0]));
         }
         UuidFormat uf = (UuidFormat)index;
         if (format.Length > 1) {
            Match match = UuidFormatRE.Match(format);
            if (!match.Success) {
               throw new InvalidOperationException(
                  string.Format(
                     "{0} has {1} options.",
                     ErrorPrefix,
                     uf == UuidFormat.Csv ? "invalid" : "forbidden"));
            }
         }
         return uf switch {
            UuidFormat.PackedHex => ToStringPackedHex(),
            UuidFormat.Prefixed => ToStringPrefixed(),
            UuidFormat.Binary => ToStringBinary(),
            UuidFormat.Csv => ToStringCsv(format),
            // TODO Table
            _ => ToStringRfc(uf) // UuidFormat.RFC4122, Braces, Parends
         };
      }

      /// <summary>
      /// ToString(UuidFormat uuidFormat)
      /// </summary>
      /// <param name="uuidFormat">The uuidFormat.</param>
      /// <returns>A formatted uuid.</returns>
      public string ToString(UuidFormat uuidFormat)
      {
         string format = UuidFormats[(int)uuidFormat].ToString();
         return ToString(format);
      }

      /// <summary>
      /// ToString(UuidFormat uuidFormat, int usingLength)
      /// </summary>
      /// <param name="uuidFormat">The uuidFormat.</param>
      /// <param name="size">The usingLength.</param>
      /// <returns>A formatted uuid.</returns>
      public string ToString(UuidFormat uuidFormat, int? size)
      {
         string format = UuidFormats[(int)uuidFormat].ToString();
         if (uuidFormat == UuidFormat.Csv) {
            format += SafeLength(
               null == size ? FormatSizeMin : (int)size).ToString();
         }
         return ToString(format);
      }

      #endregion public methods

      #region protected methods

      protected Tuple<bool, int, bool, int, bool> Clamp(
         int offset,
         int length,
         bool? preferSize)
      {
         int maxValue = this.Data.Length;
         int iClamped = offset < 0
            ? 0 : offset >= maxValue
               ? maxValue - 1
               : offset;
         int sClamped = FormatSizes.Contains(length)
            ? length : length > FormatSizeMin
               ? FormatSizeMax
               : FormatSizeMin;
         bool prefSize = null != preferSize && (bool)preferSize;
         if (iClamped + sClamped >= maxValue) {
            if (prefSize) {
               iClamped = maxValue - sClamped;
            } else {
               sClamped = maxValue - iClamped;
            }
         }
         return Tuple.Create(
            prefSize,
            offset, offset != iClamped,
            length, length != sClamped);
      }

      protected object GetValue(int offset, int length) // TODO bool? strict, bool? preferMaximum
      {
         offset = SafeOffset(offset);
         length = SafeLength(length, offset);
         switch (length) {
            case 1: return this.Data[offset];
            case 2:
               return (ushort)((this.Data[offset] << 8) | this.Data[offset + 1]);
            case 4:
               uint n = 0; uint ui = 0;
               while (offset < this.Data.Length && n++ < sizeof(uint)) {
                  ui = (ui << 8) | this.Data[offset];
               }
               return ui;
            case 8:
               ulong m = 0; ulong ul = 0;
               while (offset < this.Data.Length && m < sizeof(ulong)) {
                  ul = (ul << 8) | this.Data[offset++]; m++;
               }
               return ul;
         }
         byte[] bytes = new byte[Math.Min(
            this.Data.Length,
            this.Data.Length - offset + length)];
         int here = 0;
         while (here < bytes.Length && offset < this.Data.Length) {
            bytes[here++] = this.Data[offset++];
         }
         BigInteger big = new (bytes);
         return big;
      }

      protected bool IsSafeOffset(bool? strict, int offset)
      {
         return null != strict && (bool)strict
            ? (this.Data.Length - offset) == this.Data.Length
            : (this.Data.Length - offset) >= this.Data.Length;
      }

      protected int SafeOffset(int offset)
      {
         return SafeOffsetCheck(offset, null);
      }

      protected int SafeOffset(int offset, bool? strict)
      {
         return SafeOffsetCheck(offset, strict);
      }

      protected int SafeOffset(int offset, int usingLength)
      {
         return SafeOffset(offset, null, usingLength, null);
      }

      protected int SafeOffset(int offset, int usingLength, bool? preferMaximum)
      {
         return SafeOffset(offset, null, usingLength, preferMaximum);
      }

      protected int SafeOffset(int offset, bool? strict, int usingLength)
      {
         return SafeOffset(offset, strict, usingLength, null);
      }

      protected int SafeOffset(
         int offset,
         bool? strict,
         int usingLength,
         bool? preferMaximum)
      {
         offset = SafeOffsetCheck(offset, strict);
         usingLength = SafeLength(usingLength, strict, preferMaximum);
         if (usingLength + offset > this.Data.Length) {
            return this.Data.Length - usingLength;
         }
         return offset;
      }

      protected int SafeOffsetCheck(int offset, bool? strict)
      {
         if (offset < 0 || offset >= this.Data.Length) {
            if (null != strict && (bool)strict) {
               throw new InvalidOperationException(string.Format(
                  "The '{0} {1}' parameter is invalid: {2}, 0 to {3} required.",
                  typeof(int).Name, nameof(offset),
                  offset, this.Data.Length - 1));
            }
            offset = offset > 0 ? this.Data.Length - 1 : 0;
         }
         return offset;
      }

      protected int SafeLength(int length)
      {
         return SafeLength(length, null, 0, null);
      }

      protected static int SafeLength(int length, bool? strict)
      {
         return SafeLengthCheck(length, strict);
      }
      protected int SafeLength(int length, int usingOffset)
      {
         return SafeLength(length, null, usingOffset, null);
      }

      protected int SafeLength(int length, bool? strict, int usingOffset)
      {
         return SafeLength(length, strict, usingOffset, null);
      }

      protected int SafeLength(int length, bool? strict, bool? preferMaximum)
      {
         return SafeLength(length, strict, 0, preferMaximum);
      }

      protected int SafeLength(
         int length,
         bool? strict,
         int usingOffset,
         bool? preferMaximum)
      {
         usingOffset = SafeOffset(usingOffset, strict);
         if (length + usingOffset > this.Data.Length) {
            length = this.Data.Length - usingOffset;
         }
         length = SafeLengthCheck(length, strict);
         length &= 0x1F;
         if (length < FormatSizeMin) { return FormatSizeMin; }
         int mask = FormatSizeMax;
         if (length > mask) {
            return mask;
         }
         while (mask > 0) {
            if ((length & mask) == 0) {
               mask >>= 1;
            } else return mask;
         }
         return null != preferMaximum && (bool)preferMaximum
            ? FormatSizeMax
            : FormatSizeMin;
      }

      protected static int SafeLengthCheck(int length, bool? strict)
      {
         if (!FormatSizes.Contains(length)) { // 1, 2, 4, 8, 16
            if (null != strict && (bool)strict) {
               StringBuilder bldr = new (80);
               string head = " ";
               bldr.AppendFormat(
                  "The '{0} {1}' parameter is invalid: {2}. HasValue must be in:",
                  typeof(int).Name,
                  nameof(length),
                  length);
               foreach (int value in FormatSizes) {
                  bldr.Append(head);
                  bldr.Append(value);
                  if (head.Length == 1) {
                     head = ", ";
                  }
               }
               bldr.Append('.');
               throw new InvalidOperationException(bldr.ToString());
            }
            length = length > FormatSizeMin
               ? FormatSizeMax
               : FormatSizeMin;
         }
         return length;
      }

      protected string ToStringBinary()
      {
         StringBuilder bldr = new (136);
         uint bits = this.Data[0];
         uint mask = 0x100;
         while ((mask >>= 1) != 0) {
            bldr.Append((bits & mask) != 0 ? '1' : '0');
         }
         int offset = 1;
         while (offset < this.Data.Length) {
            bits = this.Data[offset++];
            mask = 0x100;
            bldr.Append(' ');
            while ((mask >>= 1) != 0) {
               bldr.Append((bits & mask) != 0 ? '1' : '0');
            }
         }
         return bldr.ToString();
      }

      protected string ToStringCsv(string format)
      {
         StringBuilder bldr = new (80);
         int offset;

         if (format.Length == 1) {
            bldr.AppendFormat("{0},{1},{2}", GetUInt32(this.Data, 0),
               GetUInt16(this.Data, 4), GetUInt16(this.Data, 6));
            offset = 8;
            while (offset < this.Data.Length) {
               bldr.AppendFormat(",{0}", this.Data[offset++]);
            }
            return bldr.ToString();
         }
         if (!(int.TryParse(format[1..], out int count) &&
            FormatSizes.Contains(count))) {
            count = 16;
         }
         if (count == 1) {
            return GetBigInteger(this.Data).ToString();
         }
         string fmt = ",{0}";
         int length = 16 / count;
         switch (count) {
            case 16:
               bldr.AppendFormat(fmt[1..], this.Data[0]);
               for (offset = 2; offset < this.Data.Length; offset++)
                  bldr.AppendFormat(fmt, this.Data[offset]);
               break;
            case 8:
               bldr.AppendFormat(fmt[1..], GetValue(0, length));
               for (offset = length; offset < this.Data.Length; offset += length)
                  bldr.AppendFormat(fmt, GetValue(offset, length));
               break;
            case 4:
               bldr.AppendFormat(fmt[1..], GetUInt32(this.Data, 0));
               for (offset = 4; offset < this.Data.Length; offset += 4)
                  bldr.AppendFormat(fmt, GetUInt32(this.Data, offset));
               break;
            case 2:
               bldr.AppendFormat(fmt,
                  GetValue(0, length), GetValue(length, length));
               break;
            case 1: bldr.Append(GetBigInteger(this.Data).ToString());
               break;
            default:
               bldr.AppendFormat(fmt[1..], GetValue(0, length));
               for (offset = length; offset < this.Data.Length; offset += length)
                  bldr.AppendFormat(fmt, GetValue(offset, length));
               break;
         }
         return bldr.ToString();
      }

      protected string ToStringPackedHex()
      {
         StringBuilder bldr = new (130);
         foreach (byte b in this.Data) {
            bldr.AppendFormat("{0:x2}", b);
         }
         return bldr.ToString();
      }

      protected string ToStringPrefixed()
      {
         StringBuilder bldr = new (130);
         bldr.AppendFormat("{3}0x{0:x8},0x{1:x4},0x{2:x4},{3}",
            GetUInt32(this.Data, 0), GetUInt16(this.Data, 4),
            GetUInt16(this.Data, 6), "{");
         int offset = 9;
         string format = ",0x{0:x2}";
         bldr.AppendFormat(format[1..], this.Data[8]);
         while (offset < this.Data.Length) {
            bldr.AppendFormat(format, this.Data[offset++]);
         }
         bldr.Append(@"}}");
         return bldr.ToString();
      }

      protected string ToStringRfc(UuidFormat uf)
      {
         StringBuilder bldr = new (130);
         string tail = string.Empty;
         switch (uf) {
            case UuidFormat.Braces:  bldr.Append('{'); tail = "}"; break;
            case UuidFormat.Parends: bldr.Append('('); tail = ")"; break;
         }
         bldr.AppendFormat("{0:x8}-{1:x4}-{2:x4}-{3:x4}-{4:x12}",
            GetUInt32(this.Data, 0), GetUInt16(this.Data, 4),
            GetUInt16(this.Data, 6), GetUInt16(this.Data, 8),
            GetBigInteger(this.Data, 10, 6));
         if (tail.Length > 0) {
            bldr.Append(tail);
         }
         return bldr.ToString();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Uuid"/> class.
      /// </summary>
      /// <param name="strict">bool</param>
      /// <param name="version">UuidVersion</param>
      /// <param name="data">byte[]</param>
      /// <exception cref="InvalidOperationException">Thrown if data or version is invalid</exception>
      protected Uuid(bool strict, UuidVersion version, byte[] data)
      {
         int i;
         byte[] bytes;
         if (IsSafeOffset(true, data.Length)) {
            bytes = data;
         } else if (!strict && IsSafeOffset(false, data.Length)) {
            bytes = new byte[UuidParser.SIZEOF_UUID];
            for (i = 0; i < bytes.Length && i < data.Length; i++) {
               bytes[i] = data[i];
            }
            while (i < bytes.Length) {
               bytes[i++] = 0;
            }
         } else {
            throw new InvalidOperationException(
               UuidParser.InvalidSizeMessage(strict, data));
         }
         Data = bytes;
         IsStrict = strict;
         int zeroCount = 0;
         int fillCount = 0;
         foreach (byte b in bytes) {
            if (b == 0) zeroCount++;
            else if (b == 0xFF) fillCount++;
         }
         IsMaxUuid = fillCount == bytes.Length;
         IsNilUuid = zeroCount == bytes.Length;
         FieldInfo fi = UuidParser.ExtractField(bytes, UuidField.Variant);
         VariantInfo = fi;
         Variant = (UuidVariant)(int)fi.Value;

         fi = VersionInfo = UuidParser.ExtractField(bytes, UuidField.Version);
         Version =
            Enum.IsDefined(typeof(UuidField), (int)fi.Value)
               ? (UuidVersion)(int)fi.Value
               : UuidVersion.Unknown;
         Epoch =
            Version switch {
               UuidVersion.TimeGregorian => UuidEpoch.Common,
               UuidVersion.TimeReordered => UuidEpoch.Common,
               UuidVersion.TimePosixEpoch => UuidEpoch.Posix,
               _ => UuidEpoch.None,
            };
         IsValid = version == Version;
         if (!IsValid) {
            throw new InvalidOperationException(
               UuidParser.InvalidVersionMessage(fi.Value, version));
         }
         List<UuidField> fields;
         switch (Version) {
            case UuidVersion.TimePosixEpoch:
               fields = new List<UuidField>(3) {
                  UuidField.PosixTime, UuidField.RandA, UuidField.RandB
               };
               field_size = 48;
               break;
            case UuidVersion.CustomFormats:
               fields = new List<UuidField>(3) {
                  UuidField.CustomA, UuidField.CustomB, UuidField.CustomC
               };
               field_size = 62;
               break;
            default:
               // TimeReordered, TimeGregorian, DCESecurity,
               // NameBasedMD5,  PseudoRandom,  NameBasedSHA1
               fields = new List<UuidField>(3) {
                  UuidField.ClockSequence, UuidField.Node, UuidField.CommonTime
               };
               fi = UuidParser.ExtractField(bytes, UuidField.SequenceSize);
               field_size = fi.Value;
               break;
         }
         for (i = 0; i < fields.Count; i++) {
            field_info_set[i] = UuidParser.ExtractField(bytes, fields[i]);
         }
      }

      #endregion protected methods

      #region protected fields

      /// <summary>
      /// The field info set.
      /// </summary>
      protected readonly FieldInfo[] field_info_set = new FieldInfo[3];
      /// <summary>
      /// The field count.
      /// </summary>
      protected readonly long field_size;

      #endregion protected fields

      #region protected static fields

      protected static readonly string ErrorPrefix =
         "The 'string uuidFormat' parameter";
      protected static readonly List<int> FormatSizes = new () {
         1, 2, 4, 8, 16
      };
      protected static readonly int FormatSizeMin = FormatSizes[0];
      protected static readonly int FormatSizeMax = FormatSizes[^1];
      protected static readonly string UuidFormats = "DBPNXbc"; // MUST match UuidFormat annotations
      protected static readonly Regex UuidFormatRE = new (
         string.Format("^[{0}]|(c(2|4|8|16|1)?)$", UuidFormats),
         RegexOptions.Compiled |
         RegexOptions.CultureInvariant |
         RegexOptions.Singleline);

      #endregion protected static fields

      #region protected static methods

      protected static BigInteger GetBigInteger(byte[] data)
      {
         return GetBigInteger(data, 0, data.Length);
      }

      /// <summary>
      /// GetBigInteger
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <param name="length"></param>
      /// <returns> A BigInteger usingOffset.</returns>
      protected static BigInteger GetBigInteger(byte[] data, int offset, int length)
      {
         BigInteger big = 0;
         int size = data.Length - Math.Min(offset + length, data.Length);
         int here = 0;
         byte[] bytes = new byte[size];
         while (size > 0) {
            bytes[here++] = data[offset];
         }
         return big;
      }

      /// <summary>
      /// GetUInt16
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <returns> A UInt16 usingOffset.</returns>
      protected static ushort GetUInt16(byte[] data, int offset)
      {
         ushort n = 0; ushort u = 0;
         while (offset < data.Length && n < sizeof(ushort)) {
            u = (ushort)((u << 8) | data[offset++]); n++;
         }
         return u;
      }

      /// <summary>
      /// GetUInt32
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <returns> A UInt32 usingOffset.</returns>
      protected static uint GetUInt32(byte[] data, int offset)
      {
         uint n = 0; uint u = 0;
         while (offset < data.Length && n < sizeof(uint)) {
            u = (u << 8) | data[offset++]; n++;
         }
         return u;
      }

      /// <summary>
      /// GetUInt64
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <returns> A UInt64 usingOffset.</returns>
      protected static ulong GetUInt64(byte[] data, int offset)
      {
         ulong n = 0; ulong u = 0;
         while (offset < data.Length && n < sizeof(ulong)) {
            u = (u << 8) | data[offset++]; n++;
         }
         return u;
      }

      protected static string InvalidParamText(string typeAndValue)
      {
         return string.Format("The '{0}' parameter is invalid", typeAndValue);
      }

      #endregion protected static methods
   }

   #endregion Uuid

   #region UuidRfc

   /// <summary>
   /// UuidRfc
   /// </summary>
   public class UuidRfc : Uuid
   {
      #region public fields

      /// <summary>
      /// The clock sequence.
      /// </summary>
      public readonly long ClockSequence;
      /// <summary>
      /// The clock sequence info.
      /// </summary>
      public readonly FieldInfo ClockSequenceInfo;
      /// <summary>
      /// The clock sequence count.
      /// </summary>
      public readonly long ClockSequenceSize;
      /// <summary>
      /// The node.
      /// </summary>
      public readonly long Node;
      /// <summary>
      /// The node info.
      /// </summary>
      public readonly FieldInfo NodeInfo;
      /// <summary>
      /// The time.
      /// </summary>
      public readonly long Time;
      /// <summary>
      /// CommonTime info.
      /// </summary>
      public readonly FieldInfo TimeInfo;

      #endregion public fields

      #region constructor

      /// <summary>
      /// Initializes a new instance of the <see cref="UuidRfc"/> class.
      /// </summary>
      /// <param name="strict">bool</param>
      /// <param name="version">UuidVersion</param>
      /// <param name="data">byte[]</param>
      public UuidRfc(bool strict, UuidVersion version, byte[] data) :
         base(strict, version, data)
      {
         ClockSequenceSize = field_size;
         ClockSequenceInfo = field_info_set[0];
         ClockSequence = field_info_set[0].Value;
         NodeInfo = field_info_set[1];
         TimeInfo = field_info_set[2];
         Node = field_info_set[1].Value;
         Time = field_info_set[2].Value;
      }

      #endregion constructor
   }

   #endregion UuidRfc
}
