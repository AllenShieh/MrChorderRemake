namespace Lyra.WaveParser
{
    using System;
    using AForge.Math;
    using NAudio.Wave;
    // using Training;
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

        /* Get first n frequencies with max amplitude.
         * offset: Onset Time.
         */
        public double[][] GetNMaxAmpFreqs(int n, int offset)
        {
            // Freq start from 60.
            n = 5;
            const int count = 128;
            double[][] result = new double[count][];
            double[] fftData;
            for (int i = 0; i < count && offset < this.data.Length - this.fftLength; ++i, offset += this.fftLength / count)
            {
                fftData = GetFFTResult(offset);
                while (true)
                {
                    Array.Sort(fftData, 15, 5);
                    int max1AmplitudeIndex = 19;
                    int max2AmplitudeIndex = 18;
                    int max3AmplitudeIndex = 17;
                    int max4AmplitudeIndex = 16;
                    int max5AmplitudeIndex = 15;

                    for (int j = n + 15; j < this.fftLength / 2; ++j)
                    {
                        if (fftData[j] > fftData[max1AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = max3AmplitudeIndex;
                            max3AmplitudeIndex = max2AmplitudeIndex;
                            max2AmplitudeIndex = max1AmplitudeIndex;
                            max1AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max2AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = max3AmplitudeIndex;
                            max3AmplitudeIndex = max2AmplitudeIndex;
                            max2AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max3AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = max3AmplitudeIndex;
                            max3AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max4AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max5AmplitudeIndex])
                        {
                            max5AmplitudeIndex = j;
                        }
                    }

                    int[] tmpIndices = new int[n];
                    tmpIndices[0] = max1AmplitudeIndex;
                    tmpIndices[1] = max2AmplitudeIndex;
                    tmpIndices[2] = max3AmplitudeIndex;
                    tmpIndices[3] = max4AmplitudeIndex;
                    tmpIndices[4] = max5AmplitudeIndex;
                    Array.Sort(tmpIndices);

                    bool passed = true;
                    // Elinimate near frequency
                    for (int j = 1; j < n; ++j)
                    {
                        if (tmpIndices[j] <= tmpIndices[j - 1] + 5)
                        {
                            fftData[tmpIndices[j]] = 0;
                            passed = false;
                        }
                    }

                    if (passed)
                    {
                        result[i] = new double[n];
                        for (int j = 0; j < n; ++j)
                        {
                            result[i][j] = (double)tmpIndices[j] * this.fs / this.fftLength;
                        }

                        break;
                    }
                }
            }

            return result;
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
                    // result[i * count + j] = fftData; // Total number of samples should be count*peakCount.
                    result[i * count + j] = new double[features];
                    Array.Copy(fftData, result[i * count + j], features);
                    l++;
                }
            }
            l = 2000; // Set the number of samples returned.
            double[][] data_return = new double[l][];
            Array.Copy(result, data_return, l);
            return data_return;
        }

        /* Get result notes from audio.
         * indices: Onset time.
         */
         /*
        public int[] GetNotes(int[] indices, int length)
        {
            if(Err != AUDIO_ERROR.NONE)
            {
                return null;
            }

            int[] result = new int[length];
            for (int i = 0; i < result.Length; ++i)
            {
                int index = indices[i];
                double[][] freqs = this.GetNMaxAmpFreqs(5, index); // Seperate one note into multiple notes.
                int[] notes = new int[freqs.Length];
                for(int j = 0; j < notes.Length; ++j)
                {
                    notes[j] = this.learningModel.GetNote(freqs[j]);
                }

                result[i] = this.GetMostMember(notes); // Get result statistically (mostly).
                
            }

            return result;
        }
        */

        private int GetMostMember(int[] numbers)
        {
            int length = numbers.Length;
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for(int i = 0; i < length; ++i)
            {
                if (dic.ContainsKey(numbers[i]))
                {
                    dic[numbers[i]]++;
                }
                else
                {
                    dic[numbers[i]] = 0;
                }
            }

            int maxKey = 0;
            int maxValue = 0;
            dic[0] = 0;
            foreach(KeyValuePair<int, int> pair in dic)
            {
                if(pair.Value > maxValue)
                {
                    maxKey = pair.Key;
                    maxValue = pair.Value;
                }
            }

            return maxKey;
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
