using System;
using System.IO;
using Training;
using OnsetDetection;
using System.Text;

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

        static double[] GetAccuracy(double[][] notes, double[][] labels)
        {
            int note = 0;
            int label = 0;
            int correct = 0;
            double right_period = 0;
            double wrong_period = 0;
            double cur = 0;
            for (; note < notes.Length; note++)
            {
                for (; label < labels.Length; label++)
                {
                    if (label == labels.Length - 1)
                    {
                        break;
                    }
                    else if (cur >= labels[label + 1][1])
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (notes[note][0] + 1 == labels[label][0])
                {
                    correct++;
                }
                if (label==labels.Length-1 || cur + notes[note][1] <= labels[label + 1][1])
                {
                    if (notes[note][0] + 1 == labels[label][0])
                    {
                        right_period += notes[note][1];
                    }
                    else
                    {
                        wrong_period += notes[note][1];
                    }
                }
                else
                {
                    double first_period = labels[label + 1][1] - cur;
                    double second_period = notes[note][1] - first_period;
                    if (notes[note][0] + 1 == labels[label][0])
                    {
                        right_period += first_period;
                    }
                    else
                    {
                        wrong_period += first_period;
                    }
                    if (notes[note][0] + 1 == labels[label + 1][0])
                    {
                        right_period += second_period;
                    }
                    else
                    {
                        wrong_period += second_period;
                    }
                }
                cur += notes[note][1];
            }
            double[] acc = new double[2];
            acc[0] = (double)correct / (double)notes.Length;
            acc[1] = right_period / (right_period + wrong_period);
            return acc;
        }

        static void Main(string[] args)
        {
            double[][] inputs = { };
            int[] outputs = { };
            if (LearningModel.doTraining)
            {
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
            }

            LearningModel svm = new LearningModel(inputs, outputs);

            Console.WriteLine("Testing.");
            string testFile = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\test3.wav";
            OnsetDetector testDetector = new OnsetDetector(testFile, svm);
            double[][] notes = testDetector.GenerateNotes();

            double[][] labels = { };
            foreach (string line in File.ReadLines("C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\label3", Encoding.UTF8))
            {
                double[][] t = new double[1][];
                t[0] = new double[2];
                string[] s = line.Split(' ');
                t[0][0] = Double.Parse(s[0]);
                t[0][1] = Double.Parse(s[1]);
                labels = ConcatVector(labels, t);   
            }

            double[] acc = GetAccuracy(notes, labels);


            Console.WriteLine("End");
        }
    }
}
