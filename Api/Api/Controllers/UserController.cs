using Api.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HttpClient http = new();
        private readonly string UsersUrl = "https://csharp-task-1-default-rtdb.firebaseio.com/users";
        public UserController(IConfiguration config)
        { 
         
            //Initializing Firebase App
            if (FirebaseApp.DefaultInstance==null) {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(config["Firebase:CredientialsPath"])
                });    
            }
            
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var res= await http.GetStringAsync($"{UsersUrl}.json");
            var DecodedData = JsonConvert.DeserializeObject<Dictionary<string, UserModel>>(res);
            var users = DecodedData;
            return Ok(users);
        }

        // GET api/<UserController>/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get( string userId)
        {  
            var userUrl = $"{UsersUrl}/{userId}"; 

            var res = await http.GetStringAsync($"{userUrl}.json");
            var user = JsonConvert.DeserializeObject<UserModel>(res); // Deserialize directly to UserModel

            if (user != null)
            {
                return Ok(user); // Return the user data if found
            }
            else
            {
                return NotFound(); // Return a 404 Not Found response if the user is not found
            }
        }


        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserModel user)
        {
            var encData= JsonConvert.SerializeObject(user);
            var Content = new StringContent(encData,System.Text.Encoding.UTF8,"application/json");
            var res=await http.PostAsync($"{UsersUrl}.json", Content);

            return Ok(res.StatusCode);



        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UserModel value)
        {
            // Construct the URL for the specific user
            var userUrl = $"{UsersUrl}/{id}.json";
            var res = await http.GetStringAsync(userUrl);
            var user = JsonConvert.DeserializeObject<UserModel>(res); // Deserialize directly to UserModel

            if (user == null)
            {
                return NotFound(); // Return a 404 Not Found response if the user is not found
            }
            UserModel updatedData = new()
            {
                UserName=value.UserName,
                Email= value.Email,
                Password= value.Password,
            };


            var encData = JsonConvert.SerializeObject(updatedData);
            var Content = new StringContent(encData, System.Text.Encoding.UTF8, "application/json");
            var res2 = await http.PostAsync($"{UsersUrl}/{id}.json", Content);

            return Ok(res2.StatusCode); 

        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
           var res  =await http.DeleteAsync($"{UsersUrl}/{id}");
           return Ok(res);
        }
    }
}
