using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using to_do_items_test.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Registers controllers to the DI container for handling HTTP requests
builder.Services.AddSingleton<ITodoService, TodoService>(); // Register the TodoService as a singleton service

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    // Define Swagger documentation settings
    c.SwaggerDoc("v1", new() { Title = "To-Do API", Version = "v1" });
});

// Load JWT settings from appsettings.json
// This loads key values for JWT authentication such as the secret key, issuer, and audience
var jwtSettings = builder.Configuration.GetSection("Jwt"); // Gets the "Jwt" section from appsettings.json
var key = jwtSettings["Key"]; // Secret key used to sign the JWT
var issuer = jwtSettings["Issuer"]; // Issuer of the JWT
var audience = jwtSettings["Audience"]; // Audience for which the JWT is intended

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    // Set default authentication scheme to JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configure JWT Bearer authentication options
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Ensures that the JWT's issuer matches the configured issuer
        ValidateAudience = true, // Ensures that the JWT's audience matches the configured audience
        ValidateLifetime = true, // Validates the expiration time of the JWT
        ValidateIssuerSigningKey = true, // Validates the signing key used in the JWT
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), // The secret key used to sign the JWT
        ValidIssuer = issuer, // The expected issuer of the JWT
        ValidAudience = audience // The expected audience of the JWT
    };

    // Store JWT in HttpOnly cookie for added security
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Extract JWT from a cookie named "access_token"
            var token = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token; // Set the token to be validated
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Displays detailed error information during development
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS for added security
app.UseAuthentication(); // Enables authentication middleware for validating JWT
app.UseAuthorization(); // Enables authorization middleware

// Middleware for adding security headers to HTTP responses
app.Use(async (context, next) =>
{
    try
    {
        // Check and update security headers to enhance protection against common attacks

        // X-Content-Type-Options: Prevents MIME-sniffing attacks by forcing the correct MIME type
        if (!context.Response.Headers.TryGetValue("X-Content-Type-Options", out var contentTypeOption) || contentTypeOption != "nosniff")
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        }

        // X-Frame-Options: Prevents clickjacking attacks by disallowing iframes
        if (!context.Response.Headers.TryGetValue("X-Frame-Options", out var frameOptions) || frameOptions != "DENY")
        {
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }

        // X-XSS-Protection: Enables Cross-Site Scripting filter
        if (!context.Response.Headers.TryGetValue("X-XSS-Protection", out var xssProtection) || xssProtection != "1; mode=block")
        {
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        }

        // Strict-Transport-Security: Enforces HTTPS connections for a year (31536000 seconds)
        if (!context.Response.Headers.TryGetValue("Strict-Transport-Security", out var hsts) || hsts != "max-age=31536000; includeSubDomains")
        {
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        // Referrer-Policy: Controls the information sent in the Referer header
        if (!context.Response.Headers.TryGetValue("Referrer-Policy", out var referrerPolicy) || referrerPolicy != "no-referrer")
        {
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
        }
    }
    catch (ArgumentException ex)
    {
        // Log the exception if any issues occur while adding headers
        Console.WriteLine($"Error adding security headers: {ex.Message}");
    }

    await next(); // Passes the request to the next middleware component
});
app.MapControllers(); // Maps controller endpoints to the HTTP pipeline
app.UseSwagger(); // Enables Swagger for generating API documentation
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "To-Do API v1")); // Configures Swagger UI for API exploration

app.Run(); // Starts the application
