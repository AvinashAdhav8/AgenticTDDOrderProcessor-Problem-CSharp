// ***************************************************************************
// Copyright (c) 2026, Industrial Logic, Inc., All Rights Reserved.
//
// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
// used by students during Industrial Logic's workshops or by individuals
// who are being coached by Industrial Logic on a project.
//
// This code may NOT be copied or used for any other purpose without the prior
// written consent of Industrial Logic, Inc.
// ****************************************************************************

using Microsoft.Extensions.FileProviders;
using OrderProcessor.Api;
using OrderProcessor.Storage;

var builder = WebApplication.CreateBuilder(args);
var ordersFilePath = builder.Configuration["OrdersFilePath"]
    ?? Path.Combine(builder.Environment.ContentRootPath, "orders.json");
builder.Services.AddSingleton<IOrderStore>(_ => new JsonOrderStore(ordersFilePath));
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
var app = builder.Build();

// Serve the static UI from /frontend. You build index.html / app.js / styles.css there,
// test-driven; this wiring serves whatever you put in that folder at "/".
var frontendPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "frontend"));
if (Directory.Exists(frontendPath))
{
    app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(frontendPath) });
    app.MapGet("/", () => Results.File(Path.Combine(frontendPath, "index.html"), "text/html"));
}

app.MapOrderEndpoints();

app.Run();

// Exposed so unit/integration tests can boot the app in-process via WebApplicationFactory<Program>
// (the fast inner loop) without standing up a live server.
public partial class Program { }
