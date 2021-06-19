using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace WebCamProject
{
    class WebCamera
    {
        private const int WM_CAP = 0x400;
        private const int WM_CAP_DRIVER_CONNECT = 0x40a;
        private const int WM_CAP_DRIVER_DISCONNECT = 0x40b;
        private const int WM_CAP_EDIT_COPY = 0x41e;
        private const int WM_CAP_SET_PREVIEW = 0x432;
        private const int WM_CAP_SET_OVERLAY = 0x433;
        private const int WM_CAP_SET_PREVIEWRATE = 0x434;
        private const int WM_CAP_SET_SCALE = 0x435;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;

        [DllImport("avicap32.dll")]
        protected static extern bool capGetDriverDescriptionA(int wDriverIndex, [MarshalAs(UnmanagedType.VBByRefStr)]ref String ldevicename, int namelength, [MarshalAs(UnmanagedType.VBByRefStr)] ref String deviceversion, int versionlength);
        [DllImport("avicap32.dll")]
        protected static extern int capCreateCaptureWindowA([MarshalAs(UnmanagedType.VBByRefStr)] ref string windowname,
            int style, int x, int y, int width, int height, int parent, int id);

        [DllImport("user32", EntryPoint = "SendMessageA")]
        protected static extern int SendMessage(int hwnd, int window, int x, [MarshalAs(UnmanagedType.AsAny)] object y);

        int deviceHandle;


        public static void LoadAllDevices(ListBox lstDevices)
        {
            String devicename = "".PadRight(100);
            String deviceversion = "".PadRight(100);
            lstDevices.Items.Clear();
            for (int i = 0; i <= 10; i++)
            {
                bool isDeviceReady = capGetDriverDescriptionA(i, ref devicename, 100, ref deviceversion, 100);
                if (!isDeviceReady)
                    continue;
                {
                    WebCamera d = new WebCamera(devicename, deviceversion);


                    lstDevices.Items.Add(d);
                }
            }


        }


        public System.Drawing.Image GetImage()
        {
            IDataObject data;
            System.Drawing.Image snapshot;

            SendMessage(deviceHandle, WM_CAP_EDIT_COPY, 0, 0);

            data = Clipboard.GetDataObject();
            if (data.GetDataPresent(typeof(System.Drawing.Bitmap)))
            {
                snapshot = (System.Drawing.Image)data.GetData(typeof(System.Drawing.Bitmap));
                return snapshot;
            }
            return null;
        }

        string devicename, deviceversion;

        public WebCamera(string devicename, string deviceversion)
        {
            this.devicename = devicename;
            this.deviceversion = deviceversion;

        }



        public override string ToString()
        {
            return "Device Name = " + this.devicename + " , Device Version = " + this.deviceversion;
        }

        public void StartWebCam(int height, int width, int handle, int deviceno)
        {
            string deviceIndex = "" + deviceno;
            deviceHandle = capCreateCaptureWindowA(ref deviceIndex, WS_VISIBLE | WS_CHILD, 0, 0, width, height, handle, 0);

            if (SendMessage(deviceHandle, WM_CAP_DRIVER_CONNECT, deviceno, 0) > 0)
            {
                SendMessage(deviceHandle, WM_CAP_SET_SCALE, -1, 0);
                SendMessage(deviceHandle, WM_CAP_SET_PREVIEWRATE, 0x42, 0);
                SendMessage(deviceHandle, WM_CAP_SET_PREVIEW, -1, 0);
            }
        }

        public void DisplayWebCam(PictureBox pic, int deviceno)
        {
            StartWebCam(pic.Height, pic.Width, (int)pic.Handle, deviceno);
        }


        public void StopWebCam(int deviceno)
        {
            SendMessage(deviceHandle, WM_CAP_DRIVER_DISCONNECT, deviceno, 0);
        }

    }
}