using AdonisUI;
using SmallBusinessSuite.Data;
using SmallBusinessSuite.Data.Enums;
using SmallBusinessSuite.Data.Models;
using SmallBusinessSuite.Utilities;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SmallBusinessSuite {
    public partial class MainWindow : Window {
        private ConfigLoader loader;
        private Config config;
        private PayPeriodHelper periods;
        private DateHelper dateHelper;
        private List<string> years;
        private Employee SelectedEmployee;
        private Client SelectedClient;
        private Expense SelectedExpense;
        private Revenue SelectedRevenue;
        private Shift SelectedShift;
        private Payroll SelectedPayroll;
        private Invoice SelectedInvoice;
        private InvoiceItem SelectedInvoiceItem;
        private DatabaseInterface dbInterface;
        private InvoiceGenerator generator;

        List<Month> months = Enum.GetValues(typeof(Month)).Cast<Month>().ToList();
        List<ExpenseCategory> expenseCategories = Enum.GetValues(typeof(ExpenseCategory)).Cast<ExpenseCategory>().ToList();
        List<RevenueCategory> revenueCatgories = Enum.GetValues(typeof(RevenueCategory)).Cast<RevenueCategory>().ToList();
        List<Frequency> frequencies = Enum.GetValues(typeof(Frequency)).Cast<Frequency>().ToList();

        public MainWindow() {
            loader = new ConfigLoader();
            periods = new PayPeriodHelper();
            dateHelper = new DateHelper();
            config = loader.Load();
            dbInterface = new DatabaseInterface(config.RecordLimit);
            generator = new InvoiceGenerator();

            if (config.UseDarkMode) {
                AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, ResourceLocator.DarkColorScheme);
            } else {
                AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, ResourceLocator.LightColorScheme);
            }

            InitializeComponent();
            AddScheduledExpenses();

            ShiftData.ItemsSource = dbInterface.GetShifts();
            PayrollData.ItemsSource = dbInterface.GetPayrolls();
            EmployeeData.ItemsSource = dbInterface.GetEmployees();
            ClientData.ItemsSource = dbInterface.GetClients();
            InvoiceRecipient.ItemsSource = dbInterface.GetClients();
            ExpenseData.ItemsSource = dbInterface.GetExpenses();
            RevenueData.ItemsSource = dbInterface.GetRevenues();
            ClientSelect.ItemsSource = dbInterface.GetClients();
            ShiftEmployeeSelect.ItemsSource = dbInterface.GetEmployees();
            EmployeeSelect.ItemsSource = dbInterface.GetEmployees();
            Invoices.ItemsSource = dbInterface.GetInvoices();
            years = dateHelper.GetYears(dbInterface.GetExpenses());
            ExpenseType.ItemsSource = expenseCategories;
            RevenueType.ItemsSource = revenueCatgories;
            RF.ItemsSource = frequencies;
            IncomeCategorySelect.ItemsSource = revenueCatgories;
            ExpenseCategorySelect.ItemsSource = expenseCategories;
            ExpenseYearSelect.ItemsSource = years;
            FinancialYear.ItemsSource = years;
            IncomeYearSelect.ItemsSource = years;
            ShiftYearSelect.ItemsSource = years;
            ExpenseMonthSelect.ItemsSource = months;
            ShiftMonthSelect.ItemsSource = months;
            IncomeMonthSelect.ItemsSource = months;
            ShiftMonthSelect.ItemsSource = months;
            InvoicePreview.NavigateToString(generator.Preview(config.InvoicePlaceholders, config.HTML));

            RF.IsEnabled = false;
            RF.SelectedIndex = 0;
            PayPeriod.ItemsSource = periods.Generate(config.RecordLimit);
            RowLimit.Text = config.RecordLimit.ToString();
            BusinessAddress.Text = config.BusinessAddress;
            BusinessEmail.Text = config.BusinessEmail;
            BusinessURL.Text = config.BusinessURL;
            BusinessName.Text = config.BusinessName;
            BusinessPhone.Text = config.BusinessPhone;
            InvoiceAppreciation.Text = config.InvoiceAppreciation;

            System.Globalization.Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            PayPeriod.SelectedIndex = calendar.GetWeekOfYear(
                DateTime.Now,
                CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
                CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
            ) - 1;
        }

        //UI Stuff

        private void UpdateLists() {
            EmployeeSelect.ItemsSource = null;
            ClientSelect.ItemsSource = null;
            ShiftData.ItemsSource = null;
            PayrollData.ItemsSource = null;
            EmployeeData.ItemsSource = null;
            ClientData.ItemsSource = null;
            ExpenseData.ItemsSource= null;
            RevenueData.ItemsSource = null;
            InvoiceItems.ItemsSource = null;
            Invoices.ItemsSource = null;
            InvoiceRecipient.ItemsSource = null;

            EmployeeSelect.ItemsSource = dbInterface.GetEmployees();
            ClientSelect.ItemsSource = dbInterface.GetClients();
            ShiftData.ItemsSource = dbInterface.GetShifts();
            PayrollData.ItemsSource = dbInterface.GetPayrolls();
            EmployeeData.ItemsSource = dbInterface.GetEmployees();
            ClientData.ItemsSource = dbInterface.GetClients();
            ExpenseData.ItemsSource = dbInterface.GetExpenses();
            RevenueData.ItemsSource = dbInterface.GetRevenues();
            InvoiceItems.ItemsSource = dbInterface.GetInvoiceItems();
            Invoices.ItemsSource = dbInterface.GetInvoices();
            InvoiceRecipient.ItemsSource = dbInterface.GetClients();

            ClearEmployeeValues();
            ClearExpenseValues();
            ClearRevenueValues();
            ClearShiftValues();
            ClearPayrollValues();
            ClearClientValues();
            ClearInvoiceData();
        }

        private void ClearEmployeeValues() {
            SelectedEmployee = null;
            EmployeeName.Clear();
            DriveTime.Clear();
            HourlyRate.Clear();
            EmployeeAddress.Clear();
            EmployeePhone.Clear();
        }

        private void ClearExpenseValues() {
            SelectedExpense = null;
            ExpenseDate.SelectedDate = null;
            ExpenseName.Clear();
            ExpenseAmount.Clear();
            ExpenseType.SelectedIndex = 0;
            IsRecurringExpense.IsChecked = false;
            RF.IsEnabled = false;
            RF.SelectedIndex = 0;
        }

        private void ClearRevenueValues() {
            SelectedRevenue = null;
            RevenueDate.SelectedDate = null;
            RevenueSource.Clear();
            RevenueAmount.Clear();
            RevenueType.SelectedIndex = 0;
        }

        private void ClearShiftValues() {
            SelectedShift = null;
            EmployeeSelect.SelectedItem = null;
            ShiftDate.SelectedDate = null;
            HoursWorked.Clear();
        }

        public void ClearPayrollValues() {
            SelectedPayroll = null;
            PayrollPayments.ItemsSource = null;
            PayrollShifts.ItemsSource =null;
        }

        private void ClearClientValues() {
            SelectedClient = null;
            ClientName.Clear();
            ClientAddress.Clear();
            ClientEmail.Clear();
            ClientPhone.Clear();
            ContactName.Clear();
        }

        private void ClearInvoiceData() {
            SelectedInvoice = null;
            ItemAmount.Clear();
            InvoiceRecipient.SelectedValue = null;
            InvoiceDate.SelectedDate = null;
            ItemDescription.Clear();
            ItemHours.Clear();
            ItemRate.Clear();
            WorkOrder.Clear();
        }

        //EVENTS

        private void Data_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            DataGrid Data = (DataGrid)e.Source;

            if (Data.SelectedItem != null) {
                if (Data.SelectedItem.GetType() == typeof(Employee)) {
                    SelectedEmployee = (Employee)Data.SelectedItem;
                    EmployeeName.Text = SelectedEmployee.Name;
                    DriveTime.Text = SelectedEmployee.DriveTime.ToString();
                    HourlyRate.Text = SelectedEmployee.HourlyRate.ToString();
                    EmployeeAddress.Text = SelectedEmployee.Address;
                    EmployeePhone.Text = SelectedEmployee.PhoneNumber;
                } else if (Data.SelectedItem.GetType() == typeof(Expense)) {
                    SelectedExpense = (Expense)Data.SelectedItem;
                    ExpenseName.Text = SelectedExpense.Item;
                    ExpenseAmount.Text = SelectedExpense.Amount.ToString();
                    ExpenseDate.Text = SelectedExpense.Date.ToString();
                    ExpenseType.SelectedItem = SelectedExpense.Type;
                    IsRecurringExpense.IsChecked = SelectedExpense.IsRecurring;
                    RF.SelectedItem = SelectedExpense.RecurrenceFrequency;
                    RF.IsEnabled = (bool)IsRecurringExpense.IsChecked;
                } else if (Data.SelectedItem.GetType() == typeof(Revenue)) {
                    SelectedRevenue = (Revenue)Data.SelectedItem;
                    RevenueSource.Text = SelectedRevenue.Item;
                    RevenueAmount.Text = SelectedRevenue.Amount.ToString();
                    RevenueDate.Text = SelectedRevenue.Date.ToString();
                    RevenueType.SelectedItem = SelectedRevenue.Type;
                } else if (Data.SelectedItem.GetType() == typeof(Shift)) {
                    SelectedShift = (Shift)Data.SelectedItem;
                    EmployeeSelect.SelectedIndex = SelectedShift.Employee.ID - 1;
                    ShiftDate.Text = SelectedShift.Date.ToString();
                    HoursWorked.Text = SelectedShift.TimeWorked.ToString();
                    ClientSelect.SelectedIndex = SelectedShift.Client.ID - 1;
                } else if (Data.SelectedItem.GetType() == typeof(Payroll)) {
                    SelectedPayroll = (Payroll)Data.SelectedItem;
                    PayrollPayments.ItemsSource = dbInterface.GetRelatedPayments(SelectedPayroll.ID);
                    PayrollShifts.ItemsSource = dbInterface.GetRelatedShifts(SelectedPayroll.ID);
                    PayPeriod.SelectedIndex = SelectedPayroll.Period.ID - 1;
                } else if (Data.SelectedItem.GetType() == typeof(Client)) {
                    SelectedClient = (Client)Data.SelectedItem;
                    ClientName.Text = SelectedClient.Name;
                    ContactName.Text = SelectedClient.Contact;
                    ClientAddress.Text = SelectedClient.Address;
                    ClientEmail.Text = SelectedClient.Email;
                    ClientPhone.Text = SelectedClient.PhoneNumber;
                } else if (Data.SelectedItem.GetType() == typeof(Invoice)) {
                    SelectedInvoice = (Invoice)Data.SelectedItem;
                    InvoiceItems.ItemsSource = null;
                    InvoiceItems.ItemsSource = dbInterface.GetRelatedInvoiceItems(SelectedInvoice.ID);
                    InvoiceRecipient.SelectedIndex = SelectedInvoice.Client.ID - 1;
                    InvoiceDate.SelectedDate = SelectedInvoice.Date;
                } else if (Data.SelectedItem.GetType() == typeof(InvoiceItem)) {
                    SelectedInvoiceItem = (InvoiceItem)Data.SelectedItem;
                    ItemAmount.Text = SelectedInvoiceItem.Amount.ToString();
                    ItemHours.Text = SelectedInvoiceItem.Hours.ToString();
                    ItemRate.Text = SelectedInvoiceItem.Rate.ToString();
                    ItemDescription.Text = SelectedInvoiceItem.Description;
                    WorkOrder.Text = SelectedInvoiceItem.WorkOrder;
                }
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e) {
            try {
                Dictionary<string, string> invoiceInfo = new Dictionary<string, string>();

                config.UseDarkMode = (bool)UseDarkTheme.IsChecked;
                config.RecordLimit = Int32.Parse(RowLimit.Text);
                config.BusinessName = BusinessName.Text;
                config.BusinessAddress = BusinessAddress.Text;
                config.BusinessPhone = BusinessPhone.Text;
                config.BusinessEmail = BusinessEmail.Text;
                config.BusinessURL = BusinessURL.Text;
                config.InvoiceAppreciation = InvoiceAppreciation.Text;

                invoiceInfo.Add("[COMPANY]", config.BusinessName);
                invoiceInfo.Add("[PHONE]", config.BusinessPhone);
                invoiceInfo.Add("[EMAIL]", config.BusinessEmail);
                invoiceInfo.Add("[ADDRESS]", config.BusinessAddress);
                invoiceInfo.Add("[URL]", config.BusinessURL);
                invoiceInfo.Add("[INVOICE_DATE]", " ");
                invoiceInfo.Add("[INVOICE_NUM]", " ");
                invoiceInfo.Add("[RECIPIENT_COMPANY]", " ");
                invoiceInfo.Add("[RECIPIENT_CONTACT]", " ");
                invoiceInfo.Add("[SUM]", " ");
                invoiceInfo.Add("[INVOICE_APPRECIATION]", config.InvoiceAppreciation);

                for (int i = 0; i < 10; i++) {
                    invoiceInfo.Add($"[DESC{i}]", "");
                    invoiceInfo.Add($"[WORK{i}]", "");
                    invoiceInfo.Add($"[HOUR{i}]", "");
                    invoiceInfo.Add($"[AMNT{i}]", "");
                    invoiceInfo.Add($"[RATE{i}]", "");
                }

                config.InvoicePlaceholders = invoiceInfo;
                loader.Save(config, @"C:\SmallBusinessSuite\config.json");

                if (config.UseDarkMode) {
                    AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, ResourceLocator.DarkColorScheme);
                } else {
                    AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, ResourceLocator.LightColorScheme);
                }

                UpdateLists();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IsRecurringExpense_Checked(object sender, RoutedEventArgs e) {
            RF.IsEnabled = (bool)IsRecurringExpense.IsChecked;
            
            if (!RF.IsEnabled) {
                RF.SelectedIndex = 0;
            }
        }

        private void GenerateInvoice_Click(object sender, RoutedEventArgs e) {
            generator.UpdatePlaceholders(config, SelectedInvoice, dbInterface);
            generator.Generate(config.InvoicePlaceholders, SelectedInvoice.Client.Name, SelectedInvoice.Date);
        }

        private void InvoiceRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            generator.UpdatePlaceholders(config, SelectedInvoice, dbInterface);
            InvoicePreview.NavigateToString(generator.Preview(config.InvoicePlaceholders, config.HTML));
        }

        private void InvoiceDate_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            generator.UpdatePlaceholders(config, SelectedInvoice, dbInterface);
            InvoicePreview.NavigateToString(generator.Preview(config.InvoicePlaceholders, config.HTML));
        }

        private void FinancialYear_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            PopulateMonthlyBalance();
        }

        private void FYReset_Click(object sender, RoutedEventArgs e) {
            FinancialYear.SelectedValue = null;
            JanProfit.Content = $"Profit:";
            FebProfit.Content = $"Profit:";
            MarProfit.Content = $"Profit:";
            AprProfit.Content = $"Profit:";
            MayProfit.Content = $"Profit:";
            JunProfit.Content = $"Profit:";
            JulProfit.Content = $"Profit:";
            AugProfit.Content = $"Profit:";
            SepProfit.Content = $"Profit:";
            OctProfit.Content = $"Profit:";
            NovProfit.Content = $"Profit:";
            DecProfit.Content = $"Profit:";
            JanRevenue.Content = $"Revenue:";
            FebRevenue.Content = $"Revenue:";
            MarRevenue.Content = $"Revenue:";
            AprRevenue.Content = $"Revenue:";
            MayRevenue.Content = $"Revenue:";
            JunRevenue.Content = $"Revenue:";
            JulRevenue.Content = $"Revenue:";
            AugRevenue.Content = $"Revenue:";
            SepRevenue.Content = $"Revenue:";
            OctRevenue.Content = $"Revenue:";
            NovRevenue.Content = $"Revenue:";
            DecRevenue.Content = $"Revenue:";
            JanExpense.Content = $"Expense:";
            FebExpense.Content = $"Expense:";
            MarExpense.Content = $"Expense:";
            AprExpense.Content = $"Expense:";
            MayExpense.Content = $"Expense:";
            JunExpense.Content = $"Expense:";
            JulExpense.Content = $"Expense:";
            AugExpense.Content = $"Expense:";
            SepExpense.Content = $"Expense:";
            OctExpense.Content = $"Expense:";
            NovExpense.Content = $"Expense:";
            DecExpense.Content = $"Expense:";
        }

        //REPORTS

        private List<decimal> PopulateExpensesByMonth() {
            if (FinancialYear.SelectedItem != null) {
                int year = Int32.Parse(FinancialYear.SelectedItem.ToString());
                List<Expense> expenses = dbInterface.GetExpenseByYear(year).ToList();
                decimal january = expenses.Where(e => e.Date.Month == 1).ToList().Sum(e => e.Amount);
                decimal february = expenses.Where(e => e.Date.Month == 2).ToList().Sum(e => e.Amount);
                decimal march = expenses.Where(e => e.Date.Month == 3).ToList().Sum(e => e.Amount);
                decimal april = expenses.Where(e => e.Date.Month == 4).ToList().Sum(e => e.Amount);
                decimal may = expenses.Where(e => e.Date.Month == 5).ToList().Sum(e => e.Amount);
                decimal june = expenses.Where(e => e.Date.Month == 6).ToList().Sum(e => e.Amount);
                decimal july = expenses.Where(e => e.Date.Month == 7).ToList().Sum(e => e.Amount);
                decimal august = expenses.Where(e => e.Date.Month == 8).ToList().Sum(e => e.Amount);
                decimal september = expenses.Where(e => e.Date.Month == 9).ToList().Sum(e => e.Amount);
                decimal october = expenses.Where(e => e.Date.Month == 10).ToList().Sum(e => e.Amount);
                decimal november = expenses.Where(e => e.Date.Month == 11).ToList().Sum(e => e.Amount);
                decimal december = expenses.Where(e => e.Date.Month == 12).ToList().Sum(e => e.Amount);

                JanExpense.Content = $"Expense:  {january.ToString("C2")}";
                FebExpense.Content = $"Expense:  {february.ToString("C2")}";
                MarExpense.Content = $"Expense:  {march.ToString("C2")}";
                AprExpense.Content = $"Expense:  {april.ToString("C2")}";
                MayExpense.Content = $"Expense:  {may.ToString("C2")}";
                JunExpense.Content = $"Expense:  {june.ToString("C2")}";
                JulExpense.Content = $"Expense:  {july.ToString("C2")}";
                AugExpense.Content = $"Expense:  {august.ToString("C2")}";
                SepExpense.Content = $"Expense:  {september.ToString("C2")}";
                OctExpense.Content = $"Expense:  {october.ToString("C2")}";
                NovExpense.Content = $"Expense:  {november.ToString("C2")}";
                DecExpense.Content = $"Expense:  {december.ToString("C2")}";

                return new List<decimal>() {
                    january, february, march, april, may, june, july,
                    august, september, october, november, december
                };
            }

            return new List<decimal>();
        }

        private List<decimal> PopulateRevenueByMonth() {
            if (FinancialYear.SelectedItem != null) {
                int year = Int32.Parse(FinancialYear.SelectedItem.ToString());
                List<Revenue> revenues = dbInterface.GetRevenuesByYear(year);
                decimal january = revenues.Where(e => e.Date.Month == 1).ToList().Sum(e => e.Amount);
                decimal february = revenues.Where(e => e.Date.Month == 2).ToList().Sum(e => e.Amount);
                decimal march = revenues.Where(e => e.Date.Month == 3).ToList().Sum(e => e.Amount);
                decimal april = revenues.Where(e => e.Date.Month == 4).ToList().Sum(e => e.Amount);
                decimal may = revenues.Where(e => e.Date.Month == 5).ToList().Sum(e => e.Amount);
                decimal june = revenues.Where(e => e.Date.Month == 6).ToList().Sum(e => e.Amount);
                decimal july = revenues.Where(e => e.Date.Month == 7).ToList().Sum(e => e.Amount);
                decimal august = revenues.Where(e => e.Date.Month == 8).ToList().Sum(e => e.Amount);
                decimal september = revenues.Where(e => e.Date.Month == 9).ToList().Sum(e => e.Amount);
                decimal october = revenues.Where(e => e.Date.Month == 10).ToList().Sum(e => e.Amount);
                decimal november = revenues.Where(e => e.Date.Month == 11).ToList().Sum(e => e.Amount);
                decimal december = revenues.Where(e => e.Date.Month == 12).ToList().Sum(e => e.Amount);

                JanRevenue.Content = $"Revenue: {january.ToString("C2")}";
                FebRevenue.Content = $"Revenue: {february.ToString("C2")}";
                MarRevenue.Content = $"Revenue: {march.ToString("C2")}";
                AprRevenue.Content = $"Revenue: {april.ToString("C2")}";
                MayRevenue.Content = $"Revenue: {may.ToString("C2")}";
                JunRevenue.Content = $"Revenue: {june.ToString("C2")}";
                JulRevenue.Content = $"Revenue: {july.ToString("C2")}";
                AugRevenue.Content = $"Revenue: {august.ToString("C2")}";
                SepRevenue.Content = $"Revenue: {september.ToString("C2")}";
                OctRevenue.Content = $"Revenue: {october.ToString("C2")}";
                NovRevenue.Content = $"Revenue: {november.ToString("C2")}";
                DecRevenue.Content = $"Revenue: {december.ToString("C2")}";

                return new List<decimal>() {
                    january, february, march, april, may, june, july,
                    august, september, october, november, december
                };
            }

            return new List<decimal>();
        }

        private void PopulateMonthlyBalance() {
            List<decimal> expenses = PopulateExpensesByMonth();
            List<decimal> revenues = PopulateRevenueByMonth();
            List<decimal> profits = new List<decimal>();

            for (int i = 0; i < expenses.Count; i++) {
                profits.Add(revenues[i] - expenses[i]);
            }

            if (profits.Count > 0) {
                JanProfit.Content = $"Profit:\t  {profits[0].ToString("C2")}";
                FebProfit.Content = $"Profit:\t  {profits[1].ToString("C2")}";
                MarProfit.Content = $"Profit:\t  {profits[2].ToString("C2")}";
                AprProfit.Content = $"Profit:\t  {profits[3].ToString("C2")}";
                MayProfit.Content = $"Profit:\t  {profits[4].ToString("C2")}";
                JunProfit.Content = $"Profit:\t  {profits[5].ToString("C2")}";
                JulProfit.Content = $"Profit:\t  {profits[6].ToString("C2")}";
                AugProfit.Content = $"Profit:\t  {profits[7].ToString("C2")}";
                SepProfit.Content = $"Profit:\t  {profits[8].ToString("C2")}";
                OctProfit.Content = $"Profit:\t  {profits[9].ToString("C2")}";
                NovProfit.Content = $"Profit:\t  {profits[10].ToString("C2")}";
                DecProfit.Content = $"Profit:\t  {profits[11].ToString("C2")}";

            }
        }

        private void ExecuteShiftReport_Click(object sender, RoutedEventArgs e) {
            List<Shift> selectedShifts = dbInterface.GetShifts();

            if (ShiftEmployeeSelect.SelectedIndex > -1) {
                Employee worker = (Employee)ShiftEmployeeSelect.SelectedValue;
                selectedShifts = selectedShifts.Where(e => e.Employee.ID == worker.ID).ToList();
            }

            if (ShiftMonthSelect.SelectedIndex > -1) {
                int month = ShiftMonthSelect.SelectedIndex + 1;
                selectedShifts = selectedShifts.Where(m => m.Date.Month == month).ToList();
            }

            if (ShiftYearSelect.SelectedIndex > -1) {
                int year = Int32.Parse(ShiftYearSelect.Text);
                selectedShifts = selectedShifts.Where(y => y.Date.Year == year).ToList();
            }

            ShiftReportData.ItemsSource = null;
            ShiftReportData.ItemsSource = selectedShifts;
        }

        private void ClearShiftReport_Click(object sender, RoutedEventArgs e) {
            ShiftMonthSelect.SelectedItem = null;
            ShiftYearSelect.SelectedItem = null;
            ShiftEmployeeSelect.SelectedItem = null;
        }

        private void ExecuteExpenseReport_Click(object sender, RoutedEventArgs e) {
            List<Expense> selectedExpenses = dbInterface.GetExpenses();

            if (ExpenseCategorySelect.SelectedItem != null) {
                selectedExpenses = selectedExpenses.Where(e => e.Type == (ExpenseCategory)ExpenseCategorySelect.SelectedValue).ToList();
            }

            if (ExpenseMonthSelect.SelectedItem != null) {
                int month = ExpenseMonthSelect.SelectedIndex + 1;
                selectedExpenses = selectedExpenses.Where(e => e.Date.Month == month).ToList();
            }

            if (ExpenseYearSelect.SelectedItem != null) {
                int year = Int32.Parse(ExpenseYearSelect.Text);
                selectedExpenses = selectedExpenses.Where(e => e.Date.Year == year).ToList();
            }

            ExpenseReportData.ItemsSource = null;
            ExpenseReportData.ItemsSource = selectedExpenses;
        }

        private void ClearExpenseReport_Click(object sender, RoutedEventArgs e) {
            ExpenseYearSelect.SelectedItem = null;
            ExpenseMonthSelect.SelectedItem = null;
            ExpenseCategorySelect.SelectedItem = null;
        }

        private void ExecuteRevenueReport_Click(object sender, RoutedEventArgs e) {
            List<Revenue> selectedRevenues = dbInterface.GetRevenues();

            if (IncomeCategorySelect.SelectedItem != null) {
                selectedRevenues = selectedRevenues.Where(r => r.Type == (RevenueCategory)IncomeCategorySelect.SelectedValue).ToList();
            }

            if (IncomeMonthSelect.SelectedItem != null) {
                int month = IncomeMonthSelect.SelectedIndex + 1;
                selectedRevenues = selectedRevenues.Where(r => r.Date.Month == month).ToList();
            }

            if (IncomeYearSelect.SelectedItem != null) {
                int year = Int32.Parse(IncomeYearSelect.Text);
            }
            
            IncomeReportData.ItemsSource = selectedRevenues;
            IncomeReportData.ItemsSource = selectedRevenues;
        }

        private void ClearRevenueReport_Click(object sender, RoutedEventArgs e) {
            IncomeCategorySelect.SelectedItem = null;
            IncomeMonthSelect.SelectedItem = null;
            IncomeYearSelect.SelectedItem = null;
        }

        //EMPLOYEE CRUD

        private void CreateEmployee_Click(object sender, RoutedEventArgs e) {
            string msgBoxTitle = "Employee Record Error";
            bool canCreate = true;
            int driveTime;
            decimal wage;

            if (!Int32.TryParse(DriveTime.Text, out driveTime)) {
                MessageBox.Show("Please input a valid whole number into the Drive Time field.", msgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (!Decimal.TryParse(HourlyRate.Text, out wage)) {
                MessageBox.Show("Please input a valid number into the Hourly Rate field.", msgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (dbInterface.EmployeeExists(EmployeeName.Text)) {
                SaveEmployeeChanges_Click(sender, e);
                canCreate = false;
            }

            if (canCreate) {
                dbInterface.AddEmployee(
                    new Employee(
                        0,
                        EmployeeName.Text,
                        driveTime,
                        wage,
                        EmployeeAddress.Text,
                        EmployeePhone.Text
                    )
                );

                UpdateLists();
            }
        }

        private void SaveEmployeeChanges_Click(object sender, RoutedEventArgs e) {
            string msgBoxTitle = "Employee Record Error";
            bool canUpdate = true;
            int driveTime;
            decimal wage;

            if (SelectedEmployee == null) {
                MessageBox.Show("Please select an Employee record to update.", msgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (!Int32.TryParse(DriveTime.Text, out driveTime)) {
                MessageBox.Show("Please input a valid whole number into the Drive Time field.", msgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (!Decimal.TryParse(HourlyRate.Text, out wage)) {
                MessageBox.Show("Please input a valid number into the Hourly Rate field.", msgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (canUpdate) {
                SelectedEmployee.Name = EmployeeName.Text;
                SelectedEmployee.DriveTime = driveTime;
                SelectedEmployee.HourlyRate = wage;
                SelectedEmployee.Address = EmployeeAddress.Text;
                SelectedEmployee.PhoneNumber = EmployeePhone.Text;
                dbInterface.UpdateEmployee(SelectedEmployee);
                UpdateLists();
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e) {
            bool canDelete = SelectedEmployee != null;

            if (canDelete) {
                dbInterface.RemoveEmployee(SelectedEmployee);
                UpdateLists();
            }  else {
                MessageBox.Show("Please select an employee record to remove.", "Employee Record Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        //CLIENT CRUD

        private void CreateClient_Click(object sender, RoutedEventArgs e) {
            bool canCreate = true;

            if (ClientName.Text == "") {
                canCreate = false;
                MessageBox.Show("Please input the Client's name. All other fields are not mandatory.", "Client Record Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (dbInterface.ClientExists(ClientName.Text)) {
                canCreate = false;
                MessageBox.Show($"There is already a client in the record with {ClientName.Text} as its name. Did you intend to update the record?", "Client Record Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (canCreate) {
                dbInterface.AddClient(
                    new Client(
                        0,
                        ClientName.Text,
                        ContactName.Text,
                        ClientEmail.Text,
                        ClientPhone.Text,
                        ClientAddress.Text
                    )
                );

                UpdateLists();
            }
        }

        private void UpdateClient_Click(object sender, RoutedEventArgs e) {
            bool canUpdate = true;

            if (SelectedClient == null) {
                canUpdate = false;
            }

            if (canUpdate) {
                SelectedClient.Name = ClientName.Text;
                SelectedClient.Contact = ContactName.Text;
                SelectedClient.Email = ClientEmail.Text;
                SelectedClient.PhoneNumber = ClientPhone.Text;
                SelectedClient.Address = ClientAddress.Text;
                dbInterface.UpdateClient(SelectedClient);

                UpdateLists();
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e) {
            bool canDelete = SelectedClient != null;

            if (canDelete) {
                dbInterface.RemoveClient(SelectedClient);
                UpdateLists();
            } else {
                MessageBox.Show("Please select a client record to remove.", "Client Record Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        //EXPENSE CRUD

        private void AddExpense_Click(object sender, RoutedEventArgs e) {
            bool canCreate = true;
            string msgTitle = "Expense Record Error";
            decimal amount;

            if (!Decimal.TryParse(ExpenseAmount.Text, out amount)) {
                MessageBox.Show("Please input a valid number in the Expense Amount field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (ExpenseDate.SelectedDate == null) {
                MessageBox.Show("Please input a valid date in the Expense Date field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (ExpenseName.Text == "") {
                MessageBox.Show("Please input a valid name in the Expense Name field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (ExpenseType.SelectedValue == null) {
                MessageBox.Show("Please select a valid Expense Type.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if ((bool)IsRecurringExpense.IsChecked && (Frequency)RF.SelectedValue == Frequency.OneTime) {
                MessageBox.Show("Please select a valid recurrence frequency for the recurring charge.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (canCreate) {
                dbInterface.AddExpense(
                    new Expense(
                        0,
                        new DateTime(
                            ExpenseDate.SelectedDate.Value.Year,
                            ExpenseDate.SelectedDate.Value.Month,
                            ExpenseDate.SelectedDate.Value.Day
                        ),
                        ExpenseName.Text,
                        amount,
                        (ExpenseCategory)ExpenseType.SelectedValue,
                        (bool)IsRecurringExpense.IsChecked,
                        (Frequency)RF.SelectedValue
                    )
                );

                if ((bool)IsRecurringExpense.IsChecked) {
                    AddScheduledExpenses();
                }

                UpdateLists();
            }
        }

        private void UpdateExpense_Click(object sender, RoutedEventArgs e) {
            bool canUpdate = true;
            string msgTitle = "Expense Record Error";
            decimal amount;

            if (!Decimal.TryParse(ExpenseAmount.Text, out amount)) {
                MessageBox.Show("Please input a valid number in the Expense Amount field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (ExpenseDate.SelectedDate == null) {
                MessageBox.Show("Please input a valid date in the Expense Date field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (ExpenseName.Text == "") {
                MessageBox.Show("Please input a valid name in the Expense Name field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (ExpenseType.SelectedValue == null) {
                MessageBox.Show("Please select a valid Expense Type.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if ((bool)IsRecurringExpense.IsChecked && (Frequency)RF.SelectedValue == Frequency.OneTime) {
                MessageBox.Show("Please select a valid recurrence frequency for the recurring charge.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (SelectedExpense == null) {
                MessageBox.Show("Please select an Expense record to update.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (canUpdate) {
                Frequency frequency = (Frequency)SelectedExpense.RecurrenceFrequency;

                if (SelectedExpense.Type != ExpenseCategory.Payroll) {
                    SelectedExpense.Date = new DateTime(
                        ExpenseDate.SelectedDate.Value.Year,
                        ExpenseDate.SelectedDate.Value.Month,
                        ExpenseDate.SelectedDate.Value.Day
                    );
                    SelectedExpense.Amount = Decimal.Parse(ExpenseAmount.Text);
                    SelectedExpense.Type = (ExpenseCategory)ExpenseType.SelectedValue;
                    SelectedExpense.Item = ExpenseName.Text;
                    SelectedExpense.IsRecurring = (bool)IsRecurringExpense.IsChecked;
                    SelectedExpense.RecurrenceFrequency = (Frequency)RF.SelectedValue;
                    dbInterface.UpdateExpense(SelectedExpense);

                    if (SelectedExpense.IsRecurring && frequency == (Frequency)SelectedExpense.RecurrenceFrequency) {
                        AddScheduledExpenses();
                    } else {
                        foreach (Expense exp in dbInterface.GetRecurringExpenses(SelectedExpense.Item)) {
                            if (exp.Date > SelectedExpense.Date) {
                                dbInterface.RemoveExpense(exp);
                            } else {
                                dbInterface.UpdateExpense(SelectedExpense);
                            }
                        }

                        AddScheduledExpenses();
                    }

                    UpdateLists();
                } else {
                    MessageBox.Show("Payroll expenses cannot be altered. Please utilize the Payroll tab to make changes to this expense.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RemoveExpense_Click(object sender, RoutedEventArgs e) {
            bool canDelete = SelectedExpense != null;

            if (canDelete) {
                if (SelectedExpense.IsRecurring) {
                    foreach (Expense expense in dbInterface.GetRecurringExpenses(SelectedExpense.Item)) {
                        if (expense.Date > SelectedExpense.Date) {
                            dbInterface.RemoveExpense(expense);
                        } else if (expense.Date == SelectedExpense.Date) {
                            SelectedExpense.IsRecurring = false;
                            dbInterface.UpdateExpense(SelectedExpense);
                        }
                    }
                } else {
                    dbInterface.RemoveExpense(SelectedExpense);
                }

                UpdateLists();
            } else {
                MessageBox.Show("Please select an Expense record to remove.", "Client Record Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //REVENUE CRUD

        private void AddRevenue_Click(object sender, RoutedEventArgs e) {
            string msgTitle = "Revenue Record Error";
            bool canCreate = true;
            decimal amount;

            if (RevenueDate.SelectedDate == null) {
                MessageBox.Show("Please select a valid date for the Revenue record.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (!Decimal.TryParse(RevenueAmount.Text, out amount)) {
                MessageBox.Show("Please input a valid number for the Revenue record.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (RevenueSource.Text == "") {
                MessageBox.Show("Please provide a source of the revenue.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (RevenueType.SelectedItem == null) {
                MessageBox.Show("Please select a valid category for the Revenue record.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (canCreate) {
                dbInterface.AddRevenue(
                    new Revenue(
                        0,
                        new DateTime(
                            RevenueDate.SelectedDate.Value.Year,
                            RevenueDate.SelectedDate.Value.Month,
                            RevenueDate.SelectedDate.Value.Day
                        ),
                        RevenueSource.Text,
                        decimal.Parse(RevenueAmount.Text),
                        (RevenueCategory)RevenueType.SelectedValue
                    )
                );

                UpdateLists();
            }
        }

        private void UpdateRevenue_Click(object sender, RoutedEventArgs e) {
            string msgTitle = "Revenue Record Error";
            bool canUpdate = true;
            decimal amount;

            if (RevenueDate.SelectedDate == null) {
                MessageBox.Show("Please select a valid date for the Revenue record.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (!Decimal.TryParse(RevenueAmount.Text, out amount)) {
                MessageBox.Show("Please input a valid number for the Revenue record.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (RevenueSource.Text == "") {
                MessageBox.Show("Please provide a source of the revenue.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (RevenueType.SelectedItem == null) {
                MessageBox.Show("Please select a valid category for the Revenue record.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (SelectedRevenue == null) {
                MessageBox.Show("Please select a Revenue record to update.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (canUpdate) {
                SelectedRevenue.Date = new DateTime(
                    RevenueDate.SelectedDate.Value.Year,
                    RevenueDate.SelectedDate.Value.Month,
                    RevenueDate.SelectedDate.Value.Day
                );
                SelectedRevenue.Amount = Decimal.Parse(RevenueAmount.Text);
                SelectedRevenue.Type = (RevenueCategory)RevenueType.SelectedValue;
                SelectedRevenue.Item = RevenueSource.Text;
                dbInterface.UpdateRevenue(SelectedRevenue);

                UpdateLists();
            }
        }

        private void RemoveRevenue_Click(object sender, RoutedEventArgs e) {
            bool canDelete = SelectedRevenue != null;

            if (canDelete) {
                dbInterface.RemoveRevenue(SelectedRevenue);
                UpdateLists();
            } else {
                MessageBox.Show("Please select a revenue record to remove.", "Revenue Record Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //SHIFT CRUD

        private void AddShift_Click(object sender, RoutedEventArgs e) {
            bool canCreate = true;
            string msgTitle = "Shift Record Error";
            Employee employee = (Employee)EmployeeSelect.SelectedValue;
            DateTime? date = ShiftDate.SelectedDate;
            double hours = 0;

            if (employee == null) {
                MessageBox.Show("Please select an employee for the shift.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (ShiftDate.SelectedDate == null) {
                MessageBox.Show("Please input a valid shift date.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (date != null && dbInterface.ShiftExists(employee.ID, (DateTime)date)) {
                MessageBox.Show("This shift already exists. Did you intend to update the record?", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (!Double.TryParse(HoursWorked.Text, out hours)) {
                MessageBox.Show("Please input a valid number in the Hours Worked field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (hours < 1) {
                MessageBox.Show("Please input a valid number greater than 0 in the Hours Worked field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canCreate = false;
            }

            if (canCreate) {
                dbInterface.AddShift(
                    new Shift(
                        0,
                        (Employee)EmployeeSelect.SelectedValue,
                        new DateTime(
                            ShiftDate.SelectedDate.Value.Year,
                            ShiftDate.SelectedDate.Value.Month,
                            ShiftDate.SelectedDate.Value.Day
                        ),
                        Double.Parse(HoursWorked.Text),
                        (Client)ClientSelect.SelectedValue
                    )
                );

                UpdateLists();
            }
        }

        private void UpdateShift_Click(object sender, RoutedEventArgs e) {
            bool canUpdate = true;
            string msgTitle = "Shift Record Error";
            Employee employee = (Employee)EmployeeSelect.SelectedValue;
            Client client = (Client)ClientSelect.SelectedValue;
            double hours = 0;

            if (employee == null) {
                MessageBox.Show("Please select an employee for the shift?", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (client == null) {
                MessageBox.Show("Please select a client for the shift?", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (SelectedShift == null) {
                MessageBox.Show("Please select a Shift record to update.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (!Double.TryParse(HoursWorked.Text, out hours)) {
                MessageBox.Show("Please input a valid number in the Hours Worked field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (hours < 1) {
                MessageBox.Show("Please input a valid number greater than 0 in the Hours Worked field.", msgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                canUpdate = false;
            }

            if (canUpdate) {
                SelectedShift.Employee = (Employee)EmployeeSelect.SelectedValue;
                SelectedShift.TimeWorked = Double.Parse(HoursWorked.Text);
                SelectedShift.Date = new DateTime(
                    ShiftDate.SelectedDate.Value.Year,
                    ShiftDate.SelectedDate.Value.Month,
                    ShiftDate.SelectedDate.Value.Day
                );
                SelectedShift.Client = (Client)ClientSelect.SelectedValue;
                dbInterface.UpdateShift(SelectedShift);
                UpdateLists();
            }
        }

        private void RemoveShift_Click(object sender, RoutedEventArgs e) {
            bool canDelete = SelectedShift != null;

            if (canDelete) {
                dbInterface.RemoveShift(SelectedShift);
                UpdateLists();
            } else {
                MessageBox.Show("Please select a Shift record to remove.", "Shift Record Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //INVOICE ITEM CRUD
        private void AddInvoiceItem_Click(object sender, RoutedEventArgs e) {
            if (SelectedInvoice == null) {
                SelectedInvoice = new Invoice(
                    0,
                    (Client)InvoiceRecipient.SelectedValue,
                    (DateTime)InvoiceDate.SelectedDate,
                    0,
                    ""
                );

                dbInterface.AddInvoice(SelectedInvoice);
                SelectedInvoice = dbInterface.GetInvoices().Last();
                dbInterface.UpdateInvoiceNumber(SelectedInvoice.Client.ID);
                SelectedInvoice.InvoiceNumber = $"INV{SelectedInvoice.Client.ID}{dbInterface.GetInvoiceNumber(SelectedInvoice.Client.ID).ToString("00000")}";
                dbInterface.UpdateInvoice(SelectedInvoice);
            }
            
            try {
                dbInterface.AddInvoiceItem(
                    new InvoiceItem(
                        0,
                        WorkOrder.Text,
                        ItemDescription.Text,
                        Double.Parse(ItemHours.Text),
                        Decimal.Parse(ItemRate.Text),
                        Decimal.Parse(ItemAmount.Text),
                        SelectedInvoice
                    )
                );

                SelectedInvoice.Total = dbInterface.GetRelatedInvoiceItems(SelectedInvoice.ID).Sum(x => x.Amount);
                dbInterface.UpdateInvoice(SelectedInvoice);
                generator.UpdatePlaceholders(config, SelectedInvoice, dbInterface);
                InvoiceItems.ItemsSource = null;
                InvoiceItems.ItemsSource = dbInterface.GetRelatedInvoiceItems(SelectedInvoice.ID);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "InvoiceItem Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            UpdateLists();
        }

        private void UpdateInvoiceItem_Click(object sender, RoutedEventArgs e) {
            if (SelectedInvoiceItem != null) {
                SelectedInvoiceItem.WorkOrder = WorkOrder.Text;
                SelectedInvoiceItem.Rate = Decimal.Parse(ItemRate.Text);
                SelectedInvoiceItem.Hours = Double.Parse(ItemHours.Text);
                SelectedInvoiceItem.Description = ItemDescription.Text;
                SelectedInvoiceItem.Amount = Decimal.Parse(ItemAmount.Text);
                dbInterface.UpdateInvoiceItem(SelectedInvoiceItem);
                dbInterface.UpdateInvoice(SelectedInvoice);
                generator.UpdatePlaceholders(config, SelectedInvoice, dbInterface);
            } else {
                MessageBox.Show("Please select an item to update.", "Invoice Item Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            UpdateLists();
        }

        private void RemoveInvoiceItem_Click(object sender, RoutedEventArgs e) {
            if (SelectedInvoiceItem != null) {
                dbInterface.RemoveInvoiceItem(SelectedInvoiceItem);
            } else {
                MessageBox.Show("Please select an item to remove.", "Invoice Item Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            UpdateLists();
        }

        //INVOICE CRUD

        private void AddInvoice_Click(object sender, RoutedEventArgs e) {
            try {
                if (SelectedInvoice == null) {
                    SelectedInvoice = new Invoice(
                        0,
                        (Client)InvoiceRecipient.SelectedValue,
                        (DateTime)InvoiceDate.SelectedDate,
                        0,
                        ""
                    );

                    dbInterface.AddInvoice(SelectedInvoice);
                    SelectedInvoice = dbInterface.GetInvoices().Last();
                    dbInterface.UpdateInvoiceNumber(SelectedInvoice.Client.ID);
                    SelectedInvoice.InvoiceNumber = $"INV{SelectedInvoice.Client.ID}{dbInterface.GetInvoiceNumber(SelectedInvoice.Client.ID).ToString("00000")}";
                    dbInterface.UpdateInvoice(SelectedInvoice);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Invoice Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            UpdateLists();
        }

        private void UpdateInvoice_Click(object sender, RoutedEventArgs e) {
            if (SelectedInvoice != null) {
                SelectedInvoice.Total = dbInterface.GetRelatedInvoiceItems(SelectedInvoice.ID).Sum(x => x.Amount);
                SelectedInvoice.Client = (Client)InvoiceRecipient.SelectedValue;
                SelectedInvoice.Date = (DateTime)InvoiceDate.SelectedDate;
                dbInterface.UpdateInvoice(SelectedInvoice);
            } else {
                MessageBox.Show("Please select an invoice to update.", "Invoice Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            UpdateLists();
        }

        private void RemoveInvoice_Click(object sender, RoutedEventArgs e) {
            if (SelectedInvoice != null) {
                int number = dbInterface.GetInvoiceNumber(SelectedInvoice.ID);
                dbInterface.RemoveInvoice(SelectedInvoice);
                dbInterface.UpdateInvoiceNumber(SelectedInvoice.Client.ID, number--);
            }

            UpdateLists();
        }

        //PAYROLL

        private void RunPayroll_Click(object sender, RoutedEventArgs e) {
            ExecutePayrollJob();
        }

        private void ExecutePayrollJob() {
            PayPeriod period = (PayPeriod)PayPeriod.SelectedValue;
            Payroll lastRun = dbInterface.GetPayrolls().OrderByDescending(x => x.Date).First();

            PayrollJob(period, lastRun);
        }

        private void PayrollJob(PayPeriod period, Payroll lastRun) {
            List<Shift> shifts = dbInterface.GetShifts()
                    .Where(item => item.Date >= period.Start)
                    .Where(item => item.Date < period.End)
                    .ToList();

            List<int> ids = shifts.Select(item => item.Employee.ID).Distinct().ToList();
            decimal total = (decimal)shifts.Sum(x => x.AmountEarned);

            Expense expense = new Expense(
                0,
                DateTime.Now,
                "Payroll Run",
                total,
                ExpenseCategory.Payroll,
                false,
                Frequency.OneTime
            );

            Payroll payroll = new Payroll(0, period, total);
            dbInterface.AddExpense(expense);
            dbInterface.AddPayroll(payroll);

            lastRun = dbInterface.GetPayrolls().LastOrDefault();
            expense = dbInterface.GetExpenses().LastOrDefault();

            expense.Parent = lastRun.ID;
            dbInterface.UpdateExpense(expense);

            foreach (Shift shift in shifts) {
                shift.Parent = lastRun.ID;
                dbInterface.UpdateShift(shift);
            }

            foreach (int id in ids) {
                Employee employee = shifts.Where(x => x.Employee.ID == id).Select(x => x.Employee).FirstOrDefault();
                List<Shift> s = shifts.Where(shift => shift.Employee.ID == id).ToList();
                decimal amount = s.Sum(item => item.AmountEarned);
                dbInterface.AddPayment(new Payment(0, lastRun.Date, employee, amount, lastRun.ID));
            }

            UpdateLists();
        }

        private void ReRunPayroll_Click(object sender, RoutedEventArgs e) {
            if (SelectedPayroll != null) {
                dbInterface.RemovePayroll(SelectedPayroll);
                ExecutePayrollJob();
            }
        }

        private void RemovePayroll_Click(object sender, RoutedEventArgs e) {
            if (SelectedPayroll != null) {
                dbInterface.RemovePayroll(SelectedPayroll);
                UpdateLists();
            } else {
                MessageBox.Show("Please select a payroll to remove.", "Payroll Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddScheduledExpenses() {
            List<Expense> expenses = dbInterface.GetRecurringExpenses()
                .DistinctBy(x => x.Item)
                .ToList();

            foreach (Expense expense in expenses) {
                List<Expense> recurringExpense =  dbInterface.GetRecurringExpenses(expense.Item).Where(x => x.Date >= expense.Date).ToList();
                DateTime date = expense.Date;
                DateTime EndOfYear = new DateTime(DateTime.Now.Year, 12, 31);

                if (expense.RecurrenceFrequency == Frequency.Daily) {
                    int limit = Math.Abs(expense.Date.DayOfYear - EndOfYear.DayOfYear);

                    for (int i = 0; i < limit + 1; i++) {
                        Expense current = new Expense(
                            0, date, expense.Item,
                            expense.Amount, expense.Type,
                            true, expense.RecurrenceFrequency
                        );

                        if (recurringExpense.Where(x => x.Date == current.Date).Count() == 0) {
                            dbInterface.AddExpense(current);
                        } else {
                            Expense existingExpense = recurringExpense.Where(x => x.Date == current.Date).FirstOrDefault();
                            dbInterface.RemoveExpense(existingExpense);
                            dbInterface.AddExpense(current);
                        }

                        date = date.AddDays(1);
                    }
                } else if (expense.RecurrenceFrequency == Frequency.Weekly) {
                    int limit = Math.Abs(expense.Date.DayOfYear - EndOfYear.DayOfYear) / 7;

                    for (int i = 0; i < limit + 1; i++) {
                        Expense current = new Expense(
                            0, date, expense.Item,
                            expense.Amount, expense.Type,
                            true, expense.RecurrenceFrequency
                        );

                        if (recurringExpense.Where(x => x.Date == current.Date).Count() == 0) {
                            dbInterface.AddExpense(current);
                        } else {
                            Expense existingExpense = recurringExpense.Where(x => x.Date == current.Date).FirstOrDefault();
                            dbInterface.RemoveExpense(existingExpense);
                            dbInterface.AddExpense(current);
                        }

                        date = date.AddDays(7);
                    }
                } else if (expense.RecurrenceFrequency == Frequency.Monthly) {
                    int limit = Math.Abs(expense.Date.Month - 12);

                    for (int i = 0; i < limit + 1; i++) {
                        Expense current = new Expense(
                            0, date, expense.Item,
                            expense.Amount, expense.Type,
                            true, expense.RecurrenceFrequency
                        );

                        if (recurringExpense.Where(x => x.Date == current.Date).Count() == 0) {
                            dbInterface.AddExpense(current);
                        } else {
                            Expense existingExpense = recurringExpense.Where(x => x.Date == current.Date).FirstOrDefault();
                            dbInterface.RemoveExpense(existingExpense);
                            dbInterface.AddExpense(current);
                        }

                        date = date.AddMonths(1);
                    }
                } else {
                    Expense current = new Expense(
                        0, date, expense.Item,
                        expense.Amount, expense.Type,
                        true, expense.RecurrenceFrequency
                    );

                    if (recurringExpense.Where(x => x.Date == current.Date).Count() == 0) {
                        dbInterface.AddExpense(current);
                    } else {
                        Expense existingExpense = recurringExpense.Where(x => x.Date == current.Date).FirstOrDefault();
                        dbInterface.RemoveExpense(existingExpense);
                        dbInterface.AddExpense(current);
                    }
                }
            }
        }

        private void Item_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                decimal hours = 0;
                decimal rate = 0;

                if (ItemHours.Text != "") {
                    hours = Decimal.Parse(ItemHours.Text);
                }

                if (ItemRate.Text != "") {
                    rate = Decimal.Parse(ItemRate.Text);
                }

                if (hours > 0 && rate > 0) {
                    ItemAmount.Text = (hours * rate).ToString();
                }


            } catch (Exception ex) {
                //do nothing
            }
        }

    }
}