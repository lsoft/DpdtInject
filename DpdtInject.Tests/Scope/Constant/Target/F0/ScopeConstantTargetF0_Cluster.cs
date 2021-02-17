using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Scope.Constant.Target.F0
{
    public partial class ScopeConstantTargetF0_Cluster : DefaultCluster
    {
        //NOT A READONLY!!!
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string _someString;

#if IN_UNIT_TEST_SYMBOL
        /// <inheritdoc />
        public ScopeConstantTargetF0_Cluster()
            : this((ICluster)null!)
        {
            _someString = "some string";
        }
#endif

        public override void Load()
        {
            Bind<string>()
                .WithConstScope(_someString)
                ;
        }

        public class ScopeConstantTargetF0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetF0_Cluster>(
                    );

                var s = cluster.Get<string>();
                Assert.IsNotNull(s);
            }
        }

    }
}
