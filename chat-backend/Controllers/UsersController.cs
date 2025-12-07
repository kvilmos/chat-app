using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public UserController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet(Name = "GetUsers")]
    public Task<List<UserDTO>> GetUsers()
    {
        return _appDbContext.Users
            .Include(t => t.Groups)
            .Select(p => new UserDTO(p))
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
                .SingleOrDefaultAsync(t => t.Id == groupId);
            if (group != null)
            {
                user.Groups.Add(group);
            }
        }
        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, new UserDTO(user));
    }

}