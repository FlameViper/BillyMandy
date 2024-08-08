using UnityEngine;
using System.IO;
using System;
using NAudio.Wave;

public static class NAudioPlayer {
    public static AudioClip FromMp3Data(byte[] data) {
        // Load the data into a stream
        MemoryStream mp3stream = new MemoryStream(data);
        // Convert the data in the stream to WAV format
        Mp3FileReader mp3audio = new Mp3FileReader(mp3stream);
        WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3audio);
        // Convert to WAV data
        byte[] wavBytes = AudioMemStream(waveStream).ToArray();
        WAV wav = new WAV(wavBytes);

        AudioClip audioClip;
        if (wav.ChannelCount == 2) {
            audioClip = AudioClip.Create("Audio File Name", wav.SampleCount, 2, wav.Frequency, false);
            audioClip.SetData(wav.StereoChannel, 0);
        }
        else {
            audioClip = AudioClip.Create("Audio File Name", wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
        }
        // Now return the clip
        return audioClip;
    }
   
    private static MemoryStream AudioMemStream(WaveStream waveStream) {
        MemoryStream outputStream = new MemoryStream();
        using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat)) {
            byte[] bytes = new byte[waveStream.Length];
            waveStream.Position = 0;
            waveStream.Read(bytes, 0, Convert.ToInt32(waveStream.Length));
            waveFileWriter.Write(bytes, 0, bytes.Length);
            waveFileWriter.Flush();
        }
        return outputStream;
    }
   
    public static void SaveWav(byte[] wavData, string path) {
        File.WriteAllBytes(path, wavData);
    }

    //public static AudioClip LoadWav(string path) {
    //    byte[] wavData = File.ReadAllBytes(path);
    //    WAV wav = new WAV(wavData);

    //    AudioClip audioClip;
    //    if (wav.ChannelCount == 2) {
    //        audioClip = AudioClip.Create("Audio File Name", wav.SampleCount, 2, wav.Frequency, false);
    //        audioClip.SetData(wav.StereoChannel, 0);
    //    }
    //    else {
    //        audioClip = AudioClip.Create("Audio File Name", wav.SampleCount, 1, wav.Frequency, false);
    //        audioClip.SetData(wav.LeftChannel, 0);
    //    }
    //    return audioClip;
    //}
    public static AudioClip LoadWav(string path) {
        byte[] wavData = File.ReadAllBytes(path);
        WAV wav = new WAV(wavData);

        // Extract the file name without the extension
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

        AudioClip audioClip;

        if (wav.ChannelCount == 2) {
            
            audioClip = AudioClip.Create(fileNameWithoutExtension, wav.SampleCount, 2, wav.Frequency, false);
            audioClip.SetData(wav.StereoChannel, 0);
        }
        else {
            
            audioClip = AudioClip.Create(fileNameWithoutExtension, wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
        }

        return audioClip;
    }

}
public static class AudioClipToWav {
    public static byte[] Convert(AudioClip clip) {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] wav = new byte[samples.Length * 2 + 44];
        int hz = clip.frequency;
        int channels = clip.channels;
        int samplesLength = samples.Length;

        // RIFF header
        wav[0] = (byte)'R';
        wav[1] = (byte)'I';
        wav[2] = (byte)'F';
        wav[3] = (byte)'F';
        byte[] chunkSize = BitConverter.GetBytes(samplesLength * 2 + 36);
        Buffer.BlockCopy(chunkSize, 0, wav, 4, 4);
        wav[8] = (byte)'W';
        wav[9] = (byte)'A';
        wav[10] = (byte)'V';
        wav[11] = (byte)'E';
        wav[12] = (byte)'f';
        wav[13] = (byte)'m';
        wav[14] = (byte)'t';
        wav[15] = (byte)' ';
        wav[16] = 16;
        wav[20] = 1;
        wav[22] = (byte)channels;
        byte[] hzBytes = BitConverter.GetBytes(hz);
        Buffer.BlockCopy(hzBytes, 0, wav, 24, 4);
        byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
        Buffer.BlockCopy(byteRate, 0, wav, 28, 4);
        wav[32] = (byte)(channels * 2);
        wav[34] = 16;
        wav[36] = (byte)'d';
        wav[37] = (byte)'a';
        wav[38] = (byte)'t';
        wav[39] = (byte)'a';
        byte[] subchunk2Size = BitConverter.GetBytes(samplesLength * 2);
        Buffer.BlockCopy(subchunk2Size, 0, wav, 40, 4);

        int offset = 44;
        foreach (float sample in samples) {
            short s = (short)(sample * 32767);
            byte[] shortBytes = BitConverter.GetBytes(s);
            Buffer.BlockCopy(shortBytes, 0, wav, offset, 2);
            offset += 2;
        }

        return wav;
    }
}

/* From http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html */
public class WAV {

    // convert two bytes to one float in the range -1 to 1
    static float bytesToFloat(byte firstByte, byte secondByte) {
        // convert two bytes to one short (little endian)
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    static int bytesToInt(byte[] bytes, int offset = 0) {
        int value = 0;
        for (int i = 0; i < 4; i++) {
            value |= ((int)bytes[offset + i]) << (i * 8);
        }
        return value;
    }
    // properties
    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }
    public float[] StereoChannel { get; internal set; }
    public int ChannelCount { get; internal set; }
    public int SampleCount { get; internal set; }
    public int Frequency { get; internal set; }

    public WAV(byte[] wav) {

        // Determine if mono or stereo
        ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

        // Get the frequency
        Frequency = bytesToInt(wav, 24);

        // Get past all the other sub chunks to get to the data subchunk:
        int pos = 12;   // First Subchunk ID from 12 to 16

        // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97)) {
            pos += 4;
            int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
            pos += 4 + chunkSize;
        }
        pos += 8;

        // Pos is now positioned to start of actual sound data.
        SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
        if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

        // Allocate memory (right will be null if only mono sound)
        LeftChannel = new float[SampleCount];
        if (ChannelCount == 2) RightChannel = new float[SampleCount];
        else RightChannel = null;

        // Write to double array/s:
        int i = 0;
        while (pos < wav.Length) {
            LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
            pos += 2;
            if (ChannelCount == 2) {
                RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
            }
            i++;
        }

        //Merge left and right channels for stereo sound
        if (ChannelCount == 2) {
            StereoChannel = new float[SampleCount * 2];
            //Current position in our left and right channels
            int channelPos = 0;
            //After we've changed two values for our Stereochannel, we want to increase our channelPos
            short posChange = 0;

            for (int index = 0; index < (SampleCount * 2); index++) {

                if (index % 2 == 0) {
                    StereoChannel[index] = LeftChannel[channelPos];
                    posChange++;
                }
                else {
                    StereoChannel[index] = RightChannel[channelPos];
                    posChange++;
                }
                //Two values have been changed, so update our channelPos
                if (posChange % 2 == 0) {
                    if (channelPos < SampleCount) {
                        channelPos++;
                        //Reset the counter for next iterations
                        posChange = 0;
                    }
                }
            }
        }
        else {
            StereoChannel = null;
        }
    }
    
    public override string ToString() {
        return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
    }
}
