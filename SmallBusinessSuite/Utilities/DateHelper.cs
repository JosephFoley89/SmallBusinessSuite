using SmallBusinessSuite.Data.Models;

namespace SmallBusinessSuite.Utilities {
    class DateHelper {
        public List<string> GetYears(List<Expense> expenses) {
            List<string> years = new List<string>();

            if (expenses != null && expenses.Count > 0) {
                int year = expenses[0].Date.Year;

                while (year < DateTime.Now.Year + 1) {
                    years.Add(year.ToString());
                    year++;
                }
            } else {
                years.Add(DateTime.Now.Year.ToString());
            }

            return years;
        }
    }
}
