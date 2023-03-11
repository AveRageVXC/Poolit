using Microsoft.AspNetCore.Mvc;
using Poolit.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;
using Poolit.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Poolit.Controllers;

[Route("[controller]")]
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

    /// <summary>
    /// File uploading
    /// </summary>
    /// <param name="formFile">File to upload.</param>
    /// <param name="accessEnabledUserIds">Users' ids that have access to the file.</param>
    /// <returns>Url to file</returns>
    [Route("/upload")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Upload(IFormFile formFile, List<int> accessEnabledUserIds)
    {
        try
        {
            using var ms = new MemoryStream();
            if (formFile.Length > 0)
            {
                formFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
            }

            var newFile = new FileEntity { };
            var userId = _userService.GetIdFromToken(Response.Headers["token"]);

            _fileService.AddFile(newFile, userId, accessEnabledUserIds);

            var path = $"id";
            await S3Manager.PutFileAsync(ms, newFile.S3Key);

            var dataEntry = new DataEntry<FileEntity>()
            {
                Type = "file",
                Data = newFile
            };

            var response = new Response
            {
                Data = new DataEntry<FileEntity>[] { dataEntry }
            };

            return response;
        }
        catch (Exception)
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }

    }

    /// <summary>
    /// File downloading
    /// </summary>
    /// <param name="poolitKey">Poolit key</param>
    /// <returns>File</returns>
    [Route("/download/{poolitKey}")]
    [HttpGet, Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Download(string poolitKey)
    {
        try
        {
            var file = _fileService.GetFileByPoolitKey(poolitKey);

            var S3Object = await S3Manager.GetFileAsync(file.S3Key);

            var stream = S3Object.ResponseStream;
            var contentType = S3Object.Headers.ContentType;
            var fileName = file.Name;

            return File(stream, contentType, fileName);
        }
        catch (Exception)
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }

    /// <summary>
    /// Getting user's files
    /// </summary>
    /// <param name="userId">user's id</param>
    /// <returns>List of user's files</returns>
    [Route("/getuserfiles")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetAvailableFiles(int userId)
    {
        try
        {
            var userFiles = _fileService.GetAvailableFiles(userId);

            var dataEntry = new DataEntry<FileEntity[]>()
            {
                Data = userFiles,
                Type = "filearray"
            };

            var response = new Response
            {
                Data = new DataEntry<FileEntity[]>[] { dataEntry }
            };
            return response;
        }
        catch (Exception)
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }
}
