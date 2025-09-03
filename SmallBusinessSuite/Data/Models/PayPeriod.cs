namespace SmallBusinessSuite.Data.Models {
    class PayPeriod {
        public int ID;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Period { get; set; }

        public PayPeriod(int id, DateTime start, DateTime end) {
            ID = id;
            Start = start;
            End = end;
            Period = GenerateTimeSpan();
        }

        private TimeSpan GenerateTimeSpan() {
            return End.Subtract(Start);
        }

        public override string ToString() {
            return $"{Start.ToString("MM/dd/yy")} - {End.ToString("MM/dd/yy")}";
        }
    }
}
