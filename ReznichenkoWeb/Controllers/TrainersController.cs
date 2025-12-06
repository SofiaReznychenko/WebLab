using Microsoft.AspNetCore.Mvc;
using ReznichenkoWeb.ViewModels;
using ReznichenkoWeb.Models;
using ReznichenkoWeb.Repositories;

namespace ReznichenkoWeb.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ITrainerRepository _trainerRepository;

        public TrainersController(ITrainerRepository trainerRepository)
        {
            _trainerRepository = trainerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var trainers = await _trainerRepository.GetAllAsync();
            var viewModels = trainers.Select(t => new TrainerViewModel
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

            return View(viewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var trainer = new Trainer
                {
                    Name = model.Name,
                    Age = model.Age,
                    Gender = model.Gender,
                    Experience = model.Experience,
                    Specialization = model.Specialization,
                    Phone = model.Phone,
                    Email = model.Email
                };

                await _trainerRepository.AddAsync(trainer);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id);
            if (trainer == null) return NotFound();

            var model = new TrainerViewModel
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var trainer = await _trainerRepository.GetByIdAsync(id);
                if (trainer == null) return NotFound();

                trainer.Name = model.Name;
                trainer.Age = model.Age;
                trainer.Gender = model.Gender;
                trainer.Experience = model.Experience;
                trainer.Specialization = model.Specialization;
                trainer.Phone = model.Phone;
                trainer.Email = model.Email;

                await _trainerRepository.UpdateAsync(trainer);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id);
            if (trainer != null)
            {
                await _trainerRepository.DeleteAsync(trainer);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
