using System;
using Training;
using OnsetDetection;

namespace TrainingSample
{
    class Program
    {
        static void Main(string[] args)
        {
            
            for (int i = 1; i <= 8; i++)
            {
                string training_file = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\piano"+i.ToString()+".wav";
                OnsetDetector od = new OnsetDetector(training_file);
                // Get onset times.

                Console.WriteLine(i);

                double[][] FAData = od.GenerateFAData();
                Console.WriteLine("FA Data generated.");
            }
            /*
            string training_file = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\piano4.wav";
            OnsetDetector od = new OnsetDetector(training_file);
            double[][] FAData = od.GenerateFAData();
            /*
            LearningModel model = new LearningModel();

            double[] test = { 132, 260, 392, 784, 916 };
            int res = model.GetNote(test);
            Console.WriteLine(res);
            Console.ReadLine();
            */
        }
    }
}
