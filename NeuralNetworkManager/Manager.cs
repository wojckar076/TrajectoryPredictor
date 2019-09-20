using NeuralNetworkManager.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord.Neuro;
using Accord.Neuro.Learning;
using NeuralNetworkManager.Helpers;


namespace NeuralNetworkManager
{
    public class Manager
    {
        #region Fields

        private readonly object _threadLocker = new object();
        private ActivationNetwork _network;
        private int _id;
        private string _functionX;
        private string _functionY;
        private int _first;
        private int _last;
        private int _points;
        private int _inputPoints;
        private string _hiddenLayers;
        private double _learningRate;
        private double _momentum;
        private double _alpha;
        private double _error;
        private double _errorX;
        private double _errorY;
        private bool _stop;
        public bool IsNew { get; set; }
        public int MaxId { get; set; }

        #endregion

        #region Async methods

        /// <summary>
        /// Method called from button. Starts learning network.
        /// </summary>
        public async void StartLearning(BaseModel configModel)
        {
            UpdateFields(configModel);
            double[][] correctPoints = FunctionSolver.Solve(_functionX, _functionY, _first, _last, _points);
            await Learn(correctPoints);
        }

        /// <summary>
        /// Method called from button. Stops learning network.
        /// </summary>
        public void StopLearning()
        {
            if (!_stop)
            {
                _stop = true;
            }
        }

        /// <summary>
        /// Method called from button. Gets data trained on a network.
        /// </summary>
        public async Task<Tuple<double[][], double[][], double, double, double>> GetTrainedPlot(BaseModel configModel)
        {
            UpdateFields(configModel);
            double[][] correctPoints = FunctionSolver.Solve(_functionX, _functionY, _first, _last, _points);
            double[][] trainedPoints = await ComputeTrainedPoints(correctPoints);
            return new Tuple<double[][], double[][], double, double, double>(correctPoints, trainedPoints, _error, _errorX, _errorY);
        }

        /// <summary>
        /// Method called from button. Gets predicted data.
        /// </summary>
        public async Task<Tuple<double[][], double[][], double, double, double>> GetPredictedPlot(BaseModel configModel)
        {
            UpdateFields(configModel);
            double[][] correctPoints = FunctionSolver.Solve(_functionX, _functionY, _first, _last, _points);
            double[][] predictedPoints = await ComputePredictedPoints(correctPoints);
            return new Tuple<double[][], double[][], double, double, double>(correctPoints, predictedPoints, _error, _errorX, _errorY);
        }

        /// <summary>
        /// Main network learning method.
        /// </summary>
        private async Task Learn(double[][] correctPoints)
        {
            await Task.Run(() =>
            {
                lock (_threadLocker)
                {
                    try
                    {
                        _network = (ActivationNetwork)Network.Load(ConfigurationPath());

                        if (_network == null)
                            throw new NullReferenceException();
                    }
                    catch (Exception)
                    {
                        _network = new ActivationNetwork(
                                    new BipolarSigmoidFunction(_alpha), _inputPoints * 2, GetLayers());
                        _network.Randomize();
                    }

                    try
                    {
                        var teacher = new BackPropagationLearning(_network)
                        {
                            LearningRate = _learningRate,
                            Momentum = _momentum
                        };

                        Scaler scaler = new Scaler(correctPoints);
                        var inputScaledData = correctPoints.Select(a => a.ToArray()).ToArray();
                        scaler.Scale(ref inputScaledData);

                        var resultTrainingData = ComputeTrainingData(inputScaledData);
                        var inputTrainingData = resultTrainingData.Item1.Select(a => a.ToArray()).ToArray();
                        var outputTrainingData = resultTrainingData.Item2.Select(a => a.ToArray()).ToArray();
                        var solution = inputScaledData.Select(a => a.ToArray()).ToArray();

                        double[] networkInput = new double[2 * _inputPoints];

                        while (!_stop)
                        {
                            RandomizeOrder(ref inputTrainingData, ref outputTrainingData);

                            try
                            {
                                // run single epoch and return its summarized learning error
                                _error = teacher.RunEpoch(inputTrainingData, outputTrainingData) /
                                         outputTrainingData.GetLength(0);
                            }
                            catch
                            {
                                continue;
                            }

                            _errorX = 0;
                            _errorY = 0;

                            for (int j = _inputPoints; j < solution.GetLength(0); j++)
                            {
                                for (int k = _inputPoints, l = 0; k > 0; k--, l = l + 2)
                                {
                                    networkInput[l] = inputScaledData[j - k][0];
                                    networkInput[l + 1] = inputScaledData[j - k][1];
                                }

                                try
                                {
                                    var result = _network.Compute(networkInput);
                                    solution[j][0] = result[0];
                                    solution[j][1] = result[1];
                                }
                                catch
                                {
                                    solution[j][0] = double.NaN;
                                    solution[j][1] = double.NaN;
                                }

                                if (double.IsNaN(solution[j][0])) continue;
                                _errorX += Math.Pow(scaler.Rescale(solution[j][0] - inputScaledData[j][0], XYEnum.X), 2);
                                _errorY += Math.Pow(scaler.Rescale(solution[j][1] - inputScaledData[j][1], XYEnum.Y), 2);
                            }

                            _errorX = Math.Sqrt(_errorX / inputScaledData.GetLength(0));
                            _errorY = Math.Sqrt(_errorY / inputScaledData.GetLength(0));
                        }
                    }
                    finally
                    {
                        SaveProgress();
                        _network.Save(ConfigurationPath());

                        _stop = false;
                        _network = null;
                    }
                }
            });
        }

        /// <summary>
        /// Predicts trajectory basing on how well the network was trained.
        /// </summary>
        private async Task<double[][]> ComputePredictedPoints(double[][] correctPoints)
        {
            return await Task.Run(() =>
            {
                lock (_threadLocker)
                {
                    try
                    {
                        _network = (ActivationNetwork)Network.Load(ConfigurationPath());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    var teacher = new BackPropagationLearning(_network)
                    {
                        LearningRate = _learningRate,
                        Momentum = _momentum
                    };

                    Scaler scaler = new Scaler(correctPoints);
                    var correctScaledPoints = correctPoints.Select(a => a.ToArray()).ToArray();
                    scaler.Scale(ref correctScaledPoints);

                    var resultTrainingData = ComputeTrainingData(correctScaledPoints);
                    var inputTrainingData = resultTrainingData.Item1.Select(a => a.ToArray()).ToArray();
                    var outputTrainingData = resultTrainingData.Item2.Select(a => a.ToArray()).ToArray();

                    RandomizeOrder(ref inputTrainingData, ref outputTrainingData);

                    try
                    {
                        _error = teacher.RunEpoch(inputTrainingData, outputTrainingData) /
                                 outputTrainingData.GetLength(0);
                    }
                    catch
                    {
                        RandomizeOrder(ref inputTrainingData, ref outputTrainingData);
                        _error = teacher.RunEpoch(inputTrainingData, outputTrainingData) /
                                 outputTrainingData.GetLength(0);
                    }

                    var first = _last;
                    var last = _last + _points;
                    var inputResultData = FunctionSolver.Solve(_functionX, _functionY, first, last, _points);

                    var pointFromInputData = correctPoints
                        .Where((x, i) => (i > correctPoints.GetLength(0) - _inputPoints - 2) && i != correctPoints.GetLength(0) - 1)
                        .Select(x => x.ToArray()).ToArray();
                    var resultData = pointFromInputData.Concat(inputResultData).ToArray();

                    Scaler resultScaler = new Scaler(resultData);
                    var solution = resultData.Select(a => a.ToArray()).ToArray<double[]>();
                    resultScaler.Scale(ref solution);

                    _errorX = 0;
                    _errorY = 0;

                    for (int j = _inputPoints; j < solution.GetLength(0); j++)
                    {
                        double[] networkInput = new double[2 * _inputPoints];

                        for (int k = _inputPoints, l = 0; k > 0; k--, l = l + 2)
                        {
                            networkInput[l] = solution[j - k][0];
                            networkInput[l + 1] = solution[j - k][1];
                        }

                        try
                        {
                            var result = _network.Compute(networkInput);
                            solution[j][0] = result[0];
                            solution[j][1] = result[1];
                        }
                        catch
                        {
                            solution[j][0] = double.NaN;
                            solution[j][1] = double.NaN;
                        }
                    }

                    for (int i = 0; i < solution.GetLength(0); i++)
                    {
                        if (double.IsNaN(solution[i][0])) continue;
                        solution[i][0] = resultScaler.Rescale(solution[i][0], XYEnum.X);
                        solution[i][1] = resultScaler.Rescale(solution[i][1], XYEnum.Y);
                        _errorX += Math.Pow(resultScaler.Rescale(solution[i][0], XYEnum.X) - resultData[i][0], 2);
                        _errorY += Math.Pow(resultScaler.Rescale(solution[i][1], XYEnum.Y) - resultData[i][1], 2);
                    }

                    _errorX = Math.Sqrt(_errorX / (solution.GetLength(0) - _inputPoints));
                    _errorY = Math.Sqrt(_errorY / (solution.GetLength(0) - _inputPoints));

                    _network.Save(ConfigurationPath());
                    _network = null;

                    solution = solution.Where((el, i) => i >= _inputPoints).ToArray();

                    return solution;
                }
            });
        }

        /// <summary>
        /// Generates points trained points from learning.
        /// </summary>
        private async Task<double[][]> ComputeTrainedPoints(double[][] correctPoints)
        {
            return await Task.Run(() =>
            {
                lock (_threadLocker)
                {
                    try
                    {
                        _network = (ActivationNetwork)Network.Load(ConfigurationPath());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    Scaler scaler = new Scaler(correctPoints);
                    var inputScaledData = correctPoints.Select(a => a.ToArray()).ToArray();
                    scaler.Scale(ref inputScaledData);
                    var solution = inputScaledData.Select(a => a.ToArray()).ToArray();
                    var networkInput = new double[2 * _inputPoints];

                    for (int j = _inputPoints; j < solution.GetLength(0); j++)
                    {
                        for (int k = _inputPoints, l = 0; k > 0; k--, l = l + 2)
                        {
                            networkInput[l] = solution[j - k][0];
                            networkInput[l + 1] = solution[j - k][1];
                        }

                        try
                        {
                            var result = _network.Compute(networkInput);
                            solution[j][0] = result[0];
                            solution[j][1] = result[1];
                        }
                        catch
                        {
                            solution[j][0] = double.NaN;
                            solution[j][1] = double.NaN;
                        }
                    }
                    for (int i = 0; i < solution.GetLength(0); i++)
                    {
                        if (double.IsNaN(solution[i][0]) || double.IsNaN(solution[i][1])) continue;
                        solution[i][0] = scaler.Rescale(solution[i][0], XYEnum.X);
                        solution[i][1] = scaler.Rescale(solution[i][1], XYEnum.Y);
                    }

                    _network.Save(ConfigurationPath());
                    _network = null;

                    return solution;
                }
            });
        }

        #endregion

        #region Private methods

        private void UpdateFields(BaseModel configModel)
        {
            _id = configModel.ID;
            _functionX = configModel.FunctionX;
            _functionY = configModel.FunctionY;
            _first = configModel.FirstPoint;
            _last = configModel.LastPoint;
            _points = configModel.DrawRange;
            _inputPoints = configModel.InputLayerPoints;
            _hiddenLayers = configModel.HiddenLayersAsString;
            _learningRate = configModel.LearningRate;
            _momentum = configModel.Momentum;
            _alpha = configModel.SteepnessAlpha;
        }

        /// <summary>
        /// Generates training data for the network.
        /// </summary>
        private Tuple<double[][], double[][]> ComputeTrainingData(double[][] inputData)
        {
            int inputDataLength = inputData.GetLength(0);
            int sizeData = inputDataLength - _inputPoints;
            double[][] input = new double[sizeData][];
            double[][] output = new double[sizeData][];

            for (int i = _inputPoints, j = 0; i < inputDataLength; i++, j++)
            {
                output[j] = new double[]
                {
                    inputData[i][0],
                    inputData[i][1]
                };

                input[j] = new double[_inputPoints * 2];

                for (int k = _inputPoints, l = 0; k > 0; k--, l = l + 2)
                {
                    input[j][l] = inputData[i - k][0];
                    input[j][l + 1] = inputData[i - k][1];
                }
            }

            return new Tuple<double[][], double[][]>(input, output);
        }

        /// <summary>
        /// Gets layers as an array ready to be used in a network.
        /// </summary>
        private int[] GetLayers()
        {
            string[] layers = _hiddenLayers.Split(null);
            var layersCount = layers.Length;
            int[] totalLayers = new int[layersCount + 1];

            for (int i = 0; i < layersCount; i++)
            {
                totalLayers[i] = int.Parse(layers[i]);
            }
            // 2 points (for x and y) on the output layer
            totalLayers[layersCount] = 2;

            return totalLayers;
        }

        /// <summary>
        /// Randomizes order of the result arrays so the network can learn in more natural way.
        /// </summary>
        private void RandomizeOrder(ref double[][] input, ref double[][] output)
        {
            Random rng = new Random();

            int n = input.GetLength(0);
            while (n > 1)
            {
                int k = rng.Next(n--);
                double[] tempInput = input[n];
                input[n] = input[k];
                input[k] = tempInput;

                double[] tempOutput = output[n];
                output[n] = output[k];
                output[k] = tempOutput;
            }
        }

        /// <summary>
        /// Saves new configuration and the errors.
        /// </summary>
        private void SaveProgress()
        {
            if (!Directory.Exists(@".\LearnedConfigurations"))
            {
                Directory.CreateDirectory(@".\LearnedConfigurations");
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter($@".\LearnedConfigurations\id_{_id}.txt"))
            {
                if (IsNew)
                {
                    File.AppendAllText($@".\LearnedConfigurations\configurations.csv", $"{_id};{_functionX};{_functionY};{_inputPoints};{_hiddenLayers};{_learningRate};{_momentum};{_alpha}{Environment.NewLine}");
                }

                file.WriteLine($"{_id}. x = {_functionX}; y = {_functionY}; Input: {_inputPoints}; Hidden layers: [{_hiddenLayers}]; [LR:{_learningRate}, Mom:{_momentum}, α:{_alpha}] {Environment.NewLine} " +
                               $"Error: {_error} {Environment.NewLine}Error X:  {_errorX} {Environment.NewLine}Error Y:  {_errorY}");
            }
            IsNew = false;
        }

        /// <summary>
        /// Gets the path for saving and loading the networks.
        /// </summary>
        /// <returns></returns>
        private string ConfigurationPath()
        {
            if (!Directory.Exists(@".\LearnedConfigurations"))
            {
                Directory.CreateDirectory(@".\LearnedConfigurations");
            }

            return $@".\LearnedConfigurations\id_{_id}";
        }

        #endregion
    }
}
