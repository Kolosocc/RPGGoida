using RPGGoida.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace RPGGoida.Models
{
    public class Database
    {
        private static Database _instance;
        private readonly string _connectionString;

        private Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static Database GetInstance(string connectionString = null)
        {
            if (_instance == null && connectionString != null)
            {
                _instance = new Database(connectionString);
            }
            return _instance;
        }

        public void CreateUser(string email, string password)
        {
            string hashedPassword = Hashing.HashPassword(password);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO users (username, email, password_hash) VALUES (@Username, @Email, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", "Player");
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    command.ExecuteNonQuery();
                }
            }
        }

        public int? AuthenticateUser(string email, string password)
        {
            string hashedPassword = Hashing.HashPassword(password);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT user_id FROM users WHERE email = @Email AND password_hash = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return null;
        }


        public bool CheckUserExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM users WHERE email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
        }

        public List<Person> GetCharactersByUserId(int userId)
        {
            List<Person> persons = new List<Person>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT p.person_id, p.user_id, p.person_name, p.person_level, p.person_nation, p.person_gender, c.class_id, c.class_name
                FROM person p
                LEFT JOIN class c ON p.class_id = c.class_id
                WHERE p.user_id = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persons.Add(new Person
                            {
                                PersonId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                PersonName = reader.GetString(2),
                                PersonLevel = reader.GetInt32(3),
                                PersonNation = reader.GetString(4),
                                PersonGender = reader.GetString(5),
                                Class = new Class
                                {
                                    ClassId = reader.GetInt32(6),
                                    ClassName = reader.GetString(7)
                                }
                            });
                        }
                    }
                }
            }

            return persons;
        }

        public void CreateCharacter(int userId, string characterName, string nation, string gender, int classId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO person (user_id, person_name, person_nation, person_gender, class_id) " +
                               "VALUES (@UserId, @CharacterName, @Nation, @Gender, @ClassId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CharacterName", characterName);
                    command.Parameters.AddWithValue("@Nation", nation);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@ClassId", classId);

                    command.ExecuteNonQuery();
                }
            }
        }
        public List<Class> GetClasses()
        {
            List<Class> classes = new List<Class>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT class_id, class_name FROM class";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            classes.Add(new Class
                            {
                                ClassId = reader.GetInt32(0),
                                ClassName = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return classes;
        }

        public void UpdateCharacter(int personId, string characterName, string nation, string gender, int classId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE person SET person_name = @CharacterName, person_nation = @Nation, person_gender = @Gender, class_id = @ClassId WHERE person_id = @PersonId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CharacterName", characterName);
                    command.Parameters.AddWithValue("@Nation", nation);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@ClassId", classId);
                    command.Parameters.AddWithValue("@PersonId", personId);
                    command.ExecuteNonQuery();
                }
            }
        }



        public bool UpdateCharacterLocation(int personId, int locationId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Обновляем локацию персонажа
                string query = "UPDATE person SET location_id = @LocationId WHERE person_id = @PersonId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocationId", locationId);
                    command.Parameters.AddWithValue("@PersonId", personId);

                    int rowsAffected = command.ExecuteNonQuery();

                    // Если строк обновлена, значит обновление прошло успешно
                    return rowsAffected > 0;
                }
            }
        }





        public List<Person> GetCharactersByLocationAndUserId(int locationId, int userId)
        {
            List<Person> characters = new List<Person>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT person_name, person_level FROM person WHERE user_id = @userId AND location_id = @locationId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@locationId", locationId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                characters.Add(new Person
                                {
                                    PersonName = reader.GetString(0),
                                    PersonLevel = reader.GetInt32(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при получении персонжей  {ex.Message}");
            }
            return characters;

        }

        public List<Mob> GetMobsByLocationId(int locationId)
        {
            List<Mob> mobs = new List<Mob>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT mob_id, mob_name, mob_level, mob_damage, mob_type, location_id FROM mob WHERE location_id = @locationId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@locationId", locationId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mobs.Add(new Mob
                                {
                                    MobId = reader.GetInt32(0),
                                    MobName = reader.GetString(1),
                                    MobLevel = reader.GetInt32(2),
                                    MobDamage = reader.GetInt32(3),
                                    MobType = reader.GetString(4),
                                    LocationId = reader.GetInt32(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при получении мобов: {ex.Message}");
            }

            return mobs;
        }

        public List<Npc> GetNPCByLocationId(int locationId)
        {
            List<Npc> npc = new List<Npc>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT npc_id, npc_name, npc_level, location_id FROM npc WHERE location_id = @locationId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@locationId", locationId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                npc.Add(new Npc
                                {
                                    NpcId = reader.GetInt32(0),
                                    NpcName = reader.GetString(1),
                                    NpcLevel = reader.GetInt32(2),
                                    LocationId = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при получении мобов: {ex.Message}");
            }

            return npc;
        }




        public void AddSkillToCharacter(int skillId, int personId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                INSERT INTO person_skill (person_id, skill_id) 
                VALUES (@personId, @skillId)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@personId", personId);
                        command.Parameters.AddWithValue("@skillId", skillId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при добавлении скилла к персонажу: {ex.Message}");
            }
        }





        public List<Skill> GetSkillsByPersonIdByClassId(int personId)
        {
            List<Skill> skills = new List<Skill>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string classIdQuery = "SELECT class_id FROM person WHERE person_id = @personId";
                    int classId = -1; 

                    using (SqlCommand classIdCommand = new SqlCommand(classIdQuery, connection))
                    {
                        classIdCommand.Parameters.AddWithValue("@personId", personId);
                        var result = classIdCommand.ExecuteScalar();
                        if (result != null)
                        {
                            classId = Convert.ToInt32(result);
                        }
                    }

                    if (classId == -1)
                    {
                        MessageBox.Show("Class not found for the given person.");
                        return skills;
                    }

                    string skillQuery = "SELECT skill_id, skill_name, skill_level FROM skill WHERE class_id = @classId";

                    using (SqlCommand skillCommand = new SqlCommand(skillQuery, connection))
                    {
                        skillCommand.Parameters.AddWithValue("@classId", classId);

                        using (SqlDataReader reader = skillCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                skills.Add(new Skill
                                {
                                    SkillId = Convert.ToInt32(reader["skill_id"]),
                                    SkillName = reader["skill_name"].ToString(),
                                    SkillLevel = Convert.ToInt32(reader["skill_level"])
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error while retrieving skills: {ex.Message}");
            }

            return skills;
        }


        public List<Skill> GetSkillsByPerson(int personId)
        {
            List<Skill> skills = new List<Skill>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string skillQuery = @"
                SELECT 
                    p.skill_id, 
                    p.skill_level, 
                    s.skill_name 
                FROM person_skill p
                INNER JOIN skill s ON s.skill_id = p.skill_id
                WHERE p.person_id = @personId";

                    using (SqlCommand skillCommand = new SqlCommand(skillQuery, connection))
                    {
                        skillCommand.Parameters.AddWithValue("@personId", personId);

                        using (SqlDataReader reader = skillCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                skills.Add(new Skill
                                {
                                    SkillId = reader.GetInt32(0),
                                    SkillName = reader.GetString(2),
                                    SkillLevel = reader.GetInt32(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error while retrieving skills: {ex.Message}");
            }

            return skills;
        }



    }
}
