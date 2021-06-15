using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace КГ_лаба6
{
    public partial class Form1 : Form
    {
        List<int[]> Points = new List<int[]>();
        List<int[]> Border = new List<int[]>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Set()
        {
            //считывание координат многоугольника и отрезка без проверки правильности введенных данных
            //обязателен enter при вводе координат

            //многоугольник
            try
            {
                Points.Clear();
                string str = mnTB.Text;

                int pos = str.IndexOf(' ');
                int pos2 = str.IndexOf('\r');
                int X1 = Convert.ToInt32(str.Substring(0, pos));
                int Y1 = Convert.ToInt32(str.Substring(pos + 1, pos2 - pos - 1));
                int[] coordinates = { X1, Y1, 1 };
                Points.Add(coordinates);
                str = str.Substring(pos2 + 2);
                int rx1 = X1, ry1 = Y1;

                while (str != "")
                {
                    pos = str.IndexOf(' ');
                    pos2 = str.IndexOf('\r');
                    X1 = Convert.ToInt32(str.Substring(0, pos));
                    Y1 = Convert.ToInt32(str.Substring(pos + 1, pos2 - pos - 1));
                    int[] coordinates_ = { X1, Y1, 1 };
                    Points.Add(coordinates_);
                    str = str.Substring(pos2 + 2);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }

        private void Draw(List<int[]> A)
        {
            //настройка области
            GraphPane pane = zgc.GraphPane;
            //pane.XAxis.Title.Text = "Ось X";
            //pane.YAxis.Title.Text = "Ось Y";
            pane.XAxis.MajorGrid.IsZeroLine = false;
            pane.YAxis.MajorGrid.IsZeroLine = false;

            //координаты отображаемой области
            pane.XAxis.Scale.Min = -15;
            pane.XAxis.Scale.Max = 15;
            pane.YAxis.Scale.Min = -15;
            pane.YAxis.Scale.Max = 15;


            pane.CurveList.Clear();

            PointPairList list2 = new PointPairList();

            for (int i = 0; i < A.Count; ++i)
            {
                list2.Add(A[i][0], A[i][1]);
            }
            list2.Add(A[0][0], A[0][1]);
            LineItem myFig = pane.AddCurve("", list2, Color.Crimson, SymbolType.Circle);
            zgc.Invalidate();
            zgc.AxisChange();
            zgc.Refresh();
            Thread.Sleep(500);
        }

        private void Draw_Line()
        {
            //настройка области
            GraphPane pane = zgc.GraphPane;
            //pane.XAxis.Title.Text = "Ось X";
            //pane.YAxis.Title.Text = "Ось Y";
            pane.XAxis.MajorGrid.IsZeroLine = false;
            pane.YAxis.MajorGrid.IsZeroLine = false;

            //координаты отображаемой области
            pane.XAxis.Scale.Min = -15;
            pane.XAxis.Scale.Max = 15;
            pane.YAxis.Scale.Min = -15;
            pane.YAxis.Scale.Max = 15;

            //списки точек 

            PointPairList list2 = new PointPairList();

            for (int i = 0; i < Points.Count; ++i)
            {
                list2.Add(Points[i][0], Points[i][1]);
            }
            list2.Add(Points[0][0], Points[0][1]);

            LineItem myFig = pane.AddCurve("", list2, Color.Black, SymbolType.Circle);
            myFig.Line.IsVisible = false;
            zgc.Invalidate();
            zgc.AxisChange();
        }

        private void Sort(ref List<int[]> P)
        {
            for(int i = 0; i < P.Count; ++i)
            {
                for(int j = i+1; j < P.Count; ++j)
                {
                    if(P[i][0] > P[j][0])
                    {
                        int[] t = { P[i][0], P[i][1] };
                        P[i] = P[j];
                        P[j] = t;
                    }
                }
            }
        }

        private void Algorithm()
        {
            Sort(ref Points);

            List<int[]> A1 = new List<int[]>();
            List<int[]> B1 = new List<int[]>();
            Sort(ref A1);
            Sort(ref B1);
            for (int i = 0; i < Points.Count / 2; ++i)
            {
                A1.Add(new int[] { Points[i][0], Points[i][1] });
            }
            for (int i = Points.Count / 2; i < Points.Count; ++i)
            {
                B1.Add(new int[] { Points[i][0], Points[i][1] });
            }
            Border = Cut(A1, B1);
        }

        private int Multiplicate_Vector(int[] a1, int[] a2, int[] a3)
        {
            //векторное произведение
            //если оно положительно, то порядок обхода против часовой стрелки
            //return (Len(a1, a2) * Len(a2, a3) * Sin_(a1, a2, a3) > 0);
            //если все зависит только от знака синуса, то его только можно и считать
            int[] L = new int[] { a2[0] - a1[0], a2[1] - a1[1] };
            int[] R = new int[] { a2[0] - a3[0], a2[1] - a3[1] };

            return (L[0] * R[1] - L[1] * R[0]);
        }

        private List<int[]> Cut(List<int[]> A, List<int[]> B)
        {
            List<int[]> Otvet = new List<int[]>();

            if (A.Count > 3)
            {
                List<int[]> A1 = new List<int[]>();
                List<int[]> B1 = new List<int[]>();
                for (int i = 0; i < A.Count / 2; ++i)
                {
                    A1.Add(new int[] { A[i][0], A[i][1] });
                }
                for (int i = A.Count / 2; i < A.Count; ++i)
                {
                    B1.Add(new int[] { A[i][0], A[i][1] });
                }
                A = Cut(A1, B1);
            }

            if (B.Count > 3)
            {
                List<int[]> A1 = new List<int[]>();
                List<int[]> B1 = new List<int[]>();
                for (int i = 0; i < B.Count / 2; ++i)
                {
                    A1.Add(new int[] { B[i][0], B[i][1] });
                }
                for (int i = B.Count / 2; i < B.Count; ++i)
                {
                    B1.Add(new int[] { B[i][0], B[i][1] });
                }
                B = Cut(A1, B1);
            }

            //нахождение верхней касательной
            //векторное произведение больше 0
            //обход против часовой
            bool f = true;

            //Sort(ref A);
            //Sort(ref B);
            int old_s = Otvet.Count, new_s = Otvet.Count, verh_b = -1, verh_a = -1;
            for (int i = 0; i < A.Count; ++i)
            {
                for (int j = 0; j < B.Count; ++j)
                {
                    for (int k = 0; k < A.Count; ++k)
                    {
                        if (k != i) {
                            if (Multiplicate_Vector(A[i], B[j], A[k]) < 0)
                            {
                                f = false;
                                break;
                            }
                        }
                    }
                    for (int k = 0; k < B.Count; ++k)
                    {
                        if (k != j)
                        {
                            if (Multiplicate_Vector(A[i], B[j], B[k]) < 0)
                            {
                                f = false;
                                break;
                            }
                        }
                    }
                    if(f)
                    {
                        Otvet.Add(A[i]);
                        Otvet.Add(B[j]);

                        verh_b = j;
                        verh_a = i;
                        new_s = Otvet.Count;
                        break;
                    }
                    f = true;
                }
                if (old_s - new_s != 0)
                    break;
            }

            //нахождение нижней касательной
            //векторное произведение меньше 0
            //обход против часовой
            bool f2 = true;
            int old_s2 = Otvet.Count, new_s2 = Otvet.Count, niz = -1, niz_A = -1; ;
            for (int i = 0; i < A.Count; ++i)
            {
                for (int j = 0; j < B.Count; ++j)
                {
                    for (int k = 0; k < A.Count; ++k)
                    {
                        if (k != i)
                        {
                            if (Multiplicate_Vector(A[i], B[j], A[k]) > 0)
                            {
                                f2 = false;
                                break;
                            }
                        }
                    }
                    for (int k = 0; k < B.Count; ++k)
                    {
                        if (k != j)
                        {
                            if (Multiplicate_Vector(A[i], B[j], B[k]) > 0)
                            {
                                f2 = false;
                                break;
                            }
                        }
                    }
                    if (f2)
                    {
                        //Otvet.Add(B[j]);
                        niz = j;
                        niz_A = i;
                        //Otvet.Add(A[i]);
                        new_s2 = Otvet.Count;
                        break;
                    }
                    f2 = true;
                }
                if (niz!= -1 && niz_A != -1)
                    break;
            }

            //поиск самой правой тут, потому что
            //нам нужно сравнивать нижнюю+верхнюю границу и самую правую точку
            //если какие-то при проверке дадут лишнюю выпуклость, то мы их добавляем

            if(B.Count >= 3)
            {
                int right = Int32.MinValue, lm = 0;
                for(int i = 0; i < B.Count; ++i)
                {
                    if(B[i][0] > right)
                    {
                        right = B[i][0];
                        lm = i;
                    }
                }
                for(int i = 0; i < B.Count; ++i)
                {
                    if (Multiplicate_Vector(B[verh_b], B[lm], B[i]) < 0)
                        Otvet.Add(B[i]);
                }
                if (Multiplicate_Vector(B[verh_b], B[niz], B[lm]) < 0)
                    Otvet.Add(B[lm]);
                for (int i = 0; i < B.Count; ++i)
                {
                    if (Multiplicate_Vector(B[lm], B[niz], B[i]) < 0)
                        Otvet.Add(B[i]);
                }

            }
            Otvet.Add(B[niz]);
            Otvet.Add(A[niz_A]);

            //поиск самой левой точки из левого множества
            //чтобы не нарушался порядок обхода оболочки
            if (A.Count >= 3)
            {
                int left = Int32.MaxValue, lm = 0;
                for (int i = 0; i < A.Count; ++i)
                {
                    if (A[i][0] < left)
                    {
                        left = A[i][0];
                        lm = i;
                    }
                }

                for (int i = 0; i < A.Count; ++i)
                {
                    if (Multiplicate_Vector(A[lm], A[niz_A], A[i]) > 0)
                        Otvet.Add(A[i]);
                }

                if (Multiplicate_Vector(A[verh_a], A[niz_A], A[lm]) > 0)
                    Otvet.Add(A[lm]);
                for (int i = 0; i < A.Count; ++i)
                {
                    if (Multiplicate_Vector(A[verh_a], A[lm], A[i]) > 0)
                        Otvet.Add(A[i]);
                }
            }
            Draw(Otvet);
            return Otvet;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            GraphPane pane = zgc.GraphPane;
            pane.CurveList.Clear();

            Points.Clear();
            Border.Clear();
            Set();
            Algorithm();
            pane.CurveList.Clear();
            Draw(Border);
            Draw_Line();
        }
    }
}

/*2 3
5 2
8 4
4 6
7 8
10 7


1 1
4 4
6 3
9 4
12 2
13 7
10 8
6 8
2 7
11 12
4 13
5 1
3 7
4 0
2 9
-1 8
-2 -5
-3 6
8 0
4 7


*/

