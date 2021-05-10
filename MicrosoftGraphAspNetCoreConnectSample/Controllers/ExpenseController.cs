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

namespace PGWebFormsCore.Controllers
{
    public class ExpenseController : Controller
    {




        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public ExpenseController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
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
            string accountcheck = await checkAccounting();
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["ownexpense"] = getownexpense();
            ViewData["checksup"] = checkifsup() + accountcheck;
            ViewData["getyears"] = getyear();
            ViewData["getmonths"] = getmonth();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Sup()
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["supexpenseg"] = await getsupexpensegroup();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> SupSearch()
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["supexpense"] = getsupexpense();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Finance()
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["getfinance"] = await getfinexpensegroup();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> SupEdit(string supeditid, string supredirect)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["supeditdata"] = await getsupedit(supeditid);
            ViewData["supredirect"] = supredirect;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> FinanceEdit(string editid)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["financeeditdata"] = await getfinanceedit(editid);
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Edit(string editid)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["editdata"] =  getedit(editid);

            string attachmentid = getattachmentid(editid);
            ViewData["attachmentid"] = attachmentid;
            ViewData["attachments"] = await GetImages(attachmentid);
            return View();
        }

        [Authorize]
        public async Task<IActionResult> New()
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["userlist"] = await getusers("a");
            ViewData["curuser"] = User.Identity.Name;
            ViewData["UID"] = getGUID();

            string cats = getCats();
            string facilities = getFacilities();
            ViewData["CATS"] = "<select id=\"ddCat\" style=\"width: 280px!important\" class=\"txtbox\">" + cats;
            ViewData["DCATS"] = "<select id=\"ddDCat\" style=\"width: 280px!important\" class=\"txtbox\">" + cats;
            ViewData["MCATS1"] = "<select id=\"ddMCat1\" class=\"txtboxmd\">" + cats;
            ViewData["MCATS2"] = "<select id=\"ddMCat2\" class=\"txtboxmd\">" + cats;
            ViewData["MCATS3"] = "<select id=\"ddMCat3\" class=\"txtboxmd\">" + cats;
            ViewData["MCATS4"] = "<select id=\"ddMCat4\" class=\"txtboxmd\">" + cats;
            ViewData["MCATS5"] = "<select id=\"ddMCat5\" class=\"txtboxmd\">" + cats;
            ViewData["MCATS6"] = "<select id=\"ddMCat6\" class=\"txtboxmd\">" + cats;

            ViewData["Facilities1"] = "<select id=\"ddFac\" class=\"txtbox\" style=\"width: 280px!important\">" + facilities;
            ViewData["Facilities2"] = "<select id=\"ddDFac\" class=\"txtbox\" style=\"width: 280px!important\">" + facilities;
            ViewData["Facilities3"] = "<select id=\"ddMFac\" class=\"txtbox\" style=\"width: 280px!important\">" + facilities;
            return View();
        }

        public string getattachmentid(string editid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("SELECT attachmentid, expensetype, approvalstatus from expense where id = '"+editid+"'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "stop";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (Convert.ToString(idr["expensetype"]) == "MULTIPLE")
                    {

                    } else
                    {
                        returnvalue = Convert.ToString(idr["AttachmentID"]);
                        ViewData["exptype"] = Convert.ToString(idr["expensetype"]);
                        ViewData["apstatus"] = Convert.ToString(idr["approvalstatus"]);
                    }
                    
                }
            }
            con.Close();
            return returnvalue;
        }

        public async Task<string> checkAccounting()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "";

            if (response.Contains("Finance") || response.Contains("Executives_SG"))
            {
                operations = "<a style=\"margin-left:15px;\" href=\"/Expense/Finance\" class=\"btn btn-primary\">Finance View</a> <button type=\"submit\" class=\"btn btn-primary\" onclick=\"genreport()\" style=\"margin-left:15px;\">Expense Report</button>";
            }
            
            return operations;

        }

        public string getedit(string editid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from Expense where id = '" + editid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";
            string bgcolor = "lightblue";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    DateTime expensedate = Convert.ToDateTime(idr["expensedate"]);
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    {
                        bgcolor = "lightgreen";
                    }

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Declined")
                    {
                        bgcolor = "lightcoral";
                    }
                    
                    returnvalue += "<input type=\"text\" class=\"hidden\" value=\"" + editid + "\" id=\"UID\"><input type=\"text\" class=\"hidden\" value=\"" + Convert.ToString(idr["SubmitEmail"]) + "\" id=\"txtUser\">";
                    returnvalue += "<div style=\"background-color: " + bgcolor + "; width:100%; text-align:center; padding: 5px; border: 1px solid black; margin-bottom:10px; border-radius: 5px\"><b>" + Convert.ToString(idr["ApprovalStatus"]) + "</b></div>";
                    returnvalue += "<div class=\"row\"><div class=\"col-md-4\">";
                    returnvalue += "<div class=\"txtlabel\">Status Change Notes</div>";
                    returnvalue += "<textarea disabled=\"disabled\" class=\"txtbox\" style=\"height:50px\" id=\"txtNotes\">" + Convert.ToString(idr["ApproveNotes"]) + "</textarea>";
                    
                    
                    returnvalue += "</div><div class=\"col-md-4\"><div style=\"margin-top: 20px; margin-bottom: 10px\"><b><i>Submitted: " + Convert.ToString(idr["SubmitDate"]) + "</i></b></div>";

                    if (Convert.ToString(idr["ApprovalDate"]) == "")
                    { }
                    else
                    {
                        returnvalue += "<div><b><i>Status Changed By: " + Convert.ToString(idr["ApprovedBy"]) + "</i></b></div>";
                        returnvalue += "<div><b><i>Status Changed: " + Convert.ToString(idr["ApprovalDate"]) + "</i></b></div>";
                    }

                    returnvalue += "</div></div><hr>";

                    

                    if (Convert.ToString(idr["ExpenseType"]) == "EXPENSE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">* Facility</div><div>" + getFacilitiesEdit(Convert.ToString(idr["Facility"])) + "</div>";
                        returnvalue += "<div id=\"validateFacility\" class=\"hidden\">Please select a facility.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Merchant</div><div><input type=\"text\" id=\"txtMerchant\" class=\"txtbox\" value=\"" + Convert.ToString(idr["Merchant"]) + "\"/></div>";
                        returnvalue += "<div id=\"validateMerchant\" class=\"hidden\">Please enter a merchant.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Date</div><div><input type=\"text\" id=\"txtDate\" class=\"txtbox\" value=\"" + expensedate.ToString("MM/dd/yyyy") + "\"/></div>";
                        returnvalue += "<div id=\"validateDate\" class=\"hidden\">Please enter a valid date.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Total</div><div><input type=\"text\" id=\"txtTotal\" class=\"txtbox\" value=\"" + expensetotal.ToString() + "\"/></div>";
                        returnvalue += "<div id=\"validateTotal\" class=\"hidden\">Please enter a valid total.</div>";

                        if (Convert.ToString(idr["reimbursable"]) == "False")
                        {
                            returnvalue += "<div class=\"hidden\"><input id=\"cbreimbursable\" type=\"checkbox\" />&nbsp;&nbsp;Reimbursable</div>";
                        }
                        else
                        {
                            returnvalue += "<div class=\"hidden\"><input id=\"cbreimbursable\" type=\"checkbox\" checked=\"checked\"/>&nbsp;&nbsp;Reimbursable</div>";
                        }

                        returnvalue += "<div class=\"txtlabel\">* Category</div><div>" + getCatsEdit(Convert.ToString(idr["Category"])) + "</div>";
                        returnvalue += "<div id=\"validateCategory\" class=\"hidden\">Please select a category.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Attendees</div><div><input type=\"text\" id=\"txtAttendees\" class=\"txtbox\" value=\"" + Convert.ToString(idr["Attendees"]) + "\"/></div>";
                        returnvalue += "<div id=\"validateAttendees\" class=\"hidden\">Please enter an attendee.</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div><input type=\"text\" id=\"txtDescription\" class=\"txtbox\" value=\"" + Convert.ToString(idr["ExpenseDescription"]) + "\"/></div>";
                        returnvalue += "<div class=\"hidden\">* Report</div><div class=\"hidden\"><select id=\"ddReport\" class=\"txtbox\" style=\"width: 280px!important\"><option>" + Convert.ToString(idr["Report"]) + "</option></select></div></div>";
                        returnvalue += "<div id=\"validateReport\" class=\"hidden\">Please select a report.</div>";

                    }

                    if (Convert.ToString(idr["ExpenseType"]) == "MULTIPLE")
                    {
                        returnvalue += "<div class=\"txtlabel\">* Facility</div><div>" + getFacilitiesEdit(Convert.ToString(idr["Facility"])) + "</div>";
                        returnvalue += "<div id=\"validateFacility\" class=\"hidden\">Please select a facility.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Merchant</div><div><input type=\"text\" id=\"txtMerchant\" class=\"txtbox\" value=\"" + Convert.ToString(idr["Merchant"]) + "\"/></div>";
                        returnvalue += "<div id=\"validateMerchant\" class=\"hidden\">Please enter a merchant.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Date</div><div><input type=\"text\" id=\"txtDate\" class=\"txtbox\" value=\"" + expensedate.ToString("MM/dd/yyyy") + "\"/></div>";
                        returnvalue += "<div id=\"validateDate\" class=\"hidden\">Please enter a valid date.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Total</div><div><input type=\"text\" id=\"txtTotal\" class=\"txtbox\" value=\"" + expensetotal.ToString() + "\"/></div>";
                        returnvalue += "<div id=\"validateTotal\" class=\"hidden\">Please enter a valid total.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Category</div><div>" + getCatsEdit(Convert.ToString(idr["Category"])) + "</div>";
                        returnvalue += "<div id=\"validateCategory\" class=\"hidden\">Please select a category.</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div><input type=\"text\" id=\"txtDescription\" class=\"txtbox\" value=\"" + Convert.ToString(idr["ExpenseDescription"]) + "\"/></div>";

                        if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                        { }
                        else
                        {
                            returnvalue += "<input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px; margin-top:15px;\" value=\"Submit\" onclick=\"validatesubmulti()\" />";
                            returnvalue += "<input type=\"button\" class=\"btn btn-danger\" style=\"margin-bottom:10px; margin-top:15px; margin-left:30px\" value=\"Delete\" onclick=\"showdelexpense()\" />";
                        }
                    }

                    if (Convert.ToString(idr["ExpenseType"]) == "DISTANCE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">* Facility</div><div>" + getFacilitiesEdit(Convert.ToString(idr["Facility"])) + "</div>";
                        returnvalue += "<div id=\"validateFacility\" class=\"hidden\">Please select a facility.</div>";
                        returnvalue += "<div class=\"txtlabel\">* From Address</div><div><input type=\"text\" id=\"txtFromAddress\" class=\"txtbox\" value=\"" + Convert.ToString(idr["FromAddress"]) + "\"/></div>";
                        returnvalue += "<div id=\"validateFromAddress\" class=\"hidden\">Please enter a from address.</div>";
                        returnvalue += "<div class=\"txtlabel\">* To Address</div><div><input type=\"text\" id=\"txtToAddress\" class=\"txtbox\" value=\"" + Convert.ToString(idr["ToAddress"]) + "\"/></div>";
                        returnvalue += "<div id=\"validateToAddress\" class=\"hidden\">Please enter a to address.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Distance</div><div><input type=\"text\" id=\"txtDistance\" class=\"txtbox\" value=\"" + Convert.ToString(idr["Distance"]) + "\" onkeyup=\"calcmile()\"/></div>";
                        returnvalue += "<div id=\"validateDistance\" class=\"hidden\">Please enter a valid distance.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Rate</div><div><select id=\"ddRate\" class=\"txtbox\" style=\"width: 280px!important\"><option>" + Convert.ToString(idr["Rate"]) + "</option></select></div>";
                        returnvalue += "<div id=\"validateRate\" class=\"hidden\">Please enter a valid distance.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Date</div><div><input type=\"text\" id=\"txtDate\" class=\"txtbox\" value=\"" + expensedate.ToString("MM/dd/yyyy") + "\"/></div>";
                        returnvalue += "<div id=\"validateDate\" class=\"hidden\">Please enter a valid date.</div>";
                        returnvalue += "<div class=\"txtlabel\">* Amount</div><div><input id=\"txtTotal\" disabled=\"disabled\" class=\"txtbox\" value=\"" + expensetotal.ToString("C2") + "\"/></div>";
                        returnvalue += "<div id=\"validateTotal\" class=\"hidden\">Please enter a valid date.</div>";

                        returnvalue += "</div><div class=\"col-md-4\">";

                        returnvalue += "<div class=\"txtlabel\">* Category</div><div>" + getCatsEdit(Convert.ToString(idr["Category"])) + "</div>";
                        returnvalue += "<div id=\"validateCategory\" class=\"hidden\">Please select a category.</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div><input type=\"text\" id=\"txtDescription\" class=\"txtbox\" value=\"" + Convert.ToString(idr["ExpenseDescription"]) + "\"/></div>";
                        returnvalue += "<div class=\"hidden\">* Report</div><div class=\"hidden\"><select id=\"ddReport\" class=\"txtbox\" style=\"width: 280px!important\"><option>" + Convert.ToString(idr["Report"]) + "</option></select></div>";
                        returnvalue += "<div id=\"validateReport\" class=\"hidden\">Please enter a valid date.</div>";
                        if (Convert.ToString(idr["reimbursable"]) == "False")
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" id=\"cbreimbursable\" />&nbsp;&nbsp;Reimbursable</div>";
                        }
                        else
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" id=\"cbreimbursable\" checked=\"checked\"/>&nbsp;&nbsp;Reimbursable</div>";
                        }

                        if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                        { }
                        else
                        {
                            //returnvalue += "<input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px; margin-top:15px;\" value=\"Submit\" onclick=\"validatesubdistance()\" />";
                            //returnvalue += "<input type=\"button\" class=\"btn btn-danger\" style=\"margin-bottom:10px; margin-top:15px; margin-left:30px\" value=\"Delete\" onclick=\"showdelexpense()\" />";
                            //returnvalue += "</div></div>";
                        }

                    }



                    
                }
            }
            con.Close();

            return returnvalue;
        }


            public async Task<string> getsupedit(string supeditid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from Expense where id = '"+ supeditid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";
            string bgcolor = "lightblue";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    DateTime expensedate = Convert.ToDateTime(idr["expensedate"]);
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    {
                        bgcolor = "lightgreen";
                    }

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Declined")
                    {
                        bgcolor = "lightcoral";
                    }
                    returnvalue += "<input type=\"text\" class=\"hidden\" value=\""+supeditid+ "\" id=\"UID\"><input type=\"text\" class=\"hidden\" value=\"" + Convert.ToString(idr["attachmentid"]) + "\" id=\"AUID\"><input type=\"text\" class=\"hidden\" value=\"" + Convert.ToString(idr["SubmitEmail"]) + "\" id=\"txtUser\">";
                    returnvalue += "<div style=\"background-color: "+bgcolor+"; width:100%; text-align:center; padding: 5px; border: 1px solid black; margin-bottom:10px; border-radius: 5px\"><b>"+ Convert.ToString(idr["ApprovalStatus"]) + "</b></div>";

                    returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div><b><i>Submitted By: " + Convert.ToString(idr["submitby"]) + "</i></b></div>";
                    returnvalue += "<div><b><i>Submitted: " + Convert.ToString(idr["SubmitDate"]) + "</i></b></div></div><div class=\"col-md-4\">";

                    if (Convert.ToString(idr["ApprovalDate"]) == "")
                    { }
                    else
                    {
                        returnvalue += "<div><b><i>Status Changed By: " + Convert.ToString(idr["ApprovedBy"]) + "</i></b></div>";
                        returnvalue += "<div><b><i>Status Changed: " + Convert.ToString(idr["ApprovalDate"]) + "</i></b></div>";
                    }

                    returnvalue += "</div></div>";



                    if (Convert.ToString(idr["ExpenseType"]) == "EXPENSE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">Facility</div><div class=\"txtbox\">" + Convert.ToString(idr["Facility"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Merchant</div><div class=\"txtbox\">" + Convert.ToString(idr["Merchant"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Date</div><div class=\"txtbox\">" + expensedate.ToShortDateString() + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Total</div><div class=\"txtbox\">" + expensetotal.ToString("C2") + "</div>";

                        if (Convert.ToString(idr["reimbursable"]) == "False")
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" />&nbsp;&nbsp;Reimbursable</div>";
                        } else
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\"/>&nbsp;&nbsp;Reimbursable</div>";
                        }

                        returnvalue += "<div class=\"txtlabel\">Category</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["Category"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Attendees</div><div class=\"txtbox\">" + Convert.ToString(idr["Attendees"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["ExpenseDescription"]) + "</div>";
                        returnvalue += "<div class=\"hidden\">Report</div><div class=\"hidden\">" + Convert.ToString(idr["Report"]) + "</div>";

                        returnvalue += "</div><div class=\"col-md-4\">";
                        returnvalue += "<div id=\"imagelist\">" + await GetImages(Convert.ToString(idr["AttachmentID"])) + "</div>";
                    }

                    if (Convert.ToString(idr["ExpenseType"]) == "MULTIPLE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">Facility</div><div class=\"txtbox\">" + Convert.ToString(idr["Facility"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Merchant</div><div class=\"txtbox\">" + Convert.ToString(idr["Merchant"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Date</div><div class=\"txtbox\">" + expensedate.ToShortDateString() + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Total</div><div class=\"txtbox\">" + expensetotal.ToString("C2") + "</div>";

                        returnvalue += "<div class=\"txtlabel\">Category</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["Category"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["ExpenseDescription"]) + "</div>";

                        returnvalue += "</div><div class=\"col-md-4\">";
                    }

                    if (Convert.ToString(idr["ExpenseType"]) == "DISTANCE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">Facility</div><div class=\"txtbox\">" + Convert.ToString(idr["Facility"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">From Address</div><div class=\"txtbox\">" + Convert.ToString(idr["FromAddress"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">To Address</div><div class=\"txtbox\">" + Convert.ToString(idr["ToAddress"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Distance</div><div class=\"txtbox\">" + Convert.ToString(idr["Distance"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Rate</div><div class=\"txtbox\">" + Convert.ToString(idr["Rate"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Date</div><div class=\"txtbox\">" + expensedate.ToShortDateString() + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Amount</div><div class=\"txtbox\">" + expensetotal.ToString("C2") + "</div>";





                        returnvalue += "</div><div class=\"col-md-4\">";
                        returnvalue += "<div class=\"txtlabel\">Category</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["Category"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["ExpenseDescription"]) + "</div>";
                        returnvalue += "<div class=\"hidden\">Report</div><div class=\"hidden\">" + Convert.ToString(idr["Report"]) + "</div>";
                        if (Convert.ToString(idr["reimbursable"]) == "False")
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" />&nbsp;&nbsp;Reimbursable</div>";
                        }
                        else
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\"/>&nbsp;&nbsp;Reimbursable</div>";
                        }

                    }

                    returnvalue += "<div id=\"imagelist\">" + await GetImages(Convert.ToString(idr["AttachmentID"])) + "</div>";
                    returnvalue += "<div class=\"txtlabel\">Status Change Notes</div>";
                    returnvalue += "<textarea class=\"txtbox\" style=\"height:100px\" id=\"txtNotes\">" + Convert.ToString(idr["ApproveNotes"]) + "</textarea>";
                    returnvalue += "<div class=\"txtlabel\">Status</div>";

                    //if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    //{
                    //    returnvalue += "<div class=\"txtbox\">Approved</div>";
                    //    returnvalue += "<input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px; margin-top:15px;\" value=\"Submit\" onclick=\"validatesub()\" />";
                    //}
                    //else
                    //{
                        returnvalue += "<select class=\"txtbox\" id=\"ddStatus\"><option>Approved</option><option>Declined</option></select>";
                        returnvalue += "<div><input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px; margin-top:15px;\" value=\"Submit\" onclick=\"validatesub()\" />";
                        returnvalue += "<input type=\"button\" class=\"btn btn-danger\" style=\"margin-bottom:10px; margin-top:15px; margin-left:30px\" value=\"Delete\" onclick=\"showdelexpense()\" /></div>";
                    //}

                    returnvalue += "</div></div>";
                }
            }
            con.Close();

            return returnvalue;
        }

        public async Task<string> getfinanceedit(string supeditid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from Expense where id = '" + supeditid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";
            string bgcolor = "lightblue";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    DateTime expensedate = Convert.ToDateTime(idr["expensedate"]);
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    {
                        bgcolor = "lightgreen";
                    }

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Declined")
                    {
                        bgcolor = "lightcoral";
                    }
                    returnvalue += "<input type=\"text\" class=\"hidden\" value=\"" + supeditid + "\" id=\"UID\"><input type=\"text\" class=\"hidden\" value=\"" + Convert.ToString(idr["attachmentid"]) + "\" id=\"AUID\"><input type=\"text\" class=\"hidden\" value=\"" + Convert.ToString(idr["SubmitEmail"]) + "\" id=\"txtUser\">";
                    returnvalue += "<div style=\"background-color: " + bgcolor + "; width:100%; text-align:center; padding: 5px; border: 1px solid black; margin-bottom:10px; border-radius: 5px\"><b>" + Convert.ToString(idr["ApprovalStatus"]) + "</b></div>";

                    returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div><b><i>Submitted By: " + Convert.ToString(idr["submitby"]) + "</i></b></div>";
                    returnvalue += "<div><b><i>Submitted: " + Convert.ToString(idr["SubmitDate"]) + "</i></b></div></div><div class=\"col-md-4\">";

                    if (Convert.ToString(idr["ApprovalDate"]) == "")
                    { }
                    else
                    {
                        returnvalue += "<div><b><i>Status Changed By: " + Convert.ToString(idr["ApprovedBy"]) + "</i></b></div>";
                        returnvalue += "<div><b><i>Status Changed: " + Convert.ToString(idr["ApprovalDate"]) + "</i></b></div>";
                    }

                    returnvalue += "</div></div>";



                    if (Convert.ToString(idr["ExpenseType"]) == "EXPENSE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">Facility</div><div class=\"txtbox\">" + Convert.ToString(idr["Facility"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Merchant</div><div class=\"txtbox\">" + Convert.ToString(idr["Merchant"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Date</div><div class=\"txtbox\">" + expensedate.ToShortDateString() + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Total</div><div class=\"txtbox\">" + expensetotal.ToString("C2") + "</div>";

                        if (Convert.ToString(idr["reimbursable"]) == "False")
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" />&nbsp;&nbsp;Reimbursable</div>";
                        }
                        else
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\"/>&nbsp;&nbsp;Reimbursable</div>";
                        }

                        returnvalue += "<div class=\"txtlabel\">Category</div><div>" + getCatsEdit(Convert.ToString(idr["Category"])) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Attendees</div><div class=\"txtbox\">" + Convert.ToString(idr["Attendees"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["ExpenseDescription"]) + "</div>";
                        returnvalue += "<div class=\"hidden\">Report</div><div class=\"hidden\">" + Convert.ToString(idr["Report"]) + "</div>";
                         
                        returnvalue += "</div><div class=\"col-md-4\">";
                        returnvalue += "<div id=\"imagelist\">" + await GetImages(Convert.ToString(idr["AttachmentID"])) + "</div>";
                    }

                    if (Convert.ToString(idr["ExpenseType"]) == "MULTIPLE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">Facility</div><div class=\"txtbox\">" + Convert.ToString(idr["Facility"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Merchant</div><div class=\"txtbox\">" + Convert.ToString(idr["Merchant"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Date</div><div class=\"txtbox\">" + expensedate.ToShortDateString() + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Total</div><div class=\"txtbox\">" + expensetotal.ToString("C2") + "</div>";

                        returnvalue += "<div class=\"txtlabel\">Category</div><div>" + getCatsEdit(Convert.ToString(idr["Category"])) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["ExpenseDescription"]) + "</div>";

                        returnvalue += "</div><div class=\"col-md-4\">";
                    }

                    if (Convert.ToString(idr["ExpenseType"]) == "DISTANCE")
                    {
                        returnvalue += "<div class=\"row\"><div class=\"col-md-4\"><div class=\"txtlabel\">Facility</div><div class=\"txtbox\">" + Convert.ToString(idr["Facility"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">From Address</div><div class=\"txtbox\">" + Convert.ToString(idr["FromAddress"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">To Address</div><div class=\"txtbox\">" + Convert.ToString(idr["ToAddress"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Distance</div><div class=\"txtbox\">" + Convert.ToString(idr["Distance"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Rate</div><div class=\"txtbox\">" + Convert.ToString(idr["Rate"]) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Date</div><div class=\"txtbox\">" + expensedate.ToShortDateString() + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Amount</div><div class=\"txtbox\">" + expensetotal.ToString("C2") + "</div>";





                        returnvalue += "</div><div class=\"col-md-4\">";
                        returnvalue += "<div class=\"txtlabel\">Category</div><div>" + getCatsEdit(Convert.ToString(idr["Category"])) + "</div>";
                        returnvalue += "<div class=\"txtlabel\">Description</div><div class=\"txtbox\" style=\"min-height:28px\">" + Convert.ToString(idr["ExpenseDescription"]) + "</div>";
                        returnvalue += "<div class=\"hidden\">Report</div><div class=\"hidden\">" + Convert.ToString(idr["Report"]) + "</div>";
                        if (Convert.ToString(idr["reimbursable"]) == "False")
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" />&nbsp;&nbsp;Reimbursable</div>";
                        }
                        else
                        {
                            returnvalue += "<div class=\"hidden\"><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\"/>&nbsp;&nbsp;Reimbursable</div>";
                        }

                        returnvalue += "<div id=\"imagelist\">" + await GetImages(Convert.ToString(idr["AttachmentID"])) + "</div>";

                    }
                    returnvalue += "<div class=\"txtlabel\">Status Change Notes</div>";
                    returnvalue += "<textarea class=\"txtbox\" disabled=\"disabled\" style=\"height:100px\" id=\"txtNotes\">" + Convert.ToString(idr["ApproveNotes"]) + "</textarea>";
                    returnvalue += "<div class=\"txtlabel\">Status</div>";

                        returnvalue += "<select class=\"txtbox\" id=\"ddStatus\"><option>Approved</option><option>Declined</option></select>";
                        returnvalue += "<div><input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px; margin-top:15px;\" value=\"Save\" onclick=\"validatesub()\" />";
                        returnvalue += "</div>";
                    

                    returnvalue += "</div></div>";
                }
            }
            con.Close();

            return returnvalue;
        }

        public string checkifsup()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select count(*) as 'totalsup', (select count(*) from Expense where ApprovedEmail = '" + User.FindFirst("preferred_username")?.Value + "' and ApprovalStatus = 'Pending Approval') as 'pendsup' from Expense where ApprovedEmail = '" + User.FindFirst("preferred_username")?.Value + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (Convert.ToInt32(idr["totalsup"]) > 0)
                    {
                        returnvalue = "<h4 style=\"color: #007bff\">You have <b>("+ Convert.ToString(idr["pendsup"]) + ")</b> expenses waiting for your approval</h4><a href=\"/Expense/Sup\" class=\"btn btn-primary\">Supervisor View</a>";
                    }
                    
                }
            }
            con.Close();

            return returnvalue;
        }

        public string getownexpense()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select id, ExpenseDate, ExpenseTotal, Category, ExpenseDescription, Facility, SubmitDate, isnull(ApprovalStatus, 'Pending Approval') as 'approvalstatus', isnull(completed, 'no') as 'completed' from Expense where SubmitEmail = '" + User.FindFirst("preferred_username")?.Value + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Date</th>";
            prepaidtable += "<th>Total</th>";
            prepaidtable += "<th>Category</th>";
            prepaidtable += "<th>Description</th>";
            prepaidtable += "<th>Facility</th>";
            prepaidtable += "<th>Submitted</th>";
            prepaidtable += "<th>Status</th>";
            prepaidtable += "<th>Completed</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    string statuscolor = "lightblue";
                    if (Convert.ToString(idr["approvalstatus"]) == "Declined")
                    {
                        statuscolor = "lightcoral";
                    }
                    if (Convert.ToString(idr["approvalstatus"]) == "Approved")
                    {
                        statuscolor = "lightgreen";
                    }


                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    DateTime expensedate = Convert.ToDateTime(idr["expensedate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);
                    prepaidtable += "<td>" + expensetotal.ToString("C2") + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["category"]), 25) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["expensedescription"]), 25) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["facility"]) + "</td>";
                    DateTime subdate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + subdate.ToShortDateString() + "</td>";
                    prepaidtable += "<td style=\"background-color:"+statuscolor+" !important\">" + Convert.ToString(idr["approvalstatus"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["completed"]) + "</td>";
                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            prepaidtable += "</tbody></table>";

            return prepaidtable;
        }

        public string getsupexpense()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select id,submitby, ExpenseDate, ExpenseTotal, Category, ExpenseDescription, Facility, SubmitDate, isnull(ApprovalStatus, 'Pending Approval') as 'approvalstatus', isnull(completed, 'no') as 'completed' from Expense where ApprovedEmail = '" + User.FindFirst("preferred_username")?.Value + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Submitted By</th>";
            prepaidtable += "<th>Date</th>";
            prepaidtable += "<th>Total</th>";
            prepaidtable += "<th>Category</th>";
            prepaidtable += "<th>Description</th>";
            prepaidtable += "<th>Facility</th>";
            prepaidtable += "<th>Submitted</th>";
            prepaidtable += "<th>Status</th>";
            prepaidtable += "<th>Completed</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    string statuscolor = "lightblue";
                    if (Convert.ToString(idr["approvalstatus"]) == "Declined")
                    {
                        statuscolor = "lightcoral";
                    }
                    if (Convert.ToString(idr["approvalstatus"]) == "Approved")
                    {
                        statuscolor = "lightgreen";
                    }


                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["submitby"]) + "</td>";
                    DateTime expensedate = Convert.ToDateTime(idr["expensedate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);
                    prepaidtable += "<td>" + expensetotal.ToString("C2") + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["category"]), 25) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["expensedescription"]), 25) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["facility"]) + "</td>";
                    DateTime subdate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + subdate.ToShortDateString() + "</td>";
                    prepaidtable += "<td style=\"background-color:" + statuscolor + " !important\">" + Convert.ToString(idr["approvalstatus"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["completed"]) + "</td>";
                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            prepaidtable += "</tbody></table>";

            return prepaidtable;
        }

        public async Task<string> getsupexpensegroup()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select distinct submitemail, submitby from Expense where ApprovalStatus = 'Pending Approval' and ApprovedEmail = '" + User.FindFirst("preferred_username")?.Value + "' Order by SubmitBy";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    prepaidtable += "<button class=\"collapsible\">&nbsp;&nbsp;&nbsp;&nbsp;"+ Convert.ToString(idr["submitby"]) + "</button>";
                    prepaidtable += "<div class=\"content\">";

                    prepaidtable += await getsupexpensegroupexp(Convert.ToString(idr["submitemail"]));
                    prepaidtable += await getsupexpensegroupdist(Convert.ToString(idr["submitemail"]));

                    prepaidtable += "</div>";
                }
            }
            con.Close();

            

            return prepaidtable;
        }

        public async Task<string> getsupexpensegroupexp(string passname)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from Expense where ApprovalStatus = 'Pending Approval' and ApprovedEmail = '" + User.FindFirst("preferred_username")?.Value + "' and SubmitEmail = '"+passname+"' and ExpenseType <> 'distance' order by SubmitDate";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "";

            if (idr.HasRows)
            {
                prepaidtable += "<h4>Expenses</h4>";
                prepaidtable += "<table>";
                prepaidtable += "<thead>";
                prepaidtable += "<tr>";
                prepaidtable += "<th>Date</th>";
                prepaidtable += "<th>Total</th>";
                prepaidtable += "<th>Merchant</th>";
                prepaidtable += "<th>Category</th>";
                prepaidtable += "<th>Description</th>";
                prepaidtable += "<th>Facility</th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "</tr>";
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";

                while (idr.Read())
                {
                    prepaidtable += "<tr id=\""+ Convert.ToString(idr["ID"]) + "\">";
                    DateTime expensedate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);
                    prepaidtable += "<td>" + expensetotal.ToString("C2") + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["merchant"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["category"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["expensedescription"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 20) + "</td>";
                    prepaidtable += "<td>";

                    if (Convert.ToString(idr["attachmentid"]) != "")
                    {
                        prepaidtable += await GetImagessmall(Convert.ToString(idr["attachmentid"]));
                    }

                    prepaidtable += "</td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Quick Approve\" class=\"btn btn-primary\" onclick=\"quickapprove('"+ Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Go To Expense\" class=\"btn btn-primary\" onclick=\"goto('" + Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            if (prepaidtable != "")
            {
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";
                prepaidtable += "</table>";
                prepaidtable += "<div style=\"height:25px\"></div>";
            }


            return prepaidtable;
        }

        public async Task<string> getsupexpensegroupdist(string passname)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from Expense where ApprovalStatus = 'Pending Approval' and ApprovedEmail = '" + User.FindFirst("preferred_username")?.Value + "' and SubmitEmail = '" + passname + "' and ExpenseType = 'distance' order by SubmitDate";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "";

            if (idr.HasRows)
            {
                prepaidtable += "<h4>Mileage</h4>";
                prepaidtable += "<table>";
                prepaidtable += "<thead>";
                prepaidtable += "<tr>";
                prepaidtable += "<th>Date</th>";
                prepaidtable += "<th>Total</th>";
                prepaidtable += "<th>Distance</th>";
                prepaidtable += "<th>Category</th>";
                prepaidtable += "<th>Description</th>";
                prepaidtable += "<th>Facility</th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "</tr>";
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";

                while (idr.Read())
                {
                    prepaidtable += "<tr id=\"" + Convert.ToString(idr["ID"]) + "\">";
                    DateTime expensedate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);
                    prepaidtable += "<td>" + expensetotal.ToString("C2") + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["distance"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["category"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["expensedescription"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 20) + "</td>";
                    prepaidtable += "<td>";

                    if (Convert.ToString(idr["attachmentid"]) != "")
                    {
                        prepaidtable += await GetImagessmall(Convert.ToString(idr["attachmentid"]));
                    }

                    prepaidtable += "</td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Quick Approve\" class=\"btn btn-primary\" onclick=\"quickapprove('" + Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Go To Expense\" class=\"btn btn-primary\" onclick=\"goto('" + Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            if (prepaidtable != "")
            {
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";
                prepaidtable += "</table>";
                prepaidtable += "<div style=\"height:25px\"></div>";
            }


            return prepaidtable;
        }

        public string QuickApprove(string stritem)
        {
            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_Expense_SupervisorEdit", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@ApprovalStatus", SqlDbType.VarChar).Value = "Approved";
            cmd.Parameters.Add("@ApproveNotes", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@completed", SqlDbType.VarChar).Value = "yes";
            cmd.Parameters.Add("@UID", SqlDbType.VarChar).Value = stritem;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }



        public async Task<string> getfinexpensegroup()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select distinct submitemail, submitby from Expense where FinanceApproved = 0 Order by SubmitBy";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    prepaidtable += "<button class=\"collapsible\">&nbsp;&nbsp;&nbsp;&nbsp;" + Convert.ToString(idr["submitby"]) + "</button>";
                    prepaidtable += "<div class=\"content\">";

                    prepaidtable += await getfinexpensegroupexp(Convert.ToString(idr["submitemail"]));
                    prepaidtable += await getfinexpensegroupdist(Convert.ToString(idr["submitemail"]));

                    prepaidtable += "</div>";
                }
            }
            con.Close();



            return prepaidtable;
        }

        public async Task<string> getfinexpensegroupexp(string passname)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from Expense where FinanceApproved = 0 and SubmitEmail = '" + passname + "' and ExpenseType <> 'distance' order by SubmitDate";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "";

            if (idr.HasRows)
            {
                prepaidtable += "<h4>Expenses</h4>";
                prepaidtable += "<table>";
                prepaidtable += "<thead>";
                prepaidtable += "<tr>";
                prepaidtable += "<th>Date</th>";
                prepaidtable += "<th>Total</th>";
                prepaidtable += "<th>Merchant</th>";
                prepaidtable += "<th>Category</th>";
                prepaidtable += "<th>Description</th>";
                prepaidtable += "<th>Facility</th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "</tr>";
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";

                while (idr.Read())
                {
                    prepaidtable += "<tr id=\"" + Convert.ToString(idr["ID"]) + "\">";
                    DateTime expensedate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);
                    prepaidtable += "<td>" + expensetotal.ToString("C2") + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["merchant"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["category"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["expensedescription"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 20) + "</td>";
                    prepaidtable += "<td>";

                    if (Convert.ToString(idr["attachmentid"]) != "")
                    {
                        prepaidtable += await GetImagessmall(Convert.ToString(idr["attachmentid"]));
                    }

                    prepaidtable += "</td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Quick Approve\" class=\"btn btn-primary\" onclick=\"quickapprove('" + Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Go To Expense\" class=\"btn btn-primary\" onclick=\"goto('" + Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            if (prepaidtable != "")
            {
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";
                prepaidtable += "</table>";
                prepaidtable += "<div style=\"height:25px\"></div>";
            }


            return prepaidtable;
        }

        public async Task<string> getfinexpensegroupdist(string passname)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from Expense where FinanceApproved = 0 and SubmitEmail = '" + passname + "' and ExpenseType = 'distance' order by SubmitDate";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "";

            if (idr.HasRows)
            {
                prepaidtable += "<h4>Mileage</h4>";
                prepaidtable += "<table>";
                prepaidtable += "<thead>";
                prepaidtable += "<tr>";
                prepaidtable += "<th>Date</th>";
                prepaidtable += "<th>Total</th>";
                prepaidtable += "<th>Distance</th>";
                prepaidtable += "<th>Category</th>";
                prepaidtable += "<th>Description</th>";
                prepaidtable += "<th>Facility</th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "<th></th>";
                prepaidtable += "</tr>";
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";

                while (idr.Read())
                {
                    prepaidtable += "<tr id=\"" + Convert.ToString(idr["ID"]) + "\">";
                    DateTime expensedate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);
                    prepaidtable += "<td>" + expensetotal.ToString("C2") + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["distance"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["category"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["expensedescription"]), 20) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 20) + "</td>";
                    prepaidtable += "<td>";

                    if (Convert.ToString(idr["attachmentid"]) != "")
                    {
                        prepaidtable += await GetImagessmall(Convert.ToString(idr["attachmentid"]));
                    }

                    prepaidtable += "</td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Quick Approve\" class=\"btn btn-primary\" onclick=\"quickapprove('" + Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "<td><input type=\"button\" value=\"Go To Expense\" class=\"btn btn-primary\" onclick=\"goto('" + Convert.ToString(idr["id"]) + "')\" /></td>";
                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            if (prepaidtable != "")
            {
                prepaidtable += "</thead>";
                prepaidtable += "<tbody>";
                prepaidtable += "</table>";
                prepaidtable += "<div style=\"height:25px\"></div>";
            }


            return prepaidtable;
        }

        public string QuickApprovefin(string stritem)
        {
            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_Expense_FinanceEditQuick", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FinanceBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@FinanceEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@FinanceApproved", SqlDbType.Bit).Value = "True";
            cmd.Parameters.Add("@UID", SqlDbType.VarChar).Value = stritem;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }


        public string getfinance()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select id, ExpenseDate, ExpenseTotal, Category, ExpenseDescription, Facility, SubmitDate, financeapproved from Expense where approvalstatus = 'approved'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Date</th>";
            prepaidtable += "<th>Total</th>";
            prepaidtable += "<th>Category</th>";
            prepaidtable += "<th>Description</th>";
            prepaidtable += "<th>Facility</th>";
            prepaidtable += "<th>Submitted</th>";
            prepaidtable += "<th>Checked</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    DateTime expensedate = Convert.ToDateTime(idr["expensedate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    decimal expensetotal = Convert.ToDecimal(idr["expensetotal"]);
                    prepaidtable += "<td>" + expensetotal.ToString("C2") + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["category"]), 25) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["expensedescription"]), 25) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["facility"]) + "</td>";
                    DateTime subdate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + subdate.ToShortDateString() + "</td>";
                    
                    string glcheck = "No";
                    string glbg = "lightcoral";
                    if (Convert.ToString(idr["FinanceApproved"]) == "True")
                    {
                        glcheck = "Yes";
                        glbg = "lightgreen";
                    }
                    prepaidtable += "<td style=\"background-color:"+glbg+" !important\">" + glcheck + "</td>";

                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            prepaidtable += "</tbody></table>";

            return prepaidtable;
        }

        public string trimstrings(string passvalue, int length)
        {
            if (passvalue.Length > length)
            {
                length = passvalue.Length - length;
                passvalue = passvalue.Substring(0, passvalue.Length - length);

                passvalue += " ...";
            }

            return passvalue;
        }

        public string getCats()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from ExpenseCategory", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue += "<option>" + Convert.ToString(idr["Category"]) + "</option>";
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string getCatsEdit(string passcat)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from ExpenseCategory", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<select id=\"ddCat\" style=\"width: 280px!important\" class=\"txtbox\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (Convert.ToString(idr["Category"]) == passcat)
                    {
                        returnvalue += "<option selected=\"selected\">" + Convert.ToString(idr["Category"]) + "</option>";
                    } else
                    {
                        returnvalue += "<option>" + Convert.ToString(idr["Category"]) + "</option>";
                    }
                    
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string getFacilities()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select operationname from operations union select 'Headquarters' order by operationName", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<option></option>";
            string userfacility = getUserFacilities();

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    string opname = Convert.ToString(idr["operationName"]);

                    if (opname.Trim() == userfacility)
                    {
                        returnvalue += "<option selected=\"selected\">" + Convert.ToString(idr["operationName"]) + "</option>";
                    } else
                    {
                        returnvalue += "<option>" + Convert.ToString(idr["operationName"]) + "</option>";
                    }
                    
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string getFacilitiesEdit(string facility)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select operationname from operations union select 'Headquarters' order by operationName", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<select id=\"ddFac\" class=\"txtbox\" style=\"width: 280px!important\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    string opname = Convert.ToString(idr["operationName"]);

                    if (opname.Trim() == facility)
                    {
                        returnvalue += "<option selected=\"selected\">" + Convert.ToString(idr["operationName"]) + "</option>";
                    }
                    else
                    {
                        returnvalue += "<option>" + Convert.ToString(idr["operationName"]) + "</option>";
                    }

                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string getUserFacilities()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select facility from ExpenseFacilities where UserAccount = '"+ User.FindFirst("preferred_username")?.Value + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "ZZZZZZZZZZZZZZZZZZZZZZZ";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue = Convert.ToString(idr["facility"]);
                }
            }
            con.Close();

            return returnvalue;
        }

        public string getGUID()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("SELECT NEWID()", con);
            con.Open();
            string UID = cmd.ExecuteScalar().ToString();
            con.Close();
            return UID;
        }


        public string GetSup(string stritem)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from ExpenseSupervisors where UserAccount = '" + User.FindFirst("preferred_username")?.Value + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "stop";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    returnvalue = "Report will be submitted to <b>" + Convert.ToString(idr["SupervisorUser"]) + "</b>.<br />Click <a href=\"\" onclick=\"showsup();return false;\">HERE</a> to change supervisors.$<input type=\"text\"  name=\"supemail\"  value=\"" + Convert.ToString(idr["SupervisorAccount"]) + "\"/><input type=\"text\" name=\"supname\"  value=\"" + Convert.ToString(idr["SupervisorUser"]) + "\"/>";
                }
            }
            con.Close();
            return returnvalue;

        
        }

        public async Task<string> getgiven(string search)
        {
            string response = await GraphService.GetAllUsersGiven(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, search);
            response = response.Replace(System.Environment.NewLine, "");
            response = response.Replace("[  {    \"givenName\": \"", "");
            string[] splitresponse = response.Split("\"");
            return splitresponse[0];
        }

        public async Task<string> getsur(string search)
        {
            string response = await GraphService.GetAllUsersSur(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, search);
            response = response.Replace(System.Environment.NewLine, "");
            response = response.Replace("[  {    \"surname\": \"", "");
            string[] splitresponse = response.Split("\"");
            return splitresponse[0];
        }

        public async Task<string> getusers(string supsearch)
        {

            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            
            string response = await GraphService.GetAllUsers(graphClient, email, HttpContext, supsearch);
            response = response.Replace(System.Environment.NewLine, "");
            do
            {
                response = response.Replace("  ", " ");
            } while (response.Contains("  "));
            response = response.Replace("[ { " , "");
            response = response.Replace(" }]", "");
            response = response.Replace(": ", ":");
            response = response.Replace(", ", ",");
            response = response.Replace("\"displayName\":", "");
            response = response.Replace("\"mail\":", "");
            response = response.Replace(",\"@odata.type\":\"microsoft.graph.user\"", "");
            response = response.Replace("\",\"", "$$$");
            response = response.Replace("\"", "");
            string[] ireponse = response.Split(" },{ ");
            string splitresponse = "<table class=\"fixed_header\">";

            splitresponse += "<tbody>";

            foreach (var item in ireponse)
            {
                splitresponse += "<tr onclick=\"changesup('" + item+"', this)\">";
                string[] splitagain = item.Split("$$$");
                foreach(var newsplit in splitagain)
                {
                    splitresponse += "<td>" + newsplit + "</td>";
                }
                splitresponse += "</tr>";
            }

            splitresponse += "</tbody></table>";
            
            return splitresponse;
        }


        public async Task<string> GetImages(string stritem)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=pgcorestorage;AccountKey="+ _configuration.GetConnectionString("blobkey") + ";EndpointSuffix=core.windows.net";
            string picturelist = "";

            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Get the container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(stritem);

                // List all blobs in the container
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    if (blobItem.Name.Contains("Thumb$$$$"))
                    {
                        string actualname = blobItem.Name.Replace("Thumb$$$$", "");
                        actualname = actualname.Substring(0, actualname.Length - 4);
                        picturelist += "<div class=\"imgdiv\"><table><tr><td rowspan=\"2\">";
                        picturelist += "<img src=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + blobItem.Name + "\" style=\"border-radius:5px\" />";
                        picturelist += "</td><td colspan=\"2\" class=\"imgdivtd\">" + actualname + "</td></tr><tr>";
                        picturelist += "<td><a href=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + actualname + "\">Download</a></td>";
                        picturelist += "<td><a href=\"\" onclick=\"delblob('" + blobItem.Name + "');return false;\">Remove</a></td></tr></table></div>";
                    }


                }
            } catch
            {
                picturelist = "";
            }


            return picturelist;
        }

        public async Task<string> GetImagessmall(string stritem)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=pgcorestorage;AccountKey=" + _configuration.GetConnectionString("blobkey") + ";EndpointSuffix=core.windows.net";
            string picturelist = "";

            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Get the container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(stritem);

                // List all blobs in the container
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    if (blobItem.Name.Contains("Thumb$$$$"))
                    {
                        string actualname = blobItem.Name.Replace("Thumb$$$$", "");
                        actualname = actualname.Substring(0, actualname.Length - 4);
                        picturelist += "<a href=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + actualname + "\"><img src=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + blobItem.Name + "\" style=\"border-radius:5px\" /></a>";
                    }


                }
            }
            catch
            {
                picturelist = "";
            }


            return picturelist;
        }

        public async Task<string> DelBlob(string stritem, string strblob)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=pgcorestorage;AccountKey=" + _configuration.GetConnectionString("blobkey") + ";EndpointSuffix=core.windows.net";
            string containerName = stritem;
            string blobNameThumb = strblob;
            string actualname = strblob.Replace("Thumb$$$$", "");
            actualname = actualname.Substring(0, actualname.Length - 4);

            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            // Get a reference to a blob named "sample-file" in a container named "sample-container"
            BlobClient blobThumb = container.GetBlobClient(blobNameThumb);
            BlobClient blob = container.GetBlobClient(actualname);

            await blobThumb.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
            await blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);

            string returnitem = await GetImages(stritem);

            return returnitem.ToString();
        }



        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string FUID)
        {


            string connectionString = "DefaultEndpointsProtocol=https;AccountName=pgcorestorage;AccountKey=" + _configuration.GetConnectionString("blobkey") + ";EndpointSuffix=core.windows.net";
            try
            {
                using (var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(100, 100),
                        Mode = ResizeMode.Crop
                    })
                    );

                    using (var ms = new MemoryStream())
                    {
                        string containerName = FUID;
                        string blobNameThumb = "Thumb" + "$$$$" + file.FileName + ".png";
                        // Get a reference to a container named "sample-container" and then create it
                        BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                        container.CreateIfNotExists();
                        container.SetAccessPolicy(PublicAccessType.BlobContainer);

                        // Get a reference to a blob named "sample-file" in a container named "sample-container"
                        BlobClient blobThumb = container.GetBlobClient(blobNameThumb);

                        image.SaveAsPng(ms);
                        ms.Position = 0;
                        await blobThumb.UploadAsync(ms);

                    }
                }
            }
            catch
            {
                var imgpath = Path.Combine(_env.ContentRootPath, "wwwroot/images/Filetype-Docs-icon.png");
                using (var image = SixLabors.ImageSharp.Image.Load(imgpath))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(100, 100),
                        Mode = ResizeMode.Crop
                    })
                    );

                    using (var ms = new MemoryStream())
                    {
                        string containerName = FUID;
                        string blobNameThumb = "Thumb" + "$$$$" + file.FileName + ".png";
                        // Get a reference to a container named "sample-container" and then create it
                        BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                        container.CreateIfNotExists();
                        container.SetAccessPolicy(PublicAccessType.BlobContainer);

                        // Get a reference to a blob named "sample-file" in a container named "sample-container"
                        BlobClient blobThumb = container.GetBlobClient(blobNameThumb);

                        image.SaveAsPng(ms);
                        ms.Position = 0;
                        await blobThumb.UploadAsync(ms);

                    }
                }
            }




            if (file.Length > 0)
                {
                using (var stream = file.OpenReadStream())
                {


                    string containerName = FUID;
                    string blobName = file.FileName;
                    // Get a reference to a container named "sample-container" and then create it
                    BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                    //container.CreateIfNotExists();
                    //container.SetAccessPolicy(PublicAccessType.BlobContainer);

                    // Get a reference to a blob named "sample-file" in a container named "sample-container"
                    BlobClient blob = container.GetBlobClient(blobName);

                    // Upload local file

                    await blob.UploadAsync(stream);


                    

                }

            }


            return View("New");

        }


        public string savesup(string supinfo)
        {
            string[] sepsupinfo = supinfo.Split("$$$");

            supinfo = "Report will be submitted to <b>" + sepsupinfo[0] + "</b>.<br />Click <a href=\"\" onclick=\"showsup();return false;\">HERE</a> to change supervisors.<input type=\"text\" class=\"hidden\" id=\"supemail\" name=\"supemail\" value=\"" + sepsupinfo[1] + "\"/><input type=\"text\" id=\"supname\" class=\"hidden\" name=\"supname\"  value=\"" + sepsupinfo[0] + "\"/>";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_ExpenseSupervisor_Add", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@USERACCOUNT", SqlDbType.VarChar).Value = User.FindFirst("preferred_username")?.Value;
            cmd.Parameters.Add("@SUPACCOUNT", SqlDbType.VarChar).Value = sepsupinfo[1];
            cmd.Parameters.Add("@SUPUSER", SqlDbType.VarChar).Value = sepsupinfo[0];

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return supinfo;
        }

        public string getmonth()
        {
            int intmonth = DateTime.Now.Month;

            var ddmonth = "<select id=\"ddmonth\">";

            if (intmonth == 1)
            { ddmonth += "<option selected=\"selected\">1</option>"; }
            else
            { ddmonth += "<option>1</option>"; }

            if (intmonth == 2)
            { ddmonth += "<option selected=\"selected\">2</option>"; }
            else
            { ddmonth += "<option>2</option>"; }

            if (intmonth == 3)
            { ddmonth += "<option selected=\"selected\">3</option>"; }
            else
            { ddmonth += "<option>3</option>"; }

            if (intmonth == 4)
            { ddmonth += "<option selected=\"selected\">4</option>"; }
            else
            { ddmonth += "<option>4</option>"; }

            if (intmonth == 5)
            { ddmonth += "<option selected=\"selected\">5</option>"; }
            else
            { ddmonth += "<option>5</option>"; }

            if (intmonth == 6)
            { ddmonth += "<option selected=\"selected\">6</option>"; }
            else
            { ddmonth += "<option>6</option>"; }

            if (intmonth == 7)
            { ddmonth += "<option selected=\"selected\">7</option>"; }
            else
            { ddmonth += "<option>7</option>"; }

            if (intmonth == 8)
            { ddmonth += "<option selected=\"selected\">8</option>"; }
            else
            { ddmonth += "<option>8</option>"; }

            if (intmonth == 9)
            { ddmonth += "<option selected=\"selected\">9</option>"; }
            else
            { ddmonth += "<option>9</option>"; }

            if (intmonth == 10)
            { ddmonth += "<option selected=\"selected\">10</option>"; }
            else
            { ddmonth += "<option>10</option>"; }

            if (intmonth == 11)
            { ddmonth += "<option selected=\"selected\">11</option>"; }
            else
            { ddmonth += "<option>11</option>"; }

            if (intmonth == 12)
            { ddmonth += "<option selected=\"selected\">12</option>"; }
            else
            { ddmonth += "<option>12</option>"; }

            ddmonth += "</select>";
            return ddmonth;
        }

        public string getyear()
        {
            int intyear = DateTime.Now.Year;
            var ddyear = "<select id=\"ddyear\">";
            ddyear += "<option>" + Convert.ToString(intyear - 4) + "</option>";
            ddyear += "<option>" + Convert.ToString(intyear - 3) + "</option>";
            ddyear += "<option>" + Convert.ToString(intyear - 2) + "</option>";
            ddyear += "<option>" + Convert.ToString(intyear - 1) + "</option>";
            ddyear += "<option selected=\"selected\">" + Convert.ToString(intyear) + "</option>";


            ddyear += "</select>";
            return ddyear;
        }

        [HttpPost]
        public async Task<IActionResult> PostExpense(
            string txtMerchant, string txtDate, string txtNewTotal, string txtReimbursalbe, 
            string txtCategory, string txtAttendees, string txtDescription, string txtReport, 
            string UID, string supemail, string supname, string txtFac)
        {
            try
            {
                if (txtDescription is null)
                {
                    txtDescription = "";
                }
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_Expense_AddExpense", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = txtMerchant;
                cmd.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDate;
                cmd.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = txtNewTotal;
                cmd.Parameters.Add("@reimbursable", SqlDbType.Bit).Value = txtReimbursalbe;
                cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtCategory;
                cmd.Parameters.Add("@Attendees", SqlDbType.VarChar).Value = txtAttendees;
                cmd.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDescription;
                cmd.Parameters.Add("@Report", SqlDbType.VarChar).Value = txtReport;
                cmd.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                cmd.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                cmd.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtFac;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "Expense Report Submitted";
                string body = "A expense report was just submitted.<br/><br/>You can view the report <a href=\"https://pacs-technology.com/Expense\">HERE</a>.";

                // Send the email.
                await GraphService.SendEmail(graphClient, _env, supemail, HttpContext, subject, body);

                
                return RedirectToAction("Index" , new { strSave = "Success! Your record was saved." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }

        [HttpPost]
        public IActionResult PostExpenseEdit(
            string strMerchant, string strDate, string strTotal, string strReimbursalbe,
            string strCategory, string strAttendees, string strDescription, string strReport,
            string strUID, string strFacility)
        {
            try
            {
                if (strDescription is null)
                {
                    strDescription = "";
                }
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_Expense_EditExpense", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = strMerchant;
                cmd.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = strDate;
                cmd.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = strTotal;
                cmd.Parameters.Add("@reimbursable", SqlDbType.Bit).Value = strReimbursalbe;
                cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = strCategory;
                cmd.Parameters.Add("@Attendees", SqlDbType.VarChar).Value = strAttendees;
                cmd.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = strDescription;
                cmd.Parameters.Add("@Report", SqlDbType.VarChar).Value = strReport;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = strUID;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strFacility;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index", new { strSave = "Success! Your record was edited." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }


        [HttpPost]
        public async Task<IActionResult> PostDistance(
            string txtDistance, string txtDDate, decimal txtRate, string txtDreimbursalbe, 
            string txtNewAmount, string txtDCategory, string txtDDescription, string txtDReport, 
            string dUID, string supemail, string supname, string txtDFac, string txtToAddress, 
            string txtFromAddress)
        {

            txtDistance = txtDistance.Replace(",", "");

            try
            {
                if (txtDDescription is null)
                {
                    txtDDescription = "";
                }

                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_Expense_AddDistance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@Distance", SqlDbType.Int).Value = txtDistance;
                cmd.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDDate;
                cmd.Parameters.Add("@Rate", SqlDbType.Money).Value = txtRate;
                cmd.Parameters.Add("@reimbursable", SqlDbType.Bit).Value = txtDreimbursalbe;
                cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtDCategory;
                cmd.Parameters.Add("@Amount", SqlDbType.Money).Value = txtNewAmount;
                cmd.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDDescription;
                cmd.Parameters.Add("@Report", SqlDbType.VarChar).Value = txtDReport;
                cmd.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                cmd.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtDFac;
                cmd.Parameters.Add("@ToAddress", SqlDbType.VarChar).Value = txtToAddress;
                cmd.Parameters.Add("@FromAddress", SqlDbType.VarChar).Value = txtFromAddress;
                cmd.Parameters.Add("@AtachmentID", SqlDbType.VarChar).Value = dUID;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "Expense Report Submitted";
                string body = "A expense report was just submitted.<br/><br/>You can view the report <a href=\"https://pacs-technology.com/Expense\">HERE</a>.";

                // Send the email.
                await GraphService.SendEmail(graphClient, _env, supemail, HttpContext, subject, body);

                return RedirectToAction("Index", new { strSave = "Success! Your record was saved." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }

        [HttpPost]
        public IActionResult PostDistanceEdit(
    string strDistUID, string strDistFacility, string strDistFromAddress, string strDistToAddress,
    string strDistDistance, string strDistRate, string strDistDate, string strDistTotal,
    string strDistReimbursable, string strDistCategory, string strDistDescription, string strDistReport)
        {

            strDistDistance = strDistDistance.Replace(",", "");

            try
            {
                if (strDistDescription is null)
                {
                    strDistDescription = "";
                }

                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_Expense_EditDistance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = strDistUID;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strDistFacility;
                cmd.Parameters.Add("@FromAddress", SqlDbType.VarChar).Value = strDistFromAddress;
                cmd.Parameters.Add("@ToAddress", SqlDbType.VarChar).Value = strDistToAddress;
                cmd.Parameters.Add("@Distance", SqlDbType.Int).Value = strDistDistance;
                cmd.Parameters.Add("@Rate", SqlDbType.Money).Value = strDistRate;
                cmd.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = strDistDate;
                cmd.Parameters.Add("@Total", SqlDbType.Money).Value = strDistTotal;
                cmd.Parameters.Add("@reimbursable", SqlDbType.Bit).Value = strDistReimbursable;
                cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = strDistCategory; 
                cmd.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = strDistDescription;
                cmd.Parameters.Add("@Report", SqlDbType.VarChar).Value = strDistReport;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index", new { strSave = "Success! Your record was edited." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> PostMulti(
            string txtDate1, string txtMerch1, string txtmtotal1, string txtDesc1, string txtmcat1,
            string txtDate2, string txtMerch2, string txtmtotal2, string txtDesc2, string txtmcat2,
            string txtDate3, string txtMerch3, string txtmtotal3, string txtDesc3, string txtmcat3,
            string txtDate4, string txtMerch4, string txtmtotal4, string txtDesc4, string txtmcat4,
            string txtDate5, string txtMerch5, string txtmtotal5, string txtDesc5, string txtmcat5,
            string txtDate6, string txtMerch6, string txtmtotal6, string txtDesc6, string txtmcat6,
            string supemail, string supname, string UID, string txtMFac
            )
        {

            try
            {
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;
                var connection = _configuration.GetConnectionString("pgWebForm");

                int check1 = 0;
                int check2 = 0;
                int check3 = 0;
                int check4 = 0;
                int check5 = 0;
                int check6 = 0;


                if (!(txtDate1 is null)) { if (txtDate1 != "") { check1 = 1; } }
                if (!(txtDate2 is null)) { if (txtDate2 != "") { check2 = 1; } }
                if (!(txtDate3 is null)) { if (txtDate3 != "") { check3 = 1; } }
                if (!(txtDate4 is null)) { if (txtDate4 != "") { check4 = 1; } }
                if (!(txtDate5 is null)) { if (txtDate5 != "") { check5 = 1; } }
                if (!(txtDate6 is null)) { if (txtDate6 != "") { check6 = 1; } }

                if (!(txtMerch1 is null)) { if (txtMerch1 != "") { check1 = 1; } }
                if (!(txtMerch2 is null)) { if (txtMerch2 != "") { check2 = 1; } }
                if (!(txtMerch3 is null)) { if (txtMerch3 != "") { check3 = 1; } }
                if (!(txtMerch4 is null)) { if (txtMerch4 != "") { check4 = 1; } }
                if (!(txtMerch5 is null)) { if (txtMerch5 != "") { check5 = 1; } }
                if (!(txtMerch6 is null)) { if (txtMerch6 != "") { check6 = 1; } }

                if (!(txtmtotal1 is null)) { if (txtmtotal1 != "") { check1 = 1; } }
                if (!(txtmtotal2 is null)) { if (txtmtotal2 != "") { check2 = 1; } }
                if (!(txtmtotal3 is null)) { if (txtmtotal3 != "") { check3 = 1; } }
                if (!(txtmtotal4 is null)) { if (txtmtotal4 != "") { check4 = 1; } }
                if (!(txtmtotal5 is null)) { if (txtmtotal5 != "") { check5 = 1; } }
                if (!(txtmtotal6 is null)) { if (txtmtotal6 != "") { check6 = 1; } }

                if (!(txtmcat1 is null)) { if (txtmcat1 != "") { check1 = 1; } }
                if (!(txtmcat2 is null)) { if (txtmcat2 != "") { check2 = 1; } }
                if (!(txtmcat3 is null)) { if (txtmcat3 != "") { check3 = 1; } }
                if (!(txtmcat4 is null)) { if (txtmcat4 != "") { check4 = 1; } }
                if (!(txtmcat5 is null)) { if (txtmcat5 != "") { check5 = 1; } }
                if (!(txtmcat6 is null)) { if (txtmcat6 != "") { check6 = 1; } }

                if (check1 == 1)
                {
                    if (txtDesc1 is null)
                    {
                        txtDesc1 = "";
                    }

                    SqlConnection con1 = new SqlConnection(connection);
                    SqlCommand cmd1 = new SqlCommand();
                    cmd1 = new SqlCommand("sp_Expense_AddMultiple", con1);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                    cmd1.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                    cmd1.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = txtMerch1;
                    cmd1.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDate1;
                    cmd1.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = txtmtotal1;
                    cmd1.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtmcat1;
                    cmd1.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDesc1;
                    cmd1.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                    cmd1.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                    cmd1.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                    cmd1.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtMFac;

                    con1.Open();
                    cmd1.ExecuteNonQuery();
                    con1.Close();
                }

                if (check2 == 1)
                {
                    if (txtDesc2 is null)
                    {
                        txtDesc2 = "";
                    }

                    SqlConnection con2 = new SqlConnection(connection);
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2 = new SqlCommand("sp_Expense_AddMultiple", con2);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                    cmd2.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                    cmd2.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = txtMerch2;
                    cmd2.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDate2;
                    cmd2.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = txtmtotal2;
                    cmd2.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtmcat2;
                    cmd2.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDesc2;
                    cmd2.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                    cmd2.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                    cmd2.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                    cmd2.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtMFac;

                    con2.Open();
                    cmd2.ExecuteNonQuery();
                    con2.Close();
                }

                if (check3 == 1)
                {
                    if (txtDesc3 is null)
                    {
                        txtDesc3 = "";
                    }

                    SqlConnection con3 = new SqlConnection(connection);
                    SqlCommand cmd3 = new SqlCommand();
                    cmd3 = new SqlCommand("sp_Expense_AddMultiple", con3);
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                    cmd3.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                    cmd3.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = txtMerch3;
                    cmd3.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDate3;
                    cmd3.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = txtmtotal3;
                    cmd3.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtmcat3;
                    cmd3.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDesc3;
                    cmd3.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                    cmd3.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                    cmd3.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                    cmd3.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtMFac;

                    con3.Open();
                    cmd3.ExecuteNonQuery();
                    con3.Close();
                }

                if (check4 == 1)
                {
                    if (txtDesc4 is null)
                    {
                        txtDesc4 = "";
                    }

                    SqlConnection con4 = new SqlConnection(connection);
                    SqlCommand cmd4 = new SqlCommand();
                    cmd4 = new SqlCommand("sp_Expense_AddMultiple", con4);
                    cmd4.CommandType = CommandType.StoredProcedure;
                    cmd4.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                    cmd4.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                    cmd4.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = txtMerch4;
                    cmd4.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDate4;
                    cmd4.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = txtmtotal4;
                    cmd4.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtmcat4;
                    cmd4.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDesc4;
                    cmd4.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                    cmd4.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                    cmd4.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                    cmd4.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtMFac;

                    con4.Open();
                    cmd4.ExecuteNonQuery();
                    con4.Close();
                }

                if (check5 == 1)
                {
                    if (txtDesc5 is null)
                    {
                        txtDesc5 = "";
                    }

                    SqlConnection con5 = new SqlConnection(connection);
                    SqlCommand cmd5 = new SqlCommand();
                    cmd5 = new SqlCommand("sp_Expense_AddMultiple", con5);
                    cmd5.CommandType = CommandType.StoredProcedure;
                    cmd5.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                    cmd5.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                    cmd5.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = txtMerch5;
                    cmd5.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDate5;
                    cmd5.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = txtmtotal5;
                    cmd5.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtmcat5;
                    cmd5.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDesc5;
                    cmd5.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                    cmd5.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                    cmd5.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                    cmd5.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtMFac;

                    con5.Open();
                    cmd5.ExecuteNonQuery();
                    con5.Close();
                }

                if (check6 == 1)
                {
                    if (txtDesc6 is null)
                    {
                        txtDesc6 = "";
                    }

                    SqlConnection con6 = new SqlConnection(connection);
                    SqlCommand cmd6 = new SqlCommand();
                    cmd6 = new SqlCommand("sp_Expense_AddMultiple", con6);
                    cmd6.CommandType = CommandType.StoredProcedure;
                    cmd6.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                    cmd6.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                    cmd6.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = txtMerch6;
                    cmd6.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = txtDate6;
                    cmd6.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = txtmtotal6;
                    cmd6.Parameters.Add("@Category", SqlDbType.VarChar).Value = txtmcat6;
                    cmd6.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = txtDesc6;
                    cmd6.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = supname;
                    cmd6.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = supemail;
                    cmd6.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                    cmd6.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtMFac;

                    con6.Open();
                    cmd6.ExecuteNonQuery();
                    con6.Close();
                }


                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "Expense Report Submitted";
                string body = "A expense report was just submitted.<br/><br/>You can view the report <a href=\"https://pacs-technology.com/Expense\">HERE</a>.";

                // Send the email.
                await GraphService.SendEmail(graphClient, _env, supemail, HttpContext, subject, body);

                return RedirectToAction("Index", new { strSave = "Success! Your record was saved." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }

        [HttpPost]
        public IActionResult PostMultiEdit(
    string strMultiMerchant, string strMultiDate, string strMultiTotal,
    string strMultiCategory, string strMultiDescription,
    string strMultiUID, string strMultiFacility)
        {
            try
            {
                if (strMultiDescription is null)
                {
                    strMultiDescription = "";
                }
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_Expense_EditMultiple", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Merchant", SqlDbType.VarChar).Value = strMultiMerchant;
                cmd.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = strMultiDate;
                cmd.Parameters.Add("@ExpenseTotal", SqlDbType.Money).Value = strMultiTotal;
                cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = strMultiCategory;
                cmd.Parameters.Add("@ExpenseDescription", SqlDbType.VarChar).Value = strMultiDescription;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = strMultiUID;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strMultiFacility;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index", new { strSave = "Success! Your record was edited." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }


        [HttpPost]
        public IActionResult PostSupEdit(string txtID, string txtRedirect)
        {

            return RedirectToAction("SupEdit", new { supeditid = txtID, supredirect = txtRedirect });

        }

        [HttpPost]
        public IActionResult PostEdit(string strUID)
        {
            return RedirectToAction("Edit", new { editid = strUID });
        }

        [HttpPost]
        public IActionResult PostFinanceEdit(string txtID)
        {
            return RedirectToAction("FinanceEdit", new { editid = txtID });
        }

        [HttpPost]
        public async Task<IActionResult> PostSupSave(string strUID, string strStatus, string strNotes, string strUser, string txtRedirect)
        {
            string completed = "no";

            if (strStatus == "Approved")
            {
                completed = "yes";
            }

            if (strNotes is null)
            {
                strNotes = "";
            }

            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_Expense_SupervisorEdit", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@ApprovedEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@ApprovalStatus", SqlDbType.VarChar).Value = strStatus;
            cmd.Parameters.Add("@ApproveNotes", SqlDbType.VarChar).Value = strNotes;
            cmd.Parameters.Add("@completed", SqlDbType.VarChar).Value = completed;
            cmd.Parameters.Add("@UID", SqlDbType.VarChar).Value = strUID;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

            string subject = "";
            string body = "";
            if (strStatus == "Declined")
            {
                subject = "Expense Report Declined";
                body = "Your expense report was declined.<br/>You can see notes on why it was declined <a href\"https://pacs-technology.com/Expense\">HERE</a>";
                await GraphService.SendEmail(graphClient, _env, strUser, HttpContext, subject, body);
            } else
            {
                //subject = "Expense Report Approved";
                //body = "A expense report was Approved.<br/>Please verify the GL Code <a href\"https://pacs-technology.com/Expense\">HERE</a>";
                //strUser = "finance@pacshc.com";
            }


            

            return RedirectToAction(txtRedirect);

        }

        [HttpPost]
        public IActionResult PostFinanceSave(string strUID, string strStatus, string strCat)
        {
            if (strStatus == "Approved")
            {
                strStatus = "True";
            } else
            {
                strStatus = "False;";
            }

            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_Expense_FinanceEdit", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FinanceBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@FinanceEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@FinanceApproved", SqlDbType.Bit).Value = strStatus;
            cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = strCat;
            cmd.Parameters.Add("@UID", SqlDbType.VarChar).Value = strUID;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Finance");

        }

        [HttpPost]
        public IActionResult PostDelExpense(string strDUID, string txtRedirect)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("DELETE FROM EXPENSE WHERE ID = '"+strDUID+"'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction(txtRedirect);

        }


        [HttpPost]
        public IActionResult PostDelExpenseEdit(string strDUID)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("DELETE FROM EXPENSE WHERE ID = '" + strDUID + "'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> GenReport(string txtMonth, string txtYear)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            tw.WriteLine("PAYCHEX ID,Last Name,First Name,Bonus Supplemental,Bonus,Vaccine Bonus,Expense Reimb- Non tax");

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select SubmitEmail, sum(ExpenseTotal) from Expense where year(ApprovalDate) = '"+txtYear+"' and month(ApprovalDate) = '"+txtMonth+ "' and ApprovalStatus = 'Approved' and FinanceApproved = 1 group by SubmitEmail order by SubmitEmail";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    var wrline = ",";
                    wrline += await getgiven(Convert.ToString(idr["submitemail"])) + ",";
                    wrline += await getsur(Convert.ToString(idr["submitemail"])) + ",";
                    wrline += ",";
                    wrline += ",";
                    wrline += ",";
                    wrline += Convert.ToString(idr["expensetotal"]) + ",";

                    tw.WriteLine(wrline);
                }
            }
            con.Close();

            tw.Flush();
            tw.Close();

            return File(memoryStream.GetBuffer(), "text/plain", txtMonth + "_" + txtYear + "_" + "PREPAID.csv");
        }
    }
}
