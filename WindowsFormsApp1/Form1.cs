using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
        private double get_value_of_function(double x, double eps, int ind_of_problem)
        {
            switch (ind_of_problem)
            {
                case 0:
                    return (1.0 - Math.Exp(-x / eps)) / (1.0 - Math.Exp(-1 / eps));
                case 1:
                    return (x * Math.Sin(Math.PI * x / eps));
                case 2:
                    if (x == 0) return 0;
                    else return x * Math.Log(1 / x);
                case 3:
                    if (0 <= x && x < 0.5 - eps) return 0.5;
                    else if (0.5 - eps <= x && x < 0.5 + eps) return (1.0 - 2.0 * x) / (4.0 * eps);
                    else return -0.5;
                case 4:
                    return x * x - eps;
                default:
                    return 0;
            }
        }

        private void draw_function(int ind_of_problem, double eps)
        {
            GraphPane pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();

            PointPairList list = new PointPairList();

            double xmin = 0;
            double xmax = 1;

            for (double x = xmin; x <= xmax; x += 0.001)
            {
                list.Add(x, get_value_of_function(x, eps, ind_of_problem));
            }

            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Green, SymbolType.None);
            myCurve.Line.Width = 3.0F;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double eps = Convert.ToDouble(epsilon.Text);
            int ind_of_problem = number_of_problem.SelectedIndex;
            draw_function(ind_of_problem, eps);
        }

    }
}


