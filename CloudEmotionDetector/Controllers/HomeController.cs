using CloudDetector.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace CloudDetector.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UploadImage()
        {
            try
            {
                string output = string.Empty;

                //get the image file from the request, I assume only one file
                var fileContent = Request.Form.Files[0];
                if (fileContent != null && fileContent.Length > 0)
                {
                    var client = new HttpClient();
                    var queryString = HttpUtility.ParseQueryString(string.Empty);

                    // Request headers, include your own subscription key
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{ enter your subscription key }");
  
                    var uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?" + queryString;

                    HttpResponseMessage response;

                    //copy the file into a stream and into a byte array
                    using (var stream = new MemoryStream())
                    {
                        fileContent.CopyTo(stream);
                        byte[] byteData = stream.ToArray();

                        using (var content = new ByteArrayContent(byteData))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                            //post to the emotion API
                            response = await client.PostAsync(uri, content);

                            output = await response.Content.ReadAsStringAsync();
                        }
                    }
                }

                //convert the raw response string into an OutPutEmotion class
                OutputEmotion outputEmotion = ConvertToEmotionModel(output);

                //return as Json
                return Json(JsonConvert.SerializeObject(outputEmotion));
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        private static OutputEmotion ConvertToEmotionModel(string output)
        {
            //first, deserialize using the Emotion class
            var emotions = JsonConvert.DeserializeObject<Emotion[]>(output);
            //next, find the highest scoring emotion
            var primaryEmotion = emotions[0].scores.FirstOrDefault(x => x.Value == emotions[0].scores.Values.Max()).Key;

            //put these in a new model class
            OutputEmotion outputEmotion = new OutputEmotion
            {
                faceRectangle = emotions[0].faceRectangle,
                primaryEmotion = primaryEmotion
            };

            return outputEmotion;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
