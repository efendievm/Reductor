using System.Xml;
using TypesLibrary;

namespace ModelLibrary
{
    public interface IModel
    {
        SpringParameters CalculateBarSpring(BarSpringCalculationInputData barStringCalculationInputData, ValveStroke valveStroke);
        SpringParameters CalculateMainSpring(double Draw, double Resiliency, double Index);
        SpringParameters PreciseSpring(double CoilDiameter, double CoilCount, double Pitch, double Index, double Draw);
        double CalculateMainSpringDraw(MainSpringDrawCalculationInputData mainSpringDrawCalculationInputData);
        double CalculateValveDiameter(InputData inputData);
        double CalculatePressureDrop(InputData inputData, double ValveDiameter, double ValveSaddleWidth);
        ReductorCharacteristics CalculateReductor(ReductorCalculationInputData reductorCalculationInputData);
        void SaveResults(
            string FileName,
            double MaximumInletPressure,
            OutletPressureCharacteristic outletPressureCharacteristic,
            ConsumptionCharacteristic consumptionCharacteristic);
        void SaveReductor(string FileName, ReductorCalculationInputData reductorCalculationInputData);
        ReductorCalculationInputData OpenReductor(string FileName);
        ReductorCalculationInputData OpenReductor(XmlDocument xmlDoc);
    }
}
