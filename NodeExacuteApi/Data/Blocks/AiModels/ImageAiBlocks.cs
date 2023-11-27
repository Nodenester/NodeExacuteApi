using NodeBaseApi.Version2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Type = NodeBaseApi.Version2.Type;
using System.Net.Http.Headers;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text.RegularExpressions;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    public class Segmentation : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(5);
            programStructure.CurrentPrizing += 5;
            if (inputs[0] is byte[] imageData)
            {
                var segmentationResults = await CallSegformerApiAsync(imageData);
                programStructure.InputValues[Outputs[0].Id] = segmentationResults;
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array representing image data.");
            }
        }

        private async Task<List<Dictionary<string, byte[]>>> CallSegformerApiAsync(byte[] imageData)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/nvidia/segformer-b0-finetuned-ade-512-512";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(responseString);
                    var results = new List<Dictionary<string, byte[]>>();

                    foreach (var item in apiResponse)
                    {
                        var label = item["label"];
                        var mask = Convert.FromBase64String(item["mask"]);
                        results.Add(new Dictionary<string, byte[]> { { label, mask } });
                    }

                    return results;
                }
                else
                {
                    throw new Exception($"API call failed: {response.StatusCode}");
                }
            }
        }
    }

    public class ImageClassification : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(5);
            programStructure.CurrentPrizing += 5;
            if (inputs[0] is byte[] imageData)
            {
                var analysisResults = await CallViTBasePatch16ApiAsync(imageData);
                programStructure.InputValues[Outputs[0].Id] = analysisResults;
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array representing image data.");
            }
        }

        private async Task<List<Dictionary<string, object>>> CallViTBasePatch16ApiAsync(byte[] imageData)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/google/vit-base-patch16-224";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseString);
                    return apiResponse;
                }
                else
                {
                    throw new Exception($"API call failed: {response.StatusCode}");
                }
            }
        }
    }

    public class ImageCaptioning : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(5);
            programStructure.CurrentPrizing += 5;
            try
            {
                byte[] imageData = Convert.FromBase64String(inputs[0]?.ToString());
                var caption = await CallImageCaptioningApiAsync(imageData);
                programStructure.InputValues[Outputs[0].Id] = caption;
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<string> CallImageCaptioningApiAsync(byte[] imageData)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/nlpconnect/vit-gpt2-image-captioning";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(responseString);
                    return apiResponse[0]["generated_text"];
                }
                else
                {
                    throw new Exception($"API call failed: {response.StatusCode}");
                }
            }
        }
    }

    public class ObjectCroping : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(5);
            programStructure.CurrentPrizing += 5;
            try
            {
                byte[] imageData = Convert.FromBase64String(inputs[0]?.ToString());
                var detectionResults = await CallObjectDetectionApiAsync(imageData);
                var croppedImagesWithLabels = await CropImagesAsync(imageData, detectionResults);
                programStructure.InputValues[Outputs[0].Id] = croppedImagesWithLabels;
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<List<Dictionary<string, object>>> CallObjectDetectionApiAsync(byte[] imageData)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/facebook/detr-resnet-50";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseString);
                }
                else
                {
                    throw new Exception($"API call failed: {response.StatusCode}");
                }
            }
        }

        private async Task<List<Tuple<string, byte[]>>> CropImagesAsync(byte[] originalImage, List<Dictionary<string, object>> detectionResults)
        {
            var croppedImagesWithLabels = new List<Tuple<string, byte[]>>();
            foreach (var result in detectionResults)
            {
                var label = result["label"].ToString();
                var box = (Dictionary<string, int>)result["box"];
                var croppedImage = CropImage(originalImage, box);
                croppedImagesWithLabels.Add(new Tuple<string, byte[]>(label, croppedImage));
            }
            return croppedImagesWithLabels;
        }

        private byte[] CropImage(byte[] originalImage, Dictionary<string, int> box)
        {
            using (var ms = new MemoryStream(originalImage))
            {
                using (var image = Image.FromStream(ms))
                {
                    var cropRect = new Rectangle(box["xmin"], box["ymin"], box["xmax"] - box["xmin"], box["ymax"] - box["ymin"]);
                    var target = new Bitmap(cropRect.Width, cropRect.Height);

                    using (var g = Graphics.FromImage(target))
                    {
                        g.DrawImage(image, new Rectangle(0, 0, target.Width, target.Height),
                                         cropRect,
                                         GraphicsUnit.Pixel);
                    }

                    var croppedImageStream = new MemoryStream();
                    target.Save(croppedImageStream, ImageFormat.Jpeg);
                    return croppedImageStream.ToArray();
                }
            }
        }
    }

    public class TextToImage : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(5);
            programStructure.CurrentPrizing += 5;
            var imageData = await CallImageGenerationApiAsync(inputs[0].ToString());
            programStructure.InputValues[Outputs[0].Id] = imageData;
        }

        private async Task<byte[]> CallImageGenerationApiAsync(string textPrompt)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/runwayml/stable-diffusion-v1-5";
            //var apiUrl = "https://api-inference.huggingface.co/models/openskyml/dalle-3-xl";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var requestData = new { inputs = textPrompt };
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class Dalle3 : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(10);
            programStructure.CurrentPrizing += 10;
            var imageData = await CallImageGenerationApiAsync2(inputs[0].ToString());
            programStructure.InputValues[Outputs[0].Id] = imageData;
        }

        private async Task<byte[]> CallImageGenerationApiAsync2(string textPrompt)
        {
            //var apiUrl = "https://api-inference.huggingface.co/models/runwayml/stable-diffusion-v1-5";
            var apiUrl = "https://api-inference.huggingface.co/models/openskyml/dalle-3-xl";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            var requestData = new { inputs = textPrompt };
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            else
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
        }
    }

    public class TextReader : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            programStructure.HasTokens(5);
            programStructure.CurrentPrizing += 5;
            if (inputs[0] is byte[] imageData)
            {
                string extractedText = await CallOcrDonutApiAsync(imageData);
                programStructure.InputValues[Outputs[0].Id] = ProcessExtractedText(extractedText);
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array representing image data.");
            }
        }

        private async Task<string> CallOcrDonutApiAsync(byte[] imageData)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_HF_TOKEN_HERE");

            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync("https://api-inference.huggingface.co/models/jinhybr/OCR-Donut-CORD", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"API call failed: {response.StatusCode}");
                }
            }
        }

        private string ProcessExtractedText(string rawText)
        {
            // Remove all tag-like elements
            var cleanedText = Regex.Replace(rawText, "<[^>]*>", "");

            // Further processing to filter out nonsensical text
            // This is a basic example and might need refinement based on observed output patterns
            var sensibleText = new StringBuilder();
            var words = cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (!word.StartsWith("@") && word.Length > 1) // Basic check to filter out nonsensical parts
                {
                    sensibleText.Append(word + " ");
                }
            }

            return sensibleText.ToString().Trim();
        }
    }
}
