namespace SmallBusinessSuite.Data.Models {
    class Client {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public Client(int id, string name, string contact, string email, string phoneNumber, string address) {
            ID = id;
            Name = name;
            Contact = contact;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }
        public Client() {

        }

        public override string ToString() {
            return Name;
        }

        public static Client FromCSV(string csvLine) {
            Client client = new Client();
            string[] properties = csvLine.Split(',');

            client.Name = properties[0];
            client.Contact = properties[1];
            client.PhoneNumber = properties[2];
            client.Address = properties[3];

            return client;
        }
    }
}
