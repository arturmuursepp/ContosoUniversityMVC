using ContosoUniversity.Models;

namespace ContosoUniversity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            //otsib õpilasi
            if (context.Students.Any())
            {
                return; //väljub meetodist kui andmebaas sisaldab juba andmeid ning meetodis kirjeldatud näidisõpilasi
                        //kursuseid ja aineosalusi ei lisata
            }

            var students = new Student[]
            {
                new Student {FirstMidName="Markus", LastName="Nirgi", EnrollmentDate=DateTime.Parse("2022-09-01")},
                new Student {FirstMidName="Virkus", LastName="Uusküla", EnrollmentDate=DateTime.Parse("2022-09-01")},
                new Student {FirstMidName="Juuli", LastName="Virtsu", EnrollmentDate=DateTime.Parse("2022-09-01")},
                new Student {FirstMidName="Maali", LastName="Plärtsu", EnrollmentDate=DateTime.Parse("2022-09-01")},
                new Student {FirstMidName="Beta", LastName="Male", EnrollmentDate=DateTime.Parse("2022-09-01")},
                new Student {FirstMidName="Alpha", LastName="Chad", EnrollmentDate=DateTime.Now}
            };

            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();

            var instructors = new Instructor[]
            {
                new Instructor {FirstMidName = "Kim", LastName ="Kardashian", HireDate=DateTime.Parse("2005-09-01")},
                new Instructor {FirstMidName = "Mr", LastName ="Bean", HireDate=DateTime.Parse("2015-09-01")},
                new Instructor {FirstMidName = "Pikachu", LastName ="Reynolds", HireDate=DateTime.Parse("2010-09-01")},
                new Instructor {FirstMidName = "Balti jaama parm", LastName ="Oss", HireDate=DateTime.Parse("2069-09-01")},
                new Instructor {FirstMidName = "Alar", LastName ="Karis", HireDate=DateTime.Parse("2023-09-01")}
            };

            foreach (Instructor i in instructors)
            {
                context.Instructors.Add(i);
            }
            context.SaveChanges();

            var departments = new Department[]
            {
                new Department {Name="Infotechnology",Budget=100,StartDate=DateTime.Parse("2005-09-01"), InstructorID=instructors.Single(i => i.LastName == "Kardashian").ID},
                new Department {Name="Home Economics",Budget=35000,StartDate=DateTime.Parse("2007-09-01"), InstructorID=instructors.Single(i => i.LastName == "Reynolds").ID},
                new Department {Name="Internet Trolling",Budget=0,StartDate=DateTime.Parse("2010-09-01"), InstructorID=instructors.Single(i => i.LastName == "Bean").ID},
                new Department {Name="Joomarlus",Budget=23765471,StartDate=DateTime.Parse("1780-09-01"), InstructorID=instructors.Single(i => i.LastName == "Oss").ID}
            };

            foreach (Department d in departments)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            var courses = new Course[]
            {
                new Course {CourseID=1001,Title="Programming", Credits=3,DepartmentID=departments.Single(s => s.Name == "Infotechnology").DepartmentID},
                new Course {CourseID=2221,Title="Databases 101", Credits=4,DepartmentID=departments.Single(s => s.Name == "Infotechnology").DepartmentID},
                new Course {CourseID=3001,Title="Html stuff", Credits=5,DepartmentID=departments.Single(s => s.Name == "Infotechnology").DepartmentID},
                new Course {CourseID=6543,Title="Cupcakes", Credits=5,DepartmentID=departments.Single(s => s.Name == "Home Economics").DepartmentID},
                new Course {CourseID=4290,Title="Chocolate tempering", Credits=1,DepartmentID=departments.Single(s => s.Name == "Home Economics").DepartmentID}
            };

            foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            context.SaveChanges();

            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment {InstructorID = instructors.Single(i => i.LastName == "Kardashian").ID, Location = "Classroom D428" },
                new OfficeAssignment {InstructorID = instructors.Single(i => i.LastName == "Bean").ID, Location = "Classroom A004" },
                new OfficeAssignment {InstructorID = instructors.Single(i => i.LastName == "Reynolds").ID, Location = "Classroom T001" },
                new OfficeAssignment {InstructorID = instructors.Single(i => i.LastName == "Oss").ID, Location = "Balta Burger King" }
            };

            foreach (OfficeAssignment o in officeAssignments)
            {
                context.OfficeAssignments.Add(o);
            }
            context.SaveChanges();

            var courseInstructors = new CourseAssignment[]
            {
                new CourseAssignment {CourseID = courses.Single(c => c.Title == "Programming").CourseID, InstructorID = instructors.Single(i => i.LastName == "Kardashian").ID },
                new CourseAssignment {CourseID = courses.Single(c => c.Title == "Databases 101").CourseID, InstructorID = instructors.Single(i => i.LastName == "Bean").ID },
                new CourseAssignment {CourseID = courses.Single(c => c.Title == "Html stuff").CourseID, InstructorID = instructors.Single(i => i.LastName == "Reynolds").ID },
                new CourseAssignment {CourseID = courses.Single(c => c.Title == "Cupcakes").CourseID, InstructorID = instructors.Single(i => i.LastName == "Karis").ID },
                new CourseAssignment {CourseID = courses.Single(c => c.Title == "Chocolate tempering").CourseID, InstructorID = instructors.Single(i => i.LastName == "Kardashian").ID }
            };

            foreach (CourseAssignment ci in courseInstructors)
            {
                context.CourseAssignments.Add(ci);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[] // [] masiiv
            {
                new Enrollment {StudentId = students.Single(s => s.LastName == "Nirgi").ID, CourseId = courses.Single(c => c.Title == "Programming").CourseID, Grade = Grade.A},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Nirgi").ID, CourseId = courses.Single(c => c.Title == "Html stuff").CourseID, Grade = Grade.D},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Uusküla").ID, CourseId = courses.Single(c => c.Title == "Programming").CourseID, Grade = Grade.F},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Virtsu").ID, CourseId = courses.Single(c => c.Title == "Cupcakes").CourseID, Grade = Grade.A},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Plärtsu").ID, CourseId = courses.Single(c => c.Title == "Programming").CourseID, Grade = Grade.C},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Plärtsu").ID, CourseId = courses.Single(c => c.Title == "Databases 101").CourseID, Grade = Grade.B},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Plärtsu").ID, CourseId = courses.Single(c => c.Title == "Html stuff").CourseID, Grade = Grade.B},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Male").ID, CourseId = courses.Single(c => c.Title == "Programming").CourseID, Grade = Grade.F},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Male").ID, CourseId = courses.Single(c => c.Title == "Html stuff").CourseID, Grade = Grade.F},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Chad").ID, CourseId = courses.Single(c => c.Title == "Programming").CourseID, Grade = Grade.A},
                new Enrollment {StudentId = students.Single(s => s.LastName == "Chad").ID, CourseId = courses.Single(c => c.Title == "Chocolate tempering").CourseID, Grade = Grade.A},
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                    s.Student.ID == e.Student.ID &&
                    s.Course.CourseID == e.CourseId)
                    .SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            };
            context.SaveChanges();
        }
    }
}
