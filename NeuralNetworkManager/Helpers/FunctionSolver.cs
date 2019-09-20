using System;
using System.Linq;
using org.mariuszgromada.math.mxparser;

namespace NeuralNetworkManager.Helpers
{
    public static class FunctionSolver
    {
        /// <summary>
        /// Solves the functions given by the user and returns the result as 2D array of pair of doubles.
        /// </summary>
        public static double[][] Solve(string functionX, string functionY, int start, int end, int size)
        {
            double argValue = start;
            double interval = (double)Math.Abs((end - start)) / (size - 1);

            Argument arg = new Argument("a", argValue);
            Expression expressionX = new Expression(functionX, arg);
            Expression expressionY = new Expression(functionY, arg);

            double[][] array = new double[size][];

            for (int i = 0; i < size; i++)
            {
                try
                {
                    var xResult = expressionX.calculate();
                    var yResult = expressionY.calculate();
                    if (double.IsNaN(xResult) || double.IsNaN(yResult))
                    {
                        argValue += interval;
                        arg.setArgumentValue(argValue);
                        --i;
                        --size;
                        continue;
                    }
                    array[i] = new double[2]
                    {
                        xResult,
                        yResult
                    };
                }
                catch (Exception)
                {
                    continue;
                }

                argValue += interval;
                arg.setArgumentValue(argValue);
            }

            double[][] result = new double[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i];
            }

            return result;
        }
    }
}
