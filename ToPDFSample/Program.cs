using PDF;

namespace ToPDFSample
{
    class Program
    {
        static void Main(string[] args)
        {

            // Array test
            float[] testMusic;
            testMusic = new float[50];
            for (int n = 0; n < 50; n++)
            {
                testMusic[n] = n % 20 - 4;
            }
            int size = 50;
            ToPDF.ScoreCreation("./", "./t1.pdf", testMusic, size, "NAME", "X", "Y", 1);

        }
    }
}
