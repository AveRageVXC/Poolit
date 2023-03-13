using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Poolit.Models;
using Poolit.Models.Requests;
using Poolit.Services;

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
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Upload([FromBody] UploadRequest request)
    {
        var response = new Response();
        try
        {
            var accessEnabledUserIds = request.AccessEnabledUserIds;
            var formFile = request.FormFile;
            var name = request.Name.Trim();
            if (name.Length > 128)
            {
                response.Error = "Length of the file name can't be more than 128 symbols";
                return BadRequest(response);
            }
            var description = request.Description.Trim();
            using var ms = new MemoryStream();
            if (formFile.Length > 0)
            {
                formFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                var s = Convert.ToBase64String(fileBytes);
            }

            var userId = request.UserId;
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

            _fileService.SaveFile(newFile, accessEnabledUserIds);

            await S3Manager.PutFileAsync(ms, newFile.S3Key);

            var dataEntry = new DataEntry<FileEntity>()
            {
                Type = "file",
                Data = newFile
            };
            response.Data = new[] { dataEntry };

            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }

    [Route("upload-file")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> UploadFile(IFormFile file, string allowedUsersIds, int userId)
    {
        var response = new Response();
        try
        {
            var usersIds = JsonConvert.DeserializeObject<List<int>>(allowedUsersIds);
            var formFile = file;
            var name = file.FileName.Trim();
            if (name.Length > 128)
            {
                response.Error = "Length of the file name can't be more than 128 symbols";
                return BadRequest(response);
            }
            var description = "";
            using var ms = new MemoryStream();
            if (formFile.Length > 0)
            {
                formFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                var s = Convert.ToBase64String(fileBytes);
            }

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

            _fileService.SaveFile(newFile, usersIds);

            await S3Manager.PutFileAsync(ms, newFile.S3Key);

            var dataEntry = new DataEntry<FileEntity>()
            {
                Type = "file",
                Data = newFile
            };
            response.Data = new[] { dataEntry };

            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }

    [Route("download/{poolitKey}")]
    [HttpGet]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Download(string poolitKey)
    {
        var response = new Response();
        try
        {
            /*var userFiles = _fileService.GetAvailableFiles(userId);
            if (userFiles.Any(f => f.PoolitKey == poolitKey) is false)
            {
                response.Error = "You don't have acces to this file or it doesn't exist";
                return BadRequest(response);
            }*/

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
    [HttpPost]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetAvailableFiles(GetAvailableFilesRequest request)
    {
        var response = new Response();
        try
        {
            var userId = request.UserId;
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
