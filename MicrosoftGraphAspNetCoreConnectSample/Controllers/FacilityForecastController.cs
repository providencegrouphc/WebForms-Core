using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using MicrosoftGraphAspNetCoreConnectSample.Services;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using MicrosoftGraphAspNetCoreConnectSample.Models;
using System;
using System.Collections.Generic;
using Microsoft.Identity.Client;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Data;

namespace PGWebFormsCore.Controllers
{
    public class FacilityForecastController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public FacilityForecastController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            TempData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "FacilityForecast");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["forecastoperations"] = await operationlist();
            return View();
        }

        public async Task<string> operationlist()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "<select id=\"ddfacility\" class=\"txtbox\">";

            //if (response.Contains("PACS Regional Directors of Operations") || response.Contains("Executives_SG") || response.Contains("Regional Directors of Operations DG"))
            //{
                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("select operationname from operations order by operationname", con);
                con.Open();
                SqlDataReader idr = cmd.ExecuteReader();

                if (idr.HasRows)
                {
                    operations += "<option></option>";
                    while (idr.Read())
                    {

                        
                        operations += "<option>" + Convert.ToString(idr["operationname"]) + "</option>";
                    }
                }
                con.Close();

                
            //} else
            //{
            //    var connection = _configuration.GetConnectionString("pgWebForm");
            //    SqlConnection con = new SqlConnection(connection);
            //    SqlCommand cmd = new SqlCommand();
            //    cmd = new SqlCommand("sp_ds_GETFacility", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.Add("@Groups", SqlDbType.VarChar).Value = response;
            //    con.Open();
            //    SqlDataReader idr = cmd.ExecuteReader();


            //    if (idr.HasRows)
            //    {
            //        while (idr.Read())
            //        {

            //            operations += "<option>" + Convert.ToString(idr["operationname"]) + "</option>";
            //        }
            //    }
            //    con.Close();
            //}

            operations += "</select>";
            return operations;
            
        }

        public string AddForecast(string strFacility, string strAmount, string strWeek)
        {

            try
            {
                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_Forecast_add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@loggedby", SqlDbType.VarChar).Value = User.Identity.Name;
                cmd.Parameters.Add("@facility", SqlDbType.VarChar).Value = strFacility;
                cmd.Parameters.Add("@forecastweek", SqlDbType.VarChar).Value = strWeek;
                cmd.Parameters.Add("@amount", SqlDbType.Money).Value = strAmount;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch 
            {
                
            }

            return getforcast(strFacility);
        }

        public string getforcast(string strFacility)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "SELECT * FROM FacilityForecast where Facility = '"+strFacility+ "' order by CAST(right(ForecastWeek, len(ForecastWeek) - charindex(',', ForecastWeek)) as int), CAST(LEFT((RIGHT(ForecastWeek, LEN(ForecastWeek) - 5)) , CHARINDEX(',', (RIGHT(ForecastWeek, LEN(ForecastWeek) - 5)) ) - 1) as int)";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string paymenttable = "<div class=\"tableFixHead\"><table id=\"additonstable\"  style=\"margin-bottom:10px; \">";
            paymenttable += "<thead>";
            paymenttable += "<tr>";
            paymenttable += "<th>Week</th>";
            paymenttable += "<th>Amount</th>";
            paymenttable += "<th>Logged By</th>";
            paymenttable += "<th>Logged Date</th>";
            paymenttable += "</tr>";
            paymenttable += "</thead>";
            paymenttable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    paymenttable += "<tr>";

                    paymenttable += "<td>" + Convert.ToString(idr["forecastweek"]) + "</td>";
                    decimal amount = Convert.ToDecimal(idr["amount"]);
                    paymenttable += "<td>" + amount.ToString("C2") + "</td>";
                    paymenttable += "<td>" + Convert.ToString(idr["loggedby"]) + "</td>";
                    DateTime paymentdate = Convert.ToDateTime(idr["loggeddate"]);
                    paymenttable += "<td>" + paymentdate.ToShortDateString() + "</td>";
                    paymenttable += "</tr>";
                }
            }
            con.Close();

            paymenttable += "</tbody></table></div>";

            return paymenttable;
        }
    }
}
