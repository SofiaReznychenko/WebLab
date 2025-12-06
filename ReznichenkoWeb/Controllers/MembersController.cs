using Microsoft.AspNetCore.Mvc;
using ReznichenkoWeb.ViewModels;
using ReznichenkoWeb.Models;
using ReznichenkoWeb.Repositories;

namespace ReznichenkoWeb.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberRepository _memberRepository;

        public MembersController(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _memberRepository.GetAllAsync();
            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                JoinDate = m.JoinDate,
                MembershipType = m.MembershipType,
                IsActive = m.IsActive,
                Age = m.Age,
                Gender = m.Gender
            }).ToList();

            return View(memberViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                var member = new Member
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    MembershipType = model.MembershipType,
                    JoinDate = DateTime.UtcNow,
                    IsActive = true,
                    Age = model.Age,
                    Gender = model.Gender
                };

                await _memberRepository.AddAsync(member);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) return NotFound();

            var model = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                JoinDate = member.JoinDate,
                MembershipType = member.MembershipType,
                IsActive = member.IsActive,
                Age = member.Age,
                Gender = member.Gender
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var member = await _memberRepository.GetByIdAsync(id);
                if (member == null) return NotFound();

                member.Name = model.Name;
                member.Email = model.Email;
                member.Phone = model.Phone;
                member.MembershipType = model.MembershipType;
                member.IsActive = model.IsActive;
                member.Age = model.Age;
                member.Gender = model.Gender;

                await _memberRepository.UpdateAsync(member);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member != null)
            {
                await _memberRepository.DeleteAsync(member);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
