/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

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
using Microsoft.AspNetCore.Http;

namespace MicrosoftGraphAspNetCoreConnectSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;
    public HomeController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
        }



        [AllowAnonymous]
        // Load user's profile.
        public async Task<IActionResult> Index(string email)
        {

            if (User.Identity.IsAuthenticated)
            {
                var connection = _configuration.GetConnectionString("pgWebForm");
                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
                email = User.FindFirst("preferred_username")?.Value;




                ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
                email ??= User.FindFirst("preferred_username")?.Value;
                ViewData["Email"] = email;

                // Initialize the GraphServiceClient.

                ViewData["Response"] = await GraphService.GetUserJson(graphClient, email, HttpContext);

                ViewData["Picture"] = await GraphService.GetPictureBase64(graphClient, email, HttpContext);
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        // Send an email message from the current user.
        public async Task<IActionResult> SendEmail(string recipients)
        {
            if (string.IsNullOrEmpty(recipients))
            {
                TempData["Message"] = "Please add a valid email address to the recipients list!";
                return RedirectToAction("Index");
            }

            try
            {
                // Initialize the GraphServiceClient.
                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                // Send the email.
                await GraphService.SendEmail(graphClient, _env, recipients, HttpContext, "test", "test");

                // Reset the current user's email address and the status to display when the page reloads.
                TempData["Message"] = "Success! Your mail was sent.";
                return RedirectToAction("Index");
            }
            catch (ServiceException se)
            {
                if (se.Error.Code == "Caller needs to authenticate.") return new EmptyResult();
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }
        }


        public IActionResult About()
        {
            Dropdownlist multi_Dropdownlist = new Dropdownlist
            {
                operationlist = GetOperationList(),
                issuelist = GetIssueList(),
                getGUID = SaveGUID()
            };
            return View(multi_Dropdownlist);
        }

        public List<Operation_List> GetOperationList()
        {

            //SqlConnectionStringBuilder conbuild = new SqlConnectionStringBuilder();
            //conbuild["Data Source"] = "pgsql02";
            //conbuild["User ID"] = "appWebForm";
            //conbuild["Password"] = "$25/!agv5’wl1-$:b'";
            //conbuild["Initial Catalog"] = "pgWebForm";


            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("Select operationName From operations order by operationName", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            List<Operation_List> operation = new List<Operation_List>();
            operation.Insert(0, new Operation_List { operationName = "" });
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    operation.Add(new Operation_List
                    {
                        operationName = Convert.ToString(idr["operationName"])
                    });
                }
            }
            con.Close();
            return operation;
        }

        public List<Issue_list> GetIssueList()

        {
            //SqlConnectionStringBuilder conbuild = new SqlConnectionStringBuilder();
            //conbuild["Data Source"] = "pgsql02";
            //conbuild["User ID"] = "appWebForm";
            //conbuild["Password"] = "$25/!agv5’wl1-$:b'";
            //conbuild["Initial Catalog"] = "FinanceDW";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("SELECT ticketIssueType AS ISSUENAME, ticketIssueTypeTarget AS ISSUETARGET FROM TICKETISSUETYPES ORDER BY ticketIssueTypeSortOrder", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            List<Issue_list> Issues = new List<Issue_list>();
            Issues.Insert(0, new Issue_list { IssueTarget = "", IssueName = ""});
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    Issues.Add(new Issue_list
                    {
                        IssueTarget = Convert.ToString(idr["IssueTarget"]),
                        IssueName = Convert.ToString(idr["IssueName"]),
                    });
                }
            }
            con.Close();
            return Issues;
        }

        public string SaveGUID()
        {
            
            string userName = User.FindFirst("name").Value;
            string userEmail = User.FindFirst("preferred_username")?.Value;
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("DECLARE @NEWID UNIQUEIDENTIFIER = NEWID() INSERT INTO DSTEST(ISSUEID, LOGDATE, USERNAME, EMAIL) VALUES(@NEWID, GETDATE(), '"+userName+"', '"+userEmail+"') SELECT @NEWID", con);
            con.Open();
            string UID = cmd.ExecuteScalar().ToString();
            con.Close();


            return UID;
        }

        public string issuechanged(string stritem, string struid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("UPDATE DSTEST SET ISSUE = '" + stritem + "' WHERE ISSUEID = '" + struid + "'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();


            return "";
        }

        [HttpPost]
        public IActionResult PostForm(string lastName, string facility, string UID, string Phone)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("UPDATE DSTEST SET FACILITY = '" + facility + "', USERNAME = '"+lastName+"', PHONENUMBER = '"+Phone+"' WHERE ISSUEID = '" + UID + "'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Error", "Home", new { message = lastName + " " + facility + " " + UID}); 
        }

            [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
