using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public partial class PlayForm : Form
    {
        public bool first = true;

        public Cell[,] UserCell
        {
            get; set;
        }

        public Cell[,] CompCell
        {
            get; set;
        }
        Cell[,] userShip;
        Cell[,] compShip;
        public bool IsClosing
        {
            get; set;
        }

        public PlayForm()
        {
            IsClosing = false;
            FiledForm filedForm = new FiledForm(this);
            if (!filedForm.IsClosing)
                filedForm.ShowDialog();
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Battleship_Load(object sender, EventArgs e)
        {
            label43.Text = "Ваш ход";
            if (IsClosing)
            {
                Close();
                return;
            }
            init(); //Метод задания только для инициализации назначает значение
                    //свойству или элементу индексатора только во время создания объекта.
                    //Это обеспечивает неизменяемость, чтобы после инициализации объекта его нельзя было изменить снова.

            // Определяем поле пользователя
            var myControls = panel1.Controls;
            userShip = new Cell[10, 10];
            foreach (Control control in myControls)
            {
                if (control is Button)
                {
                    ((Button)control).BackColor = SystemColors.Control;
                    int tag = int.Parse(control.Tag.ToString());
                    userShip[(tag - 100) / 10, (tag - 100) % 10] = new Cell();
                    userShip[(tag - 100) / 10, (tag - 100) % 10].Btn = (Button)control;
                    userShip[(tag - 100) / 10, (tag - 100) % 10].Type = UserCell[(tag - 100) / 10, (tag - 100) % 10].Type;
                    userShip[(tag - 100) / 10, (tag - 100) % 10].Column = (tag - 100) % 10;
                    userShip[(tag - 100) / 10, (tag - 100) % 10].Row = (tag - 100) / 10;
                    userShip[(tag - 100) / 10, (tag - 100) % 10].Ship = UserCell[(tag - 100) / 10, (tag - 100) % 10].Ship;
                    ((Button)control).Enabled = false;
                    // Если пользователь попал по кораблю, то закрашиваем ячейку серым 
                    if (UserCell[(tag - 100) / 10, (tag - 100) % 10].Type == CellType.Ship)
                    {
                        ((Button)control).BackColor = Color.Gray;
                    }
                }
            }

            // Определяем поле компьютера
            var compControls = panel2.Controls;
            compShip = new Cell[10, 10];
            foreach (Control control in compControls)
            {
                if (control is Button)
                {
                    ((Button)control).BackColor = SystemColors.Control;
                    int tag = int.Parse(control.Tag.ToString());
                    compShip[(tag - 100) / 10, (tag - 100) % 10] = new Cell();
                    compShip[(tag - 100) / 10, (tag - 100) % 10].Btn = (Button)control;
                    compShip[(tag - 100) / 10, (tag - 100) % 10].Type = CompCell[(tag - 100) / 10, (tag - 100) % 10].Type;
                    compShip[(tag - 100) / 10, (tag - 100) % 10].Column = (tag - 100) % 10;
                    compShip[(tag - 100) / 10, (tag - 100) % 10].Row = (tag - 100) / 10;
                    compShip[(tag - 100) / 10, (tag - 100) % 10].Ship = CompCell[(tag - 100) / 10, (tag - 100) % 10].Ship;
                    if (first) ((Button)control).Click += userAttack;
                }
            }

            // Компьютер запоминает ячейки, которые он "ранил" у пользователя
            first = false;
            attacks = new List<Tuple<int, int>>(); // Инициализируем новый экземпляр класса List, который представляет собой кортеж Tuple, состоящий из 2х элементов
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Tuple<int, int> tuple = new Tuple<int, int>(i, j);
                    attacks.Add(tuple);
                }
            }
        }
        double userAllHits = 0;
        double userAllAttacks = 0;
        private void userAttack(object sender, EventArgs e)
        {
            //обработать атаку
            bool success = false;
            Button btn = (Button)sender;
            int tag = int.Parse(btn.Tag.ToString());
            int row = (tag - 100) / 10;
            int column = (tag - 100) % 10;
            //если пусто - ставим точку
            //если был корабль, открываем и проверяем все части убиты или нет, если все - 
            //обновляем все ячейки
            if (compShip[row, column].Type == CellType.Ship)
            {
                userAllHits++;
                userAllAttacks++;
                success = true; // успешное попадание
                btn.Enabled = false; // ячейка становится недоступной
                compShip[row, column].Type = CellType.Shot; // отмечаем попадание
                bool dead = true;
                foreach (Cell cell in compShip[row, column].Ship)
                {
                    if (compShip[cell.Row, cell.Column].Type != CellType.Shot) // для каждой не "раненого" корабля
                    {
                        dead = false;
                        break;
                    }
                }
                if (dead) // Если полнотью убили корабль, закрашиваем его зеленым и отмечаем Х
                {
                    if (compShip[row, column].Ship.Count == 1)
                        c1--;
                    if (compShip[row, column].Ship.Count == 2)
                        c2--;
                    if (compShip[row, column].Ship.Count == 3)
                        c3--;
                    if (compShip[row, column].Ship.Count == 4)
                        c4--;
                    foreach (Cell cell in compShip[row, column].Ship)
                    {
                        compShip[cell.Row, cell.Column].Btn.Text = "X";
                        compShip[cell.Row, cell.Column].Btn.BackColor = Color.Green;
                    }
                }
                else // Если ранили часть корабля
                {
                    btn.Text = "X";
                    btn.BackColor = Color.Gray;
                }
            }
            else // Если промах
            {
                compShip[row, column].Btn.Text = "•";
                compShip[row, column].Btn.Enabled = false;
                userAllAttacks++;
            }

            if ((u1 > 0 || u2 > 0 || u3 > 0 || u4 > 0) && (c1 > 0 || c2 > 0 || c3 > 0 || c4 > 0))
            {
                if (!success)
                {
                    label43.Text = "Ход компьютера";
                    compAttack();
                    System.Threading.Thread.Sleep(50);
                }
                else label43.Text = "Ваш ход";
            }
            else
            {
                if ((u1 > 0 || u2 > 0 || u3 > 0 || u4 > 0))
                {
                    double compper = Math.Round((compAllHits / compAllAttacks) * 100, 2);
                    double userper = Math.Round((userAllHits / userAllAttacks) * 100, 2);
                    string text = "Вы выиграли!\nПроцент попадания компьютера: "
                + compper.ToString() + " %" + "\nПроцент попадания игрока: " + userper.ToString() + " %";
                    MessageBox.Show(text, "Информация");
                }
                else
                {
                    double userper = Math.Round((userAllHits / userAllAttacks) * 100, 2);
                    double compper = Math.Round((compAllHits / compAllAttacks) * 100, 2);
                    string text = "Вы проиграли...\nПроцент попадания компьютера: "
                    + compper.ToString() + " %" + "\nПроцент попадания игрока: " + userper.ToString() + " %";
                    MessageBox.Show(text, "Информация");
                }
            }
        }

        List<Tuple<int, int>> attacks = new List<Tuple<int, int>>();
        Random random = new Random((int)DateTime.Now.Ticks);

        double compAllHits = 0;
        double compAllAttacks = 0;
        static class GlobalVars
        {
            public static int shotrow;
            public static int shotcol;
        }

        bool got = false;
        private void compAttack()
        {
            //совершить атаку
            bool success = true;
            while (success)
            {
                int index = random.Next() % attacks.Count;
                int row;
                int column;
                Button btn;
                if (got)
                {
                    row = GlobalVars.shotrow;
                    column = GlobalVars.shotcol;
                    btn = userShip[row, column].Btn;
                    attacks.Remove(new Tuple<int, int>(row, column));
                }
                else
                {
                    Tuple<int, int> tuple = attacks[index];
                    attacks.RemoveAt(index);
                    row = tuple.Item1;
                    column = tuple.Item2;
                    btn = userShip[row, column].Btn;
                }

                //если пусто - ставим точку
                //если был корабль, открываем и проверяем все части убиты или нет, если все - 
                //обновляем все ячейки
                if (userShip[row, column].Type == CellType.Ship)
                {
                    getShoot();
                    int a = row;
                    int b = column;
                    GlobalVars.shotrow = a;
                    GlobalVars.shotcol = b;
                    attacks.Remove(new Tuple<int, int>(a - 1, b - 1));
                    attacks.Remove(new Tuple<int, int>(a + 1, b - 1));
                    attacks.Remove(new Tuple<int, int>(a - 1, b + 1));
                    attacks.Remove(new Tuple<int, int>(a + 1, b + 1));
                    compAllHits++;
                    compAllAttacks++;
                    btn.Enabled = false;
                    userShip[a, b].Type = CellType.Shot;
                    bool dead = true;
                    if (a - 1 >= 0)
                    {
                        if (userShip[a - 1, b].Type == CellType.Ship)
                        {
                            a--;
                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else if ((userShip[a - 1, b].Type == CellType.Shot) && (a + 1 < 10))
                        {
                            a++;
                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else
                        {
                            userShip[a - 1, b].Btn.Text = "•";
                            a--;
                            userShip[a, b].Btn.Enabled = false;
                            compAllAttacks++;
                            success = false;
                            miss();
                        }
                    }
                    else if (b + 1 < 10)
                    {
                        if (userShip[a, b + 1].Type == CellType.Ship)
                        {
                            b++;
                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else if ((userShip[a, b + 1].Type == CellType.Shot) && (b - 1 > 0))
                        {
                            b--;
                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else
                        {
                            userShip[a, b + 1].Btn.Text = "•";
                            b++;
                            userShip[a, b].Btn.Enabled = false;
                            compAllAttacks++;
                            success = false;
                            miss();
                        }
                    }
                    else if (a + 1 < 10)
                    {
                        if (userShip[a + 1, b].Type == CellType.Ship)
                        {
                            a++;
                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else if ((userShip[a + 1, b].Type == CellType.Shot) && (a - 1 > 0))
                        {
                            a--;

                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else
                        {
                            userShip[a + 1, b].Btn.Text = "•";
                            a++;
                            userShip[a, b].Btn.Enabled = false;
                            compAllAttacks++;
                            success = false;
                            miss();
                        }
                    }
                    else if (b - 1 > 0)
                    {
                        if (userShip[a, b - 1].Type == CellType.Ship)
                        {
                            b--;
                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else if ((userShip[a, b - 1].Type == CellType.Shot) && (b + 1 < 10))
                        {
                            b++;
                            btn.Enabled = false;
                            userShip[a, b].Type = CellType.Shot;
                            compAllAttacks++;
                            compAllHits++;
                            getShoot();
                        }
                        else
                        {
                            userShip[a, b - 1].Btn.Text = "•";
                            b--;
                            userShip[a, b].Btn.Enabled = false;
                            compAllAttacks++;
                            success = false;
                            miss();
                        }
                    }


                    foreach (Cell cell in userShip[row, column].Ship)
                    {
                        if (userShip[cell.Row, cell.Column].Type != CellType.Shot)
                        {
                            dead = false;
                            break;
                        }
                    }

                    if (dead)
                    {
                        //удалить из списка все ячейки вокруг
                        foreach (Cell cell in userShip[row, column].Ship)
                        {
                            int i = cell.Row;
                            int j = cell.Column;
                            attacks.Remove(new Tuple<int, int>(i, j));
                            attacks.Remove(new Tuple<int, int>(i - 1, j));
                            attacks.Remove(new Tuple<int, int>(i + 1, j));
                            attacks.Remove(new Tuple<int, int>(i, j - 1));
                            attacks.Remove(new Tuple<int, int>(i - 1, j - 1));
                            attacks.Remove(new Tuple<int, int>(i + 1, j - 1));
                            attacks.Remove(new Tuple<int, int>(i, j + 1));
                            attacks.Remove(new Tuple<int, int>(i - 1, j + 1));
                            attacks.Remove(new Tuple<int, int>(i + 1, j + 1));

                            if (userShip[cell.Row, cell.Column].Type != CellType.Shot)
                            {
                                dead = false;
                                break;
                            }

                        }

                        if (userShip[row, column].Ship.Count == 1)
                            u1--;
                        miss();
                        if (userShip[row, column].Ship.Count == 2)
                            u2--;
                        if (userShip[row, column].Ship.Count == 3)
                            u3--;
                        if (userShip[row, column].Ship.Count == 4)
                            u4--;
                        foreach (Cell cell in userShip[row, column].Ship)
                        {
                            userShip[cell.Row, cell.Column].Btn.Text = "X";
                            userShip[cell.Row, cell.Column].Btn.BackColor = Color.Green;
                        }
                    }
                    else
                    {
                        btn.Text = "X";
                        btn.BackColor = Color.Gray;

                    }
                }
                else
                {
                    userShip[row, column].Btn.Text = "•";
                    userShip[row, column].Btn.Enabled = false;
                    success = false;
                    miss();
                    compAllAttacks++;
                }
                if ((u1 > 0 || u2 > 0 || u3 > 0 || u4 > 0) && (c1 > 0 || c2 > 0 || c3 > 0 || c4 > 0))
                {

                }
                else
                {
                    if ((u1 > 0 || u2 > 0 || u3 > 0 || u4 > 0))
                    {
                        double compper = Math.Round((compAllHits / compAllAttacks) * 100, 2);
                        double userper = Math.Round((userAllHits / userAllAttacks) * 100, 2);
                        string text = "Вы выиграли!\nПроцент попадания компьютера: "
                    + compper.ToString() + " %" + "\nПроцент попадания игрока: " + userper.ToString() + " %";
                        MessageBox.Show(text, "Информация");
                        break;
                    }
                    else
                    {
                        double userper = Math.Round((userAllHits / userAllAttacks) * 100, 2);
                        double compper = Math.Round((compAllHits / compAllAttacks) * 100, 2);
                        string text = "Вы проиграли...\nПроцент попадания компьютера: "
                        + compper.ToString() + " %" + "\nПроцент попадания игрока: " + userper.ToString() + " %";
                        MessageBox.Show(text, "Информация");
                        break;
                    }
                }
            }
        }

        void getShoot()
        {
            got = true;
        }
        void miss()
        {
            got = false;
        }

        // Количество кораблей пользователя
        int u1 = 4; // однопалубники
        int u2 = 3; // двухпалубники
        int u3 = 2; // трехпалубники
        int u4 = 1; // четырехпалубники
        // Количество кораблей компьютера
        int c1 = 4;
        int c2 = 3;
        int c3 = 2;
        int c4 = 1;

        private void play()
        {
            bool user = true;
            while ((u1 > 0 || u2 > 0 || u3 > 0 || u4 > 0) && (c1 > 0 || c2 > 0 || c3 > 0 || c4 > 0))
            {
                if (user)
                {

                }
                else
                {

                }
            }
        }
        private void init()
        {
            //очищаем ячейки
            var myControls = panel1.Controls;
            userShip = new Cell[10, 10];
            foreach (Control control in myControls)
            {
                if (control is Button)
                {
                    int tag = int.Parse(control.Tag.ToString());
                    userShip[(tag - 100) / 10, (tag - 100) % 10] = new Cell();
                    userShip[(tag - 100) / 10, (tag - 100) % 10].Btn = (Button)control;
                    control.Text = "";
                    control.BackgroundImage = null;
                }
            }
            var compControls = panel2.Controls;
            compShip = new Cell[10, 10];
            foreach (Control control in compControls)
            {
                if (control is Button)
                {
                    int tag = int.Parse(control.Tag.ToString());
                    compShip[(tag - 100) / 10, (tag - 100) % 10] = new Cell();
                    compShip[(tag - 100) / 10, (tag - 100) % 10].Btn = (Button)control;
                    control.Text = "";
                    control.BackgroundImage = null;
                }
            }
            //инициализируем массивы ячеек

        }

        private void PlayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsClosing)
            {
                if (MessageBox.Show("Выйти из программы?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void button202_Click(object sender, EventArgs e)
        {
            IsClosing = false;
            FiledForm filedForm = new FiledForm(this);
            if (!filedForm.IsClosing)
                filedForm.ShowDialog();
            Battleship_Load(null, null);
        }

        // Если пользователь сдался, показываем корабли копьютера
        private void button201_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (compShip[i, j].Type != CellType.Open)
                    {
                        compShip[i, j].Btn.BackColor = Color.Green;
                    }
                    compShip[i, j].Btn.Enabled = false;
                }
            }
            string text = "Вы проиграли...\nПроцент попадания компьютера: "
                    + $"" + "\nПроцент попадания игрока: " + $"";
            MessageBox.Show(text, "Информация");
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}