namespace Lyra.WaveParser
{
    using System;
    using AForge.Math;
    using NAudio.Wave;
    using System.Collections.Generic;

    // Audio file reader.
    public class Audio
    {
        // Chunk data of wav file.
        public byte[] data { get; set; }

        // Frequency of sampling.
        public int fs { get; set; }

        // Count of sample point to do fft.
        public int fftLength { get; set; }

        // Error of audio file.
        private AUDIO_ERROR Err = AUDIO_ERROR.NONE;

        // private LearningModel learningModel;

        // Max value of int.
        private const long MAX_INT = 2147483647;

        // Min frequency to recognize.
        private const int MIN_FS = 4;

        // Max frequency to recognize.
        private const int MAX_FS = 4096;

        // Number of samples returned for each training note.
        private const int sampleCountEach = 2000;

        /* Read wav file
         * filename: File name of audio file.
         */
        public Audio(string filename)
        {
            try
            {
                // Read music file.
                if (!filename.EndsWith(".wav"))
                {
                    Err = AUDIO_ERROR.UNKNOWN_FILE_FORMAT;
                    return;
                }

                var fileReader = new WaveFileReader(filename);
                var wavStream = WaveFormatConversionStream.CreatePcmStream(fileReader);

                // Audio too long.
                if (wavStream.Length > MAX_INT)
                {
                    Err = AUDIO_ERROR.AUDIO_TOO_LONG;
                    return;
                }
                // Audio too short.
                else if (wavStream.TotalTime.TotalMilliseconds < 500)
                {
                    Err = AUDIO_ERROR.AUDIO_TOO_SHORT;
                    return;
                }

                // Read raw data.
                int rawLength = (int)wavStream.Length;
                double rawFs = rawLength / wavStream.TotalTime.TotalSeconds;
                if (rawFs < MAX_FS * 2)
                {
                    Err = AUDIO_ERROR.AUDIO_SAMPLE_NOT_ENOUGH;
                    return;
                }

                byte[] rawData = new byte[rawLength];
                wavStream.Read(rawData, 0, rawLength);

                // Down sampling.
                this.fs = MAX_FS * 2; // 1 second, 8192
                this.fftLength = this.fs / 4; // 0.25 second, 2048
                double scale = rawFs / this.fs;
                int length = (int)(rawLength / scale);
                this.data = new byte[length];
                for (int i = 0; i < length; ++i)
                {
                    int startIndex = (int)(scale * i);
                    int endIndex = (int)(scale * (i + 1));
                    // Mid value filter.
                    Array.Sort(rawData, startIndex, endIndex - startIndex);
                    this.data[i] = rawData[(startIndex + endIndex) / 2];
                }

                // Initialize learning model.
                // learningModel = new LearningModel();
            }
            catch
            {
                Err = AUDIO_ERROR.INTERNAL_ERROR;
                return;
            }
        }

        // Get fft result after index, use 0.25 second.
        public double[] GetFFTResult(int index)
        {
            Complex[] fftData = new Complex[this.fftLength];
            double[] result = new double[this.fftLength];
            for (int i = 0; i < this.fftLength; ++i)
            {
                fftData[i] = new Complex(this.data[index + i], 0);
            }

            FourierTransform.FFT(fftData, FourierTransform.Direction.Forward);
            for (int i = 0; i < this.fftLength / 2; ++i)
            {
                result[i] = fftData[i].Re * fftData[i].Re + fftData[i].Im * fftData[i].Im;
            }

            return result;
        }

        // Get data in form of frequency/amplitude.
        public double[][] GetFAData(int[] onsetTime, int peakCount)
        {
            if(Err != AUDIO_ERROR.NONE)
            {
                return null;
            }

            // This count is used to generate more samples.
            // For example, divide each note into 128 notes, so that we have 128 samples for each note detected.
            const int count = 128;
            const int features = 1024;
            double[][] result = new double[count * peakCount][];
            int l = 0;
            for (int i = 0; i < peakCount; i++)
            {
                int index = onsetTime[i];
                double[] fftData;
                // Here length of fftLength is used to transform, so index+fftLength should not exceed the data length.
                for (int j = 0; j < count && index < this.data.Length - this.fftLength; ++j, index += this.fftLength / count)
                {
                    fftData = GetFFTResult(index);
                    result[i * count + j] = new double[features];
                    Array.Copy(fftData, result[i * count + j], features);
                    l++;
                }
            }
            l = sampleCountEach; // Set the number of samples returned.
            double[][] data_return = new double[l][];
            Array.Copy(result, data_return, l);
            return data_return;
        }

        public double[][] GetNoteFAData(int onsetTime)
        {
            if (Err != AUDIO_ERROR.NONE)
            {
                return null;
            }

            // This count is used to generate more samples.
            // For example, divide each note into 128 notes, so that we have 128 samples for each note detected.
            const int count = 128;
            const int features = 1024;
            double[][] result = new double[count][];
            int l = 0;
            
            int index = onsetTime;
            double[] fftData;
            // Here length of fftLength is used to transform, so index+fftLength should not exceed the data length.
            for (int j = 0; j < count && index < this.data.Length - this.fftLength; ++j, index += this.fftLength / count)
            {
                fftData = GetFFTResult(index);
                result[j] = new double[features];
                Array.Copy(fftData, result[j], features);
                l++;
            }
            double[][] data_return = new double[l][];
            Array.Copy(result, data_return, l);
            return data_return;
        }

        public string GetError()
        {
            switch (Err)
            {
                case AUDIO_ERROR.NONE:
                    return "";
                case AUDIO_ERROR.AUDIO_TOO_LONG:
                    return "Audio too long.";
                case AUDIO_ERROR.AUDIO_TOO_SHORT:
                    return "Audio too short.";
                case AUDIO_ERROR.UNKNOWN_FILE_FORMAT:
                    return "Unknown file format.";
                case AUDIO_ERROR.AUDIO_SAMPLE_NOT_ENOUGH:
                    return "Audio sampling frequency not enough";
                default:
                    return "Internal error.";
            }
        }
    }
    
    internal enum AUDIO_ERROR { NONE, UNKNOWN_FILE_FORMAT, AUDIO_TOO_LONG, AUDIO_TOO_SHORT, AUDIO_SAMPLE_NOT_ENOUGH, INTERNAL_ERROR};
}
