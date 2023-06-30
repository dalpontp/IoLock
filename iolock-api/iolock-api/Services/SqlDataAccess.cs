using Dapper;
using iolock_api.Models;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace iolock_api.Models;

public class SqlDataAccess : IDataAccess
{
    private readonly string _connectionString;

    public SqlDataAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("iolockDb");
    }

    public async Task<string> GetAccessPasswordAsync(string preferredUsername, int picCode)
    {
        const string query = """
            SELECT d.psw from dbo."Permissions" as p
            JOIN dbo."Users" as u ON (p.userId = u.id)
            JOIN dbo."Rooms" as r ON (p.roomId = r.id)
            JOIN dbo."DoorCredentials" as d ON (r.picId = d.picId)
            WHERE u.preferredUsername = @preferredUsername
            AND d.picCode = @picCode
            ORDER BY d.id ASC
            ;
            """;
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<string>(query, new { preferredUsername, picCode });
    }

    public async Task<IEnumerable<UserEntity>> GetUsersAsync()
    {
        const string query = """
            SELECT givenName, familyName, email, preferredUsername, emailVerified FROM Users;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<UserEntity>(query);
    }
    public async Task<IEnumerable<RoomBuildingEntity>> getUserAvailablesRoomsAsync(string email)
    {
        const string query = """
            SELECT room, building FROM "Permissions" as p
            JOIN "Users" as u ON (p.userId = u.id)
            JOIN "Rooms" as r ON (r.id = p.roomId)
            WHERE u.email = @email;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<RoomBuildingEntity>(query, new { email });
    }

    public async Task<UserEntity> GetUserByEmailAsync(string Email)
    {
        const string query = """
            SELECT givenName, familyName, email, preferredUsername, emailVerified FROM Users WHERE email = @Email;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryFirstOrDefaultAsync<UserEntity>(query, new { Email });
    }

    public async Task<int> CreateUserAsync(UserEntity user)
    {
        const string query = """
            INSERT INTO Users (givenName, familyName, email, preferredUsername, emailVerified)
            OUTPUT Inserted.ID
            VALUES (@GivenName, @FamilyName, @Email, @PreferredUsername, @EmailVerified);
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.ExecuteAsync(query, user);
    }

    public async Task<int> UpdateUserAsync(UserEntity user)
    {
        const string query = """
            UPDATE Users SET 
            givenName = @GivenName,
            familyName = @FamilyName,
            email = @Email,
            preferredUsername = @PreferredUsername,
            emailVerified = @EmailVerified
            OUTPUT Inserted.ID
            WHERE id = @Id;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.ExecuteAsync(query, new { user });
    }

    public async Task<IEnumerable> GetBuildings()
    {
        const string query = """
            SELECT DISTINCT building FROM "Rooms";
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync(query);
    }

    public async Task<IEnumerable> GetBuildingRooms(string building)
    {
        const string query = """
            SELECT room FROM "Rooms"
            WHERE building = @building;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync(query, new { building });
    }

    public async Task<int> RevokeUserPermission(string email, string room, string building)
    {
        const string query = """
            DELETE FROM "Permissions" WHERE 
            id = (SELECT p.id FROM "Permissions" as p
            JOIN "Users" as u ON (p.userId = u.id)
            JOIN "Rooms" as r ON (r.id = p.roomId)
            WHERE u.email = @email
            AND room = @room
            AND building = @building);
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.ExecuteAsync(query, new { email, room, building });
    }
}
