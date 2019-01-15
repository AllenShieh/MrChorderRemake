using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.Data;
using OnsetDetection;

namespace Training
{
    public class LearningModel
    {
        private MulticlassSupportVectorMachine<Gaussian> machine;
        private MulticlassSupportVectorLearning<Gaussian> teacher;

        public LearningModel()
        {
            TreeTraining();
        }

        // Get single from input array.
        public int GetNote(double[] input)
        {
            return 1;
            // return tree.Compute(input);
        }

        private double[][] ConcatVector(double[][] array1, double[][] array2)
        {
            double[][] result = new double[array1.Length + array2.Length][];

            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);

            return result;
        }

        private int[] ConcatElement(int[] array1, int[] array2)
        {
            int[] result = new int[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);

            return result;
        }

        private void TreeTraining()
        {
            Console.WriteLine("hello");

            double[][] inputs = { };
            int[] outputs = { };
            for (int i = 1; i <= 8; i++)
            {
                string training_file = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\piano" + i.ToString() + ".wav";
                OnsetDetector od = new OnsetDetector(training_file);
                double[][] FAData = od.GenerateFAData();
                inputs = ConcatVector(inputs, FAData);
                int[] arr = new int[FAData.Length];
                for (int j = 0; j < arr.Length; j++) arr[j] = i - 1;
                outputs = ConcatElement(outputs, arr);

                Console.WriteLine("FA Data generated.");
            }
            Console.WriteLine("Training samples generated.");
            
            teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    UseKernelEstimation = true
                }
            };

            double[][] inputs_training = new double[8000][];
            int[] outputs_training  = new int[8000];
            double[][] inputs_validating = new double[8000][];
            int[] outputs_validating = new int[8000];

            for(int i = 0; i < 8000; i++)
            {
                inputs_training[i] = inputs[2 * i];
                inputs_validating[i] = inputs[2 * i + 1];
                outputs_training[i] = outputs[2 * i];
                outputs_validating[i] = outputs[2 * i + 1];
            }

            Console.WriteLine("Learning.");
            machine = teacher.Learn(inputs_training, outputs_training);

            Console.WriteLine("Deciding.");
            int[] predicted = machine.Decide(inputs_validating);
            /*
            double[] scores = machine.Score(inputs);
            
            double error = new ZeroOneLoss(outputs).Loss(predicted);

            Console.WriteLine(error);
            Console.WriteLine(answer);
            */
            for(int i = 0; i < 8000; i++)
            {
                Console.WriteLine("{0:D} {1:D}", outputs_validating[i], predicted[i]);
            }

            Console.WriteLine("Training done.");
        }
    }
}
