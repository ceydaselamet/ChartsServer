using ChartsServer.Hubs;
using ChartsServer.Models;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using TableDependency.SqlClient;

namespace ChartsServer.Subscriptions;

public class DatabaseSubscription<T> : IDatabaseSubscription
    where T : class, new()
{

    IConfiguration _configuration;
    IHubContext<SaleHub> _hubContext;

    public DatabaseSubscription(IConfiguration configuration, IHubContext<SaleHub> hubContext)
    {
        _configuration = configuration;
        _hubContext = hubContext;
    }

    SqlTableDependency<T> _tableDependency;
    public void Configure(string tableName)
    {
        _tableDependency = new SqlTableDependency<T>(_configuration.GetConnectionString("SQL"), tableName);
        _tableDependency.OnChanged += async (o, e) =>
        {


            SaleDbContext context = new SaleDbContext();
            var query = (from employee in context.Employees
                        join sale in context.Sales
                        on employee.Id equals sale.Id
                        select new { employee, sale }).ToList();

            List<object> data = new List<object>();
            var employeeNames = query.Select(d =>
              d.employee.FirstName).Distinct().ToList();

            employeeNames.ForEach(em =>
            {
                var prices = query.Where(s => s.employee.FirstName == em)
                                  .Select(s => s.sale.Price).ToList();

                data.Add(new
                {
                    EmployeeName = em,
                    Prices = prices
                });
            });
            await _hubContext.Clients.All.SendAsync("receiveMessage", data);

        };
        _tableDependency.OnError += (o, e) =>
        {

        };

        _tableDependency.Start();
    }

    ~DatabaseSubscription()
    {
        _tableDependency.Stop();
    }
}
