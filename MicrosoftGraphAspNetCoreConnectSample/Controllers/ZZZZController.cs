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
using System.Data;

namespace PGWebFormsCore.Controllers
{
    public class ZZZZController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public ZZZZController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string SaveTime(string stritem)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            string returntext = "";
            DateTime dateValue = DateTime.Now;
            returntext = dateValue.AddHours(1).ToString("MM/dd/yyyy hh:mm:ss.fff tt");

            var sqlcommandtext = "insert into dsTest (IssueID, LogDate, JavaDate) values (NEWID(), GETDATE(), '"+returntext+"')";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            
            if (idr.HasRows)
            {
                while (idr.Read())
                {

                }
            }
            con.Close();

            return "";
        }

        public string GetTime(string stritem)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select logdate from dsTest order by LogDate";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returntext = "";
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    DateTime dateValue = Convert.ToDateTime(idr["logdate"]);
                    returntext += dateValue.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + "</br>";
                }
            }
            con.Close();

            return returntext;
        }

        public string GetCount(string stritem)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select count(logdate) as sqlcount from dsTest";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returntext = "";
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returntext += Convert.ToString(idr["sqlcount"]);
                }
            }
            con.Close();

            return returntext;
        }

        public string Clear(string stritem)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "delete from dsTest";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();


            con.Close();

            return "";
        }
    }
}
