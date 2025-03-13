using RPGGoida.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RPGGoida.Views
{
    public partial class CharacterAddSkillWindow : Window
    {
        private readonly int _userId;
        private readonly Database _database;
        private PersonViewModel _selectedPerson;

        public CharacterAddSkillWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _database = Database.GetInstance();
            PopulateExistingCharacters();
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

        private void PopulateSkill()
        {
            if (_selectedPerson != null)
            {
                // Получаем доступные навыки
                List<Skill> existingSkills = _database.GetSkillsByPersonIdByClassId(_selectedPerson.PersonId);
                var skillViewModels = existingSkills.Select(skill => new SkillViewModel
                {
                    SkillId = skill.SkillId,   // Добавляем SkillId
                    SkillName = skill.SkillName,
                    SkillLevel = skill.SkillLevel
                }).ToList();

                ExistingSkillsDataGrid.ItemsSource = skillViewModels;

                // Получаем навыки, которыми персонаж уже обладает
                List<Skill> personSkills = _database.GetSkillsByPerson(_selectedPerson.PersonId);
                var skillHaveViewModels = personSkills.Select(skill => new SkillViewModel
                {
                    SkillId = skill.SkillId,   // Добавляем SkillId
                    SkillName = skill.SkillName,
                    SkillLevel = skill.SkillLevel
                }).ToList();

                HaveSkillsDataGrid.ItemsSource = skillHaveViewModels;
            }
        }


        private void ExistingCharactersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedPerson = ExistingCharactersDataGrid.SelectedItem as PersonViewModel;
            PopulateSkill();
        }

        /// <summary>
        /// Обработчик начала перетаскивания навыка из списка доступных навыков.
        /// </summary>
        private void ExistingSkillsDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid?.SelectedItem is SkillViewModel selectedSkill)
            {
                // Передаем сам объект `SkillViewModel`
                DragDrop.DoDragDrop(dataGrid, selectedSkill, DragDropEffects.Move);
            }
        }



        /// <summary>
        /// Обработчик сброса навыка в список имеющихся у персонажа навыков.
        /// </summary>
        private void HaveSkillsDataGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SkillViewModel)))
            {
                SkillViewModel skill = e.Data.GetData(typeof(SkillViewModel)) as SkillViewModel;
                if (skill != null && _selectedPerson != null)
                {
                    // Добавляем навык персонажу в БД по SkillId
                    _database.AddSkillToCharacter(skill.SkillId, _selectedPerson.PersonId);

                    // Обновляем список навыков
                    PopulateSkill();
                }
            }
        }


    }
}
