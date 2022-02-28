using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    public class HabitsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;

        private readonly IHabitsRepository _habitsRepository;

        private IMapper _mapper;
        private readonly IPixelaService _pixelaService;

        public HabitsController(DataContext context, IUserRepository userRepository,
         IHabitsRepository habitsRepository, IMapper mapper, IPixelaService pixelaService)
        {
            _pixelaService = pixelaService;
            _habitsRepository = habitsRepository;
            _context = context;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> AddUserHabit(AppUserHabitDto habitDto)
        {
            var sourceUserId = User.GetUserId();
            var currUser = await _habitsRepository.GetUserWithHabits(sourceUserId);

            var existingHabits = await _habitsRepository.GetAllHabits();
            var existingUserHabits = currUser.UserHabits;
            if (existingUserHabits.Any(p => p.Name == habitDto.Name))
            {
                return BadRequest("Habit already added");
            }
            if (!existingHabits.Any(p => p.Name == habitDto.Name))
            {
                await _habitsRepository.AddHabitAsync(habitDto.Name.ToLower());
            }
            existingUserHabits.Add(new AppUserHabit() { Name = habitDto.Name.ToLower() });
            if (await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Unexpected error while adding habit");
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<IEnumerable<AppUserHabit>>> GetUserHabits(string userName)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            var userWithHabits = await _habitsRepository.GetUserWithHabits(user.Id);

            return Ok(user.UserHabits);
        }

        [HttpGet("pair/{username}")]
        public async Task<ActionResult> GetHabitPairs(string username) {
            var sourceUserId = User.GetUserId();
            var otherUser = await _userRepository.GetUserByUsernameAsync(username);
            var otherUserId = otherUser.Id;

            var habitPairs = await _habitsRepository.GetUserHabitPairs(sourceUserId, otherUserId);

            return Ok(habitPairs);
        }

        [HttpGet("graph/{otherUserName}")]
        public async Task<ActionResult<string>> GetUserHabitGraph([FromQuery] string habit, string otherUserName)
        {
            var habitName = habit;
            var sourceUserId = User.GetUserId();
            var sourceUser = await _userRepository.GetUserByIdAsync(sourceUserId);
            var otherUser = await _userRepository.GetUserByUsernameAsync(otherUserName);
            var otherUserId = otherUser.Id;
            var userWithHabits = await _habitsRepository.GetUserWithHabits(sourceUserId);

            if(!_habitsRepository.GetHabitPairExits(sourceUserId, otherUserId, habitName)) {
                return BadRequest("No such habit pair exists"); 
            }
            
            return Ok(JsonConvert.SerializeObject(_pixelaService.GetUserGraph(habitName, sourceUser.UserName)));        
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddHabitPair([FromBody] AppUserHabitDto habitDto, string username)
        {
            habitDto.Name = habitDto.Name.ToLower();
            var sourceUserId = User.GetUserId();
            var currUserWithHabits = await _habitsRepository.GetUserWithHabits(sourceUserId);
            var otherUser = await _userRepository.GetUserByUsernameAsync(username);

            if(otherUser.Id == currUserWithHabits.Id) return BadRequest("Cannot form Habit Pair with yourself");
            var otherUserWithHabits = await _habitsRepository.GetUserWithHabits(otherUser.Id);
            var existingCurrUserHabits = currUserWithHabits.UserHabits;
            var existingOtherUserHabits = otherUserWithHabits.UserHabits;

            if (!existingCurrUserHabits.Any(p => p.Name == habitDto.Name) &&
             !existingOtherUserHabits.Any(p => p.Name == habitDto.Name))
            {
                return BadRequest("Both users have not added this habit");
            }

            if (!existingCurrUserHabits.Any(p => p.Name == habitDto.Name))
            {
                existingCurrUserHabits.Add(new AppUserHabit() { Name = habitDto.Name });
            }

            if (!existingOtherUserHabits.Any(p => p.Name == habitDto.Name))
            {
                existingOtherUserHabits.Add(new AppUserHabit() { Name = habitDto.Name });
            }

            var pairExists = _habitsRepository.GetHabitPairExits(currUserWithHabits.Id, otherUserWithHabits.Id, habitDto.Name);
            if(pairExists) {
                return BadRequest("Habit Pair Already Exists");
            }

            await addHabitGraphInformation(currUserWithHabits, otherUserWithHabits, habitDto.Name);
            var habitPairId = GetHabitPairId(sourceUserId, otherUser.Id, habitDto.Name);
            var habitPair = new HabitPair
            {
                HabitName = habitDto.Name,
                SourceUser = await _userRepository.GetUserByUsernameAsync(currUserWithHabits.UserName),
                SourceUserId = currUserWithHabits.Id,
                SourceUserName = currUserWithHabits.UserName,
                OtherUserId = otherUserWithHabits.Id,
                OtherUserName = otherUserWithHabits.UserName,
                OtherUser =  await _userRepository.GetUserByUsernameAsync(otherUserWithHabits.UserName),
                SourceUserGraph = _pixelaService.GetUserGraph(habitPairId, currUserWithHabits.UserName),
                OtherUserGraph = _pixelaService.GetUserGraph(habitPairId, otherUserWithHabits.UserName)
            };

            _habitsRepository.AddUserPair(habitPair);

            try
            {
                if (await _userRepository.SaveAllAsync())
                {
                    return Ok(habitPair);
                }
            } catch(Exception e) {
                return BadRequest("Habit Pair Already Added");
            }

            return BadRequest("Unexpected error while adding habit pair");
        }

        [HttpPost("action/{otherUserName}")]
        public async Task<ActionResult> AddHabitActivity([FromBody] AppUserHabitDto habitDto, string otherUserName) { 
            var sourceUserId = User.GetUserId();
            var habitName = habitDto.Name;
            var sourceUser = await _userRepository.GetUserByIdAsync(sourceUserId);
            var otherUser = await _userRepository.GetUserByUsernameAsync(otherUserName);
            var otherUserId = otherUser.Id;

            if(!_habitsRepository.GetHabitPairExits(sourceUserId, otherUserId, habitName)) {
                return BadRequest("No such habit pair exists"); 
            }

            await _pixelaService.AddActivity(GetHabitPairId(sourceUserId, otherUserId, habitName), sourceUser.UserName);

            return Ok();
        }

        private async Task addHabitGraphInformation(AppUser sourceUser, AppUser otherUser, string habitName)
        {   var habitPairId = GetHabitPairId(sourceUser.Id, otherUser.Id, habitName);
            await _pixelaService.AddHabitPair(habitPairId);
            await _pixelaService.CreateUserGraph(habitPairId, sourceUser.UserName);
            await _pixelaService.CreateUserGraph(habitPairId, otherUser.UserName);
        }

        private static string GetHabitPairId(int sourceUserId, int otherUserId, string habitName)
        {
            int minId = sourceUserId > otherUserId ? otherUserId : sourceUserId;
            int maxId = minId == sourceUserId ? otherUserId : sourceUserId;

            return String.Format("{0}{1}{2}", habitName, minId, maxId);
        }
    }
}