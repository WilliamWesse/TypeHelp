// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// Enumerations.cs
// Copyright © William Edward Wesse
//
using System;

namespace TypeHelp
{
    #region enumerations

    /// <summary>
    /// Specifies standard <c>LSS</c>, <c>EQU</c> and <c>GTR</c> comparison values (all positive) in addition to error values (all negative) such as <c>NAN</c> and <c>OVF</c> (overflow). These are used by <see cref="TypeParser.Compare(int, int)"/> and similar methods for other <c>TypeParser</c> supported types.
    /// </summary>
    public enum Cmp : int
    {
        /* Comparison 3-Letter Symbols: TypeParser.Compare* methods ------------
        Symbol  Value  Description
        ------  -----  ------------------------------------------------------ */
        NUL = -4,  // Null
        NAN = -3,  // Not a Number
        UNF = -2,  // Underflow
        OVF = -1,  // Overflow
        LSS =  0,  // Less-Than
        EQU =  1,  // Equal To
        GTR =  2,  // Greater-Than
    }

    #endregion enumerations
}
