using Microsoft.EntityFrameworkCore;
using SmallNeptun.Database;
using SmallNeptun.Entities;
using SmallNeptun.Enums.CourseEnums;
using SmallNeptun.Enums.UserEnums;

namespace SmallNeptun.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();

            if (await context.Users.AnyAsync() || await context.Subjects.AnyAsync())
            {
                return;
            }

            var semesters = CreateSemesters();
            context.Semesters.AddRange(semesters);

            var users = CreateUsers();
            context.Users.AddRange(users);

            var subjects = CreateSubjects();
            context.Subjects.AddRange(subjects);

            await context.SaveChangesAsync();

            var teachers = await context.Users
                .Where(u => u.UserType == UserType.Teacher)
                .ToListAsync();

            var students = await context.Users
                .Where(u => u.UserType == UserType.Student)
                .ToListAsync();

            var savedSubjects = await context.Subjects.ToListAsync();
            var savedSemesters = await context.Semesters.ToListAsync();

            var courses = CreateCourses(savedSubjects, savedSemesters);
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            var savedCourses = await context.Courses
                .Include(c => c.Subject)
                .ToListAsync();

            context.CourseInstructors.AddRange(CreateCourseInstructors(savedCourses, teachers));
            context.Schedules.AddRange(CreateSchedules(savedCourses));
            context.Enrollments.AddRange(CreateEnrollments(savedCourses, students));

            await context.SaveChangesAsync();
        }

        private static List<Semester> CreateSemesters()
        {
            return new List<Semester>
            {
                new Semester { AcademicYear = "2024/25", SemesterNumber = 1 },
                new Semester { AcademicYear = "2024/25", SemesterNumber = 2 },
                new Semester { AcademicYear = "2025/26", SemesterNumber = 1 },
                new Semester { AcademicYear = "2025/26", SemesterNumber = 2 },
                new Semester { AcademicYear = "2026/27", SemesterNumber = 1 },
                new Semester { AcademicYear = "2026/27", SemesterNumber = 2 }
            };
        }

        private static List<User> CreateUsers()
        {
            var users = new List<User>
            {
                new User { Name = "Admin Anna", Email = "anna.agent@smallneptun.hu", Password = "Pass1234", UserType = UserType.Agent, StudyForm = StudyForm.None },
                new User { Name = "Admin Balazs", Email = "balazs.agent@smallneptun.hu", Password = "Pass1234", UserType = UserType.Agent, StudyForm = StudyForm.None },

                new User { Name = "Dr. Kovacs Peter", Email = "kovacs.peter@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None },
                new User { Name = "Dr. Nagy Eva", Email = "nagy.eva@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None },
                new User { Name = "Dr. Szabo Daniel", Email = "szabo.daniel@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None },
                new User { Name = "Dr. Toth Lilla", Email = "toth.lilla@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None },
                new User { Name = "Dr. Horvath Mark", Email = "horvath.mark@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None },
                new User { Name = "Dr. Varga Nora", Email = "varga.nora@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None },
                new User { Name = "Dr. Farkas Mate", Email = "farkas.mate@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None },
                new User { Name = "Dr. Molnar Dora", Email = "molnar.dora@smallneptun.hu", Password = "Pass1234", UserType = UserType.Teacher, StudyForm = StudyForm.None }
            };

            var jokeStudentNames = new[] { "Git Aron", "Bekre Pal", "Gaz Ella", "Korm Odon" };

            var firstNames = new[]
            {
                "Adam", "Bence", "Csenge", "Dora", "Eszter", "Ferenc", "Gergely", "Hanna",
                "Istvan", "Judit", "Kata", "Levente", "Milan", "Nora", "Oliver", "Petra",
                "Reka", "Samuel", "Tamas", "Viktoria", "Zalan", "Luca", "Mira", "Boglarka",
                "Borbala", "Kristof", "Mate", "Noemi", "Patrik", "Zsofia"
            };

            var lastNames = new[]
            {
                "Kiss", "Nagy", "Toth", "Szabo", "Horvath", "Varga", "Molnar", "Farkas",
                "Balogh", "Papp", "Takacs", "Juhasz", "Lakatos", "Simon", "Racz"
            };

            for (var i = 0; i < 30; i++)
            {
                var studyForm = i % 3 == 0 ? StudyForm.PartTime : StudyForm.FullTime;
                users.Add(new User
                {
                    Name = i < jokeStudentNames.Length
                        ? jokeStudentNames[i]
                        : $"{lastNames[i % lastNames.Length]} {firstNames[i]}",
                    Email = $"student{i + 1:00}@smallneptun.hu",
                    Password = "Pass1234",
                    UserType = UserType.Student,
                    StudyForm = studyForm
                });
            }

            return users;
        }

        private static List<Subject> CreateSubjects()
        {
            var data = new (string Code, string Name, int Credits)[]
            {
                ("PTI001", "Az informatika logikai es algebrai alapjai", 5),
                ("PTI002", "Linearis algebra", 5),
                ("PTI003", "Matematikai alapismeretek", 4),
                ("PTI004", "A programozas alapjai", 5),
                ("PTI005", "Informacios technologia", 4),
                ("PTI006", "Programozas I.", 5),
                ("PTI007", "Web programozas I.", 4),
                ("PTI008", "Diszkret matematika", 5),
                ("PTI009", "Matematikai analizis I.", 5),
                ("PTI010", "A digitalis szamitas elmelete", 4),
                ("PTI011", "Adatstrukturak es algoritmusok I.", 5),
                ("PTI012", "Programozas II.", 5),
                ("PTI013", "Szamitogep-architekturak I.", 4),
                ("PTI014", "Web programozas II.", 4),
                ("PTI015", "Alkalmazott statisztika", 4),
                ("PTI016", "Matematikai analizis II.", 5),
                ("PTI017", "Adatbazis-kezelo rendszerek I.", 5),
                ("PTI018", "Adatstrukturak es algoritmusok II.", 5),
                ("PTI019", "Halado programozasi technikak I.", 5),
                ("PTI020", "Java programozas I.", 4),
                ("PTI021", "Szoftvertechnologia", 5),
                ("PTI022", "A rendszerfejlesztes halado modszerei", 5),
                ("PTI023", "Adatbazis-kezelo rendszerek II.", 5),
                ("PTI024", "Halado programozasi technikak II.", 5),
                ("PTI025", "Mesterseges intelligencia alapjai", 5),
                ("PTI026", "Mobil programozas", 4),
                ("PTI027", "Operacios rendszerek", 5),
                ("PTI028", "Szamitogep-halozatok I.", 5),
                ("PTI029", "Projekt labor", 5),
                ("PTI030", "Rendszerteszteles", 4),
                ("PTI031", ".NET alapu webfejlesztes", 5),
                ("PTI032", "Python programozas", 4),
                ("PTI033", "Informatikai biztonsag", 5),
                ("PTI034", "IoT rendszerek", 4),
                ("PTI035", "Virtualizacios technologiak a gyakorlatban", 4),
                ("PTI036", "Szoftverfejlesztes Qt keretrendszerrel", 4)
            };

            return data.Select(x => new Subject
            {
                Code = x.Code,
                Name = x.Name,
                Credits = x.Credits,
                IsActive = x.Code != "PTI035"
            }).ToList();
        }

        private static List<Course> CreateCourses(List<Subject> subjects, List<Semester> semesters)
        {
            var courses = new List<Course>();

            for (var i = 0; i < subjects.Count; i++)
            {
                var subject = subjects[i];
                var semester = semesters[i % semesters.Count];
                var number = i + 1;

                courses.Add(new Course
                {
                    CourseCode = $"{subject.Code}-E",
                    SubjectId = subject.Id,
                    Subject = subject,
                    SemesterId = semester.Id,
                    Semester = semester,
                    MaxStudents = 80,
                    CourseType = CourseType.Lecture,
                    CourseForm = CourseForm.Both,
                    HourType = HourType.Weekly,
                    HourCount = 2
                });

                courses.Add(new Course
                {
                    CourseCode = $"{subject.Code}-GY-N",
                    SubjectId = subject.Id,
                    Subject = subject,
                    SemesterId = semester.Id,
                    Semester = semester,
                    MaxStudents = 28,
                    CourseType = CourseType.Practice,
                    CourseForm = CourseForm.FullTime,
                    HourType = CourseTypeNeedsSemesterHours(subject.Name) ? HourType.Semester : HourType.Weekly,
                    HourCount = 2
                });

                courses.Add(new Course
                {
                    CourseCode = $"{subject.Code}-GY-L",
                    SubjectId = subject.Id,
                    Subject = subject,
                    SemesterId = semester.Id,
                    Semester = semester,
                    MaxStudents = 24,
                    CourseType = CourseType.Practice,
                    CourseForm = CourseForm.PartTime,
                    HourType = HourType.Semester,
                    HourCount = 8
                });

                if (HasLab(subject.Name) || number % 5 == 0)
                {
                    courses.Add(new Course
                    {
                        CourseCode = $"{subject.Code}-LAB",
                        SubjectId = subject.Id,
                        Subject = subject,
                        SemesterId = semester.Id,
                        Semester = semester,
                        MaxStudents = 22,
                        CourseType = CourseType.Lab,
                        CourseForm = CourseForm.Both,
                        HourType = HourType.Weekly,
                        HourCount = 2
                    });
                }
            }

            return courses;
        }

        private static List<CourseInstructor> CreateCourseInstructors(List<Course> courses, List<User> teachers)
        {
            var courseInstructors = new List<CourseInstructor>();

            for (var i = 0; i < courses.Count; i++)
            {
                courseInstructors.Add(new CourseInstructor
                {
                    CourseId = courses[i].Id,
                    TeacherId = teachers[i % teachers.Count].Id
                });

                if (i % 4 == 0)
                {
                    courseInstructors.Add(new CourseInstructor
                    {
                        CourseId = courses[i].Id,
                        TeacherId = teachers[(i + 3) % teachers.Count].Id
                    });
                }
            }

            return courseInstructors;
        }

        private static List<Schedule> CreateSchedules(List<Course> courses)
        {
            var schedules = new List<Schedule>();
            var days = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            var times = new[]
            {
                new TimeOnly(8, 0),
                new TimeOnly(10, 0),
                new TimeOnly(12, 0),
                new TimeOnly(14, 0),
                new TimeOnly(16, 0)
            };

            for (var i = 0; i < courses.Count; i++)
            {
                var course = courses[i];
                var start = times[i % times.Length];

                if (course.HourType == HourType.Weekly)
                {
                    schedules.Add(new Schedule
                    {
                        CourseId = course.Id,
                        IsWeekly = true,
                        DayOfWeek = days[i % days.Length],
                        StartTime = start,
                        EndTime = start.AddHours(1).AddMinutes(30)
                    });
                }
                else
                {
                    schedules.Add(new Schedule
                    {
                        CourseId = course.Id,
                        IsWeekly = false,
                        SpecificDate = new DateOnly(2026, 5, 18).AddDays(i % 20),
                        StartTime = start,
                        EndTime = start.AddHours(4)
                    });
                }
            }

            return schedules;
        }

        private static List<Enrollment> CreateEnrollments(List<Course> courses, List<User> students)
        {
            var enrollments = new List<Enrollment>();
            var activeSubjectGroups = courses
                .Where(c => c.Subject.IsActive)
                .GroupBy(c => c.SubjectId)
                .Take(12)
                .ToList();

            for (var studentIndex = 0; studentIndex < students.Count; studentIndex++)
            {
                var student = students[studentIndex];
                var subjectCount = studentIndex % 2 == 0 ? 6 : 4;

                foreach (var subjectCourses in activeSubjectGroups.Take(subjectCount))
                {
                    var selectableCourses = subjectCourses
                        .Where(c => c.CourseForm == CourseForm.Both ||
                                    (student.StudyForm == StudyForm.FullTime && c.CourseForm == CourseForm.FullTime) ||
                                    (student.StudyForm == StudyForm.PartTime && c.CourseForm == CourseForm.PartTime))
                        .GroupBy(c => c.CourseType)
                        .Select(g => g.First())
                        .ToList();

                    foreach (var course in selectableCourses)
                    {
                        enrollments.Add(new Enrollment
                        {
                            StudentId = student.Id,
                            CourseId = course.Id
                        });
                    }
                }
            }

            return enrollments;
        }

        private static bool HasLab(string subjectName)
        {
            var lowerName = subjectName.ToLowerInvariant();

            return lowerName.Contains("programozas") ||
                   lowerName.Contains("web") ||
                   lowerName.Contains("adatbazis") ||
                   lowerName.Contains("mobil") ||
                   lowerName.Contains("operacios") ||
                   lowerName.Contains("halozat") ||
                   lowerName.Contains("iot") ||
                   lowerName.Contains("virtualizacios");
        }

        private static bool CourseTypeNeedsSemesterHours(string subjectName)
        {
            var lowerName = subjectName.ToLowerInvariant();

            return lowerName.Contains("projekt") ||
                   lowerName.Contains("labor") ||
                   lowerName.Contains("rendszerteszteles");
        }
    }
}
