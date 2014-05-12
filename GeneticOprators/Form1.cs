// Лабораторная работа №2 - Операторы кроссинговера
// Выполнила: Коваленко Анастасия
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class frmGeneticOperators : Form
    {
        public const int MIN_NUM_BIN_GEN = 5;   // Минимально допустимое число генов в двоичной хромосоме
        public const int MAX_NUM_BIN_GEN = 31;  // Максимально допустимое число генов в двоичной хромосоме
        public const int NUM_GENOV = 10;        // Число генов в числовой хромосоме (4 < N < 32)
        public const int MAX_DOWN_ROWS = 15;    // Максимальное число отображаемых элементов в выпадающих списках ComboBox
        public const string CHAR_SPC = " ";     // Символ разделяющий гены в записи числовой хромосомы
        public const string CHAR_DIV = ".";     // Символ точек скрещивания в записи числовой хромосомы
        public int Len = MIN_NUM_BIN_GEN;     // Длина хромосомы по умолчанию - количество генов (битов) 
        public int LenNum = NUM_GENOV;  // Длина числовой хромосомы - количество генов
        public double GoldProp;         // коэффициент пропорции золотого сечения (~ 0,62)
        public bool Done = false;     // Признак завершения процесса инициализации окна формы
        public int[] Order1;    // Порядок генов 1-го родителя для "упорядочивающего кроссинговера"
        public int[] Order2;    // Порядок генов 2-го родителя для "упорядочивающего кроссинговера"
        public int[] Parent1;   // Гены числовой хромосомы 1-го родителя
        public int[] Parent2;   // Гены числовой хромосомы 2-го родителя

        ErrorProvider ErrPro = new ErrorProvider(); // Позволяет показать элемент окна, в значении которого ошибка

        public frmGeneticOperators()
        {
            InitializeComponent();
        }

        private void frmGeneticOperators_Load(object sender, EventArgs e)
        {
            GoldProp = 0.5D * (Math.Sqrt(5D) - 1);     // коэффициент пропорции золотого сечения (0,618...)

            lblChromosom.Text = lblChromosom.Text.Replace("<", MIN_NUM_BIN_GEN.ToString());
            lblChromosom.Text = lblChromosom.Text.Replace(">", MAX_NUM_BIN_GEN.ToString());
            txtChromosom.Text = this.Len.ToString();     // число генов в бинарной хромосоме (по умолчанию)
            txtNumNumber.Text = this.LenNum.ToString();     // число генов в числовой хромосоме (по умолчанию)

            InitBinData(this.Len); InitNumData(this.LenNum);

            this.Width = this.ClientSize.Width - panNum.Left - (this.Width - this.ClientSize.Width) / 2;
            panNum.Left = panBin.Left;
            cboAlgorithms.SelectedIndex = 0;
            Done = true;    // процедура инициализации завершена
        }

        // Инициализация данных для двоичных хромосом
        //  Number - число генов в хромосоме
        private void InitBinData(int Number)
        {
            txtParent1.MaxLength = Number; txtParent2.MaxLength = Number;
            // Генерируем случайные начальные значения хромосом родителей
            txtParent1.Text = GetChromosom(Number);
            txtParent2.Text = GetChromosom(Number);

            // Скрываем подписи на ярлыках вкладок tabOperators
            for (int p = 0; p < tabOperators.TabCount; p++) tabOperators.TabPages[p].Text = "";
            cboOperators.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboOperators.Items.Count);
            // Заполняем выпадающие списки номеров для возможных точек скрещивания
            string[] Points = new string[Number - 1];
            for (int i = 0; i < Points.Length; i++) Points[i] = (i + 2).ToString();

            cboOp1_Point.Items.Clear();
            cboOp1_Point.Items.AddRange(Points);
            cboOp1_Point.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboOp1_Point.Items.Count);

            cboOp2_PointLeft.Items.Clear();
            cboOp2_PointLeft.Items.AddRange(Points);
            cboOp2_PointLeft.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboOp2_PointLeft.Items.Count);

            cboOp2_PointRight.Items.Clear();
            cboOp2_PointRight.Items.AddRange(Points);
            cboOp2_PointRight.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboOp2_PointRight.Items.Count);

            cboOp3_Point1.Items.Clear();
            cboOp3_Point1.Items.AddRange(Points);
            cboOp3_Point1.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboOp3_Point1.Items.Count);
            cboOp3_Point2.Items.Clear();
            cboOp3_Point2.Items.AddRange(Points);
            cboOp3_Point2.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS + 1, cboOp3_Point2.Items.Count);
            cboOp3_Point3.Items.Clear();
            cboOp3_Point3.Items.AddRange(Points);
            cboOp3_Point3.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS + 1, cboOp3_Point3.Items.Count);

            // Заполняем списки для оператора "золотого сечения" и "чисел Фибоначчи"
            lstPoints.Items.Clear();
            lstGoldPoints.Items.Clear();
            for (int i = 2; i <= Number; i++)
            {
                lstPoints.Items.Add(i.ToString());
                lstGoldPoints.Items.Add(i.ToString());
            }
            int[] Fib = GetFibonacciArray(Number); int F = 1;
            lstFibonacci.Items.Clear();
            while (Fib[F] <= Number) lstFibonacci.Items.Add(Fib[F++]);

            // Устанавливаем начальные значения в списках случайным образом 
            cboOperators.SelectedIndex = 0;
            cboOp1_Point.SelectedIndex = cboOp1_Point.Items.Count / 2;
            cboOp2_PointLeft.SelectedIndex = 0;
            cboOp2_PointRight.SelectedIndex = cboOp2_PointRight.Items.Count - 1;
            cboOp3_Point1.SelectedIndex = 0;
            cboOp3_Point2.SelectedIndex = cboOp3_Point3.Items.Count / 2;
            cboOp3_Point3.SelectedIndex = cboOp3_Point3.Items.Count - 1;
            lstFibonacci.SelectedIndex = 0;
            lstGoldVariants.SelectedIndex = 0;

            // Случайная маска для универсального кроссовера
            txtMask.Text = GetChromosom(Number); CrossUniver();

            // Создаем текстовые надписи к выпадающим спискам
            lblOp1_Point.Text = "Точка скрещивания (от 2 до " + Number.ToString() + ")";
            lblOp2_PointLeft.Text = "Левая точка скрещивания (от 2 до " + Number.ToString() + ")";
            lblOp2_PointRight.Text = "Правая точка скрещивания (от 2 до " + Number.ToString() + ")";
            lblOp3_Point1.Text = "Первая точка скрещивания (от 2 до " + Number.ToString() + ")";
            lblOp3_Point2.Text = "Вторая точка скрещивания (от 2 до " + Number.ToString() + ")";
            lblOp3_Point3.Text = "Третья точка скрещивания (от 2 до " + Number.ToString() + ")";
        }

        // Инициализация данных для числовых хромосом
        //  Number - число генов в хромосоме
        private void InitNumData(int Number)
        {
            int i;

            txtNumParent1.MaxLength = 3 * Number; txtNumParent2.MaxLength = 3 * Number;
            // Генерируем случайные начальные значения хромосом родителей
            txtNumParent1.Text = NumsToText(Parent1 = GetOrderNumChrom(Number), CHAR_SPC);
            txtNumParent2.Text = NumsToText(Parent2 = GetRandomNumChrom(Number), CHAR_SPC);

            // Скрываем подписи на ярлыках вкладок tabNumOperators
            for (i = 0; i < tabNumOperators.TabCount; i++) tabNumOperators.TabPages[i].Text = "";
            cboNumOperators.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboNumOperators.Items.Count);

            // Заполняем выпадающие списки номеров для возможных точек скрещивания
            string[] NumPoints = new string[Number - 1];
            for (i = 0; i < NumPoints.Length; i++) NumPoints[i] = (i + 2).ToString();
            cboNumOp1_Point.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboNumOperators.Items.Count - 1);
            cboNumOp1_Point.Items.Clear();
            cboNumOp1_Point.Items.AddRange(NumPoints);

            cboNumOp2_PointLeft.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboNumOperators.Items.Count - 2);
            cboNumOp2_PointLeft.Items.Clear();
            cboNumOp2_PointLeft.Items.AddRange(NumPoints);
            cboNumOp2_PointRight.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS + 1, cboNumOperators.Items.Count - 1);
            cboNumOp2_PointRight.Items.Clear();
            cboNumOp2_PointRight.Items.AddRange(NumPoints);

            cboNumOp3_Point.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboNumOperators.Items.Count - 1);
            cboNumOp3_Point.Items.Clear();
            cboNumOp3_Point.Items.AddRange(NumPoints);

            cboNumOp3_PointLeft.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS, cboNumOperators.Items.Count - 2);
            cboNumOp3_PointLeft.Items.Clear();
            cboNumOp3_PointLeft.Items.AddRange(NumPoints);
            cboNumOp3_PointRight.MaxDropDownItems = Math.Min(MAX_DOWN_ROWS + 1, cboNumOperators.Items.Count - 1);
            cboNumOp3_PointRight.Items.Clear();
            cboNumOp3_PointRight.Items.AddRange(NumPoints);

            string[] NumCyclePoints = new string[Number];
            for (i = 0; i < NumCyclePoints.Length; i++) NumCyclePoints[i] = (i + 1).ToString();
            cboBeginPoint.Items.Clear();
            cboBeginPoint.Items.AddRange(NumCyclePoints);

            // Выбираем начальные значения точек скрещивания (по умолчанию)
            cboNumOperators.SelectedIndex = 0;
            cboNumOp1_Point.SelectedIndex = Number / 2;
            cboNumOp2_PointLeft.SelectedIndex = Number / 3;
            cboNumOp2_PointRight.SelectedIndex = 2 * Number / 3;
            cboNumOp3_Point.SelectedIndex = Number / 2;
            cboNumOp3_PointLeft.SelectedIndex = Number / 3;
            cboNumOp3_PointRight.SelectedIndex = 2 * Number / 3;
            cboBeginPoint.SelectedIndex = Number / 2;

            // Создаем текстовые надписи к выпадающим спискам
            lblNumOp1_Point.Text = "Точка скрещивания (от 2 до " + this.Len.ToString() + ")";
            lblNumOp2_PointLeft.Text = "Левая точка скрещивания (от 2 до " + this.Len.ToString() + ")";
            lblNumOp2_PointRight.Text = "Правая точка скрещивания (от 2 до " + this.Len.ToString() + ")";
            lblNumOp2_Point.Text = "Точка скрещивания (от 2 до " + this.Len.ToString() + ")";
            lblNumOp3_PointLeft.Text = "Левая точка скрещивания (от 2 до " + this.Len.ToString() + ")";
            lblNumOp3_PointRight.Text = "Правая точка скрещивания (от 2 до " + this.Len.ToString() + ")";

            // Данные "жадного" оператора кроссовера
            GetRandomMatrix(this.LenNum, this.LenNum);
            Random Rnd = new Random();
            lstStartPoint.Items.Clear();
            for (i = 1; i <= this.LenNum; i++) lstStartPoint.Items.Add(i.ToString());
            lstStartPoint.SelectedIndex = Rnd.Next(this.LenNum);
            while (lstStartPoint.SelectedIndices.Count<2) lstStartPoint.SelectedIndex = Rnd.Next(this.LenNum);
        }

        // Генератор случайных значений матрицы смежности в задаче коммивояжера
        // Num - число строк (столбцов в матрице смежности)
        // Max - максимальная длина пути между смежными вершинами (> 0)
        private void GetRandomMatrix(int Num, int Max)
        {
            int r, c;
            Random Rnd = new Random();
            datMatrix.Rows.Add(Num);    // создаем необходимое число строк матрицы
            datMatrix.RowCount = Num;   // показываем только строки данных матрицы (без строки шаблона)
            datMatrix.ShowEditingIcon = false;  // скрываем глиф, указывающий текущую выбранную строку
            datMatrix.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (r = 0; r < Num; r++)
            {
                for (c = 0; c < Num; c++)
                {
                    if (c == r)
                        datMatrix.Rows[r].Cells[c].Value = 0;   // диагональный элемент
                    else
                        datMatrix.Rows[r].Cells[c].Value = Rnd.Next(Max) + 1;
                }
                //datMatrix.Rows[r].InheritedStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                datMatrix.Rows[r].HeaderCell.Value = r.ToString().PadRight(2);    // заголовок (номер) строки
            }
        }

        private void cboOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOperators.SelectedIndex >= 0)
            {
                // Показываем, выбранную из раскрывающегося списка вкладку в tabOperators, отображающую работу оператора 
                if (tabOperators.TabCount > cboOperators.SelectedIndex)
                    tabOperators.SelectedIndex = cboOperators.SelectedIndex;
                else
                    MessageBox.Show("Число закладок операторов меньше, чем в списке выбора!");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (AskExitProg()) this.Close();
        }

        // Генератор хромосомы 1-го родителя
        private void btnGenerateParent1_Click(object sender, EventArgs e)
        {
            txtParent1.Text = GetChromosom(this.Len); Crossing();
        }

        private void btnGenerateParent2_Click(object sender, EventArgs e)
        {
            txtParent2.Text = GetChromosom(this.Len); Crossing();
        }

        // Генератор случайной хромосомы длиной N генов (битов)
        private string GetChromosom(int N)
        {
            int B = 1, C = 0; while (++C < N) B = (B << 1) | 1;
            Random GenRnd = new Random();
            // Возвращаем N-битную строку двоичных цифр
            return Convert.ToString(GenRnd.Next(B), 2).PadLeft(N, '0');
        }

        private void cboOp1_Point_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOp1_Point.SelectedIndex >= 0) Cross1();
        }

        // Одноточечный кроссовер
        private void Cross1()
        {
            if (cboOp1_Point.SelectedItem == null) return;
            int P = Convert.ToInt32(cboOp1_Point.SelectedItem.ToString(), 10);
            string P1 = txtParent1.Text, P2 = txtParent2.Text;
            string C1, C2;
            if (P1.Length == this.Len && P2.Length == this.Len)
            {
                OperCross1(P1, P2, P, out C1, out C2);
                int[] X = { P };
                txtOp1_Child1.Text = ShowChild(C1, X);
                txtOp1_Child2.Text = ShowChild(C2, X);
            }
            else
                ErrorParents();
        }

        // Двухточечный кроссовер
        private void Cross2()
        {
            if (cboOp2_PointLeft.SelectedItem == null || cboOp2_PointRight.SelectedItem == null) return;
            int X1 = Convert.ToInt32(cboOp2_PointLeft.SelectedItem.ToString(), 10);
            int X2 = Convert.ToInt32(cboOp2_PointRight.SelectedItem.ToString(), 10);
            string P1 = txtParent1.Text, P2 = txtParent2.Text;
            string C1, C2;
            if (P1.Length == this.Len && P2.Length == this.Len)
            {
                if (X1 < X2)
                {
                    OperCross2(P1, P2, X1, X2, out C1, out C2);
                    int[] X = { X1, X2 };
                    txtOp2_Child1.Text = ShowChild(C1, X);
                    txtOp2_Child2.Text = ShowChild(C2, X);
                }
                else
                    ErrorPoints();
            }
            else
                ErrorParents();
        }

        // Трехточечный кроссовер
        private void Cross3()
        {
            if (cboOp3_Point1.SelectedItem == null ||
                cboOp3_Point2.SelectedItem == null ||
                cboOp3_Point3.SelectedItem == null) return;
            int X1 = Convert.ToInt32(cboOp3_Point1.SelectedItem.ToString(), 10);
            int X2 = Convert.ToInt32(cboOp3_Point2.SelectedItem.ToString(), 10);
            int X3 = Convert.ToInt32(cboOp3_Point3.SelectedItem.ToString(), 10);
            string P1 = txtParent1.Text, P2 = txtParent2.Text;
            string C1, C2;
            if (P1.Length == this.Len && P2.Length == this.Len)
            {
                if (X1 < X2 && X2 < X3)
                {
                    // Операция производится с помощью двух одноточечных кроссоверов
                    // путем деления хромосомы в точке X2 на 2 части
                    int X = X2 - 1;
                    int[] XX = { X1, X2, X3 };
                    OperCross1(P1.Substring(0, X), P2.Substring(0, X), X1, out C1, out C2);
                    txtOp3_Child1.Text = C1;
                    txtOp3_Child2.Text = C2;
                    OperCross1(P1.Substring(X), P2.Substring(X), X3 - X, out C1, out C2);
                    txtOp3_Child1.Text = ShowChild(txtOp3_Child1.Text += C1, XX);
                    txtOp3_Child2.Text = ShowChild(txtOp3_Child2.Text += C2, XX);
                }
                else
                    ErrorPoints();
            }
            else
                ErrorParents();
        }

        // Универсальный кроссовер
        private void CrossUniver()
        {
            // Заполняем перечень точек разрыва lstPoints.Items на основании заданной маски
            char Gen = txtMask.Text[0];
            for (int G = 1; G < txtMask.TextLength; G++)
            {
                if (txtMask.Text[G] != Gen)
                {
                    lstPoints.SetSelected(G - 1, true);
                    Gen = txtMask.Text[G];
                }
                else
                    lstPoints.SetSelected(G - 1, false);
            }
            if (lstPoints.SelectedItems.Count == 0)
            {
                txtOp4_Child1.Text = "";
                txtOp4_Child2.Text = "";
            }
            else
            {
                string C1, C2;
                CrossMulti(txtParent1.Text, txtParent2.Text, lstPoints.SelectedItems, out C1, out C2);
                // Результат
                int[] XX = new int[lstPoints.SelectedItems.Count];
                for (int i = 0; i < XX.Length; i++) XX[i] = Convert.ToInt32(lstPoints.SelectedItems[i]);
                if (txtMask.Text[0] == '0')
                {
                    txtOp4_Child1.Text = ShowChild(C1, XX);
                    txtOp4_Child2.Text = ShowChild(C2, XX);
                }
                else if (txtMask.Text[0] == '1')
                {
                    txtOp4_Child1.Text = ShowChild(C2, XX);
                    txtOp4_Child2.Text = ShowChild(C1, XX);
                }
            }
        }

        // Кроссовер с использованием пропорции золотого сечения
        private void CrossGolden()
        {
            if (lstGoldPoints.SelectedItems.Count == 0)
            {
                txtOp5_Child1.Text = "";
                txtOp5_Child2.Text = "";
            }
            else
            {
                string C1, C2;
                CrossMulti(txtParent1.Text, txtParent2.Text, lstGoldPoints.SelectedItems, out C1, out C2);
                // Результат
                int[] XX = new int[lstGoldPoints.SelectedItems.Count];
                for (int i = 0; i < XX.Length; i++) XX[i] = Convert.ToInt32(lstGoldPoints.SelectedItems[i]);
                txtOp5_Child1.Text = ShowChild(C1, XX);
                txtOp5_Child2.Text = ShowChild(C2, XX);
            }
        }

        // Кроссовер с использованием чисел Фибоначчи
        private void CrossFibonacci()
        {
            if (lstFibonacci.SelectedItems.Count == 0)
            {
                txtOp6_Child1.Text = "";
                txtOp6_Child2.Text = "";
            }
            else
            {
                string C1, C2;
                CrossMulti(txtParent1.Text, txtParent2.Text, lstFibonacci.SelectedItems, out C1, out C2);
                // Результат
                int[] XX = new int[lstFibonacci.SelectedItems.Count];
                for (int i = 0; i < XX.Length; i++) XX[i] = Convert.ToInt32(lstFibonacci.SelectedItems[i]);
                txtOp6_Child1.Text = ShowChild(C1, XX);
                txtOp6_Child2.Text = ShowChild(C2, XX);
            }
        }

        // Многоточечный кроссовер
        // P1 и P2 - хромосомы родителей
        // SelItems - перечень выбранных в списке ListBox точек скрещивания хромосом
        // С1 и С2 - возвращаемые хромосомы потомков
        private void CrossMulti(string P1, string P2, ListBox.SelectedObjectCollection SelItems,
                                out string C1, out string C2)
        {
            int B = 0, C;
            C1 = ""; C2 = "";
            if (P1.Length == this.Len && P2.Length == this.Len)
            {
                for (C = 0; C < SelItems.Count; C++)
                {
                    int X = Convert.ToInt32(SelItems[C]) - 1;
                    if ((C & 0x1) == 0x1)     // Нечетная точка
                    {
                        C1 += P2.Substring(B, X - B);
                        C2 += P1.Substring(B, X - B);
                    }
                    else
                    {
                        C1 += P1.Substring(B, X - B);
                        C2 += P2.Substring(B, X - B);
                    }
                    B = X;
                }
                if (SelItems.Count > 0)
                {
                    if ((C & 0x1) == 0x1)     // Нечетная точка
                    {
                        C1 += P2.Substring(B);
                        C2 += P1.Substring(B);
                    }
                    else
                    {
                        C1 += P1.Substring(B);
                        C2 += P2.Substring(B);
                    }
                }
            }
            else
                ErrorParents();
        }

        // Одноточечный упорядочивающий кроссовер
        // P1 и P2 - скрещиваемые хромосомы родителей
        // X - точка скрещивания хромосом
        private void CrossOrderOne(string P1, string P2, int X, int[] Order1, int[] Order2, out string C1, out string C2)
        {
            C1 = P1.Substring(0, X - 1);
            C2 = P2.Substring(0, X - 1);
            int i, j;
            for (i = 0; i < P2.Length; i++)
            {
                for (j = 0; j < C1.Length; j++)
                {
                    if (Order1[j] == Order2[i]) break;
                }
                if (j == C1.Length) C1 += P2.Substring(i, 1);
            }
            for (i = 0; i < P1.Length; i++)
            {
                for (j = 0; j < C2.Length; j++)
                {
                    if (Order2[j] == Order1[i]) break;
                }
                if (j == C2.Length) C2 += P1.Substring(i, 1);
            }
        }

        // Выполняет одноточечный оператор кроссовера
        // P1 и P2 - скрещиваемые хромосомы родителей
        // P - точка скрещивания хромосом
        // С1 и С2 - возвращаемые хромосомы потомков
        // (предполагается, что все параметры корректны)
        private void OperCross1(string P1, string P2, int P, out string C1, out string C2)
        {
            // При скрещивании для потомка C1 берется левая часть хромосомы P1 левее позиции P
            // и добавляется правая часть хромосомы P2. Для второго потомка C2 объединяются
            // оставшиеся части хромосом родителей.
            int X = P - 1;
            C1 = String.Concat(P1.Substring(0, X), P2.Substring(X));
            C2 = String.Concat(P2.Substring(0, X), P1.Substring(X));
        }

        // Выполняет двухточечный оператор кроссовера
        // X1 и X2 - точки скрещивания хромосом (X1 < X2)
        // P1 и P2 - скрещиваемые хромосомы родителей
        // При скрещивании для потомка C1 берется левая часть хромосомы P1 левее позиции P
        // и добавляется правая часть хромосомы P2. Для второго потомка C2 объединяются
        // оставшиеся части хромосом родителей.
        // Результат скрещивания хромосом родителей - возвращаются потомки C1 и C2 
        // (предполагается, что все параметры корректны)
        private void OperCross2(string P1, string P2, int X1, int X2, out string C1, out string C2)
        {
            C1 = ""; C2 = "";
            if (X1 < X2)
            {
                int X = X2 - 1;
                // Меняем местами первые две части генотипа хромосомы
                OperCross1(P1.Substring(0, X), P2.Substring(0, X), X1, out C1, out C2);
                // Меняем местами 3-ю (правую) часть хромосом
                C1 += P2.Substring(X);
                C2 += P1.Substring(X);
            }
            else
                ErrorPoints();
        }

        private void cboOp3_Point1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done && cboOp3_Point1.SelectedIndex >= 0) Cross3();
        }

        private void cboOp3_Point2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done && cboOp3_Point2.SelectedIndex >= 0) Cross3();
        }

        private void cboOp3_Point3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done && cboOp3_Point3.SelectedIndex >= 0) Cross3();
        }

        private void cboOp2_PointLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done && cboOp2_PointLeft.SelectedIndex >= 0) Cross2();
        }

        private void cboOp2_PointRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done && cboOp2_PointRight.SelectedIndex >= 0) Cross2();
        }

        private void frmGeneticOperators_Shown(object sender, EventArgs e)
        {
            Crossing();
        }

        // Выполняет все операции кроссовера для заданной пары родителей
        private void Crossing()
        {
            if (Done)
            { 
                Cross1(); Cross2(); Cross3(); CrossUniver(); CrossGolden(); CrossFibonacci();
                NumberCrossing();
            }
        }

        private void lstPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done) CrossUniver();
        }

        private void ErrorPoints()
        {
            if (Done) MessageBox.Show("Неверно заданы точки кроссовера!");
        }

        private void ErrorParents()
        {
            if (Done) MessageBox.Show("Неверно заданы хромосомы родителей!");
        }

        private void ErrorMask()
        {
            if (Done) MessageBox.Show("Неверно задана маска универсального кроссинговера!");
        }

        // Проверка орфографии в заданной величине хромосомы или маски
        //  Должно быть двоичное число длиной Len знаков (генов)
        // Value - проверяемая величина
        // Функция возвращает True, если Value имеет допустимое значение
        private bool CheckChromosom(string Value)
        {
            if ((Value = Value.Trim()).Length != this.Len) return false;
            for (int i = 0; i < Value.Length; i++)
                if (Value[i] != '0' && Value[i] != '1') return false;
            return true;
        }

        // Проверка корректности числа генов в двоичных хромосомах
        // Value - проверяемая величина
        private bool CheckNumberGens(string Value)
        {
            UInt32 ResultValue;
            if (UInt32.TryParse(Value, out ResultValue))
                return !(ResultValue < MIN_NUM_BIN_GEN || ResultValue > MAX_NUM_BIN_GEN);
            else 
                return false;
        }

        private void txtParent1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txtParent2.Focus();
        }

        private void txtParent2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) cboOperators.Focus();
        }

        private void txtMask_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) cboOperators.Focus();
        }

        // Генерирует случайный порядок генов "упорядочивающего кроссинговера" для 1-го родителя
        private void btnOrder1_Click(object sender, EventArgs e)
        {
            Order1 = GetOrderGenom(this.LenNum);
//            txtOrderGenom1.Text = "";
//            for (int i = 0; i < Order1.Length; i++) txtOrderGenom1.Text += Order1[i].ToString() + " ";
//            txtOrderGenom1.Text.TrimEnd();
        }

        // Генерирует случайный порядок генов "упорядочивающего кроссинговера" для 2-го родителя
        private void btnOrder2_Click(object sender, EventArgs e)
        {
            Order2 = GetOrderGenom(this.LenNum);
//            txtOrderGenom2.Text = "";
//            for (int i = 0; i < Order2.Length; i++) txtOrderGenom2.Text += Order2[i].ToString() + " ";
//            txtOrderGenom2.Text.TrimEnd();
        }

        // Возвращает порядковые номера генов для "упорядочивающего кроссинговера"
        //  N - длина хромосомы (количество генов)
        private int[] GetOrderGenom(int N)
        {
            int[] Order = new int[N];
            Random GenRnd = new Random();
            int i, i1, i2, L = Order.Length, Tmp;
            for (i = 0; i < L; i++) Order[i] = i + 1;
            for (i = 0; i < L; i++)
            {
                i1 = GenRnd.Next(L); i2 = GenRnd.Next(L);
                if (i1 != i2) { Tmp = Order[i1]; Order[i1] = Order[i2]; Order[i2] = Tmp; }
            }
            return Order;
        }

        private void cboOp5_Point_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void frmGeneticOperators_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !AskExitProg();
        }

        private bool AskExitProg()
        {
            if (MessageBox.Show("Завершить работу с программой?", "Закрыть окно",
                                MessageBoxButtons.YesNo) == DialogResult.Yes) return true;
            return false;
        }

        // Возвращает N первых членов ряда Фибоначчи
        // N - число, определяющее количество возвращаемых членов ряда (N > 0)
        //  Числа Фибоначчи — это элементы числовой последовательности 1, 1, 2, 3, 5, 8, 13... ,
        //  в которой каждый последующий член равен сумме двух предыдущих, а сама эта последовательность
        //  называется рядом Фибоначчи. Возвращаемая последовательность начинается с членов 1 и 2.
        private int[] GetFibonacciArray(int N)
        {
            int[] F = new int[N];   F[0] = 1; F[1] = 2;
            int i = 2;
            while (i < N) { F[i] = F[i - 1] + F[i - 2]; i++; }
            return F;
        }

        // Очищает поля результатов скрещивания (ввиду изменения исходных данных)
        private void ClearResult()
        {
            txtOp1_Child1.Text = ""; txtOp1_Child2.Text = "";
            txtOp2_Child1.Text = ""; txtOp2_Child2.Text = "";
            txtOp3_Child1.Text = ""; txtOp3_Child2.Text = "";
            txtOp4_Child1.Text = ""; txtOp4_Child2.Text = "";
            txtOp5_Child1.Text = ""; txtOp5_Child2.Text = "";
            txtOp6_Child1.Text = ""; txtOp6_Child2.Text = "";
        }

        private void txtParent1_TextChanged(object sender, EventArgs e)
        {
            ClearResult();
        }

        private void txtParent2_TextChanged(object sender, EventArgs e)
        {
            ClearResult();
        }

        private void txtMask_TextChanged(object sender, EventArgs e)
        {
            txtOp4_Child1.Text = ""; txtOp4_Child2.Text = "";
        }

        // Возвращает потомка с отметками точек скрещивания
        // Child - хромосома потомка
        // XX - массив точек скрещивания (номера точек от 1 до ..., которые должны следовать в порядке возрастания)
        private string ShowChild(string Child, int[] XX)
        {
            int B = 0;
            string C = "";
            for (int i = 0; i < XX.Length; i++)
            {
                C += Child.Substring(B, XX[i] - B - 1) + "."; B = XX[i] - 1;
            }
            return C += Child.Substring(B);
        }

        // Возвращает потомка числовой хромосомы с отметками точек скрещивания
        // Child - гены числовой хромосомы потомка
        // X - массив точек скрещивания (номера точек от 1 до ..., которые должны следовать в порядке возрастания)
        private string ShowNumbChild(int[] Child, int[] X)
        {
            int B = 0;
            bool T;
            string C = "";
            for (int i = 0; i < Child.Length; i++)
            {
                T = (i == X[B] - 1) ? true : false; if (T && B < X.Length - 1) B++;
                C += (T ? CHAR_DIV : CHAR_SPC) + Child[i].ToString();
            }
            return C.TrimStart();
        }

        private void lstFibonacci_SelectedIndexChanged(object sender, EventArgs e)
        {
            Crossing();
        }

        private void cboAlgorithms_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboAlgorithms.SelectedIndex)
            {
                case 0:
                    panBin.Visible = true; panNum.Visible = false;
                    break;
                case 1:
                    panNum.Visible = true; panBin.Visible = false;
                    break;
                default:
                    MsgSelectInvalid();
                    break;
            }
        }

        private void btnNumParent1_Click(object sender, EventArgs e)
        {
            Parent1 = GetRandomNumChrom(this.LenNum);
            txtNumParent1.Text = NumsToText(Parent1, CHAR_SPC);
            NumberCrossing();
        }

        private void btnNumParent2_Click(object sender, EventArgs e)
        {
            Parent2 = GetRandomNumChrom(this.LenNum);
            txtNumParent2.Text = NumsToText(Parent2, CHAR_SPC);
            NumberCrossing();
        }

        private void cboNumOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboNumOperators.SelectedIndex >= 0)
            {
                // Показываем, выбранную из раскрывающегося списка вкладку в tabNumOperators,
                // отображающую работу оператора кроссинговера
                if (tabNumOperators.TabCount > cboNumOperators.SelectedIndex)
                    tabNumOperators.SelectedIndex = cboNumOperators.SelectedIndex;
                else
                    MessageBox.Show("Число закладок операторов меньше, чем в списке выбора!");
            }
        }

        private void MsgSelectInvalid()
        {
            MessageBox.Show("ДОЛЖЕН БЫТЬ ВЫБРАН ОДИН ИЗ ПРЕДЛАГАЕМЫХ В СПИСКЕ ВАРИАНТОВ!",
                            "Ошибка выбора", MessageBoxButtons.OK);
        }

        // Возвращает массив генов для заданной строки числовой хромосомы 
        // Text - анализируемая строка текста, которую нужно преобразовать в массив 
        //  При преобразовании проверяется корректность записи строки Text,
        //  если она имеет неверный формат - возвращается null.
        private int[] GetNumberGenom(string Text)
        {
            int[] GenNums = TextToNums(Text); if (GenNums == null) return null;
            // В хромосоме не должно быть двух одинаковых генов
            int i, j;
            for (i = 0; i < GenNums.Length; i++)
            {
                if (GenNums[i] < GenNums.Length) continue;
                MessageBox.Show("Величина гена вне диапазона 0 ... " + (GenNums.Length - 1).ToString() + "!");
                return null;
            }
                for (i = 0; i < GenNums.Length - 1; i++)
            {
                for (j = i + 1; j < GenNums.Length; j++)
                {
                    if (GenNums[i] == GenNums[j])
                    {
                        MessageBox.Show("В числовой хромосоме не должно быть двух одинаковых генов!");
                        return null;
                    }
                }
            }
            if (GenNums.Length != this.LenNum)
            {
                MessageBox.Show("Число генов в хромосоме не равно заданному - " + this.LenNum.ToString() + "!");
                return null;
            }
            return GenNums;
        }

        // Возвращает упорядоченные гены для числовой хромосомы
        //  Num - число генов в хромосоме (должно быть > 0)
        //  Номера генов начинаются с 0.
        private int[] GetOrderNumChrom(int Num)
        {
            int[] Nums=new int[Num];
            for (int i = 0; i < Num; i++) Nums[i] = i;
            return Nums;
        }

        // Возвращает случайную последовательность генов числовой хромосомы (гены не повторяются)
        //  Num - число генов в хромосоме (должно быть > 0)
        //  Гены имеют значения в интервале от 0 до Num-1.
        private int[] GetRandomNumChrom(int Num)
        {
            int[] Nums = GetOrderNumChrom(Num);
            Random GenRnd = new Random();
            int G1, G2, Gen;
            // Делаем случайные перестановки пар генов в хромосоме
            for (int i = 0; i < 2 * Num; i++)
            {
                G1 = GenRnd.Next(Num); G2 = GenRnd.Next(Num);
                if (G1 != G2)
                {
                    Gen = Nums[G1]; Nums[G1] = Nums[G2]; Nums[G2] = Gen;
                }
            }
            return Nums;
        }

        // Преобразует строку текста, содержащую номера числовых хромосом в массив целых чисел
        // Text - анализируемая строка текста, которую нужно преобразовать в массив 
        //  Возвращается массив целых чисел, расположенные в том же порядке, что и в проанализированной строке Text.
        //  Если строка Text не содержит символов отличных от CHAR_SPC или в ней представлены недопустимые записи
        //  целых чисел (а также отрицательные целые), то возвращается null.
        private int[] TextToNums(string Text)
        {
            int i;
            string[] StrNum = ((Text.Replace(CHAR_SPC + CHAR_SPC, CHAR_SPC)).Trim()).Split(CHAR_SPC[0]);
            int[] Nums = new int[StrNum.Length];
            for (i = 0; i < StrNum.Length; i++)
            {
                try
                {
                    Nums[i] = Convert.ToInt32(StrNum[i]);
                }
                catch
                {
                    MessageBox.Show("Невозможно преобразовать <" + StrNum[i] + "> в целое число!");
                    break;
                }
                if (Nums[i] < 0)
                {
                    MessageBox.Show("Ген числовой хромосомы не может быть отрицательным!");
                    break;
                }
            }
            if (i < StrNum.Length)
                return null;
            else
                return Nums;
        }

        // Возвращает строку, которая содержит перечень генов в числовой хромосоме
        // Numbs - массив генов числовой хромосомы
        // Char - символ разделяющий гены в полученной записи строки хромосомы
        private string NumsToText(int[] Numbs, string Char)
        {
            string C = "";
            for (int i = 0; i < Numbs.Length; i++) C += Convert.ToString(Numbs[i]) + Char;
            return C.Substring(0, C.Length - 1);
        }

        private void cboNumOp1_Point_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboNumOp1_Point.SelectedIndex >= 0) NumberCrossing1(); 
        }

        // Выполнение операций над числовыми хромосомами
        private void NumberCrossing()
        {
            if (Done)
            { 
                NumberCrossing1(); NumberCrossing2(); NumberCrossing3(); NumberCrossing4(); NumberCrossing5();
                Greeding();     // Оператор "жадного" кроссовера
            }
        }

        // Одноточечный кроссовер над числовыми хромосомами
        private void NumberCrossing1()
        {
            // Проверка корректности данных
            if (Parent1.Length == Convert.ToInt32(txtNumNumber.Text) &&
                Parent2.Length == Convert.ToInt32(txtNumNumber.Text))
            {
                int X;
                try
                {
                    X = Convert.ToInt32(cboNumOp1_Point.SelectedItem);
                }
                catch
                {
                    MessageBox.Show("Неверное значение для выбранной точки скрещивания!");
                    return;
                }
                int[] Child1, Child2;
                NumbCross1(Parent1, Parent2, X, out Child1, out Child2);
                int[] XX = { X };
                txtNumOp1_Child1.Text = ShowNumbChild(Child1, XX);
                txtNumOp1_Child2.Text = ShowNumbChild(Child2, XX);
//                txtNumOp1_Child1.Text = NumsToText(Child1, CHAR_SPC);
//                txtNumOp1_Child2.Text = NumsToText(Child2, CHAR_SPC);
            }
        }

        // Двухточечный кроссовер над числовыми хромосомами
        private void NumberCrossing2()
        {
            // Проверка корректности данных
            if (Parent1.Length == Convert.ToInt32(txtNumNumber.Text) &&
                Parent2.Length == Convert.ToInt32(txtNumNumber.Text))
            {
                int X1, X2;
                try
                {
                    X1 = Convert.ToInt32(cboNumOp2_PointLeft.SelectedItem);
                    X2 = Convert.ToInt32(cboNumOp2_PointRight.SelectedItem);
                }
                catch
                {
                    MessageBox.Show("Неверное значение для выбранной точки скрещивания!");
                    return;
                }
                if (X1 < X2)
                {
                    int[] Child1, Child2;
                    NumbCross2(Parent1, Parent2, X1, X2, out Child1, out Child2);
                    int[] XX = { X1, X2 };
                    txtNumOp2_Child1.Text = ShowNumbChild(Child1, XX);
                    txtNumOp2_Child2.Text = ShowNumbChild(Child2, XX);
                }
                else
                {
                    txtNumOp2_Child1.Text = "";
                    txtNumOp2_Child2.Text = "";
                    MessageBox.Show("Неверно выбраны точки скрещивания!");
                }
            }
        }

        // Частично-соответствующий одноточечный кроссовер
        private void NumberCrossing3()
        {
            // Проверка корректности данных
            if (Parent1.Length == Convert.ToInt32(txtNumNumber.Text) &&
                Parent2.Length == Convert.ToInt32(txtNumNumber.Text))
            {
                int X;
                try
                {
                    X = Convert.ToInt32(cboNumOp3_Point.SelectedItem);
                }
                catch
                {
                    MessageBox.Show("Неверное значение для выбранной точки скрещивания!");
                    return;
                }
                int[] Child1, Child2;
                NumbCross3(Parent1, Parent2, X, out Child1, out Child2);
                int[] XX = { X };
                txtNumOp3_Child1.Text = ShowNumbChild(Child1, XX);
                txtNumOp3_Child2.Text = ShowNumbChild(Child2, XX);
            }
        }

        private void NumberCrossing4()
        {
            // Проверка корректности данных
            if (Parent1.Length == Convert.ToInt32(txtNumNumber.Text) &&
                Parent2.Length == Convert.ToInt32(txtNumNumber.Text))
            {
                int X1, X2;
                try
                {
                    X1 = Convert.ToInt32(cboNumOp3_PointLeft.SelectedItem);
                    X2 = Convert.ToInt32(cboNumOp3_PointRight.SelectedItem);
                }
                catch
                {
                    MessageBox.Show("Неверное значение для выбранной точки скрещивания!");
                    return;
                }
                if (X1 < X2)
                {
                    int[] Child1, Child2;
                    NumbCross4(Parent1, Parent2, X1, X2, out Child1, out Child2);
                    int[] XX = { X1, X2 };
                    txtNumOp4_Child1.Text = ShowNumbChild(Child1, XX);
                    txtNumOp4_Child2.Text = ShowNumbChild(Child2, XX);
                }
                else
                {
                    txtNumOp4_Child1.Text = "";
                    txtNumOp4_Child2.Text = "";
                    if (this.Done) MessageBox.Show("Неверно выбраны точки скрещивания!");
                }
            }
        }

        // Циклический кроссовер
        private void NumberCrossing5()
        {
            // Проверка корректности данных
            if (Parent1.Length == Convert.ToInt32(txtNumNumber.Text) &&
                Parent2.Length == Convert.ToInt32(txtNumNumber.Text))
            {
                int[] Child1, Child2;
                NumbCrossCycle(Parent1, Parent2, cboBeginPoint.SelectedIndex, out Child1, out Child2);
                txtNumOp5_Child1.Text = NumsToText(Child1, CHAR_SPC);
                txtNumOp5_Child2.Text = NumsToText(Child2, CHAR_SPC);
            }
        }

        // Выполняет операцию одноточечного кроссовера с числовыми хромосомами
        // P1, P2 - родители (числовые хромосомы, над которыми выполняется операция) - задаются массивами целых чисел (генами)
        // X - позиция точки скрещивания (отсчитывается от 1)
        // C1, C2 - полученные в результате скрещивания потомки (массивы генов потомков)
        //  Входные параметры должны быть корректными P1.Length=P2.Length и т.д.
        private void NumbCross1(int[] P1, int[] P2, int X, out int[] C1, out int[] C2)
        {
            int i, j, X1, X2;
            int[] Child1 = new int[P1.Length];
            int[] Child2 = new int[P2.Length];
            // определяем хромосомы потомков Child1 и Child2
            for (i = 0; i < X - 1; i++)
            {
                Child1[i] = P1[i]; Child2[i] = P2[i];
            }
            X1 = X - 1; X2 = X - 1;
            for (i = 0; i < P2.Length; i++)
            {
                for (j = 0; j < X - 1; j++) if (P2[i] == P1[j]) break;
                if (j == X - 1) Child1[X1++] = P2[i];
            }
            for (i = 0; i < P1.Length; i++)
            {
                for (j = 0; j < X - 1; j++) if (P1[i] == P2[j]) break;
                if (j == X - 1) Child2[X2++] = P1[i];
            }
            C1 = Child1;
            C2 = Child2;
        }

        // Выполняет операцию двухточечного кроссовера с числовыми хромосомами
        // P1, P2 - родители (числовые хромосомы, над которыми выполняется операция) - задаются массивами целых чисел (генами)
        // X1, X2 - позиции точек скрещивания (отсчитываются от 1). Скрещивание производится в интервале между X1 и X2.
        // C1, C2 - полученные в результате скрещивания потомки (массивы генов потомков)
        //  Входные параметры должны быть корректными P1.Length=P2.Length и т.д.
        private void NumbCross2(int[] P1, int[] P2, int X1, int X2, out int[] C1, out int[] C2)
        {
            int i, j, i1, i2, X;
            int[] Child1 = new int[P1.Length];
            int[] Child2 = new int[P2.Length];
            // определяем хромосомы потомков Child1 и Child2
            i1 = X1 - 1; i2 = X2 - 1; 
            for (i = 0; i < i1; i++)
            {
                Child1[i] = P1[i]; Child2[i] = P2[i];
            }
            for (i = i2; i < P1.Length; i++)
            {
                Child1[i] = P1[i]; Child2[i] = P2[i];
            }
            X = X1 - 1;
            for (i = 0; i < P2.Length; i++)
            {
                    for (j = 0; j < P1.Length; j++)
                    {
                        if (j < i1 || j >= i2)
                        {
                            if (P2[i] == P1[j]) break;
                        }
                    }
                    if (j == P1.Length) { Child1[X] = P2[i]; X++; }
            }
            X = X1 - 1;
            for (i = 0; i < P1.Length; i++)
            {
                for (j = 0; j < P2.Length; j++)
                {
                    if (j < i1 || j >= i2)
                    {
                        if (P1[i] == P2[j]) break;
                    }
                }
                if (j == P2.Length) { Child2[X] = P1[i]; X++; }
            }
            C1 = Child1;
            C2 = Child2;
        }

        // Выполняет операцию частично-соответствующего одноточечного кроссовера с числовыми хромосомами
        // P1, P2 - родители (числовые хромосомы, над которыми выполняется операция) - задаются массивами целых чисел (генами)
        // X - позиция точки скрещивания (отсчитывается от 1)
        // C1, C2 - полученные в результате скрещивания потомки (массивы генов потомков)
        //  Входные параметры должны быть корректными P1.Length=P2.Length и т.д.
        private void NumbCross3(int[] P1, int[] P2, int X, out int[] C1, out int[] C2)
        {
            int i, Gen;
            int[] Child1 = new int[P1.Length];
            int[] Child2 = new int[P2.Length];
            // определяем хромосомы потомков Child1 и Child2
            for (i = 0; i < P1.Length; i++)
            {
                Child1[i] = P1[i]; Child2[i] = P2[i];
            }
            for (i = X - 1; i < P1.Length; i++)
            {
                Gen = Child1[P2[i]]; Child1[P2[i]] = Child1[i]; Child1[i] = Gen;
            }
            for (i = X - 1; i < P2.Length; i++)
            {
                Gen = Child2[P1[i]]; Child2[P1[i]] = Child2[i]; Child2[i] = Gen;
            }
            C1 = Child1;
            C2 = Child2;
        }

        // Выполняет операцию частично-соответствующего двухточечного кроссовера с числовыми хромосомами
        // P1, P2 - родители (числовые хромосомы, над которыми выполняется операция) - задаются массивами целых чисел (генами)
        // X1, X2 - позиции точек скрещивания (отсчитываются от 1). Скрещивание производится в интервале между X1 и X2.
        // C1, C2 - полученные в результате скрещивания потомки (массивы генов потомков)
        //  Входные параметры должны быть корректными P1.Length=P2.Length и т.д.
        private void NumbCross4(int[] P1, int[] P2, int X1, int X2, out int[] C1, out int[] C2)
        {
            int i, i1, i2, Gen;
            int[] Child1 = new int[P1.Length];
            int[] Child2 = new int[P2.Length];
            // определяем хромосомы потомков Child1 и Child2
            for (i = 0; i < P1.Length; i++)
            {
                Child1[i] = P1[i]; Child2[i] = P2[i];
            }
            i1 = X1 - 1; i2 = X2 - 1;
            for (i = i1; i < i2; i++)
            {
                Gen = Child1[P2[i]]; Child1[P2[i]] = Child1[i]; Child1[i] = Gen;
            }
            for (i = i1; i < i2; i++)
            {
                Gen = Child2[P1[i]]; Child2[P1[i]] = Child2[i]; Child2[i] = Gen;
            }
            C1 = Child1;
            C2 = Child2;
        }

        // Циклический оператор скрещивания
        // P1, P2 - родители (числовые хромосомы, над которыми выполняется операция) - задаются массивами целых чисел (генами)
        // S - стартовый ген в 1-ом родителе (от 0 до ...)
        // C1, C2 - полученные в результате скрещивания потомки (массивы генов потомков)
        private void NumbCrossCycle(int[] P1, int[] P2, int S, out int[] C1, out int[] C2)
        {
            int i, B, C = S;
            int[] Child1 = new int[P1.Length];
            int[] Child2 = new int[P2.Length];
            bool[] Used = new bool[P1.Length];  // Признаки просмотренных генов 1-го родителя

            // определяем хромосому потомка Child1
            for (i = 0; i < P2.Length; i++) Used[i] = false;
            bool A = true;  // Признак копирования генов хромосомы первого родителя, если false - второго родителя
            while (C < P1.Length)
            {
                if (Used[C])
                {
                    C++;
                }
                else
                {
                    if (A)
                    {
                        B = P1[C]; Child1[C] = B; Used[C] = true;
                        do
                        {
                            for (i = 0; i < P1.Length; i++)
                                if (P2[C] == P1[i]) break;
                            C = i; Child1[C] = P1[C]; Used[C] = true;
                        }
                        while (P2[C] != B);
                        A = false; C = 0;
                    }
                    else
                    {
                        B = P1[C]; Child1[C] = P2[C]; Used[C] = true;
                        do
                        {
                            for (i = 0; i < P1.Length; i++)
                                if (P2[C] == P1[i]) break;
                            C = i; Child1[C] = P2[C]; Used[C] = true;
                        }
                        while (P2[C] != B);
                        C = 0;
                    }
                }
            }
            C1 = Child1;

            // определяем хромосому потомка Child2
            for (i = 0; i < P1.Length; i++) Used[i] = false;
            A = true; C = S; 
            while (C < P2.Length)
            {
                if (Used[C])
                {
                    C++;
                }
                else
                {
                    if (A)
                    {
                        B = P2[C]; Child2[C] = B; Used[C] = true;
                        do
                        {
                            for (i = 0; i < P2.Length; i++)
                                if (P1[C] == P2[i]) break;
                            C = i; Child2[C] = P2[C]; Used[C] = true;
                        }
                        while (P1[C] != B);
                        A = false; C = 0;
                    }
                    else
                    {
                        B = P2[C]; Child2[C] = P1[C]; Used[C] = true;
                        do
                        {
                            for (i = 0; i < P2.Length; i++)
                                if (P1[C] == P2[i]) break;
                            C = i; Child2[C] = P1[C]; Used[C] = true;
                        }
                        while (P1[C] != B);
                        C = 0;
                    }
                }
            }
            C2 = Child2;
        }

        private void txtNumParent1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txtNumParent2.Focus();
        }

        private void txtNumParent2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) cboNumOperators.Focus();
        }

        private void cboNumOp2_PointLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done && cboNumOp2_PointLeft.SelectedIndex >= 0) NumberCrossing2();
        }

        private void cboNumOp2_PointRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Done && cboNumOp2_PointRight.SelectedIndex >= 0) NumberCrossing2();
        }

        private void lstGoldVariants_SelectedIndexChanged(object sender, EventArgs e)
        {
            int G = (int)((GoldProp / (GoldProp + 1)) * (this.Len - 1));
            int G1 = (int)((GoldProp / (2 * (GoldProp + 1))) * (this.Len - 1));
            int G2 = (int)(0.5 * (this.Len - 1));
            switch (lstGoldVariants.SelectedIndex)
            {
                case 0:
                    for (int i = 0; i < lstGoldPoints.Items.Count; i++)
                    {
                        if (i == G)
                            lstGoldPoints.SetSelected(i, true);
                        else
                            lstGoldPoints.SetSelected(i, false);
                    }
                    break;
                case 1:
                    for (int i = 0; i < lstGoldPoints.Items.Count; i++)
                    {
                        if (i == G1 || i == G2)
                            lstGoldPoints.SetSelected(i, true);
                        else
                            lstGoldPoints.SetSelected(i, false);
                    }
                    break;
            }
            CrossGolden();
        }

        private void cboNumOp3_Point_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboNumOp3_Point.SelectedIndex >= 0) NumberCrossing3(); 
        }

        private void cboNumOp3_PointLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboNumOp3_PointLeft.SelectedIndex >= 0) NumberCrossing4(); 
        }

        private void cboNumOp3_PointRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboNumOp3_PointRight.SelectedIndex >= 0) NumberCrossing4(); 
        }

        private void txtNumParent1_TextChanged(object sender, EventArgs e)
        {
            ClearNumbResult();
        }

        private void txtNumParent2_TextChanged(object sender, EventArgs e)
        {
            ClearNumbResult();
        }

        // Очищает поля результатов при изменении исходных данных для числовых хромосом
        private void ClearNumbResult()
        {
            txtNumOp1_Child1.Text = ""; txtNumOp1_Child2.Text = "";
            txtNumOp2_Child1.Text = ""; txtNumOp2_Child2.Text = "";
            txtNumOp3_Child1.Text = ""; txtNumOp3_Child2.Text = "";
            txtNumOp4_Child1.Text = ""; txtNumOp4_Child2.Text = "";
        }

        private void cboBeginPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBeginPoint.SelectedIndex >= 0) NumberCrossing5();
        }

        private void txtParent1_Validating(object sender, CancelEventArgs e)
        {
            if (CheckChromosom(txtParent1.Text))
            {
                Crossing();
                ErrPro.SetError(txtParent1, "");    // Сброс отображения ошибки
            }
            else
            {
                ErrorParents(); e.Cancel = true;
                ErrPro.SetError(txtParent1, "Недопустимая хромосома 1-го родителя!");
            }
        }

        private void txtParent2_Validating(object sender, CancelEventArgs e)
        {
            if (CheckChromosom(txtParent2.Text))
            {
                Crossing();
                ErrPro.SetError(txtParent2, "");    // Сброс отображения ошибки
            }
            else
            {
                ErrorParents(); e.Cancel = true;
                ErrPro.SetError(txtParent2, "Недопустимая хромосома 1-го родителя!");
            }
        }

        private void txtNumParent1_Validating(object sender, CancelEventArgs e)
        {
            if ((Parent1 = GetNumberGenom(txtNumParent1.Text)) == null)
            {
                ErrorParents(); e.Cancel = true;
                ErrPro.SetError(txtNumParent1, "Недопустимая хромосома 1-го родителя!");
            }
            else
            {
                NumberCrossing();
                ErrPro.SetError(txtNumParent1, "");    // Сброс отображения ошибки
            }
        }

        private void txtNumParent2_Validating(object sender, CancelEventArgs e)
        {
            if ((Parent2 = GetNumberGenom(txtNumParent2.Text)) == null)
            {
                ErrorParents(); e.Cancel = true;
                ErrPro.SetError(txtNumParent2, "Недопустимая хромосома 2-го родителя!");
            }
            else
            {
                NumberCrossing();
                ErrPro.SetError(txtNumParent2, "");    // Сброс отображения ошибки
            }
        }

        private void txtMask_Validating(object sender, CancelEventArgs e)
        {
            if (CheckChromosom(txtMask.Text))
            {
                Crossing();
                ErrPro.SetError(txtMask, "");    // Сброс отображения ошибки
            }
            else
            {
                ErrorMask(); e.Cancel = true;
                ErrPro.SetError(txtMask, "Недопустимая маска хромосомы!");
            }
        }

        private void txtChromosom_Validating(object sender, CancelEventArgs e)
        {
            if (CheckNumberGens(txtChromosom.Text))
            {
                InitBinData(this.Len = Convert.ToInt32(txtChromosom.Text));
                Crossing();
                ErrPro.SetError(txtChromosom, "");    // Сброс отображения ошибки
            }
            else
            {
                MessageBox.Show("Неверно указано число генов в хромосомах родителей!"); e.Cancel = true;
                ErrPro.SetError(txtChromosom, "Недопустимое число генов в хромосомах!");
            }
        }

        private void txtChromosom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txtParent1.Focus();
        }

        private void lstStartPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstStartPoint.SelectedItems.Count == 2)
                Greeding();
            else
            {
                txtNumOp6_Child1.Text = ""; txtNumOp6_Child2.Text = "";
                txtGoal1.Text = ""; txtGoal2.Text = "";
            }
        }

        // ОБновление потомков с применением "жадного" оператора
        private void Greeding()
        {
            int[,] Matrix = MatrixToArray(datMatrix);
            if (Matrix == null) return;
            txtNumOp6_Child1.Text = Greedy(Matrix, txtNumParent1.Text, txtNumParent2.Text, lstStartPoint.SelectedIndices[0]);
            txtNumOp6_Child2.Text = Greedy(Matrix, txtNumParent1.Text, txtNumParent2.Text, lstStartPoint.SelectedIndices[1]);
            txtGoal1.Text = GoalFunc(Matrix, txtNumOp6_Child1.Text).ToString();
            txtGoal2.Text = GoalFunc(Matrix, txtNumOp6_Child2.Text).ToString();
        }

        // "Жадный" оператор кроссовера
        // Matrix - матрица смежности в задаче коммивояжера
        // P1, P2 - родители
        // Start - стартовая точка в матрице смежности (номер строки в матрице смежности), отсчет от 0
        // Возвращает пустую строку, если в исходных данных имеется ошибка
        private string Greedy(int[,] Matrix, string P1, string P2, int Start)
        {
            int i, j, k,
                G1, G2, // значения целевых функций выбранного пути обхода вершин
                N = 0;  // счетчик отобранных для потомка вершин
            if ((G1 = GoalFunc(Matrix, P1)) == -1) return "";
            if ((G2 = GoalFunc(Matrix, P2)) == -1) return "";
            int[] Parent1 = TextToNums(P1);
            int[] Parent2 = TextToNums(P2);
            int[] Gens = new int[Parent1.Length];

            Gens[N] = Start;
            while (++N < Gens.Length)
            {
                // Ищем порядковый номер гена в 1 родителе для стартовой точки
                for (i = 0; i < Parent1.Length; i++) if (Parent1[i] == Gens[N - 1]) break;
                do
                {
                    // Выбираем следующий ген 1 родителя, если текущий завершает цикл
                    i++; if (i == Parent1.Length) i = 0;
                    // Это должен быть ген, который ранее не использовался в 1 потомке
                    for (k = 0; k < N; k++) if (Parent1[i] == Gens[k]) break;
                } while (k < N);
                // Ищем порядковый номер гена во 2 родителе для стартовой точки
                for (j = 0; j < Parent2.Length; j++) if (Parent2[j] == Gens[N - 1]) break;
                do
                {
                    // Аналогично выбираем следующий ген 2 родителя, если текущий завершает цикл
                    j++; if (j == Parent2.Length) j = 0;
                    // Это должен быть ген, который ранее не использовался во 2 потомке
                    for (k = 0; k < N; k++) if (Parent2[j] == Gens[k]) break;
                } while (k < N);
                // Выбираем следующий ген для родителя с меньшей длиной пути
                if (Matrix[Gens[N - 1], Parent1[i]] < Matrix[Gens[N - 1], Parent2[j]])
                    Gens[N] = Parent1[i];
                else
                    Gens[N] = Parent2[j];
            }


/*
            Gens[N] = Start;
            // Ищем в строке Start матрицы элемент с минимальным значением (кроме диагонального)
            while (++N < Gens.Length)
            {
                Value = 0;
                for (i = 0; i < Matrix.GetLength(0); i++)
                {
                    if (i != Start)
                    {
                        // Отбираем вершину, которая ранее не была использована в потомке
                        for (j = 0; j < N; j++) if (i == Gens[j]) break;
                        if (j == N)
                        {
                            // Для отобранной вершины определям ближайшую следующую вершину в пути
                            if (Value == 0)
                            {
                                Gens[N] = i; Value = Matrix[Start, i];
                            }
                            else if (Matrix[Start, i] < Value)
                            {
                                Gens[N] = i; Value = Matrix[Start, i];
                            }
                        }
                    }
                }
                Start = Gens[N];
            }
 * */
            return NumsToText(Gens, CHAR_SPC);
        }

        // Преобразует данные элемента управления DataGridView в целочисленную матрицу, возвращая ее
        // Возвращает null, если в матрице имеется элемент, не являющийся целым неотрицательным числом
        private int[,] MatrixToArray(DataGridView Matrix)
        {
            int[,] Arr = new int[Matrix.Rows.Count, Matrix.Rows.Count];
            for (int r = 0; r < Matrix.Rows.Count; r++)
                for (int c = 0; c < Matrix.Rows.Count; c++)
                {
                    if (IsInteger(Matrix.Rows[r].Cells[c].Value.ToString()))
                    {
                        // элемент матрицы является целым положительным числом, включая 0
                        Arr[r, c] = Convert.ToInt32(Matrix.Rows[r].Cells[c].Value);
                        // диагональный элемент должен быть равен 0
                        if (r == c && Arr[r, c] != 0) return null;
                    }
                    else
                        return null;
                }
            return Arr;
        }

        // Вычисляет целевую функцию в задаче коммивояжера
        // Matrix - матрица смежности
        // Path - путь обхода вершин графа, заданных матрицей смежности
        // Возвращает -1, если строка с генами родителей имеет недопустимый формат или значение
        private int GoalFunc(int[,] Matrix, string Path)
        {
            int i, L = 0;
            int[] Vertex = TextToNums(Path);
            if (Vertex == null) return -1;
            for (i = 0; i < Vertex.Length; i++)
            {
                if (i == 0)
                    L += Matrix[Vertex[Vertex.Length - 1], Vertex[i]];
                else
                    L += Matrix[Vertex[i - 1], Vertex[i]];
            }
            return L;
        }

        // Проверяет: является ли величина Value правильной записью целого положительного числа
        private bool IsInteger(string Value)
        {
            string Digits = "0123456789";
            for (int c = 0; c < Value.Length; c++)
            {
                if (Digits.Contains(Value[c])) continue;
                return false;
            }
            return Value.Length == 0 ? false : true;
        }
    }
}
