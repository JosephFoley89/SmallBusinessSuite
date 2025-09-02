namespace SmallBusinessSuite.Data.Models {
    internal class InvoiceItem {
        public int ID { get; set; }
        public string WorkOrder { get; set; }
        public string Description { get; set; }
        public double Hours { get; set;  }
        public decimal Rate { get; set; }
        public decimal Amount { get; set;  }
        public Invoice Parent { get; set; }

        public InvoiceItem(int id, string workOrder, string description, double hours, decimal rate, decimal amount, Invoice parent) {
            ID = id;
            WorkOrder = workOrder;
            Description = description;
            Hours = hours;
            Rate = rate;
            Amount = amount;
            Parent = parent;
        }
    }
}
