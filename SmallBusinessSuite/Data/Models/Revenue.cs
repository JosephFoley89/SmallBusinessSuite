using SmallBusinessSuite.Data.Enums;

namespace SmallBusinessSuite.Data.Models {
    class Revenue {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public RevenueCategory Type { get; set; }

        public Revenue(int id, DateTime date, string item, decimal amount, RevenueCategory type) {
            ID = id;
            Date = date;
            Item = item;
            Amount = amount;
            Type = type;
        }

        public Revenue() { 
        
        }

        public static Revenue FromCSV(string csvLine) {
            Revenue revenue = new Revenue();
            string[] properties = csvLine.Split(',');
            RevenueCategory category;
            Enum.TryParse(properties[4], out category);

            revenue.Date = new DateTime(
                Int32.Parse(properties[0]),
                Int32.Parse(properties[1]),
                Int32.Parse(properties[2])
            );

            revenue.Item = properties[3];
            revenue.Amount = Decimal.Parse(properties[4]);
            revenue.Type = category;
            return revenue;
        }
    }
}
