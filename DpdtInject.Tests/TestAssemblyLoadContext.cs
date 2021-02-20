using System.Runtime.Loader;

namespace DpdtInject.Tests
{
    public class TestAssemblyLoadContext : AssemblyLoadContext
    {
        //private AssemblyDependencyResolver _resolver;

        //public TestAssemblyLoadContext(string mainAssemblyToLoadPath)
        //    : base(isCollectible: true)
        //{
        //    _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
        //}

        public TestAssemblyLoadContext()
            : base(isCollectible: true)
        {
        }

        //protected override Assembly Load(AssemblyName name)
        //{
        //    string assemblyPath = _resolver.ResolveAssemblyToPath(name);
        //    if (assemblyPath != null)
        //    {
        //        return LoadFromAssemblyPath(assemblyPath);
        //    }

        //    return null;
        //}
    }
}
