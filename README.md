PSU SWENG 861 MUSIC APP
Description
This ASP.NET MVC application allows users to search for songs and artists, add them to their playlist, and edit or delete their selections.

Prerequisites
Before you begin, ensure you have met the following requirements:

Visual Studio: Install the latest version of Visual Studio with the ASP.NET and web development workload.
SQL Server: Install SQL Server.
Installation
Clone the repository and open the project:

bash
Copy code
git clone https://github.com/john-mccoubrie/SWENG861MusicApp.git
cd SWENG861MusicApp
Open the solution file MusicApp.sln in Visual Studio.

Configuring the Database
Open SQL Server Management Studio (SSMS) and connect to your local SQL Server instance.
In Visual Studio, open the Package Manager Console:
Add-Migration InitialCreate
Update-Database
These commands scaffold a migration based on changes you make to your models and apply updates to the database respectively.
Update Connection String
Modify the connection string in the web.config file to match your SQL Server configuration:

Copy code
<connectionStrings>
    <add name="DefaultConnection" connectionString="Server=your_server_name; Database=your_database_name; Integrated Security=True;" providerName="System.Data.SqlClient"/>
</connectionStrings>
Running the Application
To run the MusicApp:

Press F5 or click the "Start Debugging" button in Visual Studio.
The application builds and opens in a web browser displaying the home page.
Using MusicApp
Access the Application: Navigate to http://localhost:xxxx/.
Navigation: Use the top menu to access different pages - Home, Search, and Playlist.

Contact
If you need to contact me, you can reach me at jhm5262@psu.edu.