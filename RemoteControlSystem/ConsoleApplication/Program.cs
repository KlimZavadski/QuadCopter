using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;
using JoystickLibrary;
using JoystickLibrary.Parsers;

namespace ConsoleApplication
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            new Program().Worker();
        }

        private SerialPort _port;
        private readonly string _portName = "COM1";

        // xBox - HID\VID_054C&PID_0268
        // Playstation - HID\VID_0810&PID_0003
        private const int _vendorId = 0x054C;
        private static HidDevice _device;

        private void Worker()
        {
            try
            {
                if (InitializePort(_portName, 115200))
                {
                    Console.WriteLine(string.Format("\nConnected to {0}\n", _portName));
                }

                var devices = HidDevices.Enumerate(_vendorId);

                if (devices != null && devices.Any())
                {
                    _device = devices.First();
                }
                else
                {
                    _port.Close();
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            finally
            {
                if (_port != null)
                {
                    _port.Close();
                }

                if (_device != null)
                {
                    _device.CloseDevice();
                }
            }
        }

        private bool InitializePort(String name, int rate)
        {
            try
            {
                _port = new SerialPort(name, rate, Parity.None, 8, StopBits.One)
                {
                    Encoding = Encoding.UTF8
                };
                _port.Open();
                _port.ErrorReceived += ErrorReceived;
                _port.DataReceived += DataReceived;
            }
            catch (Exception ex)
            {
                _port = null;
                Console.WriteLine("Error: Can't open COM port ({0})", ex.Message);
                return false;
            }

            return true;
        }

        #region COM port handlers

        private void ErrorReceived(object sender, EventArgs e)
        {
            if (_port.IsOpen)
            {
                Console.WriteLine("{0}: Error Received!", DateTime.Now);
            }
        }

        private void DataReceived(object sender, EventArgs e)
        {
            if (_port != null && _port.BytesToRead >= 0)
            {
                var data = new byte[4];
                var len = _port.Read(data, 0, 4);

                Console.WriteLine("Values = [{0}, {1}, {2}, {3}]", data[0], data[1], data[2], data[3]);
            }
        }

        #endregion

        #region HID handlers

        private int _count;
        private const int _reportsInPackage = 10;

        private readonly JoystickData[] _joystickDataArray = new JoystickData[_reportsInPackage];
        private readonly IJoystickDataParser _joystickDataParser = new XBoxJoystickDataParser();

        private void OnReport(HidReport report)
        {
            if (_device.IsConnected == false)
            {
                return;
            }

            if (report.Data.Length == _joystickDataParser.DataSize)
            {
                // Event appears 130 times per sec. Reduce it to 13 times (about 72ms solving package).
                _joystickDataArray[_count] = _joystickDataParser.Parse(report.Data);
                _count++;

                // Send one package with averaged data.
                if (_count == _reportsInPackage)
                {
                    _count = 0;
                    var averageJoystickData = new JoystickData
                    {
                        UpDown = Convert.ToByte(_joystickDataArray.Average(x => x.UpDown)),
                        RotateLeftRight = Convert.ToByte(_joystickDataArray.Average(x => x.RotateLeftRight)),
                        ForwardBack = Convert.ToByte(_joystickDataArray.Average(x => x.ForwardBack)),
                        LeftRight = Convert.ToByte(_joystickDataArray.Average(x => x.LeftRight)),
                        Buttons = _joystickDataArray.Last().Buttons,
                        Button4PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.Button4PressingDegree)),
                        Button5PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.Button5PressingDegree)),
                        Button6PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.Button6PressingDegree)),
                        Button7PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.Button7PressingDegree)),
                        AdditionalButtons = _joystickDataArray.Last().AdditionalButtons,
                        AdditionalButton0PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton0PressingDegree)),
                        AdditionalButton1PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton1PressingDegree)),
                        AdditionalButton2PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton2PressingDegree)),
                        AdditionalButton3PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton3PressingDegree)),
                        AdditionalButton4PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton4PressingDegree)),
                        AdditionalButton5PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton5PressingDegree)),
                        AdditionalButton6PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton6PressingDegree)),
                        AdditionalButton7PressingDegree = Convert.ToByte(_joystickDataArray.Average(x => x.AdditionalButton7PressingDegree))
                    };

                    Task.Run(() =>
                    {
                        var radioCarByteArray = averageJoystickData.ToRadioCarByteArray();
                        _port.Write(radioCarByteArray, 0, radioCarByteArray.Length);
                    });
                }

//                foreach (var d in report.Data.Skip(17).Take(20))
//                {
//                    Console.Write("{0,3} ", d);
//                }
//                Console.WriteLine();
            }

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

        #endregion
    }
}
