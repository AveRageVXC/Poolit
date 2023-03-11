using Microsoft.AspNetCore.Mvc;
using Poolit.Services.Interfaces;
using Poolit.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;

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
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Upload(IFormFile file, ulong id)
    {
        try
        {
            string path = "./" + file.FileName;
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                await S3Manager.PutObjectAsync(fileStream, "1");
            }

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
        catch (Exception e)
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
    [HttpPost]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Download(ulong id)
    {
        try
        {
            var url = _fileService.GetFileUrlById(id);

            var dataEntry = new DataEntry<string>()
            {
                Data = url,
                Type = "string"
            };

            var response = new Response
            {
                Data = new DataEntry<string>[] { dataEntry }
            };
            return response;
        }
        catch (Exception e)
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
    [HttpPost]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetUserFiles(ulong userId)
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
        catch (Exception e)
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }
}
