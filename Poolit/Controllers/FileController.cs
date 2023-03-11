using Microsoft.AspNetCore.Mvc;
using Poolit.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;
using Poolit.Services;
using Microsoft.AspNetCore.Authorization;

namespace Poolit.Controllers;

[Route("[controller]")]
[ApiController]
public class FileController : Controller
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;
    private readonly IS3Manager S3Manager;

    public FileController(IFileService fileService, ILogger<FileController> logger, IS3Manager s3Manager)
    {
        _fileService = fileService;
        _logger = logger;
        S3Manager = s3Manager;
    }

    /// <summary>
    /// File uploading
    /// </summary>
    /// <param name="file">File to upload.</param>
    /// <param name="id">User's id.</param>
    /// <returns>Url to file</returns>
    [Route("/upload")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Upload(IFormFile file, int id)
    {
        try
        {
            using var ms = new MemoryStream();
            if (file.Length > 0)
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
            }
            var path = $"id";
            await S3Manager.PutObjectAsync(ms, "1");

            var dataEntry = new DataEntry<string>()
            {
                Data = path,
                Type = "string"
            };

            var response = new Response
            {
                Data = new DataEntry<string>[] { dataEntry }
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
    /// <param name="id">File's id</param>
    /// <returns>Url to file</returns>
    [Route("/download")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Download(int id)
    {
        try
        {
            var url = _fileService.GetFileUrlById(id);
            var S3Object = await S3Manager.GetObjectAsync($"{id}");

            var stream = S3Object.ResponseStream;
            var contentType = S3Object.Headers.ContentType;
            var fileName = $"{id}.pdf";

            var dataEntry = new DataEntry<string>()
            {
                Data = url,
                Type = "string"
            };

            var response = new Response
            {
                Data = new DataEntry<string>[] { dataEntry }
            };
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
    public async Task<ActionResult<Response>> GetUserFiles(int userId)
    {
        try
        {
            var userFiles = _fileService.GetUserFiles(userId);

            var dataEntry = new DataEntry<UserFile[]>()
            {
                Data = userFiles,
                Type = "filearray"
            };

            var response = new Response
            {
                Data = new DataEntry<UserFile[]>[] { dataEntry }
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
