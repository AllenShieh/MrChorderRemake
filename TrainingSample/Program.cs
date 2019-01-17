using System;
using Training;
using OnsetDetection;

namespace TrainingSample
{
    class Program
    {
        static public double[][] ConcatVector(double[][] array1, double[][] array2)
        {
            double[][] result = new double[array1.Length + array2.Length][];

            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);

            return result;
        }

        static public int[] ConcatElement(int[] array1, int[] array2)
        {
            int[] result = new int[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);

            return result;
        }

        static void Main(string[] args)
        {
            double[][] inputs = { };
            int[] outputs = { };
            for (int i = 1; i <= Training.LearningModel.noteCount; i++)
            {
                string training_file = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\piano" + i.ToString() + ".wav";
                OnsetDetector onsetDetector = new OnsetDetector(training_file, null);
                double[][] FAData = onsetDetector.GenerateTrainingFAData();
                inputs = ConcatVector(inputs, FAData);
                int[] arr = new int[FAData.Length];
                for (int j = 0; j < arr.Length; j++) arr[j] = i - 1;
                outputs = ConcatElement(outputs, arr);

                Console.WriteLine("Training FA Data generated.");
            }

            LearningModel svm = new LearningModel(inputs, outputs);

            Console.WriteLine("Testing.");
            string testFile = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\t.wav";
            OnsetDetector testDetector = new OnsetDetector(testFile, svm);
            int[] notes = testDetector.GenerateNotes();
            for (int j = 0; j < notes.Length; j++)
            {
                Console.Write(notes[j]);
            }
            Console.Write("\n");


            Console.WriteLine("End");
        }
    }
}
