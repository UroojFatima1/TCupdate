using Grpc.Net.Client;
using Grpc.Core;
using GrpcUploadService;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TCupdate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TC2Controller : Controller
    {
        [HttpPost(Name = "update")]
        public async Task<TCResponse> Response(TCRequest request)
        {
            TCResponse termsConditionResponse = new TCResponse();

            try
            {
                string html = "";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "TC", "sample.html");
                html = System.IO.File.ReadAllText(path);

                foreach (var r in request.replaces)
                {
                    html = html.Replace(r.key, r.value);
                }

                string filename = request.cnic + ".html";
                string fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TC", filename);
                System.IO.File.WriteAllText(fullFilePath, html);

                // gRPC Channel creation
                using var channel = GrpcChannel.ForAddress("https://localhost:7229"); 
                var client = new UploadService.UploadServiceClient(channel);

                // Read the file and send it via gRPC
                var fileBytes = await System.IO.File.ReadAllBytesAsync(fullFilePath);

                var uploadRequest = new UploadFileRequest
                {
                    FileName = filename,
                    FileData = Google.Protobuf.ByteString.CopyFrom(fileBytes)
                };

                var response = await client.UploadFileAsync(uploadRequest);

                // Process the gRPC response
                termsConditionResponse.ResponseCode = response.ResponseCode;
                termsConditionResponse.ResponseMessage = response.ResponseMessage;
                termsConditionResponse.url = response.Url;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return termsConditionResponse;
        }
    }
}
