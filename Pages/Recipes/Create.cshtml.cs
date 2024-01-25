using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace RecipeApp.Pages.Recipes
{
    public class CreateModel : PageModel
    {
        public Recipe recipe = new Recipe();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
        }

        public void OnPost() 
        {
            recipe.recipe_name = Request.Form["recipe_name"];
            recipe.servings = Request.Form["servings"];
            recipe.recipe_procedure = Request.Form["recipe_procedure"];

            if (recipe.recipe_name.Length == 0 || recipe.servings.Length == 0 || recipe.recipe_procedure.Length == 0) 
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
                    String sql = "INSERT INTO recipes " +
                                  "(recipe_name, recipe_procedure, servings) VALUES " +
                                  "(@recipe_name, @recipe_procedure, @servings);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
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

            recipe.recipe_name = "";
            recipe.servings = "";
            recipe.recipe_procedure = "";

            successMessage = "New Client Added Correctly";

            Response.Redirect("/Recipes/Index");
        }
    }
}
