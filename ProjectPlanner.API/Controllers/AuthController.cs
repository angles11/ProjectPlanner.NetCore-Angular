using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectPlanner.API.Dtos;
using ProjectPlanner.API.Models;
using Microsoft.EntityFrameworkCore.Internal;
using ProjectPlanner.API.Services.PhotoUploader;
using Microsoft.AspNetCore.Authorization;
using ProjectPlanner.API.Services.EmailConfirmation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web;

namespace ProjectPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IPhotoUploader _photoUploader;
        private readonly IEmailSender _emailSender;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration configuration, IMapper mapper, IPhotoUploader photoUploader, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _photoUploader = photoUploader;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Create a user account.
        /// </summary>
        /// <param name="userForRegisterDto"> Dto with the new user's information. </param>
        /// <returns> An IActionResult. </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserForRegisterDto userForRegisterDto)
        {
            // Map user from the dto to model.
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            // Upload profile picture to azure using the Photo Uploader Service.
            userToCreate.PhotoUrl = await _photoUploader.UploadPhotoAsync(userForRegisterDto.Photo);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            if (result.Succeeded)
            {   
                // Get the user after creation. 
                var user = await _userManager.FindByNameAsync(userToCreate.UserName);

                // Map the user to a DTO with the desired returning properties.
                var userToReturn = _mapper.Map<UserForDetailedDto>(user);

                // Generate a token to be send through an Email Sender Service.
                await GenerateAndSendEmailToken(user, userForRegisterDto.Email);

                /*
                  Returns:
                  - The route to get the created user.
                  - The parameters needed to retrieve that user.
                  - The user created mapped into the returning DTO.
                 */
                return CreatedAtRoute("GetUser", new { controller = "User", userId = user.Id }, userToReturn);
            }

            // If the result isn't successful, iterate from each error.
            foreach(var error in result.Errors)
            {
                return BadRequest(error.Description);
            }
          
            return BadRequest("Something happened, try again");
        }

        /// <summary>
        /// Logs a user account.
        /// </summary>
        /// <param name="userForLoginDto"> Dto with the user's username and password.</param>
        /// <returns> An IActionResult. </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login (UserForLoginDto userForLoginDto)
        {
            // Get the user with the username.
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            if (user == null)
                return Unauthorized("User doesn't exist");

            // Attempt a signin using the user and the provided password.
            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);
           

            if (result.Succeeded)
            {
                // Mapped the user to a DTO.
                var userToReturn = _mapper.Map<UserForDetailedDto>(user);

                // Return an Ok with the user and a JwtToken to be used as an authentication token.
                return Ok(new
                {
                    token = GenerateJwtToken(user),
                    user = userToReturn
                });
            }

            else
            {
                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                if (!isEmailConfirmed)
                    return Unauthorized("You have to confirm your email");

                return Unauthorized("Wrong password");
            }


        }

        /// <summary>
        /// Generates the Jason Web Token for authenticating the user.
        /// </summary>
        /// <param name="user"> Entity of the user. </param>
        /// <returns></returns>
        private string GenerateJwtToken (User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration["AppSettings:Token"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates the token for account confirmation and Send it through the Email Service.
        /// </summary>
        /// <param name="user"> Entity of the User. </param>
        /// <param name="email"> User's email </param>
        /// <returns> </returns>
        private async Task GenerateAndSendEmailToken(User user, string email)
        {
            // Generate the confirmation token.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encode it to be sent through the email Service.
            var encodedToken = HttpUtility.UrlEncode(token);

            // CallBack to the SPA with the token and email.
            // The SPA will make a call to the ConfirmEmail method, passing the two parameters.
            var callBackUrl = "http://localhost:5000/#/register/ConfirmEmail?token=" + encodedToken + "&email=" + email;

            // Send the email. 
            await _emailSender.SendEmailAsync(user.Email, "Confirm your account", $"<b>Please click in the link below to confirm your account</b> <br> <a href='{callBackUrl}'>Confirm Now</a>");
        }

        /// <summary>
        /// Confirms the newly created account.
        /// </summary>
        /// <param name="emailConfirmationDto"> Dto containing the user's email and the confirmation token. </param>
        /// <returns> An IActionResult. </returns>
        [HttpPost("confirm")]
        [AllowAnonymous]

        public async Task<IActionResult> ConfirmEmail(EmailConfirmationDto emailConfirmationDto)
       {
            var user = await _userManager.FindByEmailAsync(emailConfirmationDto.Email);

            if (user == null)
                return Unauthorized();

            // Check if the user has already confirmed the account.
           if(await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("You have already confirmed your account");
            }

           // Check if the token hasn't expired.
           // If expired, send a new one.
            if (!await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", emailConfirmationDto.Token))
            {
                await GenerateAndSendEmailToken(user, emailConfirmationDto.Email);

                return Unauthorized("The token has expired, please check your email for the new confirmation link");
            }

            // Confirm the email with the user and a token sent through the email and 
            // Recieved  through the SPA.
            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationDto.Token);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return Unauthorized("Something happened");
        }

        /// <summary>
        /// Starts the process to recover the user's password.
        /// </summary>
        /// <param name="email"> Email of the user.</param>
        /// <returns>An IActionResult.</returns>
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            // Get the user from provided email.
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound("There is no user associated with this email.");

            // Check if user's account is confirmed. 
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Your have to confirm your account first.");

            // Generates the token and send it through the email Service.
            await GenerateAndSendPasswordToken(user, email);

            return NoContent();
        }

        /// <summary>
        /// Generates the reset password token and send an email with the callback URL
        /// </summary>
        /// <param name="user"> User's entity. </param>
        /// <param name="email"> User's email. </param>
        /// <returns></returns>
        public async Task GenerateAndSendPasswordToken(User user, string email)
        {
            // Generate the password reset token.
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Enconde it. 
            var encodedToken = HttpUtility.UrlEncode(token);

            // Generate the callback URL with the encodedToken and the user's email as query parameters.
            var callBackUrl = "http://localhost:5000/#/login/RecoverPassword?token=" + encodedToken + "&email=" + email;

            // Send the email. 
            await _emailSender.SendEmailAsync(user.Email, "Reset your password", $"<b>Please click in the link below to reset your password</b> <br> <a href='{callBackUrl}'>Confirm Now</a>");
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        /// <param name="passwordToRecoverDto"> Dto containing: user's email, reset token and new password.</param>
        /// <returns> An IActionResult. </returns>
        [HttpPatch("resetPassword")]
        public async Task<IActionResult> ResetPassword(PasswordToRecoverDto passwordToRecoverDto)
        {
            var user = await _userManager.FindByEmailAsync(passwordToRecoverDto.Email);

            if (user == null)
                return NotFound();

            // Check if the token hasn't expired and in that case, resend a new one.
            if (!await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", passwordToRecoverDto.Token))
            {
                await GenerateAndSendPasswordToken(user, passwordToRecoverDto.Email);

                return Unauthorized("The token has expired, please check your email for the new link");
            }

            // Reset the user's password with the new password. 
            var result = await _userManager.ResetPasswordAsync(user, passwordToRecoverDto.Token, passwordToRecoverDto.Password);

            if (result.Succeeded)
                return NoContent();

            return BadRequest("Something happened");
        }
    }
}