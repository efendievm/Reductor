namespace TypesLibrary
{
    public class SpringParameters
    {
        public double Draw { get; set; }
        public double Resiliency { get; set; }
        public double Index { get; set; }
        public double CoilDiameter { get; set; }
        public double CoilCount { get; set; }
        public double Pitch { get; set; }
        public double Diameter { get; private set; }
        public double UnloadedLength { get; private set; }
        public double LoadedLength { get; private set; }
        public SpringParameters(
            double Draw,
            double Resiliency,
            double Index,
            double CoilDiameter,
            double CoilCount,
            double Pitch,
            double Diameter,
            double UnloadedLength,
            double LoadedLength)
        {
            this.Draw = Draw;
            this.Resiliency = Resiliency;
            this.Index = Index;
            this.CoilDiameter = CoilDiameter;
            this.CoilCount = CoilCount;
            this.Pitch = Pitch;
            this.Diameter = Diameter;
            this.UnloadedLength = UnloadedLength;
            this.LoadedLength = LoadedLength;
        }
    }

    public class MainSpringParameters : SpringParameters
    {
        public MainSpringParameters(
            double Draw,
            double Resiliency,
            double Index,
            double CoilDiameter,
            double CoilCount,
            double Pitch,
            double Diameter,
            double UnloadedLength,
            double LoadedLength)
            : base(Draw, Resiliency, Index, CoilDiameter, CoilCount, Pitch, Diameter, UnloadedLength, LoadedLength) { }
    }

    public class BarSpringParameters : SpringParameters
    {
        public double PressureDrop { get; private set; }
        public double HermeticPressure { get; private set; }
        public BarSpringParameters(
            double Draw,
            double Resiliency,
            double Index,
            double CoilDiameter,
            double CoilCount,
            double Pitch,
            double Diameter,
            double UnloadedLength,
            double LoadedLength,
            double PressureDrop,
            double HermeticPressure) 
            : base(Draw, Resiliency, Index, CoilDiameter, CoilCount, Pitch, Diameter, UnloadedLength, LoadedLength)
        {
            this.PressureDrop = PressureDrop;
            this.HermeticPressure = HermeticPressure;
        }
    }

    public struct BarSpringCalculationInputData
    {
        public double PressureDrop { get; private set; }
        public double ValveSaddleWidth { get; private set; }
        public double ValveDiameter { get; private set; }
        public double HermeticPressure { get; private set; }
        public double Resiliency { get; private set; }
        public double Index { get; private set; }
        public BarSpringCalculationInputData(
            double PressureDrop,
            double ValveSaddleWidth,
            double ValveDiameter,
            double HermeticPressure,
            double Resiliency,
            double Index) : this()
        {
            this.PressureDrop = PressureDrop;
            this.ValveSaddleWidth = ValveSaddleWidth;
            this.ValveDiameter = ValveDiameter;
            this.HermeticPressure = HermeticPressure;
            this.Resiliency = Resiliency;
            this.Index = Index;
        }
    }

    public struct MainSpringDrawCalculationInputData
    {
        public ReductorType reductorType  { get; private set; }
        public double MaximimInletPressure { get; private set; }
        public double OutletPressure { get; private set; }
        public double ValveDiameter  { get; private set; }
        public double HighPressurePistonDiameter { get; private set; }
        public double LowPressurePistonDiameter { get; private set; }
        public double BarStringDraw { get; private set; }
        public MainSpringDrawCalculationInputData(
            ReductorType reductorType,
            double MaximimInletPressure,
            double OutletPressure,
            double ValveDiameter,
            double HighPressurePistonDiameter,
            double LowPressurePistonDiameter,
            double BarStringDraw) : this()
        {
            this.reductorType = reductorType;
            this.MaximimInletPressure = MaximimInletPressure;
            this.OutletPressure = OutletPressure;
            this.ValveDiameter = ValveDiameter;
            this.HighPressurePistonDiameter = HighPressurePistonDiameter;
            this.LowPressurePistonDiameter = LowPressurePistonDiameter;
            this.BarStringDraw = BarStringDraw;
        }
    }
}
