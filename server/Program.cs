using PDF2;
using PDF1;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Replace with your React app's domain
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
}); 
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp"); // Enable CORS with the policy for your React app


JArray dataArray = null;
app.MapPost("/excelData", async (HttpRequest request) =>
{
    using (StreamReader reader = new StreamReader(request.Body))
    {
        string jsonString = await reader.ReadToEndAsync();
        JObject jsonData = JObject.Parse(jsonString);

        // Handle JSON data here
        // Example: Extract data from JSON object
        int nbRows = 5;
       dataArray = (JArray)jsonData["dataArray"];
        Console.WriteLine(dataArray);
        var pdf = new Generate65PDF1(800, 1000,nbRows,dataArray);
    }

    // Return your response, if needed
    return Results.Ok("Excel data received successfully");
})
.WithName("ExcelData")
.WithOpenApi();

app.MapPost("/params", (string paperWidth, string paperHeight,string nbRows) =>
{
    Console.WriteLine("{0} {1}", paperHeight, paperWidth);
    
    // Convert paperWidth and paperHeight to integers
    int width = Convert.ToInt32(paperWidth);
    int height = Convert.ToInt32(paperHeight);
    int rows = Convert.ToInt32(nbRows);
    
    // Generate PDF with the specified width and height
    var pdf = new Generate65PDF1(width, height,rows,dataArray);
    
    var filePath = "65pdf.pdf"; // Path to the generated PDF file
    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
    
    // Return the file as a response with the appropriate content type
    return Results.File(fileBytes, "application/pdf", "65pdf.pdf");
})
.WithName("PostParams")
.WithOpenApi();
app.MapGet("/generate65", () => 
{       

    var filePath = "65pdf.pdf"; // Path to the generated PDF file

    // Read the file into a byte array
    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
    //  Console.WriteLine(" {0} {1} ", paperHeight,paperWidth);
    // Return the file as a response with the appropriate content type
    return Results.File(fileBytes, "application/pdf", "65pdf.pdf");
})
.WithName("GetPDF65")
.WithOpenApi();

app.MapGet("/generate24", () => 
{       
    var pdf = new Generate24PDF1();
    var filePath = "24pdf.pdf"; // Path to the generated PDF file

    // Read the file into a byte array
    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
   
    // Return the file as a response with the appropriate content type
    return Results.File(fileBytes, "application/pdf", "24pdf.pdf");
})
.WithName("GetPDF24")
.WithOpenApi();

app.Run();
