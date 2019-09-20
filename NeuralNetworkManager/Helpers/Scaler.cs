namespace NeuralNetworkManager.Helpers
{
    public class Scaler
    {
        private double _minX;
        private double _minY;
        private double _diffX;
        private double _diffY;

        /// <summary>
        /// Finds the scale of both dimensions of the input array.
        /// </summary>
        public Scaler(double[][] input)
        {
            double maxX = 0;
            double maxY = 0;

            for (int i = 0; i < input.GetLength(0); i++)
            {
                if (input[i][0] > maxX)
                {
                    maxX = input[i][0];
                }
                else if (input[i][0] < _minX)
                {
                    _minX = input[i][0];
                }

                if (input[i][1] > maxY)
                {
                    maxY = input[i][1];
                }
                else if (input[i][1] < _minY)
                {
                    _minY = input[i][1];
                }
            }
            _diffX = maxX - _minX;
            _diffY = maxY - _minY;
        }

        /// <summary>
        /// Scales array values to [0;1].
        /// </summary>
        public void Scale(ref double[][] values)
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                values[i][0] = Scale(values[i][0], XYEnum.X);
                values[i][1] = Scale(values[i][1], XYEnum.Y);
            }
        }

        private double Scale(double value, XYEnum dimension)
        {
            double scaledValue;
            switch (dimension)
            {
                case XYEnum.X:
                    scaledValue = (value - _minX) / _diffX;
                    break;
                default:
                    scaledValue = (value - _minY) / _diffY;
                    break;
            }
            return scaledValue;
        }

        /// <summary>
        /// Rescales array values to their original state.
        /// </summary>
        public double Rescale(double value, XYEnum dimension)
        {
            switch (dimension)
            {
                case XYEnum.X:
                    return (value * _diffX) + _minX;
                default:
                    return (value * _diffY) + _minY;
            }
        }
    }
}
