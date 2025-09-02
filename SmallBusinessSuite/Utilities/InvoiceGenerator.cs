using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using SmallBusinessSuite.Data;
using SmallBusinessSuite.Data.Models;
using System.IO;

namespace SmallBusinessSuite.Utilities {
    internal class InvoiceGenerator {
        public void Generate(Dictionary<string, string> replacements, string companyName, DateTime invoiceDate) {
            if (!Directory.Exists($"C:\\SmallBusinessSuite\\Invoices\\{companyName}\\")) {
                Directory.CreateDirectory($"C:\\SmallBusinessSuite\\Invoices\\{companyName}\\");
            }

            string filename = $"C:\\SmallBusinessSuite\\Invoices\\{companyName}\\INVOICE-{invoiceDate.ToString("MMM-dd-yyyy")}.docx";
            string template = @"C:\SmallBusinessSuite\InvoiceTemplate.dotx";
            string invoiceText = string.Empty;
            File.Copy(template, filename, true);

            using (WordprocessingDocument invoice = WordprocessingDocument.Open(filename, true)) {
                invoice.ChangeDocumentType(WordprocessingDocumentType.Document);

                using (StreamReader reader = new StreamReader(invoice.MainDocumentPart.GetStream())) {
                    invoiceText = reader.ReadToEnd();
                }

                foreach (string key in replacements.Keys) {
                    invoiceText = invoiceText.Replace(key, replacements[key]);
                }

                using (StreamWriter writer = new StreamWriter(invoice.MainDocumentPart.GetStream(FileMode.Create))) {
                    writer.Write(invoiceText);
                }

                invoice.Save();
            }
        }

        public string Preview(Dictionary<string, string> replacements, string html) {
            foreach (string key in replacements.Keys) {
                html = html.Replace(key, replacements[key]);
            }
            return html;
        }

        public void UpdatePlaceholders(Config config, Invoice invoice, DatabaseInterface dbInterface) {
            if (invoice != null) {
                List<InvoiceItem> items = dbInterface.GetRelatedInvoiceItems(invoice.ID);
                int i = 0;

                foreach (InvoiceItem item in items) {
                    config.InvoicePlaceholders[$"[WORK{i}]"] = item.WorkOrder;
                    config.InvoicePlaceholders[$"[DESC{i}]"] = item.Description;
                    config.InvoicePlaceholders[$"[RATE{i}]"] = item.Rate.ToString("C2");
                    config.InvoicePlaceholders[$"[HOUR{i}]"] = item.Hours.ToString();
                    config.InvoicePlaceholders[$"[AMNT{i}]"] = item.Amount.ToString("C2");

                    i++;
                }

                while (i < 10) {
                    config.InvoicePlaceholders[$"[WORK{i}]"] = string.Empty;
                    config.InvoicePlaceholders[$"[DESC{i}]"] = string.Empty;
                    config.InvoicePlaceholders[$"[RATE{i}]"] = string.Empty;
                    config.InvoicePlaceholders[$"[HOUR{i}]"] = string.Empty;
                    config.InvoicePlaceholders[$"[AMNT{i}]"] = string.Empty;
                    i++;
                }

                config.InvoicePlaceholders["[SUM]"] = items.Sum(x => x.Amount).ToString("C2");
                config.InvoicePlaceholders["[INVOICE_NUM]"] = $"{invoice}";
                config.InvoicePlaceholders["[INVOICE_DATE]"] = invoice.Date.ToString("MM/dd/yyyy");
                config.InvoicePlaceholders["[RECIPIENT_COMPANY]"] = invoice.Client.Name;
                config.InvoicePlaceholders["[RECIPIENT_CONTACT]"] = invoice.Client.Contact;
            }
        }
    }
}
