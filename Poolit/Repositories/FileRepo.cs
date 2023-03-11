using Poolit.Models;
using Poolit.Handlers;
using Dapper;
using System.Collections.Generic;

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

        connection.Execute(@"
        INSERT INTO ""File"" (file_name, description, creation_date, size, owner_id, s3_key, poolit_key)
        VALUES (@fileName, @description, @creationDate, @size, @ownerId, @s3Key, @poolitKey);
        ", 
        new {
            fileName = file.Name,
            description = file.Description,
            creationDate = file.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"),
            size=file.Size,
            ownerId = file.OwnerId,
            s3Key = file.S3Key,
            poolitKey = file.PoolitKey
        });

        var fileId = GetFileByPoolitKey(file.PoolitKey).Id;

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
        SELECT *
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
        SELECT * FROM ""File""
        WHERE file_id=@fileId;
        ",
        new { fileId });

        return file;
    }

    public FileEntity GetFileByPoolitKey(string poolitKey)
    {
        var file = DBConnectionHandler.Connection.ExecuteScalar<FileEntity>(@"
        SELECT * FROM ""File""
        WHERE poolit_key=@poolitKey;
        ",
        new { poolitKey });

        return file;
    }
}