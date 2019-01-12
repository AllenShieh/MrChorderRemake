namespace Lyra.ConsoleSample
{
    using System;
    using WaveParser;

    class Program
    {
        static void Main(string[] args)
        {
            Audio audio = new Audio("star.wav");
            
            int[] indices = { 256, 2048, 3328, 6912, 11008, 15104, 19200, 23296, 31744, 34816, 39680, 43008, 43776, 47360, 52224, 56064, 59392, 69120 };
            int[] notes = audio.GetNotes(indices, indices.Length);

            for (int i = 0; i < notes.Length; ++i)
            {
                Console.Write(notes[i]);
                Console.Write(" ");
            }
            Console.WriteLine();
            Console.ReadLine();
        }

        public static void generateLearningSample()
        {
            /*
            //string[] filePrefixies = { "1", "2", "3", "4", "5", "6", "7", "8" };
            string[] filePrefixies = { "star" };
            StreamWriter writer = new StreamWriter("result.csv", false);
            writer.WriteLine("freq1,freq2,freq3,freq4,freq5,note");
            writer.Close();
            foreach (string filePrefix in filePrefixies)
            {
                Audio audio = new Audio(filePrefix + ".wav");
                float[][] freqs = audio.GetNMaxAmpFreqs(5);
                writer = new StreamWriter("result.csv", true);
                for (int i = 0; i < freqs.Length; ++i)
                {
                    if (freqs[i] != null)
                    {
                        writer.WriteLine("{0},{1},{2},{3},{4},{5}", freqs[i][0], freqs[i][1], freqs[i][2], freqs[i][3], freqs[i][4], filePrefix);
                    }
                }

                writer.Close();
            }
            */
        }
    }
}
