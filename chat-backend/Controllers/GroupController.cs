using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public GroupController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet(Name = "GetGroups")]
    public Task<List<GroupDTO>> GetGroups()
    {
        return _appDbContext.Groups
            .Include(t => t.Creator)
            .Include(t => t.Members)
            .Select(p => new GroupDTO(p))
            .ToListAsync();
    }

    [HttpPost(Name = "CreateGroup")]
    public async Task<ActionResult<GroupDTO>> CreateGroup(NewGroupDTO newGroup)
    {
        var group = new Group(newGroup);
        var creator = await _appDbContext.Users
            .AsTracking()
            .SingleOrDefaultAsync(t => t.Id == newGroup.CreatorId);
        if (creator != null)
        {
            group.Creator = creator;
        }

        newGroup.UserIds.Add(newGroup.CreatorId);
        foreach (var userId in newGroup.UserIds)
        {
            var user = await _appDbContext.Users
                .AsTracking()
                .SingleOrDefaultAsync(t => t.Id == userId);
            if (user != null)
            {
                group.Members.Add(user);
            }
        }
        _appDbContext.Groups.Add(group);
        await _appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, new GroupDTO(group));
    }
}
