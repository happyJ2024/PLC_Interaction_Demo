using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace kepwareForm
{
    static class Program
    {
        public delegate void OnValueChangedDelegate(string data);
        public static event OnValueChangedDelegate OnValueChangedEvent;

        private static Thread thread;
      
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new KepwareForm());

            thread = new Thread(new ParameterizedThreadStart(OpenWorkerForm));
            thread.Start();
        }
        static void OpenWorkerForm(object obj)
        {
            OPCConnector testForm = new kepwareForm.OPCConnector();
            testForm.OnTagValueChanged += testForm_OnValueChangedEvent;

            Application.Run(testForm);
        }

        static void testForm_OnValueChangedEvent(string data)
        {
            if (OnValueChangedEvent != null)
                OnValueChangedEvent(data);
        }
    }
}
