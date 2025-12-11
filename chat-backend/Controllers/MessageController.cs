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
    const int PAGE_NUMBER = 10;
    private readonly AppDbContext _appDbContext;

    public MessageController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [Authorize]
    [HttpGet(Name = "GetMessages")]
    public async Task<ActionResult<List<MessageDTO>>> GetMessages([FromQuery] MessageFilter filter)
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = !string.IsNullOrEmpty(userIdStr) ? int.Parse(userIdStr) : 0;
        if (userId == 0)
        {
            return BadRequest();
        }

        return await _appDbContext.Messages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Include(m => m.Group)
            .Where(m => m.GroupId == filter.GroupId && m.SenderId == userId)
            .OrderByDescending(m => m.Date)
            .Skip((filter.Page - 1) * PAGE_NUMBER)
            .Take(10)
            .Select(m => new MessageDTO(m))
            .ToListAsync();
    }

    [Authorize]
    [HttpPost(Name = "SendMessage")]
    public async Task<ActionResult<UserDTO>> SendMessage(NewMessageDTO newMessage)
    {
        var message = new Message(newMessage);
        _appDbContext.Messages.Add(message);
        await _appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, new MessageDTO(message));
    }
}
