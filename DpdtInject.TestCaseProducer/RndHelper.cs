using System;
using System.Collections.Generic;

namespace DpdtInject.TestCaseProducer
{


    public static class RndHelper
    {
        public static T GetRandom<T>(
            this Random rnd,
            IReadOnlyList<T> list
            )
        {
            return list[rnd.Next(list.Count)];
        }
    }

}
