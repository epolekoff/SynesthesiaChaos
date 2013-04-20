using SharpDX.IO;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System.Collections.Generic;
using System.Linq;

namespace SynesthesiaChaos
{
    public class WaveManager
    {
        int defaultSampleRate = 44100;

        private XAudio2 xAudio;
        private List<Wave> waves = new List<Wave>();
        private SourceVoice voice;
        private Wave firstWave;
        AudioBufferAndMetaData buffer;

        public WaveManager()
        {
            xAudio = new XAudio2();
            var mastering = new MasteringVoice(xAudio);
            mastering.SetVolume(1, 0);
            xAudio.StartEngine();
        }

        public void LoadWave(string path, string key)
        {
            var buffer = GetBuffer(path);
            waves.Add(new Wave { Buffer = buffer, Key = key });
        }

        public void PlayWave(string key)
        {
            
            var wave = waves.FirstOrDefault(x => x.Key == key);
            firstWave = wave;
            if (wave != null)
            {
                voice = new SourceVoice(xAudio, wave.Buffer.WaveFormat, true);
                voice.SourceSampleRate = defaultSampleRate;
                voice.SubmitSourceBuffer(wave.Buffer, wave.Buffer.DecodedPacketsInfo);
                voice.Start();
            }
        }

        public void ChangeSpeed(double modifier)
        {
            AudioBufferAndMetaData newBuffer = firstWave.Buffer;
            voice.Stop();

            newBuffer.Stream.Read(firstWave.Buffer.Stream.PositionPointer, 100000, 3);
            voice = new SourceVoice(xAudio, newBuffer.WaveFormat, true);
            voice.SourceSampleRate = (int)(defaultSampleRate * modifier);
            voice.SubmitSourceBuffer(newBuffer, newBuffer.DecodedPacketsInfo);
            voice.Start();
        }

        public void StopSong()
        {
            voice.Stop();
        }

        public void RestartSong()
        {
            voice.Stop();
            voice = new SourceVoice(xAudio, firstWave.Buffer.WaveFormat, true);
            voice.SourceSampleRate = defaultSampleRate;
            voice.SubmitSourceBuffer(firstWave.Buffer, firstWave.Buffer.DecodedPacketsInfo);
            voice.Start();
        }

        private AudioBufferAndMetaData GetBuffer(string soundfile)
        {
            var nativefilestream = new NativeFileStream(soundfile, NativeFileMode.Open, NativeFileAccess.Read, NativeFileShare.Read);
            var soundstream = new SoundStream(nativefilestream);
            var buffer = new AudioBufferAndMetaData
            {
                Stream = soundstream.ToDataStream(),
                AudioBytes = (int)soundstream.Length,
                Flags = BufferFlags.EndOfStream,
                WaveFormat = soundstream.Format,
                DecodedPacketsInfo = soundstream.DecodedPacketsInfo
            };
            return buffer;
        }

        private sealed class AudioBufferAndMetaData : AudioBuffer
        {
            public WaveFormat WaveFormat { get; set; }
            public uint[] DecodedPacketsInfo { get; set; }
        }

        private class Wave
        {
            public AudioBufferAndMetaData Buffer { get; set; }
            public string Key { get; set; }
        }
    }
}