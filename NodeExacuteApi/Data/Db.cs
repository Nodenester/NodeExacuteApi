using System;
using System.ComponentModel;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
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
        public async Task<CustomProgram> LoadProgramAsyncApi(string ApiKey)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT ProgramData, IsCustomBlock
                    FROM Ludde.programs
                    WHERE ApiKey = @ApiKey ;
                ";

                var result = await connection.QueryFirstOrDefaultAsync(query, new { ApiKey = ApiKey });

                string programDataJson = result.ProgramData;

                var program = JsonConvert.DeserializeObject<CustomProgram>(programDataJson, _jsonSerializerSettings);
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

        //Session stuff
        public async Task<Guid> CreateSessionAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            INSERT INTO Ludde.sessions (SessionId, UserId, ProgramId, Variables, SessionName, CreatedTime, LastEditedTime)
            VALUES (@SessionId, @UserId, @ProgramId, @Variables, @SessionName, GETUTCDATE(), GETUTCDATE());
        ";

                await connection.ExecuteAsync(query, session);
            }

            return session.SessionId;
        }

        public async Task UpdateSessionAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            UPDATE Ludde.sessions
            SET Variables = @Variables, LastEditedTime = GETUTCDATE()
            WHERE SessionId = @SessionId;
        ";

                await connection.ExecuteAsync(query, session);
            }

        }

        public async Task<Session> GetSessionAsync(Guid sessionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT SessionId, UserId, ProgramId, Variables, SessionName, CreatedTime, LastEditedTime
                    FROM Ludde.sessions
                    WHERE SessionId = @SessionId;
                ";

                return await connection.QuerySingleOrDefaultAsync<Session>(query, new { SessionId = sessionId });
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
                    SELECT Tokens
                    FROM Ludde.TokenWallet
                    WHERE UserId = @UserId;
                ";
                return await connection.QuerySingleOrDefaultAsync<int>(tokensQuery, new { UserId = identifier });
            }
        }
        public async Task UpdateUserTokensAsync(Guid identifier, int tokens, bool isApiKey = true)
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
                    UPDATE Ludde.TokenWallet
                    SET Tokens = @Tokens
                    WHERE UserId = @UserId;
                ";
                await connection.ExecuteAsync(updateTokensQuery, new { UserId = identifier, Tokens = tokens });
            }
        }
    }
}
 