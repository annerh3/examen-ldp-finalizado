using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using ProyectoExamenU1.Constants;
using SolicitudPermiso.Database.Entities;

namespace ProyectoExamenU1.Database
{
    public class PermissionRequestSeeder
    {

        public static async Task LoadDataAsync(
            PermissionRequestContext context,
            ILoggerFactory loggerFactory,
            UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            try
            {
                await LoadRolesAndUSersAsync(userManager, roleManager, loggerFactory);
                await LoadPermissionTypesAsync(loggerFactory, context);
                await LoadPermissionStatusAsync(loggerFactory, context);

            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<PermissionRequestSeeder>();
                logger.LogError(e, "Error inicializando la data del API");
            }
        }


        public static async Task LoadRolesAndUSersAsync(
            UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            try
            {
                if (!await roleManager.Roles.AnyAsync())
                {
                    //Evitamos tener los string quemados
                    await roleManager.CreateAsync(new IdentityRole(RolesConstant.ADMIN));
                    await roleManager.CreateAsync(new IdentityRole(RolesConstant.HUMAN_RESOURCES));
                    await roleManager.CreateAsync(new IdentityRole(RolesConstant.EMPLOYEE));
                    await roleManager.CreateAsync(new IdentityRole(RolesConstant.COUNTER));
                    await roleManager.CreateAsync(new IdentityRole(RolesConstant.ITMANAGER));
                    await roleManager.CreateAsync(new IdentityRole(RolesConstant.MARKETING));
                }

                if (!await userManager.Users.AnyAsync())
                {
                    var userAdmin = new Employee
                    {
                        Email = "admin@gmail.com",
                        UserName = "admin@gmail.com",
                        Name = "Martha Escobar",
                        EmployeeJoinDate = new DateOnly(2003, 10, 23)  // AAAA, MM, DD

                    };

                    var userHumanResources = new Employee // Human Resources
                    {
                        Email = "marlon_alejandro@gmail.com",
                        UserName = "marlon_alejandro@gmail.com",
                        Name = "Marlon Alejandro Santos",
                        EmployeeJoinDate = new DateOnly(2009, 01, 23)
                    };

                    var userCounter = new Employee 
                    {
                        Email = "walter_white@gmail.com",
                        UserName = "walter_white@gmail.com",
                        Name = "Walter White",
                        EmployeeJoinDate = new DateOnly(2002, 12, 12)
                    };

                    var userITManager = new Employee 
                    {
                        Email = "elliot_alderson@gmail.com",
                        UserName = "elliot_alderson@gmail.com",
                        Name = "Elliot Alderson",
                        EmployeeJoinDate = new DateOnly(2012, 04, 02)
                    };

                    var userMarketing = new Employee 
                    {
                        Email = "deborah_vance@gmail.com",
                        UserName = "deborah_vancen@gmail.com",
                        Name = "Deborah Vance",
                        EmployeeJoinDate = new DateOnly(2015, 06, 17)
                    };


                    int employeeAmount = 5; // cantidad de empleados

                    List<Employee> employees = new List<Employee>();


                    for (int i = 1; i <= employeeAmount; i++)
                    {
                        var employee = new Employee
                        {
                            Email = $"employee{i}@gmail.com",
                            UserName = $"employee{i}@gmail.com",
                            Name = $"Employee{i}",
                            EmployeeJoinDate = new DateOnly(2013, 09, 01)
                        };

                        employees.Add(employee);
                        await userManager.CreateAsync(employee, "Temporal01*");
                    }

                    await userManager.CreateAsync(userAdmin, "Temporal01*");
                    await userManager.CreateAsync(userHumanResources, "Temporal01*");
                    await userManager.CreateAsync(userCounter, "Temporal01*");
                    await userManager.CreateAsync(userITManager, "Temporal01*");
                    await userManager.CreateAsync(userMarketing, "Temporal01*");
                    



                    await userManager.AddToRoleAsync(userAdmin, RolesConstant.ADMIN);
                    await userManager.AddToRoleAsync(userHumanResources, RolesConstant.HUMAN_RESOURCES);
                    await userManager.AddToRoleAsync(userCounter, RolesConstant.COUNTER);
                    await userManager.AddToRoleAsync(userITManager, RolesConstant.ITMANAGER);
                    await userManager.AddToRoleAsync(userMarketing, RolesConstant.MARKETING);

                    // Asignar rol a los empleados en la variable empleados 
                    foreach (var employee in employees)
                    {
                        await userManager.AddToRoleAsync(employee, RolesConstant.EMPLOYEE);
                    }
                }

            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<PermissionRequestSeeder>();
                logger.LogError(e.Message);
            }
        }

        public static async Task LoadPermissionTypesAsync(ILoggerFactory loggerFactory, PermissionRequestContext context)
        {
            try
            {
                var jsonFilePath = "SeedData/permission_types.json";
                var jsonContent = await File.ReadAllTextAsync(jsonFilePath); 
                var permission_types = JsonConvert.DeserializeObject<List<PermissionTypeEntity>>(jsonContent);
               
                if (!await context.PermissionTypes.AnyAsync()) 
                {
                    context.AddRange(permission_types); 
                    await context.SaveChangesAsync(); 
                }
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<PermissionRequestContext>();
                logger.LogError(e, "Error al ejecutar el Seed de tipos de permisos.");
            }
        }

        public static async Task LoadPermissionStatusAsync(ILoggerFactory loggerFactory, PermissionRequestContext context)
        {
            try
            {
                var jsonFilePath = "SeedData/permission_status.json";
                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var permission_status = JsonConvert.DeserializeObject<List<PermissionRequestStatusEntity>>(jsonContent);

                if (!await context.PermissionRequestStatus.AnyAsync())
                {
                    context.AddRange(permission_status);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<PermissionRequestContext>();
                logger.LogError(e, "Error al ejecutar el Seed de estados de permiso.");
            }
        }

    }
}
