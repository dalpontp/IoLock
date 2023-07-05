using Microsoft.AspNetCore.Authentication.JwtBearer;
using iolock_api.Models;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // KeyCloak
    c.CustomSchemaIds(type => type.ToString());
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "KEYCLOAK",
        Type = SecuritySchemeType.OAuth2,
        In = ParameterLocation.Header,
        BearerFormat = "JWT",
        Scheme = "bearer",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["Jwt:AuthorizationUrl"]),
                TokenUrl = new Uri(builder.Configuration["Jwt:TokenUrl"]),
                Scopes = new Dictionary<string, string> { }
            }
        },
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                                                {securityScheme, new string[] { }}
                                            });
});

builder.Services.AddScoped<IDataAccess, SqlDataAccess>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = "preferred_username",
    };
    o.Authority = builder.Configuration["Jwt:Authority"];
    o.Audience = builder.Configuration["Jwt:Audience"];
    o.RequireHttpsMetadata = false;
    o.Events = new JwtBearerEvents()
    {
        //OnTokenValidated = c => {

        //},
        OnAuthenticationFailed = c =>
        {
            c.NoResult();

            c.Response.StatusCode = 500;
            c.Response.ContentType = "text/plain";

            // Debug only for security reasons
            // return c.Response.WriteAsync(c.Exception.ToString());

            return c.Response.WriteAsync("An error occured processing your authentication.");
        }
    };
});

//builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAppAPI");
        c.OAuthClientId(builder.Configuration["Jwt:ClientId"]);
        c.OAuthClientSecret(builder.Configuration["Jwt:ClientSecret"]);
        c.OAuthRealm(builder.Configuration["Jwt:Realm"]);
        c.OAuthAppName("KEYCLOAK");
    });
    IdentityModelEventSource.ShowPII = true;

}

app.UseHttpsRedirection();

app.UseCors(x =>
    x.AllowAnyHeader()
      .AllowAnyMethod()
      .AllowAnyOrigin()
);

//app.UseStaticFiles();

//if (Directory.Exists(Path.GetFullPath("wwwroot/iolock-webapp")))

//    app.UseFileServer(new FileServerOptions
//    {
//        FileProvider = new PhysicalFileProvider(Path.GetFullPath("wwwroot/iolock-webapp")),
//        RequestPath = "",
//        EnableDirectoryBrowsing = app.Environment.IsDevelopment()
//    });


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
