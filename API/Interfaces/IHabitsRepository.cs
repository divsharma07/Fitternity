using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IHabitsRepository
    {
        Task<AppUser> GetUserWithHabits(int userId);
        Task<Habit> AddHabitAsync(String habitName);

        Task<IEnumerable<AppUserHabitDto>> GetAllHabits();
        void AddUserPair(HabitPair pair);
        Task<List<HabitPair>> GetUserHabitPairs(int sourceUserId, int otherUserId);
        bool GetHabitPairExits(int id1, int id2, string name);
    }
}