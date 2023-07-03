using Dapper;
using iolock_api.Models;
using Microsoft.Data.SqlClient;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iolock_api.Models;

public class SqlDataAccess : IDataAccess
{
    private readonly string _connectionString;

    public SqlDataAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("iolockDb");
    }

    public async Task<string> GetAccessPasswordAsync(string email, int picCode)
    {
        const string query = """
            SELECT d.psw, u.email, r.room, r.building FROM dbo."Permissions" as p
            JOIN dbo."Users" as u ON (p.userId = u.id)
            JOIN dbo."Rooms" as r ON (p.roomId = r.id)
            JOIN dbo."DoorCredentials" as d ON (r.picId = d.picId)
            WHERE u.email = @email
            AND d.picCode = @picCode
            ORDER BY d.id ASC
            ;
            """;
        using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryFirstOrDefaultAsync(query, new { email, picCode });

        if(result != null)
        {
            await InsertUserLogs(result.email, result.room, result.building);
            return result.psw;
        }
        return null;        
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        const string query = """
            SELECT givenName, familyName, email, preferredUsername, emailVerified FROM Users;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<User>(query);
    }
    public async Task<IEnumerable<RoomBuilding>> GetUserAvailablesRoomsAsync(string email)
    {
        const string query = """
            SELECT room, building FROM "Permissions" as p
            JOIN "Users" as u ON (p.userId = u.id)
            JOIN "Rooms" as r ON (r.id = p.roomId)
            WHERE u.email = @email;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<RoomBuilding>(query, new { email });
    }

    public async Task<User> GetUserByEmailAsync(string Email)
    {
        const string query = """
            SELECT givenName, familyName, email, preferredUsername, emailVerified FROM Users WHERE email = @Email;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Email });
    }

    public async Task<int> CreateUserAsync(User user)
    {
        const string query = """
            INSERT INTO Users (givenName, familyName, email, preferredUsername, emailVerified)
            OUTPUT Inserted.ID
            VALUES (@GivenName, @FamilyName, @Email, @PreferredUsername, @EmailVerified);
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.ExecuteAsync(query, user);
    }

    public async Task<int> UpdateUserAsync(Dictionary<string, object> diffs, string email)
    {
        string joinedDiffs = "";
        foreach (var diff in diffs)
        {
            joinedDiffs += diff.Key + " = " + diff.Value;
        }

        const string query = """
            UPDATE Users SET
            @joinedDiffs
            OUTPUT Inserted.ID
            WHERE email = @Email;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.ExecuteAsync(query, new { joinedDiffs, email });
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

    public async Task<int> GiveUserPermission(string email, string room, string building)
    {
        const string query = """
            BEGIN
                IF NOT EXISTS(SELECT * FROM Permissions
                    WHERE userId = (SELECT id FROM "Users" WHERE email = @email)
                    AND roomId = (SELECT id FROM "Rooms" WHERE room = @room AND building = @building))
                BEGIN
                    INSERT INTO "Permissions" (userId, "roomId") VALUES (
                        (SELECT id FROM "Users" WHERE email = @email),
                        (SELECT id FROM "Rooms" WHERE room = @room AND building = @building)
                    )
                END
            END
            """;
        //const string query = """
        //    INSERT INTO "Permissions" (userId, "roomId") VALUES (
        //    (SELECT id FROM "Users" WHERE email = @email),
        //    (SELECT id FROM "Rooms" WHERE room = @room AND building = @building)
        //    );
        //    """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.ExecuteAsync(query, new { email, room, building });
    }

    public async Task<IEnumerable<Log>> GetLogs()
    {
        const string query = """
            SELECT u."givenName" as UserGivenName,
            u."familyName" as UserFamilyName,
            u.email as UserEmail,
            r.room, r.building,
            l."accessDatetime" FROM Logs as l
            JOIN "Users" as u ON (u.id = l."userId")
            JOIN "Rooms" as r ON (r.id = l."roomId")
            ORDER BY "accessDatetime" DESC;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<Log>(query);
    }

    public async Task<IEnumerable<Log>> GetUserLogs(string email)
    {
        const string query = """
            SELECT u."givenName" as UserGivenName,
            u."familyName" as UserFamilyName,
            u.email as UserEmail,
            r.room, r.building,
            l."accessDatetime" FROM Logs as l
            JOIN "Users" as u ON (u.id = l."userId")
            JOIN "Rooms" as r ON (r.id = l."roomId")
            WHERE u.email = @email
            ORDER BY "accessDatetime" DESC;
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<Log>(query, new { email });
    }

    public async Task<int> InsertUserLogs(string email, string room, string building)
    {
        const string query = """
            INSERT INTO Logs ("userId", "roomId") VALUES (
                (SELECT id FROM "Users" WHERE email = @email),
                (SELECT id FROM "Rooms" WHERE room = @room AND building = @building)
            );
            """;
        using var connection = new SqlConnection(_connectionString);

        return await connection.ExecuteAsync(query, new { email, room, building });
    }
}
