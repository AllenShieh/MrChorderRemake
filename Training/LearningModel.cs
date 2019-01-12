using Accord.IO;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Data;

namespace Training
{
    public class LearningModel
    {
        // private DecisionTree tree;

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

        private void TreeTraining()
        {
            Console.WriteLine("hello");

            string training_file = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\training.wav";
            /*
            string modelPath = "C:\\UCLA\\MrChorder-master\\MrChorder\\Training\\LearningModelData\\";
            DataTable table = new ExcelReader(modelPath + "result.xls").GetWorksheet("Sheet1");
            DataTable test_table = new ExcelReader(modelPath + "test_star.xls").GetWorksheet("Sheet1");

            // [index][features] featrues: [p1 p2 p3 p4]
            double[][] inputs = table.ToArray<double>("freq1", "freq2", "freq3", "freq4", "freq5");
            double[][] test_inputs = test_table.ToArray<double>("freq1", "freq2", "freq3", "freq4", "freq5");
            // [outputs]
            int[] outputs = table.Columns["note"].ToArray<int>();

            tree = new DecisionTree(
                inputs: new List<DecisionVariable>
                {
                    DecisionVariable.Continuous("freq1"),
                    DecisionVariable.Continuous("freq2"),
                    DecisionVariable.Continuous("freq3"),
                    DecisionVariable.Continuous("freq4"),
                    DecisionVariable.Continuous("freq5")
                    //...
                },
                classes: 9);
            C45Learning teacher = new C45Learning(tree);

            //C45 results should be 0 to k;

            //train
            double error = teacher.Run(inputs, outputs);
            
            */
        }
    }
}
