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

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double f(double x, double eps)
        {
            return (1.0 - Math.Exp(-x / eps)) / (1.0 - Math.Exp(-1 / eps));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            double eps = Convert.ToDouble(textBox1.Text);

            GraphPane pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();

            PointPairList list = new PointPairList();

            double xmin = 0;
            double xmax = 1;

            for (double x = xmin; x <= xmax; x += 0.001)
            {
                list.Add(x, f(x, eps));
            }

            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Green, SymbolType.None);
            myCurve.Line.Width = 3.0F;

            zedGraphControl1.AxisChange();

            zedGraphControl1.Invalidate();


        }
    }
}


