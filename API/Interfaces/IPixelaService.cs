using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPixelaService
    {
        string GetUserGraph(string habitPairName, string userName);
        Task AddActivity(string habitPairName, string userName);
        Task AddHabitPair(string habitPairName);
        Task CreateUserGraph(string habitPairName, string userName);
    }
}