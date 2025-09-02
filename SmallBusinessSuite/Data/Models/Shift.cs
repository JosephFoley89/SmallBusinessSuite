namespace SmallBusinessSuite.Data.Models {
    internal class Shift {
        public int ID { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public double TimeWorked { get; set; }
        public decimal AmountEarned { get; set; }
        public Client Client { get; set; }
        public int? Parent { get; set;  }

        private decimal CalculateAmountEarned() {
            double timeWorked = TimeWorked + (double)this.Employee.DriveTime;
            decimal amountEarned = (decimal)timeWorked * Employee.HourlyRate;
            return amountEarned;
        }

        public Shift(int id, Employee employee, DateTime date, double timeWorked, Client client) {
            ID = id;
            Employee = employee;
            Date = date;
            TimeWorked = timeWorked;
            AmountEarned = CalculateAmountEarned();
            Client = client;
        }
    }
}
