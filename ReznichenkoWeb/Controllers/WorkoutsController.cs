using Microsoft.AspNetCore.Mvc;
using ReznichenkoWeb.ViewModels;
using ReznichenkoWeb.Models;
using ReznichenkoWeb.Repositories;

namespace ReznichenkoWeb.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly IWorkoutRepository _workoutRepository;

        public WorkoutsController(IWorkoutRepository workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public async Task<IActionResult> Index()
        {
            var workouts = await _workoutRepository.GetAllAsync();
            var viewModels = workouts.Select(w => new WorkoutViewModel
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                DurationMinutes = w.DurationMinutes,
                Instructor = w.Instructor,
                MaxParticipants = w.MaxParticipants,
                ScheduledDateTime = w.ScheduledDateTime,
                WorkoutType = w.WorkoutType
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var workout = new Workout
                {
                    Name = model.Name,
                    Description = model.Description,
                    DurationMinutes = model.DurationMinutes,
                    Instructor = model.Instructor,
                    MaxParticipants = model.MaxParticipants,
                    ScheduledDateTime = model.ScheduledDateTime.ToUniversalTime(),
                    WorkoutType = model.WorkoutType
                };

                await _workoutRepository.AddAsync(workout);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var workout = await _workoutRepository.GetByIdAsync(id);
            if (workout == null) return NotFound();

            var model = new WorkoutViewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                DurationMinutes = workout.DurationMinutes,
                Instructor = workout.Instructor,
                MaxParticipants = workout.MaxParticipants,
                ScheduledDateTime = workout.ScheduledDateTime,
                WorkoutType = workout.WorkoutType
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var workout = await _workoutRepository.GetByIdAsync(id);
                if (workout == null) return NotFound();

                workout.Name = model.Name;
                workout.Description = model.Description;
                workout.DurationMinutes = model.DurationMinutes;
                workout.Instructor = model.Instructor;
                workout.MaxParticipants = model.MaxParticipants;
                workout.ScheduledDateTime = model.ScheduledDateTime.ToUniversalTime();
                workout.WorkoutType = model.WorkoutType;

                await _workoutRepository.UpdateAsync(workout);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var workout = await _workoutRepository.GetByIdAsync(id);
            if (workout != null)
            {
                await _workoutRepository.DeleteAsync(workout);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
