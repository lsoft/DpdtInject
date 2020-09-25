using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Helper
{
    public static class MathHelper
    {
        public static int GetPower2Length(int number)
        {
            var log = Math.Log(number, 2);
            var power = (int)Math.Ceiling(log);

            return (int)Math.Pow(2, power);
        }

    }
}
