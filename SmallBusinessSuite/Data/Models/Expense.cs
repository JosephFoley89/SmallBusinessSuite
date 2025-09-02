using SmallBusinessSuite.Data.Enums;

namespace SmallBusinessSuite.Data.Models {
    class Expense {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategory Type { get; set; }
        public bool IsRecurring { get; set; }
        public Frequency? RecurrenceFrequency { get; set; }
        public int? Parent { get; set; }

        public Expense(int id, DateTime date, string item, decimal amount, ExpenseCategory type, bool isRecurring = false, Frequency? frequency = null, int? parent = null) {
            ID = id;
            Date = date;
            Item = item;
            Amount = amount;
            Type = type;
            IsRecurring = isRecurring;
            RecurrenceFrequency = frequency;
            Parent = parent;
        }

        public Expense() {
            
        }

        public static Expense FromCSV(string csvLine) {
            Expense expense = new Expense();
            string[] properties = csvLine.Split(',');
            ExpenseCategory category;
            Frequency frequency;
            Enum.TryParse(properties[5], out category);
            Enum.TryParse(properties[7], out frequency);

            expense.Date = new DateTime(Int32.Parse(properties[0]), Int32.Parse(properties[1]), Int32.Parse(properties[2]));
            expense.Item = properties[3];
            expense.Amount = decimal.Parse(properties[4]);
            expense.Type = category;
            expense.IsRecurring = bool.Parse(properties[6]);
            expense.RecurrenceFrequency = frequency;

            return expense;
        }
    }
}
