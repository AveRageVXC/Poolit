using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Poolit.Models;
using Poolit.Services;
using System.Collections;

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
    public async Task<ActionResult<Response>> Upload(IFormFile formFile, string accessEnabledUserIds)
    {
        try
        {
            var ids = JsonConvert.DeserializeObject<List<int>>(accessEnabledUserIds);
            using var ms = new MemoryStream();
            if (formFile.Length > 0)
            {
                formFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
            }

            Request.Headers.TryGetValue("Authorization", out var tokenHeader);
            var token = tokenHeader[0].Split(' ')[1];
            var userId = _userService.GetIdFromToken(token);
            var newFile = new FileEntity
            {
                Name = formFile.FileName,
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
            using var ms = new MemoryStream();

            stream.CopyTo(ms);
            return File(ms.ToArray(), file.ContentType, file.Name);
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
    [Route("/get-user-files")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetAvailableFiles(int userId)
    {
        try
        {
            var userFiles = _fileService.GetAvailableFiles(userId);

            var files = userFiles.Select(f => new DataEntry<FileEntity> { Type = "file", Data = f }).ToArray();

            var response = new Response
            {
                Data = files
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
