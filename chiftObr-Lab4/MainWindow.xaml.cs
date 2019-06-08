using System;
using System.Windows;
using System.Numerics;
using System.Windows.Forms.DataVisualization.Charting;


namespace chiftObr_Lab4
{
    public partial class MainWindow : Window
    {
        int N; double T;
        System.Collections.Generic.List<double> xk = new System.Collections.Generic.List<double>();
        public MainWindow()
        {
            InitializeComponent();
            chart1.ChartAreas.Add(new ChartArea());
            for (int i = 0; i < 3; i++)// Добавляем графики
            { chart1.Legends.Add(new Legend()); chart1.Series.Add(new Series()); chart1.Series[i].BorderWidth = 4; }
            chart1.Series[0].Name = "Дискретный сигнал"; chart1.Series[1].Name = "Восстановленный сигнал"; chart1.Series[2].Name = "Точки отсчетов"; chart1.ChartAreas[0].AxisX.Minimum = 0; chart1.Series[0].ChartType = SeriesChartType.Line; chart1.Series[1].ChartType = SeriesChartType.Spline; chart1.Series[2].ChartType = SeriesChartType.Point; chart1.Series[2].MarkerSize = 8; chart1.Series[2].MarkerStyle = MarkerStyle.Circle; chart1.Legends[0].Docking = Docking.Bottom;
        }
        void Calculating_Click(object sender, RoutedEventArgs e)
        {
            if (xk_input.Text.Length < 1 || !double.TryParse(delta_input.Text, out double delta)) { MessageBox.Show("Ошибка в входных данных!"); return; }
            chart1.Series[0].Points.Clear(); chart1.Series[1].Points.Clear(); chart1.Series[2].Points.Clear(); Ci_list.Items.Clear(); xk.Clear();
            foreach (string s in xk_input.Text.Split(' '))
            {// Считываем значения xk
                if (!int.TryParse(s, out int x) || x < 0) { MessageBox.Show("Ошибка в поле ввода xk!"); return; };
                xk.Add(x);
            }
            N_label.Content = N = xk.Count;
            T = N * delta;
            for (int n = 0; n < N; n++) C(n, true, true);//Считаем и выводим коэффициенты дискретного преобразования Фурье
            int z = 0;
            for (double t = 0; t < T; t += T / N, z++) chart1.Series[1].Points.AddXY(t, xk[z]);//Строит исходный сигнал
            for (double t = 0; t < T; t += delta)
            {
                chart1.Series[0].Points.AddXY(t, xk[(int)(t / delta)]);//Строим восстановленный сигнал
                chart1.Series[2].Points.AddXY(t, xk[(int)(t / delta)]);//Строим точки отсчетов
            }
        }
        Complex X(double t)
        {
            Complex sum = C(0).Magnitude;
            for (int n = 1; n < N / 2; n++)
                sum += 2 * C(n).Magnitude * Math.Cos(2 * Math.PI * n * t / T + C(n).Phase);
            sum += Cof2(N / 2) * Math.Cos(Math.PI * N * t / T + Cof2(N / 2).Phase);
            return sum;
        }
        Complex C(int n, bool t = false, bool d = false)
        {
            Complex sum = new Complex();
            for (int k = 0; k < N; k++)
                if (n > 0) sum += xk[k] * new Complex(Math.Cos(-2 * Math.PI * n * k / N), Math.Sin(-2 * Math.PI * n * k / N));
                else sum += xk[k];
            if (t && d) Ci_list.Items.Add("C[" + n + "] = " + sum / N);
            return sum / N;
        }
        Complex Cof2(int n)
        {
            Complex sum = new Complex(0, 0);
            for (int k = 0; k < N; k++) if (n > 0) sum += xk[k] * Complex.Pow(-1, k); else sum += xk[k];
            return sum / N;
        }
        void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) DragMove(); }
    }
}