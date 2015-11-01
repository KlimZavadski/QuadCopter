using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;

namespace ConsoleApp
{
    internal class Program
    {
        private SerialPort port;


        public static void Main(String[] args)
        {
            new Program().Worker();
        }

        // xBox - HID\VID_054C&PID_0268
        // Playstation - HID\VID_0810&PID_0003
        private const int _vendorId = 0x054C;
        private static HidDevice _device;
        private IJoystickDataParser _joystickDataParser = new XBoxJoystickDataParser();

        private void Worker()
        {
            try
            {
                if (InitializePort("COM1", 115200))
                {
                    Console.WriteLine("\nConnected to COM1.\n");
                }

                var devices = HidDevices.Enumerate(_vendorId);
                if (devices != null && devices.Any())
                {
                    _device = devices.ElementAt(0);
                }
                else
                {
                    port.Close();
                    Console.WriteLine("Could not find a joystick.");
                    Console.ReadKey();
                    return;
                }

                _device.OpenDevice();
                _device.Inserted += DeviceAttachedHandler;
                _device.Removed += DeviceRemovedHandler;
                _device.MonitorDeviceEvents = true;

                Console.WriteLine("Joystick found and opened.");
                Thread.CurrentThread.Join();
                _device.CloseDevice();
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
                    _device.CloseDevice();
                    port.Close();
                }
            }
        }

        private bool InitializePort(String name, int rate)
        {
            try
            {
                port = new SerialPort(name, rate, Parity.None, 8, StopBits.One)
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                port.Open();
            }
            catch (Exception ex)
            {
                port = null;
                Console.WriteLine("Error: Can't open COM port ({0})", ex.Message);
                return false;
            }
            port.ErrorReceived += ErrorReceived;
            port.DataReceived += DataReceived;

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
            if (port != null && port.BytesToRead >= 0)
            {
                var ar = new byte[10];

                //for (int i = 0; i < port.BytesToRead; i++)
                //{
                //    ar[i] = port.ReadByte();
                //}
                //var data = port.ReadExisting();
                //int val1, val2;

                var len = port.Read(ar, 0, 10);
                var r = len;

                //if (int.TryParse(data.Substring(0, 1), out val1) && int.TryParse(data.Substring(1, 1), out val2))
                //{
                Console.WriteLine("Values = [{0}, {1}]", ar[0], ar[1]);
                //}

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

        private const int _reportsInPackage = 10;
        private int _count = 0;
        private JoystickData[] JoystickDataArray = new JoystickData[_reportsInPackage];

        private void OnReport(HidReport report)
        {
            if (_device.IsConnected == false)
            {
                return;
            }

            if (report.Data.Length == _joystickDataParser.DataSize)
            {
                byte[] data = report.Data;

                // xBox joystick return 48 bytes data, but used only 6 (0-1 & 4-5).
                // Playstation joystick return 8 bytes data, but used only 6 (0-6).
                // Event appears 130 times per sec. Reduce it to 10 times.
                JoystickDataArray[_count] = _joystickDataParser.Parse(report.Data);

                // Send one package with averaged data.
                if (_count == _reportsInPackage - 1)
                {
                    _count = 0;
                    var averageJoystickData = new JoystickData
                    {
                        UpDown = Convert.ToByte(JoystickDataArray.Average(x => x.UpDown)),
                        RotateLeftRight = Convert.ToByte(JoystickDataArray.Average(x => x.RotateLeftRight)),
                        ForwardBack = Convert.ToByte(JoystickDataArray.Average(x => x.ForwardBack)),
                        LeftRight = Convert.ToByte(JoystickDataArray.Average(x => x.LeftRight)),
                        Buttons = JoystickDataArray.Last().Buttons,
                        Button4PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.Button4PressingDegree)),
                        Button5PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.Button5PressingDegree)),
                        Button6PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.Button6PressingDegree)),
                        Button7PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.Button7PressingDegree)),
                        AdditionalButtons = JoystickDataArray.Last().AdditionalButtons,
                        AdditionalButton0PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton0PressingDegree)),
                        AdditionalButton1PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton1PressingDegree)),
                        AdditionalButton2PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton2PressingDegree)),
                        AdditionalButton3PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton3PressingDegree)),
                        AdditionalButton4PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton4PressingDegree)),
                        AdditionalButton5PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton5PressingDegree)),
                        AdditionalButton6PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton6PressingDegree)),
                        AdditionalButton7PressingDegree = Convert.ToByte(JoystickDataArray.Average(x => x.AdditionalButton7PressingDegree))
                    };

                    Task.Run(() =>
                    {
                        var radioCarByteArray = averageJoystickData.ToRadioCarByteArray();
                        radioCarByteArray[0] = 110;
                        radioCarByteArray[1] = 120;
                        radioCarByteArray[2] = 130;
                        radioCarByteArray[3] = 140;

                        port.Write(radioCarByteArray, 0, radioCarByteArray.Length);
                    });
                }

                //foreach (var b in data.Take(16))
                //{
                //    Console.Write("{0,3} ", b);
                //}
                //Console.WriteLine();
            }

            _count++;
            _device.ReadReport(OnReport);
        }

        private void DeviceAttachedHandler()
        {
            Console.WriteLine("Joystick attached.");
            _device.ReadReport(OnReport);
        }

        private void DeviceRemovedHandler()
        {
            Console.WriteLine("Joystick removed.");
        }
    }

    public struct JoystickData
    {
        public byte UpDown;
        public byte RotateLeftRight;

        public byte ForwardBack;
        public byte LeftRight;

        public byte Buttons;
        public byte Button4PressingDegree;
        public byte Button5PressingDegree;
        public byte Button6PressingDegree;
        public byte Button7PressingDegree;

        public byte AdditionalButtons;
        public byte AdditionalButton0PressingDegree;
        public byte AdditionalButton1PressingDegree;
        public byte AdditionalButton2PressingDegree;
        public byte AdditionalButton3PressingDegree;
        public byte AdditionalButton4PressingDegree;
        public byte AdditionalButton5PressingDegree;
        public byte AdditionalButton6PressingDegree;
        public byte AdditionalButton7PressingDegree;

        public byte[] ToRadioCarByteArray()
        {
            return new[] { Button4PressingDegree, Button6PressingDegree, Button7PressingDegree, Button5PressingDegree };
        }

        public byte[] ToQuadCopterByteArray()
        {
            return new[] { ForwardBack, LeftRight, UpDown, RotateLeftRight, Buttons, AdditionalButtons };
        }
    }

    public interface IJoystickDataParser
    {
        int DataSize { get; }

        JoystickData Parse(byte[] data);
    }

    /// <summary>
    /// Represent class for parsing data from xBox joystick;
    /// </summary>
    public class XBoxJoystickDataParser : IJoystickDataParser
    {
        public int DataSize { get { return 48; } }

        public JoystickData Parse(byte[] data)
        {
            return new JoystickData
            {
                UpDown = data[5],
                RotateLeftRight = data[4],
                ForwardBack = data[7],
                LeftRight = data[6],
                Buttons = data[0],
                Button4PressingDegree = data[12],
                Button5PressingDegree = data[13],
                Button6PressingDegree = data[14],
                Button7PressingDegree = data[15],
                AdditionalButtons = data[1],
                AdditionalButton0PressingDegree = data[17],
                AdditionalButton1PressingDegree = data[18],
                AdditionalButton2PressingDegree = data[19],
                AdditionalButton3PressingDegree = data[20],
                AdditionalButton4PressingDegree = data[21],
                AdditionalButton5PressingDegree = data[22],
                AdditionalButton6PressingDegree = data[23],
                AdditionalButton7PressingDegree = data[24]
            };
        }
    }

    /// <summary>
    /// Represent class for parsing data from Playstation joystick;
    /// </summary>
    public class PlaystationJoystickDataParser : IJoystickDataParser
    {
        public int DataSize { get { return 8; } }

        public JoystickData Parse(byte[] data)
        {
            var joystickData = new JoystickData
            {
                ForwardBack = data[1],
                LeftRight = data[2],
                UpDown = data[4],
                RotateLeftRight = data[3]
            };

            byte b5 = (byte)(data[5] - 0xF);
            byte b6 = data[6];

            // b[5](0:2) , b[6](0:3)
            int ub = b6 & 0xF;
            int ob = GetButtonNumber(b5) << 4;
            joystickData.Buttons = (byte)(ob + ub);

            // b[6](4:5) , b[5](4:7)
            ub = (b5 & 0xF0) >> 4;
            ob = b6 & 0x30;
            joystickData.AdditionalButtons = (byte)(ob + ub);

            return joystickData;
        }

        private byte GetButtonNumber(byte b)
        {
            switch ((b - 1) & 0x7) // Get last 3 bits.
            {
                // 000b - 1000b
                case 0x0:
                    return 0x8;
                // 001b - 1010b
                case 0x1:
                    return 0xA;
                // 010b - 0010b
                case 0x2:
                    return 0x2;
                // 011b - 0110b
                case 0x3:
                    return 0x6;
                // 100b - 0100b
                case 0x4:
                    return 0x4;
                // 101b - 0101b
                case 0x5:
                    return 0x5;
                // 110b - 0001b
                case 0x6:
                    return 0x1;
                // 111b - 1001b
                case 0x7:
                    return 0x9;
            }
            return 0;
        }
    }
}
