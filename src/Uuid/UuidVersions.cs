// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// UuidVersions.cs
// Copyright © William Edward Wesse
//
using System;
using System.Reflection;
using System.Collections.Generic;

namespace TypeHelp
{
   #region UuidV1-8

   #region UuidV1-5

   /// <summary>
   /// UuidV1.
   /// </summary>
   public class UuidV1 : UuidRfc
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV1"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV1(bool strict, byte[] data) :
         base(strict, UuidVersion.TimeGregorian, data)
      { }
   }

   /// <summary>
   /// UuidV2.
   /// </summary>
   public class UuidV2 : UuidRfc
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV2"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV2(bool strict, byte[] data) :
         base(strict, UuidVersion.DCESecurity, data)
      { }
   }

   /// <summary>
   /// UuidV3.
   /// </summary>
   public class UuidV3 : UuidRfc
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV3"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV3(bool strict, byte[] data) :
         base(strict, UuidVersion.NameBasedMD5, data)
      { }
   }

   /// <summary>
   /// UuidV4.
   /// </summary>
   public class UuidV4 : UuidRfc
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV4"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV4(bool strict, byte[] data) :
         base(strict, UuidVersion.PseudoRandom, data)
      { }
   }

   /// <summary>
   /// UuidV5.
   /// </summary>
   public class UuidV5 : UuidRfc
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV5"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV5(bool strict, byte[] data) :
         base(strict, UuidVersion.NameBasedSHA1, data)
      { }
   }

   #endregion UuidV1-5

   #region UuidV6

   /// <summary>
   /// UuidV6.
   /// </summary>
   public class UuidV6 : Uuid
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV6"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV6(bool strict, byte[] data) :
         base(strict, UuidVersion.TimeReordered, data)
      { }
   }

   #endregion UuidV6

   #region UuidV7

   /// <summary>
   /// UuidV7.
   /// </summary>
   public class UuidV7 : Uuid
   {
      /// <summary>
      /// The unix time.
      /// </summary>
      public readonly ulong UnixTime;
      /// <summary>
      /// The unix time info.
      /// </summary>
      public readonly FieldInfo UnixTimeInfo;
      /// <summary>
      /// The rand a.
      /// </summary>
      public readonly ulong RandA;
      /// <summary>
      /// The rand a info.
      /// </summary>
      public readonly FieldInfo RandAInfo;
      /// <summary>
      /// The rand b.
      /// </summary>
      public readonly ulong RandB;
      /// <summary>
      /// The rand b info.
      /// </summary>
      public readonly FieldInfo RandBInfo;

      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV7"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV7(bool strict, byte[] data) :
         base(strict, UuidVersion.TimePosixEpoch, data)
      {
         UnixTimeInfo = field_info_set[0];
         RandAInfo = field_info_set[1];
         RandBInfo = field_info_set[2];
         UnixTime = (ulong)field_info_set[0].Value;
         RandA = (ulong)field_info_set[1].Value;
         RandB = (ulong)field_info_set[2].Value;
      }
   }

   #endregion UuidV7

   #region UuidV8

   /// <summary>
   /// UuidV8.
   /// </summary>
   public class UuidV8 : Uuid
   {
      /// <summary>
      /// The custom a.
      /// </summary>
      public readonly ulong CustomA;
      /// <summary>
      /// The custom a info.
      /// </summary>
      public readonly FieldInfo CustomAInfo;
      /// <summary>
      /// The custom b.
      /// </summary>
      public readonly ulong CustomB;
      /// <summary>
      /// The custom b info.
      /// </summary>
      public readonly FieldInfo CustomBInfo;
      /// <summary>
      /// The custom c.
      /// </summary>
      public readonly ulong CustomC;
      /// <summary>
      /// The custom c info.
      /// </summary>
      public readonly FieldInfo CustomCInfo;

      /// <summary>
      /// Initializes a new instance of the <see cref="UuidV8"/> class.
      /// </summary>
      /// <param name="strict">If true, strict.</param>
      /// <param name="data">The data.</param>
      public UuidV8(bool strict, byte[] data) :
         base(strict, UuidVersion.CustomFormats, data)
      {
         CustomAInfo = field_info_set[0];
         CustomBInfo = field_info_set[1];
         CustomCInfo = field_info_set[2];
         CustomA = (ulong)field_info_set[0].Value;
         CustomB = (ulong)field_info_set[1].Value;
         CustomC = (ulong)field_info_set[2].Value;
      }
   }

   #endregion UuidV8

   #endregion UuidV1-8
}
