using Dapper;
using Poolit.Handlers;
using Poolit.Models;

namespace Poolit.Repositories;

public class FileRepo : IFileRepo
{
    public bool ValidS3Key(string S3Key)
    {
        var counter = DBConnectionHandler.Connection.ExecuteScalar<int>(@"
        SELECT COUNT(*) FROM ""File""
        WHERE s3_key=@S3Key;
        ",
        new { S3Key });

        return counter == 0;
    }

    public bool ValidPoolitKey(string PoolitKey)
    {
        var counter = DBConnectionHandler.Connection.ExecuteScalar<int>(@"
        SELECT COUNT(*) FROM ""File""
        WHERE poolit_key=@PoolitKey;
        ",
        new { PoolitKey });

        return counter == 0;
    }

    public void SaveFile(FileEntity file, List<int> accessEnabledUserIds)
    {
        using var connection = DBConnectionHandler.Connection;

        var fileId = connection.ExecuteScalar<int>(@"
        INSERT INTO ""File"" (name, real_file_name, description, creation_date, size, owner_id, content_type, s3_key, poolit_key)
        VALUES (@name, @realFileNameName, @description, @creationDate, @size, @ownerId, @contentType, @s3Key, @poolitKey)
        RETURNING file_id;
        ",
        new
        {
            name = file.Name,
            realFileNameName = file.RealFileName,
            description = file.Description,
            creationDate = file.CreationDate,
            size = file.Size,
            ownerId = file.OwnerId,
            contentType = file.ContentType,
            s3Key = file.S3Key,
            poolitKey = file.PoolitKey
        });

        file.Id = fileId;

        accessEnabledUserIds.Append(file.OwnerId);
        foreach (var userId in accessEnabledUserIds.Distinct())
        {
            DBConnectionHandler.Connection.Execute(@"
            INSERT INTO ""User_File"" (user_id, file_id)
            VALUES (@userId, @fileId)
            ",
            new
            {
                userId,
                fileId
            });
        }
    }

    public List<FileEntity> GetAvailableFiles(int userId)
    {
        var files = DBConnectionHandler.Connection.Query<FileEntity>(@"
        SELECT
            f.file_id AS Id,
            f.name AS Name,
            f.real_file_name AS RealFileName,
            f.description,
            f.creation_date AS CreationDate,
            f.size,
            f.owner_id AS OwnerId,
            f.s3_key AS S3Key,
            f.content_type AS ContentType,
            f.poolit_key AS PoolitKey
        FROM ""File"" as f, ""User_File"" as u_f
        WHERE u_f.user_id = @userId
            AND  u_f.file_id = f.file_id
        ",
        new { userId });

        return files.ToList();
    }

    public FileEntity GetFileById(int fileId)
    {
        var file = DBConnectionHandler.Connection.ExecuteScalar<FileEntity>(@"
        SELECT
            file_id AS Id,
            name AS Name,
            real_file_name AS RealFileName,
            description,
            creation_date AS CreationDate,
            size,
            owner_id AS OwnerId,
            s3_key AS S3Key,
            content_type AS ContentType,
            poolit_key AS PoolitKey
        FROM ""File""
        WHERE file_id=@fileId;
        ",
        new { fileId });

        return file;
    }

    public FileEntity GetFileByPoolitKey(string poolitKey)
    {
        var file = DBConnectionHandler.Connection.QueryFirst<FileEntity>(@"
        SELECT
            file_id AS Id,
            name AS Name,
            real_file_name AS RealFileName,
            description,
            creation_date AS CreationDate,
            size,
            owner_id AS OwnerId,
            s3_key AS S3Key,
            content_type AS ContentType,
            poolit_key AS PoolitKey
        FROM ""File""
        WHERE poolit_key=@poolitKey;
        ",
        new { poolitKey });

        return file;
    }
}