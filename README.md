This is an AI generated summary, this is made after finished the project:

CourseManager
University Course Management System

User Manual

For Non-Programmers and Junior Developers
 
PART 1
Manual for Non-Programmers
(Administrators and Staff)

1. What is CourseManager?
CourseManager is a web-based system for managing university courses, students, instructors, and enrollments. It works similarly to the Neptun system used by Hungarian universities.

Think of it as a digital office where you can:
•	Register new students, instructors, and administrators
•	Create and manage subjects (e.g. Mathematics, Programming)
•	Create course groups within subjects
•	Enroll students into courses
•	Set up class schedules
•	Receive automatic reminders before each class

2. How to Start the System
The system runs on a computer that has the application installed. To use it:

1.	Ask your IT team or developer to start the application in Visual Studio by pressing F5.
2.	Once started, open a web browser (Chrome, Firefox, Edge).
3.	Type the following address in the browser address bar:

URL	https://localhost:7189/swagger

You will see the Swagger interface — a webpage listing all available actions you can perform in the system. This is your main control panel.

Important: The port number (7189) might be different on your computer. Check with your IT team if the address does not work.

3. Understanding the Swagger Interface
The Swagger page shows all available actions grouped by category:

•	Users — manage people in the system
•	Subjects — manage university subjects
•	Courses — manage course groups
•	Notifications — view automatic reminders

Each action has a colored label showing what type of operation it is:

GET (green)	Retrieve / view information
POST (blue)	Create new record or perform an action
PUT (yellow)	Update / modify an existing record
DELETE (red)	Permanently remove a record

4. Step-by-Step Common Tasks
4.1 Registering a New Student
4.	On the Swagger page, find the Users section.
5.	Click on POST /api/users/register.
6.	Click the "Try it out" button.
7.	Fill in the request body with the student's details:

Field	What to enter
username	The student's login name (e.g. kiss.anna)
email	Their email address (must be unique)
password	A starting password
userType	Enter: 0 for Student, 1 for Instructor, 2 for Administrator
studyMode	Enter: 0 for Full-time (nappali), 1 for Part-time (levelező). Leave empty for non-students.

8.	Click "Execute". If successful, you will see a green response with the new student's ID.

4.2 Deactivating a User
Users are never permanently deleted. Instead, they are deactivated. This keeps all their historical data intact.

9.	Find POST /api/users/{userId}/deactivate in the Users section.
10.	Click "Try it out".
11.	Enter the user's ID number in the userId field.
12.	Click "Execute".

To reactivate a user later, use POST /api/users/{userId}/reactivate in the same way.

4.3 Creating a New Subject
13.	Find POST /api/subjects in the Subjects section.
14.	Click "Try it out" and fill in:
•	code — a unique short code (e.g. MATH101)
•	name — the full name (e.g. Mathematics I)
•	credits — the credit value (e.g. 5)
15.	Click "Execute".

4.4 Enrolling a Student in a Subject
Before enrolling, you need to know:
•	The subject's ID (find it with GET /api/subjects)
•	The student's ID (find it with GET /api/users/{userId})
•	The course IDs the student wants to join

16.	Find POST /api/subjects/{subjectId}/register.
17.	Enter the subjectId in the URL field.
18.	In the request body, provide the studentId and a list of courseIds.
19.	Click "Execute".

Note: The system will automatically check all enrollment rules (study mode compatibility, capacity, semester matching) and will show an error message if any rule is violated.

4.5 Viewing Notifications
The system automatically generates reminders 30 minutes before each class. To view them:

20.	Find GET /api/notifications.
21.	Click "Try it out".
22.	Optionally enter a userId or courseId to filter results.
23.	Click "Execute" to see all notifications.

5. Important Rules to Know
•	Students are never deleted — only deactivated
•	Subjects are never deleted — only deactivated
•	A full-time student can only join full-time or mixed courses
•	A part-time student can only join part-time or mixed courses
•	A course cannot be deleted if it has enrolled students
•	Each student must enroll in exactly one course per type (lecture, practice, lab) per subject
•	All courses a student enrolls in must be from the same semester

6. Troubleshooting
Problem	Solution
Page won't open in browser	Make sure the application is running. Ask your developer to start it in Visual Studio.
"Email already in use" error	The email address is already registered. Use a different email.
"Course is full" error	The course has reached its maximum capacity. Choose a different course group.
"User is inactive" error	The user has been deactivated. Use the reactivate endpoint first.
Response shows 404 Not Found	The ID you entered does not exist. Double-check the ID number.

 
PART 2
Manual for Junior Developers
(Technical Reference)

1. Project Overview
CourseManager is a RESTful backend API built with C# .NET 9, Entity Framework Core, and Microsoft SQL Server. It manages the administrative side of a university course system — users, subjects, courses, enrollments, schedules, and notifications.

There is no frontend — the API is consumed directly via Swagger UI for testing and administration.

2. Technology Stack
Layer	Technology
Framework	.NET 9 Web API
Language	C# 13
ORM	Entity Framework Core
Database	Microsoft SQL Server (MSSQL)
API Docs	Swagger / Swashbuckle
Background Jobs	.NET BackgroundService

3. Project Structure
The project follows a clean layered architecture:

Folder	Purpose
Models/	EF Core entity classes (one per database table)
Models/Enums/	Enum definitions (UserType, CourseType, etc.)
Data/	AppDbContext and DatabaseSeeder
DTOs/	Data Transfer Objects for API input/output
Services/	Business logic layer
Controllers/	HTTP endpoint definitions
BackgroundServices/	Timed notification service
Migrations/	Auto-generated EF Core migration files

4. Database Schema
The database contains 7 tables. Enums are stored as strings for readability. Soft delete is used for Users and Subjects (IsActive flag) instead of physical deletion.

Table	Description
Users	All system users (students, instructors, admins). StudyMode only applies to students.
Subjects	University subjects with code, name, and credits.
Courses	Specific instances of subjects in a semester with type, form, and schedule type.
CourseInstructors	Junction table linking courses to their instructors (many-to-many).
Enrollments	Links students to courses. Also stores the final grade.
ScheduleEntries	Date/time slots for courses. Weekly courses have one entry; block courses have multiple.
Notifications	Log of all auto-generated pre-class reminders.

5. Setup & Running Locally
5.1 Prerequisites
•	Visual Studio 2022 or later
•	.NET 9 SDK
•	SQL Server (any edition — Express works fine)
•	SQL Server Management Studio (SSMS) — optional but recommended

5.2 Connection String
Set your connection string in appsettings.json. Replace NEPTUN with your actual Neptun code:

Connection String	Server=(local)\SQLEXPRESS;Database=CourseManagerDB_NEPTUN;Trusted_Connection=True;TrustServerCertificate=True;

5.3 First-Time Setup
24.	Clone or open the project in Visual Studio.
25.	Update the connection string in appsettings.json.
26.	Open Package Manager Console (Tools > NuGet Package Manager > Package Manager Console).
27.	Run: Add-Migration InitialCreate
28.	Run: Update-Database
29.	Press F5 to run the project.
30.	The database seeder will automatically populate test data on first run.
31.	Navigate to https://localhost:{port}/swagger to access the API.

5.4 Seed Data
The DatabaseSeeder.cs file automatically populates the database if it is empty. It creates:
•	1 administrator
•	3 instructors (Dr. Nagy, Dr. Szabó, Dr. Horváth)
•	3 full-time students (Anna, Balázs, Eszter)
•	2 part-time students (Gábor, Réka)
•	6 subjects (including 1 inactive)
•	7 courses with various types and forms
•	Realistic schedule entries and enrollments

6. API Reference
6.1 Users
Method	Endpoint	Description
POST	/api/users/register	Register a new user
GET	/api/users/{userId}	Get user by ID
PUT	/api/users/{userId}	Update username and email
PUT	/api/users/{userId}/password	Change password
POST	/api/users/{userId}/deactivate	Deactivate user (soft delete)
POST	/api/users/{userId}/reactivate	Reactivate user

6.2 Subjects
Method	Endpoint	Description
GET	/api/subjects	List all subjects
GET	/api/subjects/{subjectId}	Get subject by ID
POST	/api/subjects	Create a new subject
PUT	/api/subjects/{subjectId}	Update subject name and credits
POST	/api/subjects/{subjectId}/deactivate	Deactivate subject
POST	/api/subjects/{subjectId}/reactivate	Reactivate subject
POST	/api/subjects/{subjectId}/register	Enroll student in subject
POST	/api/subjects/{subjectId}/unregister	Unregister student from subject
GET	/api/subjects/{subjectId}/students	List students by subject and semester

6.3 Courses
Method	Endpoint	Description
POST	/api/courses	Create a new course
GET	/api/courses/{courseId}	Get course by ID
PUT	/api/courses/{courseId}	Update course details
DELETE	/api/courses/{courseId}	Delete course (only if no students)
POST	/api/courses/change	Change student from one course to another
GET	/api/courses/{courseId}/students	List students in a course
POST	/api/courses/{courseId}/schedule	Add schedule entries
POST	/api/courses/{courseId}/schedule/modify	Replace schedule entries
GET	/api/courses/{courseId}/schedule	Get course schedule

6.4 Notifications
Method	Endpoint	Description
GET	/api/notifications	Get all notifications (filter by userId and/or courseId)

7. Key Business Rules
7.1 User Rules
•	UserType cannot be changed after creation
•	StudyMode is required for students, forbidden for other types
•	Inactive users cannot be updated, enrolled, or perform any action
•	Soft delete only — use IsActive flag

7.2 Enrollment Rules
•	Students must select exactly one course per available course type (Lecture, Practice, Lab)
•	Full-time students: only FullTime or Mixed courses
•	Part-time students: only PartTime or Mixed courses
•	All selected courses must belong to the same semester
•	Course capacity (MaxStudents) must not be exceeded
•	Course change: must be within the same subject, same type, and same semester

7.3 Schedule Rules
•	Weekly courses: exactly one ScheduleEntry (conceptually repeats 14 times)
•	Block courses: one entry per session (multiple allowed)
•	EndTime must always be after StartTime
•	Modifying schedule replaces all existing entries (full replacement)

8. Background Service
The NotificationBackgroundService runs continuously in the background. Every minute it:

32.	Queries all ScheduleEntries where StartTime is between now+30min and now+31min.
33.	For each upcoming entry, generates a Notification record for every enrolled student and every assigned instructor.
34.	Skips duplicates — checks if a notification was already generated for the same user, course, and time.
35.	Logs the count of generated notifications to the console.

Notifications are stored in the database and queryable via GET /api/notifications. They serve as a log since no actual email or push notification system is integrated.

9. Adding New Features
When extending the project, follow this pattern for consistency:

36.	Create DTO classes in DTOs/{FeatureName}/ folder.
37.	Add or update the model in Models/ and run a new migration.
38.	Create a Service class in Services/ with the business logic.
39.	Create or update a Controller in Controllers/ that calls the service.
40.	Register the new service in Program.cs with builder.Services.AddScoped<YourService>().

10. Common Development Tips
•	Always use async/await for all database operations — never call .Result or .Wait().
•	Keep business logic in Services, not in Controllers.
•	Use KeyNotFoundException for missing records (returns 404) and InvalidOperationException for rule violations (returns 400).
•	After changing any Model, always run Add-Migration and Update-Database.
•	Use .Include() in EF Core queries when you need related data (navigation properties).
•	The Background Service needs IServiceScopeFactory to create its own DbContext scope — never inject AppDbContext directly into a BackgroundService.
