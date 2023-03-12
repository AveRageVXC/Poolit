﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poolit.Controllers;
using Poolit.Models;
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
        var okResult = await _fileController.Upload(new FormFile(ms, 0, ms.Length, "test", "test"), "name", "desc", "[1]");
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
        var okResult = await _fileController.GetAvailableFiles();
        Assert.IsType<ActionResult<Response>>(okResult);
    }
}