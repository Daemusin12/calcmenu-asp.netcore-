using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace RecipeApp.Pages.Recipes
{
    public class EditModel : PageModel
    {
        public Recipe recipe = new Recipe();
        public Ingredients ingredients = new Ingredients();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                String connectionString = "Data Source=.\\mssqlserver01;Initial Catalog=recipe;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT " +
                                 "recipes.id, recipes.recipe_name, recipes.recipe_procedure, recipes.servings, recipes.created_at, " +
                                 "ingredients.id, ingredients.recipe_id, ingredients.ingredient_name, ingredients.quantity, ingredients.unit " +
                                 "FROM recipes " +
                                 "LEFT JOIN ingredients ON recipes.id = ingredients.recipe_id " +
                                 "WHERE recipes.id = @id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            
         
                            while (reader.Read())
                            {
                                if (recipe.id == null)
                                {
                                    recipe.id = reader.GetInt32(0).ToString();
                                    recipe.recipe_name = reader.GetString(1);
                                    recipe.recipe_procedure = reader.GetString(2);
                                    recipe.servings = reader.GetInt32(3).ToString();
                                    recipe.created_at = reader.GetDateTime(4).ToString();
                                    recipe.ingredients = new List<Ingredients>();
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

                                    recipe.ingredients.Add(ingredient);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            recipe.id = Request.Form["id"];
            recipe.recipe_name = Request.Form["recipe_name"];
            recipe.servings = Request.Form["servings"];
            recipe.recipe_procedure = Request.Form["recipe_procedure"];

            if (recipe.id.Length == 0 || recipe.recipe_name.Length == 0 || recipe.servings.Length == 0 || recipe.recipe_procedure.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }

            try
            {
                String connectionString = "Data Source=.\\mssqlserver01;Initial Catalog=recipe;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE recipes " +
                                  "SET recipe_name=@recipe_name, recipe_procedure=@recipe_procedure, servings=@servings " +
                                  "WHERE id=@id;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", recipe.id);
                        command.Parameters.AddWithValue("@recipe_name", recipe.recipe_name);
                        command.Parameters.AddWithValue("@recipe_procedure", recipe.recipe_procedure);
                        command.Parameters.AddWithValue("@servings", recipe.servings);

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Recipes/Index");
        }

        public void OnPostIngredients()
        {
            ingredients.recipe_id = Request.Form["id"];
            ingredients.ingredient_name = Request.Form["ingredient_name"];
            ingredients.quantity = Request.Form["quantity"];
            ingredients.unit = Request.Form["unit"];

            if (ingredients.recipe_id.Length == 0 || ingredients.ingredient_name.Length == 0 || ingredients.quantity.Length == 0 || ingredients.unit.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }

            try
            {
                String connectionString = "Data Source=.\\mssqlserver01;Initial Catalog=recipe;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO ingredients " +
                                  "(recipe_id, ingredient_name, quantity, unit) VALUES " +
                                  "(@recipe_id, @ingredient_name, @quantity, @unit);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@recipe_id", ingredients.recipe_id);
                        command.Parameters.AddWithValue("@ingredient_name", ingredients.ingredient_name);
                        command.Parameters.AddWithValue("@quantity", ingredients.quantity);
                        command.Parameters.AddWithValue("@unit", ingredients.unit);

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Recipes/Edit?id=" + ingredients.recipe_id);
        }

        public void OnPostDeleteIngredient(int id)
        {

            try
            {

                String connectionString = "Data Source=.\\mssqlserver01;Initial Catalog=recipe;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "DELETE FROM ingredients WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }

                }
                
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Recipes/");

        }
    }
}
