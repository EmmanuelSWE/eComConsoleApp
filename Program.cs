using cmdDistrict.DataAccess;
using cmdDistrict.Models;

using dotenv.net;

DotEnv.Load();

AppDbContext.CheckConnection();
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("==========================================");
Console.WriteLine("        Welcome to Cmd District          ");
Console.WriteLine("==========================================");

GlobalMenuHolder.Bootstrap();
GlobalMenuHolder.Run();
