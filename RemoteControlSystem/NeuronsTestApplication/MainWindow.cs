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
            joystickData[0] -= 127.0;
            joystickData[1] -= 128.0;

            var tan = joystickData[1] != 0.0 ? -joystickData[0] / joystickData[1] : 0.0;
            var angle = Math.Atan(Double.IsNaN(tan) ? 0 : tan);
            var a = Math.Abs(angle) * 180.0 / Math.PI + (angle > 0 ? 0 : 180);
            var r = Math.Sqrt(Math.Pow(joystickData[0], 2.0) + Math.Pow(joystickData[1], 2.0));

            var inputs = data.Skip(2).Take(2).Select(Helper.MapJoystickValueToNetwork).ToArray();
            var outputs = _network.Compute(inputs).Select(Helper.MapNetworkValueToDriver).ToArray();

            Invoke(new InvokeDelegate(UpdateUI), outputs, a, r);
        }

        private void UpdateUI(int[] data, double a, double r)
        {
            knobControl.Value = Convert.ToDecimal(a);
            textBoxR.Text = r.ToString();

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
