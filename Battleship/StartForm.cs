using System;
using System.Windows.Forms;

namespace Battleship
{
    public partial class StartForm : Form
    {
        bool IsClosing 
        {
            get;set;
        }

        public StartForm()
        {
            IsClosing = false;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Если в поле что-то написано,то 
            if (textBox1.Text!="")
            {
                // Очищаем поле ошибок
                errorProvider1.Clear();
                IsClosing = true;
                // Записываем в переменную данные, которые ввел пользователь
                ((FiledForm)Owner).UserName = textBox1.Text;
                Close();
            }
            // Если пользователь не ввел имя, то выводим ошибку
            else
            {
                errorProvider1.SetError(textBox1, "Пожалуйста, введите имя");
            }
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }

        // Организуем выход из программы на этапе ввода имени
        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!((FiledForm)Owner).IsClosing&&!IsClosing)
            {
                if (MessageBox.Show("Выйти из программы?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ((FiledForm)Owner).IsClosing = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}