using frar.LobbyServer;
using System.Security.Cryptography;
using System.Text;

string name = "barney";

UserManager userManager = new UserManager();
userManager.Connect("./config.json");
userManager.AddUser(name, "password", "email");

Console.WriteLine(userManager.VerifyUser(name, "password"));

userManager.RemoveUser(name);

userManager.Disconnect();

