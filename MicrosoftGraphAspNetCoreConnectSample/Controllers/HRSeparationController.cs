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
    public class HRSeparationController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public HRSeparationController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
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
            ViewData["facility"] = await operationlist();
            ViewData["HRCheck"] = await HRCheck();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> HRView(string strSave)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["HRCheck"] = await HRCheck();
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["changetable"] = getrequests();
            ViewData["Message"] = strSave;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Edit(string passid)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["passid"] = passid;
            ViewData["getdetails"] = getdetails(passid);
            return View();
        }

        public string getdetails(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select *, CASE WHEN CompleteBy IS NULL THEN 'NO' ELSE 'YES' END AS 'COMPLETED' from SeparationChecklist where ID = '" + passid + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    prepaidtable += "<div class=\"row\">";
                    prepaidtable += "<div class=\"col-md-4\">";
                    prepaidtable += "<div>Notes</div>";
                    prepaidtable += "<textarea id=\"txtNotes\" style=\"width:280px; height:100px\">" + Convert.ToString(idr["completenotes"]) + "</textarea>";
                    prepaidtable += "</div>";
                    prepaidtable += "<div class=\"col-md-3\" style=\"padding-top:15px\">";

                    if (Convert.ToString(idr["completed"]) == "YES")
                    {
                        prepaidtable += "<div><input type=\"checkbox\" id=\"cbComplete\" checked=\"checked\" />&nbsp;&nbsp;Complete Request</div>";
                        prepaidtable += "<div class=\"textlabel\">Completed By: " + Convert.ToString(idr["completeby"]) + "</div>";
                        prepaidtable += "<div><b>Completed: " + Convert.ToString(idr["completedate"]) + "</b></div>";
                    }
                    else
                    {
                        prepaidtable += "<div><input type=\"checkbox\" id=\"cbComplete\" />&nbsp;&nbsp;Complete Request</div>";
                    }

                    prepaidtable += "<input id=\"btnSub\" type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px;margin-top:15px;\" value=\"Save\" onclick=\"validatesub()\" />";
                    prepaidtable += "</div>";
                    prepaidtable += "</div>";

                    prepaidtable += "<hr />";
                    prepaidtable += "<div class=\"row\">";
                    prepaidtable += "<div class=\"col-md-4\">";
                    prepaidtable += "<div class=\"textlabel\">Submitted By</div>";
                    prepaidtable += "<div>" + Convert.ToString(idr["submitby"]) + "</div>";
                    prepaidtable += "</div>";
                    prepaidtable += "<div class=\"col-md-4\">";
                    prepaidtable += "<div class=\"textlabel\">Date Submitted</div>";
                    prepaidtable += "<div>" + Convert.ToString(idr["submitdate"]) + "</div>";
                    prepaidtable += "</div>";
                    prepaidtable += "</div>";

                    prepaidtable += "<hr />";

                    prepaidtable += " <div class=\"row\">";
                    prepaidtable += " <div class=\"col-md-4\">";
                    prepaidtable += " <div class=\"txtlabel\">Employee Name</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["EmployeeName"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " <div class=\"txtlabel\">Facility</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["Facility"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " <div class=\"txtlabel\">Department</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["Department"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " <div class=\"txtlabel\">Job Title</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["JobTitle"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"col-md-4\">";
                    prepaidtable += " <div class=\"txtlabel\">Last Day Worked</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["LastDayWorked"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " <div class=\"txtlabel\">Termination Date</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["TerminationDate"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " <div class=\"txtlabel\">Type of Termination</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["TypeOfTermination"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " <div class=\"txtlabel\">Rehire</div>";
                    prepaidtable += " <div><input type=\"text\" value=\"" + Convert.ToString(idr["Rehire"]) + "\" class=\"txtbox\" disabled=\"disabled\" /></div>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">Reason</div>";
                    prepaidtable += " <div><textarea class=\"txtbox\" disabled=\"disabled\" style=\"height: 100px\" >"+ Convert.ToString(idr["reason"]) + "</textarea></div>";
                    prepaidtable += " <div class=\"formheader\">Items to be returned</div>";
                    prepaidtable += " <div class=\"row\">";
                    prepaidtable += " <div class=\"col-md-3\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["IDBadge"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> ID Badge</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["ComputerLaptop"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Computer/Laptop</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"col-md-3\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["buildingkeys"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Building Keys/Access Card</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["cellphone"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Cell Phone</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"col-md-3\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["deskkeys"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Desk/File Keys</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["creditcard"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\" > Credit Card</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["returnedother"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\" onclick=\"changecb('cbReturnOther')\"> Other</span>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\""+ Convert.ToString(idr["returnedothertext"]) + "\" />";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"formheader\">Information to be reviewed with exiting employee</div>";
                    prepaidtable += " <div style=\"height:15px\"></div>";
                    prepaidtable += " <div class=\"row\">";
                    prepaidtable += " <div class=\"col-md-3\" style=\"margin-right:20px\">";
                    prepaidtable += " <b>Agreements:</b>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"row\">";
                    prepaidtable += " <div class=\"col-md-4\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["noncompete"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Non-compete &#38; non-solicitation Agreement</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"col-md-3\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["confidentiality"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Confidentiality Agreement</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\"><b>Pay and Benefits:</b></div>";
                    prepaidtable += " <div class=\"row\">";
                    prepaidtable += " <div class=\"col-md-4\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["healthflexible"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Health/Flexible Spending</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["medicaldental"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Medical/Dental/Vision/COBRA</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["Severancepay"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Severance Pay (if applicable)</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["lifeinsurance"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Life - and/or Conversion/Continuation</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"col-md-4\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["unemployment"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Unemployment Insurance</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["hsahra"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> HSA/HRA (if applicable)</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["expensereimbursement"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Expense Reimbursement</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["reviewd401k"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> 401(k)</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["finalpaydate"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Final Pay Date</span>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\"" + Convert.ToString(idr["finalpaydatetext"]) + "\" />";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["Vacationbalance"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Vacation/PTO Balance</span>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\"" + Convert.ToString(idr["vacationbalancetext"]) + "\" />";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["sicktime"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Sick Time (CA)</span>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\"" + Convert.ToString(idr["sicktimetext"]) + "\" />";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["reviewedother"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Other</span>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\"" + Convert.ToString(idr["reviewedothertext"]) + "\" />";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"formheader\">Required Notifications</div>";
                    prepaidtable += " <div class=\"row\">";
                    prepaidtable += " <div class=\"col-md-3\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["pcccontact"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> PCC Contact</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["payroll"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Payroll</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["required401k"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> 401(k)</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"col-md-3\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["itcontact"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> IT Contact</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["BenefitVendors"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Benefit Vendors</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["requiredcreditcard"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Credit Card/Cell Phone Contact</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"col-md-3\">";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["casambacontact"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Casamba Contact</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["oasis"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\" > Oasis</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["requiredunion"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Union Rep</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\"><b>Email</b></div>";
                    prepaidtable += " <div>";
                    if (Convert.ToString(idr["Emaildisable"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Disable for 30 days</span>";
                    if (Convert.ToString(idr["emaildelete"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Delete</span>";
                    if (Convert.ToString(idr["emailforward"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Forward To </span>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\"" + Convert.ToString(idr["emailforwardtext"]) + "\" />";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["requiredother"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Other</span>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\"" + Convert.ToString(idr["requiredothertext"]) + "\" />";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"formheader\">Miscellaneous</div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["signedresignation"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Obtain signed Resignation notice (if applicable)</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["companyreports"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Check company reports against employee participation (credit cards, special programs, etc.)</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">";
                    if (Convert.ToString(idr["employeefile"]) == "True") { prepaidtable += " <input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/>"; } else { prepaidtable += " <input type=\"checkbox\" disabled=\"disabled\"/>"; }
                    prepaidtable += " <span class=\"cbtext\"> Employee File</span>";
                    prepaidtable += " </div>";
                    prepaidtable += " <div class=\"txtlabel\">Total Hours for Final Paycheck </div>";
                    prepaidtable += " <input type=\"text\" class=\"txtbox\" disabled=\"disabled\" value=\"" + Convert.ToString(idr["finalpaycheck"]) + "\" />";
                    prepaidtable += " <div class=\"txtlabel\">Additional Comments</div>";
                    prepaidtable += " <div><textarea class=\"txtbox\" disabled=\"disabled\" style=\"height: 100px\">" + Convert.ToString(idr["additionalcomments"]) + "</textarea></div>";

                }
            }
            con.Close();

            return prepaidtable;
        }

        public string getrequests()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select ID, SubmitBy, SubmitDate, EmployeeName, Facility, TerminationDate, TypeOfTermination, CASE WHEN CompleteBy IS NULL THEN 'NO' ELSE 'YES' END AS 'COMPLETED' from SeparationChecklist";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Submitted</th>";
            prepaidtable += "<th>Submitted By</th>";
            prepaidtable += "<th>Employee</th>";
            prepaidtable += "<th>Facility</th>";
            prepaidtable += "<th>Term Date</th>";
            prepaidtable += "<th>Type of Term</th>";
            prepaidtable += "<th>Completed</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    string statuscolor = "lightblue";
                    if (Convert.ToString(idr["completed"]) == "YES")
                    {
                        statuscolor = "lightgreen";
                    }


                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    DateTime expensedate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + expensedate.ToShortDateString() + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["submitby"]), 25) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["employeename"]), 50) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 25) + "</td>";
                    DateTime termdate = Convert.ToDateTime(idr["terminationdate"]);
                    prepaidtable += "<td>" + termdate.ToShortDateString() + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["typeoftermination"]), 25) + "</td>";
                    prepaidtable += "<td style=\"background-color:" + statuscolor + " !important\">" + Convert.ToString(idr["completed"]) + "</td>";
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

        public async Task<string> HRCheck()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string returntext = "";

            if (response.Contains("HR Team") || response.Contains("Executives_SG"))
            {
                returntext = "<a href=\"/HRSeparation/HRView\">Go to HR View</a>";
            }

            return returntext;
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
            string txtEmployeeName, string txtFacility, string txtDepartment, string txtJobTitle,
            string txtLastDay, string txtTermDate, string txtTermType, string txtRehire,
            string strReason, string strcbIDBadge, string strcbComputerLaptop, string strcbBuildingKeys,
            string strcbCellPhone, string strcbDeskKeys, string strcbCreditCard, string strcbReturnOther,
            string txtReturnOther, string strcbNonCompete, string strcbConfidential, string strcbFlexSpending,
            string strcbCobra, string strcbSeverance, string strcbLife, string strcbUnemployment,
            string strcbHSA, string strcbExpense, string strcbReview401, string strcbFinalPay,
            string txtFinalPay, string strcbVacation, string txtVacation, string strcbSickTime,
            string txtSickTime, string strcbReviewOther, string txtReviewOther, string strcbPCC,
            string strcbPayroll, string strcbRequired401, string strcbIT, string strcbBenefit,
            string strcbRequiredCell, string strcbCasamba, string strcbOasis, string strcbUnion,
            string strcbDisable, string strcbDelete, string strcbForward, string txtForward,
            string strcbRequiredOther, string txtRequiredOther, string strcbSigned, string strcbCheck,
            string strcbEmployee, string txtHours, string strAddComments)
        {
            try
            {
                if (txtReturnOther is null) { txtReturnOther = ""; }
                if (txtFinalPay is null) { txtFinalPay = ""; }
                if (txtVacation is null) { txtVacation = ""; }
                if (txtSickTime is null) { txtSickTime = ""; }
                if (txtReviewOther is null) { txtReviewOther = ""; }
                if (txtForward is null) { txtForward = ""; }
                if (txtRequiredOther is null) { txtRequiredOther = ""; }
                if (strAddComments is null) { strAddComments = ""; }

                string username = User.Identity.Name;
                string email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_SeparationChecklist_Add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@EmployeeName", SqlDbType.VarChar).Value = txtEmployeeName;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtFacility;
                cmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = txtDepartment;
                cmd.Parameters.Add("@JobTitle", SqlDbType.VarChar).Value = txtJobTitle;
                cmd.Parameters.Add("@LastDayWorked", SqlDbType.Date).Value = txtLastDay;
                cmd.Parameters.Add("@TerminationDate", SqlDbType.Date).Value = txtTermDate;
                cmd.Parameters.Add("@TypeOfTermination", SqlDbType.VarChar).Value = txtTermType;
                cmd.Parameters.Add("@Rehire", SqlDbType.VarChar).Value = txtRehire;
                cmd.Parameters.Add("@Reason", SqlDbType.VarChar).Value = strReason;
                cmd.Parameters.Add("@IDBadge", SqlDbType.Bit).Value = strcbIDBadge;
                cmd.Parameters.Add("@ComputerLaptop", SqlDbType.Bit).Value = strcbComputerLaptop;
                cmd.Parameters.Add("@BuildingKeys", SqlDbType.Bit).Value = strcbBuildingKeys;
                cmd.Parameters.Add("@CellPhone", SqlDbType.Bit).Value = strcbCellPhone;
                cmd.Parameters.Add("@DeskKeys", SqlDbType.Bit).Value = strcbDeskKeys;
                cmd.Parameters.Add("@CreditCard", SqlDbType.Bit).Value = strcbCreditCard;
                cmd.Parameters.Add("@ReturnedOther", SqlDbType.Bit).Value = strcbReturnOther;
                cmd.Parameters.Add("@ReturnedOtherText", SqlDbType.VarChar).Value = txtReturnOther;
                cmd.Parameters.Add("@NonCompete", SqlDbType.Bit).Value = strcbNonCompete;
                cmd.Parameters.Add("@Confidentiality", SqlDbType.Bit).Value = strcbConfidential;
                cmd.Parameters.Add("@HealthFlexible", SqlDbType.Bit).Value = strcbFlexSpending;
                cmd.Parameters.Add("@MedicalDental", SqlDbType.Bit).Value = strcbCobra;
                cmd.Parameters.Add("@SeverancePay", SqlDbType.Bit).Value = strcbSeverance;
                cmd.Parameters.Add("@LifeInsurance", SqlDbType.Bit).Value = strcbLife;
                cmd.Parameters.Add("@Unemployment", SqlDbType.Bit).Value = strcbUnemployment;
                cmd.Parameters.Add("@HSAHRA", SqlDbType.Bit).Value = strcbHSA;
                cmd.Parameters.Add("@ExpenseReimbursement", SqlDbType.Bit).Value = strcbExpense;
                cmd.Parameters.Add("@Reviewd401k", SqlDbType.Bit).Value = strcbReview401;
                cmd.Parameters.Add("@FinalPayDate", SqlDbType.Bit).Value = strcbFinalPay;
                cmd.Parameters.Add("@FinalPayDateText", SqlDbType.VarChar).Value = txtFinalPay;
                cmd.Parameters.Add("@VacationBalance", SqlDbType.Bit).Value = strcbVacation;
                cmd.Parameters.Add("@VacationBalanceText", SqlDbType.VarChar).Value = txtVacation;
                cmd.Parameters.Add("@SickTime", SqlDbType.Bit).Value = strcbSickTime;
                cmd.Parameters.Add("@SickTimeText", SqlDbType.VarChar).Value = txtSickTime;
                cmd.Parameters.Add("@ReviewedOther", SqlDbType.Bit).Value = strcbReviewOther;
                cmd.Parameters.Add("@ReviewedOtherText", SqlDbType.VarChar).Value = txtReviewOther;
                cmd.Parameters.Add("@PCCContact", SqlDbType.Bit).Value = strcbPCC;
                cmd.Parameters.Add("@ITContact", SqlDbType.Bit).Value = strcbIT;
                cmd.Parameters.Add("@CasambaContact", SqlDbType.Bit).Value = strcbCasamba;
                cmd.Parameters.Add("@Payroll", SqlDbType.Bit).Value = strcbPayroll;
                cmd.Parameters.Add("@BenefitVendors", SqlDbType.Bit).Value = strcbBenefit;
                cmd.Parameters.Add("@Oasis", SqlDbType.Bit).Value = strcbOasis;
                cmd.Parameters.Add("@Required401k", SqlDbType.Bit).Value = strcbRequired401;
                cmd.Parameters.Add("@RequiredCreditCard", SqlDbType.Bit).Value = strcbRequiredCell;
                cmd.Parameters.Add("@RequiredUnion", SqlDbType.Bit).Value = strcbUnion;
                cmd.Parameters.Add("@EmailDisable", SqlDbType.Bit).Value = strcbDisable;
                cmd.Parameters.Add("@EmailDelete", SqlDbType.Bit).Value = strcbDelete;
                cmd.Parameters.Add("@EmailForward", SqlDbType.Bit).Value = strcbForward;
                cmd.Parameters.Add("@EmailForwardText", SqlDbType.VarChar).Value = txtForward;
                cmd.Parameters.Add("@RequiredOther", SqlDbType.Bit).Value = strcbRequiredOther;
                cmd.Parameters.Add("@RequiredOtherText", SqlDbType.VarChar).Value = txtRequiredOther;
                cmd.Parameters.Add("@SignedResignation", SqlDbType.Bit).Value = strcbSigned;
                cmd.Parameters.Add("@CompanyReports", SqlDbType.Bit).Value = strcbCheck;
                cmd.Parameters.Add("@EmployeeFile", SqlDbType.Bit).Value = strcbEmployee;
                cmd.Parameters.Add("@FinalPaycheck", SqlDbType.VarChar).Value = txtHours;
                cmd.Parameters.Add("@AdditionalComments", SqlDbType.VarChar).Value = strAddComments;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                if (strcbIDBadge == "True") { strcbIDBadge = "&#9746"; } else { strcbIDBadge = "&#9744"; }
                if (strcbComputerLaptop == "True") { strcbComputerLaptop = "&#9746"; } else { strcbComputerLaptop = "&#9744"; }
                if (strcbBuildingKeys == "True") { strcbBuildingKeys = "&#9746"; } else { strcbBuildingKeys = "&#9744"; }
                if (strcbCellPhone == "True") { strcbCellPhone = "&#9746"; } else { strcbCellPhone = "&#9744"; }
                if (strcbDeskKeys == "True") { strcbDeskKeys = "&#9746"; } else { strcbDeskKeys = "&#9744"; }
                if (strcbCreditCard == "True") { strcbCreditCard = "&#9746"; } else { strcbCreditCard = "&#9744"; }
                if (strcbReturnOther == "True") { strcbReturnOther = "&#9746"; } else { strcbReturnOther = "&#9744"; }
                if (strcbNonCompete == "True") { strcbNonCompete = "&#9746"; } else { strcbNonCompete = "&#9744"; }
                if (strcbConfidential == "True") { strcbConfidential = "&#9746"; } else { strcbConfidential = "&#9744"; }
                if (strcbFlexSpending == "True") { strcbFlexSpending = "&#9746"; } else { strcbFlexSpending = "&#9744"; }
                if (strcbCobra == "True") { strcbCobra = "&#9746"; } else { strcbCobra = "&#9744"; }
                if (strcbSeverance == "True") { strcbSeverance = "&#9746"; } else { strcbSeverance = "&#9744"; }
                if (strcbLife == "True") { strcbLife = "&#9746"; } else { strcbLife = "&#9744"; }
                if (strcbUnemployment == "True") { strcbUnemployment = "&#9746"; } else { strcbUnemployment = "&#9744"; }
                if (strcbHSA == "True") { strcbHSA = "&#9746"; } else { strcbHSA = "&#9744"; }
                if (strcbReview401 == "True") { strcbReview401 = "&#9746"; } else { strcbReview401 = "&#9744"; }
                if (strcbFinalPay == "True") { strcbFinalPay = "&#9746"; } else { strcbFinalPay = "&#9744"; }
                if (strcbVacation == "True") { strcbVacation = "&#9746"; } else { strcbVacation = "&#9744"; }
                if (strcbSickTime == "True") { strcbSickTime = "&#9746"; } else { strcbSickTime = "&#9744"; }
                if (strcbReviewOther == "True") { strcbReviewOther = "&#9746"; } else { strcbReviewOther = "&#9744"; }
                if (strcbPCC == "True") { strcbPCC = "&#9746"; } else { strcbPCC = "&#9744"; }
                if (strcbIT == "True") { strcbIT = "&#9746"; } else { strcbIT = "&#9744"; }
                if (strcbCasamba == "True") { strcbCasamba = "&#9746"; } else { strcbCasamba = "&#9744"; }
                if (strcbPayroll == "True") { strcbPayroll = "&#9746"; } else { strcbPayroll = "&#9744"; }
                if (strcbBenefit == "True") { strcbBenefit = "&#9746"; } else { strcbBenefit = "&#9744"; }
                if (strcbOasis == "True") { strcbOasis = "&#9746"; } else { strcbOasis = "&#9744"; }
                if (strcbRequired401 == "True") { strcbRequired401 = "&#9746"; } else { strcbRequired401 = "&#9744"; }
                if (strcbRequiredCell == "True") { strcbRequiredCell = "&#9746"; } else { strcbRequiredCell = "&#9744"; }
                if (strcbUnion == "True") { strcbUnion = "&#9746"; } else { strcbUnion = "&#9744"; }
                if (strcbDisable == "True") { strcbDisable = "&#9746"; } else { strcbDisable = "&#9744"; }
                if (strcbDelete == "True") { strcbDelete = "&#9746"; } else { strcbDelete = "&#9744"; }
                if (strcbForward == "True") { strcbForward = "&#9746"; } else { strcbForward = "&#9744"; }
                if (strcbRequiredOther == "True") { strcbRequiredOther = "&#9746"; } else { strcbRequiredOther = "&#9744"; }
                if (strcbSigned == "True") { strcbSigned = "&#9746"; } else { strcbSigned = "&#9744"; }
                if (strcbCheck == "True") { strcbCheck = "&#9746"; } else { strcbCheck = "&#9744"; }
                if (strcbEmployee == "True") { strcbEmployee = "&#9746"; } else { strcbEmployee = "&#9744"; }

                string subject = "Separation Checklist";
                string body = "";
                string padding = " style=\"margin-left:15px;\" ";

                body += "<table>";
                body += "<tr>";
                body += "<td><b>Employee Name</b><br/>" + txtEmployeeName + "</td>";
                body += "<td "+padding+"><b>Last Day Worked</b><br/>" + txtLastDay + "</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td><b>Facility</b><br/>" + txtFacility + "</td>";
                body += "<td " + padding + "><b>Termination Date</b><br/>" + txtTermDate + "</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td><b>Department</b><br/>" + txtDepartment + "</td>";
                body += "<td " + padding + "><b>Type of Termination</b><br/>" + txtTermType + "</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td><b>Job Title</b><br/>" + txtJobTitle + "</td>";
                body += "<td " + padding + "><b>Rehire</b><br/>" + txtRehire + "</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"2\"><b>Reason</b><br/>" + strReason + "</td>";
                body += "</tr>";
                body += "</table>";
                body += "<br/><br/>";
                body += "<div><b><u>Items to be returned</u></b></div>";
                body += "<table>";
                body += "<tr>";
                body += "<td>"+strcbIDBadge+" ID Badge</td>";
                body += "<td " + padding + ">" + strcbBuildingKeys+" Building Keys/Access Card</td>";
                body += "<td " + padding + ">" + strcbDeskKeys+" Desk/File Keys</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>"+strcbComputerLaptop+" Computer/Laptop</td>";
                body += "<td " + padding + ">" + strcbCellPhone+" Cell Phone</td>";
                body += "<td " + padding + ">" + strcbCreditCard+" Credit Card</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"3\">"+strcbReturnOther+" Other: "+txtReturnOther+"</td>";
                body += "</tr>";
                body += "</table>";
                body += "<br/><br/>";
                body += "<div><b><u>Information to be reviewed with exiting employee</u></b></div>";
                body += "<table>";
                body += "<tr>";
                body += "<td Colspan=\"2\"><b>Agreements:</b></td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>" + strcbNonCompete + " Non-compete & non-solicitation Agreement</td>";
                body += "<td " + padding + ">" + strcbConfidential + " Confidentiality Agreement</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td Colspan=\"2\"><b>Pay and Benefits:</b></td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>" + strcbFlexSpending + " Health/Flexible Spending</td>";
                body += "<td " + padding + ">" + strcbUnemployment + " Unemployment Insurance</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>" + strcbCobra + " Medical/Dental/Vision/COBRA</td>";
                body += "<td " + padding + ">" + strcbHSA + " HSA/HRA</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>" + strcbSeverance + " Severance Pay (if applicable)</td>";
                body += "<td " + padding + ">" + strcbExpense + " Expense Reimbursement</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>" + strcbLife + " Life - and/or Conversion/Continuation</td>";
                body += "<td " + padding + ">" + strcbReview401 + " 401(k)</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"2\">" + strcbFinalPay + " Final Pay Date: " + txtFinalPay + "</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"2\">" + strcbVacation + " Vacation/PTO Balance: " + txtVacation + "</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"2\">" + strcbSickTime + " Sick Time (CA): " + txtSickTime + "</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"2\">" + strcbReviewOther + " Other: " + txtReviewOther + "</td>";
                body += "</tr>";
                body += "</table>";
                body += "<br/><br/>";
                body += "<div><b><u>Required Notifications</u></b></div>";
                body += "<table>";
                body += "<tr>";
                body += "<td>" + strcbPCC + " PCC Contact</td>";
                body += "<td " + padding + ">" + strcbIT + " IT Contact</td>";
                body += "<td " + padding + ">" + strcbCasamba + " Casamba Contact</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>" + strcbPayroll + " Payroll</td>";
                body += "<td " + padding + ">" + strcbBenefit + " Benefit Vendors</td>";
                body += "<td " + padding + ">" + strcbOasis + " Oasis</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td>" + strcbRequired401 + " 401(k)</td>";
                body += "<td " + padding + ">" + strcbRequiredCell + " Credit Card/Cell Phone Contact</td>";
                body += "<td " + padding + ">" + strcbUnion + " Union Rep</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"3\"><b>Email</b></td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"3\">"+strcbDisable+" Disable for 30 days  "+strcbDelete+" Delete  "+strcbForward+" Forward To: "+txtForward+"</td>";
                body += "</tr>";
                body += "<tr>";
                body += "<td colspan=\"3\">" + strcbRequiredOther + " Other: " + txtRequiredOther + "</td>";
                body += "</tr>";
                body += "</table>";
                body += "<br/><br/>";
                body += "<div><b><u>Miscellaneous</u></b></div>";
                body += "<div>" + strcbSigned + " Obtain signed Resignation notice (if applicable)</div>";
                body += "<div>" + strcbCheck + " Check company reports against employee participation (credit cards, special programs, etc.)</div>";
                body += "<div>" + strcbEmployee + " Employee File</div>";
                body += "<div><b>Total Hours for Final Paycheck</b><br/>" + txtHours + "</div>";
                body += "<div><b>Additional Comments</b><br/>" + strAddComments + "</div>";

                await GraphService.SendEmail(graphClient, _env, "daniel.stump@pacshc.com", HttpContext, subject, body);


                return RedirectToAction("Index", new { strSave = "Success! Your request was submitted." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }

        [HttpPost]
        public IActionResult PostEdit(string txtID)
        {
            return RedirectToAction("Edit", new { passid = txtID });
        }

        [HttpPost]
        public IActionResult PostSaveNotes(
        string txtID, string strNotes, string strComplete)
        {
            if (strNotes is null) { strNotes = ""; }
            try
            {
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_SeparationChecklist_SaveNotes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = txtID;
                cmd.Parameters.Add("@CompletedNotes", SqlDbType.VarChar).Value = strNotes;
                cmd.Parameters.Add("@completed", SqlDbType.VarChar).Value = strComplete;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();


                return RedirectToAction("HRView", new { strSave = "Success! Your record was saved." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }
    }
}
