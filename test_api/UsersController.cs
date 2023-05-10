using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace test_api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.UserGroup).Include(u => u.UserState).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.UserGroup).Include(u => u.UserState).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Login == user.Login))
            {
                return Conflict("User with this login already exists.");
            }

            if (user.UserGroup.Code == "Admin")
            {
                return BadRequest("Cannot create user with Admin group.");
            }

            user.CreatedDate = DateTime.UtcNow;
            user.UserStateId = 1;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.UserStateId = 2;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
