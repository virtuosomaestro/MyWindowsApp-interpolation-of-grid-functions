using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
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
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.Title.Text = "График интерполяции";
            pane.Title.FontSpec.Size = 16;
            pane.XAxis.Title.Text = "Ось X";
            pane.YAxis.Title.Text = "Ось Y";
            zedGraphControl1.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zedGraphControl1.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
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
                    double ans = 0;
                    for(double i = 0; i <= eps; i++)
                    {
                        double tmp = Math.Pow(x, i);
                        if (i % 2 == 1) tmp *= -1;
                        ans += tmp;                   
                    }
                    return ans;
                default:
                    return 0;
            }
        }
        private double get_value_of_interpolant(double x, PointPairList nodes)
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
                nodes.Add(0.0, get_value_of_function(0.0));
                double step = 1.0 / (num_of_nodes - 1);
                for(int i  = 1; i < num_of_nodes; i++)
                {
                    double x = nodes.Last().X + step;
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

            int N = 100;
            double step = 1.0 / (N - 1), x = 0.0;
            for (int i = 0; i < N; i++)
            {
                list.Add(x, get_value_of_function(x));
                x += step;
            }

            LineItem myCurve = pane.AddCurve("Функция", list, Color.Gray, SymbolType.None);
            myCurve.Line.Width = 3.0F;
        }

        private void draw_nodes(GraphPane pane, PointPairList nodes)
        {
            LineItem myCurve = pane.AddCurve("Узел", nodes, Color.Red, SymbolType.Circle);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);
            myCurve.Line.Width = 8.0F;
        }
        private void draw_restored_function(GraphPane pane, PointPairList nodes)
        {
            PointPairList list = new PointPairList();

            int N = 100;
            double step = 1.0 / (N - 1), x = 0.0;
            for (int i = 0; i < N; i++)
            {
                list.Add(x, get_value_of_interpolant(x, nodes));
                x += step;
            }

            LineItem myCurve = pane.AddCurve("Интерполянт", list, Color.Green, SymbolType.None);
            myCurve.Line.Width = 3.0F;
        }

        private double calc_err(PointPairList nodes)
        {
            double max_err = 0;
            for(int i  = 0; i < num_of_nodes -1 ; i++)
            {
                double x = (nodes[i+1].X - nodes[i].X) / 2.0;
                double cur_err = Math.Abs(get_value_of_function(x) - get_value_of_interpolant(x, nodes));
                max_err = Math.Max(max_err, cur_err);
            }
            return max_err;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            eps = Convert.ToDouble(epsilon.Text);
            ind_of_problem = number_of_problem.SelectedIndex;
            ind_of_grid = grid.SelectedIndex;
            num_of_nodes = Convert.ToInt32(number_of_nodes.Text);
            PointPairList nodes = init_nodes();

            double err = calc_err(nodes); 
            label8.Text = err.ToString();

            GraphPane pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();
            
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            draw_nodes(pane, nodes);
            draw_restored_function(pane, nodes);
            draw_initial_function(pane);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

    }
}


