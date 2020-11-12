using System;
using TypesLibrary;

namespace ModelLibrary
{
    static class Spring
    {
        static double UnloadedLength(double CoilCount, double Index, double CoilDiameter, double Draw)
        {
            return (1.5 + CoilCount) * CoilDiameter + 8 * CoilCount * Math.Pow(Index, 3) / (7.85E10 * CoilDiameter) * Draw;
        }

        static double LoadedLength(double CoilCount, double CoilDiameter)
        {
            return (1.5 + CoilCount) * CoilDiameter;
        }
        static double VaalRatio(double Index)
        {
            return (4 * Index - 1) / (4 * Index - 4) + 0.615 / Index;
        }

        static SpringParameters CalculateSpring(double Draw, double Resiliency, double Index)
        {
            if (Draw > 0)
            {
                double CoilDiameter = 1.6 * Math.Sqrt(VaalRatio(Index) * Index * Draw / 7.36E8);
                double Diameter = CoilDiameter * Index;
                double CoilCount = 7.85E10 * CoilDiameter / (8 * Resiliency * Math.Pow(Index, 3));
                double Pitch = CoilDiameter + Draw / (Resiliency * CoilCount);
                return new SpringParameters(
                    Draw,
                    Resiliency,
                    Index,
                    CoilDiameter,
                    CoilCount,
                    Pitch,
                    Diameter,
                    UnloadedLength(CoilCount, Index, CoilDiameter, Draw),
                    LoadedLength(CoilCount, CoilDiameter));
            }
            else
                return new SpringParameters(0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        public static SpringParameters CalculateBarSpring(BarSpringCalculationInputData barSpringCalculationInputData, ValveStroke valveStroke)
        {
            double Draw = Math.PI * (barSpringCalculationInputData.ValveDiameter + barSpringCalculationInputData.ValveSaddleWidth) *
                barSpringCalculationInputData.ValveSaddleWidth * barSpringCalculationInputData.HermeticPressure +
                Math.PI * Math.Pow(barSpringCalculationInputData.ValveDiameter, 2) / 4 * barSpringCalculationInputData.PressureDrop *
                (valveStroke == ValveStroke.Reverse ? 1 : -1);
            if (Draw <= 0)
                return new SpringParameters(0, 0, 0, 0, 0, 0, 0, 0, 0);
            else 
                return CalculateSpring(Draw, barSpringCalculationInputData.Resiliency, barSpringCalculationInputData.Index);
        }

        public static SpringParameters CalculateMainSpring(double Draw, double Resiliency, double Index)
        {
            return CalculateSpring(Draw, Resiliency, Index);
        }

        public static double CalculateMainSpringDraw(MainSpringDrawCalculationInputData mainSpringDrawCalculationInputData)
        {
            MainSpringDrawCalculationInputData id = mainSpringDrawCalculationInputData;
            Func<double, double> Sqr = x => Math.PI * Math.Pow(x, 2) / 4;
            if (mainSpringDrawCalculationInputData.reductorType.ValveStroke == ValveStroke.Reverse)
            {
                return id.OutletPressure * (Sqr(id.LowPressurePistonDiameter) + Sqr(id.HighPressurePistonDiameter) - Sqr(id.ValveDiameter)) +
                    id.MaximimInletPressure * (Sqr(id.ValveDiameter) - Sqr(id.HighPressurePistonDiameter)) + id.BarStringDraw;
            }
            else
            {
                if (mainSpringDrawCalculationInputData.reductorType.Balanced)
                    return id.OutletPressure * (Sqr(id.ValveDiameter) + Sqr(id.LowPressurePistonDiameter) - Sqr(id.HighPressurePistonDiameter)) +
                        id.MaximimInletPressure * (Sqr(id.HighPressurePistonDiameter) - Sqr(id.ValveDiameter)) + id.BarStringDraw;
                else
                    return id.OutletPressure * Sqr(id.ValveDiameter) + id.MaximimInletPressure * (Sqr(id.HighPressurePistonDiameter) - Sqr(id.ValveDiameter)) +
                        id.BarStringDraw;
            }
        }

        public static SpringParameters PreciseSpring(double CoilDiameter, double CoilCount, double Pitch, double Index, double Draw)
        {
            //double Draw = Math.Pow(CoilDiameter / 1.6, 2) * 7.36E8 / (VaalRatio(Index) * Index);
            double Diameter = CoilDiameter * Index;
            double Resiliency = 7.85E10 * CoilDiameter / (8 * CoilCount * Math.Pow(Index, 3));
            return new SpringParameters(
                Draw,
                Resiliency,
                Index,
                CoilDiameter,
                CoilCount,
                Pitch,
                Diameter,
                UnloadedLength(CoilCount, Index, CoilDiameter, Draw),
                LoadedLength(CoilCount, CoilDiameter));
        }       
    }
}