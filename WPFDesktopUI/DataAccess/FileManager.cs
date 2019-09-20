using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using WPFDesktopUI.Models;

namespace WPFDesktopUI.DataAccess
{
    public class FileManager
    {
        public BindableCollection<BaseDataModel> AvailableConfigurations { get; private set; }
        public bool Available { get; }
        public string ErrorMessage { get; set; }

        public FileManager()
        {
            if (!Directory.Exists(@".\LearnedConfigurations"))
            {
                Directory.CreateDirectory(@".\LearnedConfigurations");
                Available = false;
            }
            else
            {
                if (File.Exists(@".\LearnedConfigurations\configurations.csv"))
                {
                    try
                    {
                        var lines = File.ReadAllLines(@".\LearnedConfigurations\configurations.csv").ToList();
                        AvailableConfigurations = MapModelsFromFile(lines);
                        Available = true;
                    }
                    catch (Exception e)
                    {
                        ErrorMessage = e.ToString();
                    }
                }
            }
        }

        private BindableCollection<BaseDataModel> MapModelsFromFile(List<string> linesFromFile)
        {
            var configurations = new BindableCollection<BaseDataModel>();
            foreach (var line in linesFromFile)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var splitLine = line.Split(';');
                configurations.Add(MapSingleModelFromLine(splitLine));
            }

            return configurations;
        }

        private BaseDataModel MapSingleModelFromLine(string[] line)
        {
            var numbers = line[4].Split(null).Select(int.Parse).ToList();
            var layers = new ObservableCollection<int>(numbers);

            return
                new BaseDataModel
                {
                    ID = int.Parse(line[0]),
                    FunctionX = line[1],
                    FunctionY = line[2],
                    InputLayerPoints = int.Parse(line[3]),
                    HiddenLayers = layers,
                    LearningRate = double.Parse(line[5]),
                    Momentum = double.Parse(line[6]),
                    SteepnessAlpha = double.Parse(line[7]),
                    FirstPoint = -500,
                    LastPoint = 1000,
                    DrawRange = 800
                };
        }
    }
}
