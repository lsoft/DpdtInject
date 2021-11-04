﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Constant.Target.RO0
{
    public partial class ScopeConstantTargetRO0_Cluster : DefaultCluster
    {
        private readonly string _roString;

#if IN_UNIT_TEST_SYMBOL
        /// <inheritdoc />
        public ScopeConstantTargetRO0_Cluster()
            : this((ICluster)null!)
        {
            _roString = "readonly string";
        }
#endif

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<string>()
                .WithConstScope(_roString)
                ;
        }

        public class ScopeConstantTargetRO0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetRO0_Cluster>(
                    );

                var s = cluster.Get<string>();
                Assert.IsNotNull(s);
                Assert.AreEqual("readonly string", s);
            }
        }
    }
}
