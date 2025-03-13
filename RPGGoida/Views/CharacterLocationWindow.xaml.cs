using RPGGoida.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace RPGGoida.Views
{
    public partial class CharacterLocationWindow : Window
    {
        private int _userId;
        private int _locationId;
        private readonly Database _database;

        public CharacterLocationWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _database = Database.GetInstance();
            PopulateComboBoxWithCharacterNames();

        }

        private void PopulateComboBoxWithCharacterNames()
        {

            List<Person> characters = _database.GetCharactersByUserId(_userId);


            CharacterNameComboBox.ItemsSource = characters.Select(c => new KeyValuePair<int, string>(c.PersonId, c.PersonName)).ToList();
            CharacterNameComboBox.DisplayMemberPath = "Value";  
            CharacterNameComboBox.SelectedValuePath = "Key";    
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (CharacterNameComboBox.SelectedItem != null)
            {
                var selectedCharacter = (KeyValuePair<int, string>)CharacterNameComboBox.SelectedItem;
                int selectedCharacterId = selectedCharacter.Key;
                bool success = _database.UpdateCharacterLocation(selectedCharacterId, _locationId);

                if (success)
                {
                    MessageBox.Show("Локация персонажа обновлена!");
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении локации.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите персонажа.");
            }
            getFullInformationFromBD();
        }



        private void Location1_Click(object sender, RoutedEventArgs e)
        {
            _locationId = 1;
            getFullInformationFromBD();
        }

        private void Location2_Click(object sender, RoutedEventArgs e)
        {
            _locationId = 2;
            getFullInformationFromBD();
        }

        private void Location3_Click(object sender, RoutedEventArgs e)
        {
            _locationId = 3;
            getFullInformationFromBD();
        }

        private void getFullInformationFromBD()
        {
            List<Person> person_by_location_and_userID = _database.GetCharactersByLocationAndUserId(_locationId, _userId);
            CharactersInLocationDataGrid.ItemsSource = person_by_location_and_userID;



            List<Mob> mobs_by_location = _database.GetMobsByLocationId(_locationId);
            EnemiInLocationDataGrid.ItemsSource = mobs_by_location;


            List<Npc> npcs_by_location = _database.GetNPCByLocationId(_locationId);
            NpcInLocationDataGrid.ItemsSource = npcs_by_location;
        }




    }
}
