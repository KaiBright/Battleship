using System; 
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
namespace Battleship 
{
    public partial class FiledForm : Form
    {
        private Panel activePanel;
        private int count1 = 0;
        private int count2 = 0;
        private int count3 = 0;
        private int count4 = 0;
        private Cell[,] field;

        public String  UserName
        {
            get;set;
        }

        public bool IsClosing
        {
            get;set;
        }

        PlayForm owner;

        // Из StartForm берем имя и записываем его заголовок окна с приветствием
        public FiledForm(PlayForm owner)
        {
            UserName = "";
            IsClosing = false;
            this.owner = owner;
            StartForm startForm = new StartForm();
            startForm.Owner = this;
            startForm.ShowDialog();
            if (IsClosing)
            {
                owner.IsClosing = true;
                this.Close();
                return;
            }
            InitializeComponent();
            Text = "Добро пожаловать, " + UserName+"!";
        }

        private void initForm()
        {
            l11.Text = "0";
            l21.Text = "4";
            l12.Text = "0";
            l22.Text = "3";
            l13.Text = "0";
            l23.Text = "2";
            l14.Text = "0";
            l24.Text = "1";
            count1 = 0;
            count2 = 0;
            count3 = 0;
            count4 = 0;
            var controls = panel1.Controls;

            // Создаем поле для расстановки
            field = new Cell[10, 10];
            foreach (Control control in controls)
            {
                if (control is Button)
                {
                    ((Button)control).BackColor = SystemColors.Control;
                    int tag = int.Parse(control.Tag.ToString());
                    field[(tag - 100) / 10, (tag - 100) % 10] = new Cell();
                    field[(tag - 100) / 10, (tag - 100) % 10].Btn = (Button)control;
                    field[(tag - 100) / 10, (tag - 100) % 10].Type = CellType.Open;
                    field[(tag - 100) / 10, (tag - 100) % 10].Column = (tag - 100) % 10;
                    field[(tag - 100) / 10, (tag - 100) % 10].Row = (tag - 100) / 10;
                }
            }
        }

        private void panelClicked(object sender, EventArgs e)
        {
            if (sender is Panel)
                setActivePanel((Panel)sender);
        }

        private void FiledForm_Load(object sender, EventArgs e)
        {
            initForm();
            var controls = panel1.Controls;
            foreach(Control control in controls)
            {
                if (control is Button)
                {
                    ((Button)control).Click += cellClick;
                }
            }
        }

        private void cellClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int tag = int.Parse(btn.Tag.ToString());
            int row = (tag - 100) / 10;
            int column = (tag - 100) % 10;
            var ship=field[(tag - 100) / 10, (tag - 100) % 10].Ship; // Переменная, которая содержит размеры корабля
            if (ship.Count>0)
            {
                if (ship.Count == 4) // четырехпалубники
                {
                    count4--;
                    l14.Text = "" + count4;
                    l24.Text = "" + (1 - count4);
                }
                if (ship.Count == 3) // трехпалубники
                {
                    count3--;
                    l13.Text = "" + count3;
                    l23.Text = "" + (2 - count3);
                }
                if (ship.Count == 2) // двухпалубники
                {
                    count2--;
                    l12.Text = "" + count2;
                    l22.Text = "" + (3 - count2);
                }
                if (ship.Count == 1) // однопалубники
                {
                    count1--;
                    l11.Text = "" + count1;
                    l21.Text = "" + (4 - count1);
                }

                foreach (Cell cell in ship)
                {
                    cell.Type = CellType.Open;
                    cell.Btn.BackColor = SystemColors.Control;
                }
                ship.Clear();
            }
            else
            {// проверяем возможность установки корабля
                if (activePanel!=null)
                {
                    int shipLen = int.Parse(activePanel.Tag.ToString()) / 10;
                    bool vertical=int.Parse(activePanel.Tag.ToString()) %2==0;
                    if (checkShip(row, column, vertical, shipLen))
                    {
                        setShip(row, column, vertical, shipLen);
                        setActivePanel(activePanel);
                    }
                    else
                    {
                        SystemSounds.Asterisk.Play();
                    }
                }
            }
        }

        private void setShip(int row, int column, bool vertical, int shipLen)
        {
            if (vertical) // Если пользователь выбрал корабль вертикальный
            {
                switch (shipLen)
                {
                    case 1:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]); // Задаем ячейке тип корабля, закрашиваем зеленым цветом
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            count1++; // Увеличиваем переменную, которая отвечает за количество расставленных кораблей
                            l11.Text = "" + count1;
                            l21.Text = "" + (4 - count1);
                            break;
                        }
                    case 2:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]);
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row+1, column]); // Из-за того, что корабль двухпалубный и вертикальный добавляем строчку
                            field[row+1, column].Ship = newShip;
                            field[row+1, column].Type = CellType.Ship;
                            field[row+1, column].Btn.BackColor = Color.Green;
                            count2++;
                            l12.Text = "" + count2;
                            l22.Text = "" + (3 - count2);
                            break;
                        }
                    case 3:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]);
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row + 1, column]);
                            field[row + 1, column].Ship = newShip;
                            field[row + 1, column].Type = CellType.Ship;
                            field[row + 1, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row + 2, column]);
                            field[row + 2, column].Ship = newShip;
                            field[row + 2, column].Type = CellType.Ship;
                            field[row + 2, column].Btn.BackColor = Color.Green;
                            count3++;
                            l13.Text = "" + count3;
                            l23.Text = "" + (2 - count3);
                            break;
                        }
                    case 4:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]);
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row + 1, column]);
                            field[row + 1, column].Ship = newShip;
                            field[row + 1, column].Type = CellType.Ship;
                            field[row + 1, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row + 2, column]);
                            field[row + 2, column].Ship = newShip;
                            field[row + 2, column].Type = CellType.Ship;
                            field[row + 2, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row + 3, column]);
                            field[row + 3, column].Ship = newShip;
                            field[row + 3, column].Type = CellType.Ship;
                            field[row + 3, column].Btn.BackColor = Color.Green;
                            count4++;
                            l14.Text = "" + count4;
                            l24.Text = "" + (1 - count4);
                            break;
                        }
                }
            }
            else // Если пользователь выбрал корабль горизонтальный
            {
                switch (shipLen)
                {
                    case 1:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]);
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            count1++;
                            l11.Text = "" + count1;
                            l21.Text = "" + (4 - count1);
                            break;
                        }
                    case 2:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]);
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row, column+1]); // Из-за того, что корабль двухпалубный и горизонтальный добавляем столбец
                            field[row , column+1].Ship = newShip;
                            field[row , column+1].Type = CellType.Ship;
                            field[row , column+1].Btn.BackColor = Color.Green;
                            count2++;
                            l12.Text = "" + count2;
                            l22.Text = "" + (3 - count2);
                            break;
                        }
                    case 3:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]);
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row, column + 1]);
                            field[row, column + 1].Ship = newShip;
                            field[row, column + 1].Type = CellType.Ship;
                            field[row, column + 1].Btn.BackColor = Color.Green;

                            newShip.Add(field[row, column + 2]);
                            field[row, column + 2].Ship = newShip;
                            field[row, column + 2].Type = CellType.Ship;
                            field[row, column + 2].Btn.BackColor = Color.Green;
                            count3++;
                            l13.Text = "" + count3;
                            l23.Text = "" + (2 - count3);
                            break;
                        }
                    case 4:
                        {
                            List<Cell> newShip = new List<Cell>();
                            newShip.Add(field[row, column]);
                            field[row, column].Ship = newShip;
                            field[row, column].Type = CellType.Ship;
                            field[row, column].Btn.BackColor = Color.Green;
                            newShip.Add(field[row, column + 1]);
                            field[row, column + 1].Ship = newShip;
                            field[row, column + 1].Type = CellType.Ship;
                            field[row, column + 1].Btn.BackColor = Color.Green;

                            newShip.Add(field[row, column + 2]);
                            field[row, column + 2].Ship = newShip;
                            field[row, column + 2].Type = CellType.Ship;
                            field[row, column + 2].Btn.BackColor = Color.Green;
                            newShip.Add(field[row, column + 3]);
                            field[row, column + 3].Ship = newShip;
                            field[row, column + 3].Type = CellType.Ship;
                            field[row, column + 3].Btn.BackColor = Color.Green;
                            count4++;
                            l14.Text = "" + count4;
                            l24.Text = "" + (1 - count4);
                            break;
                        }
                }
            }
        }

        private bool checkShip(int row, int column, bool vertical, int shipLen)
        {
            bool result=true;
            if (vertical)
            {
                //проверить что корабль вмещается
                if (row + shipLen > 10)
                    return false;
                
                //проверим, что на этом пути нет кораблей
                for (int i=0;i<shipLen;i++)
                {
                    if (field[row+i, column].Type != CellType.Open)
                        return false;
                }

                //проверим,что рядом нет кораблей
                int rowCheck = row - 1;
                for (int i=column-1;i<=column+1;i++)//сверху
                {
                    if (rowCheck>=00&&rowCheck<10&&i>=0&&i<10)
                    {
                        if (field[rowCheck, i].Type != CellType.Open)
                            return false;
                    }
                }

                rowCheck = row +shipLen;
                for (int i = column - 1; i <= column + 1; i++)//снизу
                {
                    if (rowCheck >= 00 && rowCheck < 10 && i >= 0 && i < 10)
                    {
                        if (field[rowCheck, i].Type != CellType.Open)
                            return false;
                    }
                }

                int columnCheck = column - 1;
                for (int i = row - 1; i <= row + shipLen; i++)//слева
                {
                    if (columnCheck >= 00 && columnCheck < 10 && i >= 0 && i < 10)
                    {
                        if (field[i, columnCheck].Type != CellType.Open)
                            return false;
                    }
                }

                columnCheck =column+1;
                for (int i = row - 1; i <= row  + shipLen; i++)//справа
                {
                    if (columnCheck >= 00 && columnCheck < 10 && i >= 0 && i < 10)
                    {
                        if (field[i, columnCheck].Type != CellType.Open)
                            return false;
                    }
                }
            }
            else
            {
                //проверить что корабль вмещается
                if (column + shipLen > 10)
                    return false;

                //проверим, что на этом пути нет кораблей
                for (int i = 0; i < shipLen; i++)
                {
                    if (field[row, column+i].Type != CellType.Open)
                        return false;
                }

                //проверим,что рядом нет кораблей
                int rowCheck = row - 1;
                for (int i = column - 1; i <= column + shipLen; i++)//сверху
                {
                    if (rowCheck >= 00 && rowCheck < 10 && i >= 0 && i < 10)
                    {
                        if (field[rowCheck, i].Type != CellType.Open)
                            return false;
                    }
                }

                rowCheck = row + 1;
                for (int i = column - 1; i <= column + shipLen; i++)//снизу
                {
                    if (rowCheck >= 00 && rowCheck < 10 && i >= 0 && i < 10)
                    {
                        if (field[rowCheck, i].Type != CellType.Open)
                            return false;
                    }
                }

                int columnCheck = column - 1;
                for (int i = row - 1; i <= row + 1; i++)//слева
                {
                    if (columnCheck >= 00 && columnCheck < 10 && i >= 0 && i < 10)
                    {
                        if (field[i, columnCheck].Type != CellType.Open)
                            return false;
                    }
                }

                columnCheck = column + shipLen;
                for (int i = row - 1; i <= row + 1; i++)//справа
                {
                    if (columnCheck >= 00 && columnCheck < 10 && i >= 0 && i < 10)
                    {
                        if (field[i, columnCheck].Type != CellType.Open)
                            return false;
                    }
                }
            }
            return result;
        }
        private void FiledForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsClosing)
            {
                if (MessageBox.Show("Выйти из программы?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    owner.IsClosing = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        // Когда выбираем корабль
        private void setActivePanel(Panel panel)
        {
            if (activePanel!=panel)
            {
                if (activePanel!=null) // Если еще не выбрали корабль для расстановки, то он зеленый
                {
                    var actButtons = activePanel.Controls;
                    foreach (Control control in actButtons)
                    {
                        if (control is Button)
                        {
                            ((Button)control).BackColor = Color.Green;
                        }
                    }
                   
                }
                int shipLen = int.Parse(panel.Tag.ToString()) / 10;
                if ((shipLen == 4 && count4 < 1)||
                    (shipLen == 3 && count3 < 2) ||
                    (shipLen == 2 && count2 < 3) ||
                    (shipLen == 1 && count1 < 4) ) // Если есть не расставленные корабли, то их можно выбрать
                {
                    var buttons = panel.Controls;
                    foreach (Control control in buttons)
                    {
                        if (control is Button)
                        {
                            ((Button)control).BackColor = Color.Red; // Если выбрали корабль, то он становится красным
                        }
                    }
                }
                else // Если корабль уже невозможно выбрать, а пользователь нажимает на эту кнопку, то будет звук ошибки
                {                
                    SystemSounds.Asterisk.Play();
                    activePanel = null;
                }
               
                activePanel = panel;
            }
            else // пользователь не взаимодействует с кнопками кораблей, они по умолчанию зеленые
            {
                var buttons = panel.Controls;
                foreach (Control control in buttons)
                {
                    if (control is Button)
                    {
                        ((Button)control).BackColor = Color.Green;
                    }
                }
                activePanel = null;
                
            }
            //Закрашиваем расставленые корабли как неактивные
            if ((activePanel == null) && (count4 == 1))
            {
                var controls2 = panel2.Controls;
                var controls3 = panel3.Controls;
                foreach (Button control in controls2)
                {
                   ((Button)control).BackColor = Color.Gray;
                }
                foreach (Button control in controls3)
                {
                    ((Button)control).BackColor = Color.Gray;
                }
            }

            if ((activePanel == null) && (count3 == 2))
            {
                var controls4 = panel4.Controls;
                var controls5 = panel5.Controls;
                foreach (Button control in controls4)
                {
                    ((Button)control).BackColor = Color.Gray;
                }
                foreach (Button control in controls5)
                {
                    ((Button)control).BackColor = Color.Gray;
                }
            }
            if ((activePanel == null) && (count2 == 3))
            {
                var controls6 = panel6.Controls;
                var controls7 = panel7.Controls;
                foreach (Button control in controls6)
                {
                    ((Button)control).BackColor = Color.Gray;
                }
                foreach (Button control in controls7)
                {
                    ((Button)control).BackColor = Color.Gray;
                }
            }
            if ((activePanel == null) && (count1 == 4))
            {
                var controls8 = panel8.Controls;        
                foreach (Button control in controls8)
                {
                    ((Button)control).BackColor = Color.Gray;
                }
                
            }

        }
        // Кнопка расстановки вручную 
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Enabled)
            {
                initForm();
                panel1.Enabled = true;
                var controls2 = panel2.Controls;
                foreach (Button control in controls2)
                {
                    ((Button)control).BackColor = Color.Green;
                }
                var controls3 = panel3.Controls;
                foreach (Button control in controls3)
                {
                    ((Button)control).BackColor = Color.Green;
                }
                var controls4 = panel4.Controls;
                foreach (Button control in controls4)
                {
                    ((Button)control).BackColor = Color.Green;
                }
                var controls5 = panel5.Controls;
                foreach (Button control in controls5)
                {
                    ((Button)control).BackColor = Color.Green;
                }
                var controls6 = panel6.Controls;
                foreach (Button control in controls6)
                {
                    ((Button)control).BackColor = Color.Green;
                }
                var controls7 = panel7.Controls;
                foreach (Button control in controls7)
                {
                    ((Button)control).BackColor = Color.Green;
                }
                var controls8 = panel8.Controls;
                foreach (Button control in controls8)
                {
                    ((Button)control).BackColor = Color.Green;
                }
            }
        }
        // Кнопка автоматической расстановки
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioButton2.Enabled)
                {
                    initForm();
                    panel1.Enabled = false;
                    List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            Tuple<int, int> tuple = new Tuple<int, int>(i, j);
                            tuples.Add(tuple);
                        }
                    }

                    Random random = new Random((int)DateTime.Now.Ticks);
                    int c1 = 4;
                    int c2 = 3;
                    int c3 = 2;
                    int c4 = 1;
                    while (c1 > 0)
                    {
                        int index = random.Next() % tuples.Count;
                        Tuple<int, int> tuple = tuples[index];
                        tuples.RemoveAt(index);
                        if (checkShip(tuple.Item1, tuple.Item2, index % 2 == 0, 1))
                        {
                            setShip(tuple.Item1, tuple.Item2, index % 2 == 0, 1);
                            c1--;
                        }
                    }
                    while (c2 > 0)
                    {
                        int index = random.Next() % tuples.Count;
                        Tuple<int, int> tuple = tuples[index];
                        tuples.RemoveAt(index);
                        if (checkShip(tuple.Item1, tuple.Item2, index % 2 == 0, 2))
                        {
                            setShip(tuple.Item1, tuple.Item2, index % 2 == 0, 2);
                            c2--;
                        }
                    }
                    while (c3 > 0)
                    {
                        int index = random.Next() % tuples.Count;
                        Tuple<int, int> tuple = tuples[index];
                        tuples.RemoveAt(index);
                        if (checkShip(tuple.Item1, tuple.Item2, index % 2 == 0, 3))
                        {
                            setShip(tuple.Item1, tuple.Item2, index % 2 == 0, 3);
                            c3--;
                        }
                    }
                    while (c4 > 0)
                    {
                        int index = random.Next() % tuples.Count;
                        Tuple<int, int> tuple = tuples[index];
                        tuples.RemoveAt(index);
                        if (checkShip(tuple.Item1, tuple.Item2, index % 2 == 0, 4))
                        {
                            setShip(tuple.Item1, tuple.Item2, index % 2 == 0, 4);
                            c4--;
                        }
                    }
                }
            }
            catch
            {
                radioButton2_CheckedChanged(null, null);
            }
            //Закрашиваем расставленые корабли как неактивные
            var controls2 = panel2.Controls;
            foreach (Button control in controls2)
            {
                ((Button)control).BackColor = Color.Gray;
            }
            var controls3 = panel3.Controls;
            foreach (Button control in controls3)
            {
                ((Button)control).BackColor = Color.Gray;
            }
            var controls4 = panel4.Controls;
            foreach (Button control in controls4)
            {
                ((Button)control).BackColor = Color.Gray;
            }
            var controls5 = panel5.Controls;
            foreach (Button control in controls5)
            {
                ((Button)control).BackColor = Color.Gray;
            }
            var controls6 = panel6.Controls;
            foreach (Button control in controls6)
            {
                ((Button)control).BackColor = Color.Gray;
            }
            var controls7 = panel7.Controls;
            foreach (Button control in controls7)
            {
                ((Button)control).BackColor = Color.Gray;
            }
            var controls8 = panel8.Controls;
            foreach (Button control in controls8)
            {
                ((Button)control).BackColor = Color.Gray;
            }
        }

        private void button114_Click(object sender, EventArgs e) // Кнопка старта
        {
            if (count1==4 && count2==3 && count3==2 && count4==1) // Если все корабли расставлены
            {
                IsClosing = true;
                Cell[,] my = new Cell[10, 10];
                for (int i=0;i<10;i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        my[i, j] = new Cell();
                        my[i, j].Column = field[i, j].Column;
                        my[i, j].Row = field[i, j].Row;
                        my[i, j].Ship = field[i, j].Ship;
                        my[i, j].Type = field[i, j].Type;
                    }
                }
                owner.UserCell = my;

                Hide(); // Скрывает элементы от пользователя и создает поле для компьютера
                initForm();
                radioButton2.Checked = true;
                radioButton2_CheckedChanged(null, null);
                Cell[,] comp = new Cell[10, 10];
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        comp[i, j] = new Cell();
                        comp[i, j].Column = field[i, j].Column;
                        comp[i, j].Row = field[i, j].Row;
                        comp[i, j].Ship = field[i, j].Ship;
                        comp[i, j].Type = field[i, j].Type;
                    }
                }
                owner.CompCell = comp;
                Close();
            }
            else
            {
                MessageBox.Show("Не все корабли расставлены", "Ошибка", MessageBoxButtons.OK);
            }
        }

        // Кнопка "Правила"
        private void button121_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Для установки корабля на поле " +
                "щелкните по кораблю справа, затем по ячейке на поле. " +
                "Если корабль невозможно установить в выбранную " +
                "ячейку, вы услышите звуковой сигнал. " +
                "Для автоматической расстановки выберите " +
                "соответствующий пункт ниже поля", "Правила игры", MessageBoxButtons.OK);
        }
    }
}
