using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]
