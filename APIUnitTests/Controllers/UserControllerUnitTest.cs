using Microsoft.AspNetCore.Mvc;
using Poolit.Controllers;
using Poolit.Models;
using Poolit.Services;

namespace APIUnitTests.Controllers;

public class UserControllerUnitTest
{
    private readonly UserController _userController = new(null, null);
    private readonly IUserService _userService;

    [Fact]
    public async void Login_ShouldReturnOkResponse()
    {
        var okResult = await _userController.Login("username", "password");
        Assert.IsType<ActionResult<Response>>(okResult);
    }

    [Fact]
    public async void Register_ShouldReturnOkResponse()
    {
        var okResult = await _userController.Register("username", "password");
        Assert.IsType<ActionResult<Response>>(okResult);
    }
}