namespace SmallBusinessSuite.Data.Models {
    internal class Invoice {
        public int ID { get; set; }
        public Client Client { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string InvoiceNumber { get; set; }

        public Invoice(int id, Client client, DateTime date, decimal total, string invoiceNumber) {
            ID = id;
            Client = client;
            Date = date;
            Total = total;
            InvoiceNumber = invoiceNumber;
        }

        public override string ToString() {
            return InvoiceNumber;
        }
    }
}
