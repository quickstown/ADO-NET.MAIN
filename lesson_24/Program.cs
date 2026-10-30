using College_DB.Models;
using Microsoft.EntityFrameworkCore;
using CheToNovoe.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CollegeDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CollegeDBContext>();
    db.Database.EnsureCreated();

    // EnsureCreated не добавляет новые таблицы в уже существующую БД,
    // поэтому создаём таблицы учителей, предметов и оценок, если их ещё нет.
    db.Database.ExecuteSqlRaw(@"
        IF OBJECT_ID('dbo.Teachers') IS NULL
        CREATE TABLE dbo.Teachers (
            TeacherId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Teachers PRIMARY KEY,
            FirstName NVARCHAR(50) NOT NULL,
            LastName  NVARCHAR(50) NOT NULL
        );

        IF OBJECT_ID('dbo.Subjects') IS NULL
        CREATE TABLE dbo.Subjects (
            SubjectsId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Subjects PRIMARY KEY,
            SubjectsName NVARCHAR(100) NOT NULL,
            TeacherId    INT NOT NULL CONSTRAINT FK_Subjects_Teachers_TeacherId
                FOREIGN KEY REFERENCES dbo.Teachers(TeacherId) ON DELETE CASCADE
        );

        IF OBJECT_ID('dbo.Grades') IS NULL
        CREATE TABLE dbo.Grades (
            GradesId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Grades PRIMARY KEY,
            StudentId  INT NOT NULL CONSTRAINT FK_Grades_Students_StudentId
                FOREIGN KEY REFERENCES dbo.Students(StudentId) ON DELETE CASCADE,
            SubjectsId INT NOT NULL CONSTRAINT FK_Grades_Subjects_SubjectsId
                FOREIGN KEY REFERENCES dbo.Subjects(SubjectsId) ON DELETE CASCADE,
            Score      INT NOT NULL,
            GradeDate  DATE NOT NULL
        );");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapGet("students/all", async (CollegeDBContext db) => {
    return Results.Ok(await db.Students.AsNoTracking().ToListAsync());
});

app.MapGet("groups/all", async (CollegeDBContext db) => {
    return Results.Ok(await db.Groups.AsNoTracking().ToListAsync());
});

app.MapGet("group/{id}", async (int id, CollegeDBContext db) => {
    var student = await db.Students.AsNoTracking().SingleOrDefaultAsync(s => s.StudentId == id);
    return Results.Ok();
});

app.MapPost("/students/add", async (Student student, CollegeDBContext db) =>
{
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/students/add",
                           new { studentId = student.StudentId });
});

app.MapPost("/groups/add", async (Group group, CollegeDBContext db) =>
{
    db.Groups.Add(group);
    await db.SaveChangesAsync();
    return Results.Created($"/groups/add",
                           new { groupId = group.GroupId });
});

app.MapPut("/students/{id:int}", async (int id, Student student, CollegeDBContext db) =>
{
    var existing = await db.Students.FindAsync(id);

    if (existing != null)
    {
        existing.FirstName = student.FirstName;
        existing.LastName = student.LastName;
        existing.Age = student.Age;
        existing.GroupId = student.GroupId;
    }

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/students/{id:int}", async (int id, CollegeDBContext db) =>
{
    var existing = await db.Students.FindAsync(id);
    if (existing != null)
    {
        db.Remove(existing);
        await db.SaveChangesAsync();
    }
    return Results.NoContent();
});

app.MapGet("/teachers/all", async (CollegeDBContext db) =>
    Results.Ok(await db.Teachers.AsNoTracking()
        .Select(t => new
        {
            t.TeacherId,
            t.FirstName,
            t.LastName,
            Subjects = t.Subjects
                .Select(s => new { s.SubjectsId, s.SubjectsName })
                .ToList()
        })
        .ToListAsync()));

app.MapPost("/teachers/add", async (Teacher teacher, CollegeDBContext db) =>
{
    db.Teachers.Add(teacher);
    await db.SaveChangesAsync();
    return Results.Created("/teachers/add", new { teacherId = teacher.TeacherId });
});

app.MapPut("/teachers/{id:int}", async (int id, Teacher teacher, CollegeDBContext db) =>
{
    var existing = await db.Teachers.FindAsync(id);
    if (existing == null) return Results.NotFound();

    existing.FirstName = teacher.FirstName;
    existing.LastName = teacher.LastName;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/teachers/{id:int}", async (int id, CollegeDBContext db) =>
{
    var existing = await db.Teachers.FindAsync(id);
    if (existing != null)
    {
        db.Remove(existing);
        await db.SaveChangesAsync();
    }
    return Results.NoContent();
});
app.MapGet("/subjects/all", async (CollegeDBContext db) =>
    Results.Ok(await db.Subjects.AsNoTracking()
        .Select(s => new
        {
            s.SubjectsId,
            s.SubjectsName,
            s.TeacherId,
            TeacherName = s.Teacher!.FirstName + " " + s.Teacher.LastName
        })
        .ToListAsync()));

app.MapPost("/subjects/add", async (Subjects subject, CollegeDBContext db) =>
{
    db.Subjects.Add(subject);
    await db.SaveChangesAsync();
    return Results.Created("/subjects/add", new { subjectsId = subject.SubjectsId });
});

app.MapPut("/subjects/{id:int}", async (int id, Subjects subject, CollegeDBContext db) =>
{
    var existing = await db.Subjects.FindAsync(id);
    if (existing == null) return Results.NotFound();

    existing.SubjectsName = subject.SubjectsName;
    existing.TeacherId = subject.TeacherId;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/subjects/{id:int}", async (int id, CollegeDBContext db) =>
{
    var existing = await db.Subjects.FindAsync(id);
    if (existing != null)
    {
        db.Remove(existing);
        await db.SaveChangesAsync();
    }
    return Results.NoContent();
});
app.MapGet("/grades/all", async (CollegeDBContext db) =>
    Results.Ok(await db.Grades.AsNoTracking()
        .Select(g => new
        {
            g.GradesId,
            g.Score,
            g.GradeDate,
            g.StudentId,
            g.SubjectsId,
            StudentName = g.Student!.FirstName + " " + g.Student.LastName,
            SubjectsName = g.Subjects!.SubjectsName
        }).ToListAsync()));

app.MapPost("/grades/add", async (Grades grade, CollegeDBContext db) =>
{
    if (grade.Score < 1 || grade.Score > 5)
        return Results.BadRequest("Оценка должна быть от 1 до 5");

    db.Grades.Add(grade);
    await db.SaveChangesAsync();
    return Results.Created("/grades/add", new { gradesId = grade.GradesId });
});

app.MapPut("/grades/{id:int}", async (int id, Grades grade, CollegeDBContext db) =>
{
    if (grade.Score < 1 || grade.Score > 5)
        return Results.BadRequest("Оценка должна быть от 1 до 5");

    var existing = await db.Grades.FindAsync(id);
    if (existing == null) return Results.NotFound();

    existing.Score = grade.Score;
    existing.GradeDate = grade.GradeDate;
    existing.StudentId = grade.StudentId;
    existing.SubjectsId = grade.SubjectsId;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/grades/{id:int}", async (int id, CollegeDBContext db) =>
{
    var existing = await db.Grades.FindAsync(id);
    if (existing != null)
    {
        db.Remove(existing);
        await db.SaveChangesAsync();
    }
    return Results.NoContent();
});




app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();