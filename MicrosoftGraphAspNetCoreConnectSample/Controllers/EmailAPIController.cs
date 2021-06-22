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
using PGWebFormsCore.Models;


namespace PGWebFormsCore.Controllers
{

    public class EmailAPIController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public EmailAPIController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;

        }

        public IActionResult Index()
        {
            return View();
        }

        public List<EmailAPI> EmailAPIList(string apipw)
        {
            var connection = _configuration.GetConnectionString("emailapi");
            List<EmailAPI> EmailAPISend;
            if (apipw == connection)
            {
                EmailAPISend = GetExpenseSupEmail();
            } else
            {
                EmailAPISend = new List<EmailAPI>();
            }

            return EmailAPISend;
        }

        public List<EmailAPI> GetExpenseSupEmail()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select distinct ApprovedEmail, ApprovedBy from Expense where supapprove = 0", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            List<EmailAPI> SupEmail = new List<EmailAPI>();

            string ebodyh = "Good Morning ";
            string ebody = ",</br></br>This email is to notify you that ";
            ebody += "their are expenses that are waiting for you to approve. You can access these expenses at:</br></br>";
            ebody += "<a href=\"https://pacs-technology.com/Expense/Sup\">https://pacs-technology.com/Expense/Sup</a></br></br>";
            ebody += "Thank you and have a pleasent day!</br></br></br></br>";
            ebody += "This email box is unmonitored. Please do not reply";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    SupEmail.Add(new EmailAPI
                    {
                        //EmailAddress = Convert.ToString(idr["ApprovedEmail"]),
                        EmailAddress = "daniel.stump@pacshc.com",
                        EmailSubject = "Expense Approval Needed",
                        EmailBody = ebodyh + Convert.ToString(idr["ApprovedBy"]) + ebody,
                    });
                }
            }
            con.Close();

            

            return GetExpenseFinanceEmail(SupEmail);
        }

        public List<EmailAPI> GetExpenseFinanceEmail(List<EmailAPI> SupEmail)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select distinct submitemail, submitby from Expense where FinanceApproved = 0 and supapprove = 1 and TreasuryApproved = 1 and ApprovalStatus = 'Processing' Order by SubmitBy", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string ebody = "Good Morning,</br></br>As a member of the finance group, this email is to notify you that ";
            ebody += "their are expenses that are waiting for finance to approve. You can access these expenses at:</br></br>";
            ebody += "<a href=\"https://pacs-technology.com/Expense/Finance\">https://pacs-technology.com/Expense/Finance</a></br></br>";
            ebody += "Thank you and have a pleasent day!</br></br></br></br>";
            ebody += "This email box is unmonitored. Please do not reply";

            if (idr.HasRows)
            {
                    SupEmail.Add(new EmailAPI
                    {
                        EmailAddress = "daniel.stump@pacshc.com",
                        EmailSubject = "Finance Expense Approval Needed",
                        EmailBody = ebody,
                    });
                
            }
            con.Close();
            return SupEmail;
        }
    }
}
