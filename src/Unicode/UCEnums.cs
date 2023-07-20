// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// UCEnums.cs
// Copyright © William Edward Wesse
//
using System;
using System.Globalization;

namespace TypeHelp
{
   #region enumerations

   #region CodePoint enumerations

   /// <summary>
   /// The c p g.
   /// </summary>
   public enum CPG // CodePoint Group
   {
      None = 0,
      Underflow,
      Overflow,
      NonCharacter,
      Replacement,   // maybe error (IsStrict)
      Sentinel,
      BOM,           // TODO auth flags ? BitString?
      CodePoint,
      Supplement,
      Surrogate,
      HiSurrogate,
      LoSurrogate,
   }

   /// <summary>
   /// The c p g i.
   /// </summary>
   public enum CPGI  // CodePoint Group Id - UCInfo.CPG[] Groups
   {
      CodePoint,
      Supplement,
      HiSurrogate,
      LoSurrogate,
   }

   #endregion CodePoint enumerations

   #region Unicode Plane enumerations

   /// <summary>
   /// The u c p.
   /// </summary>
   public enum UCP // Unicode Plane
   {
      BMP, SMP, SIP, TIP,
      UN4, UN5, UN6, UN7,
      UN8, UN9, UNA, UNB,
      UNC, UND, SSP, SPA, SPB,
      NON = 0x40000000,
   }

   /// <summary>
   /// The u c p f.
   /// </summary>
   [Flags]
   public enum UCPF // Unicode Plane Flags
   {
      BMP = 0x00001, SMP = 0x00002, SIP = 0x00004, TIP = 0x00008,
      UN4 = 0x00010, UN5 = 0x00020, UN6 = 0x00040, UN7 = 0x00080,
      UN8 = 0x00100, UN9 = 0x00200, UNA = 0x00400, UNB = 0x00800,
      UNC = 0x01000, UND = 0x02000, SSP = 0x04000, SPA = 0x08000,
      SPB = 0x10000,
      NON = UCP.NON,
   }

   #endregion Unicode Plane enumerations

   #region Unicode enhanced enumerations

   /// <summary>
   /// The char DataType.
   /// </summary>
   public enum CharType
   {
      ASCII,
      ANSI,
      UTF7,
      UTF8,
      UTF16,
      UTF16BE,
      UTF32,
      UTF32BE,
      Unicode
   }

   /// <summary>
   /// The u cat.
   /// </summary>
   public enum UCat
   { // alias UnicodeCategory
      Lu = UnicodeCategory.UppercaseLetter,         //  (0) Letter,      Uppercase
      Ll = UnicodeCategory.LowercaseLetter,         //  (1) Letter,      Lowercase
      Lt = UnicodeCategory.TitlecaseLetter,         //  (2) Letter,      Titlecase
      Lm = UnicodeCategory.ModifierLetter,          //  (3) Letter,      Modifier
      Lo = UnicodeCategory.OtherLetter,             //  (4) Letter,      Other
      Mn = UnicodeCategory.NonSpacingMark,          //  (5) Mark,        Nonspacing
      Mc = UnicodeCategory.SpacingCombiningMark,    //  (6) Mark,        Spacing Combining
      Me = UnicodeCategory.EnclosingMark,           //  (7) Mark,        Enclosing
      Nd = UnicodeCategory.DecimalDigitNumber,      //  (8) IsNumber,      RFC4122 Digit
      Nl = UnicodeCategory.LetterNumber,            //  (9) IsNumber,      Letter
      No = UnicodeCategory.OtherNumber,             // (10) IsNumber,      Other
      Zs = UnicodeCategory.SpaceSeparator,          // (11) Separator,   Space
      Zl = UnicodeCategory.LineSeparator,           // (12) Separator,   Line
      Zp = UnicodeCategory.ParagraphSeparator,      // (13) Separator,   Paragraph
      Cc = UnicodeCategory.Control,                 // (14) Other,       Control
      Cf = UnicodeCategory.Format,                  // (15) Other,       Format
      Cs = UnicodeCategory.Surrogate,               // (16) Other,       Surrogate
      Co = UnicodeCategory.PrivateUse,              // (17) Other,       Private Use
      Pc = UnicodeCategory.ConnectorPunctuation,    // (18) Punctuation, Connector
      Pd = UnicodeCategory.DashPunctuation,         // (19) Punctuation, Dash
      Ps = UnicodeCategory.OpenPunctuation,         // (20) Punctuation, Open
      Pe = UnicodeCategory.ClosePunctuation,        // (21) Punctuation, Close
      Pi = UnicodeCategory.InitialQuotePunctuation, // (22) Punctuation, Initial quote
      Pf = UnicodeCategory.FinalQuotePunctuation,   // (23) Punctuation, Final quote
      Po = UnicodeCategory.OtherPunctuation,        // (24) Punctuation, Other
      Sm = UnicodeCategory.MathSymbol,              // (25) Symbol,      Math
      Sc = UnicodeCategory.CurrencySymbol,          // (26) Symbol,      Currency
      Sk = UnicodeCategory.ModifierSymbol,          // (27) Symbol,      Modifier
      So = UnicodeCategory.OtherSymbol,             // (28) Symbol,      Other
      Cn = UnicodeCategory.OtherNotAssigned,        // (29) Other,       Not Assigned or Noncharacter
      Cz,  // ZeroWidthChars
      Wz   // ZeroWidthSpaces
   }

   /// <summary>
   /// The cmp op.
   /// </summary>
   public enum CmpOp : int
   {  /* Comparison 3-Letter Symbols: Command Prompt (cmd.exe) ---------------
      Symbol  CodePoint     Char  name
      ------  ------------  ----  ----------------------------------------- */
      EQU =   UCMath.EQU, // '='  EqualsSign Sign
      NEQ =   UCMath.NEQ, // '≠'  Not Equal To
      LSS =   UCMath.LSS, // '<'  Less-Than Sign
      LEQ =   UCMath.LEQ, // '≤'  Less-Than Or Equal To
      GTR =   UCMath.GTR, // '>'  Greater-Than Sign
      GEQ =   UCMath.GEQ, // '≥'  Greater-Than Or Equal To
   }

   /// <summary>
   /// The cmp u c.
   /// </summary>
   public enum CmpUC : int
   {  /* Comparison 3-Letter Symbols: All UCMath defined ---------------------
      Symbol  CodePoint     Char  name
      ------  ------------  ----  ----------------------------------------- */
      EQU =   UCMath.EQU, // '='  EqualsSign Sign
      NEQ =   UCMath.NEQ, // '≠'  Not Equal To
      LSS =   UCMath.LSS, // '<'  Less-Than Sign
      LEQ =   UCMath.LEQ, // '≤'  Less-Than Or Equal To
      GTR =   UCMath.GTR, // '>'  Greater-Than Sign
      GEQ =   UCMath.GEQ, // '≥'  Greater-Than Or Equal To
      GLT =   UCMath.GLT, // '≷'  Greater-Than Or Less-Than
      GNE =   UCMath.GNE, // '≩'  Greater-Than But Not Equal To
      GEV =   UCMath.GEV, // '≳'  Greater-Than Or Equivalent To
      LGT =   UCMath.LGT, // '≶'  Less-Than Or Greater-Than
      LNE =   UCMath.LNE, // '≨'  Less-Than But Not Equal To
      LEV =   UCMath.LEV, // '≲'  Less-Than Or Equivalent To
   }

   /// <summary>
   /// The u c general use.
   /// </summary>
   public enum UCGeneralUse : ushort
   {  /* General Use Symbols -------------------------------------------------
      Symbol                CodePoint   Char  name
      --------------------  ----------  ----  ----------------------------- */
      BrokenBar             = 0x00A6, // '¦'  Broken Bar
      Copyright             = 0x00A9, // '©'  Copyright Sign
      DblAngleQuoteLT       = 0x00AB, // '«'  Left-Pointing Dbl Angle Quote
      DblAngleQuoteRT       = 0x00BB, // '»'  Right-Pointing Dbl Angle Quote
      Degree                = 0x00B0, // '°'  DegreeSign Sign
      Division              = 0x00F7, // '÷'  DivideSign Sign
      InvertedExclamation   = 0x00A1, // '¡'  Inverted Exclamation Mark
      InvertedQuestionMark  = 0x00BF, // '¿'  Inverted Question Mark
      Macron                = 0x00AF, // '¯'  Macron
      MiddleDot             = 0x00B7, // '·'  Middle Dot
      Multiplication        = 0x00D7, // '×'  Multiplication Sign
      NBSP                  = 0x00A0, // ' '  No-Break Space
      Not                   = 0x00AC, // '¬'  Not Sign
      Null                  = 0x0000, //      Null
      Pilcrow               = 0x00B6, // '¶'  Pilcrow Sign
      PlusMinus             = 0x00B1, // '±'  Plus-Minus Sign
      ReplChar              = 0xFFFD, // '�'  Replacement Character
      ReplacementCharacter  = 0xFFFD, // '�'  Replacement Character
      Section               = 0x00A7, // '§'  Section Sign
      BOM_LE                = 0xFEFF, //      Byte Order Mark (Little-Endian)
      BOM_BE                = 0xFFFE  //      Byte Order Mark (Big-Endian)
   }

   /// <summary>
   /// The u c greek.
   /// </summary>
   public enum UCGreek : ushort
   {  /* Greek Basic Alphabet Symbols ----------------------------------------
      Symbol       CodePoint   Char   name
      -----------  ----------  ----   ------------------------------------- */
      Alpha        = 0x0391, // 'Α'   Greek Capital Letter Alpha
      Beta         = 0x0392, // 'Β'   Greek Capital Letter Beta
      Gamma        = 0x0393, // 'Γ'   Greek Capital Letter Gamma
      Delta        = 0x0394, // 'Δ'   Greek Capital Letter Delta
      Epsilon      = 0x0395, // 'Ε'   Greek Capital Letter Epsilon
      Zeta         = 0x0396, // 'Ζ'   Greek Capital Letter Zeta
      Eta          = 0x0397, // 'Η'   Greek Capital Letter Eta
      Theta        = 0x0398, // 'Θ'   Greek Capital Letter Theta
      Iota         = 0x0399, // 'Ι'   Greek Capital Letter Iota
      Kappa        = 0x039A, // 'Κ'   Greek Capital Letter Kappa
      Lamda        = 0x039B, // 'Λ'   Greek Capital Letter Lamda
      Mu           = 0x039C, // 'Μ'   Greek Capital Letter Mu
      Nu           = 0x039D, // 'Ν'   Greek Capital Letter Nu
      Xi           = 0x039E, // 'Ξ'   Greek Capital Letter Xi
      Omicron      = 0x039F, // 'Ο'   Greek Capital Letter Omicron
      Pi           = 0x03A0, // 'Π'   Greek Capital Letter Pi
      Rho          = 0x03A1, // 'Ρ'   Greek Capital Letter Rho
      Sigma        = 0x03A3, // 'Σ'   Greek Capital Letter Sigma
      Tau          = 0x03A4, // 'Τ'   Greek Capital Letter Tau
      Upsilon      = 0x03A5, // 'Υ'   Greek Capital Letter Upsilon
      Phi          = 0x03A6, // 'Φ'   Greek Capital Letter Phi
      Chi          = 0x03A7, // 'Χ'   Greek Capital Letter Chi
      Psi          = 0x03A8, // 'Ψ'   Greek Capital Letter Psi
      Omega        = 0x03A9, // 'Ω'   Greek Capital Letter Omega
      alpha        = 0x03B1, // 'α'   Greek Small Letter Alpha
      beta         = 0x03B2, // 'β'   Greek Small Letter Beta
      gamma        = 0x03B3, // 'γ'   Greek Small Letter Gamma
      delta        = 0x03B4, // 'δ'   Greek Small Letter Delta
      epsilon      = 0x03B5, // 'ε'   Greek Small Letter Epsilon
      zeta         = 0x03B6, // 'ζ'   Greek Small Letter Zeta
      eta          = 0x03B7, // 'η'   Greek Small Letter Eta
      theta        = 0x03B8, // 'θ'   Greek Small Letter Theta
      iota         = 0x03B9, // 'ι'   Greek Small Letter Iota
      kappa        = 0x03BA, // 'κ'   Greek Small Letter Kappa
      lamda        = 0x03BB, // 'λ'   Greek Small Letter Lamda
      mu           = 0x03BC, // 'μ'   Greek Small Letter Mu
      nu           = 0x03BD, // 'ν'   Greek Small Letter Nu
      xi           = 0x03BE, // 'ξ'   Greek Small Letter Xi
      omicron      = 0x03BF, // 'ο'   Greek Small Letter Omicron
      pi           = 0x03C0, // 'π'   Greek Small Letter Pi
      rho          = 0x03C1, // 'ρ'   Greek Small Letter Rho
      sigma        = 0x03C3, // 'σ'   Greek Small Letter Sigma
      finalsigma   = 0x03C2, // 'ς'   Greek Small Letter Final Sigma
      tau          = 0x03C4, // 'τ'   Greek Small Letter Tau
      upsilon      = 0x03C5, // 'υ'   Greek Small Letter Upsilon
      phi          = 0x03C6, // 'φ'   Greek Small Letter Phi
      chi          = 0x03C7, // 'χ'   Greek Small Letter Chi
      psi          = 0x03C8, // 'ψ'   Greek Small Letter Psi
      omega        = 0x03C9  // 'ω'   Greek Small Letter Omega
   }

   /// <summary>
   /// The u c math.
   /// </summary>
   public enum UCMath : ushort
   {  /*
      Comparison: 3-Letter Symbols -------------------------------------------
      Symbol  CodePoint                      Char  name
      ------  -----------------------------  ----  ------------------------ */
      EQU     = EqualsSign,                // '='  EqualsSign Sign
      NEQ     = NotEqualTo,                // '≠'  Not Equal To
      LSS     = LessThan,                  // '<'  Less-Than Sign
      LEQ     = LessThanOrEqualTo,         // '≤'  Less-Than Or Equal To
      GTR     = GreaterThan,               // '>'  Greater-Than Sign
      GEQ     = GreaterThanOrEqualTo,      // '≥'  Greater-Than Or Equal To
      GLT     = GreaterThanOrLessThan,     // '≷'  Greater-Than Or Less-Than
      GNE     = GreaterThanButNotEqualTo,  // '≩'  Greater-Than But Not Equal To
      GEV     = GreaterThanOrEquivalentTo, // '≳'  Greater-Than Or Equivalent To
      LGT     = LessThanOrGreaterThan,     // '≶'  Less-Than Or Greater-Than
      LNE     = LessThanButNotEqualTo,     // '≨'  Less-Than But Not Equal To
      LEV     = LessThanOrEquivalentTo,    // '≲'  Less-Than Or Equivalent To
      /* Comparison: Additional Symbols --------------------------------------
      Symbol           CodePoint        Char  name
      ---------------  ---------------  ----  ----------------------------- */
      Degree          = DegreeSign,   // '°'  Degree Sign
      Divide          = DivisionSign, // '÷'  Division Sign
      DblVertBarRTurn = 0x22AB,       // '⊫'  Double Vertical Bar Double Right Turnstile
      Equals          = EqualsSign,   // '='  Equals Sign
      InvExclamation  = 0x00A1,       // '¡'  Inverted Exclamation Mark
      /* Mathematical Symbols ------------------------------------------------
      Symbol                       CodePoint    Char  name
      ---------------------------  -----------  ----- --------------------- */
      Assertion                    = 0x22A6,  // '⊦'  Assertion
      Between                      = 0x226C,  // '≬'  Between
      CircledAsteriskOp            = 0x229B,  // '⊛'  Circled Asterisk Operator
      CircledDash                  = 0x229D,  // '⊝'  Circled Dash
      CircledDivSlash              = 0x2298,  // '⊘'  Circled DivideSign Slash
      CircledDotOp                 = 0x2299,  // '⊙'  Circled Dot Operator
      CircledEquals                = 0x229C,  // '⊜'  Circled EqualsSign
      CircledMinus                 = 0x2296,  // '⊖'  Circled Minus
      CircledPlus                  = 0x2295,  // '⊕'  Circled Plus
      CircledRingOp                = 0x229A,  // '⊚'  Circled Ring Operator
      CircledTimes                 = 0x2297,  // '⊗'  Circled Times
      ContainsAsNormalSubgroup     = 0x22B3,  // '⊳'  Contains As Normal Subgroup
      ContainsAsNormSubOrEqualTo   = 0x22B5,  // '⊵'  Contains As Normal Subgroup Or Equal To
      DegreeSign                   = 0x00B0,  // '°'  DegreeSign Sign
      DivisionSign                 = 0x00F7,  // '÷'  DivideSign Sign
      DoubleVertBarRightTurnstile  = DblVertBarRTurn, // '⊫'  Double Vertical Bar Double Right Turnstile
      DoesNotForce                 = 0x22AE,  // '⊮'  Does Not Force
      DoesNotPrecede               = 0x2280,  // '⊀'  Does Not Precede
      DoesNotProve                 = 0x22AC,  // '⊬'  Does Not Prove
      DoesNotSucceed               = 0x2281,  // '⊁'  Does Not Succeed
      DotOperator                  = 0x22C5,  // '⋅'  Dot Operator
      DownTack                     = 0x22A4,  // '⊤'  Down Tack
      EqualsSign                   = 0x003D,  // '='  EqualsSign Sign
      Forces                       = 0x22A9,  // '⊩'  Forces
      GreaterThan                  = 0x003E,  // '>'  Greater-Than Sign
      GreaterThanButNotEqualTo     = 0x2269,  // '≩'  Greater-Than But Not Equal To
      GreaterThanOrEqualTo         = 0x2265,  // '≥'  Greater-Than Or Equal To
      GreaterThanOrEquivTo         = 0x2273,  // '≳'  Greater-Than Or Equivalent To
      GreaterThanOrEquivalentTo    = 0x2273,  // '≳'  Greater-Than Or Equivalent To
      GreaterThanOrLessThan        = 0x2277,  // '≷'  Greater-Than Or Less-Than
      HasNormSubgrp                = 0x22B3,  // '⊳'  Contains As Normal Subgroup
      HasNormSubgrpOrEqualTo       = 0x22B5,  // '⊵'  Contains As Normal Subgroup Or Equal To
      HermitianConjugateMatrix     = 0x22B9,  // '⊹'  Hermitian Conjugate Matrix
      IdenticalTo                  = 0x2261,  // '≡'  Identical To
      ImageOf                      = 0x22B7,  // '⊷'  Image Of
      Intercalate                  = 0x22BA,  // '⊺'  Intercalate
      InvertedExclamationMark      = InvExclamation,  //  '¡'  Inverted Exclamation Mark
      InvertedQuestionMark         = 0x00BF,  // '¿'  Inverted Question Mark
      InvertQuestion               = 0x00BF,  // '¿'  Inverted Question Mark
      LessThan                     = 0x003C,  // '<'  Less-Than Sign
      LessThanButNotEqualTo        = 0x2268,  // '≨'  Less-Than But Not Equal To
      LessThanOrEqualTo            = 0x2266,  // '≤'  Less-Than Or Equal To
      LessThanOrEquivTo            = 0x2272,  // '≲'  Less-Than Or Equivalent To
      LessThanOrEquivalentTo       = 0x2272,  // '≲'  Less-Than Or Equivalent To
      LessThanOrGreaterThan        = 0x2276,  // '≶'  Less-Than Or Greater-Than
      LeftTack                     = 0x22A3,  // '⊣'  Left Tack
      Macron                       = 0x00AF,  // '¯'  Macron
      MiddleDot                    = 0x00B7,  // '·'  Middle Dot
      Models                       = 0x22A7,  // '⊧'  Models
      Multimap                     = 0x22B8,  // '⊸'  Multimap
      Multiplication               = 0x00D7,  // '×'  Multiplication Sign
      Multiply                     = 0x00D7,  // '×'  Multiplication Sign
      Multiset                     = 0x228C,  // '⊌'  Multiset
      MultisetMultiplication       = 0x228D,  // '⊍'  Multiset Multiplication
      MultisetMultiply             = 0x228D,  // '⊍'  Multiset Multiplication
      MultisetUnion                = 0x228E,  // '⊎'  Multiset Union
      Nand                         = 0x22BC,  // '⊼'  Nand
      NegatedDblVertTurnstile      = 0x22AF,  // '⊯'  Negated Double Vertical Bar Double Right Turnstile
      NeitherGTnorEqualTo          = 0x2271,  // '≱'  Neither Greater-Than Nor Equal To
      NeitherGTnorEquivalentTo     = 0x2275,  // '≵'  Neither Greater-Than Nor Equivalent To
      NeitherGTnorLT               = 0x2279,  // '≹'  Neither Greater-Than Nor Less-Than
      Nor                          = 0x22BD,  // '⊽'  Nor
      NormalSubgroupOf             = 0x22B2,  // '⊲'  Normal Subgroup Of
      NormalSubgrpOfOrEqualTo      = 0x22B4,  // '⊴'  Normal Subgroup Of Or Equal To
      Not                          = 0x00AC,  // '¬'  Not Sign
      NotASubOfOrEqualTo           = 0x2288,  // '⊈'  Neither A Subset Of Nor Equal To
      NotASubsetOf                 = 0x2284,  // '⊄'  Not A Subset Of
      NotASupersetOfOrEqualTo      = 0x2289,  // '⊉'  Neither A Superset Of Nor Equal To
      NotASupersetOf               = 0x2285,  // '⊅'  Not A Superset Of
      NotEqualTo                   = 0x2260,  // '≠'  Not Equal To
      NotEquivalentTo              = 0x226D,  // '≭'  Not Equivalent To
      NotGreaterThan               = 0x226F,  // '≯'  Not Greater-Than
      NotIdenticalTo               = 0x2262,  // '≢'  Not Identical To
      NotLessThan                  = 0x226E,  // '≮'  Not Less-Than
      NotLessThanOrEqualTo         = 0x2270,  // '≰'  Neither Less-Than Nor Equal To
      NotLessThanOrEquivalentTo    = 0x2274,  // '≴'  Neither Less-Than Nor Equivalent To
      NotLessThanOrGreaterThan     = 0x2278,  // '≸'  Neither Less-Than Nor Greater-Than
      NotTrue                      = 0x22AD,  // '⊭'  Not True
      OriginalOf                   = 0x22B6,  // '⊶'  Original Of
      PlusMinus                    = 0x00B1,  // '±'  Plus-Minus Sign
      Precedes                     = 0x227A,  // '≺'  Precedes
      PrecedesOrEqualTo            = 0x227C,  // '≼'  Precedes Or Equal To
      PrecedesOrEquivalentTo       = 0x227E,  // '≾'  Precedes Or Equivalent To
      PrecedesUnderRelation        = 0x22B0,  // '⊰'  Precedes Under Relation
      RightAngleWithArc            = 0x22BE,  // '⊾'  Right Angle With Arc
      RightTack                    = 0x22A2,  // '⊢'  Right Tack
      RightTriangle                = 0x22BF,  // '⊿'  Right Triangle
      SquareCap                    = 0x2293,  // '⊓'  Square Cap
      SquareCup                    = 0x2294,  // '⊔'  Square Cup
      SquareImageOf                = 0x228F,  // '⊏'  Square Image Of
      SquareImageOfOrEqual         = 0x2291,  // '⊑'  Square Image Of Or Equal To
      SquareOrigOfOrEqual          = 0x2292,  // '⊒'  Square Original Of Or Equal To
      SquareOriginalOf             = 0x2290,  // '⊐'  Square Original Of
      SquaredDotOp                 = 0x22A1,  // '⊡'  Squared Dot Operator
      SquaredMinus                 = 0x229F,  // '⊟'  Squared Minus
      SquaredPlus                  = 0x229E,  // '⊞'  Squared Plus
      SquaredTimes                 = 0x22A0,  // '⊠'  Squared Times
      StrictlyEquivalentTo         = 0x2263,  // '≣'  Strictly Equivalent To
      SubOfOrEqual                 = 0x2286,  // '⊆'  Subset Of Or Equal To
      SubOfWithNotEqual            = 0x228A,  // '⊊'  Subset Of With Not Equal To
      SubsetOf                     = 0x2282,  // '⊂'  Subset Of
      Succeeds                     = 0x227B,  // '≻'  Succeeds
      SucceedsOrEqual              = 0x227D,  // '≽'  Succeeds Or Equal To
      SucceedsOrEquivalent         = 0x227F,  // '≿'  Succeeds Or Equivalent To
      SucceedsUnderRelation        = 0x22B1,  // '⊱'  Succeeds Under Relation
      SupersetOfOrEqual            = 0x2287,  // '⊇'  Superset Of Or Equal To
      SupersetOfWithNotEqual       = 0x228B,  // '⊋'  Superset Of With Not Equal To
      SupersetOf                   = 0x2283,  // '⊃'  Superset Of
      TripleVertBarRightTurnstile  = 0x22AA,  // '⊪'  Triple Vertical Bar Right Turnstile
      TriVertBarRtTurnstile        = 0x22AA,  // '⊪'  Triple Vertical Bar Right Turnstile
      True                         = 0x22A8,  // '⊨'  True
      UpTack                       = 0x22A5,  // '⊥'  Up Tack
      Xor                          = 0x22BB,  // '⊻'  Xor
   }

   /// <summary>
   /// The zero width chars.
   /// </summary>
   public enum ZeroWidthChars : ushort
   {  /*
      General Use Symbols ----------------------------------------------------
      Symbol                        CodePoint    name
      ----------------------------  ----------   -------------------------- */
      Activate_Arabic_Form_Shaping  = 0x206D, // Activate Arabic Form Shaping
      Activate_Symmetric_Swapping   = 0x206B, // Activate Symmetric Swapping
      Arabic_Letter_Mark            = 0x061C, // Arabic Letter Mark
      Combining_Grapheme_Joiner     = 0x034F, // Combining Grapheme Joiner
      Function_Application          = 0x2061, // Function Application
      Inhibit_Arabic_Form_Shaping   = 0x206C, // Inhibit Arabic Form Shaping
      Inhibit_Symmetric_Swapping    = 0x206A, // Inhibit Symmetric Swapping
      Invisible_Plus                = 0x2064, // Invisible Plus
      Invisible_Separator           = 0x2063, // Invisible Separator
      Invisible_Times               = 0x2062, // Invisible Times
      Khmer_Vowel_Inherent_Aa       = 0x17B5, // Khmer Vowel Inherent Aa
      Khmer_Vowel_Inherent_Aq       = 0x17B4, // Khmer Vowel Inherent Aq
      Left_To_Right_Mark            = 0x200E, // Left-To-Right Mark
      Mongolian_Vowel_Separator     = 0x180E, // Mongolian Vowel Separator
      National_Digit_Shapes         = 0x206E, // National Digit Shapes
      Nominal_Digit_Shapes          = 0x206F, // Nominal Digit Shapes
      Right_To_Left_Mark            = 0x200F, // Right-To-Left Mark
      Word_Joiner                   = 0x2060, // Word Joiner
      Zero_Width_Joiner             = 0x200D, // Zero Width Joiner
      Zero_Width_Non_Joiner         = 0x200C, // Zero Width Non-Joiner
      Zero_Width_Space              = 0x200B  // Zero Width Space
   }

   /// <summary>
   /// The zero width spaces.
   /// </summary>
   public enum ZeroWidthSpaces : ushort
   {  /*
      General Use Symbols ----------------------------------------------------
      Symbol                CodePoint    name
      --------------------  ----------   ---------------------------------- */
      Zero_Width_Space      = 0x200B, // Zero Width Space
      Zero_Width_Non_Joiner = 0x200C, // Zero Width Non-Joiner
      Zero_Width_Joiner     = 0x200D  // Zero Width Joiner
   };

   #endregion Unicode enhanced enumerations

   #endregion enumerations
}
