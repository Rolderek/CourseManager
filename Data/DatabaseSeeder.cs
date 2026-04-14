using CourseManager.Models;
using CourseManager.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Data;

public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // If there are already users, skip seeding
            if (context.Users.Any()) return;

            // USERS
            var users = new List<User>
            {
                // Administrators
                new User
                {
                    Username = "admin.kovacs",
                    Email = "kovacs.admin@university.hu",
                    PasswordHash = "hashed_password_1",
                    UserType = UserType.Administrator,
                    IsActive = true
                },

                // Instructors
                new User
                {
                    Username = "dr.nagy.peter",
                    Email = "nagy.peter@university.hu",
                    PasswordHash = "hashed_password_2",
                    UserType = UserType.Instructor,
                    IsActive = true
                },
                new User
                {
                    Username = "dr.szabo.maria",
                    Email = "szabo.maria@university.hu",
                    PasswordHash = "hashed_password_3",
                    UserType = UserType.Instructor,
                    IsActive = true
                },
                new User
                {
                    Username = "dr.horvath.janos",
                    Email = "horvath.janos@university.hu",
                    PasswordHash = "hashed_password_4",
                    UserType = UserType.Instructor,
                    IsActive = true
                },

                // Full-time students (nappali)
                new User
                {
                    Username = "kiss.anna",
                    Email = "kiss.anna@student.university.hu",
                    PasswordHash = "hashed_password_5",
                    UserType = UserType.Student,
                    StudyMode = StudyMode.FullTime,
                    IsActive = true
                },
                new User
                {
                    Username = "toth.balazs",
                    Email = "toth.balazs@student.university.hu",
                    PasswordHash = "hashed_password_6",
                    UserType = UserType.Student,
                    StudyMode = StudyMode.FullTime,
                    IsActive = true
                },
                new User
                {
                    Username = "varga.eszter",
                    Email = "varga.eszter@student.university.hu",
                    PasswordHash = "hashed_password_7",
                    UserType = UserType.Student,
                    StudyMode = StudyMode.FullTime,
                    IsActive = true
                },

                // Part-time students (levelező)
                new User
                {
                    Username = "molnar.gabor",
                    Email = "molnar.gabor@student.university.hu",
                    PasswordHash = "hashed_password_8",
                    UserType = UserType.Student,
                    StudyMode = StudyMode.PartTime,
                    IsActive = true
                },
                new User
                {
                    Username = "fekete.reka",
                    Email = "fekete.reka@student.university.hu",
                    PasswordHash = "hashed_password_9",
                    UserType = UserType.Student,
                    StudyMode = StudyMode.PartTime,
                    IsActive = true
                },
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // SUBJECTS
            var subjects = new List<Subject>
            {
                new Subject { Code = "MATH101", Name = "Mathematics I", Credits = 5, IsActive = true },
                new Subject { Code = "PROG101", Name = "Introduction to Programming", Credits = 5, IsActive = true },
                new Subject { Code = "PROG201", Name = "Advanced Programming", Credits = 4, IsActive = true },
                new Subject { Code = "DB101",   Name = "Databases", Credits = 4, IsActive = true },
                new Subject { Code = "NET101",  Name = "Computer Networks", Credits = 3, IsActive = true },
                // An inactive subject to test that behavior
                new Subject { Code = "OLD101",  Name = "Old Subject (inactive)", Credits = 2, IsActive = false },
            };

            context.Subjects.AddRange(subjects);
            context.SaveChanges();

            // COURSES
            var math = subjects[0];
            var prog1 = subjects[1];
            var prog2 = subjects[2];
            var db = subjects[3];
            var net = subjects[4];

            var courses = new List<Course>
            {
                // Mathematics - full-time lecture (weekly)
                new Course
                {
                    CourseCode = "MATH101-EA-FT",
                    SubjectId = math.Id,
                    Semester = "2024-25-2",
                    MaxStudents = 100,
                    Type = CourseType.Lecture,
                    Form = CourseForm.FullTime,
                    ScheduleType = ScheduleType.Weekly
                },
                // Mathematics - full-time practice (weekly)
                new Course
                {
                    CourseCode = "MATH101-GY-FT-A",
                    SubjectId = math.Id,
                    Semester = "2024-25-2",
                    MaxStudents = 30,
                    Type = CourseType.Practice,
                    Form = CourseForm.FullTime,
                    ScheduleType = ScheduleType.Weekly
                },
                // Mathematics - part-time lecture (block)
                new Course
                {
                    CourseCode = "MATH101-EA-PT",
                    SubjectId = math.Id,
                    Semester = "2024-25-2",
                    MaxStudents = 50,
                    Type = CourseType.Lecture,
                    Form = CourseForm.PartTime,
                    ScheduleType = ScheduleType.Block
                },

                // Programming I - full-time lecture
                new Course
                {
                    CourseCode = "PROG101-EA-FT",
                    SubjectId = prog1.Id,
                    Semester = "2024-25-2",
                    MaxStudents = 80,
                    Type = CourseType.Lecture,
                    Form = CourseForm.FullTime,
                    ScheduleType = ScheduleType.Weekly
                },
                // Programming I - full-time lab
                new Course
                {
                    CourseCode = "PROG101-LAB-FT-A",
                    SubjectId = prog1.Id,
                    Semester = "2024-25-2",
                    MaxStudents = 20,
                    Type = CourseType.Lab,
                    Form = CourseForm.FullTime,
                    ScheduleType = ScheduleType.Weekly
                },

                // Databases - mixed (both full and part time)
                new Course
                {
                    CourseCode = "DB101-EA-MIX",
                    SubjectId = db.Id,
                    Semester = "2024-25-2",
                    MaxStudents = 120,
                    Type = CourseType.Lecture,
                    Form = CourseForm.Mixed,
                    ScheduleType = ScheduleType.Weekly
                },

                // Networks - part-time block
                new Course
                {
                    CourseCode = "NET101-EA-PT",
                    SubjectId = net.Id,
                    Semester = "2024-25-2",
                    MaxStudents = 40,
                    Type = CourseType.Lecture,
                    Form = CourseForm.PartTime,
                    ScheduleType = ScheduleType.Block
                },
            };

            context.Courses.AddRange(courses);
            context.SaveChanges();

            // COURSE INSTRUCTORS
            var drNagy = users[1];
            var drSzabo = users[2];
            var drHorvath = users[3];

            var courseInstructors = new List<CourseInstructor>
            {
                new CourseInstructor { CourseId = courses[0].Id, InstructorId = drNagy.Id },
                new CourseInstructor { CourseId = courses[1].Id, InstructorId = drNagy.Id },
                new CourseInstructor { CourseId = courses[2].Id, InstructorId = drNagy.Id },
                new CourseInstructor { CourseId = courses[3].Id, InstructorId = drSzabo.Id },
                // Two instructors on the lab
                new CourseInstructor { CourseId = courses[4].Id, InstructorId = drSzabo.Id },
                new CourseInstructor { CourseId = courses[4].Id, InstructorId = drHorvath.Id },
                new CourseInstructor { CourseId = courses[5].Id, InstructorId = drHorvath.Id },
                new CourseInstructor { CourseId = courses[6].Id, InstructorId = drHorvath.Id },
            };

            context.CourseInstructors.AddRange(courseInstructors);
            context.SaveChanges();

            // SCHEDULE ENTRIES
            var now = DateTime.Now;
            // Find next Monday as a base date
            var nextMonday = now.AddDays((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7);

            var scheduleEntries = new List<ScheduleEntry>
            {
                // MATH101 Lecture - Mondays 8:00-10:00
                new ScheduleEntry
                {
                    CourseId = courses[0].Id,
                    StartTime = nextMonday.Date.AddHours(8),
                    EndTime = nextMonday.Date.AddHours(10),
                    Location = "A101"
                },
                // MATH101 Practice - Wednesdays 10:00-12:00
                new ScheduleEntry
                {
                    CourseId = courses[1].Id,
                    StartTime = nextMonday.Date.AddDays(2).AddHours(10),
                    EndTime = nextMonday.Date.AddDays(2).AddHours(12),
                    Location = "B203"
                },
                // MATH101 Part-time - block sessions (2 Saturdays)
                new ScheduleEntry
                {
                    CourseId = courses[2].Id,
                    StartTime = nextMonday.Date.AddDays(5).AddHours(8),
                    EndTime = nextMonday.Date.AddDays(5).AddHours(16),
                    Location = "A101"
                },
                new ScheduleEntry
                {
                    CourseId = courses[2].Id,
                    StartTime = nextMonday.Date.AddDays(12).AddHours(8),
                    EndTime = nextMonday.Date.AddDays(12).AddHours(16),
                    Location = "A101"
                },
                // PROG101 Lecture - Tuesdays 12:00-14:00
                new ScheduleEntry
                {
                    CourseId = courses[3].Id,
                    StartTime = nextMonday.Date.AddDays(1).AddHours(12),
                    EndTime = nextMonday.Date.AddDays(1).AddHours(14),
                    Location = "C301"
                },
                // PROG101 Lab - Thursdays 14:00-16:00
                new ScheduleEntry
                {
                    CourseId = courses[4].Id,
                    StartTime = nextMonday.Date.AddDays(3).AddHours(14),
                    EndTime = nextMonday.Date.AddDays(3).AddHours(16),
                    Location = "LAB1"
                },
                // DB101 Lecture - Fridays 8:00-10:00
                new ScheduleEntry
                {
                    CourseId = courses[5].Id,
                    StartTime = nextMonday.Date.AddDays(4).AddHours(8),
                    EndTime = nextMonday.Date.AddDays(4).AddHours(10),
                    Location = "A101"
                },
                // NET101 Part-time block - Sunday sessions
                new ScheduleEntry
                {
                    CourseId = courses[6].Id,
                    StartTime = nextMonday.Date.AddDays(6).AddHours(9),
                    EndTime = nextMonday.Date.AddDays(6).AddHours(17),
                    Location = "B101"
                },
            };

            context.ScheduleEntries.AddRange(scheduleEntries);
            context.SaveChanges();

            // ENROLLMENTS
            var anna = users[4];    // full-time student
            var balazs = users[5];  // full-time student
            var eszter = users[6];  // full-time student
            var gabor = users[7];   // part-time student
            var reka = users[8];    // part-time student

            var enrollments = new List<Enrollment>
            {
                // Anna - enrolled in Math (lecture + practice) and Prog (lecture + lab)
                new Enrollment { StudentId = anna.Id, CourseId = courses[0].Id },  // MATH lecture
                new Enrollment { StudentId = anna.Id, CourseId = courses[1].Id },  // MATH practice
                new Enrollment { StudentId = anna.Id, CourseId = courses[3].Id },  // PROG lecture
                new Enrollment { StudentId = anna.Id, CourseId = courses[4].Id },  // PROG lab

                // Balazs - enrolled in Math and DB
                new Enrollment { StudentId = balazs.Id, CourseId = courses[0].Id }, // MATH lecture
                new Enrollment { StudentId = balazs.Id, CourseId = courses[1].Id }, // MATH practice
                new Enrollment { StudentId = balazs.Id, CourseId = courses[5].Id }, // DB lecture (mixed)

                // Eszter - enrolled in Prog and DB
                new Enrollment { StudentId = eszter.Id, CourseId = courses[3].Id }, // PROG lecture
                new Enrollment { StudentId = eszter.Id, CourseId = courses[4].Id }, // PROG lab
                new Enrollment { StudentId = eszter.Id, CourseId = courses[5].Id }, // DB lecture (mixed)

                // Gabor - part-time, enrolled in Math part-time and NET
                new Enrollment { StudentId = gabor.Id, CourseId = courses[2].Id },  // MATH part-time lecture
                new Enrollment { StudentId = gabor.Id, CourseId = courses[5].Id },  // DB mixed lecture
                new Enrollment { StudentId = gabor.Id, CourseId = courses[6].Id },  // NET part-time lecture

                // Reka - part-time, enrolled in NET and DB
                new Enrollment { StudentId = reka.Id, CourseId = courses[5].Id },   // DB mixed lecture
                new Enrollment { StudentId = reka.Id, CourseId = courses[6].Id },   // NET part-time lecture
            };

            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
        }
    }
