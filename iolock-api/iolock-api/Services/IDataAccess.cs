using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace iolock_api.Models;

public interface IDataAccess
{
    Task<string> GetAccessPasswordAsync(string preferredUsername, int picCode);
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<RoomBuilding>> GetUserAvailablesRoomsAsync(string email);
    Task<User> GetUserByEmailAsync(string Email);
    Task<int> InsertUserAsync(User user);
    Task<int> UpdateUserAsync(Dictionary<string, object> diffs, string email);
    Task<IEnumerable> GetBuildings();
    Task<IEnumerable> GetBuildingRooms(string building);
    Task<int> RevokeUserPermission(string email, string room, string building);
    Task<int> GiveUserPermission(string email, string room, string building);
    Task<IEnumerable<Log>> GetLogs();
    Task<IEnumerable<Log>> GetUserLogs(string email);
    Task<int> InsertUserLogs(string email, string room, string building);
}
