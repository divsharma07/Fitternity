using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;

namespace API.Data
{
    public class HabitsRepository : IHabitsRepository
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        

        public HabitsRepository(DataContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<Habit> AddHabitAsync(String habitName)
        {
            var habits = _context.Habits;
            var newHabit = new Habit { Name = habitName };
            habits.Add(newHabit);

            await _context.SaveChangesAsync();

            return newHabit;
        }

        public async Task<AppUser> GetUserWithHabits(int userId)
        {
            return await _context.Users
                .Include(x => x.UserHabits)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<IEnumerable<AppUserHabitDto>> GetAllHabits()
        {
            return _context.Habits?.Select(h => new AppUserHabitDto { Name = h.Name, Id = h.Id });
        }

        public async void AddUserPair(HabitPair habitPair)
        {
            await _context.HabitsPair.AddAsync(habitPair);
        }

        public async Task<List<HabitPair>> GetUserHabitPairs(int sourceUserId, int otherUserId)
        {
            var habitPair = await _context.HabitsPair.Where(h => (h.SourceUserId == sourceUserId 
            && h.OtherUserId == otherUserId) || (h.SourceUserId == otherUserId 
            && h.OtherUserId == sourceUserId)).ToListAsync();

            return habitPair;
        }

        public bool GetHabitPairExits(int sourceUserId, int otherUserId, string habitName)
        {
            var habitPairExistsSource = _context.HabitsPair.Any(h => h.HabitName == habitName && (h.SourceUserId == sourceUserId && h.OtherUserId == otherUserId));
            var habitPairExistsOther = _context.HabitsPair.Any(h => h.HabitName == habitName && (h.SourceUserId == otherUserId && h.OtherUserId == sourceUserId));

            return habitPairExistsSource || habitPairExistsOther;
        }
    }
}