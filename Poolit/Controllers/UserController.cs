using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poolit.Models;
using Poolit.Services;

namespace Poolit.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Route("/register")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Register(string username, string password)
    {
        var response = new Response();
        try
        {
            var user = new User { Username = username };

            // CanSave == don't have users's username in db
            if (_userService.CanSave(user) is false)
            {
                response.Error = "User with this name already exists";
                return BadRequest(response);
            }

            _userService.AssignPasswordHash(user, password);

            _userService.SaveUser(user);

            Response.Headers.Add("token", _userService.CreateToken(user));

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            response.Data = new [] { dataEntry };

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            throw ex;
            return BadRequest(response);
        }
    }

    [Route("/login")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Login(string username, string password)
    {
        var response = new Response();
        try
        {
            var user = new User { Username = username };

            // CanSave user = user doesn't exists
            if (_userService.CanSave(user))
            {
                response.Error = "Wrong username or password.";
                return BadRequest(response);
            }
            
            user = _userService.GetUserByUsername(username);

            if (_userService.VerifyPassword(user, password) is false)
            {
                response.Error = "Wrong username or password.";
                return BadRequest(response);
            }

            Response.Headers.Add("token", _userService.CreateToken(user));

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            response.Data = new [] { dataEntry };
            
            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }

    [Route("/get-user-by-username")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetUserByUsername(string username)
    {
        var response = new Response();
        try
        {
            var user = new User { Username = username };

            // CanSave user = user doesn't exist
            if (_userService.CanSave(user))
            {
                response.Error = "No user with this user name";
                return BadRequest(response);
            }

            user = _userService.GetUserByUsername(username);

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            response.Data = new [] { dataEntry };
            
            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }
}
