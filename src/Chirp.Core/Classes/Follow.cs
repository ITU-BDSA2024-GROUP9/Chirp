namespace Chirp.Core.Classes;

public class Follow
{
	public string? FollowerId { get; set; }
	public Author? Follower { get; set; }

	public string? FollowedId { get; set; }
	public Author? Followed { get; set; }
}