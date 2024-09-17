using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

namespace TCupdate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TCController : Controller
    {
        [HttpPost(Name = "TCupdate")]

        public async Task<TCResponse> response(TCRequest request)
        {
            TCResponse termsConditionResponse = new TCResponse();

            try
            {
                string html = "";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "TC","sample.html");
                html = System.IO.File.ReadAllText(path);
                string uploadUrl = "https://localhost:7229/Upload";
                TnC tc = new TnC();
                Console.WriteLine(path);
                foreach (replace r in request.replaces)
                {
                    html = html.Replace(r.key, r.value);
                }


                string filename = request.cnic + ".html";
                string finalpath=Path.Combine(Directory.GetCurrentDirectory(), "TC", filename);
                Console.WriteLine(finalpath);
                System.IO.File.WriteAllText(finalpath, html);


                //lg.infos("File name : " + path + filename);
                termsConditionResponse = await tc.UploadFileAsync(finalpath, uploadUrl);


            }
            catch (Exception ex)
            {
                //lg.infos("Exception: " + ex.Message);
                Console.WriteLine(ex.Message);
            }
            return termsConditionResponse;

        }







        
    }


    
}
