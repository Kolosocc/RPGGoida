using System;
using System.Data.SqlClient;

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

        public string ConnectionString => _connectionString;

        public int? AuthenticateUser(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT user_id FROM users WHERE email = @Email AND password_hash = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

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

        // Проверка существования пользователя
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

        // Создание нового пользователя
        public void CreateUser(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO users (username, email, password_hash) VALUES (@Username, @Email, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", "Player");
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Получение пользователя по id
        public User GetUserById(int user_id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
SELECT u.username, s.skill_name AS Skill, c.class_name AS Class
FROM users u
LEFT JOIN person p ON p.user_id = u.user_id
LEFT JOIN person_class pc ON pc.person_id = p.person_id
LEFT JOIN class c ON c.class_id = pc.class_id
LEFT JOIN skill s ON s.class_id = c.class_id
WHERE u.user_id = @Id"; 

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Убираем лишние пробелы
                    command.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int) { Value = user_id});

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Username = reader["username"].ToString(),
                                Skill = reader["Skill"]?.ToString(),
                                Class = reader["Class"]?.ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }


        // Обновление пользователя в базе данных
        public void UpdateUser(int user_id, string username, string skill, string userClass)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE users SET username = @Username WHERE email = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Id", user_id);

                    command.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(skill) && !string.IsNullOrEmpty(userClass))
                {
                    // Обновляем скилл и класс пользователя, если данные не пустые
                    string updateSkillQuery = 
                        "UPDATE skill SET skill_name = @Skill" +
                        " WHERE class_id = (SELECT class_id FROM class WHERE class_name = @Class)" +
                        " AND skill_id = (SELECT skill_id FROM skill" +
                        " WHERE class_id = (SELECT class_id FROM class" +
                        " WHERE class_name = @Class))";
                    using (SqlCommand command = new SqlCommand(updateSkillQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Skill", skill);
                        command.Parameters.AddWithValue("@Class", userClass);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
