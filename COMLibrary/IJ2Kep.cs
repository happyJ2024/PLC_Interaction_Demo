using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace COMLibrary
{
    [Guid("73321ff4-367c-46a3-b865-b94a7876831a")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IJ2Kep
    {

        [DispId(4)]
        bool Connect();
               
        [DispId(5)]
        bool Disconnect();


    }
}
