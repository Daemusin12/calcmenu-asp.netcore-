using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace RecipeApp.Pages.Recipes
{
    public class EditModel : PageModel
    {
        public Recipe recipe = new Recipe();
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
                    String sql = "SELECT * FROM recipes WHERE id=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                recipe.id = reader.GetInt32(0).ToString();
                                recipe.recipe_name = reader.GetString(1);
                                recipe.recipe_procedure = reader.GetString(2);
                                recipe.servings = reader.GetInt32(3).ToString();
                                recipe.created_at = reader.GetDateTime(4).ToString();

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
    }
}
