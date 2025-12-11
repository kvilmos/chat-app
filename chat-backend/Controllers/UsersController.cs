using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _configuration;

    public UserController(AppDbContext appDbContext, IConfiguration configuration)
    {
        _appDbContext = appDbContext;
        _configuration = configuration;
    }

    [HttpGet(Name = "GetUsers")]
    public async Task<ActionResult<List<UserDTO>>> GetUsers()
    {
        return await _appDbContext.Users
            .Include(u => u.Groups)
            .Select(u => new UserDTO(u))
            .ToListAsync();
    }

    [HttpPost(Name = "CreateUser")]
    public async Task<ActionResult<UserDTO>> CreateUser(NewUserDTO newUser)
    {
        var user = new User(newUser);
        foreach (var groupId in newUser.GroupIds)
        {
            var group = await _appDbContext.Groups
                .AsTracking()
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group != null)
            {
                user.Groups.Add(group);
            }
        }
        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, new UserDTO(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(NewUserDTO loginRequest)
    {
        var user = await _appDbContext.Users
            .AsTracking()
            .SingleOrDefaultAsync(u => u.Name == loginRequest.Name);
        if (user == null || user.PassowordHash == null)
        {
            return BadRequest("User not found");
        }
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PassowordHash, loginRequest.Password)
            == PasswordVerificationResult.Failed)
        {
            return BadRequest("User not found");
        }
        string token = CreateToken(user);

        return Ok(token);
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var token = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
