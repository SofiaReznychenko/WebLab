using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
// Main file a project of GYM
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Спортивний зал API",
        Version = "v1",
        Description = "REST API для управління членами спортивного залу та тренуваннями"
    });
});

// Register JSON file data service
builder.Services.AddSingleton<IDataService, JsonFileDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Спортивний зал API V1");
    c.RoutePrefix = "swagger"; // Makes Swagger UI available at /swagger
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Models
public class Member
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class Workout
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Range(1, 300)]
    public int DurationMinutes { get; set; }
    [Required]
    public string Instructor { get; set; } = string.Empty;
    [Range(1, 50)]
    public int MaxParticipants { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string WorkoutType { get; set; } = string.Empty;
}

// Data container classes for JSON serialization
public class DataContainer<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int NextId { get; set; } = 1;
}

// Data Service Interface
public interface IDataService
{
    // Member operations
    IEnumerable<Member> GetAllMembers();
    Member? GetMemberById(int id);
    Member AddMember(Member member);
    Member? UpdateMember(int id, Member member);
    bool DeleteMember(int id);
    bool DeleteAllMembers();

    // Workout operations
    IEnumerable<Workout> GetAllWorkouts();
    Workout? GetWorkoutById(int id);
    Workout AddWorkout(Workout workout);
    Workout? UpdateWorkout(int id, Workout workout);
    bool DeleteWorkout(int id);
    bool DeleteAllWorkouts();
}

// JSON File Data Service Implementation
public class JsonFileDataService : IDataService
{
    private readonly string _membersFilePath;
    private readonly string _workoutsFilePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _membersLock = new object();
    private readonly object _workoutsLock = new object();

    public JsonFileDataService()
    {
        // Create data directory if it doesn't exist
        var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        Directory.CreateDirectory(dataDirectory);

        _membersFilePath = Path.Combine(dataDirectory, "members.json");
        _workoutsFilePath = Path.Combine(dataDirectory, "workouts.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Initialize files with sample data if they don't exist
        InitializeDataFiles();
    }

    private void InitializeDataFiles()
    {
        if (!File.Exists(_membersFilePath))
        {
            var membersContainer = new DataContainer<Member>
            {
                NextId = 3,
                Items = new List<Member>
                {
                    new Member
                    {
                        Id = 1,
                        Name = "Олександр Іваненко",
                        Email = "oleksandr@example.com",
                        Phone = "+380501234567",
                        JoinDate = DateTime.Now.AddDays(-30),
                        MembershipType = "Преміум",
                        IsActive = true
                    },
                    new Member
                    {
                        Id = 2,
                        Name = "Марія Петренко",
                        Email = "maria@example.com",
                        Phone = "+380507654321",
                        JoinDate = DateTime.Now.AddDays(-15),
                        MembershipType = "Стандарт",
                        IsActive = true
                    }
                }
            };
            SaveMembersContainer(membersContainer);
        }

        if (!File.Exists(_workoutsFilePath))
        {
            var workoutsContainer = new DataContainer<Workout>
            {
                NextId = 3,
                Items = new List<Workout>
                {
                    new Workout
                    {
                        Id = 1,
                        Name = "Кардіо тренування",
                        Description = "Інтенсивне кардіо тренування для спалювання жиру",
                        DurationMinutes = 45,
                        Instructor = "Андрій Коваль",
                        MaxParticipants = 15,
                        ScheduledDateTime = DateTime.Today.AddHours(18),
                        WorkoutType = "Кардіо"
                    },
                    new Workout
                    {
                        Id = 2,
                        Name = "Силові вправи",
                        Description = "Тренування з вагами для розвитку м'язової маси",
                        DurationMinutes = 60,
                        Instructor = "Ольга Сидоренко",
                        MaxParticipants = 10,
                        ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(16),
                        WorkoutType = "Силові"
                    }
                }
            };
            SaveWorkoutsContainer(workoutsContainer);
        }
    }

    private DataContainer<Member> LoadMembersContainer()
    {
        try
        {
            if (!File.Exists(_membersFilePath))
                return new DataContainer<Member>();

            var json = File.ReadAllText(_membersFilePath);
            return JsonSerializer.Deserialize<DataContainer<Member>>(json, _jsonOptions)
                   ?? new DataContainer<Member>();
        }
        catch
        {
            return new DataContainer<Member>();
        }
    }

    private DataContainer<Workout> LoadWorkoutsContainer()
    {
        try
        {
            if (!File.Exists(_workoutsFilePath))
                return new DataContainer<Workout>();

            var json = File.ReadAllText(_workoutsFilePath);
            return JsonSerializer.Deserialize<DataContainer<Workout>>(json, _jsonOptions)
                   ?? new DataContainer<Workout>();
        }
        catch
        {
            return new DataContainer<Workout>();
        }
    }

    private void SaveMembersContainer(DataContainer<Member> container)
    {
        var json = JsonSerializer.Serialize(container, _jsonOptions);
        File.WriteAllText(_membersFilePath, json);
    }

    private void SaveWorkoutsContainer(DataContainer<Workout> container)
    {
        var json = JsonSerializer.Serialize(container, _jsonOptions);
        File.WriteAllText(_workoutsFilePath, json);
    }

    // Member operations
    public IEnumerable<Member> GetAllMembers()
    {
        lock (_membersLock)
        {
            return LoadMembersContainer().Items;
        }
    }

    public Member? GetMemberById(int id)
    {
        lock (_membersLock)
        {
            var container = LoadMembersContainer();
            return container.Items.FirstOrDefault(m => m.Id == id);
        }
    }

    public Member AddMember(Member member)
    {
        lock (_membersLock)
        {
            var container = LoadMembersContainer();
            member.Id = container.NextId++;
            container.Items.Add(member);
            SaveMembersContainer(container);
            return member;
        }
    }

    public Member? UpdateMember(int id, Member member)
    {
        lock (_membersLock)
        {
            var container = LoadMembersContainer();
            var existingMember = container.Items.FirstOrDefault(m => m.Id == id);
            if (existingMember == null) return null;

            existingMember.Name = member.Name;
            existingMember.Email = member.Email;
            existingMember.Phone = member.Phone;
            existingMember.MembershipType = member.MembershipType;
            existingMember.IsActive = member.IsActive;

            SaveMembersContainer(container);
            return existingMember;
        }
    }

    public bool DeleteMember(int id)
    {
        lock (_membersLock)
        {
            var container = LoadMembersContainer();
            var member = container.Items.FirstOrDefault(m => m.Id == id);
            if (member == null) return false;

            container.Items.Remove(member);
            SaveMembersContainer(container);
            return true;
        }
    }

    public bool DeleteAllMembers()
    {
        lock (_membersLock)
        {
            var container = new DataContainer<Member> { NextId = 1 };
            SaveMembersContainer(container);
            return true;
        }
    }

    // Workout operations
    public IEnumerable<Workout> GetAllWorkouts()
    {
        lock (_workoutsLock)
        {
            return LoadWorkoutsContainer().Items;
        }
    }

    public Workout? GetWorkoutById(int id)
    {
        lock (_workoutsLock)
        {
            var container = LoadWorkoutsContainer();
            return container.Items.FirstOrDefault(w => w.Id == id);
        }
    }

    public Workout AddWorkout(Workout workout)
    {
        lock (_workoutsLock)
        {
            var container = LoadWorkoutsContainer();
            workout.Id = container.NextId++;
            container.Items.Add(workout);
            SaveWorkoutsContainer(container);
            return workout;
        }
    }

    public Workout? UpdateWorkout(int id, Workout workout)
    {
        lock (_workoutsLock)
        {
            var container = LoadWorkoutsContainer();
            var existingWorkout = container.Items.FirstOrDefault(w => w.Id == id);
            if (existingWorkout == null) return null;

            existingWorkout.Name = workout.Name;
            existingWorkout.Description = workout.Description;
            existingWorkout.DurationMinutes = workout.DurationMinutes;
            existingWorkout.Instructor = workout.Instructor;
            existingWorkout.MaxParticipants = workout.MaxParticipants;
            existingWorkout.ScheduledDateTime = workout.ScheduledDateTime;
            existingWorkout.WorkoutType = workout.WorkoutType;

            SaveWorkoutsContainer(container);
            return existingWorkout;
        }
    }

    public bool DeleteWorkout(int id)
    {
        lock (_workoutsLock)
        {
            var container = LoadWorkoutsContainer();
            var workout = container.Items.FirstOrDefault(w => w.Id == id);
            if (workout == null) return false;

            container.Items.Remove(workout);
            SaveWorkoutsContainer(container);
            return true;
        }
    }

    public bool DeleteAllWorkouts()
    {
        lock (_workoutsLock)
        {
            var container = new DataContainer<Workout> { NextId = 1 };
            SaveWorkoutsContainer(container);
            return true;
        }
    }
}

// Controllers
[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IDataService _dataService;

    public MembersController(IDataService dataService)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// Отримати список всіх членів залу
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<Member>> GetAllMembers()
    {
        return Ok(_dataService.GetAllMembers());
    }

    /// <summary>
    /// Отримати члена за ідентифікатором
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<Member> GetMemberById(int id)
    {
        var member = _dataService.GetMemberById(id);
        if (member == null)
            return NotFound($"Член з ID {id} не знайдений");

        return Ok(member);
    }

    /// <summary>
    /// Додати нового члена залу
    /// </summary>
    [HttpPost]
    public ActionResult<Member> CreateMember([FromBody] Member member)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        member.JoinDate = DateTime.Now;
        var createdMember = _dataService.AddMember(member);
        return CreatedAtAction(nameof(GetMemberById), new { id = createdMember.Id }, createdMember);
    }

    /// <summary>
    /// Оновити існуючого члена залу
    /// </summary>
    [HttpPut("{id}")]
    public ActionResult<Member> UpdateMember(int id, [FromBody] Member member)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedMember = _dataService.UpdateMember(id, member);
        if (updatedMember == null)
            return NotFound($"Член з ID {id} не знайдений");

        return Ok(updatedMember);
    }

    /// <summary>
    /// Видалити члена за ідентифікатором
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult DeleteMember(int id)
    {
        var deleted = _dataService.DeleteMember(id);
        if (!deleted)
            return NotFound($"Член з ID {id} не знайдений");

        return NoContent();
    }

    /// <summary>
    /// Видалити всіх членів залу
    /// </summary>
    [HttpDelete]
    public ActionResult DeleteAllMembers()
    {
        _dataService.DeleteAllMembers();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly IDataService _dataService;

    public WorkoutsController(IDataService dataService)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// Отримати список всіх тренувань
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<Workout>> GetAllWorkouts()
    {
        return Ok(_dataService.GetAllWorkouts());
    }

    /// <summary>
    /// Отримати тренування за ідентифікатором
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<Workout> GetWorkoutById(int id)
    {
        var workout = _dataService.GetWorkoutById(id);
        if (workout == null)
            return NotFound($"Тренування з ID {id} не знайдено");

        return Ok(workout);
    }

    /// <summary>
    /// Додати нове тренування
    /// </summary>
    [HttpPost]
    public ActionResult<Workout> CreateWorkout([FromBody] Workout workout)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdWorkout = _dataService.AddWorkout(workout);
        return CreatedAtAction(nameof(GetWorkoutById), new { id = createdWorkout.Id }, createdWorkout);
    }

    /// <summary>
    /// Оновити існуюче тренування
    /// </summary>
    [HttpPut("{id}")]
    public ActionResult<Workout> UpdateWorkout(int id, [FromBody] Workout workout)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedWorkout = _dataService.UpdateWorkout(id, workout);
        if (updatedWorkout == null)
            return NotFound($"Тренування з ID {id} не знайдено");

        return Ok(updatedWorkout);
    }

    /// <summary>
    /// Видалити тренування за ідентифікатором 
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult DeleteWorkout(int id)
    {
        var deleted = _dataService.DeleteWorkout(id);
        if (!deleted)
            return NotFound($"Тренування з ID {id} не знайдено");

        return NoContent();
    }

    /// <summary>
    /// Видалити всі тренування
    /// </summary>
    [HttpDelete]
    public ActionResult DeleteAllWorkouts()
    {
        _dataService.DeleteAllWorkouts();
        return NoContent();
    }
}