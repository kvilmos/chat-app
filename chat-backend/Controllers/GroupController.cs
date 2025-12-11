using System.Security.Claims;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpGet(Name = "GetGroups")]
    public Task<List<GroupDTO>> GetGroups()
    {
        return _appDbContext.Groups
            .Include(g => g.Creator)
            .Include(g => g.Members)
            .Select(g => new GroupDTO(g))
            .ToListAsync();
    }

    [Authorize]
    [HttpPost(Name = "CreateGroup")]
    public async Task<ActionResult<GroupDTO>> CreateGroup(NewGroupDTO newGroup)
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var creatorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var creatorId = !string.IsNullOrEmpty(creatorIdStr) ? int.Parse(creatorIdStr) : 0;
        if (creatorId == 0)
        {
            return Unauthorized();
        }

        var group = new Group(newGroup);
        group.CreatorId = creatorId;

        newGroup.UserIds.Add(creatorId);
        foreach (var userId in newGroup.UserIds)
        {
            var user = await _appDbContext.Users
                .AsTracking()
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                group.Members.Add(user);
            }
        }

        _appDbContext.Groups.Add(group);
        await _appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, new GroupDTO(group));
    }

    [Authorize]
    [HttpPost("{groupId}/join")]
    public async Task<ActionResult<GroupDTO>> JoinGroups(int groupId)
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = !string.IsNullOrEmpty(userIdStr) ? int.Parse(userIdStr) : 0;
        if (userId == 0)
        {
            return Unauthorized();
        }

        var alreadyMember = await _appDbContext.GroupUserJoins
            .AsTracking()
            .SingleOrDefaultAsync(gu => gu.UserId == userId && gu.GroupId == groupId);
        if (alreadyMember != null)
        {
            return Conflict("Already Joined");
        }
        var join = new GroupUserJoin
        {
            UserId = userId,
            GroupId = groupId,
        };
        _appDbContext.GroupUserJoins.Add(join);
        await _appDbContext.SaveChangesAsync();

        return Ok();
    }
}
