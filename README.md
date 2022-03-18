# Canvas Enrollment Automation

St Mark's current configuration for this automation is as follows:

- Queries are pulled through SQL Server instead of the Education Edge user interface.  There is more control on what data can be accessed via SQL Server.  However, the queries could be created via the user interface (if there is some information that can’t be obtained feel free to reach out)
- Scripts are written in VB.Net, mostly because our automation tool plays nicer with .Net since it is a Microsoft product.  These script can easily be translated into other languages, and github should already have the PHP and others equivalent.
- Automation: Being a school Microsoft has a great licensing options for school, one of the tools available with Visual Studio is SIS which allows jobs to be created and ran at specific intervals.  Our current process flow for this job is:
  - Check if there are any changes for the day using the query created
  - If no-email group stating no changes were made today.
  - If Yes-create a csv file using the query and place it on a specified location
  - Call the SIS Upload API
  - Wait five minutes for processing
  - Call Check SIS Status API and return status
  - Email group with SIS Status
  - Enter date and Completed status into database to log successful run.  This date will be used the next day by the query.

## Query

### Add/Drop

This query will return all the changes within a given range (at St Marks we are using a table to store the last run date of this job).  The key thing to note is that  Blackbaud has 3 different change types that behave a bit different

`ChangeType = 1` means `Added` (student added a new course)

`ChangeType = 3` means `Deleted` (student dropped a course)

And `ChangeType = 4` means transferred (student has switched sections).  Change Type 4 requires additional union queries to grab the previous course (in order to delete the student from this course) and grab the new course (in order to enroll the student to the new course).

Hence there are 3 total queries unioned together.  The query is configured to spit out the values in Canvas CSV format.

## SIS Script

This is a VB.Net Script that will decrypt stored values, grab the csv file created by the SQL query, call the Canvas SIS API and upload the file. This script returns an SIS job ID
Once the file is uploaded.  I have a job that waits 5 minutes before checking the status of this SIS Job ID

## Check Status Script

This is a VB.Net Script that will decrypt stored values on the server.  This script required the SISJobId obtained from the SIS Script.  It will the Canvas check SIS Status API, and will return a string of the status of completed or with the specific errors of why the job failed.

## Email Script

This is a VB.Net Script that will email the results, of the SIS status to specified users.

## Encryption Script

This is a VB.Net Script that will encrypt locally stored variables-given that the auth token is locally stored, along with the username and password for the email script.  This class is called in each of the scripts above in order to decrypt the variables needed.


