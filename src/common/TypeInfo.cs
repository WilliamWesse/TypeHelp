// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// TypeInfo.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Collections;
using System.Numerics;

namespace TypeHelp
{
   #region enumerations

   /// <summary>
   /// The TypeInfo Property (TIP) enumeration defines the <see cref="TypeInfo"/> Property values.
   /// </summary>
   public enum TIP
   {
      /// <summary>
      /// Indicates the value provided to a <see cref="TypeInfo"/> constructor is not null.
      /// </summary>
      HasValue,

      /// <summary>
      /// Indicates the value provided to a <see cref="TypeInfo"/> constructor is null.
      /// </summary>
      IsEmpty,

      /// <summary>
      /// Indicates the value is or type indicates an enumeration.
      /// </summary>
      IsEnum,

      /// <summary>
      /// Indicates the value is or type indicates a signed integer, i.e., one of the following data types:
      /// <code>
      ///  System.SByte  System.Int16  System.Int32  System.Int64
      ///  System.Numerics.BigInteger
      /// </code>
      /// </summary>
      IsInteger,

      /// <summary>
      /// Indicates the value is or type indicates a natural number, i.e., an unsigned integer, one of the following data types:
      /// <code>
      ///  System.Byte  System.UInt16  System.UInt32  System.UInt64
      /// </code>
      /// </summary>
      IsNatural,

      /// <summary>
      /// Indicates the value or type is a number, i.e., one of the following data types:
      /// <code>
      ///  System.Byte    System.Decimal  System.Double
      ///  System.Int16   System.Int32    System.Int64
      ///  System.SByte   System.Single
      ///  System.UInt16  System.UInt32   System.UInt64
      ///  System.Numerics.BigInteger
      ///  System.Numerics.Complex
      /// </code>
      /// </summary>
      IsNumber,

      /// <summary>
      /// Indicates the value or type is a <see cref="System.Object"/>.
      /// </summary>
      IsObject,

      /// <summary>
      /// Indicates the value or type is floating-point, i.e., one of the following data types:
      /// <code>
      ///  System.Decimal  System.Double
      ///  System.Single   System.Numerics.Complex
      /// </code>
      /// </summary>
      IsPrecision,

      /// <summary>
      /// IsString
      /// <para>Indicates the value or type is a <see cref="System.String"/>.
      /// </para>
      /// </summary>
      IsString,

      /// <summary>
      /// IsType
      /// <para>Indicates the <see cref="Type"/> Property is a
      /// <see cref="System.Type"/>.</para>
      /// </summary>
      IsType,

      /// <summary>
      /// Indicates the status of the argument (by type) provided to the constructor:
      /// <list type="table">
      /// <listheader><term>Type</term><description>Status</description></listheader>
      /// <item><term><c>System.Object</c></term><description>value is not null.</description></item>
      /// <item><term><c>System.String</c></term><description>value is not null.</description></item>
      /// <item><term><c>System.TypeCode</c></term><description>value is defined.</description></item>
      /// <item><term><c>TypeHelp.TypeCoex</c></term><description>value is defined.</description></item>
      /// </list>
      /// </summary>
      IsValid,
   }

   #endregion enumerations

   #region TypeInfo

   /// <summary>
   /// Contains information about a data type or value defined by either the <see cref="System.TypeCode"/> or <seealso cref="TypeCoex"/> enumeration.
   /// </summary>
   public struct TypeInfo
   {
      #region constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="TypeInfo"/> class using a <see cref="System.Type"/> or <see cref="System.ValueType"/> object.
      /// </summary>
      /// <param name="obj">The object to deconstruct. This may be any <see cref="System.Type"/> or <see cref="System.ValueType"/> defined by the <see cref="System.TypeCode"/> and <seealso cref="TypeHelp.TypeCoex"/> enumerations.</param>
      public TypeInfo(Object? obj)
      {
         Text = string.Empty;
         Type? type = this.Type = TypeParser.GetType(obj);
         Value = obj;
         props = create_props();
         longSize = elements = length = size = 0;
         fullName = name = string.Empty;
         TypeCode typeCode = this.TypeCode = TypeParser.GetTypeCode(type);
         TypeCoex typeCoex = this.TypeCoex = TypeParser.GetTypeCoex(type);
         CodeFlag = TypeParser.GetCodeFlag(typeCode);
         CoexFlag = TypeParser.GetCoexFlag(typeCoex);
         coexStat = TCF.None;
         InitializeProperties(null != obj);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TypeInfo"/> class using a <see cref="System.Type"/>.
      /// </summary>
      /// <param name="type">The type to deconstruct. This may be any type defined by the <see cref="System.TypeCode"/> and <seealso cref="TypeHelp.TypeCoex"/> enumerations.</param>
      public TypeInfo(Type type)
      {
         Text = string.Empty;
         Type = type;
         Value = null;
         props = create_props();
         longSize = elements = length = size = 0;
         fullName = name = string.Empty;
         TypeCode typeCode = this.TypeCode = TypeParser.GetTypeCode(type);
         TypeCoex typeCoex = this.TypeCoex = TypeParser.GetTypeCoex(type);
         CodeFlag = TypeParser.GetCodeFlag(typeCode);
         CoexFlag = TypeParser.GetCoexFlag(typeCoex);
         coexStat = TCF.None;
         InitializeProperties(null != type);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TypeInfo"/> class using a <see cref="System.TypeCode"/>.
      /// </summary>
      /// <param name="typeCode">A <see cref="System.TypeCode"/>
      /// enumeration value to deconstruct.</param>
      public TypeInfo(TypeCode typeCode)
      {
         Text = string.Empty;
         Type = TypeParser.GetType(typeCode);
         Value = null;
         props = create_props();
         longSize = elements = length = size = 0;
         fullName = name = string.Empty;
         TypeCode = typeCode;
         TypeCoex = (TypeCoex)typeCode;
         CodeFlag = TypeParser.GetCodeFlag(typeCode);
         CoexFlag = TypeParser.GetCoexFlag((TypeCoex)typeCode);
         coexStat = TCF.None;
         InitializeProperties(Enum.IsDefined(typeof(TypeCode), typeCode));
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TypeInfo"/> class using a <see cref="TypeHelp.TypeCoex"/>.
      /// </summary>
      /// <param name="typeCoex">A <see cref="TypeHelp.TypeCoex"/> enumeration value
      /// to deconstruct.</param>
      public TypeInfo(TypeCoex typeCoex)
      {
         Text = string.Empty;
         Type = TypeParser.GetType(typeCoex);
         Value = null;
         props = create_props();
         longSize = elements = length = size = 0;
         fullName = name = string.Empty;
         TypeCode typeCode = TypeCode = TypeParser.GetTypeCode(typeCoex);
         TypeCoex = typeCoex;
         CodeFlag = TypeParser.GetCodeFlag(typeCode);
         CoexFlag = TypeParser.GetCoexFlag(typeCoex);
         coexStat = TCF.None;
         InitializeProperties(Enum.IsDefined(typeof(TypeCoex), typeCoex));
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TypeInfo"/> class using a <see cref="System.String"/>.
      /// </summary>
      /// <param name="text">A <see cref="System.String"/>
      /// value to deconstruct.</param>
      public TypeInfo(String text)
      {
         this.Text = text ?? throw new ArgumentNullException(nameof(text));
         Type = text.GetType();
         Value = text;
         props = create_props();
         longSize = elements = length = size = 0;
         fullName = name = string.Empty;
         TypeCode = TypeCode.String;
         TypeCoex = TypeCoex.String;
         CodeFlag = TypeParser.GetCodeFlag(TypeCode.String);
         CoexFlag = TypeParser.GetCoexFlag(TypeCoex.String);
         coexStat = TCF.None;
         InitializeProperties(true);
      }

      #endregion constructors

      #region indexer

      /// <summary>
      /// Indexer [<see cref="TIP"/>]
      /// <para>
      /// Gets the <see cref="System.Boolean"/> for the specified property.
      /// </para>
      /// </summary>
      /// <param name="prop">The <see cref="TIP"/> property to obtain.</param>
      /// <returns>The <see cref="System.Boolean"/> value for the specified property.</returns>
      public bool this[TIP prop] {
         get { return props[(int)prop]; }
      }

      #endregion indexer

      #region public properties

      /// <summary>
      /// CoexStat Property
      /// <para>TypeOf: <see cref="TCF"/></para>
      /// Gets and sets the CoexStat value.
      /// </summary>
      public TCF CoexStat {
         get {  return coexStat; }
         set { coexStat = value; }
      }

      /// <summary>
      /// Elements
      /// <para>TypeOf: <see cref="System.Int32"/></para>
      /// The number of elements comprising the IsType or HasValue.
      /// </summary>
      public int Elements { get { return elements; } }

      /// <summary>
      /// FullName
      /// <para>TypeOf: <see cref="System.String"/></para>
      /// The FullName of the <see cref="Type"/> Property,
      /// or "null" if the type is null.
      /// </summary>
      public String FullName { get { return fullName; } }

      /// <summary>
      /// HasValue Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.HasValue"/> value from this struct indexer.
      /// </summary>
      public bool HasValue { get { return this[TIP.HasValue]; } }

      /// <summary>
      /// IsEmpty Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsEmpty"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is null.
      /// </summary>
      public bool IsEmpty { get { return this[TIP.IsEmpty]; } }

      /// <summary>
      /// IsEnum Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsEnum"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is an enumeration.
      /// </summary>
      public bool IsEnum { get { return this[TIP.IsEnum]; } }

      /// <summary>
      /// IsInteger Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsInteger"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is an integer or
      /// <seealso cref="System.Numerics.BigInteger"/>.
      /// </summary>
      public bool IsInteger { get { return this[TIP.IsInteger]; } }

      /// <summary>
      /// IsNatural Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsNatural"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is a natural number.
      /// </summary>
      public bool IsNatural { get { return this[TIP.IsNatural]; } }

      /// <summary>
      /// IsNumber Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsNumber"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is an integer, natural,
      /// <seealso cref="System.Numerics.BigInteger"/> or
      /// floating point number.
      /// </summary>
      public bool IsNumber { get { return this[TIP.IsNumber]; } }

      /// <summary>
      /// IsObject Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsObject"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is a <seealso cref="System.Object"/>.
      /// </summary>
      public bool IsObject { get { return this[TIP.IsObject]; } }

      /// <summary>
      /// IsPrecision Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsPrecision"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is a floating point number.
      /// </summary>
      public bool IsPrecision { get { return this[TIP.IsPrecision]; } }

      /// <summary>
      /// IsString Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsString"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is a <seealso cref="System.String"/>.
      /// </summary>
      public bool IsString { get { return this[TIP.IsString]; } }

      /// <summary>
      /// IsType Property
      /// <para>TypeOf: <see cref="System.Boolean"/></para>
      /// The <see cref="TIP.IsType"/> value from this struct indexer,
      /// indicating the <seealso cref="Value"> Property
      /// </seealso> is a <seealso cref="System.Type"/>.
      /// </summary>
      public bool IsType { get { return this[TIP.IsType]; } }

      /// <summary>
      /// Length
      /// <para>TypeOf: <see cref="System.Int32"/></para>
      /// The length of the value in bytes, or zero if the value is null.
      /// </summary>
      public int Length { get { return length; } }

      /// <summary>
      /// LongSize
      /// <para>TypeOf: <see cref="System.Int64"/></para>
      /// The size of the value in bytes, or zero if the value is null.
      /// <para>Note this value applies to <see cref="System.String"/>,
      /// <seealso cref="System.Numerics.BigInteger"/> and array or
      /// multi-part values.</para>
      /// </summary>
      public long LongSize { get { return longSize; } }

      /// <summary>
      /// Name
      /// <para>TypeOf: <see cref="System.String"/></para>
      /// The Name of the <see cref="Type"/> Property,
      /// or "null" if the type is null.
      /// </summary>
      public String Name { get { return name; } }

      /// <summary>
      /// Size
      /// <para>TypeOf: <see cref="System.Int32"/></para>
      /// The size of the value in bytes, or zero if the value is null.
      /// <para><see cref="LongSize"/> concerning array or
      /// multi-part values.</para>
      /// </summary>
      public int Size { get { return size; } }

      #endregion public properties

      #region public fields

      /// <summary>
      /// CodeFlag Field
      /// <para>TypeOf: <see cref="TCF"/></para>
      /// </summary>
      public readonly TCF CodeFlag;

      /// <summary>
      /// CoexFlag Field
      /// <para>TypeOf: <see cref="TCF"/></para>
      /// </summary>
      public readonly TCF CoexFlag;

      /// <summary>
      /// Text Field
      /// <para>TypeOf: <see cref="System.String"/></para>
      /// This will be <see cref="System.String.Empty"/>,
      /// a <seealso cref="System.String"/>, or <c>ToString()</c>
      /// against a <seealso cref="System.Object"/>
      /// provided to the <see cref="TypeInfo"/> constructor.
      /// </summary>
      public readonly String Text;

      /// <summary>
      /// IsType Field
      /// <para>TypeOf: <see cref="System.Type"/> or null.</para>
      /// The <see cref="System.Type"/> of the <see cref="Value"/>,
      /// <seealso cref="TypeCode"/> Property
      /// or <seealso cref="TypeCode"/> Property
      /// </summary>
      public readonly Type? Type;

      /// <summary>
      /// TypeCode Field
      /// <para>TypeOf: <see cref="System.TypeCode"/></para>
      /// The value provided to the <see cref="TypeInfo"/> constructor,
      /// or from the <see cref="Value"/> Property.
      /// </summary>
      public readonly TypeCode TypeCode;

      /// <summary>
      /// TypeCoex Field
      /// <para>TypeOf: <see cref="TypeCoex"/></para>
      /// The value provided to the <see cref="TypeInfo"/> constructor,
      /// or from the <see cref="Value"/> Property.
      /// </summary>
      public readonly TypeCoex TypeCoex;

      /// <summary>
      /// Value Field
      /// <para>TypeOf: <see cref="System.Object"/> or null.</para>
      /// The value provided to the <see cref="TypeInfo"/> constructor.
      /// </summary>
      public readonly Object? Value;

      #endregion public fields

      #region public methods

      public bool Is(TypeCode typeCode)
      {
         return typeCode == TypeCode;
      }

      public bool Is(TypeCoex typeCoex)
      {
         return typeCoex == TypeCoex;
      }

      #endregion public methods

      #region private indexer and methods

      private bool this[int prop] {
         set {
            props[(int)prop] = value;
         }
      }

      private void InitializeProperties(bool valid)
      {
         this[(int)TIP.HasValue] = null != Value;
         this[(int)TIP.IsEmpty]  = null == Type;
         this[(int)TIP.IsEnum]   = null != Type && Type.IsEnum;
         this[(int)TIP.IsType]   = TypeParser.IsType(Type);
         this[(int)TIP.IsValid]  = valid;
         if (null != Type) {
            fullName = Type.FullName ?? string.Empty;
            length = 1;
            name = Type.Name;
            this[(int)TIP.IsObject] = TypeCode == TypeCode.Object;
            this[(int)TIP.IsString] = TypeCode == TypeCode.String;
            this[(int)TIP.IsNumber] = TypeParser.IsNumeric(TypeCoex);
            if (this[TIP.IsNumber]) {
               this[(int)TIP.IsPrecision] = TypeParser.IsPrecision(TypeCoex);
               this[(int)TIP.IsInteger]   = TypeParser.IsInteger(TypeCoex);
               this[(int)TIP.IsNatural]   = TypeParser.IsNatural(TypeCoex);
            }
         } else {
            name = fullName = TypeParser.RunProps.Properties.NullName;
         }
         InitializeValue(valid);
      }

      private void InitializeValue(bool valid)
      {
         if (null != Value) {
            coexStat |= TCF.HasElement;
         }
         if (TypeCoex == TypeCoex.Complex) {
            elements = length = 2;
            longSize = size = TypeParser.SizeOf(TypeCoex.Complex);
         }
         if (!valid || null == Value || null == Type || !Type.IsEnum) {
            coexStat |= TCF.Error;
            return;
         }
         switch (TypeCoex) {
            case TypeCoex.String:
               elements = length = ((String)Value).Length;
               longSize = length * TypeParser.SizeOf(TypeCoex.Char);
               size = (int)longSize;
               break;
            case TypeCoex.Enum:
               elements = length = Enum.GetValues(Type).Length;
               longSize = size = TypeParser.SizeOf(
                  Type!.GetEnumUnderlyingType());
               break;
            case TypeCoex.BigInteger:
               longSize = elements = length = size =
                  ((BigInteger)Value).ToByteArray().Length;
               break;
            case TypeCoex.BitArray:
               longSize = elements = length = size =
                  (int)(TypeParser.PadMod(((BitArray)Value).Count, 8) >> 3);
               break;
            case TypeCoex.Tuple:
               longSize = elements = length = size =
                  TypeParser.TupleElementCount(Value);
               break;
            case TypeCoex.ValueTuple:
               longSize = elements = length = size =
                  TypeParser.TupleElementCount(Value);
               break;
            default:
               elements = length = 1;
               longSize = size = TypeParser.SizeOf(Value);
               break;
         }
      }

      #endregion private indexer and methods

      #region private fields

      private readonly BitArray props;
      private string name;
      private String fullName;
      private long longSize;
      private TCF coexStat;
      private int elements;
      private int length;
      private int size;

      #endregion private fields

      #region private static fields and methods

      private static readonly int prop_count =
         Enum.GetValues(typeof(TIP)).Length;

      private static BitArray create_props()
      {
         BitArray bitProps = new(prop_count);
         for (int i = 0; i < bitProps.Length; i++) {
            bitProps[i] = false;
         }
         return bitProps;
      }

      #endregion private static fields and methods
   }

   #endregion TypeInfo
}
