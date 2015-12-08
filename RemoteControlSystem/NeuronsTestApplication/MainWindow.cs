using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JoystickLibrary.DataProviders;

namespace NeuronsTestApplication
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            var xboxDataProvider = new XboxJoystickDataProvider();

            if (xboxDataProvider.OpenDevice())
            {
                xboxDataProvider.OnPackageAvailableEvent += OnPackageAvailable;
            }
        }

        private void OnPackageAvailable(byte[] data)
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
    }
}
