# documentmgr

Demo Application that accept user information and uploaded documents.

##**To Run**
* Clone the repository into visual studio
* The application is build on .Net Core 5
* Create a new MSSQL server database for the application.
* Clone the appsettings.json to appsettings.Development.json for development mode. Update the following:
  - database connection string
  - update the following under `EmailSettings` based your google settings: 
    - `SenderName` 
    - `SenderEmail` 
    - `Password`

* The application uses the code-first database development pattern. AS such on your Package Manager Console, run ``` update-database``` to execute the migration files.
* Once the migration is successful, go ahead to load the application!!!
