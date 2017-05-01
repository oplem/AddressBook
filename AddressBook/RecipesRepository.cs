using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AddressBook
{
    public class RecipesRepository
    {
        private string _connectionString;

        public RecipesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Recipe> GetAllRecipes()
        {
            List<Recipe> recipes = new List<Recipe>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT RecipeTypeId
                         , Name
                      FROM Recipes
                  ORDER BY RecipeTypeId
                         , Name
                ";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int recipeTypeId = reader.GetInt32(0);
                    string title = reader.GetString(1);

                    Recipe recipe = new Recipe(title, (RecipeType) recipeTypeId);
                    recipes.Add(recipe);

                }
            }

            return recipes;
        }

        public void Create(string title, RecipeType choice)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"
                    insert into recipes(recipetypeid, name)
                    values(@giraffe, @lemur)
                ";
                command.Parameters.AddWithValue("@giraffe", choice);
                command.Parameters.AddWithValue("@lemur", title);
                command.ExecuteNonQuery();
            }
        }
    }
}