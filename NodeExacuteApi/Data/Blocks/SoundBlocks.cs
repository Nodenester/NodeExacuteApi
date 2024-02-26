using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;
using FlacLibSharp;
using NAudio;
using NAudio.Flac;
using LibVLCSharp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NodeExacuteApi.Data.Blocks
{
    public class AudioLength : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            byte[] audioData = (byte[])inputs[0];

            // Decode the FLAC audio data
            FlacFile flacStream = new FlacFile(new MemoryStream(audioData));
            var duration = flacStream.StreamInfo.Duration;

            programStructure.InputValues[Outputs[0].Id] = duration;
        }
    }

    public class AudioVolumeChange : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            byte[] audioData = AudioConverter.EnsureWavFormat((byte[])inputs[0]);
            float volume = (float)Convert.ToDouble(inputs[1]);
            MemoryStream outputStream = new MemoryStream(); // This will hold the output audio data

            using (var ms = new MemoryStream(audioData))
            using (var reader = new WaveFileReader(ms)) // Now assuredly for WAV files
            {
                var volumeProvider = new VolumeWaveProvider16(reader) { Volume = volume };
                WaveFileWriter.WriteWavFileToStream(outputStream, volumeProvider);
            }

            byte[] outputAudioData = outputStream.ToArray();
            programStructure.InputValues[Outputs[0].Id] = outputAudioData; // Return the modified audio data
        }
    }

    //public class AudioTrimming : Block
    //{
    //    public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
    //    {
    //        byte[] audioData = AudioConverter.EnsureWavFormat((byte[])inputs[0]);
    //        double startTime = Convert.ToDouble(inputs[1]);
    //        double endTime = Convert.ToDouble(inputs[2]);
    //        MemoryStream outputStream = new MemoryStream();

    //        using (var ms = new MemoryStream(audioData))
    //        using (var reader = new AudioFileReader(ms)) // Correct usage of MemoryStream with AudioFileReader
    //        {
    //            var trimmed = new OffsetSampleProvider(reader)
    //            {
    //                SkipOver = TimeSpan.FromSeconds(startTime),
    //                Take = TimeSpan.FromSeconds(endTime - startTime)
    //            };

    //            WaveFileWriter.WriteWavFileToStream(outputStream, new SampleToWaveProvider(trimmed)); // Potential issue here
    //        }

    //        byte[] outputAudioData = outputStream.ToArray();
    //        programStructure.InputValues[Outputs[0].Id] = outputAudioData;
    //    }
    //}

    public class AudioAmplification : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            byte[] audioData = AudioConverter.EnsureWavFormat((byte[])inputs[0]);
            float factor = (float)Convert.ToDouble(inputs[1]);
            MemoryStream outputStream = new MemoryStream();

            using (var ms = new MemoryStream(audioData))
            using (var reader = new WaveFileReader(ms))
            using (var writer = new WaveFileWriter(outputStream, reader.WaveFormat))
            {
                var buffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                int bytesRead;
                byte[] byteBuffer = new byte[buffer.Length * 4]; // Assuming 32-bit float samples
                while ((bytesRead = reader.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead / 4; i++)
                    {
                        float sample = BitConverter.ToSingle(byteBuffer, i * 4);
                        sample *= factor;
                        BitConverter.GetBytes(sample).CopyTo(byteBuffer, i * 4);
                    }
                    writer.Write(byteBuffer, 0, bytesRead);
                }
            }

            byte[] outputAudioData = outputStream.ToArray();
            programStructure.InputValues[Outputs[0].Id] = outputAudioData;
        }
    }

    public class AudioOverlay : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            byte[] audioData1 = AudioConverter.EnsureWavFormat((byte[])inputs[0]);
            byte[] audioData2 = AudioConverter.EnsureWavFormat((byte[])inputs[1]);
            MemoryStream outputStream = new MemoryStream();

            var targetFormat = WaveFormat.CreateIeeeFloatWaveFormat(16000, 1);

            using (var waveStream1 = new WaveFileReader(new MemoryStream(audioData1)))
            using (var waveStream2 = new WaveFileReader(new MemoryStream(audioData2)))
            {
                var provider1 = AudioConverter.ConvertToMatchingFormat(waveStream1, WaveFormat.CreateIeeeFloatWaveFormat(16000, 1));
                var provider2 = AudioConverter.ConvertToMatchingFormat(waveStream2, WaveFormat.CreateIeeeFloatWaveFormat(16000, 1));

                var mixer = new MixingWaveProvider32(new IWaveProvider[] { provider1, provider2 });

                // Ensure the mixer outputs in 16-bit to reduce file size and compatibility
                WaveFormat waveFormat = new WaveFormat(16000, 32, 1);
                using (var resampler = new MediaFoundationResampler(mixer, waveFormat))
                {
                    resampler.ResamplerQuality = 60; // Set the quality (optional)
                    WaveFileWriter.WriteWavFileToStream(outputStream, resampler);
                }
            }

            byte[] mixedAudioData = outputStream.ToArray();
            programStructure.InputValues[Outputs[0].Id] = mixedAudioData;
        }
    }


    public static class AudioConverter
    {
        public static byte[] EnsureWavFormat(byte[] audioData)
        {
            using (var msInput = new MemoryStream(audioData))
            {
                if (IsWaveFormat(msInput))
                {
                    return audioData; // It's already a WAV file, return it directly
                }
                else
                {
                    msInput.Position = 0; // Reset stream position after checking format
                                          // Placeholder for actual FLAC to WAV conversion
                    return ConvertFlacToWav(msInput); // Implement this method based on your FLAC decoding library
                }
            }
        }

        public static IWaveProvider ConvertToMatchingFormat(WaveFileReader waveReader, WaveFormat targetFormat)
        {
            // Check if conversion to IEEE float format is necessary
            // This checks encoding explicitly alongside sample rate and channel count
            bool needsIeeeFloatConversion = waveReader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat ||
                                            waveReader.WaveFormat.SampleRate != targetFormat.SampleRate ||
                                            waveReader.WaveFormat.Channels != targetFormat.Channels;

            if (needsIeeeFloatConversion)
            {
                // Convert to IEEE float format with target sample rate and channels
                WaveFormat ieeeFloatFormat = WaveFormat.CreateIeeeFloatWaveFormat(targetFormat.SampleRate, targetFormat.Channels);
                return new MediaFoundationResampler(waveReader, ieeeFloatFormat) { ResamplerQuality = 60 };
            }
            else
            {
                // Already matches the target format including IEEE float encoding, return as is
                return waveReader;
            }
        }

        private static bool IsWaveFormat(MemoryStream audioStream)
        {
            try
            {
                using (var reader = new WaveFileReader(audioStream))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static byte[] ConvertFlacToWav(MemoryStream flacStream)
        {
            // Assuming FlacReader is analogous to WaveFileReader for FLAC files in NAudio.Flac
            using (var flacReader = new FlacReader(flacStream))
            {
                // Create a MemoryStream to hold the converted WAV data
                using (var wavStream = new MemoryStream())
                {
                    WaveFileWriter.WriteWavFileToStream(wavStream, flacReader);

                    return wavStream.ToArray();
                }
            }
        }
    }
}
