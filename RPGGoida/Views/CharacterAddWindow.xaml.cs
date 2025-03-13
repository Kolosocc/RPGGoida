using RPGGoida.Models;
using RPGGoida.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RPGGoida.Views
{
    public partial class AddCharacterWindow : Window
    {
        private readonly int _userId;
        private readonly Database _database;
        private int? _selectedCharacterId = null; 

        public AddCharacterWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _database = Database.GetInstance();
            PopulateClassComboBox();
            PopulateExistingCharacters();
        }

        private void PopulateClassComboBox()
        {
            List<Class> classes = _database.GetClasses();
            ClassComboBox.Items.Clear();
            foreach (var classItem in classes)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = classItem.ClassName,
                    Tag = classItem.ClassId
                };
                ClassComboBox.Items.Add(item);
            }
        }

        private void PopulateExistingCharacters()
        {
            List<Person> existingCharacters = _database.GetCharactersByUserId(_userId);
            var personViewModels = existingCharacters.Select(person => new PersonViewModel
            {
                PersonName = person.PersonName,
                PersonLevel = person.PersonLevel,
                PersonNation = person.PersonNation,
                PersonGender = person.PersonGender,
                ClassNames = person.Class.ClassName,
                PersonId = person.PersonId
            }).ToList();

            ExistingCharactersDataGrid.ItemsSource = personViewModels;
        }

        private void CreateOrUpdateCharacter_Button_Click(object sender, RoutedEventArgs e)
        {
            string characterName = CharacterNameTextBox.Text;
            string nation = (NationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int selectedClassId = (int)((ComboBoxItem)ClassComboBox.SelectedItem)?.Tag;

            if (string.IsNullOrEmpty(characterName) || string.IsNullOrEmpty(nation) || selectedClassId == 0 || string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            try
            {
                if (_selectedCharacterId.HasValue)
                {
                    _database.UpdateCharacter(_selectedCharacterId.Value, characterName, nation, gender, selectedClassId);
                    MessageBox.Show("Персонаж обновлен!");
                }
                else
                {
                    _database.CreateCharacter(_userId, characterName, nation, gender, selectedClassId);
                    MessageBox.Show("Персонаж создан!");
                }

                PopulateExistingCharacters();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении персонажа: " + ex.Message);
            }
        }

        private void ExistingCharactersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExistingCharactersDataGrid.SelectedItem is PersonViewModel selectedPerson)
            {
                _selectedCharacterId = selectedPerson.PersonId;
                CharacterNameTextBox.Text = selectedPerson.PersonName;
                NationComboBox.SelectedItem = NationComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == selectedPerson.PersonNation);
                GenderComboBox.SelectedItem = GenderComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == selectedPerson.PersonGender);
                ClassComboBox.SelectedItem = ClassComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == selectedPerson.ClassNames);

                CreateCharacterButton.Content = "Изменить";
            }
        }

        private void ResetForm()
        {
            _selectedCharacterId = null;
            CharacterNameTextBox.Clear();
            NationComboBox.SelectedIndex = -1;
            GenderComboBox.SelectedIndex = -1;
            ClassComboBox.SelectedIndex = -1;
            CreateCharacterButton.Content = "Создать";
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChooseLocationButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterLocationWindow CharacterLocationWindow = new CharacterLocationWindow(_userId);
            CharacterLocationWindow.ShowDialog();
        }

        private void ChooseSkillButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterAddSkillWindow characterAddSkillWindow = new CharacterAddSkillWindow(_userId);
            characterAddSkillWindow.ShowDialog();
        }
        private void SortAscButton_Click(object sender, RoutedEventArgs e)
        {

            var sortedList = _database.GetCharactersByUserId(_userId)
                .OrderBy(person => person.PersonLevel)
                .Select(person => new PersonViewModel
                {
                    PersonName = person.PersonName,
                    PersonLevel = person.PersonLevel,
                    PersonNation = person.PersonNation,
                    PersonGender = person.PersonGender,
                    ClassNames = person.Class.ClassName,
                    PersonId = person.PersonId
                }).ToList();

            ExistingCharactersDataGrid.ItemsSource = sortedList;
        }

        private void SortDescButton_Click(object sender, RoutedEventArgs e)
        {
            var sortedList = _database.GetCharactersByUserId(_userId)
                .OrderByDescending(person => person.PersonLevel)
                .Select(person => new PersonViewModel
                {
                    PersonName = person.PersonName,
                    PersonLevel = person.PersonLevel,
                    PersonNation = person.PersonNation,
                    PersonGender = person.PersonGender,
                    ClassNames = person.Class.ClassName,
                    PersonId = person.PersonId
                }).ToList();

            ExistingCharactersDataGrid.ItemsSource = sortedList;
        }

    }

}
