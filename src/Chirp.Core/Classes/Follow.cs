namespace Chirp.Core.Classes;

public class Follow
{
    public int FollowerId { get; set; }
    public Author Follower { get; set; }
        
    public int FollowedId { get; set; }
    public Author Followed { get; set; }
}