namespace TypesLibrary
{
    public struct InputData
    {
        public double MaximumInletPressure { get; private set; }
        public double MinimumInletPressure { get; private set; }
        public double OutletPressure { get; private set; }
        public double InitialInletTemperature { get; private set; }
        public double Consumption { get; private set; }
        public InputData(
            double MaximumInletPressure,
            double MinimumInletPressure,
            double OutletPressure,
            double InitialInletTemperature,
            double Consumption) : this()
        {
            this.MaximumInletPressure = MaximumInletPressure;
            this.MinimumInletPressure = MinimumInletPressure;
            this.OutletPressure = OutletPressure;
            this.InitialInletTemperature = InitialInletTemperature;
            this.Consumption = Consumption;
        }
    }

    public struct ReductorCalculationInputData
    {
        public InputData inputData { get; private set; }
        public ReductorType reductorType { get; private set; }
        public BarSpringParameters BarSpringParameters { get; private set; }
        public MainSpringParameters MainSpringParameters { get; private set; }
        public double ValveSaddleWidth { get; private set; }
        public double ValveDiameter { get; private set; }
        public double HighPressurePistonDiameter { get; private set; }
        public double LowPressurePistonDiameter { get; private set; }
        public ReductorCalculationInputData(
            InputData inputData,
            ReductorType reductorType,
            BarSpringParameters BarSpringParameters,
            MainSpringParameters MainSpringParameters,
            double ValveSaddleWidth,
            double ValveDiameter,
            double HighPressurePistonDiameter,
            double LowPressurePistonDiameter) : this()
        {
            this.inputData = inputData;
            this.reductorType = reductorType;
            this.BarSpringParameters = BarSpringParameters;
            this.MainSpringParameters = MainSpringParameters;
            this.ValveSaddleWidth = ValveSaddleWidth;
            this.ValveDiameter = ValveDiameter;
            this.HighPressurePistonDiameter = HighPressurePistonDiameter;
            this.LowPressurePistonDiameter = LowPressurePistonDiameter;
        }
    }
}