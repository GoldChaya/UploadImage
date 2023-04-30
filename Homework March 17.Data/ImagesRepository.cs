using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_March_17.Data
{
    public class ImagesRepository
    {
        private string _connectionString;
        public ImagesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Add (Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Image (FileName, Password, Views) VALUES (@fileName, @password, 0) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@fileName", image.FileName);
            command.Parameters.AddWithValue("@password", image.Password);
            connection.Open();
            image.Id = (int)(decimal)command.ExecuteScalar();
        }
        public bool PasswordMatch(int id, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT (*) FROM Image WHERE Id = @id AND Password = @password";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@password", password);
            connection.Open();
            int result = (int)command.ExecuteScalar();
            if(result==0)
            {
                return false;
            }
            return true;
        }
        public Image GetImageForId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Image WHERE Id = @id";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            Image image = new Image
            {
                Id = (int)reader["Id"],
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]

            };
            return image;
        }
        public void IncrementViewCount(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Image SET Views = Views + 1 WHERE Id = @id";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
