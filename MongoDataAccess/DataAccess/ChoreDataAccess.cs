using MongoDataAccess.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDataAccess.DataAccess;

public class ChoreDataAccess
{

    private const string ConnectionString = "mongodb://127.0.0.1:27017";

    private const string DatabaseName = "choredb";

    private const string ChoreCollection = "chore_collection";

    private const string UserCollection = "users";

    private const string ChoreHistoryCollection = "chore_histroy";


    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(ConnectionString);

        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public    async Task<List<UserModel>>GetAllUsers()
    {
        var userCollection = ConnectToMongo<UserModel>(UserCollection);

        var result = await userCollection.FindAsync(_ => true);

        return await result.ToListAsync();
    }

   public async Task<List<ChoreModel>> GetAllChores()
    {
        var choreCollection= ConnectToMongo<ChoreModel>(ChoreCollection);

        var result = await choreCollection.FindAsync(_ => true);

        return result.ToList();
    }

    public async Task<List<ChoreModel>>GetAllChoresForUser(UserModel user)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);

        var result = await choreCollection.FindAsync(s => s.AssignTo.Id == user.Id);

        return result.ToList();
    }

    public async Task<List<ChoreHistoryModel>> GetAllChoresHistoryForUser(UserModel user)
    {
        var choreCollection = ConnectToMongo<ChoreHistoryModel>(ChoreHistoryCollection);

        //var result = await choreCollection.FindAsync(s =>s.WhoCompleted ==user);


        var result = await choreCollection.FindAsync(s => s.WhoCompleted.Id == user.Id);

        return result.ToList();
    }

    public Task CreateUser(UserModel userModel)
    {
        var userCollection = ConnectToMongo<UserModel>(UserCollection);
        return userCollection.InsertOneAsync(userModel);
    }

    public async Task<UserModel> GetUserByName(string name)
    {
        var userCollection = ConnectToMongo<UserModel>(UserCollection);

        var result = await userCollection.FindAsync(s => s.firstName.ToLower() == name.ToLower());

        //var filter= Builders<UserModel>.Filter.Eq("")
        return result.FirstOrDefault();
    }
    public Task CreateChore(ChoreModel choreModel)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);

       return choreCollection.InsertOneAsync(choreModel);
    }

    public Task UpdateChore(ChoreModel choreModel)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);

        var filter = Builders<ChoreModel>.Filter.Eq("Id", choreModel.Id);

        return choreCollection.ReplaceOneAsync(filter,choreModel,new ReplaceOptions { IsUpsert = true});
    }

    public Task DeleteChore(ChoreModel choreModel)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);

        return choreCollection.DeleteOneAsync(s => s.Id == choreModel.Id);
    }


    public async Task ChoreComplete(ChoreModel choreModel)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var filter= Builders<ChoreModel>.Filter.Eq("Id",choreModel.Id);

        await choreCollection.ReplaceOneAsync(filter,choreModel);

        var choreHistoryCollection = ConnectToMongo<ChoreHistoryModel>(ChoreHistoryCollection);
        await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(choreModel));
    }

    public async Task ChoreComplete_WithTransaction(ChoreModel choreModel)
    {

        var client = new MongoClient(ConnectionString);

        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
            var filter = Builders<ChoreModel>.Filter.Eq("Id", choreModel.Id);

            await choreCollection.ReplaceOneAsync(filter, choreModel);

            var choreHistoryCollection = ConnectToMongo<ChoreHistoryModel>(ChoreHistoryCollection);
            await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(choreModel));

           await  session.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            throw;
        }
       
    }

}
