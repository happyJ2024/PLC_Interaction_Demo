using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace KepwareClientCOM
{
    [Guid("73221ff4-367c-46a3-b865-b94a7876831a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IJ2KepEvent
    {
        [DispId(2001)]
        void OnTagValueChangedEvent(string data);

        
    }
}
