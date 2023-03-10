using Microsoft.AspNetCore.Mvc;
using Poolit.Models.Requests;
using Poolit.Services.Interfaces;
using Poolit.Models.Responses;
using Poolit.Models;

namespace Poolit.Controllers;

[Route("[controller]")]
[ApiController]
public class FileController : Controller
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;

    public FileController(IFileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// File uploading
    /// </summary>
    /// <param name="file">File to upload.</param>
    /// <param name="id">User's id.</param>
    /// <returns>Url to file</returns>
    [Route("/upload")]
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadFileResponse>> Upload(IFormFile file, ulong id)
    {
        try
        {
            var request = new UploadFileRequest
            {
                File = file,
                Id = id
            };
            var response = await _fileService.UploadAsync(request);
            return response.HasError is false ? Ok(response) : BadRequest(response);
        }
        catch (Exception e)
        {
            var response = new RegisterResponse
            {
                HasError = true,
                Error = e.Message
            };
            return BadRequest(response);
        }
    }
}
