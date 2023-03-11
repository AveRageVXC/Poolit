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
    /// <param name="formFile">File to upload.</param>
    /// <param name="id">User's id.</param>
    /// <returns>Url to file</returns>
    [Route("/upload")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Upload(IFormFile formFile, ...)
    {
        try
        {
            // using var ms = new MemoryStream();
            // if (formFile.Length > 0)
            // {
            //     formFile.CopyTo(ms);
            //     var fileBytes = ms.ToArray();
            //     string s = Convert.ToBase64String(fileBytes);
            // }

            var newFile = new FileEntity {};
            _fileService.AddFile()

            var path = $"id";
            // await S3Manager.PutObjectAsync(ms, "1");
            await S3Manager.PutObjectAsync(formFile, newFile.S3Key);

            var dataEntry = new DataEntry<string>()
            {
                Type = "file",
                Data = newFile
            };
            // var dataEntry = new DataEntry<string>()
            // {
            //     Type = "string"
            //     Data = path,
            // };

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
    public async Task<ActionResult<Response>> Download(string poolitKey)
    {
        try
        {
            var file = _fileService.GetFileByPoolitKey(poolitKey);


            // ---
            var S3Object = await S3Manager.GetObjectAsync(file.S3Key.ToString());

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
