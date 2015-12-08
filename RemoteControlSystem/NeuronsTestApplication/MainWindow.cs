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

        private delegate void InvokeDelegate(int[] data);

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
            var inputs = data.Skip(2).Take(2).Select(Helper.MapJoystickValueToNetwork).ToArray();
            var outputs = _network.Compute(inputs).Select(Helper.MapNetworkValueToDriver).ToArray();

            Invoke(new InvokeDelegate(UpdateUI), outputs);
        }

        private void UpdateUI(int[] data)
        {
            textBox1.Text = data[0].ToString();
            trackBar1.Value = data[0];

            textBox2.Text = data[1].ToString();
            trackBar2.Value = data[1];

            textBox3.Text = data[2].ToString();
            trackBar3.Value = data[2];

            textBox4.Text = data[3].ToString();
            trackBar4.Value = data[3];
        }

        #endregion
    }
}
