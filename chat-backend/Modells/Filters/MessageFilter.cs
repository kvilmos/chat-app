using Microsoft.AspNetCore.Mvc;

public class MessageFilter
{
    [FromQuery(Name = "groupId")]
    public int GroupId { get; set; }

    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "latest")]
    public bool? Latest { get; set; } = true;
}
