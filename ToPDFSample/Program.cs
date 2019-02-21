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
            double[][] testMusic;
            testMusic = new double[20][];
            for(int i = 0; i < testMusic.Length; i++)
            {
                testMusic[i] = new double[2];
                testMusic[i][0] = i % 10;
                testMusic[i][1] = 1;
                /*
                testMusic[i][1] = (double)(i % 4) / 4;
                if (testMusic[i][1] == 0)
                {
                    testMusic[i][1] = 1;
                }
                */
            }
            ToPDF PDFGenerator = new ToPDF(destPath+"test.pdf", testMusic, testMusic.Length, "SONG_NAME");

        }
    }
}
