using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Kepware.ClientAce.OpcCmn;
using Kepware.ClientAce.OpcDaClient;
 

namespace KepwareClientCOM
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

        private List<Tag> defaultTagList = new List<Tag>();
        private OPCConnector connector = new OPCConnector();

        public KepwareClient()
        {
            InitDefaultTagList();
        }

        public bool Connect()
        {
            opcConnectorThread = new Thread(new ParameterizedThreadStart(RunConnector));
            opcConnectorThread.Start();

            return true;
        }
        public bool WriteTagValue(string tagName, string value)
        {
            connector.WriteTag(tagName, value);
            return true;
        }

        private void RunConnector(object obj)
        { 
            InitConnectorTagList(connector);
            connector.ConnectToServer();
            connector.SubscribeClientEvent();

            connector.OnTagValueChanged += OPCConnector_OnValueChanged;

            //Important to Start a message loop by Apllication.Run()
            Application.Run();

        }
        private void InitConnectorTagList(OPCConnector connector)
        {
            foreach (var tag in defaultTagList)
            {
                connector.TagList.Add(tag);
            }
        }

        private void InitDefaultTagList()
        {
            defaultTagList.Add(new Tag("Channel1.Device1.testFloat", "Float"));
            defaultTagList.Add(new Tag("Channel1.Device1.testBoolean", "Boolean"));
            defaultTagList.Add(new Tag("Channel1.Device1.testString", "String"));
            defaultTagList.Add(new Tag("Channel1.Device1.testInt", "Int"));
        }
        public void OPCConnector_OnValueChanged(string data)
        {
            if (OnTagValueChangedEvent != null)
            {
                Console.WriteLine("Trigger OnTagValueChangedEvent");
                AsyncCallback callback = new AsyncCallback(ValueChangedNotifyCallback);
                OnTagValueChangedEvent.BeginInvoke(data, callback, null);
            }
        }
        void ValueChangedNotifyCallback(IAsyncResult r)
        {
            Logger.Info("The value change has been been notified to interface");
        }

        public bool Disconnect()
        {
            opcConnectorThread.Abort();
            return true;
        }


        public void AddTag(string tagName, string tagValueType)
        {
            defaultTagList.Add(new Tag(tagName, tagValueType));
        }

        public bool ReConnect()
        {
            var disconnectSuccess = Disconnect();
            if (disconnectSuccess)
            {
                return Connect();
            }
            return disconnectSuccess;
        }
    }
}

