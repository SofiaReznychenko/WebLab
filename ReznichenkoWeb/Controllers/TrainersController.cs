using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/[controller]")]
public class TrainersController : ControllerBase
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IValidator<CreateTrainerDto> _createTrainerValidator;
    private readonly IValidator<UpdateTrainerDto> _updateTrainerValidator;
    private readonly IMemoryCache _cache;
    private const string TRAINERS_CACHE_KEY = "trainers_all";

    public TrainersController(
        ITrainerRepository trainerRepository,
        IValidator<CreateTrainerDto> createTrainerValidator,
        IValidator<UpdateTrainerDto> updateTrainerValidator,
        IMemoryCache cache)
    {
        _trainerRepository = trainerRepository;
        _createTrainerValidator = createTrainerValidator;
        _updateTrainerValidator = updateTrainerValidator;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrainerDto>>> GetAllTrainers()
    {
        if (!_cache.TryGetValue(TRAINERS_CACHE_KEY, out IEnumerable<TrainerDto> trainerDtos))
        {
            var trainers = await _trainerRepository.GetAllAsync();
            trainerDtos = trainers.Select(t => new TrainerDto
            {
                Id = t.Id,
                Name = t.Name,
                Age = t.Age,
                Gender = t.Gender,
                Experience = t.Experience,
                Specialization = t.Specialization,
                Phone = t.Phone,
                Email = t.Email
            }).ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(TRAINERS_CACHE_KEY, trainerDtos, cacheOptions);
        }

        return Ok(trainerDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrainerDto>> GetTrainerById(int id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer == null)
            return NotFound($"Тренер з ID {id} не знайдений");

        var trainerDto = new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Age = trainer.Age,
            Gender = trainer.Gender,
            Experience = trainer.Experience,
            Specialization = trainer.Specialization,
            Phone = trainer.Phone,
            Email = trainer.Email
        };
        return Ok(trainerDto);
    }

    [HttpPost]
    public async Task<ActionResult<TrainerDto>> CreateTrainer([FromBody] CreateTrainerDto createTrainerDto)
    {
        var validationResult = await _createTrainerValidator.ValidateAsync(createTrainerDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var trainer = new Trainer
        {
            Name = createTrainerDto.Name,
            Age = createTrainerDto.Age,
            Gender = createTrainerDto.Gender,
            Experience = createTrainerDto.Experience,
            Specialization = createTrainerDto.Specialization,
            Phone = createTrainerDto.Phone,
            Email = createTrainerDto.Email
        };

        await _trainerRepository.AddAsync(trainer);
        _cache.Remove(TRAINERS_CACHE_KEY);

        var trainerDto = new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Age = trainer.Age,
            Gender = trainer.Gender,
            Experience = trainer.Experience,
            Specialization = trainer.Specialization,
            Phone = trainer.Phone,
            Email = trainer.Email
        };

        return CreatedAtAction(nameof(GetTrainerById), new { id = trainerDto.Id }, trainerDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TrainerDto>> UpdateTrainer(int id, [FromBody] UpdateTrainerDto updateTrainerDto)
    {
        var validationResult = await _updateTrainerValidator.ValidateAsync(updateTrainerDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer == null)
            return NotFound($"Тренер з ID {id} не знайдений");

        trainer.Name = updateTrainerDto.Name;
        trainer.Age = updateTrainerDto.Age;
        trainer.Gender = updateTrainerDto.Gender;
        trainer.Experience = updateTrainerDto.Experience;
        trainer.Specialization = updateTrainerDto.Specialization;
        trainer.Phone = updateTrainerDto.Phone;
        trainer.Email = updateTrainerDto.Email;

        await _trainerRepository.UpdateAsync(trainer);
        _cache.Remove(TRAINERS_CACHE_KEY);

        var trainerDto = new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Age = trainer.Age,
            Gender = trainer.Gender,
            Experience = trainer.Experience,
            Specialization = trainer.Specialization,
            Phone = trainer.Phone,
            Email = trainer.Email
        };

        return Ok(trainerDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTrainer(int id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer == null)
            return NotFound($"Тренер з ID {id} не знайдений");

        await _trainerRepository.DeleteAsync(trainer);
        _cache.Remove(TRAINERS_CACHE_KEY);
        return NoContent();
    }
}
