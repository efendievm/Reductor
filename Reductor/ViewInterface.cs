using System;
using TypesLibrary;

namespace ViewInterface
{
    public interface IView
    {
        event EventHandler<GetValveDiameterEventArgs> GetValveDiameter;
        event EventHandler<CalculateBarSpringEventArgs> CalculateBarSpring;
        event EventHandler<CalculateMainSpringEventArgs> CalculateMainSpring;
        event EventHandler<CalculateMainSpringDrawEventArgs> CalculateMainSpringDraw;
        event EventHandler<CalculatePressureDropEventArgs> CalculatePressureDrop;
        event EventHandler<PreciseSpringEventArgs> PreciseSpring;
        event EventHandler<CalculateReductorEventArgs> CalculateReductor;
        event EventHandler<SaveResultsEventArgs> SaveResults;
        event EventHandler<SaveReductorEventArgs> SaveReductor;
        event EventHandler<OpenReductorEventArgs> OpenReductor;
        event EventHandler<OpenExampleEventArgs> OpenExample;
    }

    public class GetValveDiameterEventArgs : EventArgs
    {
        public InputData inputData { get; private set; }
        public double ValveDiameter { get; set; }
        public GetValveDiameterEventArgs(InputData inputData)
        {
            this.inputData = inputData;
        }
    }

    public class CalculateBarSpringEventArgs : EventArgs
    {
        public BarSpringCalculationInputData barSpringCalculationInputData { get; private set; }
        public ValveStroke valveStroke { get; set; }
        public SpringParameters springParameters { get; set; }
        public CalculateBarSpringEventArgs(BarSpringCalculationInputData barSpringCalculationInputData, ValveStroke valveStroke)
        {
            this.barSpringCalculationInputData = barSpringCalculationInputData;
            this.valveStroke = valveStroke;
        }
    }

    public class CalculateMainSpringEventArgs : EventArgs
    {
        public double Draw { get; private set; }
        public double Resiliency { get; private set; }
        public double Index { get; private set; }
        public SpringParameters springParameters { get; set; }
        public CalculateMainSpringEventArgs(
            double Draw,
            double Resiliency,
            double Index)
        {
            this.Draw = Draw;
            this.Resiliency = Resiliency;
            this.Index = Index;
        }
    }

    public class CalculateMainSpringDrawEventArgs : EventArgs
    {
        public MainSpringDrawCalculationInputData mainSpringCalculationInputData  { get; private set; }
        public double Draw { get; set; }
        public CalculateMainSpringDrawEventArgs(MainSpringDrawCalculationInputData mainSpringCalculationInputData)
        {
            this.mainSpringCalculationInputData = mainSpringCalculationInputData;
        }
    }

    public class CalculatePressureDropEventArgs : EventArgs
    {
        public InputData inputData { get; private set; }
        public double ValveDiameter { get; private set; }
        public double ValveSaddleWidth { get; private set; }
        public double PressureDrop { get; set; }
        public CalculatePressureDropEventArgs(InputData inputData, double ValveDiameter, double ValveSaddleWidth)
        {
            this.inputData = inputData;
            this.ValveDiameter = ValveDiameter;
            this.ValveSaddleWidth = ValveSaddleWidth;
        }
    }

    public class PreciseSpringEventArgs : EventArgs
    {
        public double CoilDiameter { get; private set; }
        public double CoilCount { get; private set; }
        public double Pitch { get; private set; }
        public double Index { get; private set; }
        public double Draw { get; private set; }
        public SpringParameters springParameters { get; set; }
        public PreciseSpringEventArgs(double CoilDiameter, double CoilCount, double Pitch, double Index, double Draw)
        {
            this.CoilDiameter = CoilDiameter;
            this.CoilCount = CoilCount;
            this.Pitch = Pitch;
            this.Index = Index;
            this.Draw  = Draw;
        }
    }

    public class CalculateReductorEventArgs : EventArgs
    {
        public ReductorCalculationInputData reductorCalculationInputData { get; private set; }
        public ReductorCharacteristics reductorCharacteristics { get; set; }
        public CalculateReductorEventArgs(ReductorCalculationInputData reductorCalculationInputData)
        {
            this.reductorCalculationInputData = reductorCalculationInputData;
        }
    }

    public class SaveResultsEventArgs : EventArgs
    {
        public string FileName { get; private set; } 
        public double MaximumInletPressure { get; private set; }
        public OutletPressureCharacteristic outletPressureCharacteristic { get; private set; }
        public ConsumptionCharacteristic consumptionCharacteristic { get; private set; }
        public SaveResultsEventArgs(
            string FileName,
            double MaximumInletPressure,
            OutletPressureCharacteristic outletPressureCharacteristic,
            ConsumptionCharacteristic consumptionCharacteristic)
        {
            this.FileName = FileName;
            this.MaximumInletPressure = MaximumInletPressure;
            this.outletPressureCharacteristic = outletPressureCharacteristic;
            this.consumptionCharacteristic = consumptionCharacteristic;
        }
    }

    public class SaveReductorEventArgs : EventArgs
    {
        public ReductorCalculationInputData reductorCalculationInputData { get; private set; }
        public string FileName { get; private set; }
        public SaveReductorEventArgs(string FileName, ReductorCalculationInputData reductorCalculationInputData)
        {
            this.FileName = FileName;
            this.reductorCalculationInputData = reductorCalculationInputData;
        }
    }

    public class OpenReductorEventArgs : EventArgs
    {
        public ReductorCalculationInputData reductorCalculationInputData { get; set; }
        public string FileName { get; private set; }
        public OpenReductorEventArgs(string FileName)
        {
            this.FileName = FileName;
        }
    }

    public class OpenExampleEventArgs
    {
        public ReductorCalculationInputData reductorCalculationInputData { get; set; }
        public int ExampleIndex { get; private set; }
        public OpenExampleEventArgs(int ExampleIndex)
        {
            this.ExampleIndex = ExampleIndex;
        }
    }
}
