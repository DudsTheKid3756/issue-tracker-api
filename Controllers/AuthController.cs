using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IssueTracker.Exceptions;
using IssueTracker.Models;
using IssueTracker.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IssueTracker.Controllers;

[Route( $"{Constants.BasePath}/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpGet("user/{username}")]
    public async Task<IActionResult> FindEmail(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null) throw new NotFoundException($"User '{username}' not found");

        return Ok(user.Email);
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> Signin([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password)) return Unauthorized();
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new (ClaimTypes.Name, user.UserName),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var token = GetToken(authClaims);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            userRoles
        });

    }

    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> Signup([FromBody] SignUpModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists is not null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "User already exists!" });

        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        var modelRole = model.Role?.ToLower();

        if (modelRole is not null && modelRole.Equals(UserRoles.RoleAdmin))
        {
            if (!await _roleManager.RoleExistsAsync(UserRoles.RoleAdmin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.RoleAdmin));

            if (await _roleManager.RoleExistsAsync(UserRoles.RoleAdmin))
                await _userManager.AddToRoleAsync(user, UserRoles.RoleAdmin);
        }
        else
        {
            if (!await _roleManager.RoleExistsAsync(UserRoles.RoleUser))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.RoleUser));
            
            if (await _roleManager.RoleExistsAsync(UserRoles.RoleUser))
                await _userManager.AddToRoleAsync(user, UserRoles.RoleUser);
        }

        return Ok(new Response
        {
            Status = "Success", 
            Message = "User created successfully!"
        });
    }

    [HttpPut]
    [Route("reset-pass")]
    public async Task<ActionResult> ResetPassword([FromBody] PassResetModel resetModel)
    {
        var user = await _userManager.FindByNameAsync(resetModel.Username);
        if (user is null) throw new NotFoundException(
            "Error: User with username '" + resetModel.Username + "' not found.");

        if (!resetModel.NewPassword!.Equals(resetModel.RepeatPassword))
            throw new InvalidException("Passwords don't match. ");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, token, resetModel.NewPassword);

        return Ok(new Response
        {
            Status = "Success", 
            Message = "Password for user '" + user.UserName + "' updated!"
        });
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512)
        );

        return token;
    }
}