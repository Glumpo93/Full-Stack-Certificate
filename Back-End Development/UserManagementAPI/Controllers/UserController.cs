using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        try
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 500);

            var all = await _userRepository.GetAllAsync(ct);
            var total = all.Count;
            var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            Response.Headers["X-Total-Count"] = total.ToString();
            return Ok(paged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetUsers");
            var problem = new ProblemDetails { Title = "Unexpected error", Status = StatusCodes.Status500InternalServerError };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetUser(int id, CancellationToken ct = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                return NotFound(new ProblemDetails { Title = "User not found", Detail = $"User with id {id} does not exist.", Status = StatusCodes.Status404NotFound });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetUser {UserId}", id);
            var problem = new ProblemDetails { Title = "Unexpected error", Status = StatusCodes.Status500InternalServerError };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // trim and normalize
            var name = createUserDto.Name?.Trim();
            var email = createUserDto.Email?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                return BadRequest(new ProblemDetails { Title = "Validation error", Detail = "Name and Email are required.", Status = StatusCodes.Status400BadRequest });

            var user = new User { Name = name, Email = email };
            var created = await _userRepository.CreateAsync(user, ct);
            return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation conflict creating user");
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CreateUser");
            var problem = new ProblemDetails { Title = "Unexpected error", Status = StatusCodes.Status500InternalServerError };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var existing = await _userRepository.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound(new ProblemDetails { Title = "User not found", Status = StatusCodes.Status404NotFound });

            var name = updateUserDto.Name?.Trim();
            var email = updateUserDto.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                return BadRequest(new ProblemDetails { Title = "Validation error", Detail = "Name and Email are required.", Status = StatusCodes.Status400BadRequest });

            existing.Name = name;
            existing.Email = email;

            var updated = await _userRepository.UpdateAsync(existing, ct);
            if (updated == null)
                return NotFound(new ProblemDetails { Title = "User not found", Status = StatusCodes.Status404NotFound });

            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation conflict updating user {UserId}", id);
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in UpdateUser {UserId}", id);
            var problem = new ProblemDetails { Title = "Unexpected error", Status = StatusCodes.Status500InternalServerError };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _userRepository.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound(new ProblemDetails { Title = "User not found", Status = StatusCodes.Status404NotFound });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in DeleteUser {UserId}", id);
            var problem = new ProblemDetails { Title = "Unexpected error", Status = StatusCodes.Status500InternalServerError };
            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }
}