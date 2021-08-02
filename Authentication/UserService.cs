using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BastetFTMAPI.Authentication
{
    public class UserService : IUserService
    {
        private const string DB_NAME = "BastetFTM";
        private const string Collection_Name = "User";
        private readonly IMongoCollection<User> userCollection;
        //private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;
        //private readonly string key;
        //private IConfiguration Configuration { get; }

        public UserService(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);
            userCollection = database.GetCollection<User>(Collection_Name);
            //key = Configuration.GetSection("JwtKey").ToString();
        }
        public async Task<List<User>> GetUsersAsync()
        {
            var users = (await userCollection.Find(new BsonDocument())
                             .ToListAsync()).AsQueryable();

            return users.ToList();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await userCollection.InsertOneAsync(user);
        }

        public async Task<User> AuthenticateAsync(string username, string userNameHash, string password)
        {
            return await userCollection.Find(x => x.Username == username && x.UserNameHash == userNameHash && x.Password == password).SingleOrDefaultAsync();
        }
    }
}
