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
    public class HRCallLogController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public HRCallLogController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
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
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "HRCallLog");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["getcalllogs"] = getcalllogs();
            ViewData["checkadmin"] = checkadmin();
            return View();
        }
        [Authorize]
        public async Task<IActionResult> New()
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "HRCallLog");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["Facilities"] = getFacilities();
            ViewData["Department"] = getDepartments();
            ViewData["Category"] = getCategory();
            ViewData["UID"] = getGUID();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> AddNew(string passid)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "HRCallLog");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["Facilities"] = getFacilitiesPass(passid);
            ViewData["Department"] = getDepartmentsPass(passid);
            ViewData["Category"] = getCategoryPass(passid);
            ViewData["getcaller"] = findcaller(passid);
            ViewData["getunion"] = findunion(passid);
            ViewData["UID"] = getattachmentid(passid);
            ViewData["passid"] = passid;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Admin()
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "HRCallLog");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["getadmins"] = getadmins();
            ViewData["getcats"] = getcats();
            ViewData["getdept"] = getdept();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Edit(string passid)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "HRCallLog");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["passid"] = passid;
            ViewData["calldetail"] = getdetails(passid);

            string attachmentid = getattachmentid(passid);
            ViewData["UID"] = attachmentid;
            ViewData["uploads"] = await GetImages(attachmentid);
            ViewData["additionalcalls"] = getadditionalcalls(passid);
            ViewData["loginfo"] = getloginfo(passid);
            return View();
        }

        public string getattachmentid(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from HRCallLog where id = '" + passid + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string strdetails = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    strdetails += Convert.ToString(idr["attachmentid"]);
                }


            }
            con.Close();


            return strdetails;
        }

        public string getadditionalcalls(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from HRCallLog where GroupID = '"+passid+"'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string strdetails = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    strdetails += "<button class=\"collapsible\">"+Convert.ToString(idr["Submitdate"])+"&nbsp;&nbsp;&nbsp;&nbsp;"+ Convert.ToString(idr["Submitby"]) + "</button>";
                    strdetails += "<div class=\"content\">";

                    string strUnion = Convert.ToString(idr["IsUnion"]);
                    if (strUnion == "False")
                    {
                        strUnion = "No";
                    }
                    else
                    {
                        strUnion = "Yes";
                    }

                    string strPrivate = Convert.ToString(idr["Private"]);
                    if (strPrivate == "False")
                    {
                        strPrivate = "No";
                    }
                    else
                    {
                        strPrivate = "Yes";
                    }

                    double duration = Convert.ToInt32(idr["Duration"]);

                    int intHours = Convert.ToInt32(Math.Floor(duration / 60 / 60));
                    int intMin = Convert.ToInt32(Math.Floor(duration / 60) - (intHours * 60));
                    int intSec = Convert.ToInt32(duration % 60);

                    string strDuration = intHours.ToString().PadLeft(2, '0') + ":";
                    strDuration += intMin.ToString().PadLeft(2, '0') + ":";
                    strDuration += intSec.ToString().PadLeft(2, '0');

                    strdetails += "<table style=\"margin-top:10px;\">";
                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Call Duration</td>";
                    strdetails += "<td>" + strDuration + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Caller</td>";
                    strdetails += "<td>" + Convert.ToString(idr["Caller"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Facility</td>";
                    strdetails += "<td>" + Convert.ToString(idr["Facility"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Department</td>";
                    strdetails += "<td>" + Convert.ToString(idr["Department"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Union</td>";
                    strdetails += "<td>" + strUnion + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Category</td>";
                    strdetails += "<td>" + Convert.ToString(idr["Category"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Private</td>";
                    strdetails += "<td>" + strPrivate + "</td>";
                    strdetails += "</tr>";
                    strdetails += "</table>";

                    strdetails += "<br/><div><b>Notes</b></div>";
                    strdetails += "<div id=\""+ Convert.ToString(idr["id"]) + "\" class=\"txtNotes\">" + GetNotes(Convert.ToString(idr["id"])) + "</div>";
                    strdetails += "<button type=\"submit\" class=\"btn btn-primary\" style=\"margin-top:5px; margin-bottom:10px;\" onclick=\"shownewnotes('" + Convert.ToString(idr["id"]) + "')\">Add Notes</button>";

                    strdetails += "</div>";
                }


            }
            con.Close();


            return strdetails;
        }

        public string getloginfo(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from HRCallLog where id = '" + passid + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string strdetails = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    strdetails += "<div><b><i>Submitted By: " + Convert.ToString(idr["submitby"]) + "</i></b></div>";
                    strdetails += "<div><b><i>Submit Date: " + Convert.ToString(idr["Submitdate"]) + "</i></b></div>";
                    strdetails += "<div style=\"height:10px;\"></div>";

                }


            }
            con.Close();


            return strdetails;
        }

        public string getdetails(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from HRCallLog where id = '"+passid+"'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string strdetails = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    string strUnion = Convert.ToString(idr["IsUnion"]);
                    if (strUnion == "False")
                    {
                        strUnion = "No";
                    } else
                    {
                        strUnion = "Yes";
                    }

                    string strPrivate = Convert.ToString(idr["Private"]);
                    if (strPrivate == "False")
                    {
                        strPrivate = "No";
                    }
                    else
                    {
                        strPrivate = "Yes";
                    }

                    double duration = Convert.ToInt32(idr["Duration"]);
                    
                    int intHours = Convert.ToInt32(Math.Floor(duration / 60 / 60));
                    int intMin = Convert.ToInt32(Math.Floor(duration / 60) - (intHours * 60));
                    int intSec = Convert.ToInt32(duration % 60);

                    string strDuration = intHours.ToString().PadLeft(2, '0') + ":";
                    strDuration += intMin.ToString().PadLeft(2, '0') + ":";
                    strDuration += intSec.ToString().PadLeft(2, '0');

                    strdetails += "<table>";
                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Call Duration</td>";
                    strdetails += "<td>" + strDuration + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Caller</td>";
                    strdetails += "<td>"+ Convert.ToString(idr["Caller"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Facility</td>";
                    strdetails += "<td>" + Convert.ToString(idr["Facility"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Department</td>";
                    strdetails += "<td>" + Convert.ToString(idr["Department"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Union</td>";
                    strdetails += "<td>" + strUnion + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Category</td>";
                    strdetails += "<td>" + Convert.ToString(idr["Category"]) + "</td>";
                    strdetails += "</tr>";

                    strdetails += "<tr>";
                    strdetails += "<td style=\"font-weight:bold; padding-right:10px;\">Private</td>";
                    strdetails += "<td>" + strPrivate + "</td>";
                    strdetails += "</tr>";
                    strdetails += "</table>";

                    strdetails += "<br/><div><b>Notes</b></div>";
                    strdetails += "<div id=\"strNotes\" class=\"txtNotes\">"+GetNotes(passid)+"</div>";

                }


            }
            con.Close();


            return strdetails;



        }

        public string GetNotes(string strid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "Select 0 as 'Sort', SubmitBy, SubmitDate, Notes from HRCallLog where ID = '"+strid+"' ";
            sqlcommandtext += "union ";
            sqlcommandtext += "Select 1 as 'Sort', SubmitBy, SubmitDate, Notes from HRCallLog_Notes where GroupID = '"+strid+"'";
            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string paymenttable = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (Convert.ToString(idr["sort"]) == "0")
                    {
                        paymenttable += Convert.ToString(idr["Notes"]);
                    } else
                    {
                        paymenttable += "<br/><br/>";
                        paymenttable += "<b><i>";
                        paymenttable += Convert.ToString(idr["SubmitBy"]) + "<br/>";
                        paymenttable += Convert.ToString(idr["SubmitDate"]) + "<br/>";
                        paymenttable += "</i></b>";
                        paymenttable += Convert.ToString(idr["Notes"]);
                    }
                }
            }
            con.Close();

            return paymenttable;
        }

        public string AddNotes(string stritem, string strid)
        {
            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_HRCallLog_NewNote", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = stritem;
            cmd.Parameters.Add("@GroupID", SqlDbType.VarChar).Value = strid;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return GetNotes(strid);
        }

        public string getadmins()
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from HRCallLog_Admins order by AdminEmail ";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string paymenttable = "<div class=\"tableFixHead\"><table id=\"paymenttable\"  >";
            paymenttable += "<thead>";
            paymenttable += "<tr>";
            paymenttable += "<th>Administrator</th>";
            paymenttable += "</tr>";
            paymenttable += "</thead>";
            paymenttable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    paymenttable += "<tr onclick=\"deladmin('" + Convert.ToString(idr["id"]) + "', '" + Convert.ToString(idr["AdminEmail"]) + "', 'Admins');\">";
                    paymenttable += "<td>" + Convert.ToString(idr["adminemail"]) + "</td>";
                    paymenttable += "</tr>";
                }
            }
            con.Close();

            paymenttable += "</tbody></table></div>";

            return paymenttable;
        }

        public string getcats()
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from HRCallLog_Category order by Category ";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string paymenttable = "<div class=\"tableFixHead\"><table>";
            paymenttable += "<thead>";
            paymenttable += "<tr>";
            paymenttable += "<th>Category</th>";
            paymenttable += "</tr>";
            paymenttable += "</thead>";
            paymenttable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    paymenttable += "<tr onclick=\"deladmin('" + Convert.ToString(idr["id"]) + "', '" + Convert.ToString(idr["Category"]) + "', 'Category');\">";
                    paymenttable += "<td>" + Convert.ToString(idr["Category"]) + "</td>";
                    paymenttable += "</tr>";
                }
            }
            con.Close();

            paymenttable += "</tbody></table></div>";

            return paymenttable;
        }

        public string getdept()
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from HRCallLog_Department order by Department ";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string paymenttable = "<div class=\"tableFixHead\"><table>";
            paymenttable += "<thead>";
            paymenttable += "<tr>";
            paymenttable += "<th>Department</th>";
            paymenttable += "</tr>";
            paymenttable += "</thead>";
            paymenttable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    paymenttable += "<tr onclick=\"deladmin('" + Convert.ToString(idr["id"]) + "', '" + Convert.ToString(idr["Department"]) + "', 'Department');\">";
                    paymenttable += "<td>" + Convert.ToString(idr["Department"]) + "</td>";
                    paymenttable += "</tr>";
                }
            }
            con.Close();

            paymenttable += "</tbody></table></div>";

            return paymenttable;
        }

        public string checkadmin()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from HRCallLog_Admins where AdminEmail = '" + User.FindFirst("preferred_username")?.Value + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";

            if (idr.HasRows)
            {
                returnvalue = "<a href=\"/HRCallLog/Admin\">Go to Admin Section</a>";
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;


        }

        public string DelAdmin(string strid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("delete from HRCallLog_Admins where id = '"+strid+"'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return getadmins();
        }

        public string DelCat(string strid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("delete from HRCallLog_Category where id = '" + strid + "'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return getcats();
        }

        public string DelDept(string strid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("delete from HRCallLog_Department where id = '" + strid + "'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return getdept();
        }

        public string AddAdmin(string strtext)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            string sqltext = "declare @admincount int = (select count(*) from HRCallLog_Admins where AdminEmail = '"+strtext.Trim()+"') ";
            sqltext += "if (@admincount < 1) ";
            sqltext += "begin ";
            sqltext += "insert into HRCallLog_Admins (ID, AdminEmail) values (NEWID(), '"+strtext.Trim()+"') ";
            sqltext += "end ";

            SqlCommand cmd = new SqlCommand(sqltext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return getadmins();
        }

        public string AddCat(string strtext)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            string sqltext = "declare @admincount int = (select count(*) from HRCallLog_Category where Category = '" + strtext.Trim() + "') ";
            sqltext += "if (@admincount < 1) ";
            sqltext += "begin ";
            sqltext += "insert into HRCallLog_Category (ID, Category) values (NEWID(), '" + strtext.Trim() + "') ";
            sqltext += "end ";

            SqlCommand cmd = new SqlCommand(sqltext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return getcats();
        }

        public string AddDept(string strtext)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            string sqltext = "declare @admincount int = (select count(*) from HRCallLog_Department where Department = '" + strtext.Trim() + "') ";
            sqltext += "if (@admincount < 1) ";
            sqltext += "begin ";
            sqltext += "insert into HRCallLog_Department (ID, Department) values (NEWID(), '" + strtext.Trim() + "') ";
            sqltext += "end ";

            SqlCommand cmd = new SqlCommand(sqltext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return getdept();
        }

        public string getcalllogs()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "declare @admin int = (select count(*) from hrcalllog_admins where adminemail = '" + User.FindFirst("preferred_username")?.Value + "') ";
            sqlcommandtext += "if (@admin > 0) ";
            sqlcommandtext += "begin ";
            sqlcommandtext += "select id, submitdate, submitby, caller, facility, category, department, notes, private from hrcalllog where IsParent = 1 ";
            sqlcommandtext += "end ";
            sqlcommandtext += "else ";
            sqlcommandtext += "begin ";
            sqlcommandtext += "select id, submitdate, submitby, caller, facility, category, department, notes, private from hrcalllog ";
            sqlcommandtext += "where IsParent = 1 and submitemail = '" + User.FindFirst("preferred_username")?.Value + "' ";
            sqlcommandtext += "union ";
            sqlcommandtext += "select id, submitdate, submitby, caller, facility, category, department, notes, private from hrcalllog ";
            sqlcommandtext += "where IsParent = 1 and submitemail <> '" + User.FindFirst("preferred_username")?.Value + "' and private = 0 ";
            sqlcommandtext += "end";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Date</th>";
            prepaidtable += "<th>Caller</th>";
            prepaidtable += "<th>Facility</th>";
            prepaidtable += "<th>Category</th>";
            prepaidtable += "<th>Department</th>";
            prepaidtable += "<th>Notes</th>";
            prepaidtable += "<th>Private</th>";
            prepaidtable += "<th>Submitted By</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {


                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    DateTime submitdate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + submitdate.ToShortDateString() + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["caller"]) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 20) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["category"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["department"]) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["notes"]), 25) + "</td>";
                    string strprivate = Convert.ToString(idr["private"]);

                    if (strprivate == "False")
                    {
                        strprivate = "No";
                    } else
                    {
                        strprivate = "Yes";
                    }

                    prepaidtable += "<td>" + strprivate + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["submitby"]) + "</td>";
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

                passvalue += "...";
            }

            return passvalue;
        }

        public string getFacilities()
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
                    returnvalue += "<option>" + Convert.ToString(idr["operationName"]) + "</option>";
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string getFacilitiesPass(string passid)
        {
            string selectedfacility = findfacility(passid);
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
                    if (selectedfacility == Convert.ToString(idr["operationName"]))
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

        public string findfacility(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select Facility from HRCallLog where id = '"+passid+"'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue += Convert.ToString(idr["facility"]) ;
                }
            }
            con.Close();

            return returnvalue;
        }

        public string getDepartments()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select department from HRCallLog_Department order by department", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<select id=\"ddDepartment\" class=\"txtbox\" style=\"width: 280px!important\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue += "<option>" + Convert.ToString(idr["department"]) + "</option>";
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string getDepartmentsPass(string passid)
        {
            string strdepartment = finddepartment(passid);
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select department from HRCallLog_Department order by department", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<select id=\"ddDepartment\" class=\"txtbox\" style=\"width: 280px!important\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (strdepartment == Convert.ToString(idr["department"]))
                    {
                        returnvalue += "<option selected=\"selected\">" + Convert.ToString(idr["department"]) + "</option>";
                    } else
                    {
                        returnvalue += "<option>" + Convert.ToString(idr["department"]) + "</option>";
                    }
                    
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string finddepartment(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select department from HRCallLog where id = '" + passid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue += Convert.ToString(idr["department"]);
                }
            }
            con.Close();

            return returnvalue;
        }

        public string getCategory()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select category from HRCallLog_Category order by category", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<select id=\"ddCategory\" class=\"txtbox\" style=\"width: 280px!important\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue += "<option>" + Convert.ToString(idr["category"]) + "</option>";
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string getCategoryPass(string passid)
        {
            string strCategory = findcategory(passid);
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select category from HRCallLog_Category order by category", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "<select id=\"ddCategory\" class=\"txtbox\" style=\"width: 280px!important\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (strCategory == Convert.ToString(idr["category"]))
                    {
                        returnvalue += "<option selected=\"selected\">" + Convert.ToString(idr["category"]) + "</option>";
                    } else
                    {
                        returnvalue += "<option>" + Convert.ToString(idr["category"]) + "</option>";
                    }
                    
                }
            }
            con.Close();

            returnvalue += "</select>";

            return returnvalue;
        }

        public string findcategory(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select category from HRCallLog where id = '" + passid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue += Convert.ToString(idr["category"]);
                }
            }
            con.Close();

            return returnvalue;
        }

        public string findcaller(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select caller from HRCallLog where id = '" + passid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returnvalue += Convert.ToString(idr["caller"]);
                }
            }
            con.Close();

            return returnvalue;
        }

        public string findunion(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select IsUnion from HRCallLog where id = '" + passid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string returnvalue = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (Convert.ToString(idr["isUnion"]) == "False")
                    {
                        returnvalue = "<select id=\"ddUnion\" class=\"txtbox\" style=\"width: 280px !important\"><option selected=\"selected\">No</option><option>Yes</option></select>";
                    } else
                    {
                        returnvalue = "<select id=\"ddUnion\" class=\"txtbox\" style=\"width: 280px !important\"><option>No</option><option selected=\"selected\">Yes</option></select>";
                    }
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

        public async Task<string> GetImages(string stritem)
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
                        picturelist += "<div class=\"imgdiv\"><table><tr><td rowspan=\"2\">";
                        picturelist += "<img src=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + blobItem.Name + "\" style=\"border-radius:5px\" />";
                        picturelist += "</td><td colspan=\"2\" class=\"imgdivtd\">" + actualname + "</td></tr><tr>";
                        picturelist += "<td><a href=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + actualname + "\">Download</a></td>";
                        picturelist += "<td><a href=\"\" onclick=\"delblob('" + blobItem.Name + "');return false;\">Remove</a></td></tr></table></div>";
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

        [HttpPost]
        public IActionResult PostNew(string strAttUID, string strCaller, string strFacility,
            string strDepartment, string strUnion, string strCategory, string strPrivate,
            string strDuration, string strNotes)
        {
            string[] splitduration = strDuration.Split(":");
            int hours = Int32.Parse(splitduration[0]) * 3600;
            int minutes = Int32.Parse(splitduration[1]) * 60;
            int seconds = Int32.Parse(splitduration[2]);
            strDuration = (hours + minutes + seconds).ToString();

            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_HRCallLog_NewCall", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strFacility;
            cmd.Parameters.Add("@Caller", SqlDbType.VarChar).Value = strCaller;
            cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = strCategory;
            cmd.Parameters.Add("@Private", SqlDbType.Bit).Value = strPrivate;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = strDuration;
            cmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = strDepartment;
            cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = strNotes;
            cmd.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = strAttUID;
            cmd.Parameters.Add("@IsUnion", SqlDbType.Bit).Value = strUnion;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Index", new { strSave = "Success! Your record was saved." });
        }

        [HttpPost]
        public IActionResult PostNewAdd(string strAttUID, string strCaller, string strFacility,
    string strDepartment, string strUnion, string strCategory, string strPrivate,
    string strDuration, string strNotes)
        {
            string[] splitduration = strDuration.Split(":");
            int hours = Int32.Parse(splitduration[0]) * 3600;
            int minutes = Int32.Parse(splitduration[1]) * 60;
            int seconds = Int32.Parse(splitduration[2]);
            strDuration = (hours + minutes + seconds).ToString();

            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_HRCallLog_NewCall_AddOn", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strFacility;
            cmd.Parameters.Add("@Caller", SqlDbType.VarChar).Value = strCaller;
            cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = strCategory;
            cmd.Parameters.Add("@Private", SqlDbType.Bit).Value = strPrivate;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = strDuration;
            cmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = strDepartment;
            cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = strNotes;
            cmd.Parameters.Add("@GroupID", SqlDbType.VarChar).Value = strAttUID;
            cmd.Parameters.Add("@IsUnion", SqlDbType.Bit).Value = strUnion;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Edit", new { passid = strAttUID });
        }

        [HttpPost]
        public IActionResult PostEdit(string txtID)
        {
            return RedirectToAction("Edit", new { passid = txtID });
        }

        [HttpPost]
        public IActionResult PostAddCall(string txtID)
        {
            return RedirectToAction("AddNew", new { passid = txtID });
        }
    }
}
