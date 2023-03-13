using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Poolit.Models;
using Poolit.Services;
using System.Collections;

namespace Poolit.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : Controller
{
    private readonly IFileService _fileService;
    private readonly IUserService _userService;
    private readonly ILogger<FileController> _logger;
    private readonly IS3Manager S3Manager;

    public FileController(IFileService fileService, IUserService userService, ILogger<FileController> logger, IS3Manager s3Manager)
    {
        _fileService = fileService;
        _userService = userService;
        _logger = logger;
        S3Manager = s3Manager;
    }

    [Route("upload")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Upload(IFormFile formFile, string name, string description, string accessEnabledUserIds = "[]")
    {
        var response = new Response();
        try
        {
            var ids = JsonConvert.DeserializeObject<List<int>>(accessEnabledUserIds);
            using var ms = new MemoryStream();
            if (formFile.Length > 0)
            {
                formFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                var s = Convert.ToBase64String(fileBytes);
            }

            Request.Headers.TryGetValue("Authorization", out var tokenHeader);
            var token = tokenHeader[0].Split(' ')[1];
            var userId = _userService.GetIdFromToken(token);
            var newFile = new FileEntity
            {
                Name = name,
                RealFileName = formFile.FileName,
                Description = description,
                ContentType = formFile.ContentType,
                CreationDate = DateTime.Now,
                Size = (int)ms.Length,
                OwnerId = userId
            };

            _fileService.SaveFile(newFile, ids);

            await S3Manager.PutFileAsync(ms, newFile.S3Key);

            var dataEntry = new DataEntry<FileEntity>()
            {
                Type = "file",
                Data = newFile
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

    [Route("download/{poolitKey}")]
    [HttpGet, Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Download(string poolitKey)
    {
        var response = new Response();
        try
        {
            Request.Headers.TryGetValue("Authorization", out var tokenHeader);
            var token = tokenHeader[0].Split(' ')[1];
            var userId = _userService.GetIdFromToken(token);
            var userFiles = _fileService.GetAvailableFiles(userId);
            if (userFiles.Any(f => f.PoolitKey == poolitKey) is false)
            {
                response.Error = "You don't have acces to this file or it doesn't exist";
                return BadRequest(response);
            }

            var file = _fileService.GetFileByPoolitKey(poolitKey);
            var S3Object = await S3Manager.GetFileAsync(file.S3Key);
            var stream = S3Object.ResponseStream;
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            return File(ms.ToArray(), file.ContentType, file.RealFileName);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }

    [Route("get-available-files")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetAvailableFiles()
    {
        var response = new Response();
        try
        {
            Request.Headers.TryGetValue("Authorization", out var tokenHeader);
            var token = tokenHeader[0].Split(' ')[1];
            var userId = _userService.GetIdFromToken(token);
            var userFiles = _fileService.GetAvailableFiles(userId);

            var files = userFiles.Select(f => new DataEntry<FileEntity> { Type = "file", Data = f }).ToArray();
            response.Data = files;

            return response;
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }
}
