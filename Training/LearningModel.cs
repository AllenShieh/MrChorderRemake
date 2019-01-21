using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Training
{
    public class LearningModel
    {
        private MulticlassSupportVectorMachine<Gaussian> machine;
        private MulticlassSupportVectorLearning<Gaussian> teacher;
        
        public static int noteCount = 8;
        public static bool doTraining = false;
        private string modelPath = "C:\\UCLA\\MrChorder-master\\MrChorder\\MrChorder\\Models\\model_svm";

        public LearningModel(double[][] inputs, int[] outputs)
        {
            TreeTraining(inputs, outputs);
        }

        // Get single from input array.
        public int[] GetNote(double[][] input)
        {
            return machine.Decide(input);
        }

        private void TreeTraining(double[][] inputs, int[] outputs)
        {
            if (doTraining)
            {
                Console.WriteLine("Start tree training.");

                teacher = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        UseKernelEstimation = true
                    }
                };

                int sampleCount = inputs.Length;
                int sampleCountDivided = sampleCount / 2;
                double[][] inputs_training = new double[sampleCountDivided][];
                int[] outputs_training = new int[sampleCountDivided];
                double[][] inputs_validating = new double[sampleCountDivided][];
                int[] outputs_validating = new int[sampleCountDivided];

                for (int i = 0; i < sampleCountDivided; i++)
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

                for(int i = 0; i < sampleCountDivided; i++)
                {
                    Console.WriteLine("{0:D} {1:D}", outputs_validating[i], predicted[i]);
                }
                */
                Console.WriteLine("Training done.");

                machine.Save(modelPath);
            }
            else
            {
                machine = Serializer.Load<MulticlassSupportVectorMachine<Gaussian>>(modelPath);
            }
        }
    }
}
