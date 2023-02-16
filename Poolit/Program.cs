using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;
using Amazon.S3;
using Amazon.S3.Model;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("yandex.cloud.config.json")
    .Build();
string accessKey = configuration.GetValue<string>("AWS:AccessKey");
string secretKey =configuration.GetValue<string>("AWS:SecretKey");
// Add services to the container.
/*var awsSettings = builder.Configuration.GetChildren();
foreach (var section in awsSettings)
{
    Console.WriteLine(section.Key);
}// null
Console.WriteLine(awsSettings);*/
//var token = configuration.Get("token"); // null
//string accessKey = builder.Configuration.

AmazonS3Config config = new AmazonS3Config()
{
    ServiceURL = "http://s3.yandexcloud.net:80",
    UseHttp = true,
    ForcePathStyle = true,
    ProxyHost = "s3.yandexcloud.net",
    ProxyPort = 80
};

AWSCredentials creds = new BasicAWSCredentials(accessKey, secretKey);
AmazonS3Client s3Client = new AmazonS3Client(creds, config);

BucketsController.S3Client = s3Client;
ListBucketsResponse response = await s3Client.ListBucketsAsync();
foreach (S3Bucket b in response.Buckets)
{
    Console.WriteLine("{0}\t{1}", b.BucketName, b.CreationDate);
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
/*var options = builder.Configuration.GetAWSOptions();
options
AWSOptions op2 = new AWSOptions(builder.Configuration)*/
//AmazonS3Config configsS3 = new AmazonS3Config()
        
/*AmazonS3Config configsS3 = new AmazonS3Config {
    
    ServiceURL = "http://s3.yandexcloud.net"
};*/
/*options.Region = RegionEndpoint.AFSouth1;
    
IAmazonS3 client = options.CreateServiceClient<IAmazonS3>();
*/


//AmazonS3Client s3client = new AmazonS3Client(configsS3);

/*builder.Services.AddDefaultAWSOptions(options); //options);
builder.Services.AddAWSService<IAmazonS3>(s3Client);*/



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
