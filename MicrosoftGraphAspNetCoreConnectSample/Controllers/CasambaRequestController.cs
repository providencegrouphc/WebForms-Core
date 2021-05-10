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
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Azure.Storage.Blobs.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace PGWebFormsCore.Controllers
{
    public class CasambaRequestController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public CasambaRequestController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> Index(string strSave)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["Message"] = strSave;
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["username"] = User.Identity.Name;
            ViewData["email"] = User.FindFirst("preferred_username")?.Value;
            ViewData["facility"] = await operationlist();
            ViewData["currentdate"] = getDate();
            return View();
        }

        public string getDate()
        {
            string strMonth = DateTime.Now.Month.ToString();
            string strDay = DateTime.Now.Day.ToString();
            string strYear = DateTime.Now.Year.ToString();

            if (strMonth.Length == 1){strMonth = "0" + strMonth;}
            if (strDay.Length == 1) { strDay = "0" + strMonth; }

            return strMonth + "/" + strDay + "/" + strYear;
        }

        public async Task<string> operationlist()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "<select id=\"ddFacility\" style=\"width: 280px!important\" class=\"txtbox\">";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select operationname from operations union select 'Headquarters' order by operationname", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string strSelect = "";

            if (idr.HasRows)
            {
                operations += "<option></option>";
                while (idr.Read())
                {
                    if (response.Contains(Convert.ToString(idr["operationname"])) && strSelect == "")
                    {
                        operations += "<option selected=\"selected\">" + Convert.ToString(idr["operationname"]) + "</option>";
                        strSelect = "select";
                    }
                    else
                    {
                        operations += "<option>" + Convert.ToString(idr["operationname"]) + "</option>";
                    }


                }
            }
            con.Close();

            operations += "</select>";
            return operations;

        }

        [HttpPost]
        public async Task<IActionResult> PostRequest(
            string txtRequestDate, string txtHireDate, string strFacility, string txtFirstName,
            string txtMiddleName, string txtLastName, string txtPhone, string txtEmployeeEmail,
            string txtEmployeeTitle, string strNotes, string txtToName, string txtToEmail,
            string txtRequestName, string txtRequestTitle, string txtRequestEmail)
        {
            try
            {
                if (txtMiddleName is null) { txtMiddleName = ""; }
                if (strNotes is null) { strNotes = ""; }

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_CasambaRequest_Add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@RequestDate", SqlDbType.Date).Value = txtRequestDate;
                cmd.Parameters.Add("@HireDate", SqlDbType.Date).Value = txtHireDate;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strFacility;
                cmd.Parameters.Add("@EmployeeFirstName", SqlDbType.VarChar).Value = txtFirstName;
                cmd.Parameters.Add("@EmployeeMiddleName", SqlDbType.VarChar).Value = txtMiddleName;
                cmd.Parameters.Add("@EmployeeLastName", SqlDbType.VarChar).Value = txtLastName;
                cmd.Parameters.Add("@EmployeePhone", SqlDbType.VarChar).Value = txtPhone;
                cmd.Parameters.Add("@EmployeeEmail", SqlDbType.VarChar).Value = txtEmployeeEmail;
                cmd.Parameters.Add("@EmployeeTitle", SqlDbType.VarChar).Value = txtEmployeeTitle;
                cmd.Parameters.Add("@AdditionalInformation", SqlDbType.VarChar).Value = strNotes;
                cmd.Parameters.Add("@ToName", SqlDbType.VarChar).Value = txtToName;
                cmd.Parameters.Add("@ToEmail", SqlDbType.VarChar).Value = txtToEmail;
                cmd.Parameters.Add("@RequestorName", SqlDbType.VarChar).Value = txtRequestName;
                cmd.Parameters.Add("@RequestorTitle", SqlDbType.VarChar).Value = txtRequestTitle;
                cmd.Parameters.Add("@RequestorEmail", SqlDbType.VarChar).Value = txtRequestEmail;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "Casamba Request";
                string body = "";

                body += "<b>Date of Request:</b> " + txtRequestDate + "<br/>";
                body += "<b>Official Hire Date:</b> " + txtHireDate + "<br/>";
                body += "<b>Facility:</b> " + strFacility + "<br/>";
                body += "<br/><b>Employee Information</b><br/>";
                body += "<b>First Name:</b> " + txtFirstName + "<br/>";
                body += "<b>Middle Name:</b> " + txtMiddleName + "<br/>";
                body += "<b>Last Name:</b> " + txtLastName + "<br/>";
                body += "<b>Phone Number:</b> " + txtPhone + "<br/>";
                body += "<b>Email:</b> " + txtEmployeeEmail + "<br/>";
                body += "<b>Additional Information:</b> " + strNotes + "<br/>";
                body += "<br/><b>Send Credentials To</b<br/>>";
                body += "<b>Name:</b> " + txtToName + "<br/>";
                body += "<b>Email:</b> " + txtToEmail + "<br/>";
                body += "<br/><b>Requestor Information</b><br/>";
                body += "<b>Name:</b> " + txtRequestName + "<br/>";
                body += "<b>Title:</b> " + txtRequestTitle + "<br/>";
                body += "<b>Email:</b> " + txtRequestEmail + "<br/>";


                await GraphService.SendEmail(graphClient, _env, "daniel.stump@pacshc.com", HttpContext, subject, body);


                return RedirectToAction("Index", new { strSave = "Success! Your request was submitted." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }
    }
}
