using AuthServer.OpenIddict;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Configure the context to use an in-memory store.
    options.UseInMemoryDatabase("auth-server-db");

    // Register the entity sets needed by OpenIddict.
    // Note: use the generic overload if you need
    // to replace the default OpenIddict entities.
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()

    // Register the OpenIddict core services.
    .AddCore(options =>
    {
        // Configure OpenIddict to use the EF Core stores/models.
        options.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })

    // Register the OpenIddict server handler.
    .AddServer(options =>
    {
        // Register the ASP.NET Core services used by OpenIddict.
        // Note: if you don't call this method, you won't be able to
        // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();

        // Enable the required endpoints
        options.SetTokenEndpointUris("/connect/token");
        options.SetUserinfoEndpointUris("/connect/userinfo");

        // Register signing and encryption details.
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Enable the password flow.
        options.AllowPasswordFlow();

        // Enable the refresh token flow.
        options.AllowRefreshTokenFlow();

        // Disable the access token encryption.
        options.DisableAccessTokenEncryption();

        // Set the lifetime of tokens
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));

        // Accept anonymous clients (i.e clients that don't send a client_id).
        options.AcceptAnonymousClients();
    })

    // Register the OpenIddict validation handler.
    // Note: the OpenIddict validation handler is only compatible with the
    // default token format or with reference tokens and cannot be used with
    // JWT tokens. For JWT tokens, use the Microsoft JWT bearer handler.
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
    options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
