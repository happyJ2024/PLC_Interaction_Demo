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

        [DispId(1001)]
        bool Connect();
               
        [DispId(1002)]
        bool Disconnect();

        [DispId(1003)]
        void AddTag(string tagName,string tagValueType);

        [DispId(1004)]
        bool ReConnect();
    }
}
