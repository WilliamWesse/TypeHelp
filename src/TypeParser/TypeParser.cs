// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// TypeParser.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Collections; // Everywhere?
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TypeHelp
{
   #region enumerations

   /// <summary>
   /// The DataType typeCoex.
   /// </summary>
   public enum TypeCoex
   {
      // ******************************* Value     Size
      Empty      = TypeCode.Empty,    //     0        0
      Object     = TypeCode.Object,   //     1        ?
      DBNull     = TypeCode.DBNull,   //     2        0
      Boolean    = TypeCode.Boolean,  //     3        1
      Char       = TypeCode.Char,     //     4        ?
      SByte      = TypeCode.SByte,    //     5        1
      Byte       = TypeCode.Byte,     //     6        1
      Int16      = TypeCode.Int16,    //     7        2
      UInt16     = TypeCode.UInt16,   //     8        2
      Int32      = TypeCode.Int32,    //     9        4
      UInt32     = TypeCode.UInt32,   //    10        4
      Int64      = TypeCode.Int64,    //    11        8
      UInt64     = TypeCode.UInt64,   //    12        8
      Single     = TypeCode.Single,   //    13        4
      Double     = TypeCode.Double,   //    14        8
      Decimal    = TypeCode.Decimal,  //    15       16
      DateTime   = TypeCode.DateTime, //    16        8
      Nullity,                        //    17        0
      String     = TypeCode.String,   //    18        ?
      Base64,                         //    19        ?
      // BinHex,   TODO ?  Mac .hex https://en.wikipedia.org/wiki/BinHex
      // BinHcx,   TODO ?  Compact BinHex (.hcx)
      // BinHqx,   TODO ?  BinHex 4 (.hqx)
      // Uuencode, TODO ?  https://en.wikipedia.org/wiki/Uuencoding
      SID,                            //    20        ?
      Guid,                           //    21       16
      UUID,                           //    22       16 https://www.rfc-editor.org/rfc/rfc4122
      Uri,                            //    23        ?
      Enum,                           //    24  1|2|4|8 UnderlyingSystemType
      TimeSpan,                       //    25        8
      IntPtr,                         //    26      4|8
      UIntPtr,                        //    27      4|8
      Complex,                        //    28       16
      BigInteger,                     //    29        ?
      BitArray,                       //    30        ?
      Tuple,                          //    31        ?
      ValueTuple,                     //    32        ?
      // DataType TODO
   }

   /// <summary>
   /// The typed str.
   /// </summary>
   public enum TypedStr
   {
      String = TypeCoex.String, // str  18     ?
      Guid   = TypeCoex.Guid,   // str? 19    16
      UUID   = TypeCoex.UUID,   // str? 20    16  https://www.rfc-editor.org/rfc/rfc4122
      SID    = TypeCoex.SID,    // str? 21     ?
      Uri    = TypeCoex.Uri,    // str? 22     ?
      Base64 = TypeCoex.Base64, // str? 23     ?
      // TODO BinHex,           // Mac .hex https://en.wikipedia.org/wiki/BinHex
      // TODO BinHcx,           // Compact BinHex (.hcx)
      // TODO BinHqx,           // BinHex 4 (.hqx)
      // TODO Uuencode          // https://en.wikipedia.org/wiki/Uuencoding
   }

   /// <summary>
   /// TCO Enumeration
   /// <para>Flags defining status and options to parse
   /// <see cref="System.Type"/> and values for
   /// <see cref="System.TypeCode"/> and <seealso cref="TypeCoex"/>
   /// enmeration values.
   /// </para>
   /// </summary>
   [Flags]
   public enum TCO : uint
   {
      //***********************************************************************
      // Options         0x00000000
      //-----------------------------------------------------------------------
      None             = 0,
      CustomOptions    = 0xF0000000,
      FailOptions      = 0x0000000F,
      FilterOptions    = 0x00000070,
      MatchOptions     = 0x00001F80,
      SortOptions      = 0x0000E000,
      //
      FailToEmpty      = 0x00000001, // set: 0, false, Null, "", ...
      FailToInvalid    = 0x00000002, // NaN,  other coexFlag-specific HasValue
      FailToMaximum    = 0x00000004, // coexFlag.MaxValue
      FailToMinimum    = 0x00000008, // coexFlag.MinValue
      //
      FilterInvert     = 0x00000010, // TODO
      FilterNoSigned   = 0x00000020, // TODO
      FilterNoStrings  = 0x00000040, // TODO
      //
      MatchAltFormats  = 0x00000080, // Complex: a + b[i, j if set]
      MatchAutoType    = 0x00000100, // set: near coexFlag match, 32-bit.
      MatchPrefer32Bit = 0x00000200, // set: prefer Int32, UInt32
      MatchDiscard     = 0x00000400, // set: discard, otherwise keep
      MatchMaximalType = 0x00000800, // example: 1 -> System.Int64
      MatchMinimalType = 0x00001000, // example: 1 -> System.SByte
      MatchDefaults    = MatchAltFormats  | MatchStrict,
      MatchStrict      = MatchPrefer32Bit | MatchAutoType,
      //
      // Optimize lookups under multiple DataType subsets; Well Knowns, etc..
      SortByNumerics   = 0x00002000, // see TypeCoexNumericPriority
      SortByTypeSize   = 0x00004000, // see TypeCoexSizePriority
      SortByTypeUsage  = 0x00008000, // see TypeCoexUsagePriority
      //
      //***********************************************************************
      // IsStrict          0x00000000
      //-----------------------------------------------------------------------

      //***********************************************************************
      // Custom      0xF0000000
      //-----------------------------------------------------------------------
      Custom1         = 0x10000000, // bit: 56
      Custom2         = 0x20000000, // bit: 57
      Custom3         = 0x40000000, // bit: 58
      Custom4         = 0x80000000, // bit: 59
   }

   /// <summary>
   /// The t c f.
   /// </summary>
   [Flags]
   public enum TCF : ulong // TypeCode/HasCoex Flags
   {
      //=======================================================================
      // Well Knowns 0x000000001FFFFFFF
      //-----------------------------------------------------------------------
      None             = 0,                  // Value    Size
      Empty            = 0x0000000000000001, //     0       0
      Object           = 0x0000000000000002, //     1       ?
      DBNull           = 0x0000000000000004, //     2       0
      Boolean          = 0x0000000000000008, //     3       1
      Char             = 0x0000000000000010, //     4     2 ?
      SByte            = 0x0000000000000020, //     5       1
      Byte             = 0x0000000000000040, //     6       1
      Int16            = 0x0000000000000080, //     7       1
      UInt16           = 0x0000000000000100, //     8       1
      Int32            = 0x0000000000000200, //     9       4
      UInt32           = 0x0000000000000400, //    10       4
      Int64            = 0x0000000000000800, //    11       8
      UInt64           = 0x0000000000001000, //    12       8
      Single           = 0x0000000000002000, //    13       4
      Double           = 0x0000000000004000, //    14       8
      Decimal          = 0x0000000000008000, //    15      16
      DateTime         = 0x0000000000010000, //    16       8
      Nullity          = 0x0000000000020000, //    17       0
      String           = 0x0000000000040000, //    18       ?
      Guid             = 0x0000000000080000, //    19      16
      UUID             = 0x0000000000100000, //    20      16
      SID              = 0x0000000000200000, //    21       ?
      Uri              = 0x0000000000400000, //    22       ?
      Base64           = 0x0000000000800000, //    23       ?
      Enum             = 0x0000000001000000, //    24 1|2|4|8
      TimeSpan         = 0x0000000002000000, //    25       8
      IntPtr           = 0x0000000004000000, //    26     4|8
      UIntPtr          = 0x0000000008000000, //    27     4|8
      Complex          = 0x0000000010000000, //    28      16
      BigInteger       = 0x0000000020000000, //    29       1
      BitArray         = 0x0000000040000000, //    30       1
      Tuple            = 0x0000000080000000, //    31       ?
      ValueTuple       = 0x0000000100000000, //    32       ?
      // DataType TODO
      //***********************************************************************
      // Options         0x00FFFF0000000000
      //-----------------------------------------------------------------------
      CustomOptions    = 0x0F00000000000000,
      FailOptions      = 0x00000F0000000000,
      FilterOptions    = 0x0000700000000000,
      MatchOptions     = 0x001F800000000000,
      SortOptions      = 0x00E0000000000000,
      //
      FailToEmpty      = 0x0000010000000000, // set: 0, false, Null, "", ...
      FailToInvalid    = 0x0000020000000000, // NaN,  other coexFlag-specific HasValue
      FailToMaximum    = 0x0000040000000000, // coexFlag.MaxValue
      FailToMinimum    = 0x0000080000000000, // coexFlag.MinValue
      //
      FilterInvert     = 0x0000100000000000, // TODO
      FilterNoSigned   = 0x0000200000000000, // TODO
      FilterNoStrings  = 0x0000400000000000, // TODO
      //
      MatchAltFormats  = 0x0000800000000000, // Complex: a + b[i, j if set]
      MatchAutoType    = 0x0001000000000000, // set: near coexFlag match, 32-bit.
      MatchPrefer32Bit = 0x0002000000000000, // set: prefer Int32, UInt32
      MatchDiscard     = 0x0004000000000000, // set: discard, otherwise keep
      MatchMaximalType = 0x0008000000000000, // example: 1 -> System.Int64
      MatchMinimalType = 0x0010000000000000, // example: 1 -> System.SByte
      MatchDefaults    = MatchAltFormats  | MatchStrict,
      MatchStrict      = MatchPrefer32Bit | MatchAutoType,
      // Optimize lookups under multiple types; DataType Subsets, Well Knowns, etc.
      SortByNumerics   = 0x0020000000000000, // see TypeCoexNumericPriority
      SortByTypeSize   = 0x0040000000000000, // see TypeCoexBySize
      SortByTypeUsage  = 0x0080000000000000, // see TypeCoexUsagePriority


      // TODO IsStrict (multiple flags)
      //***********************************************************************
      // Custom      0x0F00000000000000
      //-----------------------------------------------------------------------
      Custom1      = 0x0100000000000000, // bit: 56
      Custom2      = 0x0200000000000000, // bit: 57
      Custom3      = 0x0400000000000000, // bit: 58
      Custom4      = 0x0800000000000000, // bit: 59
      //***********************************************************************
      // Status      0xF000000000000000
      //-----------------------------------------------------------------------
      Processed    = 0x1000000000000000, // MBZ before operation, MB1 after.
      Success      = 0x2000000000000000,
      Warning      = 0x4000000000000000, // R-Severity 1: Severe  0: !NTSTATUS
      Error        = 0x8000000000000000, // S-Severity 1: Failure 0: Success
      ErrorMask    = Success | Fatal,
      Fatal        = Warning | Error,
      Defaulted    = Warning | Success,
      //=======================================================================
      // Masks
      //-----------------------------------------------------------------------
      StatusMask   = 0xF000000000000000,
      Custom       = 0x0F00000000000000,
      Options      = 0x00FFFF0000000000,
      Features     = 0x000000F000000000,
      Reserved     = 0x0000000E00000000,
      TypeCodes    = 0x000000000005FFFF,
      MaxCoex      = 0x0000000100000000,
      CoexOnly     = 0x00000001FFFA0000,
      TypeCoexs    = 0x00000001FFFFFFFF,
      //=======================================================================
      // Items  - Output    Note: MultiValued +Char: Unicode Surrogates
      //-----------------------------------------------------------------------
      HasElement   = 0x0000001000000000, // ptrs, refs: see DataType.HasElementType
      IsClipped    = 0x0000002000000000, // Numerics: Over/Underflow, Infinity
      IsNaN        = 0x0000004000000000, // Precisions, Stringish
      SizeVariant  = 0x0000008000000000, // see: SizeVariants
      //=======================================================================
      // Subsets
      //=======================================================================
      // Well Known DataType Subsets
      //-----------------------------------------------------------------------
      AllTypes     = TypeCoexs,
      Canonicals   = Code_Integers   | Code_Naturals,
      Floats       = Code_Floats     | Coex_Floats,
      Logicals     = Code_Logicals   | Coex_Logicals,
      Integers     = Code_Integers   | Coex_Integers,
      IntegersExt  = String,
      LargeInts    = Code_LargeInts  | Coex_LargeInts,
      MultiValued  = Char | DateTime | TimeSpan | Complex | Enum | HasElement,
      Naturals     = Code_Naturals   | Coex_Naturals,
      Nullish      = Code_Nullish    | Coex_Nullish,
      Numerics     = Code_Numerics   | Coex_Numerics,
      Pointers     = IntPtr | UIntPtr,
      Precisions   = Code_Precision  | Coex_Precision,
      Primitives   = (Code_Numerics  | Boolean) & ~(ulong)DateTime,
      Signed       = Integers        | Strings | BitArray | Precisions,
      SizeVariants = BigInteger      | Strings | BitArray | Pointers,
      Strings      = Code_Strings    | Coex_Strings,
      Stringish    = Strings & ~(ulong)Char,
      Unsigned     = Naturals        | UIntPtr,
      WellKnowns   = AllTypes,
      //=======================================================================
      // Loop limits                         Search     Result
      //-----------------------------------  ---------  -----------------------
      MaxCodeBit    = String,            //  Inclusive  Inclusive
      MaxCodeBitIdx = MaxCodeBit >> 1,   //  Inclusive  Inclusive
      MaxCoexBit    = Nullity,           //  Inclusive  Inclusive
      MaxCoexBitIdx = MaxCoexBit >> 1,   //  Exclusive  Exclusive  see RFC_Reserved
      //=======================================================================
      // Common Parameters
      //-----------------------------------------------------------------------
      Auto          = Everything | MatchAutoType,
      Everything    = Options    | WellKnowns,
      OptsAndStatus = Options    | StatusMask,
      SanitizeIn    = Everything,
      SanitizeOut   = WellKnowns,
      //***********************************************************************
      // Integers
      //-----------------------------------------------------------------------
      Small_Integers  = SByte  |  Int16,
      Medium_Integers = Int32  |  Int64,
      Large_Integers  = Int64,
      Small_Naturals  = Byte   |  UInt16,
      Medium_Naturals = UInt32 |  UInt64,
      Large_Naturals  = UInt64,
      //
      Functor_Ints    = String_Ints  | DateTime,
      Functor_Nats    = Char         | DateTime,
      Other_Ints      = Functor_Ints | Pointers | BigInteger,
      String_Ints     = String,
      String_Nats     = Char,
      String_Nums     = String_Ints  | String_Nats,
      //***********************************************************************
      // TypeCode: Floats, Integers, LargeInts, Logicals, Naturals, Nullity,
      //           Numerics, Precision, Strings
      //-----------------------------------------------------------------------
      Code_Floats     = Single  | Double,
      Code_Integers   = Small_Integers  | Medium_Integers | Large_Integers,
      Code_LargeInts  = Large_Integers,
      Code_LargeNats  = Large_Naturals,
      Code_Logicals   = Boolean,
      Code_Naturals   = Small_Naturals  | Medium_Naturals | Large_Naturals,
      Code_Nullish    = DBNull  | Empty | Object,
      Code_Numerics   = Code_Integers   | Code_Naturals   | Code_Floats,
      Code_Precision  = Decimal | Code_Floats,
      Code_Strings    = String  | Char,
      //***********************************************************************
      // HasCoex: Floats, Integers, LargeInts, Logicals, Naturals, Nullity,
      //           Numerics, Precision, Strings
      //-----------------------------------------------------------------------
      Coex_Floats     = Code_Floats   | Decimal | Complex,
      Coex_Integers   = Code_Integers | IntPtr  | BigInteger,
      Coex_LargeInts  = IntPtr,
      Coex_LargeNats  = UIntPtr,
      Coex_Logicals   = None,
      Coex_Naturals   = UIntPtr | Char | DateTime,
      Coex_Nullish    = Nullity,
      Coex_Numerics   = Coex_Integers  | Coex_Naturals  | Coex_Floats,
      Coex_Precision  = Coex_Floats    | String,
      Coex_Strings    = Base64  | Guid | SID | Uri | UUID,
   }

   /// <summary>
   /// The t c sizes.
   /// </summary>
   enum TCSizes
   {
      Empty      =   0, // IsEmpty
      Octet      =   1, // DBNull,  Boolean, Byte,   SByte
      Word       =   2, // Char,    Int16,   UInt16
      DWord      =   4, // Int32,   UInt32,  Single
      QWord      =   8, // Int64,   UInt64,  Double, DateTime
      Para       =  16, // RFC4122, Complex
      Object     =  -1, // <----------------------------------------------------- TODO
      String     =  -2, // string_value.length
      Pointer    =  -3, // [U]IntPtr: RunProps.Is64BitOS ? QWord : DWord
      Guid       =  -4, //      19      16
      UUID       =  -5, // str? 20      16  https://www.rfc-editor.org/rfc/rfc4122
      SID        =  -6, // str? 21       ?
      Uri        =  -7, //      22       ?
      Base64     =  -8, // str  23       ? <------------------------------------- TODO, move
      Enum       =  -9, //      24 1|2|4|8
      TimeSpan   = -10, //      25       8
      IntPtr     = -11, //      26     4|8
      UIntPtr    = -12, //      27     4|8
      BigInteger = -13, //      29       ?
      BitArray   = -14, //      30       ?
      Tuple      = -15, //      31       ?
      ValueTuple = -16, //      32       ?
      Multiple   = -17, // Tuple,   ValueTuple
      Unknown    = -18, // TODO Error/Unknown?
   }

   #endregion enumerations

   public partial class TypeParser
   {
      #region constructors

      public TypeParser(string? value)
      {
         this.text = value ?? string.Empty;
      }

      #endregion constructors

      #region protected fields

      protected string text;

      #endregion protected fields

      #region protected static fields

      // RegexHQ · GitHub
      // Copyright (c) 2016-2017, Jon Schlinkert.
      // Released under the MIT License.
      // base64-regex <https://github.com/regexhq/base64-regex>
      protected const string Base64RePat = // non-capturing
         @"(?:[A-Za-z0-9+\/]{4}\\n?)*(?:[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=)";
      // @"((?<chs>[A-Za-z0-9+\/])?:\k<chs>{4}\\n?)*(?:\k<chs>{2}==|\k<chs>{3}=)";

      protected const string Base64RePatExact = "(?:^" + Base64RePat + "$)";
      // regex-iso-date <https://github.com/regexhq/regex-iso-date>
      protected const string DateIsoRePat =
         @"(\d{4})-(\d{2})-(\d{2})T((\d{2}):(\d{2}):(\d{2}))\.(\d{3})Z";
      // regex-utc-date <https://github.com/regexhq/regex-utc-date>
      protected const string DateUtcRePat =
         @"(\w{3}), (\d{2}) (\w{3}) (\d{4}) ((\d{2}):(\d{2}):(\d{2})) GMT"; // IsStrict, else ' ' -> ' +'
      // @"(?<d>\d{2})(?<w>\w{3})\k<w>,\s*\k<d>\s+\k<w>\s+(\d{4})\s+(\k<d>:\k<d>:\k<d>)\s+GMT";

      protected const RegexOptions REOpts = RegexOptions.Compiled |
         RegexOptions.IgnoreCase;
      protected const RegexOptions REOptsBase64 = REOpts |
         RegexOptions.Multiline;

      protected const string REComplexSuffix = "[iI]";
      protected const string REEngCompSuffix = "[iIjJ]";
      protected const string REETX = "$";
      protected const string RESTX = "^";

      // floating-point-regex <https://github.com/regexhq/floating-point-regex>
      protected const string REFloatPattern =
         @"(([-+])?([0-9]*)(\.?[0-9]+)?(([Ee])?([-+])?([0-9]*)))+";
      protected const string REComplexPattern =
         @"^(?<f>" + REFloatPattern + @")\k<f>\s*(,?[+-])\s*\k<f>";

      protected static readonly Regex[] REFloats = new Regex[] {
         new Regex(REFloatPattern,  REOpts),             // <float>
         new Regex(string.Format("{0}{1}",               // a + bi
            REComplexPattern, REComplexSuffix), REOpts),
         new Regex(string.Format("{0}{1}",             // a + bj
            REComplexPattern, REEngCompSuffix), REOpts),
         new Regex(string.Format("{0}{1}{2}", RESTX,      // ^<float>$
            REFloatPattern, REETX), REOpts),
         new Regex(string.Format("{0}{1}{2}{3}", RESTX,      // ^a + bi$
            REComplexPattern, REComplexSuffix, REETX), REOpts),
         new Regex(string.Format("{0}{1}{2}{3}", RESTX,      // ^a + bj$
            REComplexPattern, REEngCompSuffix, REETX), REOpts),
      };

      protected static readonly int PointerSize = Marshal.SizeOf(typeof(IntPtr));

      protected static readonly List<Type> KnownTypeList = new () {
         typeof(DBNull),   // None
         typeof(Object),   typeof(DBNull),
         typeof(Boolean),  typeof(Char),     typeof(SByte),
         typeof(Byte),     typeof(Int16),    typeof(UInt16),
         typeof(Int32),    typeof(UInt32),   typeof(Int64),
         typeof(UInt64),   typeof(Single),   typeof(Double),
         typeof(Decimal),  typeof(DateTime), typeof(Nullity),
         typeof(String),
         typeof(String),   // Base64
         typeof(String),   // SID
         typeof(Guid),
         typeof(String),   // UUID
         typeof(Uri),      typeof(Enum),
         typeof(IntPtr),   typeof(UIntPtr),
         typeof(Complex),  typeof(TimeSpan),
         typeof(BigInteger), typeof(BitArray),
         typeof(Tuple),      typeof(ValueTuple)
      };

      protected static readonly List<TypeCoex> TypeCoexNumericPriority =
         new () {
            TypeCoex.Empty,      TypeCoex.Int32,      TypeCoex.UInt32,
            TypeCoex.Int64,      TypeCoex.UInt64,     TypeCoex.Byte,
            TypeCoex.Int16,      TypeCoex.UInt16,     TypeCoex.SByte,
            TypeCoex.DateTime,   TypeCoex.TimeSpan,   TypeCoex.Char,
            TypeCoex.IntPtr,     TypeCoex.UIntPtr,    TypeCoex.BigInteger,
            TypeCoex.Boolean,    TypeCoex.Single,     TypeCoex.Double,
            TypeCoex.Decimal,    TypeCoex.Complex,    TypeCoex.Guid,
            TypeCoex.UUID,       TypeCoex.SID,        TypeCoex.BitArray,
            TypeCoex.Enum,       TypeCoex.String,     TypeCoex.Object,
            TypeCoex.Base64,     TypeCoex.Uri,        TypeCoex.Tuple,
            TypeCoex.ValueTuple, TypeCoex.DBNull,     TypeCoex.Nullity
      };

      public static readonly List<TypeCoex> TypeCoexSizePriority =
         new () {
         /////////////////////// size
         TypeCoex.Empty,      //    0
         TypeCoex.Nullity,    //    0
         TypeCoex.Object,     //    0
         TypeCoex.DBNull,     //    0
         TypeCoex.Boolean,    //    1
         TypeCoex.SByte,      //    1
         TypeCoex.Byte,       //    1
         TypeCoex.Char,       //    2
         TypeCoex.Int16,      //    2
         TypeCoex.UInt16,     //    2
         TypeCoex.Int32,      //    4
         TypeCoex.UInt32,     //    4
         TypeCoex.Single,     //    4
         TypeCoex.IntPtr,     //  4|8
         TypeCoex.UIntPtr,    //  4|8
         TypeCoex.Int64,      //    8
         TypeCoex.UInt64,     //    8
         TypeCoex.Double,     //    8
         TypeCoex.DateTime,   //    8
         TypeCoex.TimeSpan,   //    8
         TypeCoex.Decimal,    //   16
         TypeCoex.Complex,    //   16
         TypeCoex.Guid,       //   16
         TypeCoex.UUID,       //   16
         TypeCoex.BigInteger, //    ?
         TypeCoex.String,     //    ?
         TypeCoex.SID,        //    ?
         TypeCoex.Uri,        //    ?
         TypeCoex.Base64,     //    ?
         TypeCoex.Tuple,      //    ?
         TypeCoex.ValueTuple, //    ?
         TypeCoex.Enum,       //    ?
      };

      protected static readonly List<TypeCoex> TypeCoexUsagePriority =
         new () {
         TypeCoex.String,     TypeCoex.Boolean,  TypeCoex.Char,
         TypeCoex.Int32,      TypeCoex.Int64,    TypeCoex.Byte,
         TypeCoex.UInt32,     TypeCoex.UInt64,   TypeCoex.Int16,
         TypeCoex.UInt16,     TypeCoex.SByte,    TypeCoex.DateTime,
         TypeCoex.Double,     TypeCoex.Single,   TypeCoex.Decimal,
         TypeCoex.Object,     TypeCoex.TimeSpan, TypeCoex.BigInteger,
         TypeCoex.IntPtr,     TypeCoex.UIntPtr,  TypeCoex.Complex,
         TypeCoex.Base64,     TypeCoex.Uri,      TypeCoex.Enum,
         TypeCoex.Guid,       TypeCoex.UUID,     TypeCoex.Tuple,
         TypeCoex.ValueTuple, TypeCoex.SID,      TypeCoex.Empty,
         TypeCoex.DBNull,     TypeCoex.Nullity,
      };

      protected static readonly int[] TypeCoexSizes = new int[] {
         (int)TCSizes.Empty,      //   0  TypeCode.Empty
         (int)TCSizes.Empty,      //   0  TypeCode.Object
         (int)TCSizes.Empty,      //   0  TypeCode.DBNull
         sizeof(bool),            //   1  TypeCode.Boolean
         sizeof(char),            //   2  TypeCode.Char
         sizeof(sbyte),           //   1  TypeCode.SByte
         sizeof(byte),            //   1  TypeCode.Byte
         sizeof(short),           //   2  TypeCode.Int16
         sizeof(ushort),          //   2  TypeCode.UInt16
         sizeof(int),             //   4  TypeCode.Int32
         sizeof(uint),            //   4  TypeCode.UInt32
         sizeof(long),            //   8  TypeCode.Int64
         sizeof(ulong),           //   8  TypeCode.UInt64
         sizeof(float),           //   4  TypeCode.Single
         sizeof(double),          //   8  TypeCode.Double
         sizeof(decimal),         //  16  TypeCode.Decimal
         sizeof(long),            //   8  TypeCode.DateTime
         0,                       //   0  TypeCoex.Nullity
         (int)TCSizes.String,     //      TypeCode.String
         (int)TCSizes.String,     //      TypeCoex.Base64
         (int)TCSizes.String,     //      TypeCoex.SID
         16,                      //  16  TypeCoex.Guid
         16,                      //  16  TypeCoex.UUID
         (int)TCSizes.String,     //      TypeCoex.Uri
         (int)TCSizes.Enum,       // 1-8  TypeCoex.Enum
         sizeof(long),            //   8  TypeCoex.TimeSpan
         PointerSize,             // 4|8  TypeCoex.IntPtr
         PointerSize,             // 4|8  TypeCoex.UIntPtr
         sizeof(double) * 2,      //  16  TypeCoex.Complex
         (int)TCSizes.BigInteger, //      TypeCoex.BigInteger
         (int)TCSizes.Multiple,   //      TypeCoex.Tuple
         (int)TCSizes.Multiple,   //      TypeCoex.ValueTuple
      };

      protected const StringSplitOptions SplitOptContentfulOnly =
         StringSplitOptions.RemoveEmptyEntries;

      protected static readonly char[][] Splits = new char[][] {
         new char[] { '+', '-' }, // Signs
         new char[] { ',' }       // Comma
      };

      #endregion protected static fields
   }
}
