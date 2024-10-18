var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<MyMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var companies = new List<Company>
{
    new Company { Id = 1, Name = "Coca Cola" },
    new Company { Id = 2, Name = "Pepsi" }
};

var employees = new List<Employee>
{
    new Employee { Id = 1, Name = "Juan", Position = "Frontend", Salary = "2000USD", CompanyId = 1 },
    new Employee { Id = 2, Name = "Jose", Position = "Backend", Salary = "3000USD", CompanyId = 2 }
};

int employeeIdCounter = employees.Max(e => e.Id);

// CRUD de Company
app.MapGet("/Company", () =>
{
    return Results.Ok(companies);
});

// Company por Id
app.MapGet("/Company/{Id}", (int Id) =>
{
    var company = companies.FirstOrDefault(p => p.Id == Id);
    return company != null ? Results.Ok(company) : Results.NotFound();
});

// Crear un Company
app.MapPost("/Company", (Company newCompany) =>
{
    // Asigna nuevo id incrementando el maximo actual
    newCompany.Id = companies.Max(p => p.Id) + 1;
    // Ahora añade  la nueva company a la lista
    companies.Add(newCompany);
    // Retorna 201 created con la ubicacion del nuevo recurso
    return Results.Created($"/Company/{newCompany.Id}", newCompany);
});

// Update de company
app.MapPut("/Company/{Id}", (int Id, Company updatedCompany) =>
{
    // Primero lo busca por Id
    var company = companies.FirstOrDefault(p => p.Id == Id);
    if (company == null)
    {
        return Results.NotFound();
    }
    // Actualiza el nombre de la company
    company.Name = updatedCompany.Name;
    return Results.NoContent();
});

// Delete company by id
app.MapDelete("/Company/{Id}", (int Id) =>
{
    var company = companies.FirstOrDefault(c => c.Id == Id);
    if (company is null) return Results.NotFound();

    var hasEmployees = employees.Any(e => e.CompanyId == Id);
    if (hasEmployees)
        return Results.BadRequest("Cannot delete company because it has assigned employees.");

    companies.Remove(company);
    return Results.NoContent();
});

// CRUD de Employees
app.MapGet("/employees", () => employees);

// Empleado por id
app.MapGet("/employees/{id}", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    return employee is not null ? Results.Ok(employee) : Results.NotFound();
});

// Crea empleado
app.MapPost("/employees", (Employee employee) =>
{
    // Asigna un nuevo id incrementado el contador
    employee.Id = ++employeeIdCounter;
    employees.Add(employee);
    return Results.Created($"/employees/{employee.Id}", employee);
});

// Actualiza el empleado
app.MapPut("/employees/{id}", (int id, Employee updatedEmployee) =>
{
    // Busca por id
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is null) return Results.NotFound();

    // Actualiza las propiedades del empleado
    employee.Name = updatedEmployee.Name;
    employee.CompanyId = updatedEmployee.CompanyId;
    // Y retorna el emp actualizado
    return Results.Ok(employee);
});

// Delete de empleado por id
app.MapDelete("/employees/{id}", (int id) =>
{
    // Busca el empleado por id 
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is null) return Results.NotFound();

    employees.Remove(employee);
    return Results.NoContent();
});

app.Run();


// Modelos
public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public string Salary { get; set; }
    public int CompanyId { get; set; }
}

public class MyMiddleware
{
    private readonly RequestDelegate _next;

    public MyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);
        Console.WriteLine("-------------------->TAMO ADENTROOOOOOOOOOOOOOOOOOO<------------------------");
    }
}

