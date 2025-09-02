# SmallBusinessSuite
This software is intended to help small businesses track a number of common concerns such as employees, client relationships, invoices, payroll runs, payments to employees, etc. As it stands this is an overly simple software and it uses a non-secure method of data storage (SQLite) so PII and other sensitive data should not be included if you plan on adapting the codebase.

### What You'll Need to Utilize This Software As Is
- Microsoft Office (for invoicing)
- Sqlite (for the database)

### What to Expect
The SmallBusinessSuite application will create the necessary files and directories associated with them (.json config file, .dotx MS word template, and SQLite database file and its tables) upon launch. Once invoicing has begun, a sub-folder will be created in the previously created directory (C:\SmallBusinessSuite\Invoices\[client-name]\invoice-file.docx).

This application is intended to be simple in nature. As a result, many data models are missing properties that one would expect, i.e. an employee's hire date or HR actions. Reporting functionality in this software is also limited as a result.

### Features
#### Create, Read, Update, Delete
- Employee records
- Client records
- Shift records
- Expense records
- Revenue records
- Invoice records
- Invoice Item records
- Payroll records
  
#### Basic Reporting
- Monthly Balance Information
- Expense, Revenue, Shift records
- Reporting can be expanded through an ODBC connection to PowerBI or Tableau
  
#### Basic Payroll
Payroll is manually run on periods that are one week long starting and ending on Friday. All shifts in the period for which payroll is ran are gathered and payment records are generated. The software does not send cheques, but allows for the user to determine who to send cheques to and in what amounts.

#### Expenses
Expenses can be set up on a recurring basis. When this option is selected, the current year's recurring expenses are calculated on the appropriate frequency. When removing a recurring expense, any future records will be removed after the date of the record that has been altered. 

# Future Plans
As of now, there are no plans to further develop this software as it suits my needs well enough. I do, however, anticipate fixing bugs and/or improving functionality after some daily usage.

# Attributions
- Newtonsoft (https://github.com/JamesNK/Newtonsoft.Json) by James Newton-King, included under the MIT license (https://opensource.org/license/mit/)
- SQLite (https://github.com/sqlite/sqlite) which is in the public domain, included under the MIT license (https://opensource.org/license/mit/)
- Adonis UI (https://github.com/spetrik/adonis-ui-framework) by spetrik, included under the MIT license (https://opensource.org/license/mit/)
- OpenXML (https://github.com/dotnet/Open-XML-SDK) by .NET Foundation and Contributors, included under the MIT license (https://opensource.org/license/mit/)

# DISCLAIMER
Although thorough testing of this software has been conducted, there is no way to confirm this software is bug-free. As such, use of this software is done at your own risk. Due to the nature of the software, this includes risk that is financial in nature. It is your business's decision-makers' responsibility to determine whether or not this is an acceptable risk. If they (your business's decision-makers) deem it an acceptable risk, they are responsible for the software's usage and any damages that come from it.

USE OF THIS SOFTWARE IS DONE AT THE USER'S RISK. I AM NOT RESPONSIBLE FOR ANYTHING USERS OF THIS SOFTWARE DO WITH IT OR FOR ANY DAMAGES THAT COME FROM ITS USE.
