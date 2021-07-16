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
    public class TripleCheckController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public TripleCheckController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
        }

        public IActionResult pdffooter()
        {
            return View();
        }



        [Authorize]
        public async Task<IActionResult> Index(string facid, string monthid)
        {
            if (facid is null)
            {
                facid = "";
            }

            if (monthid is null)
            {
                string curmontshort = DateTime.Now.ToString("MM");
                string curyear = DateTime.Now.ToString("yyyy");

                monthid = curyear + curmontshort;
            }
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["facility"] = await operationlist(facid);
            ViewData["monthstransfer"] = getmonthstransfer();
            ViewData["monthid"] = monthid;
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "TripleCheck");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            return View();
        }


        [Authorize]
        public async Task<IActionResult> Add(string passid, string passmonth)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["facilityid"] = passid;
            ViewData["reportmonth"] = passmonth;
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "TripleCheck");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Record(string passid)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "TripleCheck");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            getdetails(passid);
            ViewData["saveddata"] = getsaves(passid);
            //ViewData["getrecert"] = getrecerts(passid);
            //ViewData["teamdata"] = getteam(passid);
            //ViewData["teameditdata"] = getteamedit(passid);
            ViewData["username"] = User.Identity.Name;
            return View();
        }

        public string getsaves(string passid)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from TripleCheck_Saves where recordID = '" + passid + "' order by ParentName";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returntext += Convert.ToString(idr["parentname"]) + "$||$";
                    returntext += Convert.ToString(idr["savedvalue"]) + "$||$";
                    returntext += Convert.ToString(idr["parenttype"]) + "$||$";
                    returntext += Convert.ToString(idr["confirmby"]) + "$||$";
                    returntext += Convert.ToString(idr["confirmdate"]) + "*^^*";
                }
            }
            con.Close();

            if (returntext.Length > 4)
            {
                returntext = returntext.Substring(0, returntext.Length - 4);
            }
            

            return returntext;
        }

        public string getrecerts(string passid)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from TripleCheck_Recert where recordID = '" + passid + "' order by position";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    
                    returntext += Convert.ToString(idr["parentname"]) + "$||$";
                    returntext += Convert.ToString(idr["completedate"]) + "$||$";
                    returntext += Convert.ToString(idr["dueby"]) + "$||$";
                    returntext += Convert.ToString(idr["position"]) + "$||$";
                    returntext += Convert.ToString(idr["confirmby"]) + "$||$";
                    returntext += Convert.ToString(idr["confirmdate"]) + "*^^*";
                }
            }
            con.Close();

            if (returntext.Length > 4)
            {
                returntext = returntext.Substring(0, returntext.Length - 4);
            }


            return returntext;
        }

        public string getteam(string facilityid, string reportmonth)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from TripleCheck_Team where facilityid = '" + facilityid + "' and reportmonth = '"+reportmonth+"' order by teammember";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returntext += "<div><b>" + Convert.ToString(idr["teammember"]) + "</b> - " + Convert.ToString(idr["teammembercred"]) + "</div>";
                }
            }
            con.Close();

            return returntext;
        }

        public string getteamedit(string facilityid, string reportmonth)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from TripleCheck_Team where facilityid = '" + facilityid + "' and reportmonth = '" + reportmonth + "' order by teammember";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returntext += "<div>";
                    returntext += "<input type=\"checkbox\" name=\"checkremove\" value=\"" + Convert.ToString(idr["id"]) + "\"/>";
                    returntext += "  <b>" + Convert.ToString(idr["teammember"]) + "</b> - " + Convert.ToString(idr["teammembercred"]) + "</div>";
                }
            }
            con.Close();

            return returntext;
        }



        public string savedetails(string strid, string stritem, string strdetail)
        {
            
            if (stritem == "Yes") { stritem = "True"; }
            if (stritem == "No") { stritem = "False"; }

            string commandtext = "update TripleCheck_Records set "+strdetail+" = '"+stritem+"' where id = '"+strid+"' ";

            if (stritem == "null")
            {
                commandtext = "update TripleCheck_Records set " + strdetail + " = null where id = '" + strid + "' ";
            }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(commandtext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            
            return "";
        }

        public string saverecords(string strid, string strparent, string strvalue, string strtype, string strdate, string strUser)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_SaveRecord", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecordID", SqlDbType.VarChar).Value = strid;
            cmd.Parameters.Add("@ParentName", SqlDbType.VarChar).Value = strparent;
            cmd.Parameters.Add("@SavedValue", SqlDbType.VarChar).Value = strvalue;
            cmd.Parameters.Add("@ParentType", SqlDbType.VarChar).Value = strtype;
            cmd.Parameters.Add("@ConfirmBy", SqlDbType.VarChar).Value = strUser;
            cmd.Parameters.Add("@ConfirmDate", SqlDbType.DateTime).Value = strdate;


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }

        public string saverecordscert(string strid, string strparent, string strvalue, string strtype, string strposition, string strdate, string strUser)
        {

            string confirmnull = "no";
            if (strvalue == "" || strvalue is null)
            {
                strvalue = "";
                strdate = "1/1/1800";
                strUser = "";
                confirmnull = "yes";
            }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_SaveRecordCert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecordID", SqlDbType.VarChar).Value = strid;
            cmd.Parameters.Add("@ParentName", SqlDbType.VarChar).Value = strparent;
            cmd.Parameters.Add("@CompleteDate", SqlDbType.VarChar).Value = strvalue;
            cmd.Parameters.Add("@DueBy", SqlDbType.VarChar).Value = strtype;
            cmd.Parameters.Add("@Position", SqlDbType.Int).Value = strposition;
            cmd.Parameters.Add("@ConfirmBy", SqlDbType.VarChar).Value = strUser;
            cmd.Parameters.Add("@ConfirmDate", SqlDbType.DateTime).Value = strdate;
            cmd.Parameters.Add("@ConfirmNull", SqlDbType.VarChar).Value = confirmnull;


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }

        public string saveteam(string facilityid, string reportmonth, string strName, string strCred)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_SaveTeam", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FacilityID", SqlDbType.Int).Value = facilityid;
            cmd.Parameters.Add("@ReportMonth", SqlDbType.VarChar).Value = reportmonth;
            cmd.Parameters.Add("@TeamMember", SqlDbType.VarChar).Value = strName;
            cmd.Parameters.Add("@TeamMemberCred", SqlDbType.VarChar).Value = strCred;


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            
            return getteamedit(facilityid, reportmonth);
        }

        public string removeteam(string facilityid, string reportmonth, string strTeamID)
        {

            string commandtext = "DELETE FROM TripleCheck_Team WHERE ID = '" + strTeamID + "'";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(commandtext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            
            return getteamedit(facilityid, reportmonth);
        }

        public string savenotes(string strid, string stritem)
        {
            if (stritem is null)
            {
                stritem = "";
            }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_SaveNotes", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecordID", SqlDbType.VarChar).Value = strid;
            cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = stritem;


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }

        [HttpPost]
        public IActionResult subnotes(string txtnotesid, string txtpartialnots)
        {
            if (txtpartialnots is null)
            {
                txtpartialnots = "";
            }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_SaveNotes", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecordID", SqlDbType.VarChar).Value = txtnotesid;
            cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = txtpartialnots;


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return View("NotesPartial");
        }

        [HttpPost]
        public JsonResult AjaxMethod(string id, string notes)
        {
            if (notes is null)
            {
                notes = "";
            }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_SaveNotes", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecordID", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = notes;


            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();


            return Json(notes);
        }


        public string removerecords(string strid, string strparent)
        {

            string commandtext = "DELETE FROM TripleCheck_Saves WHERE RECORDID = '"+strid+"' AND PARENTNAME = '"+ strparent+"'";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(commandtext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }

        public string removerecordscert(string strid, string strparent)
        {

            string commandtext = "DELETE FROM TripleCheck_Recert WHERE RECORDID = '" + strid + "' AND PARENTNAME = '" + strparent + "'";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(commandtext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }

        public string getmonths(string strfac, string monthid)
        {
            string returntext = "<div class=\"txtlabel\">Month</div><select class=\"txtbox\" id=\"ddReportMonth\" onchange=\"monthchange()\">";
            returntext += "<option></option>";
            string selected = "";

            if (strfac == "")
            {
                
            } else
            {
                string curmonth = DateTime.Now.ToString("MMMM");
                string curmontshort = DateTime.Now.ToString("MM");
                string curyear = DateTime.Now.ToString("yyyy");
                string nextmonth = DateTime.Now.AddMonths(1).ToString("MMMM");
                string nextmonthshort = DateTime.Now.AddMonths(1).ToString("MM");
                string nextyear = DateTime.Now.AddMonths(1).ToString("yyyy");

                string prevmonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
                string prevmonthshort = DateTime.Now.AddMonths(-1).ToString("MM");
                string prevyear = DateTime.Now.AddMonths(-1).ToString("yyyy");

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("select distinct ReportMonth, INTMonth from TripleCheck_Records where facilityid = '" + strfac + "' order by INTMonth desc", con);
                con.Open();
                SqlDataReader idr = cmd.ExecuteReader();

                if (prevyear + prevmonthshort == monthid)
                {
                    returntext += "<option selected=\"selected\" value=\"" + prevyear + prevmonthshort + "\">" + prevmonth + " " + prevyear + "</option>";
                    selected = "1";
                }
                else
                {
                    returntext += "<option value=\"" + prevyear + prevmonthshort + "\">" + prevmonth + " " + prevyear + "</option>";
                }

                
                

                string returntextend = "";
                
                if (nextyear + nextmonthshort == monthid)
                {
                    returntextend += "<option selected=\"selected\" value=\"" + nextyear + nextmonthshort + "\">" + nextmonth + " " + nextyear + "</option>";
                    selected = "1";
                } else
                {
                    returntextend += "<option value=\"" + nextyear + nextmonthshort + "\">" + nextmonth + " " + nextyear + "</option>";
                }
                

                
                if (idr.HasRows)
                {

                    while (idr.Read())
                    {

                        if (Convert.ToString(idr["ReportMonth"]) == curmonth + " " + curyear || Convert.ToString(idr["ReportMonth"]) == nextmonth + " " + nextyear || Convert.ToString(idr["ReportMonth"]) == prevmonth + " " + prevyear)
                        {
                            
                        } else
                        {
                            if (Convert.ToString(idr["IntMonth"]) == monthid)
                            {
                                returntextend += "<option selected=\"selected\" value=\"" + Convert.ToString(idr["IntMonth"]) + "\">" + Convert.ToString(idr["ReportMonth"]) + "</option>";
                                selected = "1";
                            }
                            else
                            {
                                returntextend += "<option value=\"" + Convert.ToString(idr["IntMonth"]) + "\">" + Convert.ToString(idr["ReportMonth"]) + "</option>";
                            }

                            
                        }
                    }
                }
                con.Close();

                if (selected == "1")
                {
                    returntext += "<option value=\"" + curyear + curmontshort + "\">" + curmonth + " " + curyear + "</option>";
                }
                else
                {
                    returntext += "<option selected=\"selected\" value=\"" + curyear + curmontshort + "\">" + curmonth + " " + curyear + "</option>";
                }

                returntext += returntextend;
            }



            returntext += "</select>";
            return returntext;
        }

        public string getmonthstransfer()
        {
            string returntext = "<div class=\"txtlabel\">Transfer to Month</div><select class=\"txtbox\" id=\"ddReportMonthTransfer\">";

                string curmonth = DateTime.Now.ToString("MMMM");
                string curmontshort = DateTime.Now.ToString("MM");
                string curyear = DateTime.Now.ToString("yyyy");
                string nextmonth = DateTime.Now.AddMonths(1).ToString("MMMM");
                string nextmonthshort = DateTime.Now.AddMonths(1).ToString("MM");
                string nextyear = DateTime.Now.AddMonths(1).ToString("yyyy");

                returntext += "<option value=\"" + curyear + curmontshort + "\">" + curmonth + " " + curyear + "</option>";
                returntext += "<option value=\"" + nextyear + nextmonthshort + "\">" + nextmonth + " " + nextyear + "</option>";


            returntext += "</select>";
            return returntext;
        }

        public string getdetails(string passid)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from TripleCheck_Records where ID = '" + passid + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    ViewData["patientname"] = Convert.ToString(idr["firstname"])+ " " + Convert.ToString(idr["lastname"]) + "  --  " + Convert.ToString(idr["medicalid"]) ;
                    ViewData["patientmonth"] = getallpatientmonths(Convert.ToString(idr["medicalid"]), passid);

                    ViewData["patientnotes"] = Convert.ToString(idr["notes"]);

                    getdobgender(Convert.ToString(idr["medicalid"]), Convert.ToString(idr["facilityid"]));
                    ViewData["facid"] = Convert.ToString(idr["facilityid"]);
                    ViewData["monthid"] = Convert.ToString(idr["intmonth"]);

                    string intmonth = Convert.ToString(idr["intmonth"]);
                    string strmonth = intmonth.Substring(intmonth.Length - 2);
                    string stryear = intmonth.Substring(0, intmonth.Length - 2);

                    var dMonth = DateTime.Parse(strmonth + "/1/" + stryear);
                    
                    ViewData["intmonth"] = dMonth.AddMonths(1).ToShortDateString();

                    try
                    {
                        DateTime staystart = Convert.ToDateTime(idr["startstay"]);
                        string smonth = staystart.Month.ToString();
                        string sday = staystart.Day.ToString();
                        string syear = staystart.Year.ToString();

                        if (smonth.Length == 1) { smonth = "0" + smonth; }
                        if (sday.Length == 1) { sday = "0" + sday; }
                        ViewData["staydate"] = smonth + "/" + sday + "/" + syear;
                    } catch
                    {

                    }


                    //ViewData["InitialCert"] = staystart.AddDays(3).ToShortDateString();
                    

                    //ViewData["ReCert"] = staystart.AddDays(14).ToShortDateString();

                    string addassess = "<select class=\"txtbox\" onchange=\"addassessment(this)\" id=\"ddAddAssessment\">";

                    if (Convert.ToString(idr["AdditionalAssessment"]) == "False")
                    {
                        addassess += "<option selected=\"selected\">No</option>";
                        addassess += "<option>Yes</option>";
                    } else
                    {
                        addassess += "<option>No</option>";
                        addassess += "<option selected=\"selected\">Yes</option>";
                    }

                    addassess += "</select>";

                    ViewData["AdditionalAssessment"] = addassess;

                    string paymenttype = "<select class=\"txtbox\" id=\"ddPaymentType\" onchange=\"paymentchange()\">";

                    if (Convert.ToString(idr["PaymentType"]) == "HMO A - PDPM")
                    {
                        paymenttype += "<option selected=\"selected\">HMO A - PDPM</option>";
                    } else
                    {
                        paymenttype += "<option>HMO A - PDPM</option>";
                    }

                    if (Convert.ToString(idr["PaymentType"]) == "HMO A - RUG")
                    {
                        paymenttype += "<option selected=\"selected\">HMO A - RUG</option>";
                    }
                    else
                    {
                        paymenttype += "<option>HMO A - RUG</option>";
                    }

                    if (Convert.ToString(idr["PaymentType"]) == "HMO Levels")
                    {
                        paymenttype += "<option selected=\"selected\">HMO Levels</option>";
                    }
                    else
                    {
                        paymenttype += "<option>HMO Levels</option>";
                    }

                    if (Convert.ToString(idr["PaymentType"]) == "MCB")
                    {
                        paymenttype += "<option selected=\"selected\">MCB</option>";
                    }
                    else
                    {
                        paymenttype += "<option>MCB</option>";
                    }

                    if (Convert.ToString(idr["PaymentType"]) == "MED A - PDPM")
                    {
                        paymenttype += "<option selected=\"selected\">MED A - PDPM</option>";
                    }
                    else
                    {
                        paymenttype += "<option>MED A - PDPM</option>";
                    }

                    paymenttype += "</select>";

                    ViewData["PaymentType"] = paymenttype;

                    
                }
            }
            con.Close();

            ViewData["passid"] = passid;
            return "";
        }

        public string getdobgender(string medicalID, string facilityID)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from patients where operationId = '" + facilityID + "'  and medicalRecordNumber = '"+medicalID+"'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    DateTime dob = Convert.ToDateTime(idr["dateofbirth"]);
                    ViewData["patientdob"] = "(" +dob.ToShortDateString() + ")";
                    ViewData["patientgender"] = "(" + Convert.ToString(idr["gender"]) + ")";
                }
            }
            con.Close();

            return "";
        }

        public string getallpatientmonths(string medicalid, string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select id, ReportMonth from TripleCheck_Records where MedicalID = '"+medicalid+"' order by INTMonth, CreateDate";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "<select class=\"largedd\" onchange=\"monthchange(this)\">";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (Convert.ToString(idr["id"]) == passid)
                    {
                        returntext += "<option selected=\"selected\" value=\""+ Convert.ToString(idr["id"]) + "\">"+ Convert.ToString(idr["reportmonth"]) + "</option>";
                    } else
                    {
                        returntext += "<option value=\"" + Convert.ToString(idr["id"]) + "\">" + Convert.ToString(idr["reportmonth"]) + "</option>";
                    }

                }
            }
            con.Close();

            returntext += "</select>";
            return returntext;
        }


        public string IndexTable( string stritem, string facid)
        {

            if (stritem == "curmonth")
            {
                string curmontshort = DateTime.Now.ToString("MM");
                string curyear = DateTime.Now.ToString("yyyy");

                stritem = curyear + curmontshort;
            }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from TripleCheck_Records where FacilityID = '" + facid + "' and INTMonth = '"+stritem+"' and deleted = 0";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Medical ID</th>";
            prepaidtable += "<th>Patient</th>";
            prepaidtable += "<th>Stay Start</th>";
            prepaidtable += "<th>Payment Type</th>";
            prepaidtable += "<th>BO</th>";
            prepaidtable += "<th>DC</th>";
            prepaidtable += "<th>MDS</th>";
            prepaidtable += "<th>MDS/Ther</th>";
            prepaidtable += "<th>MR</th>";
            prepaidtable += "<th>Ther</th>";
            prepaidtable += "<th>Nurs</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["medicalid"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["lastname"]) + ", "+ Convert.ToString(idr["firstname"]) + "</td>";

                    try
                    {
                        DateTime staystart = Convert.ToDateTime(idr["startstay"]);
                        prepaidtable += "<td>" + staystart.ToShortDateString() + "</td>";
                    } catch  {
                        
                        prepaidtable += "<td></td>";
                    }

                    
                    prepaidtable += "<td>" + Convert.ToString(idr["paymenttype"]) + "</td>";

                    string headers;

                    headers = Convert.ToString(idr["businessoffice"]);
                    if (headers == "False")
                    {
                        prepaidtable += "<td style=\"color:red\"><b>X</b></td>";
                    }
                    else
                    {
                        prepaidtable += "<td style=\"color:green\"><b>&#10004;</b></td>";
                    }

                    headers = Convert.ToString(idr["DCInformation"]);
                    if (headers == "False")
                    {
                        prepaidtable += "<td style=\"color:red\"><b>X</b></td>";
                    }
                    else
                    {
                        prepaidtable += "<td style=\"color:green\"><b>&#10004;</b></td>";
                    }

                    headers = Convert.ToString(idr["MDS"]);
                    if (headers == "False")
                    {
                        prepaidtable += "<td style=\"color:red\"><b>X</b></td>";
                    }
                    else
                    {
                        prepaidtable += "<td style=\"color:green\"><b>&#10004;</b></td>";
                    }

                    headers = Convert.ToString(idr["MDSTherapy"]);
                    if (headers == "False")
                    {
                        prepaidtable += "<td style=\"color:red\"><b>X</b></td>";
                    }
                    else
                    {
                        prepaidtable += "<td style=\"color:green\"><b>&#10004;</b></td>";
                    }

                    headers = Convert.ToString(idr["MedicalRecords"]);
                    if (headers == "False")
                    {
                        prepaidtable += "<td style=\"color:red\"><b>X</b></td>";
                    }
                    else
                    {
                        prepaidtable += "<td style=\"color:green\"><b>&#10004;</b></td>";
                    }

                    headers = Convert.ToString(idr["Therapy"]);
                    if (headers == "False")
                    {
                        prepaidtable += "<td style=\"color:red\"><b>X</b></td>";
                    }
                    else
                    {
                        prepaidtable += "<td style=\"color:green\"><b>&#10004;</b></td>";
                    }

                    headers = Convert.ToString(idr["Nursing"]);
                    if (headers == "False")
                    {
                        prepaidtable += "<td style=\"color:red\"><b>X</b></td>";
                    }
                    else
                    {
                        prepaidtable += "<td style=\"color:green\"><b>&#10004;</b></td>";
                    }

                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            prepaidtable += "</tbody></table>";

            return prepaidtable;
        }

        public string TransferTable(string stritem, string facid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select *, Case when (select count(*) from TripleCheck_Saves s where RecordID = r.ID and ParentName = 'BODischargeQ' and SavedValue = 'Yes') > 0 Then 1 else 0 end as 'discharge' from TripleCheck_Records r where FacilityID = '" + facid + "' and INTMonth = '" + stritem + "' and deleted = 0";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table class=\"transfer\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th><input type=\"checkbox\" onClick=\"toggle(this)\" /></th>";
            prepaidtable += "<th>Medical ID</th>";
            prepaidtable += "<th>Patient</th>";
            prepaidtable += "<th>Stay Start</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    if (Convert.ToString(idr["discharge"]) == "0")
                    {
                        prepaidtable += "<tr>";
                        prepaidtable += "<td><input type=\"checkbox\" name=\"cbtransfer\" value=\"" + Convert.ToString(idr["id"]) + "\"/></td>";
                        prepaidtable += "<td>" + Convert.ToString(idr["medicalid"]) + "</td>";
                        prepaidtable += "<td>" + Convert.ToString(idr["lastname"]) + ", " + Convert.ToString(idr["firstname"]) + "</td>";

                        try
                        {
                            DateTime staystart = Convert.ToDateTime(idr["startstay"]);
                            prepaidtable += "<td>" + staystart.ToShortDateString() + "</td>";
                        }
                        catch
                        {

                            prepaidtable += "<td></td>";
                        }

                        prepaidtable += "</tr>";
                    }

                }
            }
            con.Close();

            prepaidtable += "</tbody></table>";

            return prepaidtable;
        }

        public async Task<string> operationlist(string facid)
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "<select id=\"ddFacility\" style=\"width: 280px!important\" class=\"txtbox\" onchange=\"facilitychange()\">";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select operationname, operationid from operations order by operationname", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string strSelect = "";

            if (idr.HasRows)
            {
                operations += "<option value=\"\"></option>";
                while (idr.Read())
                {

                    if (facid == "")
                    {
                        if (response.Contains(Convert.ToString(idr["operationname"])) && strSelect == "")
                        {
                            operations += "<option selected=\"selected\" value=\"" + Convert.ToString(idr["operationid"]) + "\">" + Convert.ToString(idr["operationname"]) + "</option>";
                            strSelect = "select";
                        }
                        else
                        {
                            operations += "<option  value=\"" + Convert.ToString(idr["operationid"]) + "\">" + Convert.ToString(idr["operationname"]) + "</option>";
                        }
                    } else
                    {
                        if (Convert.ToString(idr["operationid"]) == facid)
                        {
                            operations += "<option selected=\"selected\" value=\"" + Convert.ToString(idr["operationid"]) + "\">" + Convert.ToString(idr["operationname"]) + "</option>";
                            strSelect = "select";
                        }
                        else
                        {
                            operations += "<option  value=\"" + Convert.ToString(idr["operationid"]) + "\">" + Convert.ToString(idr["operationname"]) + "</option>";
                        }
                    }
 


                }
            }
            con.Close();

            operations += "</select>";
            return operations;

        }


        public string SearchPatient(string stritem, string facid, string reportmonth)
        {


            string operations = "";

            operations = SearchPatientDup(stritem, facid, reportmonth);

            if (operations == "") {

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("select * from patients where medicalrecordnumber = '" + stritem + "' and operationid = '" + facid + "'", con);
                con.Open();
                SqlDataReader idr = cmd.ExecuteReader();

                if (idr.HasRows)
                {

                    while (idr.Read())
                    {
                        operations += "<div class=\"txtlabel\">Medical Record #</div>";
                        operations += "<div class=\"txtbox\">" + Convert.ToString(idr["medicalrecordnumber"]) + "</div>";

                        operations += "<div class=\"txtlabel\">First Name</div>";
                        operations += "<div class=\"txtbox\">" + Convert.ToString(idr["patientfirstname"]) + "</div>";

                        operations += "<div class=\"txtlabel\">Last Name</div>";
                        operations += "<div class=\"txtbox\">" + Convert.ToString(idr["patientlastname"]) + "</div>";

                        operations += "<div><input type=\"button\" class=\"btn btn-primary\" style=\"margin-top:10px;\" value=\"Add Patient\" onclick=\"addpatient()\" /></div>";
                    }
                }
                else
                {
                    operations = "<div class=\"noresults\">No Results Found!</div>";
                }
                con.Close();
            } else
            {
                operations = "Patient already has a record for this month.";
            }



            return operations;
        }

        public string SearchPatientDup(string stritem, string facid, string reportmonth)
        {
            string operations = "";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from TripleCheck_Records where medicalid = '" + stritem + "' and facilityid = '" + facid + "' and intmonth = '"+reportmonth+"'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                operations = "dup";
    
            }
            else
            {
                operations = "";
            }
            con.Close();

            return "";
        }

        [HttpPost]
        public IActionResult GoToAdd(string txtFacility, string txtReportMonth)
        {
            return RedirectToAction("Add", new { passid = txtFacility, passmonth = txtReportMonth });
        }

        [HttpPost]
        public IActionResult PostGoToRecord(string txtID)
        {
            return RedirectToAction("Record", new { passid = txtID});
        }

        [HttpPost]
        public IActionResult DeleteReport(string txtRecID)
        {
            string commandtext = "update TripleCheck_Records set deleted = 1 WHERE ID = '" + txtRecID + "'";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(commandtext, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult AddPatient(string txtFacility, string txtMedicalID, string txtReportMonth)
        {
            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            string strmonth = txtReportMonth.Substring(txtReportMonth.Length - 2);
            string stryear = txtReportMonth.Substring(0, txtReportMonth.Length - 2);
            if (strmonth == "01") { strmonth = "January"; }
            if (strmonth == "02") { strmonth = "February"; }
            if (strmonth == "03") { strmonth = "March"; }
            if (strmonth == "04") { strmonth = "April"; }
            if (strmonth == "05") { strmonth = "May"; }
            if (strmonth == "06") { strmonth = "June"; }
            if (strmonth == "07") { strmonth = "July"; }
            if (strmonth == "08") { strmonth = "August"; }
            if (strmonth == "09") { strmonth = "September"; }
            if (strmonth == "10") { strmonth = "October"; }
            if (strmonth == "11") { strmonth = "November"; }
            if (strmonth == "12") { strmonth = "December"; }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_AddRecord", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@CreateBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@CreateEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@FacilityID", SqlDbType.Int).Value = txtFacility;
            cmd.Parameters.Add("@MedicalID", SqlDbType.Int).Value = txtMedicalID;
            cmd.Parameters.Add("@ReportMonth", SqlDbType.VarChar).Value = strmonth + " " + stryear;
            cmd.Parameters.Add("@INTMonth", SqlDbType.Int).Value = txtReportMonth;

            string returntext = "";
            con.Open();
            returntext = cmd.ExecuteScalar().ToString();
            con.Close();


            return RedirectToAction("Record", new { passid = returntext });
        }

        public string DoTransfer(string strid, string intmonth, string strmonth)
        {
            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_Transfer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@CreateBy", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@CreateEmail", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = strid;
            cmd.Parameters.Add("@INTMONTH", SqlDbType.Int).Value = intmonth;
            cmd.Parameters.Add("@REPORTMONTH", SqlDbType.VarChar).Value = strmonth;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();


            return "";
        }

        [HttpPost]
        public IActionResult PaymentChange(string txtID, string txtPayType)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_TripleCheck_PaymentChange", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecordID", SqlDbType.VarChar).Value = txtID;
            cmd.Parameters.Add("@PaymentType", SqlDbType.VarChar).Value = txtPayType;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();


            return RedirectToAction("Record", new { passid = txtID });
        }
    }
}
