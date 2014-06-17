using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZedGraph;

namespace RemoteControlSystem
{
	public partial class RemoteControlSystem : Form
	{


		public RemoteControlSystem()
		{
			InitializeComponent();

			DrawGraph();
		}

		private void DrawGraph()
		{
			var points = new PointPairList();

			for (int i = 0; i < 255; i++)
			{
				points.Add(new PointPair((double)i, (double)MapDriverSpeed(i)));
			}

			var pane = zedGraphControl.GraphPane;
			pane.CurveList.Clear();
			pane.AddCurve("label", points, Color.Blue);
			zedGraphControl.AxisChange();
			zedGraphControl.Invalidate();
		}

		#region DriverControls

		private readonly int _idleSpeed = 87;
		private readonly double _defaultSpeed = 96.0;
		private readonly int _maxSpeed = 120;

		private int MapDriverSpeed(int value)
		{
			if (0 <= value && value < 128)
			{
				return (int)(_idleSpeed + value * (_defaultSpeed - _idleSpeed) / 128);
			}
			return (int)(_defaultSpeed + (value - 128) * (_maxSpeed - _defaultSpeed) / 255);
		}
		#endregion


		#region ViewControls

		private readonly int _joyControl_X = 200;
		private readonly int _joyControl_Y = 200;
		private readonly int _joyControl_R = 100;

		private void DrawJoyControl(Graphics g, int x, int y)
		{
			// x & y form 0 to 255.
			x = (int)(_joyControl_X + x * 100.0 / 255);
			y = (int)(_joyControl_Y + y * 100.0 / 255);

			g.DrawEllipse(Pens.Red, _joyControl_X, _joyControl_Y, _joyControl_R, _joyControl_R);
			g.DrawLine(Pens.Black, x - 5, y, x + 5, y);
			g.DrawLine(Pens.Black, x, y - 5, x, y + 5);
		}

		private void DrawMagnetControl(Graphics g, double a)
		{
			g.DrawEllipse(Pens.Red, 200, 200, 100, 100);
			float x = (float)(100 * Math.Cos(a));
			float y = (float)(100 * Math.Sin(a));
			g.DrawEllipse(Pens.Black, x, y, 1, 1);
		}
		#endregion
	}
}
