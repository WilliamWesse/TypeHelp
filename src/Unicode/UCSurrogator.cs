// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// UCSurrogator.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TypeHelp
{
   #region UCSurrogator

   [StructLayout(LayoutKind.Sequential)]
   public readonly struct UCSurrogator
   {
      #region reference

      // ====================================================================
      // FAQ - Private-Use Characters, Noncharacters, and Sentinels
      // (noncharacters)
      // http://www.unicode.org/faq/private_use.html#noncharacters
      // The 66 noncharacters are allocated as follows:
      // · a contiguous range of 32 noncharacters: U+FDD0..U+FDEF in the BMP
      // · the last two typeCoex points of the BMP, U+FFFE and U+FFFF
      // · the last two typeCoex points of each of the 16 supplementary planes:
      //   U+1FFFE, U+1FFFF, U+2FFFE, U+2FFFF, ...U+10FFFE, U+10FFFF
      // ====================================================================
      // FAQ - UTF-8, UTF-16, UTF-32 & BOM
      // https://www.unicode.org/faq/utf_bom.html#utf16-3
      // see also: https://unicodeplus.com/plane
      // --------------------------------------------------------------------
      // Redacted (with added '// Note #' suffixes):
      // ..the algorithm to convert from UTF-16 to typeCoex points
      //   typedef unsigned int16 UTF16;
      //   typedef unsigned int32 UTF32;
      // Calculate the H (or leading) and L (or trailing) surrogates
      // from a character typeCoex C, where “X”, “U” and “W” correspond to
      // the labels used in Table 3-5 UTF-16 Bit Distribution.
      //   const UTF16 HI_SURROGATE_START = 0xD800
      //   const UTF16 LO_SURROGATE_START = 0xDC00
      //   UTF16 X = (UTF16) C;
      //   UTF32 U = (C >> 16) & ((1 << 5) -1);   // *1 5 lo-bits of hi-word
      //   UTF16 W = (UTF16) U - 1;               // *2 U to Plane Id
      //   UTF16 H = (UTF16) (HI_SURROGATE_START | (W << 6) | (X >> 10));//*3
      //   UTF16 L = (UTF16) (LO_SURROGATE_START | X & ((1 << 10) -1));  //*4
      // Calculate the reverse, where a H and L supplement set are merged,
      // and “c” is the resulting character.
      //   UTF32 X = (H & ((1 <<  6) -1)) << 10 |
      //             (L & ((1 << 10) -1));        // *5
      //   UTF32 U = (H >> 6) & ((1 << 5) - 1);   // *6
      //   UTF32 U = (UTF16) U + 1;               
      //   UTF32 c = U << 16 | X;
      //   Char ch = (char)c;
      // -------------------------------------------------------------------+
      // Max Plane = 17 <<6           10001......                           |
      // HI_SURROGATE_START D800 1101100000000000 => 11 bits                |
      // LO_SURROGATE_START DC00 1101110000000000 => 10 bits                |
      // -------------------------------------------------------------------+
      // * Expression / Mask/note Var ToSurrogate           bits            |
      // - ---------------------- --- --------------------------------------+
      //   (UTF16) C       0xFFFF  X  xxxxxxxxxxxxxxxx      lo  16  0-15    |
      // 1 (1<<5)-1  C>>16 0x001F  U  ...........xxxxx      hi   5 16-20    |
      // 2 (U-1)                   W  ...........wwwww      hi   5 16-20    |
      // - ---------------------- --- -------------------   ----------------+
      // 3 (W << 6)                H  .....hhhhh......      hi   5 16-20    |
      //   (X >> 10)               H  ..........xxxxxx      hi   6 10-15    |
      //   HI_SURR_START   0xD800  H  11011hhhhhxxxxxx      hi  11 10-20    |
      // - ---------------------- --- -------------------   ----------------+
      // 4 (1 << 10)-1     0x03FF  L  ......xxxxxxxxxx      lo  10  0-9     |
      //   LO_SURR_START           L  110111xxxxxxxxxx      lo  10  0-9     |
      // -------------------------------------------------------------------+
      // * Expression / Mask/note Var ToSupplement          bits            |
      // ------------- ---------- --- --------------------------------------+
      // 5 (1<<6)-1  <<10  0x003F  X  .....mmmmmm.......... mid  6          |
      //   (1<<10)-1       0x03FF  X  .....mmmmmmllllllllll lo  10  0-9     |
      // 6 (1<<5)-1  H>>6  0x001F  U  ................uuuuu hi   5 16-20    |
      //   (U+1)<<16               U  uuuuu................ hi   5 16-20    |
      //   U<<16 | X               C  uuuuullllllllllllllll all     0-20    |
      // ====================================================================

      #endregion reference

      #region public const fields

      // allowUnassigned TODO
      public static bool  DefaultStrict      { get { return false;  } }
      public static int   FirstSentinel      { get { return 0xFFFE; } }
      public static int   HiSurrogateRange   { get { return 0x0400; } }
      public static int   LoSurrogateRange   { get { return 0x0400; } }
      public static int   MaxBMPCodePoint    { get { return 0xFFFF; } }
      public static int   MaxHiSurrogate     { get { return 0xDBFF; } }
      public static int   MaxLoSurrogate     { get { return 0xDFFF; } }
      public static int   MaxNonCharacter    { get { return 0xFDEF; } }
      public static int   MaxSupplementCode  { get { return 0x10FFFF; } }
      public static int   MinHiSurrogate     { get { return 0xD800; } }
      public static int   MinLoSurrogate     { get { return 0xDC00; } }
      public static int   MinNonCharacter    { get { return 0xFDD0; } }
      public static int   MinSupplementCode  { get { return 0x010000; } }
      public static CPG   MinValidCPGroup    { get { return CPG.CodePoint; } }
      public static int   NumSurrogates      { get { return 0x100000; } }
      public static char  ReplacementChar    { get { return '\uFFFD'; } }
      public static int   ReplacementInt     { get { return 0xFFFD; } }
      public static int[] ReplacementPair    { get { return repl_pair(); } }

      #endregion public const fields

      #region public static methods

      public static bool ContainsReplacement(string text)
      {
         for (int i = 0; i < text.Length; i++) {
            if (text[i] == ReplacementChar) {
               return true;
            }
         }
         return false;
      }

      public static int[] ReplacementIndices(string text)
      {
         List<int> list = new ();
         for (int i = 0; i < text.Length; i++) {
            if (text[i] == ReplacementChar) {
               list.Add(i);
            }
         }
         return list.ToArray();
      }

      public static bool ContainsSupplements(string text)
      {
         for (int i = 0; i < text.Length; i++) {
            if (IsSupplement(text[i])) {
               return true;
            }
         }
         return false;
      }

      public static int[] SupplementsIndices(string text)
      {
         List<int> list = new ();
         for (int i = 0; i < text.Length; i++) {
            if (IsSupplement(text[i])) {
               list.Add(i);
            }
         }
         return list.ToArray();
      }

      public static bool ContainsSurrogates(string text)
      {
         for (int i = 0; i < text.Length; i++) {
            if (IsSurrogate(text[i])) {
               return true;
            }
         }
         return false;
      }

      public static bool ContainsSurrogates(bool pairedOrUnpaired, string text)
      {
         if (!pairedOrUnpaired) {
            return ContainsSurrogates(text);
         }
         for (int i = 0; i < text.Length; i++) {
            if (IsHiSurrogate(text[i]) &&
               i < text.Length - 1 &&
               IsHiSurrogate(text[i + 1])) {
               return true;
            }
         }
         return false;
      }

      public static int[] SurrogateIndices(string text)
      {
         List<int> list = new ();
         for (int i = 0; i < text.Length; i++) {
            if (IsSurrogate(text[i])) {
               list.Add(i);
            }
         }
         return list.ToArray();
      }

      public static int[] SurrogateIndices(bool pairedOrUnpaired, string text)
      {
         if (!pairedOrUnpaired) {
            return SurrogateIndices(text);
         }
         List<int> list = new ();
         for (int i = 0; i < text.Length; i++) {
            if (IsHiSurrogate(text[i]) &&
               i < text.Length - 1 &&
               IsLoSurrogate(text[i + 1])) {
               list.Add(i);
               list.Add(i + 1);
            }
         }
         return list.ToArray();
      }

      public static CPG CPGroupOf(int value)
      {
         return CPGroupOf(DefaultStrict, value);
      }

      public static CPG CPGroupOf(bool strict, int value)
      {
         CPG grp = CPG.None;
         if (value >= MinHiSurrogate && value <= MaxLoSurrogate) {
            return value > MaxHiSurrogate
               ? CPG.LoSurrogate
               : CPG.HiSurrogate;
         }
         if (IsSentinel(value)) {
            grp = CPG.Sentinel;
         }
         else if (value > MaxSupplementCode || value < 0) {
            grp = value < 0
               ? CPG.Underflow
               : CPG.Overflow;
         }
         if (value >= MinNonCharacter && value <= MaxNonCharacter) {
            grp = CPG.NonCharacter;
         }
         if (grp != CPG.None || value == ReplacementInt) {
            return strict ? grp : CPG.NonCharacter;
         }
         return value > MaxBMPCodePoint
            ? CPG.Supplement
            : CPG.CodePoint;
      }

      public static CPG[] CPGroupValues(bool strict, string text)
      {
         CPG[] grps = new CPG[text.Length];
         for (int i = 0; i < text.Length; i++) {
            grps[i] = CPGroupOf(strict, text[i]);
         }
         return grps;
      }

      public static UCPInfo[] ErrorInfo(
         bool strict,
         string text)
      {
         CPG[] cpgs = CPGroupValues(strict, text);
         List<UCPInfo> list = new ();

         for (int i = 0; i < cpgs.Length; i++) {
            CPG cpg = cpgs[i];
            int value = (int)text[i];
            switch (cpg) {
               case CPG.CodePoint: break;
               case CPG.Supplement: break;
               case CPG.HiSurrogate:
                  if (i + 1 >= cpgs.Length ||
                     (i + 1 < cpgs.Length &&
                        cpgs[i + 1] != CPG.LoSurrogate)) {
                     list.Add(new UCPInfo(strict, value));
                  }
                  break;
               case CPG.NonCharacter:
                  if (strict) {
                     list.Add(new UCPInfo(strict, value));
                  }
                  break;
               case CPG.LoSurrogate:
                  list.Add(new UCPInfo(strict, value));
                  break;
               default: // CPG.NonCharacter, Sentinel Overflow & Underflow
                  value = strict ? value : UCSurrogator.ReplacementInt;
                  list.Add(new UCPInfo(strict, value));
                  break;
            }
         }
         return list.ToArray();
      }

      public static Tuple<int, int[], int, bool[], string> Convert(
         bool strict,
         object candidate)
      {
         int val;
         bool rep;
         int i = 0;
         int value = 0;
         Type type = candidate.GetType();
         int[] ints = Array.Empty<int>();
         bool[] bools = new bool[3] { true, true, true };
         StringBuilder sb = new (80);
         string format = "0x{0:X4} {1}";

         sb.Append(strict ? "IsStrict " : "loose  ");
         if (type.IsArray) {
            sb.AppendFormat("Array {0}[{1}]",
               type.UnderlyingSystemType.Name.PadRight(6, ' '),
               ((object[])candidate).Length);
            if (type.UnderlyingSystemType == typeof(Int32)) {
               ints = ((int[])candidate);
               while (i < ints.Length) {
                  val = ints[i++];
                  rep = val == ReplacementInt;
                  sb.AppendFormat(format, val, rep ? "-" : "+");
               }
               value = ToSupplement(strict, ints);
            }
         } else if (type == typeof(Int32)) {
            value = (int)candidate;
            sb.AppendFormat("CodePoint {0} 0x{1:X6}",
               type.Name.PadRight(6, ' '),
               value);
            ToSurrogates(value, out ints);
            while (i < ints.Length) {
               val = ints[i++];
               rep = val == ReplacementInt;
               sb.AppendFormat(format, val, rep ? "-" : "+");
            }
            candidate = ToSupplement(strict, ints);
         }
         int errs = i = 0;
         while (i < ints.Length) {
            if (ints[i++] == ReplacementInt) {
               errs++;
               if (i < bools.Length) { bools[i] = false; }
            }
         }
         if (value != (int)candidate || errs > 0) {
            bools[2] = false; errs++;
            sb.Append(" Round-trip failure");
         }
         Tuple<int, int[], int, bool[], string> tuple =
            new (
               value, ints, errs, bools, sb.ToString());
         return tuple;
      }

      #region public static Is* methods

      public static bool IsCodePoint(int value)
      {
         return IsCodePoint(value, true);
      }

      public static bool IsCodePoint(int value, bool allowSurrogates)
      {
         return IsCodePoint(value, allowSurrogates, true);
      }

      public static bool IsCodePoint(
         int value,
         bool allowSurrogates,
         bool bmpOnly)
      {
         if ((value >= MinNonCharacter && value <= MaxNonCharacter) ||
            value < 0 || value > MaxBMPCodePoint) {
            return false;
         }
         if (IsSurrogate(value)) {
            return allowSurrogates;
         }
         return !bmpOnly || value <= MaxBMPCodePoint;
      }

      public static bool IsHiSurrogate(char ch)
      {
         return IsHiSurrogate((int)ch);
      }

      public static bool IsHiSurrogate(int value)
      {
         return value >= MinHiSurrogate & value <= MaxHiSurrogate;
      }

      public static bool IsLoSurrogate(char ch)
      {
         return IsLoSurrogate((int)ch);
      }

      public static bool IsLoSurrogate(int value)
      {
         return value >= MinLoSurrogate & value <= MaxLoSurrogate;
      }

      public static bool IsNonCharacter(int? value)
      {
         return IsNonCharacter(value, false);
      }

      public static bool IsNonCharacter(int? value, bool bmpOnly)
      {
         bool result = null != value;
         if (null != value) {
            bool badVal = value < 0 || value > MaxSupplementCode;
            result = IsSentinel((int)value);
            if (badVal || result ||
               (value >= MinNonCharacter && value <= MaxNonCharacter)) {
               return true;
            }
            if (result && bmpOnly) {
               result = value <= MaxBMPCodePoint;
            }
         }
         return result;
      }

      public static bool IsReplacement(int value)
      {
         return value == ReplacementInt;
      }

      public static bool IsReplacement(char value)
      {
         return IsReplacement((int)value);
      }

      public static bool IsReplacement(int[] value)
      {
         return IsReplacement(DefaultStrict, value);
      }

      public static bool IsReplacement(bool strict, int[] surrogates)
      {
         if ((strict && surrogates.Length == 2) || surrogates.Length > 1) {
            return
               ReplacementInt == surrogates[0] &&
               ReplacementInt == surrogates[1];
         }
         return false;
      }

      public static bool IsReplacement(string text)
      {
         return IsReplacement(true, text);
      }

      public static bool IsReplacement(bool startsOrEndsWith, string text)
      {
         if (text.Length == 0) {
            return false;
         }
         char ch = startsOrEndsWith ? text[0] : text[^1];
         return ch == ReplacementChar;
      }

      public static bool IsSentinel(int value)
      {
         return (value & MaxBMPCodePoint) > ReplacementInt &&
            value >= 0 && value <= MaxSupplementCode;
      }

      public static bool IsSupplement(int value)
      {
         return
            value >= MinSupplementCode &&
            value <= MaxSupplementCode &&
            value != ReplacementInt;
      }

      public static bool IsSurrogate(char value)
      {
         return IsSurrogate((int)value);
      }

      public static bool IsSurrogate(int value)
      {
         return IsHiSurrogate(value) || IsLoSurrogate(value);
      }

      public static bool IsSurrogate(char hi, char lo)
      {
         return IsSurrogate((int)hi) && IsSurrogate((int)lo);
      }

      public static bool IsSurrogate(int hi, int lo)
      {
         return IsHiSurrogate(hi) || IsLoSurrogate(lo);
      }

      public static bool IsSurrogate(int[] surrogates)
      {
         return IsSurrogate(DefaultStrict, surrogates);
      }

      public static bool IsSurrogate(bool strict, int[] surrogates)
      {
         if ((strict && surrogates.Length != 2) || surrogates.Length < 2) {
            return false;
         }
         return surrogates.Length > 1 &&
            IsHiSurrogate(surrogates[0]) &&
            IsLoSurrogate(surrogates[1]);
      }

      public static bool IsUnicode(int value)
      {
         return value >= 0 && value <= MaxSupplementCode && 
            (value & MaxBMPCodePoint) < FirstSentinel &&
            (value < MinNonCharacter && value > MaxNonCharacter);
      }

      public static bool IsUnicode(int value, bool allowSentinels)
      {
         if (value <= -1 || value > MaxSupplementCode ||
            (value >= MinNonCharacter && value <= MaxNonCharacter)) {
            return false;
         }
         return
            (value & MaxBMPCodePoint) < FirstSentinel || allowSentinels;
      }

      #endregion public static Is* methods

      #region public static Stringize methods

      public static string Stringize(char value)
      {
         return Stringize(ToSurrogates(value));
      }

      public static string Stringize(char value, bool keepReplacements)
      {
         return Stringize(ToSurrogates(value), keepReplacements);
      }

      public static string Stringize(int value)
      {
         return Stringize(ToSurrogates(value));
      }

      public static string Stringize(int value, bool keepReplacements)
      {
         return Stringize(ToSurrogates(value), keepReplacements);
      }

      public static string Stringize(int[] surrogates)
      {
         return Stringize(surrogates, true);
      }

      public static string Stringize(int[] surrogates, bool keepReplacements)
      {
         return string.Format("{0}{1}",
            (surrogates[0] == ReplacementInt && !keepReplacements)
               ? string.Empty
               : surrogates[0].ToString(),
            (surrogates[1] == ReplacementInt && !keepReplacements)
               ? string.Empty
               : surrogates[1].ToString());
      }

      #endregion public static Stringize methods

      #region public static ToSupplement & ToSurrogates methods

      #region public static ToSupplement methods

      public static int ToSupplement(int hi, int lo)
      {
         ToSupplement(DefaultStrict, hi, lo, out int supplement);
         return supplement;
      }

      public static int ToSupplement(bool strict, int hi, int lo)
      {
         ToSupplement(strict, hi, lo, out int supplement);
         return supplement;
      }

      public static bool ToSupplement(
         bool strict,
         int hi,
         int lo,
         out int supplement)
      {
         bool fhi = IsHiSurrogate(hi);
         bool flo = IsLoSurrogate(lo);
         if (fhi && flo) {
            uint X = (uint)((hi & 0x3F) << 10 | lo & 0x3FF);
            uint U = (uint)((hi >> 6) & 0x1F) + 1;
            supplement = (int)(U << 16 | X);
         } else if (strict || fhi == flo) {
            supplement = ReplacementInt;
         } else if (fhi) {
            supplement = IsCodePoint(hi, false, true)
               ? hi
               : ReplacementInt;
         } else if (flo) {
            supplement = IsCodePoint(lo, false, true)
               ? lo
               : ReplacementInt;
         }
         else {
            supplement = ReplacementInt;
         }
         return supplement != ReplacementInt;
      }

      public static int ToSupplement(int[] surrogates)
      {
         return ToSupplement(DefaultStrict, surrogates);
      }

      public static int ToSupplement(bool strict, int[] surrogates)
      {
         ToSupplement(strict, surrogates, out int supplement);
         return supplement;
      }

      public static bool ToSupplement(
         bool strict,
         int[] surrogates,
         out int supplement)
      {
         int i = 0;
         if ((strict && surrogates.Length != 2) || surrogates.Length < 2) {
            while (i < surrogates.Length) {
               if (IsSupplement(surrogates[i])) {
                  supplement = surrogates[i];
                  return true;
               }
               i++;
            }
            supplement = ReplacementInt;
            return false;
         }
         int hi = surrogates[0];
         int lo = surrogates[1];
         bool fhi = IsHiSurrogate(hi);
         bool flo = IsLoSurrogate(lo);
         if (!(fhi && flo)) {
            if (strict || !(fhi || flo)) {
               supplement = ReplacementInt;
            } else if (fhi) {
               supplement = IsCodePoint(hi, false, true)
                  ? hi
                  : ReplacementInt;
            } else {
               supplement = IsCodePoint(lo, false, true)
                  ? lo
                  : ReplacementInt;
            }
         } else {
            uint X = (uint)((hi & 0x3F) << 10 | lo & 0x3FF);
            uint U = (uint)((hi >> 6) & 0x1F) + 1;
            supplement = (int)(U << 16 | X);
         }
         return supplement != ReplacementInt;
      }

      #endregion public static ToSupplement methods

      #region public static ToSurrogates methods

      public static int[] ToSurrogates(char ch)
      {
         ToSurrogates((int)ch, out int[] surrogates);
         return surrogates;
      }

      public static bool ToSurrogates(char ch, out int[] surrogates)
      {
         return ToSurrogates((int)ch, out surrogates);
      }

      public static int[] ToSurrogates(int value)
      {
         ToSurrogates(value, out int[] surrogates);
         return surrogates;
      }

      public static bool ToSurrogates(
         int value,
         out int[] surrogates)
      {
         if (!IsSupplement(value)) {
            surrogates = new int[] { ReplacementInt, ReplacementInt };
            return false;
         }
         int H = MinHiSurrogate | ((ushort)value >> 10) |
            ((((value >> 16) & 0x1F) - 1) << 6);
         int L = MinLoSurrogate | ((ushort)value & 0x3FF);
         surrogates = new int[] { H, L, value };
         return true;
      }

      #endregion public static ToSurrogates methods

      #endregion public static ToSupplement & ToSurrogates methods

      #endregion public static methods

      #region private static methods

      private static int[] repl_pair()
      {
         return new int[2] { 0xFFFD, 0xFFFD };
      }

      #endregion private static methods
   }

   #endregion UCSurrogator
}
