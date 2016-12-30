using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using AForge.Neuro;
using JoystickLibrary.DataProviders;
using NeuronWeightsGenerator;
using Timer = System.Timers.Timer;

namespace NeuronsTestApplication
{
    public partial class MainWindow : Form
    {
        private readonly Timer _timer = new Timer(1);
        private readonly XboxJoystickDataProvider _xboxDataProvider;
        private readonly Network _network;
        private bool _canInvoke = true;

        private delegate void InvokeDelegate(int[] data, double a, double r);

        public MainWindow()
        {
            InitializeComponent();

            _xboxDataProvider = new XboxJoystickDataProvider();
            _network = Network.Load(NeuronWeightsGenerator.Program.NetworkFile);

            if (!ConnectDevice())
            {
                _timer.Elapsed += (sender, args) => ConnectDevice();
                _timer.Start();
            }
        }

        private bool ConnectDevice()
        {
            if (_xboxDataProvider.OpenDevice())
            {
                _xboxDataProvider.OnPackageAvailableEvent += OnPackageAvailable;
                _timer.Stop();

                return true;
            }

            return false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            _canInvoke = false;
        }

        #region override base of Form members

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _xboxDataProvider.OnPackageAvailableEvent -= OnPackageAvailable;
        }

        #endregion

        #region Handlers

        private void OnPackageAvailable(byte[] data)
        {
            var joystickData = data.Skip(2).Take(2).Select(Convert.ToDouble).ToArray();
            joystickData[0] = (joystickData[0] - 127.0) * (-1);
            joystickData[1] -= 128.0;

            double angle = 0;

            if (joystickData[1] != 0.0)
            {
                if (joystickData[0] != 0.0)
                {
                    var tan = Math.Abs(joystickData[0]) / Math.Abs(joystickData[1]);
                    angle = Math.Atan(Double.IsNaN(tan) ? 0.0 : tan) * 180.0 / Math.PI;

                    if (joystickData[0] > 0 && joystickData[1] < 0)  // 2 / 4
                    {
                        angle = 180.0 - angle;
                    }
                    else if (joystickData[0] < 0 && joystickData[1] < 0)  // 3 / 4
                    {
                        angle = 180.0 + angle;
                    }
                    else if (joystickData[0] < 0 && joystickData[1] > 0)  // 4 / 4
                    {
                        angle = 360.0 - angle;
                    }
                }
                else
                {
                    if (joystickData[1] > 0)
                    {
                        angle = 0;
                    }
                    if (joystickData[1] < 0)
                    {
                        angle = 180;
                    }
                }
            }
            else
            {
                if (joystickData[0] > 0)
                {
                    angle = 90;
                }
                if (joystickData[0] < 0)
                {
                    angle = 270;
                }
            }

            var r = Math.Sqrt(Math.Pow(joystickData[0], 2.0) + Math.Pow(joystickData[1], 2.0));

            var inputs = data.Skip(2).Take(2).Select(Helper.MapJoystickValueToNetwork).ToArray();
            var outputs = _network.Compute(inputs).Select(Helper.MapNetworkValueToDriver).ToArray();

            if (_canInvoke)
            {
                Invoke(new InvokeDelegate(UpdateUI), outputs, angle, r);
            }
        }

        private void UpdateUI(int[] data, double a, double r)
        {
            knobControl.Value = Convert.ToDecimal(a);
            textBoxAngle.Text = Math.Round(a, 0).ToString();
            textBoxRadius.Text = Math.Round(r, 1).ToString();

            trackBar1.Value = data[0] > 120 ? 120 : data[0];
            textBox1.Text = data[0].ToString();

            trackBar2.Value = data[1] > 120 ? 120 : data[1];
            textBox2.Text = data[1].ToString();

            trackBar3.Value = data[2] > 120 ? 120 : data[2];
            textBox3.Text = data[2].ToString();

            trackBar4.Value = data[3] > 120 ? 120 : data[3];
            textBox4.Text = data[3].ToString();
        }

        #endregion
    }
}
