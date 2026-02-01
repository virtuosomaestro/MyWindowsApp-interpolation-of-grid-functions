using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int ind_of_problem, ind_of_grid, num_of_nodes;
        double eps;
        public Form1()
        {
            InitializeComponent();
        }
        private double get_value_of_function(double x)
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
        private double get_value_of_restored_function(double x, PointPairList nodes)
        {
            double ans = 0;
            for (int i = 0; i < num_of_nodes; i++)
            {
                double tmp = 1;
                for (int j = 0; j < num_of_nodes; j++)
                {
                    if (i == j) continue;
                    tmp *= (x - nodes[j].X) / (nodes[i].X - nodes[j].X);
                }
                ans += tmp*nodes[i].Y;
            }
            return ans;
        }
        private PointPairList init_nodes()
        {
            PointPairList nodes = new PointPairList();
            if (ind_of_grid == 0)
            {
                double step = 1.0 / (num_of_nodes - 1);
                for(int i  = 0; i < num_of_nodes; i++)
                {
                    double x = Convert.ToDouble(i) * step;
                    nodes.Add(x , get_value_of_function(x));
                }
                return nodes;
            }
            else
            {
                for (int i = 0; i < num_of_nodes; i++)
                {
                    double x = (1.0-Math.Cos(Math.PI*(2.0*Convert.ToDouble(i)+1)/(2.0* num_of_nodes)))/2.0;
                    nodes.Add(x, get_value_of_function(x));
                }
                return nodes;
            }
        }
        private void draw_initial_function(GraphPane pane)
        {
            PointPairList list = new PointPairList();

            double xmin = 0;
            double xmax = 1;

            for (double x = xmin; x <= xmax; x += 0.001)
            {
                list.Add(x, get_value_of_function(x));
            }

            LineItem myCurve = pane.AddCurve("Initial function", list, Color.Gray, SymbolType.None);
            myCurve.Line.Width = 3.0F;
        }

        private void draw_nodes(GraphPane pane, PointPairList nodes)
        {
            LineItem myCurve = pane.AddCurve("Node", nodes, Color.Red, SymbolType.Circle);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);
            myCurve.Line.Width = 8.0F;
        }
        private void draw_restored_function(GraphPane pane, PointPairList nodes)
        {
            PointPairList list = new PointPairList();

            double xmin = 0;
            double xmax = 1;

            for (double x = xmin; x <= xmax; x += 0.001)
            {
                list.Add(x, get_value_of_restored_function(x, nodes));
            }

            LineItem myCurve = pane.AddCurve("Restored function", list, Color.Green, SymbolType.None);
            myCurve.Line.Width = 3.0F;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            eps = Convert.ToDouble(epsilon.Text);
            ind_of_problem = number_of_problem.SelectedIndex;
            ind_of_grid = grid.SelectedIndex;
            num_of_nodes = Convert.ToInt32(number_of_nodes.Text);
            PointPairList nodes = init_nodes();

            GraphPane pane = zedGraphControl1.GraphPane;
            pane.CurveList.Clear();
            draw_nodes(pane, nodes);
            draw_restored_function(pane, nodes);
            draw_initial_function(pane);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

    }
}


