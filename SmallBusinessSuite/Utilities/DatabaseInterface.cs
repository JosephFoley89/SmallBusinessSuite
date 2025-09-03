using Microsoft.Data.Sqlite;
using SmallBusinessSuite.Data;
using SmallBusinessSuite.Data.Enums;
using SmallBusinessSuite.Data.Models;
using System.IO;

namespace SmallBusinessSuite.Utilities {
    class DatabaseInterface {
        private static SqliteConnection connection;
        private static int RowLimit;

        public DatabaseInterface(int rowLimit) {
            if (!DatabaseCreated()) {
                connection = new SqliteConnection("Data Source=C:\\SmallBusinessSuite\\SmallBusinessData.db");
            }

            RowLimit = rowLimit;
        }

        private static void CreateDatabaseTables() {
            connection = new SqliteConnection("Data Source=C:\\SmallBusinessSuite\\SmallBusinessData.db");
            List<SqliteCommand> commands = new List<SqliteCommand>();

            //Client table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE clients(
                        id INTEGER PRIMARY KEY,
                        name TEXT NOT NULL,
                        contact TEXT,
                        email TEXT,
                        phone_number TEXT,
                        address TEXT
                    );"
                )
            );

            //Employee table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE employees(
                        id INTEGER PRIMARY KEY,
                        name TEXT NOT NULL,
                        drive_time DOUBLE NOT NULL,
                        wage DECIMAL(10,5) NOT NULL,
                        address TEXT,
                        phone_number TEXT
                    );"
                )
            );

            //Expense table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE expenses(
                        id INTEGER PRIMARY KEY,
                        date TEXT NOT NULL,
                        item TEXT NOT NULL,
                        amount DECIMAL(10,5) NOT NULL,
                        type TEXT NOT NULL,
                        is_recurring BOOLEAN NOT NULL,
                        frequency TEXT,
                        payroll INTEGER,
                        FOREIGN KEY (payroll) REFERENCES payroll_runs(id)
                    );"
                )
            );

            //Payment table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE payments (
                        id INTEGER PRIMARY KEY,
                        date TEXT NOT NULL,
                        employee INTEGER NOT NULL,
                        amount DECIMAL(10,5) NOT NULL,
                        payroll INTEGER NOT NULL,
                        FOREIGN KEY(payroll) REFERENCES payroll_runs(id),
                        FOREIGN KEY(employee) REFERENCES employees(id)
                    );"
                )
            );

            //PayPeriod table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE pay_periods(
                        id INTEGER PRIMARY KEY,
                        start_date TEXT NOT NULL,
                        end_date TEXT NOT NULL,
                        period TEXT NOT NULL
                    );"
                )
            );

            //Payroll table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE payroll_runs(
                        id INTEGER PRIMARY KEY,
                        pay_period INTEGER NOT NULL,
                        amount_paid DECIMAL NOT NULL,
                        payment_date TEXT NOT NULL,
                        FOREIGN KEY(pay_period) REFERENCES pay_periods(id)
                    );"
                )
            );

            //Revenue table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE revenue_items(
                        id INTEGER PRIMARY KEY,
                        date TEXT NOT NULL,
                        item TEXT NOT NULL,
                        amount DECIMAL(10,5) NOT NULL,
                        type TEXT NOT NULL
                    );"
                )
            );

            //Shift table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE shifts(
                        id INTEGER PRIMARY KEY,
                        employee INTEGER NOT NULL,
                        date TEXT NOT NULL,
                        time_worked DOUBLE NOT NULL,
                        amount_earned DECIMAL(10,5) NOT NULL,
                        client INTEGER NOT NULL,
                        payroll INTEGER,
                        FOREIGN KEY (client) REFERENCES clients(id),
                        FOREIGN KEY (employee) REFERENCES employees(id),
                        FOREIGN KEY (payroll) REFERENCES payroll_runs(id)
                    );"
                )
            );

            //Invoice table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE invoices(
                        id INTEGER PRIMARY KEY,
                        client INTEGER NOT NULL,
                        date TEXT NOT NULL,
                        payment_date TEXT,
                        total DECIMAL(10,5) NOT NULL,
                        invoice_number TEXT,
                        FOREIGN KEY (client) REFERENCES clients(id)
                    );"
                )    
            );

            //InvoiceItem table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE invoice_items(
                        id INTEGER PRIMARY KEY,
                        work_order INTEGER,
                        description DECIMAL(10,5) NOT NULL,
                        hours DOUBLE NOT NULL,
                        rate DECIMAL(10,5),
                        amount DECIMAL(10,5) NOT NULL,
                        invoice INTEGER NOT NULL,
                        FOREIGN KEY (invoice) REFERENCES invoices(id)
                    );"
                )
            );

            //Client Invoice table
            commands.Add(
                new SqliteCommand(
                    @"CREATE TABLE client_invoice(
                    id INTEGER PRIMARY KEY,
                    client INTEGER NOT NULL,
                    invoice_number INTEGER NOT NULL,
                    FOREIGN KEY (client) REFERENCES clients(id)
                    );"    
                )    
            );

            OpenConnectionIfNecessary();

            foreach (SqliteCommand command in commands) {
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
        }

        private static bool DatabaseCreated() {
            if (File.Exists(@"C:\SmallBusinessSuite\SmallBusinessData.db")) {
                return false;
            }

            CreateDatabaseTables();
            return true;
        }

        private static void OpenConnectionIfNecessary() {
            if (connection.State == System.Data.ConnectionState.Closed) {
                connection.Open();
            }
        }

        //Lists of Records
        public List<Client> GetClients(int? id = null) {
            List<Client> clients = new List<Client>();

            string sql = id == null ? 
                $"SELECT * FROM clients LIMIT {RowLimit};" : 
                $"SELECT * FROM clients WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();
            command.Connection = connection;

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    clients.Add(
                        new Client(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5)
                        )    
                    );
                }
            }

            return clients;
        }

        public List<Employee> GetEmployees(int? id = null) {
            List<Employee> employees = new List<Employee>();

            string sql = id == null ?
                $"SELECT * FROM employees LIMIT {RowLimit};" :
                $"SELECT * FROM employees WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();
            command.Connection = connection;

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    employees.Add(
                        new Employee(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetInt32(2),
                            reader.GetDecimal(3),
                            reader.GetString(4),
                            reader.GetString(5)
                        )
                    );
                }
            }

            return employees;
        }

        public List<Expense> GetExpenses(int? id = null) {
            List<Expense> expenses = new List<Expense>();

            string sql = id == null ? 
                $"SELECT * FROM expenses LIMIT {RowLimit};" : 
                $"SELECT * FROM expenses WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    Enum.TryParse(reader.GetString(4), out ExpenseCategory category);
                    Enum.TryParse(reader.GetString(6), out Frequency frequency);

                    expenses.Add(
                        new Expense(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetString(2),
                            reader.GetDecimal(3),
                            category,
                            reader.GetBoolean(5),
                            frequency
                        )
                    );
                }
            }

            return expenses;
        }

        public List<Expense> GetRecurringExpenses(string? item = null) {
            List<Expense> expenses = new List<Expense>();

            string sql = item == null ?
                "SELECT * FROM expenses WHERE is_recurring = 1" :
                $"SELECT * FROM expenses WHERE is_recurring = 1 AND item = '{item}'";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    Enum.TryParse(reader.GetString(4), out ExpenseCategory category);
                    Enum.TryParse(reader.GetString(6), out Frequency frequency);

                    expenses.Add(
                        new Expense(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetString(2),
                            reader.GetDecimal(3),
                            category,
                            reader.GetBoolean(5),
                            frequency
                        )
                    );
                }
            }

            return expenses;
        } 

        public List<PayPeriod> GetPayPeriods(int? id = null) {
            List<PayPeriod> periods = new List<PayPeriod>();

            string sql = id == null ?
                $"SELECT * FROM pay_periods LIMIT {RowLimit};" :
                $"SELECT * FROM pay_periods WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    periods.Add(
                        new PayPeriod(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetDateTime(2)
                        )
                    );
                }
            }

            return periods;
        }

        public List<Payment> GetPayments(int? id = null) {
            List<Payment> payments = new List<Payment>();

            string sql = id == null ?
                $"SELECT * FROM payments LIMIT {RowLimit};" :
                $"SELECT * FROM payments WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    payments.Add(
                        new Payment(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            GetEmployees(reader.GetInt32(2))[0],
                            reader.GetDecimal(3),
                            GetPayrolls(reader.GetInt32(4))[0].ID
                        )
                    );
                }
            }

            return payments;
        }

        public List<Payroll> GetPayrolls(int? id = null) {
            List<Payroll> payrolls = new List<Payroll>();

            string sql = id == null ?
                $"SELECT * FROM payroll_runs LIMIT {RowLimit};" :
                $"SELECT * FROM payroll_runs WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    payrolls.Add(
                        new Payroll(
                            reader.GetInt32(0),
                            GetPayPeriods(reader.GetInt32(1))[0],
                            reader.GetDecimal(2)
                        )
                    );
                }
            }

            return payrolls;
        }

        public List<Revenue> GetRevenues(int? id = null) {
            List<Revenue> revenues = new List<Revenue>();

            string sql = id == null ?
                $"SELECT * FROM revenue_items LIMIT {RowLimit};" :
                $"SELECT * FROM revenue_items WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);
            OpenConnectionIfNecessary();

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    Enum.TryParse(reader.GetString(4), out RevenueCategory category);

                    revenues.Add(
                        new Revenue(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetString(2),
                            reader.GetDecimal(3),
                            category
                        )    
                    );
                }
            }

            return revenues;
        }

        public List<Shift> GetShifts(int? id = null) {
            List<Shift> shifts = new List<Shift>();

            string sql = id == null ?
                $"SELECT * FROM shifts LIMIT {RowLimit};" :
                $"SELECT * FROM shifts WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);
            OpenConnectionIfNecessary();

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    shifts.Add(
                        new Shift(
                            reader.GetInt32(0),
                            GetEmployees(reader.GetInt32(1))[0],
                            reader.GetDateTime(2),
                            reader.GetDouble(3),
                            GetClients(reader.GetInt32(5))[0]
                        )
                    );
                }
            }

            return shifts;
        }

        public List<Invoice> GetInvoices(int? id = null) {
            List<Invoice> invoices = new List<Invoice>();

            string sql = id == null ?
                $"SELECT * FROM invoices LIMIT {RowLimit};" :
                $"SELECT * FROM invoices WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);
            OpenConnectionIfNecessary();

            using (SqliteDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    DateTime? PaymentDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3);
                    invoices.Add(
                        new Invoice(
                            reader.GetInt32(0),
                            GetClients(reader.GetInt32(1))[0],
                            reader.GetDateTime(2),
                            PaymentDate,
                            reader.GetDecimal(4),
                            reader.GetString(5)
                        )
                    );
                }
            }

            return invoices;
        }

        public List<InvoiceItem> GetInvoiceItems(int? id = null) {
            List<InvoiceItem> items = new List<InvoiceItem>();

            string sql = id == null ?
                $"SELECT * FROM invoice_items LIMIT {RowLimit};" :
                $"SELECT * FROM invoice_items WHERE id = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);
            OpenConnectionIfNecessary();

            using (SqliteDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    items.Add(
                        new InvoiceItem(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetDouble(3),
                            reader.GetDecimal(4),
                            reader.GetDecimal(5),
                            GetInvoices(reader.GetInt32(6))[0]
                        )
                    );
                }
            }

            return items;
        }

        public List<Shift> GetRelatedShifts(int id) {
            List<Shift> shifts = new List<Shift>();
            string sql = $"SELECT * FROM shifts WHERE payroll = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);
            OpenConnectionIfNecessary();

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    shifts.Add(
                        new Shift(
                            reader.GetInt32(0),
                            GetEmployees(reader.GetInt32(1))[0],
                            reader.GetDateTime(2),
                            reader.GetDouble(3),
                            GetClients(reader.GetInt32(5))[0]
                        )
                    );
                }
            }

            return shifts;
        }

        public List<Payment> GetRelatedPayments(int id) {
            List<Payment> payments = new List<Payment>();
            string sql = $"SELECT * FROM payments WHERE payroll = {id};";

            SqliteCommand command = new SqliteCommand(sql, connection);

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    payments.Add(
                        new Payment(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            GetEmployees(reader.GetInt32(2))[0],
                            reader.GetDecimal(3),
                            GetPayrolls(reader.GetInt32(4))[0].ID
                        )
                    );
                }
            }

            return payments;
        }

        public List<InvoiceItem> GetRelatedInvoiceItems(int id) {
            List<InvoiceItem> items = new List<InvoiceItem>();
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM invoice_items WHERE invoice = {id}",
                connection
            );

            OpenConnectionIfNecessary();
            using (SqliteDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    items.Add(
                        new InvoiceItem(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetDouble(3),
                            reader.GetDecimal(4),
                            reader.GetDecimal(5),
                            GetInvoices(reader.GetInt32(6))[0]
                        )
                    );
                }
            }

            return items;
        }

        public int GetInvoiceNumber(int id) {
            int invoiceNumber = 1;
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM client_invoice WHERE client = {id}",
                connection
            );

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    invoiceNumber = reader.GetInt32(2);
                }
            }

            return invoiceNumber;
        }

        public List<Expense> GetExpenseByYear(int year) {
            List<Expense> expenses = new List<Expense>();
            
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM expenses WHERE STRFTIME('%Y',date)='{year}'",
                connection
            );

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    Enum.TryParse(reader.GetString(4), out ExpenseCategory category);
                    Enum.TryParse(reader.GetString(6), out Frequency frequency);

                    expenses.Add(
                        new Expense(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetString(2),
                            reader.GetDecimal(3),
                            category,
                            reader.GetBoolean(5),
                            frequency
                        )
                    );
                }
            }

            return expenses;
        }

        public List<Revenue> GetRevenuesByYear(int year) {
            List<Revenue> revenues = new List<Revenue>();

            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM revenue_items WHERE STRFTIME('%Y',date)='{year}'",
                connection
            );

            OpenConnectionIfNecessary();

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    Enum.TryParse(reader.GetString(4), out RevenueCategory category);

                    revenues.Add(
                        new Revenue(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetString(2),
                            reader.GetDecimal(3),
                            category
                        )
                    );
                }
            }

            return revenues;
        }

        //CLIENT CRUD

        public void AddClient(Client client) {
            SqliteCommand command = new SqliteCommand(
                "INSERT INTO clients (name,contact,email,phone_number,address)"
                +"VALUES (@name, @contact, @email, @phone, @address);",
                connection
            );

            command.Parameters.AddWithValue("@name", client.Name);
            command.Parameters.AddWithValue("@contact", client.Contact);
            command.Parameters.AddWithValue("@email", client.Email);
            command.Parameters.AddWithValue("@phone", client.PhoneNumber);
            command.Parameters.AddWithValue("@address", client.Address);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdateClient(Client client) {
            SqliteCommand command = new SqliteCommand(
                @"UPDATE clients
                SET name = @name,
                contact = @contact,
                email = @email,
                phone_number = @phone,
                address = @address
                WHERE id = @id;",
                connection
            );

            command.Parameters.AddWithValue("@name", client.Name);
            command.Parameters.AddWithValue("@contact", client.Contact);
            command.Parameters.AddWithValue("@email", client.Email);
            command.Parameters.AddWithValue("@phone", client.PhoneNumber);
            command.Parameters.AddWithValue("@address", client.Address);
            command.Parameters.AddWithValue("@id", client.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemoveClient(Client client) {
            SqliteCommand command = new SqliteCommand(
                @"DELETE FROM clients
                WHERE id = @id;",
                connection
            );

            command.Parameters.AddWithValue("@id", client.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public bool ClientExists(string name) {
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM clients WHERE name = '{name}'",
                connection
            );

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                return reader.HasRows;
            }
        }

        //EMPLOYEE CRUD

        public void AddEmployee(Employee employee) {
            SqliteCommand command = new SqliteCommand(
                "INSERT INTO employees (name,drive_time,wage,address,phone_number)"
                + "VALUES (@name, @drive, @wage, @address, @phone);",
                connection
            );

            command.Parameters.AddWithValue("@name", employee.Name);
            command.Parameters.AddWithValue("@drive", employee.DriveTime);
            command.Parameters.AddWithValue("@wage", employee.HourlyRate);
            command.Parameters.AddWithValue("@address", employee.Address);
            command.Parameters.AddWithValue("@phone", employee.PhoneNumber);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdateEmployee(Employee employee) {
            SqliteCommand command = new SqliteCommand(
                @"UPDATE employees
                SET name = @name,
                drive_time = @drive,
                wage = @wage,
                address = @address,
                phone_number = @phone
                WHERE id = @id;",
                connection
            );

            command.Parameters.AddWithValue("@name", employee.Name);
            command.Parameters.AddWithValue("@drive", employee.DriveTime);
            command.Parameters.AddWithValue("@wage", employee.HourlyRate);
            command.Parameters.AddWithValue("@address", employee.Address);
            command.Parameters.AddWithValue("@phone", employee.PhoneNumber);
            command.Parameters.AddWithValue("@id", employee.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemoveEmployee(Employee employee) {
            SqliteCommand command = new SqliteCommand(
                @"DELETE FROM employees
                WHERE id = @id;",
                connection
            );

            command.Parameters.AddWithValue("@id", employee.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public bool EmployeeExists(string name) {
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM employees WHERE name = '{name}'",
                connection
            );

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                return reader.HasRows;
            }
        }

        //EXPENSE CRUD

        public void AddExpense(Expense expense) {
            SqliteCommand command = new SqliteCommand(
                "INSERT INTO expenses (date,item,amount,type,is_recurring,frequency)"
                + "VALUES(@date, @item, @amount, @type, @recurring, @rf)",
                connection
            );

            command.Parameters.AddWithValue("@date", expense.Date);
            command.Parameters.AddWithValue("@item", expense.Item);
            command.Parameters.AddWithValue("@amount", expense.Amount);
            command.Parameters.AddWithValue("@type", expense.Type);
            command.Parameters.AddWithValue("@recurring", expense.IsRecurring);
            command.Parameters.AddWithValue("@rf", expense.RecurrenceFrequency);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdateExpense(Expense expense) {
            string sql = expense.Parent != null ?
                @"UPDATE expenses
                SET date = @date,
                item = @item,
                amount = @amount,
                type = @type,
                is_recurring = @recurring,
                frequency = @rf,
                payroll = @payroll
                WHERE id = @id;"
                : 
                @"UPDATE expenses
                SET date = @date,
                item = @item,
                amount = @amount,
                type = @type,
                is_recurring = @recurring,
                frequency = @rf
                WHERE id = @id;";


            SqliteCommand command = new SqliteCommand(sql, connection);

            if (expense.Parent != null) {
                command.Parameters.AddWithValue("@payroll", expense.Parent);
            }

            command.Parameters.AddWithValue("@date", expense.Date);
            command.Parameters.AddWithValue("@item", expense.Item);
            command.Parameters.AddWithValue("@amount", expense.Amount);
            command.Parameters.AddWithValue("@type", expense.Type);
            command.Parameters.AddWithValue("@recurring", expense.IsRecurring);
            command.Parameters.AddWithValue("@rf", expense.RecurrenceFrequency);
            command.Parameters.AddWithValue("@id", expense.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemoveExpense(Expense expense) {
            SqliteCommand command = new SqliteCommand(
                @"DELETE FROM expenses
                WHERE id = @id;",
                connection
            );

            command.Parameters.AddWithValue("@id", expense.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        //PAYMENT CRUD

        public void AddPayment(Payment payment) {
            SqliteCommand command = new SqliteCommand(
                @"INSERT INTO payments (date,employee,amount,payroll)
                VALUES(@date, @employee, @amount, @payroll);",
                connection
            );

            command.Parameters.AddWithValue("@date", payment.Date);
            command.Parameters.AddWithValue("@amount", payment.Amount);
            command.Parameters.AddWithValue("@employee", payment.Employee.ID);
            command.Parameters.AddWithValue("@payroll", payment.PayrollID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdatePayment(Payment payment) {
            SqliteCommand command = new SqliteCommand(
                @"UPDATE payments 
                SET date = @date,
                employee = @employee,
                amount = @amount,
                WHERE id = @id;",
                connection
            );

            command.Parameters.AddWithValue("@date", payment.Date);
            command.Parameters.AddWithValue("@employee", payment.Employee.ID);
            command.Parameters.AddWithValue("@amount", payment.Amount);
            command.Parameters.AddWithValue("@id", payment.ID);
        }

        public void RemovePayment(Payment payment) {
            SqliteCommand command = new SqliteCommand(
                @"DELETE FROM payments
                WHERE id = @id;",
                connection
            );

            command.Parameters.AddWithValue("@id", payment.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        //PayPeriod Creation -- no updating / deleting
     
        public void AddPayPeriod(PayPeriod period) {
            SqliteCommand command = new SqliteCommand(
                @"INSERT INTO pay_periods (start_date,end_date,period)
                VALUES(@start,@end,@period)", 
                connection
            );

            command.Parameters.AddWithValue("@start", period.Start);
            command.Parameters.AddWithValue("@end", period.End);
            command.Parameters.AddWithValue("@period", period.Period);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        //PAYROLL CRUD

        public void AddPayroll(Payroll payroll) {
            SqliteCommand command = new SqliteCommand(
                @"INSERT INTO payroll_runs (pay_period,amount_paid,payment_date)
                VALUES(@period, @total, @date)",
                connection
            );

            command.Parameters.AddWithValue("@period", payroll.Period.ID);
            command.Parameters.AddWithValue("@total", payroll.TotalPaid);
            command.Parameters.AddWithValue("@date", payroll.Date);
            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemovePayroll(Payroll payroll) {
            List<SqliteCommand> commands = new List<SqliteCommand>();
            commands.Add(new SqliteCommand(
                $"DELETE FROM payments WHERE payroll = {payroll.ID};",
                connection
            ));

            commands.Add(new SqliteCommand(
                $"DELETE FROM expenses WHERE payroll = {payroll.ID};",
                connection
            ));

            commands.Add(new SqliteCommand(
                $"UPDATE shifts SET payroll = NULL WHERE payroll = {payroll.ID}",
                connection
            ));

            commands.Add(new SqliteCommand(
                $"DELETE FROM payroll_runs WHERE id = {payroll.ID};",
                connection
            ));

            foreach (SqliteCommand command in commands) {
                command.Parameters.AddWithValue("@id", payroll.ID);
                OpenConnectionIfNecessary();
                command.ExecuteNonQuery();
            }
        }

        //SHIFT CRUD
        
        public void AddShift(Shift shift) {
            SqliteCommand command = new SqliteCommand(
                @"INSERT INTO shifts (date,employee,time_worked,amount_earned,client)
                VALUES(@date, @employee, @time, @amount, @client)",
                connection
            );

            command.Parameters.AddWithValue("@date", shift.Date);
            command.Parameters.AddWithValue("@employee", shift.Employee.ID);
            command.Parameters.AddWithValue("@time", shift.TimeWorked);
            command.Parameters.AddWithValue("@amount", shift.AmountEarned);
            command.Parameters.AddWithValue("@client", shift.Client.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdateShift(Shift shift) {
            string sql = shift.Parent == null ?
                @"UPDATE shifts
                SET date = @date,
                employee = @employee,
                time_worked = @time,
                amount_earned = @amount,
                client = @client
                WHERE id = @id" 
                :
                @"UPDATE shifts
                SET date = @date,
                employee = @employee,
                time_worked = @time,
                amount_earned = @amount,
                client = @client,
                payroll = @payroll
                WHERE id = @id";

            SqliteCommand command = new SqliteCommand(
                sql,
                connection
            );

            if (shift.Parent != null) {
                command.Parameters.AddWithValue("@payroll", shift.Parent);
            }

            command.Parameters.AddWithValue("@date", shift.Date);
            command.Parameters.AddWithValue("@employee", shift.Employee.ID);
            command.Parameters.AddWithValue("@time", shift.TimeWorked);
            command.Parameters.AddWithValue("@amount", shift.AmountEarned);
            command.Parameters.AddWithValue("@client", shift.Client.ID);
            command.Parameters.AddWithValue("@id", shift.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemoveShift(Shift shift) {
            SqliteCommand command = new SqliteCommand(
                @"DELETE FROM shifts
                WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("@id", shift.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public bool ShiftExists(int employeeID, DateTime date) {
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM shifts WHERE employee = {employeeID} AND date = '{date.ToString("yyyy-MM-dd HH:mm:ss")}'",
                connection
            );

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                return reader.HasRows;
            }
        }

        //REVENUE CRUD
        public void AddRevenue(Revenue revenue) {
            SqliteCommand command = new SqliteCommand(
                @"INSERT INTO revenue_items (date,item,amount,type)
                VALUES(@date, @item, @amount, @type)",
                connection
            );

            command.Parameters.AddWithValue("@date", revenue.Date);
            command.Parameters.AddWithValue("@item", revenue.Item);
            command.Parameters.AddWithValue("@amount", revenue.Amount);
            command.Parameters.AddWithValue("@type", revenue.Type);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdateRevenue(Revenue revenue) {
            SqliteCommand command = new SqliteCommand(
                @"UPDATE revenue_items
                SET date = @date,
                item = @item,
                amount = @amount,
                type = @type
                WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("@date", revenue.Date);
            command.Parameters.AddWithValue("@item", revenue.Item);
            command.Parameters.AddWithValue("@amount", revenue.Amount);
            command.Parameters.AddWithValue("@type", revenue.Type);
            command.Parameters.AddWithValue("@id", revenue.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemoveRevenue(Revenue revenue) {
            SqliteCommand command = new SqliteCommand(
                $"DELETE FROM revenue_items WHERE id = {revenue.ID}",
                connection
            );

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public bool InvoicePaymentExists(Revenue revenue) {
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM revenue_items WHERE item = '{revenue.Item}'",
                connection
            );

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                return reader.HasRows;
            }
        }

        //INVOICE ITEM CRUD

        public void AddInvoiceItem(InvoiceItem item) {
            SqliteCommand command = new SqliteCommand(
                $"INSERT INTO invoice_items (work_order, description, hours, rate, amount, invoice) VALUES ('{item.WorkOrder}', '{item.Description}', {item.Hours}, {item.Rate}, {item.Amount}, {item.Parent.ID})",
                connection
            );

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdateInvoiceItem(InvoiceItem item) {
            SqliteCommand command = new SqliteCommand(
                @"UPDATE invoice_items
                SET work_order = @work,
                description = @desc,
                hours = @hours,
                rate = @rate,
                amount = @amount,
                invoice = @parent
                WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("@work", item.WorkOrder);
            command.Parameters.AddWithValue("@desc", item.Description);
            command.Parameters.AddWithValue("@hours", item.Hours);
            command.Parameters.AddWithValue("@rate", item.Rate);
            command.Parameters.AddWithValue("@amount", item.Amount);
            command.Parameters.AddWithValue("@parent", item.Parent.ID);
            command.Parameters.AddWithValue("@id", item.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemoveInvoiceItem(InvoiceItem item) {
            SqliteCommand command = new SqliteCommand(
                $"DELETE FROM invoice_items WHERE id = {item.ID}",
                connection
            );

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        //INVOICE CRUD

        public void AddInvoice(Invoice invoice) {
            SqliteCommand command = new SqliteCommand(
                $"INSERT INTO invoices (client, date, payment_date, total, invoice_number) VALUES({invoice.Client.ID}, '{invoice.Date}', '{invoice.PaymentDate}', {invoice.Total}, '{invoice.InvoiceNumber}')",
                connection
            );

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void UpdateInvoice(Invoice invoice) {
            string sql = invoice.PaymentDate == null ?
                @"UPDATE invoices 
                SET client = @client,
                date = @date,
                total = @total,
                invoice_number = @number
                WHERE id = @id"
                :
                @"UPDATE invoices 
                SET client = @client,
                date = @date,
                payment_date = @pDate,
                total = @total,
                invoice_number = @number
                WHERE id = @id";

            SqliteCommand command = new SqliteCommand(sql, connection);

            if (invoice.PaymentDate != null) {
                command.Parameters.AddWithValue("@pDate", invoice.PaymentDate);
            }

            command.Parameters.AddWithValue("@client", invoice.Client.ID);
            command.Parameters.AddWithValue("@date", invoice.Date);
            command.Parameters.AddWithValue("@total", invoice.Total);
            command.Parameters.AddWithValue("@number", invoice.InvoiceNumber);
            command.Parameters.AddWithValue("@id", invoice.ID);

            OpenConnectionIfNecessary();
            command.ExecuteNonQuery();
        }

        public void RemoveInvoice(Invoice invoice) {
            SqliteCommand relatives = new SqliteCommand(
                $"DELETE FROM invoice_items WHERE invoice = {invoice.ID}",
                connection
            );

            SqliteCommand command = new SqliteCommand(
                $"DELETE FROM invoices WHERE id = {invoice.ID}",
                connection
            );

            OpenConnectionIfNecessary();
            relatives.ExecuteNonQuery();
            command.ExecuteNonQuery();
        }

        //INVOICE NUMBER

        public void UpdateInvoiceNumber(int id, int? invoiceNumber = null) {
            SqliteCommand command = new SqliteCommand(
                $"SELECT * FROM client_invoice WHERE client = {id}",
                connection
            );

            OpenConnectionIfNecessary();
            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    if (invoiceNumber == null) {
                        invoiceNumber = reader.GetInt32(2) + 1;
                    }
                }

                string sql = reader.HasRows ?
                    $"UPDATE client_invoice SET invoice_number = {invoiceNumber} WHERE id = {id}" :
                    $"INSERT INTO client_invoice (client, invoice_number) VALUES({id}, '{invoiceNumber}')";

                SqliteCommand insertOrUpdate = new SqliteCommand(
                    sql,
                    connection
                );

                insertOrUpdate.ExecuteNonQuery();
            }
        }
    }
}
