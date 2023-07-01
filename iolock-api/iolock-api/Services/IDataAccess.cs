using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace iolock_api.Models;

public interface IDataAccess
{
    Task<string> GetAccessPasswordAsync(string preferredUsername, int picCode);
    Task<IEnumerable<UserEntity>> GetUsersAsync();
    Task<IEnumerable<RoomBuildingEntity>> getUserAvailablesRoomsAsync(string email);
    Task<UserEntity> GetUserByEmailAsync(string Email);
    Task<int> CreateUserAsync(UserEntity user);
    Task<int> UpdateUserAsync(UserEntity user);
    Task<IEnumerable> GetBuildings();
    Task<IEnumerable> GetBuildingRooms(string building);
    Task<int> RevokeUserPermission(string email, string room, string building);
    Task<int> GiveUserPermission(string email, string room, string building);
    Task<IEnumerable<Log>> GetLogs();
    Task<IEnumerable<Log>> GetUserLogs(string email);
    Task<int> InsertUserLogs(string email, string room, string building);
}
