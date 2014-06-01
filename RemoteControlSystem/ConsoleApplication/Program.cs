using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using HidLibrary;

namespace ConsoleApp
{
    class Program
    {
        private SerialPort port;


        public static void Main(String[] args)
        {
            new Program().Worker();
        }

        //HID\VID_0810&PID_0003
        private const int vendorId = 0x0810;
        private static HidDevice device;

        private void Worker()
        {
            try
            {
                if (InitializePort("COM1", 115200))
                {
                    Console.WriteLine("\nConnected to COM1.\n");
                }

                var devices = HidDevices.Enumerate(vendorId);
                if (devices != null && devices.Any())
                {
                    device = devices.ElementAt(0);
                }
                else
                {
                    port.Close();
                    Console.WriteLine("Could not find a joystick.");
                    Console.ReadKey();
                    return;
                }

                device.OpenDevice();
                device.Inserted += DeviceAttachedHandler;
                device.Removed += DeviceRemovedHandler;
                device.MonitorDeviceEvents = true;
                device.ReadReport(OnReport);

                Console.WriteLine("Joystick found and opened.");
                Thread.CurrentThread.Join();
                device.CloseDevice();
                port.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error: {0}", e.Message));
            }
            finally
            {
                if (port != null)
                {
                    device.CloseDevice();
                    port.Close();
                }
            }
        }

        private bool InitializePort(String name, int rate)
        {
            try
            {
                port = new SerialPort(name, rate, Parity.None, 8, StopBits.One);
                port.Open();
            }
            catch (System.Exception ex)
            {
                port = null;
                Console.WriteLine("Error: Don't open COM port ({0})", ex.Message);
                return false;
            }
            port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceived);
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            return true;
        }

        private void ErrorReceived(object sender, EventArgs e)
        {
            if (port.IsOpen)
            {
                Console.WriteLine("{0}: Error Received!", DateTime.Now);
            }
        }

        private void DataReceived(object sender, EventArgs e)
        {
            if (port != null && port.BytesToRead >= 6)
            {
                byte[] ar = new byte[10];

                //for (int i = 0; i < 6; i++)
                //{
                //    ar[i] = (byte)port.ReadByte();
                //}
                //port.Encoding = System.Text.Encoding.UTF8;
                //var data = port.ReadExisting();

                //for (int i = 0; i < 6; i++)
                //{
                //    ar[i] = (byte)data[i];
                //}

                //if (data.Contains("VMDPE_1|"))
                //{
                //    return;
                //}
                //Console.WriteLine("{0}: {1}", DateTime.Now.ToString("mm:ss:fff"), data);
                //Console.WriteLine("{0,3} {1,3} {2,3} {3,3} {4,3} {5,3}", ar[0], ar[1], ar[2], ar[3], ar[4], ar[5]);
            }
        }


        private void OnReport(HidReport report)
        {
            if (device.IsConnected == false) return;

            if (report.Data.Length >= 8)
            {
                byte[] data = report.Data;

                // Joystick return 8 bytes data, but used only 6 (1-7).
                var joystickData = new JoystickDataService(data);
                var buf = joystickData.ToByteArray();
                port.Write(buf, 0, buf.Length);

                //Console.WriteLine("{0,3} {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3}", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
                Console.WriteLine("{0,3} {1,3} {2,3} {3,3} {4,3} {5,3}", data[1], data[2], data[3], data[4], data[5], data[6]);
            }
            device.ReadReport(OnReport);
        }

        private void DeviceAttachedHandler()
        {
            Console.WriteLine("Joystick attached.");
            device.ReadReport(OnReport);
        }

        private void DeviceRemovedHandler()
        {
            Console.WriteLine("Joystick removed.");
        }
    }

    public struct JoystickData
    {
        public byte ForwardBack;
        public byte RightLeft;

        public byte UpDown;
        public byte RotateRightLeft;

        public byte JoyButtons;

        public byte SignalButtons;
    }

    /// <summary>
    /// Represent class for reading and parsing data;
    /// </summary>
    public class JoystickDataService
    {
        //IJoystickDataProvider;
        private JoystickData joystickData;

        public JoystickDataService(byte[] b)
        {
            ParseData(b);
        }

        private void ParseData(byte[] data)
        {
            joystickData.ForwardBack = data[1];
            joystickData.RightLeft = data[2];
            joystickData.UpDown = data[4];
            joystickData.RotateRightLeft = data[3];

            byte b5 = (byte)(data[5] - 0xF);
            byte b6 = data[6];

            // b[5](0:2) , b[6](0:3)
            int ub = b6 & 0xF;
            int ob = GetJoyButtonNumber(b5) << 4;
            joystickData.JoyButtons = (byte)(ob + ub);

            // b[6](4:5) , b[5](4:7)
            ub = (b5 & 0xF0) >> 4;
            ob = b6 & 0x30;
            joystickData.SignalButtons = (byte)(ob + ub);
        }

        private byte GetJoyButtonNumber(byte b)
        {
            switch ((b - 1) & 0x7)  // Get last 3 bits.
            {
                // 000b - 1000b
                case 0x0: return 0x8;
                // 001b - 1010b
                case 0x1: return 0xA;
                // 010b - 0010b
                case 0x2: return 0x2;
                // 011b - 0110b
                case 0x3: return 0x6;
                // 100b - 0100b
                case 0x4: return 0x4;
                // 101b - 0101b
                case 0x5: return 0x5;
                // 110b - 0001b
                case 0x6: return 0x1;
                // 111b - 1001b
                case 0x7: return 0x9;
            }
            return 0;
        }

        public byte[] ToByteArray()
        {
            byte[] array = new byte[6];

            array[0] = joystickData.ForwardBack;
            array[1] = joystickData.RightLeft;
            array[2] = joystickData.UpDown;
            array[3] = joystickData.RotateRightLeft;
            array[4] = joystickData.JoyButtons;
            array[5] = joystickData.SignalButtons;

            return array;
        }
    }
}
