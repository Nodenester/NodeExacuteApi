using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using NAudio.Wave;
using NodeBaseApi.Version2;
using Type = NodeBaseApi.Version2.Type;

namespace NodeExacuteApi.Data.Blocks
{
    public class AudioLength : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] audioData = (byte[])inputs[0];
            using (var ms = new MemoryStream(audioData))
            using (var reader = new NAudio.Wave.WaveFileReader(ms))
            {
                double lengthInSeconds = reader.TotalTime.TotalSeconds;
                programStructure.InputValues[Outputs[0].Id] = lengthInSeconds;
            }
        }
    }

    public class AudioVolumeChange : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] audioData = (byte[])inputs[0];
            float volume = (float)Convert.ToDouble(inputs[1]);
            using (var ms = new MemoryStream(audioData))
            using (var reader = new NAudio.Wave.WaveFileReader(ms))
            using (var volumeStream = new NAudio.Wave.WaveChannel32(reader) { Volume = volume })
            using (var outputStream = new MemoryStream())
            using (var writer = new NAudio.Wave.WaveFileWriter(outputStream, volumeStream.WaveFormat))
            {
                byte[] buffer = new byte[1024];
                int read;
                while ((read = volumeStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, read);
                }
                byte[] outputAudioData = outputStream.ToArray();
                programStructure.InputValues[Outputs[0].Id] = outputAudioData;
            }
        }
    }

    public class AudioConversion : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] audioData = (byte[])inputs[0];
            string targetFormat = (string)inputs[1];
            using (var ms = new MemoryStream(audioData))
            using (var reader = new NAudio.Wave.WaveFileReader(ms))
            using (var outputStream = new MemoryStream())
            {
                if (targetFormat.ToUpper() == "MP3")
                {
                    using (var writer = new NAudio.Lame.LameMP3FileWriter(outputStream, reader.WaveFormat, NAudio.Lame.LAMEPreset.STANDARD))
                    {
                        reader.CopyTo(writer);
                    }
                }
                else if (targetFormat.ToUpper() == "WAV")
                {
                    using (var writer = new NAudio.Wave.WaveFileWriter(outputStream, reader.WaveFormat))
                    {
                        reader.CopyTo(writer);
                    }
                }
                else
                {
                    throw new ArgumentException("Unsupported audio format.");
                }

                byte[] outputAudioData = outputStream.ToArray();
                programStructure.InputValues[Outputs[0].Id] = outputAudioData;
            }
        }
    }

    public class AudioTrimming : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] audioData = (byte[])inputs[0];
            double startTime = Convert.ToDouble(inputs[1]);
            double endTime = Convert.ToDouble(inputs[2]);
            using (var ms = new MemoryStream(audioData))
            using (var reader = new NAudio.Wave.WaveFileReader(ms))
            using (var outputStream = new MemoryStream())
            using (var writer = new NAudio.Wave.WaveFileWriter(outputStream, reader.WaveFormat))
            {
                reader.CurrentTime = TimeSpan.FromSeconds(startTime);
                int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;
                int startBytes = (int)startTime * bytesPerMillisecond;
                int endBytes = (int)endTime * bytesPerMillisecond;
                int bytesToRead = endBytes - startBytes;
                byte[] buffer = new byte[bytesToRead];
                reader.Read(buffer, 0, bytesToRead);
                writer.Write(buffer, 0, bytesToRead);

                byte[] outputAudioData = outputStream.ToArray();
                programStructure.InputValues[Outputs[0].Id] = outputAudioData;
            }
        }
    }

    public class AudioAmplification : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] audioData = (byte[])inputs[0];
            float factor = (float)Convert.ToDouble(inputs[1]);
            using (var ms = new MemoryStream(audioData))
            using (var reader = new NAudio.Wave.WaveFileReader(ms))
            using (var volumeStream = new NAudio.Wave.WaveChannel32(reader) { Volume = factor })
            using (var outputStream = new MemoryStream())
            using (var writer = new NAudio.Wave.WaveFileWriter(outputStream, volumeStream.WaveFormat))
            {
                byte[] buffer = new byte[1024];
                int read;
                while ((read = volumeStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, read);
                }

                byte[] outputAudioData = outputStream.ToArray();
                programStructure.InputValues[Outputs[0].Id] = outputAudioData;
            }
        }
    }

    public class AudioOverlay : Block
    {
        public override async Task ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableId)
        {
            byte[] audioData1 = (byte[])inputs[0];
            byte[] audioData2 = (byte[])inputs[1];

            using (var ms1 = new MemoryStream(audioData1))
            using (var ms2 = new MemoryStream(audioData2))
            using (var reader1 = new NAudio.Wave.WaveFileReader(ms1))
            using (var reader2 = new NAudio.Wave.WaveFileReader(ms2))
            using (var mixer = new NAudio.Wave.WaveMixerStream32())
            using (var outputStream = new MemoryStream())
            {
                mixer.AutoStop = false;

                var waveStream1 = new NAudio.Wave.WaveChannel32(reader1);
                var waveStream2 = new NAudio.Wave.WaveChannel32(reader2);

                mixer.AddInputStream(waveStream1);
                mixer.AddInputStream(waveStream2);

                // Adjusting to the same volume level for consistency
                waveStream1.Volume = waveStream2.Volume = 0.5f;

                using (var waveOut = new NAudio.Wave.WaveFileWriter(outputStream, mixer.WaveFormat))
                {
                    byte[] buffer = new byte[1024];
                    int read;
                    while ((read = mixer.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        waveOut.Write(buffer, 0, read);
                    }
                }

                byte[] outputAudioData = outputStream.ToArray();
                programStructure.InputValues[Outputs[0].Id] = outputAudioData;
            }
        }
    }
}
