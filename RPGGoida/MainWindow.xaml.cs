using RPGGoida.Models;
using RPGGoida.Views;
using System;
using System.IO;
using System.Windows;


namespace RPGGoida
{
    public partial class MainWindow : Window
    {
        private readonly Database _database;

        public MainWindow()
        {
            InitializeComponent();

            string envFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, ".env");
            DotEnv.Load(envFilePath);
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Ошибка: переменная DB_CONNECTION_STRING не найдена в окружении!");
                Application.Current.Shutdown();
                return;
            }

            _database = Database.GetInstance(connectionString);
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailTextBox.Text) && !string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                string email = EmailTextBox.Text;
                string password = PasswordTextBox.Password;
                int? id_user = _database.AuthenticateUser(email, password);

                if (id_user.HasValue)
                {
                    try
                    {
                        int userId = id_user.Value;
                        AddCharacterWindow characterForm = new AddCharacterWindow(userId);
                        characterForm.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex + " Кто-то не прав");
                    }
                }
                else
                {
                    MessageBox.Show("ИНВАЛИД ЕРОР");
                }
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailTextBox.Text) && !string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                string email = EmailTextBox.Text;
                string password = PasswordTextBox.Password;

                if (_database.CheckUserExists(email))
                {
                    MessageBox.Show("УЖЕ ЕСТЬ!");
                }
                else
                {
                    _database.CreateUser(email, password);
                    Login_Button_Click(null, null);
                }
            }
            else
            {
                MessageBox.Show("НИЧЕ НЕТ");
            }
        }
    }
}
