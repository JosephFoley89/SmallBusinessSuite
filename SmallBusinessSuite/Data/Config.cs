namespace SmallBusinessSuite.Data {
    class Config {
        public bool UseDarkMode { get; set; }
        public int RecordLimit { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessEmail { get; set; }
        public string BusinessURL { get; set; }
        public string InvoiceAppreciation { get; set; }
        public Dictionary<string, string> InvoicePlaceholders { get; set; }
        public string HTML { get; set; }

        public Config(
            bool useDarkMode, 
            int recordLimit,
            Dictionary<string, string> invoicePlaceholders,
            string html,
            string businessName = "", 
            string businessAddress = "", 
            string businessLogo = "", 
            string businessPhone = "", 
            string businessEmail = "",
            string businessURL = "",
            string invoiceAppreciation = ""
        ) {
            UseDarkMode = useDarkMode;
            RecordLimit = recordLimit;
            BusinessName = businessName;
            BusinessAddress = businessAddress;
            BusinessPhone = businessPhone;
            BusinessEmail = businessEmail;
            BusinessURL = businessURL;
            InvoiceAppreciation = invoiceAppreciation;
            InvoicePlaceholders = invoicePlaceholders;
            HTML = html;
        }
    }
}
