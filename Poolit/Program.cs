using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;
using Amazon.S3;
using Amazon.S3.Model;
using S3ClassLibrary;

//var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("yandex.cloud.config.json")
    .Build();
string accessKey = configuration.GetValue<string>("AWS:AccessKey");
string secretKey =configuration.GetValue<string>("AWS:SecretKey");

S3Manager s3Manager = new S3Manager(accessKey, secretKey, "vaga-test-bucket");
var sr = new StreamReader(@"D:\Vlad's files\Code Projects\Poolit\Poolit\input.txt");
Console.WriteLine(sr.ReadLine());
await s3Manager.PutObjectAsync("4",  @"D:\Vlad's files\Code Projects\Poolit\Poolit\Program.cs", new MetadataCollection());
/*
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();*/
