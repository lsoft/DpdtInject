using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.CustomScope;
using System;
using System.Collections.Generic;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Tests.ClusterBind.Transient
{
    public partial class ClusterBindTransient_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<SubjectCluster>()
                .To<SubjectCluster>()
                .WithTransientScope()
                ;
        }

        public class ClusterBindTransient_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ClusterBindTransient_Cluster>(
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
