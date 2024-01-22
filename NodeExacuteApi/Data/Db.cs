using System;
using System.ComponentModel;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NodeBaseApi.Version2;

namespace NodeBaseApi.Version2
{
    public class DBConnection
    {
        private readonly string _connectionString;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public DBConnection(string connectionString)
        {
            _connectionString = connectionString;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new TupleConverter(), new BlockJsonConverter() },
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
        }
        //Exacute stuff
        public async Task<CustomProgram> LoadProgramAsyncApi(string ApiKey, bool isCustomBlock)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                dynamic result;
                var query = "";
                if(!isCustomBlock)
                {
                    query = @"
                    SELECT ProgramData, IsCustomBlock
                    FROM Ludde.programs
                    WHERE ApiKey = @ApiKey ;
                    ";
                }
                else
                {
                    query = @"
                    SELECT ProgramData, IsCustomBlock
                    FROM Ludde.programs
                    WHERE Id = @ApiKey ;
                    ";
                }


                result = await connection.QueryFirstOrDefaultAsync(query, new { ApiKey = ApiKey });

                string programDataJson = result.ProgramData;

                var program = JsonConvert.DeserializeObject<CustomProgram>(programDataJson, _jsonSerializerSettings);

                foreach(var CustomBlock in program.ProgramStructure.CustomPrograms)
                {
                    var query2 = @"
                    SELECT ProgramData
                    FROM Ludde.programs
                    WHERE Id = @Id ;
                ";

                    var result2 = await connection.QueryFirstOrDefaultAsync(query2, new { Id = CustomBlock.Value.Id });

                    string programDataJson2 = result2.ProgramData;

                    program.ProgramStructure.CustomPrograms[CustomBlock.Key] = JsonConvert.DeserializeObject<CustomBlockProgram>(programDataJson2, _jsonSerializerSettings);
                }

                return program;
            }
        }
        public async Task InsertCallAsync(Call call)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Ludde.Calls (ProgramId, [Api/UserId], IsTest, StartTime, EndTime, Cost, Input, Output)
                    VALUES (@ProgramId, @ApiUserId, @IsTest, @StartTime, @EndTime, @Cost, @InputJson, @OutputJson);
                ";

                var parameters = new DynamicParameters();
                parameters.Add("ProgramId", call.ProgramId);
                parameters.Add("ApiUserId", call.ApiUserId);  // Notice I've changed the parameter name here
                parameters.Add("IsTest", call.IsTest);
                parameters.Add("StartTime", call.StartTime);
                parameters.Add("EndTime", call.EndTime);
                parameters.Add("Cost", call.Cost);
                parameters.Add("InputJson", call.InputJson);
                parameters.Add("OutputJson", call.OutputJson);

                await connection.ExecuteAsync(query, parameters);
            }
        }
        public async Task<bool> ApiKeyExistsAsync(string apiKey)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                SELECT COUNT(1)
                FROM Ludde.programs
                WHERE ApiKey = @ApiKey;
            ";

                var count = await connection.ExecuteScalarAsync<int>(query, new { ApiKey = apiKey });
                return count > 0;
            }
        }
        public async Task<Guid?> GetUserIdByApiKeyAsync(string apiKey)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            SELECT UserId 
            FROM Ludde.ApiKeys
            WHERE ApiKey = @ApiKey;
        ";

                try
                {
                    var userId = await connection.QueryFirstOrDefaultAsync<Guid?>(query, new { ApiKey = apiKey });
                    return userId;
                }
                catch (Exception ex)
                {
                    // Optionally log the exception
                    return null;
                }
            }
        }

        //Session stuff
        public async Task<Guid> CreateSessionAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Ludde.sessions (SessionId, UserId, ProgramId, Variables, SessionName, CreatedTime, LastEditedTime)
                    VALUES (@SessionId, @UserId, @ProgramId, @Variables, @SessionName, GETUTCDATE(), GETUTCDATE());
                ";

                try
                {
                    await connection.ExecuteAsync(query, session);
                    return session.SessionId;
                }
                catch (Exception ex)
                {
                    return Guid.Empty;
                }
            }
        }

        public async Task<bool> UpdateSessionAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            UPDATE Ludde.sessions
            SET Variables = @Variables, 
                LastEditedTime = GETUTCDATE()
            WHERE SessionId = @SessionId;
        ";

                try
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(query, new
                    {
                        SessionId = session.SessionId.ToString(),
                        Variables = session.Variables.ToString()
                    });
                    if (result == 0)
                    {
                        Console.WriteLine("Update operation did not affect any rows.");
                        return false;
                    }
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.ToString()}");
                    return false;
                }
            }
        }

        public async Task<Session> GetSessionAsync(string sessionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            SELECT SessionId, UserId, ProgramId, Variables, SessionName, CreatedTime, LastEditedTime
            FROM Ludde.sessions
            WHERE SessionId = @SessionId;
        ";

                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { SessionId = sessionId });
                    if (result != null)
                    {
                        var session = new Session
                        {
                            SessionId = Guid.Parse(result.SessionId),
                            UserId = result.UserId,
                            ProgramId = result.ProgramId,
                            Variables = result.Variables,
                            SessionName = result.SessionName,
                            CreatedTime = result.CreatedTime,
                            LastEditedTime = result.LastEditedTime
                        };
                        return session;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    // Log the exception here
                    return null;
                }
            }
        }

        //Token stuff
        public async Task<Guid> GetUserId(Guid ApiKey)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT UserId 
                    FROM Ludde.UserApiKeys
                    WHERE ApiKey = @ApiKey;
                ";
                return await connection.QuerySingleOrDefaultAsync<Guid>(query, new { ApiKey });
            }
        }
        public async Task<int> GetUserTokensAsync(Guid identifier, bool isApiKey = true)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (isApiKey)
                {
                    var userIdQuery = @"
                SELECT UserId 
                FROM Ludde.UserApiKeys
                WHERE ApiKey = @ApiKey;
            ";
                    var userId = await connection.QuerySingleOrDefaultAsync<Guid>(userIdQuery, new { ApiKey = identifier });

                    if (userId == default(Guid))
                        throw new InvalidOperationException("Invalid ApiKey provided.");

                    identifier = userId;
                }

                var tokensQuery = @"
                    SELECT (Tokens + BoughtTokens) AS TotalTokens
                    FROM Ludde.TokenWallet
                    WHERE UserId = @UserId;
                ";
                return await connection.QuerySingleOrDefaultAsync<int>(tokensQuery, new { UserId = identifier });
            }
        }
        public async Task UpdateUserTokensAsync(Guid identifier, int tokensToDeduct, bool isApiKey = true)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (isApiKey)
                {
                    var userIdQuery = @"
                        SELECT UserId 
                        FROM Ludde.UserApiKeys
                        WHERE ApiKey = @ApiKey;
                    ";
                    var userId = await connection.QuerySingleOrDefaultAsync<Guid>(userIdQuery, new { ApiKey = identifier });

                    if (userId == default(Guid))
                        throw new InvalidOperationException("Invalid ApiKey provided.");

                    identifier = userId;
                }

                var updateTokensQuery = @"
                    DECLARE @Tokens INT;
                    DECLARE @BoughtTokens INT;

                    SELECT @Tokens = Tokens, @BoughtTokens = BoughtTokens
                    FROM Ludde.TokenWallet
                    WHERE UserId = @UserId;

                    IF (@Tokens >= @TokensToDeduct)
                    BEGIN
                        UPDATE Ludde.TokenWallet
                        SET Tokens = @Tokens - @TokensToDeduct
                        WHERE UserId = @UserId;
                    END
                    ELSE
                    BEGIN
                        SET @TokensToDeduct = @TokensToDeduct - @Tokens;
                        UPDATE Ludde.TokenWallet
                        SET Tokens = 0, 
                            BoughtTokens = CASE 
                                             WHEN @BoughtTokens >= @TokensToDeduct THEN @BoughtTokens - @TokensToDeduct
                                             ELSE 0 
                                           END
                        WHERE UserId = @UserId;
                    END
                ";
                await connection.ExecuteAsync(updateTokensQuery, new { UserId = identifier, TokensToDeduct = tokensToDeduct });
            }
        }
    }
}
 