using PDF;

namespace ToPDFSample
{
    class Program
    {
        public static string sourcePath = "C:\\UCLA\\MrChorder-master\\MrChorder\\MrChorder\\Images\\";
        public static string destPath = "C:\\UCLA\\MrChorder-master\\MrChorder\\MrChorder\\Generate\\";

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
            ToPDF PDFGenerator = new ToPDF(destPath+"test.pdf", testMusic, size, "SONG_NAME");

        }
    }
}
