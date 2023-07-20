// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// UCPlane.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TypeHelp
{
   #region enumerations

   /// <summary>
   /// The u c p field.
   /// </summary>
   public enum UCPField
   {
      Blocks,
      Chars,
      First,
      Final,
      Id,
      Label,
      Name,
      PFlag,
      Plane,
      Size,
   }

   #endregion enumerations

   #region UCPlane

   [StructLayout(LayoutKind.Sequential)]
   public readonly struct UCPlane
   {
      #region public fields

      public readonly int Blocks;
      public readonly int Chars;
      public readonly int First;
      public readonly int Final;
      public readonly int Id;
      public readonly string Label;
      public readonly string Name;
      public readonly UCPF PFlag;
      public readonly UCP Plane;
      public readonly int Size;

      #endregion public fields

      #region constructor

      public UCPlane(UCP ucp, int blocks, int chars)
      {
         try {
            Exception? e = check_params(ucp, blocks, chars);
            if (e != null) {
               throw e;
            }
         } catch (Exception inner) {
            throw new TypeInitializationException(
               typeof(UCPlane).FullName, inner);
         }
         this.Id = (int)ucp;
         this.PFlag = Convert(ucp);
         this.Plane = ucp;
         if (ucp == UCP.NON) {
            this.Blocks = blocks;
            this.Chars = chars;
            this.Name = UCPNames[(int)ucp];
            this.Label = UCPLabels[(int)ucp];
            this.First = (int)ucp << 16;
            this.Size = 0xFFFF; // 65535
         } else {
            this.Name = InvalidPlaneName;
            this.Label = InvalidPlaneLabel;
            this.Blocks = this.Chars = this.First = this.Size = 0;
         }
         this.Final = this.First + this.Size;
      }

      #endregion constructor

      #region public static fields

      public static string InvalidPlaneLabel {
         get { return "Non-Unicode values"; }
      }

      public static string InvalidPlaneName {
         get { return "Invalid"; }
      }

      public static UCPlane NonUCPlane {
         get { return new UCPlane(UCP.NON, 0, 0); }
      }

      public static int NumCodePlanes {
         get { return 17; }
      }

      public static UCPF ValidPlanes {
         get { return (UCPF)0x1FFFF; }
      }

      public static int UCPSentinel {
         get { return (int)UCPF.NON; }
      }

      public static readonly string[] UCPLabels = new string[] {
         "BMP", "SMP", "SIP", "TIP",
         "U+4xxxx", "U+5xxxx", "U+6xxxx", "U+7xxxx",
         "U+8xxxx", "U+9xxxx", "U+Axxxx", "U+Bxxxx",
         "U+Cxxxx", "U+Dxxxx", "SSP", "SPUA-A", "SPUA-B",
      };

      public static readonly string[] UCPNames = new string[] {
         "Basic Multilingual Plane U+xxxx",
         "Supplementary Multilingual Plane U+1xxxx",
         "Supplementary Ideographic Plane U+2xxxx",
         "Tertiary Ideographic Plane U+3xxxx",
         "Unassigned U+4xxxx", "Unassigned U+5xxxx", "Unassigned U+6xxxx",
         "Unassigned U+7xxxx", "Unassigned U+8xxxx", "Unassigned U+9xxxx",
         "Unassigned U+Axxxx", "Unassigned U+Bxxxx", "Unassigned U+Cxxxx",
         "Unassigned U+Dxxxx",
         "Supplementary Special-purpose Plane U+Exxxx",
         "Supplementary Private Use Area Plane A U+Fxxxx",
         "Supplementary Private Use Area Plane B U+10xxxx",
      };

      public static readonly UCPlane[] UCPlanes = new UCPlane[] {
         // ======== Plane  Blocks  Chars
         new UCPlane(UCP.BMP,  164, 55632),
         new UCPlane(UCP.SMP,  145, 22982),
         new UCPlane(UCP.SIP,    6, 60872),
         new UCPlane(UCP.TIP,    1,  4939),
         new UCPlane(UCP.UN4, 0, 0), new UCPlane(UCP.UN5, 0, 0),
         new UCPlane(UCP.UN6, 0, 0), new UCPlane(UCP.UN7, 0, 0),
         new UCPlane(UCP.UN8, 0, 0), new UCPlane(UCP.UN9, 0, 0),
         new UCPlane(UCP.UNA, 0, 0), new UCPlane(UCP.UNB, 0, 0),
         new UCPlane(UCP.UNC, 0, 0), new UCPlane(UCP.UND, 0, 0),
         new UCPlane(UCP.SSP, 2, 337),
         new UCPlane(UCP.SPA, 1, 0), new UCPlane(UCP.SPB, 1, 0),
      };

      #endregion public static fields

      #region public static methods

      public static UCP CodePlaneOf(int codePoint)
      {
         codePoint >>= 16;
         return codePoint < 0 && codePoint > 0x10FFFF
            ? UCP.NON
            : (UCP)(codePoint >> 16);  // 0 .. UCPlanes.length - 1
      }

      public static bool CodePlaneOf(int value, out UCP ucp)
      {
         ucp = CodePlaneOf(value);
         return IsUCPlane(ucp);
      }

      public static UCP Convert(UCPF flag)
      {
         return Convert(true, flag);
      }

      public static bool Convert(UCPF flag, out UCP ucp)
      {
         ucp = Convert(flag);
         return ucp != UCP.NON;
      }

      public static UCP Convert(bool ascending, UCPF flag)
      {
         if (UCPF.NON == flag) {
            return UCP.NON;
         }
         UCP ucp = ascending ? UCP.BMP : UCP.SPB;
         int bit = ascending ? (int)UCPF.BMP : (int)UCPF.SPB;
         if (ascending) {
            while (((int)flag & bit) == 0 && bit <= (int)UCPF.SPB) {
               bit <<= 1;
               ucp++;
            }
            return bit <= (int)UCPF.SPB ? ucp : UCP.NON;
         }
         while (((int)flag & bit) == 0) {
            bit >>= 1;
            ucp--;
         }
         return ucp;
      }

      public static bool Convert(bool ascending, UCPF flag, out UCP ucp)
      {
         ucp = Convert(ascending, flag);
         return IsUCPlane(ucp);
      }

      public static UCPF Convert(UCP ucp)
      {
         return IsUCPlane(ucp) ? (UCPF)(1 << (int)ucp) : UCPF.NON;
      }

      public static bool Convert(UCP ucp, out UCPF flag)
      {
         bool isPlane = IsUCPlane(ucp);
         flag = isPlane ? Convert(ucp) : UCPF.NON;
         return isPlane;
      }

      public static UCPlane[] GetUCPlanes(UCP[] selections)
      {
         List<UCPlane> planes = new ();
         if (selections.Length > 0) {
            for (int i = 0; i < selections.Length; i++) {
               if (selections[i] != UCP.NON) {
                  planes.Add(UCPlanes[(int)selections[i]]);
               }
            }
            planes.TrimExcess();
         }
         return planes.ToArray();
      }

      public static UCPlane[] GetUCPlanes(UCPF selections)
      {
         List<UCPlane> planes = new ();

         selections = Sanitize(selections);
         if ((int)selections > 0) {
            foreach (UCPF flag in Enum.GetValues(typeof(UCPF))) {
               if (selections.HasFlag(flag)) {
                  planes.Add(UCPlanes[(int)Convert(flag)]);
               }
            }
            planes.TrimExcess();
         }
         return planes.ToArray();
      }

      public static bool HasFlags(bool exact, UCPF value, UCPF flags)
      {
         if (value == flags) return true;
         return exact ? (value & flags) == flags : (value & flags) != 0;
      }

      public static bool IsInCodePlane(int value, UCP ucp)
      {
         return IsUCPlane(ucp) && ucp == CodePlaneOf(value);
      }

      public static bool IsCodePlane(int value)
      {
         return Enum.IsDefined(typeof(UCP), value) && (UCP)value != UCP.NON;
      }

      public static bool IsUCPlane(UCP ucp)
      {
         return ucp != UCP.NON;
      }

      public static bool IsUCPlane(UCPF ucpf)
      {
         return ucpf != UCPF.NON;
      }

      public static bool IsUCPlane(bool allowNON, int value)
      {
         if (!allowNON &&
            (value == (int)UCP.NON || value == UCPSentinel)) {
            return false;
         }
         return
            Enum.IsDefined(typeof(UCP), value) ||
            Enum.IsDefined(typeof(UCPF), value);
      }

      public static object[] QueryField(UCPField field)
      {
         UCPlane[] p = UCPlanes;
         object[] list = new object[1 + p.Length];
         int i = 0;
         //
         list[i++] = field;
          switch (field) {
            case UCPField.Label:
               while (i < p.Length) list[i++] = (string)p[i].Label.Clone();
               break;
            case UCPField.Name:
               while (i < p.Length) list[i++] = (string)p[i].Name.Clone();
               break;
            case UCPField.Blocks: while (i < p.Length) list[i++] = p[i].Blocks;
               break;
            case UCPField.Chars: while (i < p.Length) list[i++] = p[i].Chars;
               break;
            case UCPField.First: while (i < p.Length) list[i++] = p[i].First;
               break;
            case UCPField.Final: while (i < p.Length) list[i++] = p[i].Final;
               break;
            case UCPField.Id: while (i < p.Length) list[i++] = p[i].Id;
               break;
            case UCPField.PFlag: while (i < p.Length) list[i++] = p[i].PFlag;
               break;
            case UCPField.Plane: while (i < p.Length) list[i++] = p[i].Plane;
               break;
            case UCPField.Size: while (i < p.Length) list[i++] = p[i].Size;
               break;
         }
         return list;
      }

      public static UCPF Sanitize(UCPF ucpf)
      {
         return (UCPF)((int)ucpf & ~(int)ValidPlanes);
      }

      public static UCPF Set(bool set, UCPF ucpf, UCPF flags)
      {
         return set ? ucpf | flags : ucpf & ~flags;
      }

      public static UCPF Set(bool set, UCPF ucpf, UCP ucp)
      {
         UCPF flag = Convert(ucp);
         return set ? ucpf | flag : ucpf & ~flag;
      }

      public static UCPlane UCPlaneOf(int codePoint)
      {
         UCP ucp = CodePlaneOf(codePoint);
         return UCPlaneOf(ucp);
      }

      public static UCPlane UCPlaneOf(UCP ucp)
      {
         return ucp != UCP.NON ? UCPlanes[(int)ucp] : NonUCPlane;
      }

      #endregion public static methods

      #region private static methods

      private static Exception? check_params(UCP ucp, int blocks, int chars)
      {
         bool badChars = chars < 0 || chars > 0x10000;

         if (badChars || blocks < 0 ||
            (blocks > chars && ucp < UCP.SPA)) {
            string format = "CodePoint is {0} the limit.";
            string[] stats = new string[] { "below", "above" };
            if (badChars) {
               return new ArgumentOutOfRangeException(nameof(chars), chars,
                  string.Format(format, stats[chars < 0 ? 0 : 1]));
            }
            return new ArgumentOutOfRangeException(nameof(blocks), blocks,
               string.Format(format, stats[blocks < 0 ? 0 : 1]));
         }
         if (!Enum.IsDefined(typeof(TypeHelp.UCP), ucp)) {
            return new InvalidEnumArgumentException(nameof(ucp),
               (int)ucp, typeof(UCP));
         }
         return null;
      }

      #endregion private static methods
   }

   #endregion UCPlane
}
