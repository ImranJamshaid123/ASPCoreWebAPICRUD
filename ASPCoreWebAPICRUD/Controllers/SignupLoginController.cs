 using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASPCoreWebAPICRUD.Models;
using Microsoft.Data.SqlClient;

namespace ASPCoreWebAPICRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SignupLoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult signup(Signup signup)
        {
            string connectionString = _configuration.GetConnectionString("MyCollege");
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string checkQuery = "SELECT COUNT(*) FROM Signup WHERE UserName = @UserName OR Email = @Email";
            SqlCommand checkCmd = new SqlCommand(checkQuery, con);

            checkCmd.Parameters.AddWithValue("@UserName", signup.UserName);
            checkCmd.Parameters.AddWithValue("@Email", signup.Email);

            int userExists = (int)checkCmd.ExecuteScalar();

            if (userExists > 0)
            {
                return BadRequest(new Response { statusCode = 400, statusMessage = "UserName or Email already exists!" });
            }

            string query = "INSERT INTO Signup (ID,UserName, FatherName, Email, Password) VALUES (@ID, @UserName, @FatherName, @Email, @Password)";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@ID", signup.ID);
            cmd.Parameters.AddWithValue("@UserName", signup.UserName);
            cmd.Parameters.AddWithValue("@FatherName", signup.FatherName);
            cmd.Parameters.AddWithValue("@Email", signup.Email);
            cmd.Parameters.AddWithValue("@Password", signup.Password);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return StatusCode(201, new Response { statusCode = 201, statusMessage = "Signup successful!" });
            }
            else
            {
                return BadRequest(new Response { statusCode = 400, statusMessage = "Signup failed!" });
            }
        }

        [HttpPost]
        [Route("login")]

        public IActionResult Login(Login login)
        {
            string connectionString = _configuration.GetConnectionString("MyCollege");

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string query = "SELECT COUNT(*) FROM Signup WHERE UserName = @UserName AND Password = @Password";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@UserName", login.UserName);
            cmd.Parameters.AddWithValue("@Password", login.Password);

            int userExists = (int)cmd.ExecuteScalar();

            if (userExists > 0)
            { 
                return Ok(new Response { statusCode = 200, statusMessage = "Login successful!" });
            }
            else
            {
                return Unauthorized(new Response { statusCode = 401, statusMessage = "Invalid username or password!" });
            }
        }
    }
}
