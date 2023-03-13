using Microsoft.AspNetCore.Mvc;
using Poolit.Controllers;
using Poolit.Models;
using Poolit.Models.Requests;
using Poolit.Services;

namespace APIUnitTests.Controllers;

public class UserControllerUnitTest
{
    private readonly UserController _userController = new(null, null);
    private readonly IUserService _userService;

    [Fact]
    public async void Login_ShouldReturnOkResponse()
    {
        var request = new LoginRequest { UserName = "username", Password = "password" };
        var okResult = await _userController.Login(request);
        Assert.IsType<ActionResult<Response>>(okResult);
    }

    [Fact]
    public async void Register_ShouldReturnOkResponse()
    {
        var request = new RegisterRequest { UserName = "username", Password = "password" };
        var okResult = await _userController.Register(request);
        Assert.IsType<ActionResult<Response>>(okResult);
    }
}