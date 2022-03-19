using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


var users = new List<User>();
var md5 = MD5.Create();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/register", (Register body) =>
{
    var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(body.password));
    User user = new User()
    {
        id = Guid.NewGuid(),
        login = body.login,
        password = hash,
        role = User.Roles.Admin
        
    }; 
    users.Add(user);
    return "Ok";
});
app.MapPost("/login", (Login body) =>
{
    var user = users.Find(u => u.login == body.login);
    
    
    var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(body.password));

    if (user == null) return "Пользователь не найден";

    if (user.password.ToString() == hash.ToString()) return "Вход выполнен";
    return "Неверный пароль";
   
});

app.MapPost("/role", (ChangeRole body) =>
{
    var user = users.Find(u => u.login == body.login);
    if (user == null) return "Пользователь не найден";
    user.role = body.role;
    return user.role.ToString();
});

app.MapGet("/users", () =>
{
    return users;
});

app.Run();

class User
{
    public Guid id { get; set; }
    public string login { get; set; }
    public byte[] password { get; set; }

    public Roles role { get; set; }

    public enum Roles
    {
        Admin,
        Customer,
        Performer
    }
}

class Register
{
    public string login { get; set; }
    public string password { get; set; }
}

class Login
{
    public string login { get; set; }
    public string password { get; set; }
}

class ChangeRole
{
    public string login { get; set; }
    public User.Roles role { get; set; }
}
