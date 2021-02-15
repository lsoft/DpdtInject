﻿using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.Generic.DifferentConditional4
{
    public partial class ClusterGenericDifferentConditional4_RootCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rt => rt.ParentTarget.TargetType == typeof(B))
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .When(rt => rt.IsRoot)
                ;
        }
    }

    public partial class ClusterGenericDifferentConditional4_ChildCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IC>()
                .To<C>()
                .WithTransientScope()
                ;
        }

    }




    public class ClusterGenericDifferentConditional4_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterGenericDifferentConditional4_RootCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterGenericDifferentConditional4_ChildCluster>(
                rootCluster
                );

            var c = childCluster.Get<IC>();
            Assert.IsNotNull(c);
            Assert.IsNotNull(c.B);
            Assert.IsNotNull(c.B.A);
        }
    }

    public interface IA
    {

    }

    public class A : IA
    {

    }

    public interface IB
    {
        IA A { get; }
    }

    public class B : IB
    {
        public IA A { get; }

        public B(IA a)
        {
            A = a;
        }

    }

    public interface IC
    {
        IB B
        {
            get;
        }
    }

    public class C : IC
    {
        public IB B
        {
            get;
        }

        public C(IB b)
        {
            B = b;
        }
    }

}