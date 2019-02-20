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
            float[][] testMusic;
            testMusic = new float[5][];
            for(int i = 0; i < 5; i++)
            {
                testMusic[i] = new float[2];
                testMusic[i][0] = i;
                testMusic[i][1] = (float)i / 4;
            }
            int size = 5;
            ToPDF PDFGenerator = new ToPDF(destPath+"test.pdf", testMusic, size, "SONG_NAME");

        }
    }
}
