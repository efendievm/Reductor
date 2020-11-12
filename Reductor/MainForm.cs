using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TypesLibrary;
using ViewInterface;
using ZedGraph;

namespace Reductor
{
    public partial class MainForm : Form, IView
    {
        public MainForm()
        {
            InitializeComponent();
            List<Image> imageList = new List<Image>() {
                ImagesResource._1,
                ImagesResource._2,
                ImagesResource._3,
                ImagesResource._4,
                ImagesResource._5,
                ImagesResource._6,
                ImagesResource._7,
                ImagesResource._8
            };
            List<string> stringList = new List<string>() {
                "Неуравновешенный редуктор обратного хода без запорной пружины",
                "Неуравновешенный редуктор обратного хода с запорной пружиной",
                "Уравновешенный редуктор обратного хода без запорной пружины",
                "Уравновешенный редуктор обратного хода с запорной пружиной",
                "Редуктор прямого хода без запорной пружины с двумя полостями",
                "Редуктор прямого хода с запорной пружиной с двумя полостями",
                "Редуктор прямого хода без запорной пружины с четырьмя полостями",
                "Редуктор прямого хода с запорной пружиной с четырьмя полостями"
            };
            ReductorForms = new List<ReductorForm>();
            var menuItem = new ToolStripMenuItem("Пример");
            MainMenu.Items.Add(menuItem);
            for (int i = 0; i < 8; i++)
            {
                ReductorForm reductorForm = new ReductorForm
                {
                    TopLevel = false,
                    Parent = this,
                    BackgroundImage = imageList[i],
                    Text = stringList[i]
                };
                ReductorForms.Add(reductorForm);
                menuItem.DropDownItems.Add(stringList[i], null, (sender, e) => 
                {
                    int Index = menuItem.DropDownItems.IndexOf((ToolStripItem)sender) + 1;
                    OpenExampleEventArgs Args = new OpenExampleEventArgs(Index);
                    OpenExample(this, Args);
                    SetInputData(Args.reductorCalculationInputData);
                    InvokeReductorCalculation();
                });
            }
            Action<GraphPane, string> AdjustGraph = (GraphPane, YAxisName) =>
            {
                GraphPane.Title.IsVisible = false;
                GraphPane.IsFontsScaled = false;
                GraphPane.XAxis.Title.Text = "Давление на входе, МПа";
                GraphPane.XAxis.Title.FontSpec.IsBold = false;
                GraphPane.XAxis.MajorGrid.IsVisible = true;
                GraphPane.XAxis.MinorGrid.IsVisible = true;
                GraphPane.YAxis.Title.Text = YAxisName;
                GraphPane.YAxis.Title.FontSpec.IsBold = false;
                GraphPane.YAxis.MajorGrid.IsVisible = true;
                GraphPane.YAxis.MinorGrid.IsVisible = true;
            };
            AdjustGraph(OutletPressureChart.GraphPane, "Давление на выходе, МПа");
            AdjustGraph(ConsumptionChart.GraphPane, "Расход, кг/с");
        }

        private List<ReductorForm> ReductorForms;
        private InputData inputData;
        private ReductorType reductorType;
        private double ValveDiameter;
        private double ValveSaddleWidth;
        private double HighPressurePistonDiameter;
        private double LowPressurePistonDiameter;
        private ReductorCharacteristics reductorCharacteristics;

        private BarSpringParameters FormBarSpringParameters()
        {
            if (reductorType.BarSpringAvaliable)
                return new BarSpringParameters(
                    Convert.ToDouble(BarSpringDrawTextBox.Text) * 1E3,
                    Convert.ToDouble(BarSpringResiliencyTextBox.Text) * 1E3,
                    Convert.ToDouble(BarSpringIndexTextBox.Text),
                    Convert.ToDouble(BarSpringCoilDiameterTextBox.Text) * 1E-3,
                    Convert.ToDouble(BarSpringCoilCountTextBox.Text),
                    Convert.ToDouble(BarSpringPitchTextBox.Text) * 1E-3,
                    Convert.ToDouble(BarSpringDiameterTextBox.Text) * 1E-3,
                    Convert.ToDouble(UnloadedBarSpringLengthTextBox.Text) * 1E-3,
                    Convert.ToDouble(LoadedBarSpringLengthTextBox.Text) * 1E-3,
                    Convert.ToDouble(PressureDropTextBox.Text) * 1E6,
                    Convert.ToDouble(HermeticPressureTextBox.Text));
            else
                return new BarSpringParameters(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
        private MainSpringParameters FormMainSpringParameters()
        {
            return new MainSpringParameters(
                Convert.ToDouble(MainSpringDrawTextBox.Text) * 1E3,
                Convert.ToDouble(MainSpringResiliencyTextBox.Text) * 1E3,
                Convert.ToDouble(MainSpringIndexTextBox.Text),
                Convert.ToDouble(MainSpringCoilDiameterTextBox.Text) * 1E-3,
                Convert.ToDouble(MainSpringCoilCountTextBox.Text),
                Convert.ToDouble(MainSpringPitchTextBox.Text) * 1E-3,
                Convert.ToDouble(MainSpringDiameterTextBox.Text) * 1E-3,
                Convert.ToDouble(UnloadedMainSpringLengthTextBox.Text) * 1E-3,
                Convert.ToDouble(LoadedMainSpringLengthTextBox.Text) * 1E-3);
        }
        private void InvokeReductorCalculation()
        {
            CalculateReductorEventArgs calculateReductorEventArgs = new CalculateReductorEventArgs(new ReductorCalculationInputData(
                inputData,
                reductorType,
                FormBarSpringParameters(),
                FormMainSpringParameters(),
                ValveSaddleWidth,
                ValveDiameter,
                HighPressurePistonDiameter,
                LowPressurePistonDiameter));
            CalculateReductor(this, calculateReductorEventArgs);
            reductorCharacteristics = calculateReductorEventArgs.reductorCharacteristics;
            ResultsGridView.RowCount = (int)(inputData.MaximumInletPressure / 0.1E6 + 1);
            int RowIndex = ResultsGridView.RowCount - 1;
            int i = 0;
            double InletPressure = 0;
            double OutletPressure = 0;
            double Consumption = 0;
            var InletPressures = new List<double>();
            var OutletPressures = new List<double>();
            var Consumptions = new List<double>();
            bool AddGridView = true;
            while (InletPressure <= inputData.MaximumInletPressure)
            {
                OutletPressure = reductorCharacteristics.outletPressureCharacteristic(InletPressure);
                Consumption = reductorCharacteristics.consumptionCharacteristic(InletPressure);
                if (AddGridView)
                {
                    ResultsGridView.Rows[RowIndex].Cells[0].Value = (InletPressure * 1E-6).ToString("0.###");
                    ResultsGridView.Rows[RowIndex].Cells[1].Value = (OutletPressure * 1E-6).ToString("0.###");
                    ResultsGridView.Rows[RowIndex].Cells[2].Value = Consumption.ToString("0.###");
                    RowIndex--;
                }
                i++;
                if (i == 100)
                {
                    i = 0;
                    AddGridView = true;
                }
                else
                    AddGridView = false;
                InletPressures.Add(InletPressure * 1E-6);
                OutletPressures.Add(OutletPressure * 1E-6);
                Consumptions.Add(Consumption);
                InletPressure += 1E3;
            }
            OutletPressureChart.GraphPane.CurveList.Clear();
            OutletPressureChart.GraphPane.AddCurve("", InletPressures.ToArray(), OutletPressures.ToArray(), Color.Navy, SymbolType.None);
            OutletPressureChart.GraphPane.AxisChange();
            OutletPressureChart.Invalidate();
            ConsumptionChart.GraphPane.CurveList.Clear();
            ConsumptionChart.GraphPane.AddCurve("", InletPressures.ToArray(), Consumptions.ToArray(), Color.Navy, SymbolType.None);
            ConsumptionChart.GraphPane.AxisChange();
            ConsumptionChart.Invalidate();
        }
        private void ApplyInputDataButton_Click(object sender, EventArgs e)
        {
            inputData = new InputData(
                Convert.ToDouble(MaximumInletPressureTextBox.Text) * Math.Pow(10, 6), 
                Convert.ToDouble(MinimumInletPressureTextBox.Text) * Math.Pow(10, 6), 
                Convert.ToDouble(OutletPressureTextBox.Text) * Math.Pow(10, 6), 
                Convert.ToDouble(InitialInletTemperatureTextBox.Text), 
                Convert.ToDouble(ConsumptionTextBox.Text));
            GetValveDiameterEventArgs getValveDiameterEventArgs = new GetValveDiameterEventArgs(inputData);
            GetValveDiameter(this, getValveDiameterEventArgs);
            ValveDiameterCalculatedTextBox.Text = (getValveDiameterEventArgs.ValveDiameter * 1E3).ToString("0.###");
            if (ValveDiameterAppliedTextBox.Text == "")
                ValveDiameterAppliedTextBox.Text = ValveDiameterCalculatedTextBox.Text;
            reductorType = new ReductorType(
                StraightRadioButton.Checked ? ValveStroke.Straight : ValveStroke.Reverse,
                BalancedRadioButton.Checked,
                BarSpringAvaibleRadioButton.Checked);
        }
        private void ClearControlsTextBoxes(Control control)
        {
            if (control is TextBox)
                (control as TextBox).Text = "";
            else
                foreach (Control _control in control.Controls)
                    ClearControlsTextBoxes(_control);
        }        
        private void BalancedRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (StraightRadioButton.Checked)
            {
                HighPressurePistonDiameterTextBox.Enabled = true;
                LowPressurePistonDiameterTextBox.Enabled = BalancedRadioButton.Checked;
                if (!BalancedRadioButton.Checked)
                {
                    LowPressurePistonDiameter = 0;
                    ClearControlsTextBoxes(LowPressurePistonDiameterTextBox);
                }
            }
            else
            {
                LowPressurePistonDiameterTextBox.Enabled = true;
                HighPressurePistonDiameterTextBox.Enabled = BalancedRadioButton.Checked;
                if (!BalancedRadioButton.Checked)
                {
                    HighPressurePistonDiameter = 0;
                    ClearControlsTextBoxes(HighPressurePistonDiameterTextBox);
                }
            }
            for (int i = 0; i < 8; i++)
                if (ReductorForms[i].Visible)
                    ShowReductorButton_Click(this, new EventArgs());
        }
        private void StraightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            BalancedRadioButton_CheckedChanged(this, new EventArgs());
            for (int i = 0; i < 8; i++)
                if (ReductorForms[i].Visible)
                    ShowReductorButton_Click(this, new EventArgs());
            BalancedLabel.Text = StraightRadioButton.Checked ? "Число полостей" : "Уравновешенный";
            BalancedRadioButton.Text = StraightRadioButton.Checked ? "Четыре" : "Да";
            UnbalancedRadioButton.Text = StraightRadioButton.Checked ? "Два" : "Нет";
        }
        private void BarSpringAvaibleRadioButton_CheckedChanged(object sender, EventArgs e)
        {    
            if (!BarSpringAvaibleRadioButton.Checked)
            {
                ClearControlsTextBoxes(BarSpringPage);
                BarSpringPage.Enabled = false;
                SpringsTabControl.SelectedIndex = 1;
            }
            else
                BarSpringPage.Enabled = true;
            for (int i = 0; i < 8; i++)
                if (ReductorForms[i].Visible)
                    ShowReductorButton_Click(this, new EventArgs());
        }
        private void GetMainStringDraw()
        {
            CalculateMainSpringDrawEventArgs calculateMainSpringDrawEventArgs = new CalculateMainSpringDrawEventArgs(
                new MainSpringDrawCalculationInputData(
                    reductorType,
                    Convert.ToDouble(MaximumInletPressureTextBox.Text) * 1E6,
                    Convert.ToDouble(OutletPressureTextBox.Text) * 1E6,
                    ValveDiameter,
                    HighPressurePistonDiameter,
                    LowPressurePistonDiameter,
                    BarSpringAvaibleRadioButton.Checked ? Convert.ToDouble(BarSpringDrawTextBox.Text) * 1E3 : 0));
            CalculateMainSpringDraw(this, calculateMainSpringDrawEventArgs);
            MainSpringDrawTextBox.Text = (calculateMainSpringDrawEventArgs.Draw * 1E-3).ToString("0.###");
        }
        private void ApplyValveAndPistonsParametersButton_Click(object sender, EventArgs e)
        {
            ValveDiameter = Convert.ToDouble(ValveDiameterAppliedTextBox.Text) * 1E-3;
            ValveSaddleWidth = Convert.ToDouble(ValveSaddleWidthTextBox.Text) * 1E-3;
            if (HighPressurePistonDiameterTextBox.Enabled)
                HighPressurePistonDiameter = Convert.ToDouble(HighPressurePistonDiameterTextBox.Text) * 1E-3;
            if (LowPressurePistonDiameterTextBox.Enabled)
                LowPressurePistonDiameter = Convert.ToDouble(LowPressurePistonDiameterTextBox.Text) * 1E-3;
            if (BarSpringAbsenceRadioButton.Checked)
                GetMainStringDraw();
            CalculatePressureDropEventArgs calculatePressureDropEventArgs = new CalculatePressureDropEventArgs(
                inputData, ValveDiameter, ValveSaddleWidth);
            CalculatePressureDrop(this, calculatePressureDropEventArgs);
            PressureDropTextBox.Text = (calculatePressureDropEventArgs.PressureDrop * 1E-6).ToString("0.###");
        }
        private void CalculateBarSpringButton_Click(object sender, EventArgs e)
        {
            CalculateBarSpringEventArgs calculateBarSpringEventArgs = new CalculateBarSpringEventArgs(
                new BarSpringCalculationInputData(
                    Convert.ToDouble(PressureDropTextBox.Text) * 1E6,
                    ValveSaddleWidth,
                    ValveDiameter, 
                    Convert.ToDouble(HermeticPressureTextBox.Text), 
                    Convert.ToDouble(BarSpringResiliencyTextBox.Text) * 1E3,
                    Convert.ToDouble(BarSpringIndexTextBox.Text)),
                reductorType.ValveStroke);
            CalculateBarSpring(this, calculateBarSpringEventArgs);
            SpringParameters springParameters = calculateBarSpringEventArgs.springParameters;
            BarSpringCoilDiameterTextBox.Text = (springParameters.CoilDiameter * 1E3).ToString("0.###");
            BarSpringCoilCountTextBox.Text = (springParameters.CoilCount).ToString("0.###");
            BarSpringPitchTextBox.Text = (springParameters.Pitch * 1E3).ToString("0.###");
            BarSpringDiameterTextBox.Text = (springParameters.Diameter * 1E3).ToString("0.###");
            BarSpringDrawTextBox.Text = (springParameters.Draw * 1E-3).ToString("0.###");
            UnloadedBarSpringLengthTextBox.Text = (springParameters.UnloadedLength * 1E3).ToString("0.###");
            LoadedBarSpringLengthTextBox.Text = (springParameters.LoadedLength * 1E3).ToString("0.###");
            GetMainStringDraw();
        }
        private void CalculateMainSpringButton_Click(object sender, EventArgs e)
        {
            CalculateMainSpringEventArgs calculateMainSpringEventArgs = new CalculateMainSpringEventArgs(
                Convert.ToDouble(MainSpringDrawTextBox.Text) * 1E3,
                Convert.ToDouble(MainSpringResiliencyTextBox.Text) * 1E3,
                Convert.ToDouble(MainSpringIndexTextBox.Text));
            CalculateMainSpring(this, calculateMainSpringEventArgs);
            SpringParameters springParameters = calculateMainSpringEventArgs.springParameters;
            MainSpringCoilDiameterTextBox.Text = (springParameters.CoilDiameter * 1E3).ToString("0.###");
            MainSpringCoilCountTextBox.Text = (springParameters.CoilCount).ToString("0.###");
            MainSpringPitchTextBox.Text = (springParameters.Pitch * 1E3).ToString("0.###");
            MainSpringDiameterTextBox.Text = (springParameters.Diameter * 1E3).ToString("0.###");
            UnloadedMainSpringLengthTextBox.Text = (springParameters.UnloadedLength * 1E3).ToString("0.###");
            LoadedMainSpringLengthTextBox.Text = (springParameters.LoadedLength * 1E3).ToString("0.###");
            InvokeReductorCalculation();
        }
        private void SpringPreciseButton_Click(object sender, EventArgs e)
        {
            bool BarSpringPresied = sender == BarSpringPreciseButton;
            PreciseSpringEventArgs preciseSpringEventArgs = new PreciseSpringEventArgs(
                Convert.ToDouble((BarSpringPresied ? BarSpringCoilDiameterTextBox : MainSpringCoilDiameterTextBox).Text) * 1E-3,
                Convert.ToDouble((BarSpringPresied ? BarSpringCoilCountTextBox    : MainSpringCoilCountTextBox).Text),
                Convert.ToDouble((BarSpringPresied ? BarSpringPitchTextBox        : MainSpringPitchTextBox).Text) * 1E-3,
                Convert.ToDouble((BarSpringPresied ? BarSpringIndexTextBox        : MainSpringIndexTextBox).Text),
                Convert.ToDouble((BarSpringPresied ? BarSpringDrawTextBox         : MainSpringDrawTextBox).Text) * 1E3);
            PreciseSpring(this, preciseSpringEventArgs);
            SpringParameters springParameters = preciseSpringEventArgs.springParameters;
            (BarSpringPresied ? BarSpringDrawTextBox           : MainSpringDrawTextBox)          .Text = (springParameters.Draw * 1E-3).ToString("0.###");
            (BarSpringPresied ? BarSpringDiameterTextBox       : MainSpringDiameterTextBox)      .Text = (springParameters.Diameter * 1E3).ToString("0.###");
            (BarSpringPresied ? BarSpringResiliencyTextBox     : MainSpringResiliencyTextBox)    .Text = (springParameters.Resiliency * 1E-3).ToString("0.###");
            (BarSpringPresied ? UnloadedBarSpringLengthTextBox : UnloadedMainSpringLengthTextBox).Text = (springParameters.UnloadedLength * 1E3).ToString("0.###");
            (BarSpringPresied ? LoadedBarSpringLengthTextBox   : LoadedMainSpringLengthTextBox).Text = (springParameters.LoadedLength * 1E3).ToString("0.###");
            if (sender == BarSpringPreciseButton)
                GetMainStringDraw();
            else
                InvokeReductorCalculation();
        }
        private void SpringsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BarSpringAbsenceRadioButton.Checked)
                SpringsTabControl.SelectedIndex = 1;
        }
        private void SaveResultsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SaveResults(this, new SaveResultsEventArgs(
                    saveDialog.FileName, 
                    inputData.MaximumInletPressure, 
                    reductorCharacteristics.outletPressureCharacteristic, 
                    reductorCharacteristics.consumptionCharacteristic));   
        }
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SaveReductor(this, new SaveReductorEventArgs(saveDialog.FileName, new ReductorCalculationInputData(
                    inputData,
                    reductorType,
                    FormBarSpringParameters(),
                    FormMainSpringParameters(),
                    ValveSaddleWidth,
                    ValveDiameter,
                    HighPressurePistonDiameter,
                    LowPressurePistonDiameter)));                       
        }
        private void SetInputData(ReductorCalculationInputData id)
        {
            Action<Control> SetNullInTextBox = null;
            SetNullInTextBox = control =>
            {
                var list = new List<TextBox>();
                foreach (Control c in control.Controls)
                {
                    if (c is TextBox)
                        ((TextBox)c).Text = "";
                    else if (c.Controls.Count != 0)
                        SetNullInTextBox(c);
                }
            };
            SetNullInTextBox(this);
            MaximumInletPressureTextBox.Text = (id.inputData.MaximumInletPressure * 1E-6).ToString("0.###");
            MinimumInletPressureTextBox.Text = (id.inputData.MinimumInletPressure * 1E-6).ToString("0.###");
            OutletPressureTextBox.Text = (id.inputData.OutletPressure * 1E-6).ToString("0.###");
            InitialInletTemperatureTextBox.Text = id.inputData.InitialInletTemperature.ToString("0.###");
            ConsumptionTextBox.Text = id.inputData.Consumption.ToString("0.###");
            StraightRadioButton.Checked = id.reductorType.ValveStroke == ValveStroke.Straight;
            BalancedRadioButton.Checked = id.reductorType.Balanced;
            BarSpringAvaibleRadioButton.Checked = id.reductorType.BarSpringAvaliable;
            ReverseRadioButton.Checked = !StraightRadioButton.Checked;
            UnbalancedRadioButton.Checked = !BalancedRadioButton.Checked;
            BarSpringAbsenceRadioButton.Checked = !BarSpringAvaibleRadioButton.Checked;
            StraightRadioButton_CheckedChanged(this, new EventArgs());
            BalancedRadioButton_CheckedChanged(this, new EventArgs());
            BarSpringAvaibleRadioButton_CheckedChanged(this, new EventArgs());
            ApplyInputDataButton_Click(this, new EventArgs());
            ValveSaddleWidthTextBox.Text = (id.ValveSaddleWidth * 1E3).ToString("0.###");
            ValveDiameterAppliedTextBox.Text = (id.ValveDiameter * 1E3).ToString("0.###");
            if (HighPressurePistonDiameterTextBox.Enabled)
                HighPressurePistonDiameterTextBox.Text = (id.HighPressurePistonDiameter * 1E3).ToString("0.###");
            if (LowPressurePistonDiameterTextBox.Enabled)
                LowPressurePistonDiameterTextBox.Text = (id.LowPressurePistonDiameter * 1E3).ToString("0.###");
            ApplyValveAndPistonsParametersButton_Click(this, new EventArgs());
            if (id.reductorType.BarSpringAvaliable)
            {
                PressureDropTextBox.Text = (id.BarSpringParameters.PressureDrop * 1E-6).ToString("#.###");
                HermeticPressureTextBox.Text = id.BarSpringParameters.HermeticPressure.ToString("0.##E0");
                BarSpringDrawTextBox.Text = (id.BarSpringParameters.Draw * 1E-3).ToString("0.###");
                BarSpringIndexTextBox.Text = (id.BarSpringParameters.Index).ToString("0.###");
                BarSpringCoilDiameterTextBox.Text = (id.BarSpringParameters.CoilDiameter * 1E3).ToString("0.###");
                BarSpringCoilCountTextBox.Text = (id.BarSpringParameters.CoilCount).ToString("0.###");
                BarSpringPitchTextBox.Text = (id.BarSpringParameters.Pitch * 1E3).ToString("0.###");
                BarSpringDiameterTextBox.Text = (id.BarSpringParameters.Diameter * 1E3).ToString("0.###");
                BarSpringResiliencyTextBox.Text = (id.BarSpringParameters.Resiliency * 1E-3).ToString("0.###");
                UnloadedBarSpringLengthTextBox.Text = (id.BarSpringParameters.UnloadedLength * 1E3).ToString("0.###");
                LoadedBarSpringLengthTextBox.Text = (id.BarSpringParameters.LoadedLength * 1E3).ToString("0.###");
            }
            else
                SetNullInTextBox(BarSpringPanel);
            MainSpringDrawTextBox.Text = (id.MainSpringParameters.Draw * 1E-3).ToString("0.###");
            MainSpringIndexTextBox.Text = (id.MainSpringParameters.Index).ToString("0.###");
            MainSpringCoilDiameterTextBox.Text = (id.MainSpringParameters.CoilDiameter * 1E3).ToString("0.###");
            MainSpringCoilCountTextBox.Text = (id.MainSpringParameters.CoilCount).ToString("0.###");
            MainSpringPitchTextBox.Text = (id.MainSpringParameters.Pitch * 1E3).ToString("0.###");
            MainSpringDiameterTextBox.Text = (id.MainSpringParameters.Diameter * 1E3).ToString("0.###");
            MainSpringResiliencyTextBox.Text = (id.MainSpringParameters.Resiliency * 1E-3).ToString("0.###");
            UnloadedMainSpringLengthTextBox.Text = (id.MainSpringParameters.UnloadedLength * 1E3).ToString("0.###");
            LoadedMainSpringLengthTextBox.Text = (id.MainSpringParameters.LoadedLength * 1E3).ToString("0.###");
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                OpenReductorEventArgs openReductorEventArgs = new OpenReductorEventArgs(openDialog.FileName);
                OpenReductor(this, openReductorEventArgs);
                SetInputData(openReductorEventArgs.reductorCalculationInputData);
                InvokeReductorCalculation();
            }
        }
        private void ShowReductorButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
                ReductorForms[i].Visible = false;
            if (ReverseRadioButton.Checked)
            {
                if (UnbalancedRadioButton.Checked)
                {
                    if (BarSpringAbsenceRadioButton.Checked)
                        ReductorForms[0].ShowReductor();
                    else
                        ReductorForms[1].ShowReductor();
                }
                else
                {
                    if (BarSpringAbsenceRadioButton.Checked)
                        ReductorForms[2].ShowReductor();
                    else
                        ReductorForms[3].ShowReductor();
                }
            }
            else
            {
                if (UnbalancedRadioButton.Checked)
                {
                    if (BarSpringAbsenceRadioButton.Checked)
                        ReductorForms[4].ShowReductor();
                    else
                        ReductorForms[5].ShowReductor();
                }
                else
                {
                    if (BarSpringAbsenceRadioButton.Checked)
                        ReductorForms[6].ShowReductor();
                    else
                        ReductorForms[7].ShowReductor();
                }
            }
        }

        public event EventHandler<CalculateBarSpringEventArgs> CalculateBarSpring;
        public event EventHandler<CalculateMainSpringEventArgs> CalculateMainSpring;
        public event EventHandler<CalculateMainSpringDrawEventArgs> CalculateMainSpringDraw;
        public event EventHandler<CalculatePressureDropEventArgs> CalculatePressureDrop;
        public event EventHandler<GetValveDiameterEventArgs> GetValveDiameter;
        public event EventHandler<PreciseSpringEventArgs> PreciseSpring;
        public event EventHandler<CalculateReductorEventArgs> CalculateReductor;
        public event EventHandler<SaveResultsEventArgs> SaveResults;
        public event EventHandler<SaveReductorEventArgs> SaveReductor;
        public event EventHandler<OpenReductorEventArgs> OpenReductor;
        public event EventHandler<OpenExampleEventArgs> OpenExample;
    }
}