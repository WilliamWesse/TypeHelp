// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// UCInfo.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Runtime.InteropServices;

namespace TypeHelp
{
   #region IUCSInfo

   public interface IUCSInfo {

      bool    Discard    { get; set; }
      CPG     Group      { get; }
      CPG[]   Groups     { get; }
      UCP     Plane      { get; }
      bool    Strict     { get; }
      int     Supplement { get; }
      int[]   Surrogates { get; }
      object? Tag        { get; set; }
      bool    Valid      { get; set; }
      int     Value      { get; }
   }

   #endregion IUCSInfo

   #region UCInfo

   [StructLayout(LayoutKind.Sequential)]
   public struct UCInfo : IUCSInfo
   {
      #region public readonly instance fields

      public bool Discard
      {
         get { return discard; }
         set { discard = value; }
      }

      public CPG Group
      {
         get { return groups[(int)CPGI.CodePoint]; }
      }

      public CPG[] Groups
      {
         get { return groups; }
      }
      public bool IsCodePoint { get { return Group == CPG.CodePoint; } }
      public bool IsReplacement { get { return Group == CPG.Replacement; } }
      public bool IsSupplement { get { return Group == CPG.Supplement; } }
      public bool IsSurrogate { get { return Group == CPG.Surrogate; } }
      public UCP Plane { get { return plane; } }
      public bool Strict { get { return strict; } }
      public int Supplement { get { return supplement; } }
      public int[] Surrogates { get { return surrogates; } }
      public object? Tag { get { return tag; } set { tag = value; } }
      public bool Valid { get { return valid; } set { valid = value; } }
      public int Value { get { return value; } }

      #endregion public readonly instance fields

      #region constructors

      #endregion constructors

      #region public static Factory methods

      public static UCInfo Create(bool strict, int value)
      {
         return create_ucinfo(strict, value);
      }

      public static UCInfo Create(bool strict, int lo, int hi)
      {
         return create_ucinfo(strict, new int[] { hi, lo });
      }

      public static UCInfo Create(bool strict, int[] surrogates)
      {
         return create_ucinfo(strict, surrogates);
      }

      public bool Is(CPGI which, CPG group)
      {
         return Groups[(int)which] == group;
      }

      #endregion public static Factory methods

      #region private static Factory methods

      private UCInfo(
         bool  strict,
         int   value,
         CPG[] groups,
         int[] surrogates,
         int   supplement,
         UCP   plane,
         bool  valid)
      {
         this.tag = null;
         this.value = value;
         this.strict = strict;
         this.groups = groups;
         this.discard = false;
         this.surrogates = surrogates;
         this.supplement = supplement;
         this.plane = plane;
         this.valid = valid;
      }

      private static UCInfo create_ucinfo(bool strict, object value)
      {
         Type? type = TypeParser.GetType(value);
         if (null == value || null == type) {
            throw new ArgumentNullException(nameof(value));
         }
         string deco = type.Name.StartsWith("I") ? "an" : "a";
         string format = "The '{0} {1}' parameter is {2} {3}; expected an {4}";
         string message = "";
         Type? eType = type.IsArray ? type.GetElementType() : type;
         if (null != eType && eType != typeof(int)) {
            message = string.Format(format,
               typeof(object).Name, nameof(value),
               deco, eType.Name, typeof(int).Name);
         }
         if (message.Length > 0) {
            throw new ArgumentException(message, nameof(value));
         }
         int[] surrogates = type.IsArray
            ? (int[])value
            : UCSurrogator.ToSurrogates((int)value);
         int supplement = UCSurrogator.ToSupplement(surrogates);
         CPG[] groups = new CPG[] {
            UCSurrogator.CPGroupOf(strict, type.IsArray ? supplement : (int)value),
            UCSurrogator.CPGroupOf(strict, supplement),
            UCSurrogator.CPGroupOf(strict, surrogates[0]),
            UCSurrogator.CPGroupOf(strict, surrogates[1]),
         };
         bool valid =
            groups[(int)CPGI.HiSurrogate] == CPG.HiSurrogate &&
            groups[(int)CPGI.LoSurrogate] == CPG.LoSurrogate;

         if (valid) {
            groups[(int)CPGI.CodePoint] = CPG.Surrogate;
            foreach (CPG grp in groups) {
               if (grp < UCSurrogator.MinValidCPGroup) {
                  valid = false;
               }
            }
         }
         UCP plane = valid
            ? (UCP)(supplement >> 16)
            : UCP.NON;
         int val = type.IsArray ? supplement : (int)value;
         return new UCInfo(
            strict, val, groups, surrogates,
            supplement, plane, valid);
      }


      #endregion private static Factory methods

      #region private instance fields

      private object? tag;
      private bool discard;
      private bool valid;

      #region private readonly instance fields

      private readonly UCP   plane;
      private readonly CPG[] groups;
      private readonly bool  strict;
      private readonly int   supplement;
      private readonly int[] surrogates;
      private readonly int   value;

      #endregion private readonly instance fields

      #endregion private instance fields
   }

   #endregion UCInfo
}
