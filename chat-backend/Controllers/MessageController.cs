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
    public Task<List<MessageDTO>> GetMessages()
    {
        return _appDbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Group)
            .Select(m => new MessageDTO(m))
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
