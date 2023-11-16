using NodeBaseApi.Version2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Type = NodeBaseApi.Version2.Type;
using System.Net.Http.Headers;

namespace NodeExacuteApi.Data.Blocks.AiModels
{
    public class SegmentationBlock : Block
    {
        public SegmentationBlock()
        {
            Id = Guid.NewGuid();
            Name = "Segmentation Block";
            Description = "Processes an image using the NVIDIA SegFormer API and returns segmentation results.";
            Inputs = new List<Input>
        {
            new Input { Name = "ImageData", Type = Type.Picture, IsList = false, Description = "Image data for segmentation" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "SegmentationResults", Type = Type.Object, IsList = true, Description = "Segmentation results with labels and masks" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
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

    public class ViTBasePatch16Block : Block
    {
        public ViTBasePatch16Block()
        {
            Id = Guid.NewGuid();
            Name = "ViT Base Patch16 API Block";
            Description = "Processes an image using the ViT Base Patch16 API and returns a list of labels with scores.";
            Inputs = new List<Input>
        {
            new Input { Name = "ImageData", Type = Type.Picture, IsList = false, Description = "Image data for analysis" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "AnalysisResults", Type = Type.Object, IsList = true, Description = "Labels and scores from image analysis" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
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
        public ImageCaptioning()
        {
            Id = Guid.NewGuid();
            Name = "Image Captioning";
            Description = "Generates a text description for a given image using the vit-gpt2-image-captioning model.";
            Inputs = new List<Input>
        {
            new Input { Name = "ImageData", Type = Type.Picture, IsList = false, Description = "Image data to be captioned" }
        };
            Outputs = new List<Output>
        {
            new Output { Name = "Caption", Type = Type.String, IsList = false, Description = "Generated image description" }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            if (inputs[0] is byte[] imageData)
            {
                var caption = await CallImageCaptioningApiAsync(imageData);
                programStructure.InputValues[Outputs[0].Id] = caption;
            }
            else
            {
                throw new ArgumentException("Invalid input type. Expected a byte array representing image data.");
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

}
