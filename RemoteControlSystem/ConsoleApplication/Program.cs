using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JoystickLibrary.DataProviders;

namespace ConsoleApplication
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            new Program().Worker();
        }

        private SerialPort _port;
        private const string _portName = "COM1";

        private XboxJoystickDataProvider _xboxDataProvider;

        private void Worker()
        {
            //if (InitializePort(_portName, 115200))
            {
                Console.WriteLine("\nConnected to {0}\n", _portName);

                _xboxDataProvider = new XboxJoystickDataProvider();

                if (_xboxDataProvider.OpenDevice())
                {
                    _xboxDataProvider.OnPackageAvailableEvent += OnPackageAvailable;

                    Console.WriteLine("Joystick found and opened.");
                    Thread.CurrentThread.Join();
                }
                else
                {
                    _port.Close();
                    Console.WriteLine("Could not find a joystick.");
                    Console.ReadKey();
                }
            }
        }

        #region COM port

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

        private void OnPackageAvailable(byte[] data)
        {
            //Task.Run(() => _port.Write(data, 0, data.Length));
            Console.WriteLine("{0,3} {1,3} {2,3} {3,3}", data[0], data[1], data[2], data[3]);
        }
    }
}
