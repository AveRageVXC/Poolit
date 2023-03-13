using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poolit.Controllers;
using Poolit.Models;
using Poolit.Models.Requests;
using Poolit.Services;

namespace APIUnitTests.Controllers;

public class FileControllerUnitTests
{
    private readonly FileController _fileController = new(null, null, null, null);
    private readonly IFileService userService;

    [Fact]
    public async void Upload_ShouldReturnOkResponse()
    {
        var ms = new MemoryStream();
        var request = new UploadRequest { FormFile = new FormFile(ms, 0, ms.Length, "test", "test"), Name = "name", Description = "desc", AccessEnabledUserIds = new List<int>() { 1 }, UserId = 1 };
        var okResult = await _fileController.Upload(request);
        Assert.IsType<ActionResult<Response>>(okResult);
    }

    [Fact]
    public async void Download_ShouldReturnOkResponse()
    {
        var okResult = await _fileController.Download("test");
        Assert.IsType<ActionResult<Response>>(okResult);
    }

    [Fact]
    public async void GetAvailableFiles_ShouldReturnOkResponse()
    {
        var request = new GetAvailableFilesRequest() { UserId = 1 };
        var okResult = await _fileController.GetAvailableFiles(request);
        Assert.IsType<ActionResult<Response>>(okResult);
    }
}