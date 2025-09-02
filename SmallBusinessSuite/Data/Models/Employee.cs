namespace SmallBusinessSuite.Data.Models {
    class Employee {
        public int ID { get; set; }
        public string Name { get; set; }
        public int DriveTime { get; set; }
        public decimal HourlyRate { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public Employee(int id, string name, int driveTime, decimal hourlyRate, string address, string phoneNumber) {
            ID = id;
            Name = name;
            DriveTime = driveTime;
            HourlyRate = hourlyRate;
            Address = address;
            PhoneNumber = phoneNumber;
        }

        public Employee() {

        }

        public override string ToString() {
            return Name;
        }

        public static Employee FromCSV(string csvLine) {
            Employee employee = new Employee();
            string[] properties = csvLine.Split(',');

            employee.Name = properties[0];
            employee.DriveTime = Int32.Parse(properties[1]);
            employee.HourlyRate = Decimal.Parse(properties[2]);
            employee.Address = properties[3];
            employee.PhoneNumber = properties[4];

            return employee;
        }
    }
}
