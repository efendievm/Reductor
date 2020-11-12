namespace TypesLibrary
{
    public struct ReductorType
    {
        public ValveStroke ValveStroke { get; set; }
        public bool Balanced { get; set; }
        public bool BarSpringAvaliable { get; set; }
        public ReductorType(ValveStroke ValveStroke, bool Balanced, bool BarSpringAvaliable) : this()
        {
            this.ValveStroke = ValveStroke;
            this.Balanced = Balanced;
            this.BarSpringAvaliable = BarSpringAvaliable;
        }
    }

    public enum ValveStroke { Straight, Reverse }
}
