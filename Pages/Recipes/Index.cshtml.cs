using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace RecipeApp.Pages.Recipes
{
    public class IndexModel : PageModel
    {
        public List<Recipe> listRecipes = new List<Recipe>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=.\\mssqlserver01;Initial Catalog=recipe;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM recipes";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Recipe recipe = new Recipe();
                                recipe.id = reader.GetInt32(0).ToString();
                                recipe.recipe_name = reader.GetString(1);
                                recipe.recipe_procedure = reader.GetString(2);
                                recipe.servings = reader.GetInt32(3).ToString();
                                recipe.created_at = reader.GetDateTime(4).ToString();

                                listRecipes.Add(recipe);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }

    public class Recipe
    {
        public String id;
        public String recipe_name;
        public String servings;
        public String recipe_procedure;
        public String created_at;
    }
}
