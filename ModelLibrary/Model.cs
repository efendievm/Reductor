using System;
using System.Linq;
using TypesLibrary;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;

namespace ModelLibrary
{
    public class Model : IModel
    {
        public SpringParameters CalculateBarSpring(BarSpringCalculationInputData barStringCalculationInputData, ValveStroke valveStroke)
        {
            return Spring.CalculateBarSpring(barStringCalculationInputData, valveStroke);
        }
        public SpringParameters CalculateMainSpring(double Draw, double Resiliency, double Index)
        {
            return Spring.CalculateMainSpring(Draw, Resiliency, Index);
        }
        public SpringParameters PreciseSpring(double CoilDiameter, double CoilCount, double Pitch, double Index, double Draw)
        {
            return Spring.PreciseSpring(CoilDiameter, CoilCount, Pitch, Index, Draw);
        }
        public double CalculateMainSpringDraw(MainSpringDrawCalculationInputData mainSpringDrawCalculationInputData)
        {
            return Spring.CalculateMainSpringDraw(mainSpringDrawCalculationInputData);
        }
        private const double k = 1.4;
        private const double mu = 0.7;
        private const double n = 1.25;
        private const double R = 296.945;
        private Func<double, double> GetA(double OutletPressure)
        {
            return InletPressure =>
                {
                    double PressureDrop = OutletPressure / InletPressure;
                    if ((PressureDrop) > Math.Pow((2 / (k + 1)), k / (k - 1)))
                        return Math.Sqrt(2 * k / (k - 1) * (Math.Pow(PressureDrop, 2 / k) - Math.Pow(PressureDrop, (k + 1) / k)));
                    else
                        return Math.Sqrt(2 * k / (k + 1) * Math.Pow(2 / (k + 1), 2 / (k - 1)));
                };
        }
        private Func<double, double> GetTemperature(double InitialPressure, double InitialTemperature)
        {
            return InletPressure => InitialTemperature * Math.Pow(InletPressure / InitialPressure, (n - 1) / n);
        }
        private double MinimumInletPressure(InputData inputData, double ValveDiameter, double ValveSaddleWidth)
        {
            var id = inputData;
            Func<double, double> Temperature = GetTemperature(id.MaximumInletPressure, id.InitialInletTemperature);
            Func<double, double> A = GetA(id.OutletPressure);
            double MaxValveStroke = Math.Pow(ValveDiameter, 2) / (4 * (ValveDiameter + ValveSaddleWidth));
            Func<double, double> MinimumInletPressureRoot = InletPressure =>
            {
                return id.Consumption / (mu * Math.PI * (ValveDiameter + ValveSaddleWidth) *
                    A(InletPressure) * MaxValveStroke / Math.Sqrt(R * Temperature(InletPressure))) - InletPressure;
            };
            double LeftPressure = id.OutletPressure;
            double RightPressure = id.MaximumInletPressure;
            double MiddlePressure = 0;
            while ((RightPressure - LeftPressure) > 1E3)
            {
                MiddlePressure = (LeftPressure + RightPressure) / 2;
                if (MinimumInletPressureRoot(MiddlePressure) > 0)
                    LeftPressure = MiddlePressure;
                else
                    RightPressure = MiddlePressure;
            }
            return MiddlePressure;
        }

        public double CalculatePressureDrop(InputData inputData, double ValveDiameter, double ValveSaddleWidth)
        {
            return MinimumInletPressure(inputData, ValveDiameter, ValveSaddleWidth) - inputData.OutletPressure;
        }
        public double CalculateValveDiameter(InputData inputData)
        {
            InputData id = inputData;
            Func<double, double> Temperature = GetTemperature(id.MaximumInletPressure, id.InitialInletTemperature);
            Func<double, double> A = GetA(id.OutletPressure);
            double ThrottleSquare = inputData.Consumption / (mu * A(id.MinimumInletPressure) * id.MinimumInletPressure /
                Math.Sqrt(R * Temperature(id.MinimumInletPressure)));
            return Math.Sqrt(4 * ThrottleSquare / Math.PI);
        }
        public ReductorCharacteristics CalculateReductor(ReductorCalculationInputData reductorCalculationInputData)
        {
            var id = reductorCalculationInputData;
            Func<double, double> Temperature = GetTemperature(id.inputData.MaximumInletPressure, id.inputData.InitialInletTemperature);
            Func<double, double> A = GetA(id.inputData.OutletPressure);
            double MaxValveStroke = Math.Pow(id.ValveDiameter, 2) / (4 * (id.ValveDiameter + id.ValveSaddleWidth));
            Func<double, double> ValveStroke = InletPressure =>
            {
                double h = id.inputData.Consumption / (mu * Math.PI * (id.ValveDiameter + id.ValveSaddleWidth) *
                    A(InletPressure) * InletPressure / Math.Sqrt(R * Temperature(InletPressure)));
                return Math.Min(h, MaxValveStroke);
            };
            double minimumInletPressure = MinimumInletPressure(id.inputData, id.ValveDiameter, id.ValveSaddleWidth);
            double Draw = id.MainSpringParameters.Draw;
            double Resiliency = id.MainSpringParameters.Resiliency;
            if (id.reductorType.BarSpringAvaliable)
            {
                Draw -= id.BarSpringParameters.Draw;
                Resiliency += id.BarSpringParameters.Resiliency;
            }
            double OutletPressureArea;
            double InletPressureArea;
            if (id.reductorType.ValveStroke == TypesLibrary.ValveStroke.Reverse)
            {
                OutletPressureArea = Math.PI * (Math.Pow(id.LowPressurePistonDiameter, 2) - Math.Pow(id.ValveDiameter, 2)) / 4;
                InletPressureArea = Math.PI * Math.Pow(id.ValveDiameter, 2) / 4;
                if (id.reductorType.Balanced)
                {
                    OutletPressureArea += Math.PI * Math.Pow(id.HighPressurePistonDiameter, 2) / 4;
                    InletPressureArea -= Math.PI * Math.Pow(id.HighPressurePistonDiameter, 2) / 4;
                }
            }
            else
            {
                OutletPressureArea = Math.PI * Math.Pow(id.ValveDiameter, 2) / 4;
                InletPressureArea = Math.PI * (Math.Pow(id.HighPressurePistonDiameter, 2) - Math.Pow(id.ValveDiameter, 2)) / 4;
                if (id.reductorType.Balanced)
                    OutletPressureArea += Math.PI * (Math.Pow(id.LowPressurePistonDiameter, 2) - Math.Pow(id.HighPressurePistonDiameter, 2)) / 4;
            }
            double ExtremeOutletPressure = 1 / (OutletPressureArea) * (Draw - minimumInletPressure * InletPressureArea - Resiliency * MaxValveStroke);
            OutletPressureCharacteristic outletPressureCharacteristic = new OutletPressureCharacteristic(
                InletPressure => (InletPressure > minimumInletPressure) ? 
                    1 / (OutletPressureArea) * (Draw - InletPressure * InletPressureArea - Resiliency * ValveStroke(InletPressure)) :
                    ExtremeOutletPressure / minimumInletPressure * InletPressure);
            ConsumptionCharacteristic consumptionCharacteristic = new ConsumptionCharacteristic(
                InletPressure => (InletPressure > id.inputData.OutletPressure) ?
                    mu * Math.PI * (id.ValveDiameter + id.ValveSaddleWidth) * A(InletPressure) * InletPressure / Math.Sqrt(R * Temperature(InletPressure)) * ValveStroke(InletPressure) :
                    0);
            return new ReductorCharacteristics(outletPressureCharacteristic, consumptionCharacteristic);
        }
        
        public void SaveResults(
            string FileName, 
            double MaximumInletPressure, 
            OutletPressureCharacteristic outletPressureCharacteristic, 
            ConsumptionCharacteristic consumptionCharacteristic)
        {
            Excel.Application ExcelApp = new Excel.Application();
            ExcelApp.Visible = false;
            ExcelApp.DisplayAlerts = false;
            Excel.Workbook ExcelWb = ExcelApp.Workbooks.Add();
            Excel.Worksheet ExcelWs = (Excel.Worksheet)ExcelWb.Worksheets.get_Item(1);
            ExcelWs.Range["A:A"].NumberFormat = "@";
            ExcelWs.Range["B:B"].NumberFormat = "@";
            ExcelWs.Range["C:C"].NumberFormat = "@";
            ExcelWs.Cells[1, 1] = "Давление на входе, МПа";
            ExcelWs.Cells[1, 2] = "Давление на выходе, МПа";
            ExcelWs.Cells[1, 3] = "Расход, кг/с";
            int RowIndex = 2;
            double InletPressure = MaximumInletPressure;
            while (InletPressure >= 0)
            {
                ExcelWs.Cells[RowIndex, 1] = InletPressure * 1E-6;
                ExcelWs.Cells[RowIndex, 2] = outletPressureCharacteristic(InletPressure) * 1E-6;
                ExcelWs.Cells[RowIndex, 3] = consumptionCharacteristic(InletPressure);
                RowIndex++;
                InletPressure -= 0.1E6;
            }
            ExcelWs.UsedRange.WrapText = true;
            ExcelWs.UsedRange.Columns.AutoFit();
            ExcelWs.UsedRange.Rows.AutoFit();
            ExcelWb.SaveAs(FileName);
            ExcelWb.Close();
            ExcelApp.Quit();
        }
        
        public void SaveReductor(string FileName, ReductorCalculationInputData reductorCalculationInputData)
        {
            XmlDocument xmlDoc = new XmlDocument();
            var id = reductorCalculationInputData;
            Action<XmlNode, string, string> AddAttribute = (node, name, value) =>
            {
                XmlAttribute attribute = xmlDoc.CreateAttribute(name);
                attribute.Value = value;
                node.Attributes.Append(attribute);
            };
            XmlNode rootNode = xmlDoc.CreateElement("Reductor");
            XmlNode inputData = xmlDoc.CreateElement("InputData");
            AddAttribute(inputData, "MaximumInletPressure", id.inputData.MaximumInletPressure.ToString());
            AddAttribute(inputData, "MinimumInletPressure", id.inputData.MinimumInletPressure.ToString());
            AddAttribute(inputData, "OutletPressure", id.inputData.OutletPressure.ToString());
            AddAttribute(inputData, "InitialInletTemperature", (id.inputData.InitialInletTemperature).ToString());
            AddAttribute(inputData, "Consumption", (id.inputData.Consumption).ToString());
            rootNode.AppendChild(inputData);
            XmlNode reductorType = xmlDoc.CreateElement("ReductorType");
            AddAttribute(reductorType, "ValveStroke", (id.reductorType.ValveStroke == ValveStroke.Reverse) ? "Reverse" : "Straight");
            AddAttribute(reductorType, "Balansed", id.reductorType.Balanced ? "Yes" : "No");
            AddAttribute(reductorType, "BarSpringAvaliable", id.reductorType.BarSpringAvaliable ? "Yes" : "No");
            rootNode.AppendChild(reductorType);
            XmlNode reductorParameters = xmlDoc.CreateElement("ReductorParameters");
            XmlNode valveParameters = xmlDoc.CreateElement("ValveParameters");
            AddAttribute(valveParameters, "ValveDiameter", (id.ValveDiameter).ToString());
            AddAttribute(valveParameters, "ValveSaddleWidth", (id.ValveSaddleWidth).ToString());
            reductorParameters.AppendChild(valveParameters);
            XmlNode pistonParameters = xmlDoc.CreateElement("PistonParameters");
            if (!id.reductorType.Balanced)
            {
                if (id.reductorType.ValveStroke == ValveStroke.Straight)
                    AddAttribute(pistonParameters, "HighPressurePistonDiameter", (id.HighPressurePistonDiameter).ToString());
                else
                    AddAttribute(pistonParameters, "LowPressurePistonDiameter", (id.LowPressurePistonDiameter).ToString());
            }
            else
            {
                AddAttribute(pistonParameters, "HighPressurePistonDiameter", (id.HighPressurePistonDiameter).ToString());
                AddAttribute(pistonParameters, "LowPressurePistonDiameter", (id.LowPressurePistonDiameter).ToString());
            }
            reductorParameters.AppendChild(pistonParameters);
            Action<XmlNode, SpringParameters> SetSpringParameters = (node, spring) =>
                {
                    AddAttribute(node, "Draw", (spring.Draw).ToString());
                    AddAttribute(node, "Resiliency", (spring.Resiliency).ToString());
                    AddAttribute(node, "Index", (spring.Index).ToString());
                    AddAttribute(node, "CoilDiameter", (spring.CoilDiameter).ToString());
                    AddAttribute(node, "CoilCount", (spring.CoilCount).ToString());
                    AddAttribute(node, "Pitch", (spring.Pitch).ToString());
                    AddAttribute(node, "Diameter", (spring.Diameter).ToString());
                    AddAttribute(node, "UnloadedLength", (spring.UnloadedLength).ToString());
                    AddAttribute(node, "LoadedLength", (spring.LoadedLength).ToString());
                };
            XmlNode springParameters = xmlDoc.CreateElement("SpringParameters");
            if (id.reductorType.BarSpringAvaliable)
            {
                XmlNode barSpringParameters = xmlDoc.CreateElement("BarSpringParameters");
                SetSpringParameters(barSpringParameters, id.BarSpringParameters);
                AddAttribute(barSpringParameters, "PressureDrop", (id.BarSpringParameters.PressureDrop).ToString());
                AddAttribute(barSpringParameters, "HermeticPressure", (id.BarSpringParameters.HermeticPressure).ToString());
                reductorParameters.AppendChild(barSpringParameters);
            }
            XmlNode mainSpringParameters = xmlDoc.CreateElement("MainSpringParameters");
            SetSpringParameters(mainSpringParameters, id.MainSpringParameters);
            reductorParameters.AppendChild(mainSpringParameters);
            rootNode.AppendChild(reductorParameters);
            xmlDoc.AppendChild(rootNode);
            xmlDoc.Save(FileName);
        }
        public ReductorCalculationInputData OpenReductor(XmlDocument xmlDoc)
        {
            Func<XmlNode, string, XmlNode> FindNode = (Node, ChildNodeName) => Node.ChildNodes.Cast<XmlNode>().First(x => x.Name == ChildNodeName);
            XmlNode inputDataNode = FindNode(xmlDoc.ChildNodes[0], "InputData");
            InputData inputData = new InputData(
                Convert.ToDouble(inputDataNode.Attributes["MaximumInletPressure"].Value),
                Convert.ToDouble(inputDataNode.Attributes["MinimumInletPressure"].Value),
                Convert.ToDouble(inputDataNode.Attributes["OutletPressure"].Value),
                Convert.ToDouble(inputDataNode.Attributes["InitialInletTemperature"].Value),
                Convert.ToDouble(inputDataNode.Attributes["Consumption"].Value));
            XmlNode reductorTypeNode = FindNode(xmlDoc.ChildNodes[0], "ReductorType");
            ReductorType reductorType = new ReductorType(
                (reductorTypeNode.Attributes["ValveStroke"].Value == "Reverse") ? ValveStroke.Reverse : ValveStroke.Straight,
                reductorTypeNode.Attributes["Balansed"].Value == "Yes",
                reductorTypeNode.Attributes["BarSpringAvaliable"].Value == "Yes");
            XmlNode reductorParametersNode = FindNode(xmlDoc.ChildNodes[0], "ReductorParameters");
            BarSpringParameters barSpringParameters = null;
            if (reductorType.BarSpringAvaliable)
            {
                XmlNode barSpringParametersNode = FindNode(reductorParametersNode, "BarSpringParameters");
                barSpringParameters = new BarSpringParameters(
                    Convert.ToDouble(barSpringParametersNode.Attributes["Draw"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["Resiliency"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["Index"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["CoilDiameter"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["CoilCount"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["Pitch"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["Diameter"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["UnloadedLength"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["LoadedLength"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["PressureDrop"].Value),
                    Convert.ToDouble(barSpringParametersNode.Attributes["HermeticPressure"].Value));
            }
            XmlNode mainSpringParametersNode = FindNode(reductorParametersNode, "MainSpringParameters");
            MainSpringParameters mainSpringParameters = new MainSpringParameters(
                    Convert.ToDouble(mainSpringParametersNode.Attributes["Draw"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["Resiliency"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["Index"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["CoilDiameter"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["CoilCount"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["Pitch"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["Diameter"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["UnloadedLength"].Value),
                    Convert.ToDouble(mainSpringParametersNode.Attributes["LoadedLength"].Value));
            XmlNode valveParametersNode = FindNode(reductorParametersNode, "ValveParameters");
            double ValveSaddleWidth = Convert.ToDouble(valveParametersNode.Attributes["ValveSaddleWidth"].Value);
            double ValveDiameter = Convert.ToDouble(valveParametersNode.Attributes["ValveDiameter"].Value);
            XmlNode pistonParametersNode = FindNode(reductorParametersNode, "PistonParameters");
            double HighPressurePistonDiameter = 0;
            double LowPressurePistonDiameter = 0;
            if (!reductorType.Balanced)
            {
                if (reductorType.ValveStroke == ValveStroke.Straight)
                    HighPressurePistonDiameter = Convert.ToDouble(pistonParametersNode.Attributes["HighPressurePistonDiameter"].Value);
                else
                    LowPressurePistonDiameter = Convert.ToDouble(pistonParametersNode.Attributes["LowPressurePistonDiameter"].Value);
            }
            else
            {
                HighPressurePistonDiameter = Convert.ToDouble(pistonParametersNode.Attributes["HighPressurePistonDiameter"].Value);
                LowPressurePistonDiameter = Convert.ToDouble(pistonParametersNode.Attributes["LowPressurePistonDiameter"].Value);
            }
            return new ReductorCalculationInputData(
                inputData,
                reductorType,
                barSpringParameters,
                mainSpringParameters,
                ValveSaddleWidth,
                ValveDiameter,
                HighPressurePistonDiameter,
                LowPressurePistonDiameter);
        }
        public ReductorCalculationInputData OpenReductor(string FileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FileName);
            return OpenReductor(xmlDoc);
        }
    }
}