using SmallBusinessSuite.Data.Models;

namespace SmallBusinessSuite.Utilities {
    class PayPeriodHelper {
        public List<PayPeriod> Generate(int recordLimit) {
            DatabaseInterface dbInterface = new DatabaseInterface(recordLimit);
            List<PayPeriod> periods = dbInterface.GetPayPeriods();
            int year = DateTime.Now.Year;
            bool missingYearlyPeriods = false;

            if (periods == null || periods.Count == 0) {
                missingYearlyPeriods = true;
            } else if (periods.OrderByDescending(x => x.End).ToList()[0].End.Year != year) {
                missingYearlyPeriods = true;
            }   

            DateTime current = new DateTime(
                DateTime.Now.Year,
                1,
                1
            );

            while (current.DayOfWeek != DayOfWeek.Sunday) {
                current = current.AddDays(1);
            }

            while(current.Year == DateTime.Now.Year) {
                DateTime start = current.AddDays(-7);
                DateTime end = current;

                PayPeriod period = new PayPeriod(periods.Count + 1, start, end);

                if (missingYearlyPeriods) {
                    if (periods.Where(item => item.Start == period.Start).Where(item => item.End == period.End).Count() > 0) {
                        break;
                    } else {
                        periods.Add(period);
                        dbInterface.AddPayPeriod(period);
                    }
                }

                current = current.AddDays(7);
            }

            return periods;
        }
    }
}
