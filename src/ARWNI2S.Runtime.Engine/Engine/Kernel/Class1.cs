using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ARWNI2S.Runtime.Engine.Kernel
{
    internal class NI2SDispatchProxy : DispatchProxy
    {
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
