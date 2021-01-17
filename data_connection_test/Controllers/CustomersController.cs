using Dapper;
using data_connection_test.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace data_connection_test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {

        private readonly IConfiguration config;
        private const string db = "Default";

        public CustomersController(IConfiguration _config)
        {
            config = _config;
        }

        // POST /api/customers
        [HttpPost]
        public ActionResult CreateCustomer(Customer customer)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(config.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@Name", customer.Name);
                p.Add("@Address", customer.Address);
                p.Add("@Phone", customer.Phone);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spCustomer_Insert", p, commandType: CommandType.StoredProcedure);

                customer.Id = p.Get<int>("@id");
            }

            return Ok();
        }


        // GET /api/customers
        [HttpGet]
        public ActionResult<List<Customer>> GetCustomers()
        {
            List<Customer> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(config.GetConnectionString(db)))
            {
                output = connection.Query<Customer>("dbo.spCustomer_GetAll").ToList();
            }

            return output;
        }

        // GET /api/customer/id
        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomer(int id)
        {
            Customer output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(config.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@id", id);
                output = connection.Query<Customer>("dbo.spCustomer_GetById", p, commandType: CommandType.StoredProcedure).SingleOrDefault();

            }
            return output;
        }

        // PUT /api/customer/id
        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(int id, Customer customer)
        {

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(config.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@id", id);
                p.Add("@Name", customer.Name);
                p.Add("@Address", customer.Address);
                p.Add("@Phone", customer.Phone);

                connection.Execute("dbo.spCustomer_UpdateById", p, commandType: CommandType.StoredProcedure);

            }

            return Ok("customer updated.");
        }

        // DELETE /api/customer/id
        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(config.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@id", id);

                connection.Execute("dbo.spCustomer_DeleteById", p, commandType: CommandType.StoredProcedure);

            }

            return Ok("customer deleted!");

        }

    }
}
