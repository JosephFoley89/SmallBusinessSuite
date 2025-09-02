namespace SmallBusinessSuite.Data.Models {
    class Payment {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public Employee Employee { get; set; }
        public decimal Amount { get; set; }
        public int PayrollID { get; set; }

        public Payment(int id, DateTime date, Employee employee, decimal amount, int payrollID) {
            ID = id;
            Date = date;
            Employee = employee;
            Amount = amount;
            PayrollID = payrollID;
        }
    }
}
