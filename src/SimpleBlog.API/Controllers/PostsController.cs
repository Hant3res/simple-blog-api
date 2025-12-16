using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.API.Data;
using SimpleBlog.API.Models;

namespace SimpleBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly BlogContext _context;

    public PostsController(BlogContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetPosts()
    {
        return await _context.Posts.Include(p => p.Comments).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetPost(int id)
    {
        var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return NotFound();

        return post;
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost(Post post)
    {
        post.CreatedAt = DateTime.Now;
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpPost("{postId}/comments")]
    public async Task<ActionResult<Comment>> AddComment(int postId, Comment comment)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            return NotFound();
            
        comment.PostId = postId;
        comment.CreatedAt = DateTime.Now;
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }
}
