using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Backend.Entites;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{

    /// <summary>
    /// UserController handles user registration, login, and password management
    /// </summary>
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        /// <summary>
        /// Registers a new user with the specified registration details
        /// </summary>
        /// <param name="model">The registration information for the new user</param>
        /// <returns> result of the registration operation</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Register model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newUser = new User{ Email = model.Email, UserName = model.Name };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                return Ok("User registered");
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Authenticates a user with the provided credentials and returns a JWT token if authentication is successful
        /// </summary>
        /// <param name="model">The login credentials submitted by the user</param>
        /// <returns> JWT token and user email if authentication succeeds
        ///  NotFound result if the user does not exist, a BadRequest result if the model state is invalid, or an Unauthorized result if authentication fails.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] Login model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound("User not found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                var token = JwtService.GenerateJwtToken(user, _config);

                return Ok(new
                {
                    token = token,
                    email = user.Email
                });
            }
            return Unauthorized();
        }

        /// <summary>
        /// Changes the password for a user identified by the specified email address.
        /// </summary>
        /// <param name="model">An object containing the user's email address and the new password to set</param>
        /// <returns>result of the password change operation</returns>
        [HttpPost("change_password")]
        public async Task<IActionResult> ChangePasswordUser([FromBody] ChangePassword model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            result = await _userManager.AddPasswordAsync(user, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors );
            
             return Ok("Password changed");

        }
    }
}
