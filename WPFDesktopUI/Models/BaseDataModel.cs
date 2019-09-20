using System.Collections.ObjectModel;

namespace WPFDesktopUI.Models
{
    public class BaseDataModel
    {
        public int ID { get; set; }
        public string FunctionX { get; set; }
        public string FunctionY { get; set; }
        public int FirstPoint { get; set; }
        public int LastPoint { get; set; }
        public int DrawRange { get; set; }
        public int InputLayerPoints { get; set; }
        public ObservableCollection<int> HiddenLayers { get; set; }
        public double LearningRate { get; set; }
        public double Momentum { get; set; }
        public double SteepnessAlpha { get; set; }
        
        public string HiddenLayersAsString
        {
            get => LayersToString();
        }

        private string LayersToString()
        {
            string result = "";
            for (int i = 0; i < HiddenLayers.Count; i++)
            {
                if (HiddenLayers[i] != 0)
                    result += HiddenLayers[i] + " ";
            }
            return result.TrimEnd();
        }

        public string FullName
        {
            get
            {
                return $"{ID}. x = {FunctionX}; y = {FunctionY}; Input: {InputLayerPoints}; Hidden layers: [{LayersToString()}]; [Learning rate:{LearningRate}, Momentum:{Momentum}, α:{SteepnessAlpha}]";
            }
        }
    }
}
