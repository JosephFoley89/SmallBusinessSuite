using System.Numerics;

namespace SmallBusinessSuite.Data.Models {
    internal class PaymentReport {
        public Employee Employee { get; set; }
        public Decimal Total { get; set; }
        public int Year { get; set; }

        public PaymentReport(Employee employee, decimal total, int year) {
            Employee = employee;
            Total = total;
            Year = year;
        }
    }
}
