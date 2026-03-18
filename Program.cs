using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

public class Student
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public Dictionary<string, double?> Grades { get; set; } = new Dictionary<string, double?>();
    public List<string> Records { get; set; } = new List<string>();
    public DateTime LastUpdated { get; set; }
    public DateTime IdentityLastUpdated { get; set; }
    public DateTime GradesLastUpdated { get; set; }
    public DateTime RecordsLastUpdated { get; set; }
}

public class Program
{
    private static string? StartupWarning;

    private static readonly string DataDirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "StudentDataConsole");

    private static readonly string DataFilePath = Path.Combine(DataDirectoryPath, "students.json");

    private static readonly string LegacyDataFilePath = Path.Combine(Environment.CurrentDirectory, "students.json");

    private static readonly string[] Subjects =
    {
        "English",
        "Math",
        "Science",
        "Social Studies",
        "Foreign Language"
    };

    public static void Main(string[] args)
    {
        List<Student> students = LoadStudents();

        if (!string.IsNullOrWhiteSpace(StartupWarning))
        {
            Console.WriteLine(StartupWarning);
            PauseForUser();
            StartupWarning = null;
        }

        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear();
            Console.WriteLine("Main Menu");
            Console.WriteLine("1. Add student");
            Console.WriteLine("2. View students");
            Console.WriteLine("3. Student data");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");

            string? input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                PauseForUser();
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddStudentMenu(students, ref isRunning);
                    break;

                case 2:
                    ViewStudentsMenu(students, ref isRunning);
                    break;

                case 3:
                    StudentDataMenu(students, ref isRunning);
                    break;

                case 4:
                    Console.WriteLine("Exiting application.");
                    isRunning = false;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    PauseForUser();
                    break;
            }
        }
    }

    private static void AddStudentMenu(List<Student> students, ref bool isRunning)
    {
        bool inAddMenu = true;

        while (inAddMenu && isRunning)
        {
            Console.Clear();
            Console.WriteLine("Add Student Menu");
            Console.WriteLine("1. Add a student");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("9. Exit");
            Console.Write("Enter your choice: ");

            string? input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                PauseForUser();
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.Write("Enter student name: ");
                    string? name = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Student name cannot be empty.");
                        PauseForUser();
                        break;
                    }

                    Console.Write("Enter student ID: ");
                    string? id = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        Console.WriteLine("Student ID cannot be empty.");
                        PauseForUser();
                        break;
                    }

                    if (students.Any(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine("A student with that ID already exists.");
                        PauseForUser();
                        break;
                    }

                    Student student = new Student
                    {
                        Name = name,
                        Id = id
                    };

                    InitializeGrades(student);
                    SetAllLastUpdated(student, DateTime.Now);
                    student.Records.Add($"Student created on {DateTime.Now:g}.");
                    students.Add(student);
                    SaveStudents(students);
                    Console.WriteLine($"Added student: {name} (ID: {id})");
                    PauseForUser();
                    break;

                case 0:
                    inAddMenu = false;
                    break;

                case 9:
                    isRunning = false;
                    inAddMenu = false;
                    Console.WriteLine("Exiting application.");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    PauseForUser();
                    break;
            }
        }
    }

    private static void ViewStudentsMenu(List<Student> students, ref bool isRunning)
    {
        bool inViewMenu = true;

        while (inViewMenu && isRunning)
        {
            Console.Clear();
            Console.WriteLine("View Students Menu");
            Console.WriteLine("1. Show all students");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("9. Exit");
            Console.Write("Enter your choice: ");

            string? input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                PauseForUser();
                continue;
            }

            switch (choice)
            {
                case 1:
                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students found.");
                    }
                    else
                    {
                        for (int i = 0; i < students.Count; i++)
                        {
                            Student student = students[i];
                            Console.WriteLine($"{i + 1}. {student.Name} (ID: {student.Id})");

                            foreach (string subject in Subjects)
                            {
                                string gradeText = FormatGradeWithLetter(student.Grades[subject]);
                                Console.WriteLine($"   {subject}: {gradeText}");
                            }

                            double gpa = CalculateGpa(student);
                            Console.WriteLine($"   GPA: {gpa:F2} ({GetGpaLetter(gpa)})");
                        }
                    }
                    PauseForUser();
                    break;

                case 0:
                    inViewMenu = false;
                    break;

                case 9:
                    isRunning = false;
                    inViewMenu = false;
                    Console.WriteLine("Exiting application.");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    PauseForUser();
                    break;
            }
        }
    }

    private static void StudentDataMenu(List<Student> students, ref bool isRunning)
    {
        bool inStudentDataMenu = true;

        while (inStudentDataMenu && isRunning)
        {
            Console.Clear();
            Console.WriteLine("Student Data Menu");
            ShowStudentDirectory(students);
            Console.WriteLine();
            Console.WriteLine("1. View student details");
            Console.WriteLine("2. Edit student grades");
            Console.WriteLine("3. Add student record");
            Console.WriteLine("4. Edit student records");
            Console.WriteLine("5. Edit student name and ID");
            Console.WriteLine("6. Delete student");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("9. Exit");
            Console.Write("Enter your choice: ");

            string? input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                PauseForUser();
                continue;
            }

            switch (choice)
            {
                case 1:
                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students available.");
                        PauseForUser();
                        break;
                    }

                    Student? student = FindStudent(students);
                    if (student == null)
                    {
                        Console.WriteLine("Student not found.");
                        PauseForUser();
                        break;
                    }

                    ShowSingleStudentData(student);
                    PauseForUser();
                    break;

                case 2:
                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students available.");
                        PauseForUser();
                        break;
                    }

                    EditStudentData(students);
                    break;

                case 3:
                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students available.");
                        PauseForUser();
                        break;
                    }

                    Console.Clear();
                    Console.WriteLine("Add Student Record");
                    Student? studentForRecordAdd = FindStudent(students);
                    if (studentForRecordAdd == null)
                    {
                        Console.WriteLine("Student not found.");
                        PauseForUser();
                        break;
                    }

                    AddCategorizedStudentRecord(studentForRecordAdd, students);
                    break;

                case 4:
                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students available.");
                        PauseForUser();
                        break;
                    }

                    EditStudentRecords(students);
                    break;

                case 5:
                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students available.");
                        PauseForUser();
                        break;
                    }

                    EditStudentIdentity(students);
                    break;

                case 6:
                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students available.");
                        PauseForUser();
                        break;
                    }

                    DeleteStudent(students);
                    break;

                case 0:
                    inStudentDataMenu = false;
                    break;

                case 9:
                    isRunning = false;
                    inStudentDataMenu = false;
                    Console.WriteLine("Exiting application.");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    PauseForUser();
                    break;
            }
        }
    }

    private static void EditStudentIdentity(List<Student> students)
    {
        Console.Clear();
        Console.WriteLine("Edit Student Name and ID");

        Student? student = FindStudent(students);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
            PauseForUser();
            return;
        }

        string oldName = student.Name;
        string oldId = student.Id;

        Console.Write("Enter new student name: ");
        string? newName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(newName))
        {
            Console.WriteLine("Student name cannot be empty.");
            PauseForUser();
            return;
        }

        Console.Write("Enter new student ID: ");
        string? newId = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(newId))
        {
            Console.WriteLine("Student ID cannot be empty.");
            PauseForUser();
            return;
        }

        bool idInUse = students.Any(s =>
            !ReferenceEquals(s, student) &&
            s.Id.Equals(newId, StringComparison.OrdinalIgnoreCase));

        if (idInUse)
        {
            Console.WriteLine("Another student already uses that ID.");
            PauseForUser();
            return;
        }

        student.Name = newName;
        student.Id = newId;
        student.Records.Add($"Identity updated on {DateTime.Now:g}: {oldName} ({oldId}) -> {newName} ({newId}).");
        UpdateIdentityLastUpdated(student, DateTime.Now);
        UpdateRecordsLastUpdated(student, DateTime.Now);
        SaveStudents(students);

        Console.WriteLine("Student name and ID updated.");
        PauseForUser();
    }

    private static void DeleteStudent(List<Student> students)
    {
        Console.Clear();
        Console.WriteLine("Delete Student");

        Student? student = FindStudent(students);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
            PauseForUser();
            return;
        }

        Console.Write($"Are you sure you want to delete {student.Name} (ID: {student.Id})? (Y/N): ");
        string? confirm = Console.ReadLine()?.Trim();

        if (!string.Equals(confirm, "Y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Delete canceled.");
            PauseForUser();
            return;
        }

        students.Remove(student);
        SaveStudents(students);
        Console.WriteLine("Student deleted.");
        PauseForUser();
    }

    private static void EditStudentData(List<Student> students)
    {
        Console.Clear();
        Console.WriteLine("Edit Student Data");

        Student? student = FindStudent(students);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
            PauseForUser();
            return;
        }

        Console.WriteLine("1. Edit grades");
        Console.WriteLine("2. Add student record");
        Console.WriteLine("0. Cancel");
        Console.Write("Enter your choice: ");

        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice))
        {
            Console.WriteLine("Please enter a valid number.");
            PauseForUser();
            return;
        }

        switch (choice)
        {
            case 1:
                foreach (string subject in Subjects)
                {
                    double grade = PromptForGrade(subject);
                    student.Grades[subject] = grade;
                }

                student.Records.Add($"Grades updated on {DateTime.Now:g}.");
                UpdateGradesLastUpdated(student, DateTime.Now);
                UpdateRecordsLastUpdated(student, DateTime.Now);
                SaveStudents(students);
                Console.WriteLine("Student grades updated.");
                PauseForUser();
                return;

            case 2:
                AddCategorizedStudentRecord(student, students);
                return;

            case 0:
                return;

            default:
                Console.WriteLine("Invalid option.");
                PauseForUser();
                return;
        }
    }

    private static Student? FindStudent(List<Student> students) 
    {
        Console.Write("Enter student name: ");
        string? name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        Console.Write("Enter student ID: ");
        string? id = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return students.Find(s =>
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
            s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    private static void AddCategorizedStudentRecord(Student student, List<Student> students)
    {
        Console.WriteLine();
        string? entry = PromptCategorizedRecordEntry(student);
        if (entry == null)
        {
            PauseForUser();
            return;
        }

        student.Records.Add(entry);
        UpdateRecordsLastUpdated(student, DateTime.Now);
        SaveStudents(students);
        Console.WriteLine("Student record added.");
        PauseForUser();
    }

    private static void EditStudentRecords(List<Student> students)
    {
        Console.Clear();
        Console.WriteLine("Edit Student Records");

        Student? student = FindStudent(students);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
            PauseForUser();
            return;
        }

        if (student.Records.Count == 0)
        {
            Console.WriteLine("No records available for this student.");
            PauseForUser();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Existing Records:");
        for (int i = 0; i < student.Records.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {student.Records[i]}");
        }

        Console.Write("Enter record number to edit: ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int recordNumber) ||
            recordNumber < 1 ||
            recordNumber > student.Records.Count)
        {
            Console.WriteLine("Invalid record number.");
            PauseForUser();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Enter updated record details:");
        string? updatedEntry = PromptCategorizedRecordEntry(student, student.Records[recordNumber - 1]);
        if (updatedEntry == null)
        {
            PauseForUser();
            return;
        }

        student.Records[recordNumber - 1] = updatedEntry;
        UpdateRecordsLastUpdated(student, DateTime.Now);
        SaveStudents(students);
        Console.WriteLine("Student record updated.");
        PauseForUser();
    }

    private static string? PromptCategorizedRecordEntry(Student student, string? existingRecord = null)
    {
        Console.WriteLine("Enter transcript record details:");
        string studentName = PromptWithDefault("Student name", student.Name, required: true);
        string studentId = PromptWithDefault("Student ID", student.Id, required: true);

        int? defaultRecordType = GetRecordTypeFromEntry(existingRecord);

        Console.WriteLine("Select record type:");
        Console.WriteLine("1. Academic Records");
        Console.WriteLine("2. Personal Information");
        Console.WriteLine("3. Health and Medical");
        Console.WriteLine("4. Behavioral/Disciplinary");
        Console.WriteLine("5. Special Services");
        Console.Write(defaultRecordType.HasValue
            ? $"Enter your choice (Enter keeps {defaultRecordType.Value}): "
            : "Enter your choice: ");

        string? input = Console.ReadLine()?.Trim();
        int recordType;
        if (string.IsNullOrWhiteSpace(input) && defaultRecordType.HasValue)
        {
            recordType = defaultRecordType.Value;
        }
        else if (!int.TryParse(input, out recordType))
        {
            Console.WriteLine("Please enter a valid number.");
            return null;
        }

        switch (recordType)
        {
            case 1:
                string gradeLevel = PromptWithDefault(
                    "Grade level",
                    GetFieldDefault(student, "Academic Records", "Grade Level", "N/A", existingRecord),
                    required: true);

                string enrollmentStatus = PromptWithDefault(
                    "Enrollment status",
                    GetFieldDefault(student, "Academic Records", "Enrollment Status", "Enrolled", existingRecord),
                    required: true);

                string counselorAdvisor = PromptWithDefault(
                    "Counselor/Advisor",
                    GetFieldDefault(student, "Academic Records", "Counselor/Advisor", "N/A", existingRecord),
                    required: true);

                string transcripts = PromptWithDefault(
                    "Transcripts details",
                    GetFieldDefault(student, "Academic Records", "Transcripts", BuildCurrentGradeSummary(student), existingRecord),
                    required: true);

                string reportCards = PromptWithDefault(
                    "Report cards details",
                    GetFieldDefault(student, "Academic Records", "Report Cards", BuildCurrentReportCardSummary(student), existingRecord),
                    required: true);

                string classSchedules = PromptWithDefault(
                    "Class schedules details",
                    GetFieldDefault(student, "Academic Records", "Class Schedules", "N/A", existingRecord),
                    required: true);

                string testScores = PromptWithDefault(
                    "Test scores details",
                    GetFieldDefault(student, "Academic Records", "Test Scores", "N/A", existingRecord),
                    required: true);

                string attendance = PromptWithDefault(
                    "Attendance details (optional)",
                    GetFieldDefault(student, "Academic Records", "Attendance", "N/A", existingRecord),
                    required: false);

                return $"{DateTime.Now:g} | Academic Records | Student Name: {studentName}; Student ID: {studentId}; Grade Level: {gradeLevel}; Enrollment Status: {enrollmentStatus}; Counselor/Advisor: {counselorAdvisor}; Transcripts: {transcripts}; Report Cards: {reportCards}; Class Schedules: {classSchedules}; Test Scores: {testScores}; Attendance: {attendance}";

            case 2:
                string birthdate = PromptWithDefault(
                    "Birthdate",
                    GetFieldDefault(student, "Personal Information", "Birthdate", "N/A", existingRecord),
                    required: true);

                string address = PromptWithDefault(
                    "Address",
                    GetFieldDefault(student, "Personal Information", "Address", "N/A", existingRecord),
                    required: true);

                string studentIdentificationNumbers = PromptWithDefault(
                    "Student identification numbers",
                    GetFieldDefault(student, "Personal Information", "Student Identification Numbers", student.Id, existingRecord),
                    required: true);

                return $"{DateTime.Now:g} | Personal Information | Student Name: {studentName}; Student ID: {studentId}; Birthdate: {birthdate}; Address: {address}; Student Identification Numbers: {studentIdentificationNumbers}";

            case 3:
                string immunizationRecords = PromptWithDefault(
                    "Immunization records details",
                    GetFieldDefault(student, "Health and Medical", "Immunization Records", "N/A", existingRecord),
                    required: true);

                string accidentReports = PromptWithDefault(
                    "Accident reports details",
                    GetFieldDefault(student, "Health and Medical", "Accident Reports", "N/A", existingRecord),
                    required: true);

                return $"{DateTime.Now:g} | Health and Medical | Student Name: {studentName}; Student ID: {studentId}; Immunization Records: {immunizationRecords}; Accident Reports: {accidentReports}";

            case 4:
                string infractions = PromptWithDefault(
                    "Infractions details",
                    GetFieldDefault(student, "Behavioral/Disciplinary", "Infractions", "N/A", existingRecord),
                    required: true);

                string disciplinaryActions = PromptWithDefault(
                    "Disciplinary actions details",
                    GetFieldDefault(student, "Behavioral/Disciplinary", "Disciplinary Actions", "N/A", existingRecord),
                    required: true);

                return $"{DateTime.Now:g} | Behavioral/Disciplinary | Student Name: {studentName}; Student ID: {studentId}; Infractions: {infractions}; Disciplinary Actions: {disciplinaryActions}";

            case 5:
                string specialServices = PromptWithDefault(
                    "IEP or special education records details",
                    GetFieldDefault(student, "Special Services", "IEP/Special Education Records", "N/A", existingRecord),
                    required: true);

                return $"{DateTime.Now:g} | Special Services | Student Name: {studentName}; Student ID: {studentId}; IEP/Special Education Records: {specialServices}";

            default:
                Console.WriteLine("Invalid option.");
                return null;
        }
    }

    private static string PromptRequired(string prompt)
    {
        return PromptWithDefault(prompt, string.Empty, required: true);
    }

    private static string PromptOptional(string prompt)
    {
        return PromptWithDefault(prompt, "N/A", required: false);
    }

    private static string PromptWithDefault(string prompt, string defaultValue, bool required)
    {
        while (true)
        {
            string displayDefault = string.IsNullOrWhiteSpace(defaultValue) ? "<none>" : defaultValue;
            Console.WriteLine($"Enter {prompt} [default: {displayDefault}] (Enter confirms, Shift+Enter/Ctrl+Enter/F2 adds line):");
            string? input = ReadTextInput(allowMultiline: true)?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                if (!string.IsNullOrWhiteSpace(defaultValue))
                {
                    return defaultValue;
                }

                if (!required)
                {
                    return "N/A";
                }

                Console.WriteLine("This field cannot be empty.");
                continue;
            }

            return input;
        }
    }

    private static int? GetRecordTypeFromEntry(string? recordEntry)
    {
        if (string.IsNullOrWhiteSpace(recordEntry))
        {
            return null;
        }

        if (recordEntry.Contains("| Academic Records |", StringComparison.OrdinalIgnoreCase)) return 1;
        if (recordEntry.Contains("| Personal Information |", StringComparison.OrdinalIgnoreCase)) return 2;
        if (recordEntry.Contains("| Health and Medical |", StringComparison.OrdinalIgnoreCase)) return 3;
        if (recordEntry.Contains("| Behavioral/Disciplinary |", StringComparison.OrdinalIgnoreCase)) return 4;
        if (recordEntry.Contains("| Special Services |", StringComparison.OrdinalIgnoreCase)) return 5;

        return null;
    }

    private static string GetFieldDefault(
        Student student,
        string section,
        string field,
        string fallback,
        string? existingRecord)
    {
        string? existingValue = ExtractFieldValue(existingRecord, field);
        if (!string.IsNullOrWhiteSpace(existingValue))
        {
            return existingValue;
        }

        string? latestValue = GetLatestRecordFieldValue(student, section, field);
        if (!string.IsNullOrWhiteSpace(latestValue))
        {
            return latestValue;
        }

        return fallback;
    }

    private static string? GetLatestRecordFieldValue(Student student, string section, string field)
    {
        for (int i = student.Records.Count - 1; i >= 0; i--)
        {
            string record = student.Records[i];
            if (!record.Contains($"| {section} |", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string? value = ExtractFieldValue(record, field);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static string? ExtractFieldValue(string? recordText, string field)
    {
        if (string.IsNullOrWhiteSpace(recordText)) 
        {
            return null;
        }

        string marker = $"{field}: ";
        int start = recordText.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
        {
            return null;
        }

        start += marker.Length;
        int end = recordText.IndexOf(';', start);
        if (end < 0)
        {
            end = recordText.Length;
        }

        return recordText[start..end].Trim();
    }

    private static string BuildCurrentGradeSummary(Student student)
    {
        List<string> gradeParts = new List<string>();
        foreach (string subject in Subjects)
        {
            gradeParts.Add($"{subject}: {FormatGradeWithLetter(student.Grades[subject])}");
        }

        return string.Join(", ", gradeParts);
    }

    private static string BuildCurrentReportCardSummary(Student student)
    {
        double gpa = CalculateGpa(student);
        return $"Current GPA: {gpa:F2} ({GetGpaLetter(gpa)})";
    }

    private static string? ReadTextInput(bool allowMultiline)
    {
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine();
        }

        StringBuilder builder = new StringBuilder();

        while (true)
        {
            ConsoleKeyInfo key;
            try
            {
                key = Console.ReadKey(intercept: true);
            }
            catch (InvalidOperationException)
            {
                // Fallback for environments where key-by-key input is not supported.
                return Console.ReadLine();
            }

            if (key.Key == ConsoleKey.Enter)
            {
                bool wantsNewLine = allowMultiline &&
                    (((key.Modifiers & ConsoleModifiers.Shift) != 0) ||
                    ((key.Modifiers & ConsoleModifiers.Control) != 0));

                if (wantsNewLine)
                {
                    builder.AppendLine();
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine();
                return builder.ToString();
            }

            if (allowMultiline && key.Key == ConsoleKey.F2)
            {
                builder.AppendLine();
                Console.WriteLine();
                continue;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (builder.Length == 0)
                {
                    continue;
                }

                if (builder.Length >= 2 && builder[^2] == '\r' && builder[^1] == '\n')
                {
                    builder.Length -= 2;
                    continue;
                }

                if (builder[^1] == '\n')
                {
                    builder.Length -= 1;
                    continue;
                }

                builder.Length -= 1;
                Console.Write("\b \b");
                continue;
            }

            if (char.IsControl(key.KeyChar))
            {
                continue;
            }

            builder.Append(key.KeyChar);
            Console.Write(key.KeyChar);
        }
    }

    private static void ShowSingleStudentData(Student student)
    {
        Console.WriteLine($"Name: {student.Name}");
        Console.WriteLine($"Last updated {FormatLastUpdated(student.IdentityLastUpdated)}");
        Console.WriteLine($"ID: {student.Id}");
        Console.WriteLine($"Last updated {FormatLastUpdated(student.IdentityLastUpdated)}");

        Console.WriteLine($"Grades last updated {FormatLastUpdated(student.GradesLastUpdated)}");
        foreach (string subject in Subjects)
        {
            string gradeText = FormatGradeWithLetter(student.Grades[subject]);
            Console.WriteLine($"{subject}: {gradeText}");
        }

        double gpa = CalculateGpa(student);
        Console.WriteLine($"GPA: {gpa:F2} ({GetGpaLetter(gpa)})");
        Console.WriteLine($"Records last updated {FormatLastUpdated(student.RecordsLastUpdated)}");
        Console.WriteLine("Records:");
        if (student.Records.Count == 0)
        {
            Console.WriteLine("- No records available.");
            return;
        }

        foreach (string record in student.Records)
        {
            Console.WriteLine($"- {record}");
        }
    }

    private static void ShowStudentDirectory(List<Student> students)
    {
        Console.WriteLine("Current Students:");

        if (students.Count == 0)
        {
            Console.WriteLine("- No students added yet.");
            return;
        }

        for (int i = 0; i < students.Count; i++)
        {
            Student student = students[i];
            Console.WriteLine($"{i + 1}. {student.Name} (ID: {student.Id})"); 
            Console.WriteLine($"   Last updated {FormatLastUpdated(student.LastUpdated)}");
        }
    }

    private static void InitializeGrades(Student student)
    {
        if (student.Grades == null)
        {
            student.Grades = new Dictionary<string, double?>();
        }

        if (student.Records == null)
        {
            student.Records = new List<string>();
        }

        foreach (string subject in Subjects)
        {
            if (!student.Grades.ContainsKey(subject))
            {
                student.Grades[subject] = null;
            }
        }

        NormalizeLastUpdatedValues(student);
    }

    private static void NormalizeLastUpdatedValues(Student student)
    {
        if (student.LastUpdated == default)
        {
            if (student.IdentityLastUpdated != default)
            {
                student.LastUpdated = student.IdentityLastUpdated;
            }
            else if (student.GradesLastUpdated != default)
            {
                student.LastUpdated = student.GradesLastUpdated;
            }
            else if (student.RecordsLastUpdated != default)
            {
                student.LastUpdated = student.RecordsLastUpdated; 
            }
        }
    }

    private static void SetAllLastUpdated(Student student, DateTime timestamp)
    {
        student.LastUpdated = timestamp;
        student.IdentityLastUpdated = timestamp;
        student.GradesLastUpdated = timestamp;
        student.RecordsLastUpdated = timestamp;
    }

    private static void UpdateIdentityLastUpdated(Student student, DateTime timestamp)
    {
        student.IdentityLastUpdated = timestamp;
        student.LastUpdated = timestamp;
    }

    private static void UpdateGradesLastUpdated(Student student, DateTime timestamp)
    {
        student.GradesLastUpdated = timestamp;
        student.LastUpdated = timestamp;
    }

    private static void UpdateRecordsLastUpdated(Student student, DateTime timestamp)
    {
        student.RecordsLastUpdated = timestamp;
        student.LastUpdated = timestamp;
    }

    private static string FormatLastUpdated(DateTime timestamp)
    {
        if (timestamp == default)
        {
            return "N/A";
        }

        return $"({timestamp:MM/dd/yyyy}) ({timestamp:hh:mm tt})";
    }

    private static List<Student> LoadStudents()
    {
        try
        {
            string fileToLoad = DataFilePath;
            if (!File.Exists(fileToLoad) && File.Exists(LegacyDataFilePath))
            {
                fileToLoad = LegacyDataFilePath;
            }

            if (!File.Exists(fileToLoad))
            {
                return new List<Student>();
            }

            string json = File.ReadAllText(fileToLoad);
            List<Student>? students = JsonSerializer.Deserialize<List<Student>>(json);

            if (students == null)
            {
                StartupWarning = "Could not load student data. The saved file was empty or invalid.";
                return new List<Student>();
            }

            foreach (Student student in students)
            {
                InitializeGrades(student);
            }

            return students;
        }
        catch (Exception ex)
        {
            StartupWarning = $"Could not load saved student data: {ex.Message}";
            return new List<Student>();
        }
    }

    private static void SaveStudents(List<Student> students)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        Directory.CreateDirectory(DataDirectoryPath);
        string json = JsonSerializer.Serialize(students, options);
        File.WriteAllText(DataFilePath, json);
    }

    private static double CalculateGpa(Student student)
    {
        List<double> availableGrades = student.Grades.Values
            .Where(grade => grade.HasValue)
            .Select(grade => grade!.Value)
            .ToList();

        if (availableGrades.Count == 0)
        {
            return 0.0;
        }

        double average = availableGrades.Average();
        return Math.Clamp(average / 25.0, 0.0, 4.0);
    }

    private static string FormatGradeWithLetter(double? grade)
    {
        if (!grade.HasValue)
        {
            return "N/A";
        }

        return $"{grade.Value:F1} ({GetPercentLetter(grade.Value)})";
    }

    private static string GetPercentLetter(double grade)
    {
        if (grade >= 97)
        {
            return "A+";
        }

        if (grade >= 93)
        {
            return "A";
        }

        if (grade >= 90)
        {
            return "A-";
        }

        if (grade >= 87)
        {
            return "B+";
        }

        if (grade >= 83)
        {
            return "B";
        }

        if (grade >= 80)
        {
            return "B-";
        }

        if (grade >= 77)
        {
            return "C+";
        }

        if (grade >= 73)
        {
            return "C";
        }

        if (grade >= 70)
        {
            return "C-";
        }

        if (grade >= 67)
        {
            return "D+";
        }

        if (grade >= 63)
        {
            return "D";
        }

        if (grade >= 60)
        {
            return "D-";
        }

        return "F";
    }

    private static string GetGpaLetter(double gpa)
    {
        if (gpa >= 3.85)
        {
            return "A";
        }

        if (gpa >= 3.5)
        {
            return "A-";
        }

        if (gpa >= 3.15)
        {
            return "B+";
        }

        if (gpa >= 2.85)
        {
            return "B";
        }

        if (gpa >= 2.5)
        {
            return "B-";
        }

        if (gpa >= 2.15)
        {
            return "C+";
        }

        if (gpa >= 1.85)
        {
            return "C";
        }

        if (gpa >= 1.5)
        {
            return "C-";
        }

        if (gpa >= 1.15)
        {
            return "D+";
        }

        if (gpa >= 0.85)
        {
            return "D";
        }

        if (gpa >= 0.5)
        {
            return "D-";
        }

        return "F";
    }

    private static double PromptForGrade(string subject)
    {
        while (true)
        {
            Console.Write($"Enter {subject} grade (0-100): ");
            string? input = Console.ReadLine();

            if (double.TryParse(input, out double grade) && grade >= 0 && grade <= 100)
            {
                return grade;
            }

            Console.WriteLine("Invalid grade. Enter a number from 0 to 100.");
        }
    }

    private static void PauseForUser()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }
}
