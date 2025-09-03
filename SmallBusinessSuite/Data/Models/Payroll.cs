using SmallBusinessSuite.Utilities;

namespace SmallBusinessSuite.Data.Models {
    class Payroll {
        public int ID { get; set; }
        public PayPeriod Period { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime Date { get; set; }
        public Expense Expense {get; set; }

        public Payroll(int id, PayPeriod period, decimal totalPaid) {
            Period = period;
            ID = id;
            Date = period != null ? period.End.AddDays(5) : new DateTime(1,1,1);
            TotalPaid = totalPaid;
        }
    }
}
