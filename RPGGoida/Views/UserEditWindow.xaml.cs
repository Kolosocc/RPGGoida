using RPGGoida.Models;  // Импортируем пространство имен для работы с Database
using System;
using System.Windows;

namespace RPGGoida.Views
{
    public partial class UserEditWindow : Window
    {
        private int _currentUserId;
        private Database _database;

        // Конструктор, где передаем email для загрузки данных
        public UserEditWindow(int id_user)
        {
            InitializeComponent();

            _currentUserId = id_user;

            _database = Database.GetInstance();

            // Загружаем данные пользователя
            var user = _database.GetUserById(id_user);

            if (user != null)
            {
                // Заполняем поля формы данными пользователя
                UsernameTextBox.Text = user.Username;
                SkillsComboBox.SelectedItem = user.Skill;
                ClassComboBox.SelectedItem = user.Class;
            }
            else
            {
                MessageBox.Show("Пользователь не найден!");
                Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _database.UpdateUser(_currentUserId, UsernameTextBox.Text, SkillsComboBox.SelectedItem?.ToString(), ClassComboBox.SelectedItem?.ToString());
                MessageBox.Show("Изменения успешно сохранены!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
            }
        }
    }
}
