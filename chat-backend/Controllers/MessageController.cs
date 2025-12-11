using System.Security.Claims;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public MessageController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [Authorize]
    [HttpGet(Name = "GetMessages")]
    public async Task<ActionResult<List<MessageDTO>>> GetMessages()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = !string.IsNullOrEmpty(userIdStr) ? int.Parse(userIdStr) : 0;
        if (userId == 0)
        {
            return BadRequest();
        }

        var groupIdStr = Request.Query["groupId"].ToString();
        var groupId = !string.IsNullOrEmpty(groupIdStr) ? int.Parse(groupIdStr) : 0;
        var group = await _appDbContext.GroupUserJoins
            .AsTracking()
            .SingleOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);
        if (group == null)
        {
            return Unauthorized();
        }

        var pageStr = Request.Query["page"].ToString();
        var page = !string.IsNullOrEmpty(pageStr) ? int.Parse(pageStr) : 1;

        return await _appDbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Group)
            .Where(m => m.GroupId == groupId)
            .Select(m => new MessageDTO(m))
            .Skip(10 * (page - 1))
            .Take(10 * page)
            .ToListAsync();
    }

    [Authorize]
    [HttpPost(Name = "SendMessage")]
    public async Task<ActionResult<UserDTO>> SendMessage(NewMessageDTO newMessage)
    {
        var message = new Message(newMessage);
        var sender = await _appDbContext.Users
            .AsTracking()
            .SingleOrDefaultAsync(m => m.Id == newMessage.SenderId);
        if (sender != null)
        {
            message.Sender = sender;
        }
        var group = await _appDbContext.Groups
            .AsTracking()
            .SingleOrDefaultAsync(m => m.Id == newMessage.GroupId);
        if (group != null)
        {
            message.Group = group;
        }

        _appDbContext.Messages.Add(message);
        await _appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, new MessageDTO(message));
    }
}
