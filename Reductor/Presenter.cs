using ViewInterface;
using ModelLibrary;
using System.Xml;

namespace Reductor
{
    class Presenter
    {
        IView View;
        IModel Model;
        public Presenter(IView View, IModel Model)
        {
            this.View = View;
            this.Model = Model;
            View.CalculateBarSpring += View_CalculateBarSpring;
            View.CalculateMainSpring += View_CalculateMainSpring;
            View.PreciseSpring += View_PreciseSpring;
            View.CalculateMainSpringDraw += View_CalculateMainSpringDraw;
            View.CalculatePressureDrop += View_CalculatePressureDrop;
            View.GetValveDiameter += View_GetValveDiameter;
            View.CalculateReductor += View_CalculateReductor;
            View.SaveResults += View_SaveResults;
            View.SaveReductor += View_SaveReductor;
            View.OpenReductor += View_OpenReductor;
            View.OpenExample += View_OpenExample;
        }

        void View_CalculateBarSpring(object sender, CalculateBarSpringEventArgs e)
        {
            e.springParameters = Model.CalculateBarSpring(e.barSpringCalculationInputData, e.valveStroke);
        }
        void View_CalculateMainSpring(object sender, CalculateMainSpringEventArgs e)
        {
            e.springParameters = Model.CalculateMainSpring(e.Draw, e.Resiliency, e.Index);
        }
        void View_PreciseSpring(object sender, PreciseSpringEventArgs e)
        {
            e.springParameters = Model.PreciseSpring(e.CoilDiameter, e.CoilCount, e.Pitch, e.Index, e.Draw);
        }
        void View_CalculateMainSpringDraw(object sender, CalculateMainSpringDrawEventArgs e)
        {
            e.Draw = Model.CalculateMainSpringDraw(e.mainSpringCalculationInputData);
        }
        void View_CalculatePressureDrop(object sender, CalculatePressureDropEventArgs e)
        {
            e.PressureDrop = Model.CalculatePressureDrop(e.inputData, e.ValveDiameter, e.ValveSaddleWidth);
        }
        void View_GetValveDiameter(object sender, GetValveDiameterEventArgs e)
        {
            e.ValveDiameter = Model.CalculateValveDiameter(e.inputData);
        }
        void View_CalculateReductor(object sender, CalculateReductorEventArgs e)
        {
            e.reductorCharacteristics = Model.CalculateReductor(e.reductorCalculationInputData);
        }
        void View_SaveResults(object sender, SaveResultsEventArgs e)
        {
            Model.SaveResults(
                e.FileName, 
                e.MaximumInletPressure, 
                e.outletPressureCharacteristic, 
                e.consumptionCharacteristic);
        }
        void View_SaveReductor(object sender, SaveReductorEventArgs e)
        {
            Model.SaveReductor(e.FileName, e.reductorCalculationInputData);
        }
        void View_OpenReductor(object sender, OpenReductorEventArgs e)
        {
            e.reductorCalculationInputData = Model.OpenReductor(e.FileName);
        }

        private void View_OpenExample(object sender, OpenExampleEventArgs e)
        {
            string s;
            if (e.ExampleIndex == 1)
                s = ExamplesResource.Редуктор_обратного_хода_неуравновешенный_без_запорной_пружины;
            else if (e.ExampleIndex == 2)
                s = ExamplesResource.Редуктор_обратного_хода_неуравновешенный_с_запорной_пружиной;
            else if (e.ExampleIndex == 3)
                s = ExamplesResource.Редуктор_обратного_хода_уравновешенный_без_запорной_пружины;
            else if (e.ExampleIndex == 4)
                s = ExamplesResource.Редуктор_обратного_хода_уравновешенный_с_запорной_пружиной;
            else if (e.ExampleIndex == 5)
                s = ExamplesResource.Редуктор_прямого_хода_с_двумя_полостями_без_запорной_пружины;
            else if (e.ExampleIndex == 6)
                s = ExamplesResource.Редуктор_прямого_хода_с_двумя_полостями_c_запорной_пружиной;
            else if (e.ExampleIndex == 7)
                s = ExamplesResource.Редуктор_прямого_хода_с_четырьмя_полостями_без_запорной_пружины;
            else
                s = ExamplesResource.Редуктор_прямого_хода_с_четырьмя_полостями_c_запорной_пружиной;
            var XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(s);
            e.reductorCalculationInputData = Model.OpenReductor(XmlDoc);
        }
    }
}
