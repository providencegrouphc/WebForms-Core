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
    public class CapitalExpenseController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public CapitalExpenseController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
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
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "CapitalExpense");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            
            string strcheck = await financecheckog();
            string btnnew = "";
            string cbapprove = "";

            if (strcheck == "finance")
            {
                cbapprove = "<input type=\"checkbox\" id=\"cbStatus\" onchange=\"filterStatus()\" style=\"margin-bottom:15px; margin-left:10px\" />&nbsp;&nbsp;<b>Show Approved</b>";
            }
            else
            {
                btnnew = "<a href=\"/CapitalExpense/New\" class=\"btn btn-primary\" style=\"width:150px\">New Expense</a>";
                cbapprove = "<input type=\"checkbox\" checked=\"checked\" id=\"cbStatus\" onchange=\"filterStatus()\" style=\"margin-bottom:15px; margin-left:10px\" />&nbsp;&nbsp;<b>Show Approved</b>";
            }

            ViewData["cbapprove"] = cbapprove;
            ViewData["newbtn"] = btnnew;
            ViewData["getexpense"] = await getexpense();
            return View();
        }
        [Authorize]
        public async Task<IActionResult> New()
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);

            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "CapitalExpense");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["Facilities"] = await operationlist();
            ViewData["UID"] = getGUID();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Edit(string passid)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);

            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "CapitalExpense");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            string strcheck = await financecheck(passid);
            string subbtn = "";
            if (strcheck == "none" || strcheck == "director")
            {
                subbtn = "<input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px;margin-top:15px;\" value=\"Save Changes\" onclick=\"validatesub()\" />";
            }

            ViewData["subbtn"] = subbtn;
            ViewData["subinfo"] = subinfo(passid, strcheck);
            ViewData["passid"] = passid;
            ViewData["UID"] = getGUIDpass(passid);
            ViewData["getdetails"] = await getdetails(passid);
            return View();
        }

        public string subinfo(string passid, string typecheck)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "Select * from capitalexpense where id = '" + passid + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    string ddstatus = "";
                    if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    {
                        returntext += "<div style=\"background-color: lightgreen; text-align:center; width:100%\"><b>APPROVED</b></div>";
                        ddstatus += "<option selected=\"selected\">Approved</option>";
                    } else
                    {
                        ddstatus += "<option>Approved</option>";
                    }

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Declined")
                    {
                        returntext += "<div style=\"background-color: lightcoral; text-align:center; width:100%\"><b>DECLINED</b></div>";
                        ddstatus += "<option selected=\"selected\">Declined</option>";
                    } else
                    {
                        ddstatus += "<option>Declined</option>";
                    }

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Pending")
                    {
                        returntext += "<div style=\"background-color: lightblue; text-align:center; width:100%\"><b>PENDING</b></div>";
                        ddstatus += "<option selected=\"selected\">Pending</option>";
                    } else
                    {
                        ddstatus += "<option>Pending</option>";
                    }

                    returntext += "<div class=\"row\">";
                    returntext += "<div class=\"col-md-4\">";
                    returntext += "<div>Approver Notes</div>";

                   if (typecheck.Contains("finance"))
                    {
                        returntext += "<div><textarea id=\"txtApprovalNotes\" name=\"txtApprovalNotes\" maxlength=\"500\" class=\"txtbox\" style=\"height:150px\">" + Convert.ToString(idr["ApproveNotes"]) + "</textarea></div>";
                        returntext += "</div>";
                        returntext += "<div class=\"col-md-4\">";
                        returntext += "<div>Change Status</div>";
                        returntext += "<select id=\"ddstatus\" class=\"txtbox\">";
                        returntext += ddstatus;
                        returntext += "</select>";
                        returntext += "<input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px;margin-top:15px;\" value=\"Save Status\" onclick=\"changestatus()\" />";

                        returntext += "</div>";
                    } else
                    {
                        returntext += "<div><textarea disabled=\"disabled\" id=\"txtApprovalNotes\" name=\"txtApprovalNotes\" maxlength=\"500\" class=\"txtbox\" style=\"height:150px\">" + Convert.ToString(idr["ApproveNotes"]) + "</textarea></div>";
                        returntext += "</div>";
                    }

                    
                    returntext += "</div>";
                }
            }
            con.Close();
            return returntext;
        }
        public string getGUIDpass(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "Select * from capitalexpense where id = '" + passid + "'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returntext = Convert.ToString(idr["AttachmentID"]);
                }
            }
            con.Close();
            return returntext;
        }

    
        public async Task<string> getdetails(string passid)
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "Select * from capitalexpense where id = '"+passid+"'";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string returntext = "";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    DateTime completionDate = Convert.ToDateTime(idr["CompletionDate"]);
                    string strMonth = completionDate.Month.ToString();
                    string strDay = completionDate.Day.ToString();
                    string strYear = completionDate.Year.ToString();

                    if (strMonth.Length == 1)
                    {
                        strMonth = "0" + strMonth;
                    }

                    if (strDay.Length == 1)
                    {
                        strDay = "0" + strDay;
                    }


                    string subbtn = "";
                    
                    string txtdisable = "";
                    if (Convert.ToString(idr["SubmitEmail"]) != email || Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    {
                        txtdisable = "disabled=\"disabled\"";
                    } else
                    {
                        subbtn = "<input type=\"button\" class=\"btn btn-primary\" style=\"margin-bottom:10px;margin-top:15px;\" value=\"Save Changes\" onclick=\"validatesub()\" />";
                    }

                    ViewData["subbtn"] = subbtn;

                    returntext += "<div><b><i>Submitted By:  " + Convert.ToString(idr["SubmitBy"]) + "</i></b></div>";
                    returntext += "<div><b><i>Submitted:  " + Convert.ToString(idr["SubmitDate"]) + "</i></b></div>";

                    returntext += "<div class=\"row\">";
                    returntext += "<div class=\"col-md-4\">";
                    returntext += "<div class=\"txtlabel\">Facility</div>";

                    if (txtdisable == "")
                    {
                        returntext += "<div>" + operationlistpass(Convert.ToString(idr["Facility"])) + "</div>" ;
                    } else
                    {
                        returntext += "<div><input "+txtdisable+" value=\""+ Convert.ToString(idr["Facility"]) + "\" type=\"text\" class=\"txtbox\" /></div>";
                    }

                    returntext += "<div id=\"validateFacility\" class=\"hidden\">Please enter select a facility.</div>";
                    returntext += "<div class=\"txtlabel\">Department</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["Department"]) + "\" type=\"text\" maxlength=\"25\" id=\"txtDepartment\" class=\"txtbox\" name=\"txtDepartment\" /></div>";
                    returntext += "<div id=\"validateDepartment\" class=\"hidden\">Please enter a department.</div>";
                    returntext += "<div class=\"txtlabel\">Request Title</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["RequestTitle"]) + "\" type=\"text\" maxlength=\"25\" id=\"txtTitle\" class=\"txtbox\" name=\"txtTitle\" /></div>";
                    returntext += "<div id=\"validateTitle\" class=\"hidden\">Please enter a title.</div>";
                    returntext += "</div>";
                    returntext += "<div class=\"col-md-4\">";
                    returntext += "<div class=\"txtlabel\">Problem History</div>";
                    returntext += "<div><textarea " + txtdisable + " id=\"txtProblem\" name=\"txtProblem\" maxlength=\"500\" class=\"txtbox\" style=\"height:150px\">" + Convert.ToString(idr["ProblemHistory"]) + "</textarea></div>";
                    returntext += "<div id=\"validateProblem\" class=\"hidden\">Please enter a problem.</div>";
                    returntext += "</div>";
                    returntext += "<div class=\"col-md-4\">";
                    returntext += "<div class=\"txtlabel\">Recommended Solution</div>";
                    returntext += "<div><textarea " + txtdisable + " id=\"txtRecommended\" name=\"txtRecommended\" maxlength=\"500\" class=\"txtbox\" style=\"height:150px\">" + Convert.ToString(idr["RecommendedSolution"]) + "</textarea></div>";
                    returntext += "<div id=\"validateRecommended\" class=\"hidden\">Please enter a solution.</div>";
                    returntext += "</div>";
                    returntext += "</div>";
                    returntext += "<div style=\"height:15px\"></div>";
                    returntext += "<h4>Bid Information</h4>";
                    returntext += "<hr style=\"margin:0px\" />";
                    returntext += "<div class=\"row\">";
                    returntext += "<div class=\"col-md-4\">";
                    returntext += "<div class=\"txtlabel\">Low Bid</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["LowBid"]) + "\" type=\"text\" id=\"txtLowBid\" class=\"txtbox\" name=\"txtLowBid\" /></div>";
                    returntext += "<div id=\"validateLowBid\" class=\"hidden\">Please enter a low bid.</div>";
                    returntext += "<div class=\"txtlabel\">High Bid</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["HighBid"]) + "\" type=\"text\" id=\"txtHighBid\" class=\"txtbox\" name=\"txtHighBid\" /></div>";
                    returntext += "<div id=\"validateHighBid\" class=\"hidden\">Please enter a high bid.</div>";
                    returntext += "<div class=\"txtlabel\">Bid Count</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["BidCount"]) + "\" type=\"text\" id=\"txtBidCount\" class=\"txtbox\" name=\"txtBidCount\" /></div>";
                    returntext += "<div id=\"validateBidCount\" class=\"hidden\">Please enter a bid count.</div>";
                    returntext += "</div>";
                    returntext += "<div class=\"col-md-4\">";
                    returntext += "<div class=\"txtlabel\">Recommended Vendor</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["RecommendedVendor"]) + "\" type=\"text\" id=\"txtVendor\" maxlength=\"25\" class=\"txtbox\" name=\"txtVendor\" /></div>";
                    returntext += "<div id=\"validateVendor\" class=\"hidden\">Please enter a vendor.</div>";
                    returntext += "<div class=\"txtlabel\">Recommended Bid</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["RecommendedBid"]) + "\" type=\"text\" id=\"txtRecBid\" class=\"txtbox\" name=\"txtRecBid\" /></div>";
                    returntext += "<div id=\"validateRecBid\" class=\"hidden\">Please enter a bid.</div>";
                    returntext += "</div>";
                    returntext += "<div class=\"col-md-4\">";
                    returntext += "<div class=\"txtlabel\">Estimated Total Cost</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + Convert.ToString(idr["EstimatedTotal"]) + "\" type=\"text\" id=\"txtTotal\" class=\"txtbox\" name=\"txtTotal\" /></div>";
                    returntext += "<div id=\"validateTotal\" class=\"hidden\">Please enter a cost.</div>";
                    returntext += "<div class=\"txtlabel\">Estimated Completion Date</div>";
                    returntext += "<div><input " + txtdisable + " value=\"" + strMonth + "/" + strDay + "/" + strYear + "\" type=\"text\" id=\"txtDate\" class=\"txtbox\" name=\"txtDate\" /></div>";
                    returntext += "<div id=\"validateDate\" class=\"hidden\">Please enter a valid date.</div>";
                    returntext += "</div>";
                    returntext += "</div>";
                }
            }
            con.Close();



            return returntext;
        }

        public async Task<string> financecheck(string passid)
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string returntext = "none";

            if (response.Contains("Technology"))
            {
                returntext = "finance";
            }

            if (response.Contains("PACS Regional Directors of Operations") || response.Contains("Executives_SG") || response.Contains("Regional Directors of Operations DG") || response.Contains("zzzzz"))
            {
                returntext = "director";
            }

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from capitalexpense where id = '"+passid+"'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    {
                        if (returntext == "finance")
                        {
                            returntext = "financeapproved";
                        }

                        if (returntext == "director")
                        {
                            returntext = "directorapproved";
                        }

                        if (returntext == "none")
                        {
                            returntext = "approved";
                        }
                    }

                }
            }
            con.Close();

            return returntext;
        }

        public async Task<string> financecheckog()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string returntext = "none";

            if (response.Contains("Technology"))
            {
                returntext = "finance";
            }

            if (response.Contains("PACS Regional Directors of Operations") || response.Contains("Executives_SG") || response.Contains("Regional Directors of Operations DG") || response.Contains("zzzzz"))
            {
                returntext = "director";
            }

            return returntext;
        }

        public async Task<string> getexpense()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "";

            if (response.Contains("PACS Regional Directors of Operations") || response.Contains("Executives_SG") || response.Contains("Regional Directors of Operations DG") || response.Contains("Technology") || response.Contains("zzzzz"))
            {
                sqlcommandtext = "select *, case when ApprovalStatus = 'approved' then '1' else '0' end as 'statusfilter' from CapitalExpense";
            } else
            {
                sqlcommandtext = "select *, case when ApprovalStatus = 'approved' then '1' else '0' end as 'statusfilter' from CapitalExpense where submitemail = '" + email+"'";
            }



            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Date</th>";
            prepaidtable += "<th>From</th>";
            prepaidtable += "<th>Facility</th>";
            prepaidtable += "<th>Department</th>";
            prepaidtable += "<th>Title</th>";
            prepaidtable += "<th>Status</th>";
            prepaidtable += "<th>StatusFilter</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    string backgroundcolor = "lightblue";
                    if (Convert.ToString(idr["ApprovalStatus"]) == "Approved")
                    {
                        backgroundcolor = "lightgreen";
                    }

                    if (Convert.ToString(idr["ApprovalStatus"]) == "Declined")
                    {
                        backgroundcolor = "lightcoral";
                    }

                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    DateTime submitdate = Convert.ToDateTime(idr["submitdate"]);
                    prepaidtable += "<td>" + submitdate.ToShortDateString() + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["submitby"]) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 25) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["Department"]), 25) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["RequestTitle"]) + "</td>";
                    prepaidtable += "<td style=\"background-color:"+backgroundcolor+"\">" + Convert.ToString(idr["ApprovalStatus"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["statusfilter"]) + "</td>";
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

        public async Task<string> operationlist()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "<select id=\"ddfacility\" class=\"txtbox\">";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select operationname from operations order by operationname", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string strSelect = "";

            if (idr.HasRows)
            {
                operations += "<option></option>";
                while (idr.Read())
                {
                    if (response.Contains("_PG_Administrators_SG") && response.Contains(Convert.ToString(idr["operationname"])) && strSelect == "")
                    {
                        operations += "<option selected=\"selected\">" + Convert.ToString(idr["operationname"]) + "</option>";
                    } else
                    {
                        operations += "<option>" + Convert.ToString(idr["operationname"]) + "</option>";
                    }

                    
                }
            }
            con.Close();

            operations += "</select>";
            return operations;

        }

        public string operationlistpass(string facility)
        {
            string operations = "<select id=\"ddfacility\" class=\"txtbox\">";

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
                    if (facility == Convert.ToString(idr["operationname"]))
                    {
                        operations += "<option selected=\"selected\">" + Convert.ToString(idr["operationname"]) + "</option>";
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
        public async Task<IActionResult> PostExpense(
    string txtFac, string txtDepartment, string txtTitle, string txtProblem,
    string txtRecommended, string strLowBid, string strHighBid, string strBidCount,
    string txtVendor, string strRecBid, string strTotal, string txtDate, string UID)
        {
            try
            {
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_CapitalExpense_Add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtFac;
                cmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = txtDepartment;
                cmd.Parameters.Add("@RequestTitle", SqlDbType.VarChar).Value = txtTitle;
                cmd.Parameters.Add("@ProblemHistory", SqlDbType.VarChar).Value = txtProblem;
                cmd.Parameters.Add("@RecommendedSolution", SqlDbType.VarChar).Value = txtRecommended;
                cmd.Parameters.Add("@LowBid", SqlDbType.Money).Value = strLowBid;
                cmd.Parameters.Add("@HighBid", SqlDbType.Money).Value = strHighBid;
                cmd.Parameters.Add("@BidCount", SqlDbType.Int).Value = strBidCount;
                cmd.Parameters.Add("@RecommendedVendor", SqlDbType.VarChar).Value = txtVendor;
                cmd.Parameters.Add("@RecommendedBid", SqlDbType.Money).Value = strRecBid;
                cmd.Parameters.Add("@EstimatedTotal", SqlDbType.Money).Value = strTotal;
                cmd.Parameters.Add("@CompletionDate", SqlDbType.Date).Value = txtDate;
                cmd.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "Capital Expense Report Submitted";
                string body = "A capital expense report was just submitted.<br/><br/>You can view the report <a href=\"https://pacs-technology.com/CapitalExpense\">HERE</a>.";

                // Send the email.
                await GraphService.SendEmail(graphClient, _env, "daniel.stump@pacshc.com", HttpContext, subject, body);

                return RedirectToAction("Index", new { strSave = "Success! Your record was saved." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }

        [HttpPost]
        public IActionResult PostExpenseEdit(
    string txtFac, string txtDepartment, string txtTitle, string txtProblem,
    string txtRecommended, string strLowBid, string strHighBid, string strBidCount,
    string txtVendor, string strRecBid, string strTotal, string txtDate, string UID, string passid)
        {
            try
            {
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_CapitalExpense_Edit", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtFac;
                cmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = txtDepartment;
                cmd.Parameters.Add("@RequestTitle", SqlDbType.VarChar).Value = txtTitle;
                cmd.Parameters.Add("@ProblemHistory", SqlDbType.VarChar).Value = txtProblem;
                cmd.Parameters.Add("@RecommendedSolution", SqlDbType.VarChar).Value = txtRecommended;
                cmd.Parameters.Add("@LowBid", SqlDbType.Money).Value = strLowBid;
                cmd.Parameters.Add("@HighBid", SqlDbType.Money).Value = strHighBid;
                cmd.Parameters.Add("@BidCount", SqlDbType.Int).Value = strBidCount;
                cmd.Parameters.Add("@RecommendedVendor", SqlDbType.VarChar).Value = txtVendor;
                cmd.Parameters.Add("@RecommendedBid", SqlDbType.Money).Value = strRecBid;
                cmd.Parameters.Add("@EstimatedTotal", SqlDbType.Money).Value = strTotal;
                cmd.Parameters.Add("@CompletionDate", SqlDbType.Date).Value = txtDate;
                cmd.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = UID;
                cmd.Parameters.Add("@PassID", SqlDbType.VarChar).Value = passid;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                return RedirectToAction("Index", new { strSave = "Success! Your record was saved." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> PostStatusChange(
    string strStatusNotes, string strStatus, string statusid)
        {
            try
            {
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_CapitalExpense_StatusChange", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@APPROVEDBY", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@APPROVEDEMAIL", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@APPROVALSTATUS", SqlDbType.VarChar).Value = strStatus;
                cmd.Parameters.Add("@APPROVENOTES", SqlDbType.VarChar).Value = strStatusNotes;
                cmd.Parameters.Add("@PassID", SqlDbType.VarChar).Value = statusid;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                SqlConnection con2 = new SqlConnection(connection);

                var sqlcommandtext = "Select * from capitalexpense where id = '" + statusid + "'";

                SqlCommand cmd2 = new SqlCommand(sqlcommandtext, con2);
                con2.Open();
                SqlDataReader idr = cmd2.ExecuteReader();

                string returntext = "";

                if (idr.HasRows)
                {
                    while (idr.Read())
                    {
                        returntext = Convert.ToString(idr["SUBMITEMAIL"]);
                    }
                }
                con2.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "You expense report has been " + strStatus;
                string body = "You expense report has been " + strStatus + ".<br/><br/>You can view the report <a href=\"https://pacs-technology.com//Expense\">HERE</a>.";

                //Send the email.
                await GraphService.SendEmail(graphClient, _env, returntext, HttpContext, subject, body);

                return RedirectToAction("Index", new { strSave = "Success! Your record was saved." });
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
    }
}
