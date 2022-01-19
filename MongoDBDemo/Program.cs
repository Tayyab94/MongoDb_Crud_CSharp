// See https://aka.ms/new-console-template for more information
using MongoDataAccess.DataAccess;
using MongoDataAccess.Models;
using MongoDB.Driver;
using MongoDBDemo;

//Console.WriteLine("Hello, World!");

//string connectionString = "mongodb://127.0.0.1:27017";
//string databaseName = "universitydb";
//string collectionName = "people";


//var client = new MongoClient(connectionString);
//var db=client.GetDatabase(databaseName);
//var collection = db.GetCollection<PersonModel>(collectionName);

//var person = new PersonModel() { firstName = "Talha", lastName = "Habib" };

//await collection.InsertOneAsync(person);

//var results = await collection.FindAsync(_ => true);

//foreach (var result in results.ToList())
//{
//    Console.WriteLine($"First NAme {result.firstName}  last Name = {result.lastName}");
//}


ChoreDataAccess db = new ChoreDataAccess();

//await db.CreateUser(new UserModel { firstName = "Team", lastName = "Corry" });

var users = await db.GetAllUsers();
var use = await db.GetUserByName("Ahmad Ali");

//var dddd = await db.GetAllChoresForUser(use);
//var chore = new ChoreModel()
//{
//    FrequencyInDayss = 30,
//    ChoreText = "This is the demo of chore",
//    AssignTo = use
//};

//await db.CreateChore(chore);

//var chores= await db.GetAllChores();
//var newchore = chores.FirstOrDefault();
//newchore.LastCompleted = DateTime.UtcNow;

//await db.ChoreComplete(newchore);

Console.WriteLine("Program Executed");

var res = await db.GetAllChoresHistoryForUser(use);

foreach (var item in res)
{
    Console.WriteLine($"chore ID {item.ChoreId}  => Chore Text = {item.ChoreText} Chore Completed {item.DateCompleted}");
    Console.WriteLine($"Whome Completed {item.WhoCompleted.firstName}- {item.WhoCompleted.lastName}");
}
Console.ReadKey();