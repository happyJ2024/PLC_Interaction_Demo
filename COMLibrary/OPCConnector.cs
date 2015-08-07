using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;
using System.Diagnostics;

using Kepware.ClientAce.OpcDaClient;
using Kepware.ClientAce.OpcCmn;
using COMLibrary;


namespace KepwareClient
{
    public partial class OPCConnector
    {
        #region event

        public delegate void OnValueChangedDelegate(string data);

        public event OnValueChangedDelegate OnTagValueChanged;

        #endregion

        #region Tag List

        public List<Tag> TagList = new List<Tag>();

        #endregion

        #region kepware fields

        DaServerMgt daServerMgt = new DaServerMgt();


        ServerIdentifier[] availableOPCServers;

        int activeServerSubscriptionHandle;

        int activeClientSubscriptionHandle;


        #endregion




        private void SubscribeToOPCDAServerEvents()
        {
            daServerMgt.DataChanged += new DaServerMgt.DataChangedEventHandler(daServerMgt_DataChanged);

        }
        public bool ConnectToServer()
        {

            Console.WriteLine("ConnectToServer");
            /// Subscribe to all the OPC DA Server events that we use
            SubscribeToOPCDAServerEvents();

            /// Create an OPC Enumerate Servers object. This will allow us to
            /// determine what OPC servers are available to us on the host PC
            /// or any other network visible machine the user may specify.
            OpcServerEnum opcServerEnum = new OpcServerEnum();

            /// Define parameters for EnumComServer method:

            /// The nodeName parameter is the host name or IP address of the PC we wish
            /// to interrogate for OPC servers. Examples are "localhost", "TESTLAB_5", 
            /// "192.168.111.10". If left empty, "localhost" is assumed. We will use the
            /// string specified by the user.
            String nodeName = "localhost";

            /// The returnAllServers parameter instructs the API to return all
            /// OPC servers found on the specified node if true, or just the
            /// servers of the types specified in serverCatagories if false.
            /// We will specify the server categories we want in this example,
            /// so this gets set to false:
            bool returnAllServers = false;

            /// The serverCatagories parameter allows us to specify which OPC server
            /// types we are interested in (OPCAE, OPCDA, OPCDX, OPCHDA, OPCXMLDA). 
            /// We will specify OPCDA only in this example:
            ServerCategory[] serverCatagories = { ServerCategory.OPCDA };

            /// Call the EnumComServer API method:
            /// (Result will be placed in availableOPCServers)
            try
            {
                opcServerEnum.EnumComServer(nodeName, returnAllServers, serverCatagories, out availableOPCServers);

                /// Handle results:

                /// Load AvailableOPCServerList list box with current search results:
                if (availableOPCServers.GetLength(0) <= 0)
                {
                    /// Let the user know that there are no servers of the specified
                    /// types found on the specified node:
                    Console.WriteLine("No OPC servers found at node: " + nodeName);

                    return false;
                }

                /// Define parameters for Connect method:

                /// The URL parameter describes the server and host we wish to connect to.
                /// The format of the URL is rather complicated. Fortunately, we don't really
                /// need to know all about that in this example. We will simply use the value
                /// stored in the availableOPCServers array for the currently selected server:
                String url = availableOPCServers[0].Url;

                /// The clientHandle allows us to give a meaningful reference number
                /// to this particular server connection. This is only used in ServerStateChanged
                /// notifications. While it is a good idea to monitor and handle server
                /// state changes, such as server shutdown, we will not do that in this
                /// simple example. Instead, we will set the connectInfo members to have
                /// the API try to keep the connection active for us. Thus, we are free 
                /// to pick an arbitrary number for our client connection handle:
                int clientHandle = 1;

                /// The connectInfo structure defines a number of connection parameters.
                /// We will describe each of them below.
                ConnectInfo connectInfo = new ConnectInfo();

                /// The LocalID member allows you to specify possible language options
                /// the server may support. We will specify "en" for english.
                connectInfo.LocalId = "en";

                /// The KeepAliveTime member is the time interval, in ms, in
                /// which the connection to the server is checked by the API.
                connectInfo.KeepAliveTime = 60000;

                /// The RetryAfterConnectionError tells the API to automatically
                /// try to reconnect after a connection loss. This is nice, so 
                /// we'll set to true:
                connectInfo.RetryAfterConnectionError = true;

                /// The RetryInitialConnection tells the API to continue to try to
                /// establish an initial connection. This is good as long as we
                /// know for sure the server is really present and will likely allow
                /// a connection at some point. if not, we could be here for a while
                /// so best to set to false:
                connectInfo.RetryInitialConnection = false;

                /// The connectFailed is set by the API. You should check this value
                /// after a to the Connect method. We will initialize it to false in case
                /// Connect throws an exception before it can be set.
                bool connectFailed = false;

                /// Call the Connect API method:
                try
                {
                    daServerMgt.Connect(url, clientHandle, ref connectInfo, out connectFailed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Handled Connect exception. Reason: " + ex.Message);

                    /// Make sure following code knows connection failed:
                    connectFailed = true;
                }

                /// Handle result:
                if (connectFailed)
                {
                    /// Tell user connection attempt failed:
                    Console.WriteLine("Connect failed");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Handled List OPC Servers exception. Reason: " + ex.Message);
                return false;
            }
            return true;
        }

        private Type ConvertType(string typeName)
        {
            switch (typeName)
            {
                case "Float":
                    return typeof(float);
                case "Boolean":
                    return typeof(Boolean);
                case "String":
                    return typeof(String);
                case "Int":
                    return typeof(Int32);
                default:
                    return typeof(String);
            }
        }

        public bool SubscribeClientEvent()
        {
            Logger.Info("SubscribeClientEvent");

            int itemIndex;

            /// Define parameters for Subscribe method:

            /// The client subscription handle is described above (see global
            /// activeServerSubscriptionHandle.) We can use an arbitrary value
            /// in this example since we will be dealing with only one subscription.
            /// if we were managing multiple subscriptions, we would want to use
            /// unique and meaningful handles.
            int clientSubscriptionHandle = 1;

            /// The active parameter is used to tells the server if we require
            /// data for the subscribed items now or not. The active state can
            /// be changed later with a call to SubscriptionModify. We will
            /// start our subscription with the state specified by user.
            bool active = true;

            /// The updateRate parameter is used to tell the server how fast we
            /// would like to see data updates. This translates roughly into how
            /// fast the server should poll the items enrolled in this subscription.
            /// This is a REQUESTED rate. The server may not be able to honor this
            /// request. This number is measured in milliseconds.
            int updateRate = System.Convert.ToInt32("500");

            /// The deadband parameter specifies the minimum deviation needed
            /// to be considered a change of value. It is expressed as a percentage
            /// (0 - 100). In a real world application, you should validate text
            /// first.
            Single deadBand = System.Convert.ToSingle("0");

            /// The itemIdentifiers array describes the items we wish to enroll 
            /// in this subscription. We will include 10 items (0 - 9). Each
            /// member of the ItemIdentifier structure will be described below.
            /// We will assume 10 tags are specified in the GUI for simplicity.
            ItemIdentifier[] itemIdentifiers = new ItemIdentifier[TagList.Count];

            for (itemIndex = 0; itemIndex < TagList.Count; itemIndex++)
            {
                itemIdentifiers[itemIndex] = new ItemIdentifier();

                /// The itemName parameter is the name of the item defined in the
                /// server. This can also be the address of a dynamic tag in
                /// KepServerEx.
                itemIdentifiers[itemIndex].ItemName = TagList[itemIndex].TagName;

                /// The ClientHandle will be used by the server to reference items in
                /// Data Changed events. This handle should uniquely identify each
                /// item enrolled in the subscription, and provide a way for us to
                /// "look up" that item.  described above, we will use the control
                /// array index for the client handle. Note, unlike other handles, which
                /// are integer types, the item client handle is an object type. This
                /// allows for a wide variety of referencing schemes. For example, this
                /// handle could be a string that concatenates a server handle, 
                /// subscription handle, and item table index. See our the complex VB
                /// .Net project for an example.
                itemIdentifiers[itemIndex].ClientHandle = itemIndex;

                /// You could request a specific data type here. We will leave the
                /// DataType member unspecified in this example so ClientAce will
                /// provide values in their canonical data type.
                itemIdentifiers[itemIndex].DataType = ConvertType(TagList[itemIndex].TagDataType);
            }

            /// The revisedUpdateRate parameter is the actual update rate that the
            /// server will be using.
            int revisedUpdateRate;

            /// Call the Subscribe API method:
            try
            {
                daServerMgt.Subscribe(clientSubscriptionHandle, active, updateRate, out revisedUpdateRate, deadBand, ref itemIdentifiers, out activeServerSubscriptionHandle);

                /// Handle result:

                /// Save the active client subscription handle for use in 
                /// DataChanged events:
                activeClientSubscriptionHandle = clientSubscriptionHandle;


                /// Check item result ID:
                for (itemIndex = 0; itemIndex < itemIdentifiers.Length; itemIndex++)
                {
                    if (itemIdentifiers[itemIndex].ResultID.Succeeded == false)
                    {
                        Logger.Info("Failed to add item " + itemIdentifiers[itemIndex].ItemName + " to subscription");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Info("Handled Subscribe exception. Reason: " + ex.Message);
                return false;
            }
            return true;
        }



        public void daServerMgt_DataChanged(int clientSubscription, bool allQualitiesGood, bool noErrors, ItemValueCallback[] itemValues)
        {
            Logger.Info("daServerMgt_DataChanged enter");

            // We need to forward the callback to the main thread of the application if we access the GUI directly from the callback. 
            //It is recommended to do this even if the application is running in the back ground.

            try
            {
                // if we were dealing with multiple subscriptions, we would want to use
                // the clientSubscripion parameter to determine which subscription this
                // event pertains to. In this simple example, we will simply validate the
                // handle.
                if (activeClientSubscriptionHandle == clientSubscription)
                {
                    // Loop over values returned. You should not assume that data
                    // for all items enrolled in a subscription will be included
                    // in every data changed event. In actuality, the number of
                    // values will likely vary each time.
                    foreach (ItemValueCallback itemValue in itemValues)
                    {
                        /// Get the item handle. We used the item's index into the
                        /// control arrays in this example.
                        int itemIndex = (int)itemValue.ClientHandle;

                        /// Update value control (Could be NULL if quaulity goes bad):

                        string tagName = "";
                        string datatype = "";
                        if (itemIndex < TagList.Count)
                        {
                            tagName = TagList[itemIndex].TagName;
                            datatype = TagList[itemIndex].TagDataType;
                        }

                        if (!string.IsNullOrEmpty(tagName))
                        {
                            var tagValue = "Unknown";
                            if (itemValue.Value != null)
                            {
                                tagValue = itemValue.Value.ToString();
                            }
                            var tagQuality = itemValue.Quality.Name;

                            var data = tagName + ":" + tagValue + ":" + tagQuality + ":" + datatype;

                            Logger.Info("Data:" + data);


                            // Event For COM
                            if (OnTagValueChanged != null)
                            {
                                //Console.WriteLine(OnValueChanged.Method.ToString());
                                Logger.Info("Invoke OnTagValueChanged");

                                OnTagValueChanged.Invoke(data);

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Handled Data Changed exception. Reason: " + ex.Message);
            }
            Logger.Info("daServerMgt_DataChanged exit");
        }




        /// <summary>
        ///  Disconnect from OPC server. The API will cancel all subscriptions 
        /// </summary>
        private void DisconnectOPCServer()
        {
            // Call Disconnect API method:
            try
            {
                if (daServerMgt.IsConnected)
                {
                    daServerMgt.Disconnect();
                }
            }
            catch (Exception e)
            {
                Logger.Info("Handled Disconnect exception. Reason: " + e.Message);
            }


        }


        /// <summary>
        /// Unsubscribe the subscribed data.
        /// </summary>
        private void Unsubscribe()
        {
            // Call SubscriptionCancel API method:
            // (Note, we are using the server subscription handle here.)
            try
            {
                daServerMgt.SubscriptionCancel(activeServerSubscriptionHandle);
            }
            catch (Exception e)
            {
                Logger.Info("Handled SubscriptionCancel exception. Reason: " + e.Message);
            }


        }


    }
}
