namespace TypesLibrary
{
    public delegate double OutletPressureCharacteristic(double InletPressure);

    public delegate double ConsumptionCharacteristic(double InletPressure);

    public struct ReductorCharacteristics
    {
        public OutletPressureCharacteristic outletPressureCharacteristic { get; private set; }
        public ConsumptionCharacteristic consumptionCharacteristic { get; private set; }
        public ReductorCharacteristics(
            OutletPressureCharacteristic outletPressureCharacteristic, 
            ConsumptionCharacteristic consumptionCharacteristic) : this()
        {
            this.outletPressureCharacteristic = outletPressureCharacteristic;
            this.consumptionCharacteristic = consumptionCharacteristic;
        }
    }
}
