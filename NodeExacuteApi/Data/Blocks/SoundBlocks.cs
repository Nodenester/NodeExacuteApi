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
        public AudioLength()
        {
            Id = Guid.NewGuid();
            Name = "AudioLength";
            Description = "Gets the length of the audio in seconds.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Audio", Description = "The audio file.", Type = Type.Audio, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Length", Description = "The length of the audio in seconds.", Type = Type.Number }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
        {
            byte[] audioData = (byte[])inputs[0];
            using (var ms = new MemoryStream(audioData))
            using (var reader = new NAudio.Wave.WaveFileReader(ms))
            {
                double lengthInSeconds = reader.TotalTime.TotalSeconds;
                programStructure.InputValues[Outputs[0].Id] = lengthInSeconds;
                return new List<object> { lengthInSeconds };
            }
        }
    }

    public class AudioVolumeChange : Block
    {
        public AudioVolumeChange()
        {
            Id = Guid.NewGuid();
            Name = "AudioVolumeChange";
            Description = "Changes the volume of the audio.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Audio", Description = "The audio file.", Type = Type.Audio, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Volume", Description = "The desired volume level (0.0 to 1.0).", Type = Type.Number, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output Audio", Description = "The audio with the new volume level.", Type = Type.Audio }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
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
                return new List<object> { outputAudioData };
            }
        }
    }

    public class AudioConversion : Block
    {
        public AudioConversion()
        {
            Id = Guid.NewGuid();
            Name = "AudioConversion";
            Description = "Converts the audio to a specified format.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Audio", Description = "The audio file.", Type = Type.Audio, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Target Format", Description = "The target audio format (e.g., MP3, WAV).", Type = Type.String, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output Audio", Description = "The converted audio file.", Type = Type.Audio }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
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
                return new List<object> { outputAudioData };
            }
        }
    }

    public class AudioTrimming : Block
    {
        public AudioTrimming()
        {
            Id = Guid.NewGuid();
            Name = "AudioTrimming";
            Description = "Trims the audio to a specified duration.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Audio", Description = "The audio file.", Type = Type.Audio, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Start Time", Description = "The start time (in seconds).", Type = Type.Number, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "End Time", Description = "The end time (in seconds).", Type = Type.Number, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output Audio", Description = "The trimmed audio file.", Type = Type.Audio }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
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
                return new List<object> { outputAudioData };
            }
        }
    }

    public class AudioAmplification : Block
    {
        public AudioAmplification()
        {
            Id = Guid.NewGuid();
            Name = "AudioAmplification";
            Description = "Amplifies the audio by a specified factor.";
            Inputs = new List<Input>
        {
            new Input { Id = Guid.NewGuid(), Name = "Audio", Description = "The audio file.", Type = Type.Audio, IsRequired = true },
            new Input { Id = Guid.NewGuid(), Name = "Amplification Factor", Description = "The amplification factor.", Type = Type.Number, IsRequired = true }
        };
            Outputs = new List<Output>
        {
            new Output { Id = Guid.NewGuid(), Name = "Output Audio", Description = "The amplified audio file.", Type = Type.Audio }
        };
        }

        public override List<object> ExecuteAsync(List<object> inputs, ProgramStructure programStructure, string sessionId, Guid variableid)
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
                return new List<object> { outputAudioData };
            }
        }
    }

}
