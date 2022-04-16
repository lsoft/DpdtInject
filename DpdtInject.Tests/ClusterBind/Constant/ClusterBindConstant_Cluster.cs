using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.CustomScope;
using System;
using System.Collections.Generic;
using DpdtInject.Injector.Src.RContext;
using System.Threading.Tasks;

namespace DpdtInject.Tests.ClusterBind.Constant
{
    public partial class ClusterBindConstant_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<SubjectCluster>()
                .WithConstScope(new SubjectCluster())
                ;
        }

        public class ClusterBindConstant_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ClusterBindConstant_Cluster>(
                    null
                    );
            }
        }
    }

    public partial class SubjectCluster : ICluster
    {
        public SubjectCluster()
        {

        }

        #region ICluster

        CustomScopeObject ICluster.CreateCustomScope()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            throw new NotImplementedException();
        }

        T IResolution.Get<T>()
        {
            throw new NotImplementedException();
        }

        T IResolution.Get<T>(CustomScopeObject customScope)
        {
            throw new NotImplementedException();
        }

        object IResolution.Get(Type requestedType)
        {
            throw new NotImplementedException();
        }

        object IResolution.Get(Type requestedType, CustomScopeObject customScope)
        {
            throw new NotImplementedException();
        }

        List<T> IResolution.GetAll<T>()
        {
            throw new NotImplementedException();
        }

        List<T> IResolution.GetAll<T>(CustomScopeObject customScope)
        {
            throw new NotImplementedException();
        }

        IEnumerable<object> IResolution.GetAll(Type requestedType)
        {
            throw new NotImplementedException();
        }

        IEnumerable<object> IResolution.GetAll(Type requestedType, CustomScopeObject customScope)
        {
            throw new NotImplementedException();
        }

        T ICluster.GetToChild<T>(IResolutionRequest resolutionRequest)
        {
            throw new NotImplementedException();
        }

        bool IResolution.IsRegisteredFrom<T>()
        {
            throw new NotImplementedException();
        }

        bool IResolution.IsRegisteredFrom(Type requestedType)
        {
            throw new NotImplementedException();
        }

        bool IResolution.IsRegisteredFromRecursive<T>()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
