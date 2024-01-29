using System;
namespace AdminWebAPI.Models;

public class User
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public static class UserLogin
{
    public const string UserName = "admin";
    public const string Password = "admin";
}
