using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using OpenCvSharp;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class ImageResize : Block
    {
        public ImageResize()
        {
            Id = Guid.NewGuid();
            Name = "ImageResize";
            Description = "Resizes an image to the specified dimensions.";
            Inputs = new List<Input>
            {
                new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Width", Description = "The new width.", Type = Type.Number, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Height", Description = "The new height.", Type = Type.Number, IsRequired = true }
            };
            Outputs = new List<Output>
            {
                new Output { Id = Guid.NewGuid(), Name = "Resized Image", Description = "The resized image.", Type = Type.Picture }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];
            int width = Convert.ToInt32(inputs[1]);
            int height = Convert.ToInt32(inputs[2]);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Bitmap originalImage = new Bitmap(ms);
                Bitmap resizedImage = new Bitmap(originalImage, new System.Drawing.Size(width, height));

                using (MemoryStream resultStream = new MemoryStream())
                {
                    resizedImage.Save(resultStream, originalImage.RawFormat);
                    byte[] resultBytes = resultStream.ToArray();
                    programStructure.InputValues[Outputs[0].Id] = resultBytes;
                }
            }
        }
    }

    public class ImageRotate : Block
    {
        public ImageRotate()
        {
            Id = Guid.NewGuid();
            Name = "ImageRotate";
            Description = "Rotates an image by the specified angle.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Angle", Description = "The angle to rotate the image.", Type = Type.Number, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Rotated Image", Description = "The rotated image.", Type = Type.Picture }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];
            float angle = Convert.ToSingle(inputs[1]);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Bitmap originalImage = new Bitmap(ms);
                Bitmap rotatedImage = RotateImage(originalImage, angle);

                using (MemoryStream resultStream = new MemoryStream())
                {
                    rotatedImage.Save(resultStream, originalImage.RawFormat);
                    byte[] resultBytes = resultStream.ToArray();
                    programStructure.InputValues[Outputs[0].Id] = resultBytes;
                }
            }
        }

        private Bitmap RotateImage(Bitmap image, float angle)
        {
            Bitmap rotatedImage = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform((float)image.Width / 2, (float)image.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)image.Width / 2, -(float)image.Height / 2);
                g.DrawImage(image, new System.Drawing.Point(0, 0));
            }
            return rotatedImage;
        }
    }

    public class ImageGrayscale : Block
    {
        public ImageGrayscale()
        {
            Id = Guid.NewGuid();
            Name = "ImageGrayscale";
            Description = "Converts an image to grayscale.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Grayscale Image", Description = "The grayscale image.", Type = Type.Picture }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Bitmap originalImage = new Bitmap(ms);
                Bitmap grayscaleImage = ConvertToGrayscale(originalImage);

                using (MemoryStream resultStream = new MemoryStream())
                {
                    grayscaleImage.Save(resultStream, originalImage.RawFormat);
                    byte[] resultBytes = resultStream.ToArray();
                    programStructure.InputValues[Outputs[0].Id] = resultBytes;
                }
            }
        }

        private Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    Color originalColor = image.GetPixel(x, y);
                    int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    grayscaleImage.SetPixel(x, y, grayColor);
                }
            }
            return grayscaleImage;
        }
    }

    public class DepthMap : Block
    {
        public DepthMap()
        {
            Id = Guid.NewGuid();
            Name = "DepthMap";
            Description = "Generates a depth map from a stereo pair of images.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Left Image", Description = "The left image of the stereo pair.", Type = Type.Picture, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Right Image", Description = "The right image of the stereo pair.", Type = Type.Picture, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Depth Map", Description = "The generated depth map.", Type = Type.Picture }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] leftImageBytes = (byte[])inputs[0];
            byte[] rightImageBytes = (byte[])inputs[1];

            using (Mat leftImage = Cv2.ImDecode(leftImageBytes, ImreadModes.Grayscale))
            using (Mat rightImage = Cv2.ImDecode(rightImageBytes, ImreadModes.Grayscale))
            using (Mat depthMap = new Mat())
            {
                StereoBM stereoBM = StereoBM.Create();
                stereoBM.Compute(leftImage, rightImage, depthMap);

                byte[] depthMapBytes = depthMap.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = depthMapBytes;
            }
        }
    }

    public class Threshold : Block
    {
        public Threshold()
        {
            Id = Guid.NewGuid();
            Name = "Threshold";
            Description = "Applies a threshold to an image.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Threshold Value", Description = "The threshold value.", Type = Type.Number, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Thresholded Image", Description = "The thresholded image.", Type = Type.Picture }
        };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];
            double thresholdValue = Convert.ToDouble(inputs[1]);

            using (Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Grayscale))
            using (Mat thresholded = new Mat())
            {
                Cv2.Threshold(image, thresholded, thresholdValue, 255, ThresholdTypes.Binary);

                byte[] thresholdedBytes = thresholded.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = thresholdedBytes;
            }
        }
    }

    public class Blur : Block
    {
        public Blur()
        {
            Id = Guid.NewGuid();
            Name = "Blur";
            Description = "Applies blurring to an image.";
            Inputs = new List<Input>
            {
                new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Kernel Size", Description = "The size of the kernel used for blurring.", Type = Type.Number, IsRequired = true }
            };
            Outputs = new List<Output>
            {
                new Output { Id = Guid.NewGuid(), Name = "Blurred Image", Description = "The blurred image.", Type = Type.Picture }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];
            int kSize = Convert.ToInt32(inputs[1]);

            using (Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Color))
            using (Mat blurred = new Mat())
            {
                Cv2.Blur(image, blurred, new OpenCvSharp.Size(kSize, kSize));

                byte[] blurredBytes = blurred.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = blurredBytes;
            }
        }
    }

    public class EdgeDetection : Block
    {
        public EdgeDetection()
        {
            Id = Guid.NewGuid();
            Name = "EdgeDetection";
            Description = "Detects edges in an image using the Canny edge detection algorithm.";
            Inputs = new List<Input>
            {
                new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Lower Threshold", Description = "The lower threshold for edge detection.", Type = Type.Number, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Upper Threshold", Description = "The upper threshold for edge detection.", Type = Type.Number, IsRequired = true }
            };
            Outputs = new List<Output>
            {
                new Output { Id = Guid.NewGuid(), Name = "Edges", Description = "The edges detected in the image.", Type = Type.Picture }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];
            double lowerThreshold = Convert.ToDouble(inputs[1]);
            double upperThreshold = Convert.ToDouble(inputs[2]);

            using (Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Grayscale))
            using (Mat edges = new Mat())
            {
                Cv2.Canny(image, edges, lowerThreshold, upperThreshold);

                byte[] edgesBytes = edges.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = edgesBytes;
            }
        }
    }

    public class ColorFilter : Block
    {
        public ColorFilter()
        {
            Id = Guid.NewGuid();
            Name = "ColorFilter";
            Description = "Filters an image based on color.";
            Inputs = new List<Input>
            {
                new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Lower Color B", Description = "The lower bound of the blue channel.", Type = Type.Number, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Lower Color G", Description = "The lower bound of the green channel.", Type = Type.Number, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Lower Color R", Description = "The lower bound of the red channel.", Type = Type.Number, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Upper Color B", Description = "The upper bound of the blue channel.", Type = Type.Number, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Upper Color G", Description = "The upper bound of the green channel.", Type = Type.Number, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Upper Color R", Description = "The upper bound of the red channel.", Type = Type.Number, IsRequired = true }
            };
            Outputs = new List<Output>
            {
                new Output { Id = Guid.NewGuid(), Name = "Filtered Image", Description = "The color filtered image.", Type = Type.Picture }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];
            Scalar lowerColor = new Scalar(Convert.ToDouble(inputs[1]), Convert.ToDouble(inputs[2]), Convert.ToDouble(inputs[3]));
            Scalar upperColor = new Scalar(Convert.ToDouble(inputs[4]), Convert.ToDouble(inputs[5]), Convert.ToDouble(inputs[6]));

            using (Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Color))
            using (Mat hsvImage = new Mat())
            using (Mat filtered = new Mat())
            {
                Cv2.CvtColor(image, hsvImage, ColorConversionCodes.BGR2HSV);
                Cv2.InRange(hsvImage, lowerColor, upperColor, filtered);

                byte[] filteredBytes = filtered.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = filteredBytes;
            }
        }
    }

    public class HistogramEqualization : Block
    {
        public HistogramEqualization()
        {
            Id = Guid.NewGuid();
            Name = "HistogramEqualization";
            Description = "Improves the contrast in an image by stretching the range of intensity values.";
            Inputs = new List<Input>
            {
                new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true }
            };
            Outputs = new List<Output>
            {
                new Output { Id = Guid.NewGuid(), Name = "Equalized Image", Description = "The histogram equalized image.", Type = Type.Picture }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];

            using (Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Grayscale))
            using (Mat equalized = new Mat())
            {
                Cv2.EqualizeHist(image, equalized);

                byte[] equalizedBytes = equalized.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = equalizedBytes;
            }
        }
    }

    public class ImageInversion : Block
    {
        public ImageInversion()
        {
            Id = Guid.NewGuid();
            Name = "ImageInversion";
            Description = "Inverts the colors of an image.";
            Inputs = new List<Input>
            {
                new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true }
            };
            Outputs = new List<Output>
            {
                new Output { Id = Guid.NewGuid(), Name = "Inverted Image", Description = "The inverted image.", Type = Type.Picture }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];

            using (Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Color))
            using (Mat inverted = new Mat())
            {
                Cv2.BitwiseNot(image, inverted);

                byte[] invertedBytes = inverted.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = invertedBytes;
            }
        }
    }

    public class SimpleChromaKey: Block
    {
        public SimpleChromaKey()
        {
            Id = Guid.NewGuid();
            Name = "SimpleChromaKey";
            Description = "Replaces a specified color in the input image with a background image.";
            Inputs = new List<Input>
            {
                new Input { Id = Guid.NewGuid(), Name = "Image", Description = "The input image.", Type = Type.Picture, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Background Image", Description = "The background image.", Type = Type.Picture, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Key Color", Description = "The color to replace in the format R,G,B.", Type = Type.String, IsRequired = true },
                new Input { Id = Guid.NewGuid(), Name = "Tolerance", Description = "Color matching tolerance (0 to 255).", Type = Type.Number, IsRequired = true },
            };
            Outputs = new List<Output>
            {
                new Output { Id = Guid.NewGuid(), Name = "Output Image", Description = "The output image with the chroma key effect applied.", Type = Type.Picture }
            };
        }

        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] imageBytes = (byte[])inputs[0];
            byte[] bgImageBytes = (byte[])inputs[1];
            string[] keyColorStrings = ((string)inputs[2]).Split(',');
            Scalar keyColor = new Scalar(
                Convert.ToDouble(keyColorStrings[2]),
                Convert.ToDouble(keyColorStrings[1]),
                Convert.ToDouble(keyColorStrings[0])
            );
            double tolerance = Convert.ToDouble(inputs[3]);

            using (Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Color))
            using (Mat bgImage = Cv2.ImDecode(bgImageBytes, ImreadModes.Color))
            using (Mat diffImage = new Mat())
            using (Mat mask = new Mat())
            using (Mat invMask = new Mat())
            using (Mat bgResized = new Mat())
            using (Mat foreground = new Mat())
            using (Mat background = new Mat())
            using (Mat outputImage = new Mat())
            {
                Cv2.Absdiff(image, keyColor, diffImage);
                Cv2.CvtColor(diffImage, mask, ColorConversionCodes.BGR2GRAY);
                Cv2.Threshold(mask, mask, tolerance, 255, ThresholdTypes.BinaryInv);
                Cv2.BitwiseNot(mask, invMask);

                Cv2.Resize(bgImage, bgResized, image.Size());

                Cv2.BitwiseAnd(image, image, foreground, mask);
                Cv2.BitwiseAnd(bgResized, bgResized, background, invMask);
                Cv2.Add(foreground, background, outputImage);

                byte[] outputBytes = outputImage.ToBytes(".png");
                programStructure.InputValues[Outputs[0].Id] = outputBytes;
            }
        }
    }
}
