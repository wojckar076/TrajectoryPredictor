namespace WPFDesktopUI.Models
{
    public class ResultDataModel
    {
        public double[][] CorrectPoints { get; set; }
        public double[][] IncorrectPoints { get; set; }
        public double Error { get; set; }
        public double ErrorX { get; set; }
        public double ErrorY { get; set; }
    }
}
