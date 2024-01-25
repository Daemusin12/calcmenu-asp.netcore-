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
                    String sql = "SELECT " +
                                 "recipes.id, recipes.recipe_name, recipes.recipe_procedure, recipes.servings, recipes.created_at, " +
                                 "ingredients.id, ingredients.recipe_id, ingredients.ingredient_name, ingredients.quantity, ingredients.unit "  +
                                 "FROM recipes " +
                                 "LEFT JOIN ingredients ON recipes.id = ingredients.recipe_id " +
                                 "ORDER BY recipes.id, ingredients.id;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Recipe currentRecipe = null;
                            while (reader.Read())
                            {
                                string recipeId = reader.GetInt32(0).ToString();
                                if (currentRecipe == null || currentRecipe.id != recipeId)
                                {
                                    currentRecipe = new Recipe
                                    {
                                        id = recipeId,
                                        recipe_name = reader.GetString(1),
                                        recipe_procedure = reader.GetString(2),
                                        servings = reader.GetInt32(3).ToString(),
                                        created_at = reader.GetDateTime(4).ToString(),
                                        ingredients = new List<Ingredients>()
                                     };

                                    listRecipes.Add(currentRecipe);
                                }

                                if (!reader.IsDBNull(5))
                                {
                                    Ingredients ingredient = new Ingredients
                                     {
                                        id = reader.GetInt32(5).ToString(),
                                        recipe_id = reader.GetInt32(6).ToString(),
                                        ingredient_name = reader.GetString(7),
                                        quantity = reader.GetInt32(8).ToString(),
                                        unit = reader.GetString(9)
                                    };

                                     currentRecipe.ingredients.Add(ingredient);
                                }
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
        public List<Ingredients> ingredients = new List<Ingredients>();
    }

    public class Ingredients
    {
        public String id;
        public String recipe_id;
        public String ingredient_name;
        public String quantity;
        public String unit;
    }
}
