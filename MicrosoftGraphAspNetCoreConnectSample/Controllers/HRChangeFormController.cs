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
    public class HRChangeFormController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public HRChangeFormController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> Index(string strSave)
        {
            await GraphService.GetUserJson(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext);
            string facilities = await operationlist();
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["oplist"] = "<select id=\"ddFacility\" class=\"txtbox\">" + facilities;
            ViewData["SharedHome"] = "<select id=\"ddSharedHome\" style=\"width: 280px!important\" class=\"txtbox\">" + facilities;
            ViewData["SharedShared"] = "<select id=\"ddSharedShared\" style=\"width: 280px!important\" class=\"txtbox\">" + facilities;
            ViewData["TransferHome"] = "<select id=\"ddTransferHome\" style=\"width: 280px!important\" class=\"txtbox\">" + facilities;
            ViewData["TransferTransfer"] = "<select id=\"ddTransferTransfer\" style=\"width: 280px!important\" class=\"txtbox\">" + facilities;
            ViewData["statelist"] = statelist();
            ViewData["HRCheck"] = await HRCheck();
            ViewData["UID"] = getGUID();
            ViewData["Message"] = strSave;
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
            ViewData["getdetails"] = await getdetails(passid);
            return View();
        }

        public async Task<string> HRCheck()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string returntext = "";

            if (response.Contains("HR Team") || response.Contains("Executives_SG"))
            {
                returntext = "<a href=\"/HRChangeForm/HRView\">Go to HR View</a>";
            }

            return returntext;
        }

        public async Task<string> getdetails(string passid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select *, CASE WHEN CompletedBy IS NULL THEN 'NO' ELSE 'YES' END AS 'COMPLETED' from HRChangeRequest where ID = '" + passid+"'";

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
                    prepaidtable += "<textarea id=\"txtNotes\" style=\"width:280px; height:100px\">"+ Convert.ToString(idr["completednotes"]) + "</textarea>";
                    prepaidtable += "</div>";
                    prepaidtable += "<div class=\"col-md-3\" style=\"padding-top:15px\">";

                    if (Convert.ToString(idr["completed"]) == "YES")
                    {
                        prepaidtable += "<div><input type=\"checkbox\" id=\"cbComplete\" checked=\"checked\" />&nbsp;&nbsp;Complete Request</div>";
                        prepaidtable += "<div class=\"textlabel\">Completed By: "+ Convert.ToString(idr["completedby"]) + "</div>";
                        prepaidtable += "<div><b>Completed: "+ Convert.ToString(idr["completeddate"]) + "</b></div>";
                    } else
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
                    prepaidtable += "<div>"+ Convert.ToString(idr["submitby"]) + "</div>";
                    prepaidtable += "</div>";
                    prepaidtable += "<div class=\"col-md-4\">";
                    prepaidtable += "<div class=\"textlabel\">Date Submitted</div>";
                    prepaidtable += "<div>"+ Convert.ToString(idr["submitdate"]) + "</div>";
                    prepaidtable += "</div>";
                    prepaidtable += "</div>";

                    prepaidtable += "<hr />";
                    prepaidtable += "<div class=\"row\">";
                    prepaidtable += "<div class=\"col-md-4\">";
                    prepaidtable += "<div class=\"textlabel\">Employee Name</div>";
                    prepaidtable += "<div>"+ Convert.ToString(idr["employeename"]) + "</div>";
                    prepaidtable += "<div class=\"textlabel\">Shared Employee</div>";
                    prepaidtable += "<div>"+ Convert.ToString(idr["sharedemployee"]) + "</div>";
                    prepaidtable += "</div>";
                    prepaidtable += "<div class=\"col-md-4\">";
                    prepaidtable += "<div class=\"textlabel\">Facility</div>";
                    prepaidtable += "<div>"+ Convert.ToString(idr["facility"]) + "</div>";
                    prepaidtable += "<div class=\"textlabel\">Supervisor Name</div>";
                    prepaidtable += "<div>"+ Convert.ToString(idr["supervisorname"]) + "</div>";
                    prepaidtable += "</div>";
                    prepaidtable += "</div>";
                    prepaidtable += "<div class=\"textlabel\">Aciton</div>";
                    prepaidtable += "<div>"+ Convert.ToString(idr["typeofaction"]) + "</div>";
                    prepaidtable += "<hr />";
                    prepaidtable += "<div class=\"row\">";
                    prepaidtable += "<div class=\"col-md-4\">";

                    if (Convert.ToString(idr["typeofaction"]) == "Address Change")
                    {
                        prepaidtable += "<div class=\"textlabel\">New Address</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option2"]) + "</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option3"]) + "</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option4"]) + "</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option5"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Name Change")
                    {
                        prepaidtable += "<div class=\"textlabel\">New Name</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Non-FMLA")
                    {
                        prepaidtable += "<div class=\"textlabel\">Effective Date</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Return Date</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option2"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Pay Rate Change")
                    {
                        prepaidtable += "<div class=\"textlabel\">Effective Date</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Rate</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option2"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Type</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option3"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Reason</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option4"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Phone Number Change")
                    {
                        prepaidtable += "<div class=\"textlabel\">New Phone Number</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Rehire")
                    {
                        prepaidtable += "<div class=\"textlabel\">Effective Date</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Job</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option2"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Department</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option3"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Rate</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option4"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Type</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option5"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Hours</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option6"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Shared")
                    {
                        prepaidtable += "<div class=\"textlabel\">Effective Date</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Home Facility</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option2"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Shared Facility</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option3"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Job</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option4"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Department</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option5"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Rate</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option6"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Type</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option7"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Hours</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option8"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Status Change")
                    {
                        prepaidtable += "<div class=\"textlabel\">Effective Date</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Change Type</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option2"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Job</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option3"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Department</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option4"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Rate</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option5"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Type</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option6"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Hours</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option7"]) + "</div>";
                    }

                    if (Convert.ToString(idr["typeofaction"]) == "Transfer")
                    {
                        prepaidtable += "<div class=\"textlabel\">Effective Date</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option1"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">From Facility</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option2"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">To Facility</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option3"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Job</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option4"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Department</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option5"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Rate</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option6"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Pay Type</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option7"]) + "</div>";
                        prepaidtable += "<div class=\"textlabel\">Hours</div>";
                        prepaidtable += "<div>" + Convert.ToString(idr["option8"]) + "</div>";
                    }


                    prepaidtable += "</div>";
                    prepaidtable += "<div class=\"col-md-4\">";
                    prepaidtable += await GetImagesnodel(Convert.ToString(idr["attachmentid"]));
                    prepaidtable += "</div>";
                    prepaidtable += "</div>";
                }
            }
            con.Close();

            return prepaidtable;
        }

        public string getrequests()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select ID, SubmitBy, SubmitDate, EmployeeName, Facility, SupervisorName, TypeOfAction, CASE WHEN CompletedBy IS NULL THEN 'NO' ELSE 'YES' END AS 'COMPLETED' from HRChangeRequest";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Submitted</th>";
            prepaidtable += "<th>Submitted By</th>";
            prepaidtable += "<th>Action</th>";
            prepaidtable += "<th>Supervisor</th>";
            prepaidtable += "<th>Employee</th>";
            prepaidtable += "<th>Facility</th>";
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
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["typeofaction"]), 50) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["supervisorname"]), 25) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["employeename"]), 25) + "</td>";
                    prepaidtable += "<td>" + trimstrings(Convert.ToString(idr["facility"]), 25) + "</td>";
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

        public async Task<string> operationlist()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "";

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

        public string statelist()
        {
            string operations = "<select id=\"ddStates\" style=\"width: 280px!important\" class=\"txtbox\">";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select statename from ds_states order by statename", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                operations += "<option></option>";
                while (idr.Read())
                {
                        operations += "<option>" + Convert.ToString(idr["statename"]) + "</option>";
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

        public async Task<string> GetImagesnodel(string stritem)
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
                        picturelist += "<td></td></tr></table></div>";
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


            return View("Index");

        }


        [HttpPost]
        public async Task<IActionResult> PostRequest(
    string strEmployee, string strFacility, string strShare, string strSup,
    string strAction, string strUID, string option1,
    string option2, string option3, string option4, string option5, string option6,
    string option7, string option8, string option9, string option10)
        {
            if (option1 is null){option1 = "";}
            if (option2 is null) { option2 = ""; }
            if (option3 is null) { option3 = ""; }
            if (option4 is null) { option4 = ""; }
            if (option5 is null) { option5 = ""; }
            if (option6 is null) { option6 = ""; }
            if (option7 is null) { option7 = ""; }
            if (option8 is null) { option8 = ""; }
            if (option9 is null) { option9 = ""; }
            if (option10 is null) { option10 = ""; }
            try
            {
                var username = User.Identity.Name;
                var email = User.FindFirst("preferred_username")?.Value;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_HRChangeForm_Add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@EmployeeName", SqlDbType.VarChar).Value = strEmployee;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strFacility;
                cmd.Parameters.Add("@SharedEmployee", SqlDbType.VarChar).Value = strShare;
                cmd.Parameters.Add("@SupervisorName", SqlDbType.VarChar).Value = strSup;
                cmd.Parameters.Add("@TypeOfAction", SqlDbType.VarChar).Value = strAction;
                cmd.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = strUID;
                cmd.Parameters.Add("@option1", SqlDbType.VarChar).Value = option1;
                cmd.Parameters.Add("@option2", SqlDbType.VarChar).Value = option2;
                cmd.Parameters.Add("@option3", SqlDbType.VarChar).Value = option3;
                cmd.Parameters.Add("@option4", SqlDbType.VarChar).Value = option4;
                cmd.Parameters.Add("@option5", SqlDbType.VarChar).Value = option5;
                cmd.Parameters.Add("@option6", SqlDbType.VarChar).Value = option6;
                cmd.Parameters.Add("@option7", SqlDbType.VarChar).Value = option7;
                cmd.Parameters.Add("@option8", SqlDbType.VarChar).Value = option8;
                cmd.Parameters.Add("@option9", SqlDbType.VarChar).Value = option9;
                cmd.Parameters.Add("@option10", SqlDbType.VarChar).Value = option10;


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "HR Change Request Submitted";
                string body = "A change request was just submitted.<br/><br/>You can view the report <a href=\"https://pacs-technology.com/HRChangeForm/HRView\">HERE</a>.";

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
                cmd = new SqlCommand("sp_HRChangeForm_SaveNotes", con);
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
