using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace TCupdate
{
    public class TnC
    {
        public  async Task<TCResponse> UploadFileAsync(string filePath, string url)
        {
            // Ensure file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return null;
            }
            TCResponse tcresp = new TCResponse();

            using (var httpClient = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var streamContent = new StreamContent(fileStream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

                    form.Add(streamContent, "file", Path.GetFileName(filePath));

                    string response = await (await httpClient.PostAsync(url, form)).Content.ReadAsStringAsync();
                    tcresp = JsonConvert.DeserializeObject<TCResponse>(response);
                    if (tcresp.ResponseCode=="00")
                    {
                        Console.WriteLine("File uploaded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("File upload failed.");
                        Console.WriteLine(response);

                    }
                    return tcresp;

                }
            }
        }

    }

    public class TCResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string url { get; set; }
    }

    public class TCRequest
    {
        public string cnic { get; set; }
        public List<replace> replaces { get; set; }

    }

    public class replace
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
