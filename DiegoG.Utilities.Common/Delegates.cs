using System;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.Utilities.Delegates
{
    public delegate T2 FuncRef<T1, T2>(ref T1 arg);
    public delegate void ActionRef<T>(ref T arg);
}
