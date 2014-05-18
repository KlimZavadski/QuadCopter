using System;
using System.Linq;
using System.Threading;
using HidLibrary;

namespace ConsoleApp
{
    class Program
    {
        public static void Main(String[] args)
        {
            new Program().Worker();
        }

        //HID\VID_0810&PID_0003
        private const int vendorId = 0x0810;
        private static HidDevice device;

        private void Worker()
        {
            device = HidDevices.Enumerate(vendorId).ElementAt(0);

            if (device == null)
            {
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
        }

        private void OnReport(HidReport report)
        {
            if (device.IsConnected == false) return;

            if (report.Data.Length >= 8)
            {
                byte[] d = report.Data;
                ParseRawData(d);
                Console.WriteLine("{0,3} {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3}", d[0], d[1], d[2], d[3], d[4], d[5], d[6], d[7]);
                //Console.WriteLine("{0,3} {1,3} {2,3} {3,3} {4,3} {5,3}", d[1], d[2], d[3], d[4], d[5], d[6]);
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

        private void ParseRawData(byte[] data)
        {
            // Joystick return 8 bytes data, but used only 6 (1-7).
            new JoystickDataService(data);
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
            //
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
            int ob = PushJoyButtonNumber(b5) << 4;
            joystickData.JoyButtons = (byte)(ob + ub);

            // b[6](4:5) , b[5](4:7)
            ub = (b5 & 0xF0) >> 4;
            ob = b6 & 0x30;
            joystickData.SignalButtons = (byte)(ob + ub);
        }

        private byte PushJoyButtonNumber(byte b)
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
    }
}
