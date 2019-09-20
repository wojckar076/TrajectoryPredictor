using Caliburn.Micro;
using NeuralNetworkManager;
using NeuralNetworkManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WPFDesktopUI.Models;
using WPFDesktopUI.DataAccess;
using WPFDesktopUI.Mappers;

namespace WPFDesktopUI.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Fields

        private Manager _manager = new Manager();
        private BaseDataModel _uiModel;
        private BaseDataModel _activeModel;
        private BaseDataModel _selectedModel;
        private BindableCollection<BaseDataModel> _learnedNetworks = new BindableCollection<BaseDataModel>();
        public int LearnedNetworkCount => _learnedNetworks.Count;
        private ObservableCollection<int> _hiddenLayers;
        private PointCollection _correctPointsGraph;
        private PointCollection _trainedPointsGraph;
        private PointCollection _predictedPointsGraph;
        public bool IsLearning { get; set; }
        private string _textBlockMessage = "Ready.";
        private bool _canStart;
        private bool _canStop;
        private bool _canShowProgress;
        private bool _canPredict;

        #endregion

        public ShellViewModel()
        {
            _hiddenLayers = new ObservableCollection<int>(new[] { 8, 8, 0, 0 });
            CanStart = true;
            CanShowProgress = true;
            CanPredict = true;
            IsLearning = false;

            FileManager fileManager = new FileManager();
            if (fileManager.Available)
            {
                LearnedNetworks = fileManager.AvailableConfigurations;
            }
            else if(fileManager.ErrorMessage != null)
            {
                TextBlockMessage = fileManager.ErrorMessage;
                fileManager.ErrorMessage = null;
            }
            _uiModel = new
                BaseDataModel
                {
                    ID = 0,
                    FunctionX = "a",
                    FunctionY = "cos(0.02*a)",
                    FirstPoint = -500,
                    LastPoint = 1000,
                    DrawRange = 800,
                    InputLayerPoints = 8,
                    HiddenLayers = new ObservableCollection<int>(new[] { 8, 8, 0, 0 }),
                    LearningRate = 0.5,
                    Momentum = 0.5,
                    SteepnessAlpha = 0.5
                };
        }

        #region UI elements setters & getters

        public BindableCollection<BaseDataModel> LearnedNetworks
        {
            get { return _learnedNetworks; }
            set
            {
                _learnedNetworks = value;
                NotifyOfPropertyChange(() => LearnedNetworks);
            }
        }
        
        public ObservableCollection<int> HiddenLayers
        {
            get { return _hiddenLayers; }
            set
            {
                _hiddenLayers = value;
                NotifyOfPropertyChange(() => HiddenLayers);
            }
        }

        public string FunctionX
        {
            get { return _uiModel.FunctionX; }
            set
            {
                _uiModel.FunctionX = value;
                NotifyOfPropertyChange(() => FunctionX);
            }
        }
        
        public string FunctionY
        {
            get { return _uiModel.FunctionY; }
            set
            {
                _uiModel.FunctionY = value;
                NotifyOfPropertyChange(() => FunctionY);
            }
        }

        public int FirstPoint
        {
            get { return _uiModel.FirstPoint; }
            set
            {
                _uiModel.FirstPoint = value;
                NotifyOfPropertyChange(() => FirstPoint);
            }
        }

        public int LastPoint
        {
            get { return _uiModel.LastPoint; }
            set
            {
                _uiModel.LastPoint = value;
                NotifyOfPropertyChange(() => LastPoint);
            }
        }

        public int InputLayerPoints
        {
            get { return _uiModel.InputLayerPoints; }
            set
            {
                _uiModel.InputLayerPoints = value;
                NotifyOfPropertyChange(() => InputLayerPoints);
            }
        }

        public int DrawRange {
            get { return _uiModel.DrawRange; }
            set
            {
                _uiModel.DrawRange = value;
                NotifyOfPropertyChange(() => DrawRange);
            }
        }

        public double LearningRate
        {
            get { return _uiModel.LearningRate; }
            set
            {
                _uiModel.LearningRate = value;
                NotifyOfPropertyChange(() => LearningRate);
            }
        }

        public double Momentum
        {
            get { return _uiModel.Momentum; }
            set
            {
                _uiModel.Momentum = value;
                NotifyOfPropertyChange(() => Momentum);
            }
        }

        public double SteepnessAlpha
        {
            get { return _uiModel.SteepnessAlpha; }
            set
            {
                _uiModel.SteepnessAlpha = value;
                NotifyOfPropertyChange(() => SteepnessAlpha);
            }
        }

        public string TextBlockMessage
        {
            get { return _textBlockMessage; }
            set
            {
                _textBlockMessage = value;
                NotifyOfPropertyChange(() => TextBlockMessage);
            }
        }

        public BaseDataModel UiModel
        {
            get { return _uiModel; }
            set
            {
                _uiModel = value;
                NotifyOfPropertyChange(() => UiModel);
            }
        }

        public BaseDataModel ActiveModel
        {
            get { return _activeModel; }
            set
            {
                _activeModel = value;
                NotifyOfPropertyChange(() => ActiveModel);
            }
        }

        public BaseDataModel SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                _uiModel = DataMapper.Default.Map<BaseDataModel>(_selectedModel);
                HiddenLayers = new ObservableCollection<int> { 0, 0, 0, 0 };
                for (int i = 0; i < _selectedModel.HiddenLayers.Count; i++)
                {
                    HiddenLayers[i] = _selectedModel.HiddenLayers[i];
                }
                TextBlockMessage = "Ready.";

                NotifyOfPropertyChange(() => SelectedModel);
                NotifyOfPropertyChange(() => UiModel);
                NotifyOfPropertyChange(() => HiddenLayers);
            }
        }

        public PointCollection CorrectPointsGraph
        {
            get { return _correctPointsGraph; }
            set
            {
                _correctPointsGraph = value;
                NotifyOfPropertyChange(() => CorrectPointsGraph);
            }
        }

        public PointCollection TrainedPointsGraph
        {
            get { return _trainedPointsGraph; }
            set
            {
                _trainedPointsGraph = value;
                NotifyOfPropertyChange(() => TrainedPointsGraph);
            }
        }

        public PointCollection PredictedPointsGraph
        {
            get { return _predictedPointsGraph; }
            set
            {
                _predictedPointsGraph = value;
                NotifyOfPropertyChange(() => PredictedPointsGraph);
            }
        }

        #endregion  

        #region Help methods

        private bool IsNewConfiguration()
        {
            if (LearnedNetworkCount == 0) return true;

            foreach (var item in LearnedNetworks)
            {
                if (item.FunctionX == _uiModel.FunctionX
                    && item.FunctionY == _uiModel.FunctionY
                    && item.InputLayerPoints == _uiModel.InputLayerPoints
                    && item.HiddenLayersAsString == _uiModel.HiddenLayersAsString
                    && Math.Abs(item.LearningRate - _uiModel.LearningRate) < 0.01
                    && Math.Abs(item.Momentum - _uiModel.Momentum) < 0.01
                    && Math.Abs(item.SteepnessAlpha - _uiModel.SteepnessAlpha) < 0.01)
                    return false;
            }
            return true;
        }

        private void LayerSetter()
        {
            _uiModel.HiddenLayers = new ObservableCollection<int> { 0, 0, 0, 0 };
            for (int i = 0; i < HiddenLayers.Count; i++)
            {
                _uiModel.HiddenLayers[i] = HiddenLayers[i];
            }
        }

        #endregion

        #region Button triggers

        public void StartLearning()
        {
            if (IsLearning)
            {
                TextBlockMessage = "Network is already being learnt.";
                return;
            }

            LayerSetter();
            _activeModel = DataMapper.Default.Map<BaseDataModel>(UiModel);
            _manager.IsNew = IsNewConfiguration();
            _manager.MaxId = LearnedNetworkCount;
            if (_manager.IsNew)
            {
                _manager.MaxId = LearnedNetworkCount + 1;
                _uiModel.ID = _manager.MaxId;
                _activeModel.ID = _manager.MaxId;
                _learnedNetworks.Add(_activeModel);
                TextBlockMessage = "Started learning new network...";
            }
            else
                TextBlockMessage = "Started learning existing network...";

            CanStop = true;
            CanStart = false;
            CanPredict = false;
            CanShowProgress = false;
            IsLearning = true;
            BaseModel inputModel = DataMapper.Default.Map<BaseModel>(ActiveModel);
            try
            {
                _manager.StartLearning(inputModel);
            }
            catch (Exception e)
            {
                TextBlockMessage = e.ToString();
                throw;
            }
        }

        public void Stop()
        {
            _manager.StopLearning();
            IsLearning = false;
            CanStop = false;
            CanStart = true;
            CanPredict = true;
            CanShowProgress = true;
            TextBlockMessage = "Success!";
        }

        public async void ShowProgress()
        {
            if (IsNewConfiguration())
            {
                TextBlockMessage = "Can't draw a plot for an unknown configuration.";
                return;
            }
            var result = new ResultDataModel();

            LayerSetter();
            _activeModel = DataMapper.Default.Map<BaseDataModel>(UiModel);
            BaseModel inputData = DataMapper.Default.Map<BaseModel>(ActiveModel);

            try
            {
                var tuple = await _manager.GetTrainedPlot(inputData);
                
                result.CorrectPoints = tuple.Item1;
                result.IncorrectPoints = tuple.Item2;
                result.Error = tuple.Item3;
                result.ErrorX = tuple.Item4;
                result.ErrorY = tuple.Item5;

                CorrectPointsGraph = null;
                TrainedPointsGraph = null;
                PredictedPointsGraph = new PointCollection();

                var correctPointsList = new List<Point>();
                for (int i = 0; i < result.CorrectPoints.GetLength(0); i++)
                {
                    correctPointsList.Add(new Point(result.CorrectPoints[i][0], result.CorrectPoints[i][1]));
                }
                var trainedPointsList = new List<Point>();
                for (int i = 0; i < result.IncorrectPoints.GetLength(0); i++)
                {
                    trainedPointsList.Add(new Point(result.IncorrectPoints[i][0], result.IncorrectPoints[i][1]));
                }

                CorrectPointsGraph = new PointCollection(correctPointsList);
                TrainedPointsGraph = new PointCollection(trainedPointsList);

                TextBlockMessage = $"error: {result.Error}\nerrorX: {result.ErrorX}\nerrorY: {result.ErrorY}";
            }
            catch (Exception e)
            {
                TextBlockMessage = e.Message;
            }
        }

        public async void Predict()
        {
            if (IsNewConfiguration())
            {
                TextBlockMessage = "Can't predict a plot for an unknown configuration.";
                return;
            }
            var result = new ResultDataModel();

            LayerSetter();
            _activeModel = DataMapper.Default.Map<BaseDataModel>(UiModel);
            BaseModel inputData = DataMapper.Default.Map<BaseModel>(ActiveModel);

            try
            {
                var tuple = await _manager.GetPredictedPlot(inputData);
                
                result.CorrectPoints = tuple.Item1;
                result.IncorrectPoints = tuple.Item2;
                result.Error = tuple.Item3;
                result.ErrorX = tuple.Item4;
                result.ErrorY = tuple.Item5;

                CorrectPointsGraph = null;
                TrainedPointsGraph = new PointCollection();
                PredictedPointsGraph = null;

                var correctPointsList = new List<Point>();
                for (int i = 0; i < result.CorrectPoints.GetLength(0); i++)
                {
                    correctPointsList.Add(new Point(result.CorrectPoints[i][0], result.CorrectPoints[i][1]));
                }
                var predictedPointsList = new List<Point>();
                for (int i = 0; i < result.IncorrectPoints.GetLength(0); i++)
                {
                    predictedPointsList.Add(new Point(result.IncorrectPoints[i][0], result.IncorrectPoints[i][1]));
                }

                CorrectPointsGraph = new PointCollection(correctPointsList);
                PredictedPointsGraph = new PointCollection(predictedPointsList);

                TextBlockMessage = $"error: {result.Error}\nerrorX: {result.ErrorX}\nerrorY: {result.ErrorY}";
            }
            catch (Exception e)
            {
                TextBlockMessage = e.Message;
            }
        }

        #endregion

        #region Button controllers

        public bool CanStop
        {
            get { return _canStop; }
            set
            {
                _canStop = value;
                NotifyOfPropertyChange(() => CanStop);
            }
        }

        public bool CanStart
        {
            get { return _canStart && IsLearning; }
            set
            {
                _canStart = value;
                NotifyOfPropertyChange(() => CanStart);
            }
        }

        public bool CanShowProgress
        {
            get { return _canShowProgress; }
            set
            {
                _canShowProgress = value;
                NotifyOfPropertyChange(() => CanShowProgress);
            }
        }

        public bool CanPredict
        {
            get { return _canPredict; }
            set
            {
                _canPredict = value;
                NotifyOfPropertyChange(() => CanPredict);
            }
        }
        #endregion
    }
}
