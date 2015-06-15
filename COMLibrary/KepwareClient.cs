using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Kepware.ClientAce.OpcCmn;
using Kepware.ClientAce.OpcDaClient;
using kepwareForm;

namespace COMLibrary
{

    [Guid("76BBA445-7554-4308-8487-322BAE955527")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComDefaultInterface(typeof(IJ2Kep)),]
    [ComSourceInterfaces(typeof(IJ2KepEvent))]
    [ComVisible(true)]
    public class KepwareClient : IJ2Kep
    {
        public delegate void OnTagValueChangedDelegate(string data);
        public event OnTagValueChangedDelegate OnTagValueChangedEvent;

        private Thread opcConnectorThread;
        public bool Connect()
        {
            opcConnectorThread = new Thread(new ParameterizedThreadStart(RunConnector));
            opcConnectorThread.Start();
          
            return true;
        }

        private void RunConnector(object obj)
        {
            OPCConnector conn = new kepwareForm.OPCConnector();
            conn.OnTagValueChanged += OPCConnector_OnValueChanged;

            Application.Run(conn);
        }

        public void OPCConnector_OnValueChanged(string data)
        { 
            if (OnTagValueChangedEvent != null)
            {

                Console.WriteLine("Trigger OnTagValueChangedEvent");
                AsyncCallback callback=new AsyncCallback(callbackMethod);
                OnTagValueChangedEvent.BeginInvoke(data, callback, null);
            }
        }
        void callbackMethod(IAsyncResult r)
        {
            //Console.WriteLine("callbackMethod");
        }

        public bool Disconnect()
        {
            opcConnectorThread.Abort();
            return true;
        }
    }
}

