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
using Newtonsoft.Json.Linq;

using System.Text.Json;
using System.Text;

using System.Net.Http;
using System.Net.Http.Headers;

namespace PGWebFormsCore.Controllers
{
    public class SupportTicketController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public SupportTicketController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
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
            ViewData["UID"] = getGUID();
            ViewData["TicketType"] = getTicketType();
            ViewData["username"] = User.Identity.Name;
            ViewData["email"] = User.FindFirst("preferred_username")?.Value;
            ViewData["facility"] = await operationlist();
            //ViewData["facilityapi"] = await operationlistapi();
            return View();
        }

        [Authorize]
        public IActionResult Calendar()
        {
            ViewData["strtimes"] = docalendar();
            return View();
        }

        public string docalendar()
        {
            string strdate = "04/24/2021 1:00 pm";
            DateTime dtdate = DateTime.Parse(strdate);
            DateTime edtdate = dtdate.AddMinutes(15);

            
            string strmin = "";

            if (dtdate.Minute.ToString().Length == 1)
            {
                strmin = "0" + dtdate.Minute.ToString();
            } else
            {
                strmin = dtdate.Minute.ToString();
            }

            strdate = dtdate.Year.ToString() + "-" + dtdate.Month.ToString() + "-" + dtdate.Day.ToString() + "T";
            strdate += dtdate.Hour.ToString() + ":" + strmin + ":00";

            if (edtdate.Minute.ToString().Length == 1)
            {
                strmin = "0" + edtdate.Minute.ToString();
            }
            else
            {
                strmin = edtdate.Minute.ToString();
            }

            string stredate = edtdate.Year.ToString() + "-" + edtdate.Month.ToString() + "-" + edtdate.Day.ToString() + "T";
            stredate += edtdate.Hour.ToString() + ":" + strmin + ":00";
            //2021-04-23T12:30:00

            //await GraphService.SendCalendar(graphClient);
            return strdate + "    " + stredate;
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

        public string InsertGUID(string UID)
        {
            
            var username = User.Identity.Name;
            var email = User.FindFirst("preferred_username")?.Value;

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("IF ((SELECT COUNT(*) FROM SupportTicket WHERE IssueID = '"+UID+"') < 1) BEGIN INSERT INTO SUPPORTTICKET (ISSUEID, LOGDATE, USERNAME, EMAIL) VALUES ('" + UID+"', GETDATE(), '"+username+"', '"+email+"') END", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return UID;
        }

        public string getTicketType()
        {
            string operations = "<select id=\"ddType\" onchange=\"typechange(this.value, this.options[this.selectedIndex].text)\" style=\"width: 280px!important\" class=\"txtbox\">";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select ticketIssueType, case when ticketIssueTypeAction = 'email' then ticketIssueTypeTarget + '$$$' + cast(t2tickets as varchar(1)) else ticketIssueTypeTarget end as 'ticketissuetypetarget' from ticketIssueTypes", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                operations += "<option value=\"0\"></option>";
                while (idr.Read())
                {
                        operations += "<option value=\""+ Convert.ToString(idr["ticketissuetypetarget"]) + "\">" + Convert.ToString(idr["ticketissuetype"]) + "</option>";
                }
            }
            con.Close();

            operations += "</select>";
            return operations;
        }

        public async Task<string> operationlist()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var response = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "<select id=\"ddFacility\" style=\"width: 280px!important\" class=\"txtbox\">";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select operationname from operations where operationname not like '%- gl' union select 'Headquarters' union select 'PACS' order by operationname", con);
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

        public class Ops
        {
            public int operationId { get; set; }
            public string operationName { get; set; }
        }

        public async Task<string> operationlistapi()
        {
            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);
            var email = User.FindFirst("preferred_username")?.Value;

            var responseg = await GraphService.GetUserGroups(graphClient, email, HttpContext);
            string operations = "<select id=\"ddFacility\" style=\"width: 280px!important\" class=\"txtbox\">";

            // Create a New HttpClient object.
            HttpClient client = new HttpClient();

            // Call asynchronous network methods in a try/catch block to handle exceptions
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://pacs-api.azurewebsites.net/operations");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //JArray json = JArray.Parse(responseBody);
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Ops[] operationsarray = JsonConvert.DeserializeObject<Ops[]>(responseBody);

                foreach (Ops item in operationsarray)
                {
                    Console.WriteLine(item.operationName);
                }

                operations = responseBody;
                //Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {

            }

            // Need to call dispose on the HttpClient object
            // when done using it, so the app doesn't leak resources
            client.Dispose();

            //var connection = _configuration.GetConnectionString("pgWebForm");
            //SqlConnection con = new SqlConnection(connection);
            //SqlCommand cmd = new SqlCommand("select operationname from operations union select 'Headquarters' order by operationname", con);
            //con.Open();
            //SqlDataReader idr = cmd.ExecuteReader();

            //string strSelect = "";

            //if (idr.HasRows)
            //{
            //    operations += "<option></option>";
            //    while (idr.Read())
            //    {
            //        if (response.Contains(Convert.ToString(idr["operationname"])) && strSelect == "")
            //        {
            //            operations += "<option selected=\"selected\">" + Convert.ToString(idr["operationname"]) + "</option>";
            //            strSelect = "select";
            //        }
            //        else
            //        {
            //            operations += "<option>" + Convert.ToString(idr["operationname"]) + "</option>";
            //        }


            //    }
            //}
            //con.Close();

            //operations += "</select>";
            return operations;

        }

        public string TypeChange(string strid, string stroption)
        {
            InsertGUID(strid);

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("UPDATE SUPPORTTICKET SET ISSUE = '"+stroption+"' WHERE ISSUEID = '"+strid+"'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            
            return "";
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

        public async Task<string> GetImagesLink(string stritem)
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
                        picturelist += "<a href=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + actualname + "\">"+actualname+"</a><br/>";
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
        public async Task<IActionResult> PostTicket(
            string strFacility, string txtName, string txtEmail, string txtPhone,
            string strContactM, string txtContactDT, string txtShared, string txtMore,
            string strNotes, string strType, string strTypeAction, string strUID, 
            string txttimezone, string txtSurvey, string txtSkip)
        {
            try { 
            if (txtMore == "Yes")
            {
                txtMore = "True";
            } else
            {
                txtMore = "False";
            }

            if (txtShared == "Yes")
            {
                txtShared = "True";
            } else
            {
                txtShared = "False";
            }

                if (txtSurvey == "Yes")
                {
                    txtSurvey = "True";
                }
                else
                {
                    txtSurvey = "False";
                }

                if (txtSkip == "Yes")
                {
                    txtSkip = "True";
                }
                else
                {
                    txtSkip = "False";
                }

                if (txtContactDT is null)
                {
                    txtContactDT = "";
                }



                string nexttech = "";
                if (strContactM == "Phone" && txtContactDT != "" && txtSkip == "False")
                {
                    nexttech = getnexttech();
                }

                var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_SupportTicket_Add", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UID", SqlDbType.VarChar).Value = strUID;
            cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = strFacility;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = txtName;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = txtEmail;
            cmd.Parameters.Add("@Phone", SqlDbType.VarChar).Value = txtPhone;
            cmd.Parameters.Add("@ContactM", SqlDbType.VarChar).Value = strContactM;
            cmd.Parameters.Add("@ContactDT", SqlDbType.VarChar).Value = txtContactDT;
            cmd.Parameters.Add("@Performance", SqlDbType.Bit).Value = txtShared;
            cmd.Parameters.Add("@More", SqlDbType.Bit).Value = txtMore;
                cmd.Parameters.Add("@Survey", SqlDbType.Bit).Value = txtSurvey;
                cmd.Parameters.Add("@Skip", SqlDbType.Bit).Value = txtSkip;
                cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = strNotes;
                cmd.Parameters.Add("@TimeZone", SqlDbType.VarChar).Value = txttimezone;
                cmd.Parameters.Add("@ContactTech", SqlDbType.VarChar).Value = nexttech;

                con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

            string subject = "Support Ticket--"+strType;
            string body = "";

                body += "<b>Web ID:</b> " + strUID + "<br/>";
                body += "<b>Type of Issue:</b> " + strType + "<br/>";
            body += "<b>Facility:</b> " + strFacility + "<br/>";
            body += "<b>Name:</b> " + txtName + "<br/>";
            body += "<b>Email:</b> " + txtEmail + "<br/>";
            body += "<b>Phone:</b> " + txtPhone + "<br/>";
            body += "<b>Preferred Contact Method:</b> " + strContactM + "<br/>";
            body += "<b>Best time to contact:</b> " + txtContactDT + " " + txttimezone + "<br/>";
            body += "<b>Interfering with job performance:</b> " + txtShared + "<br/>";
            body += "<b>Impacting more than just them:</b> " + txtMore + "<br/>";

            string getimages = await GetImagesLink(strUID);

            if (getimages != "")
            {
                body += "<b>Attachments</b><br/>";
                body += getimages;
            }

            body += "<b>Details:</b> " + strNotes + "<br/>";

                string[] splitaction = strTypeAction.Split("$$$");
                string sendemailto = splitaction[0];

            await GraphService.SendEmail(graphClient, _env, "support@fitsolutions.biz", HttpContext, subject, body);

                if (strContactM == "Phone" && txtContactDT != "" && txtSkip == "False")
                {

                    body += "<br/><br/>";
                    body += "<b>Assigned Tech:</b> " + nexttech + "<br/>";

                    string strdate = txtContactDT;
                    DateTime dtdate = DateTime.Parse(strdate);
                    DateTime edtdate = dtdate.AddMinutes(15);


                    string strmin = "";
                    string strhour = "";

                    if (dtdate.Minute.ToString().Length == 1)
                    {
                        strmin = "0" + dtdate.Minute.ToString();
                    }
                    else
                    {
                        strmin = dtdate.Minute.ToString();
                    }

                    if (dtdate.Hour.ToString().Length == 1)
                    {
                        strhour = "0" + dtdate.Hour.ToString();
                    } else
                    {
                        strhour = dtdate.Hour.ToString();
                    }

                    strdate = dtdate.Year.ToString() + "-" + dtdate.Month.ToString() + "-" + dtdate.Day.ToString() + "T";
                    strdate += strhour + ":" + strmin + ":00";

                    if (edtdate.Minute.ToString().Length == 1)
                    {
                        strmin = "0" + edtdate.Minute.ToString();
                    }
                    else
                    {
                        strmin = edtdate.Minute.ToString();
                    }

                    if (edtdate.Hour.ToString().Length == 1)
                    {
                        strhour = "0" + edtdate.Hour.ToString();
                    }
                    else
                    {
                        strhour = edtdate.Hour.ToString();
                    }


                    string stredate = edtdate.Year.ToString() + "-" + edtdate.Month.ToString() + "-" + edtdate.Day.ToString() + "T";
                    stredate += strhour + ":" + strmin + ":00";
                    

                    await GraphService.SendCalendar(graphClient, body, strdate, stredate, txttimezone, "PACSHelpdesk@providencegroup.onmicrosoft.com", nexttech);
                }
                

                return RedirectToAction("Index", new { strSave = "Success! Your support ticket was submitted." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }
        }

        public string getnexttech()
        {

            string operations = "";
            string sqltext = "";
            sqltext += "select ";
            sqltext += "case when ";
            sqltext += "(select top 1 TechNumber from SupportTicketTechs order by TechNumber desc) ";
            sqltext += "= ";
            sqltext += "(select technumber from SupportTicketTechs where TechEmail = ( ";
            sqltext += "select top 1 contacttech from SupportTicket where ContactTech is not null and ContactTech <> '' order by LogDate desc)) ";
            sqltext += "then ";
            sqltext += "(select techemail from SupportTicketTechs where TechNumber = 1) ";
            sqltext += "else ";
            sqltext += "(select techemail from SupportTicketTechs where TechNumber = ";
            sqltext += "(select technumber + 1 from SupportTicketTechs where TechEmail = ( ";
            sqltext += "select top 1 contacttech from SupportTicket where ContactTech is not null  and ContactTech <> '' order by LogDate desc))) ";
            sqltext += "end ";
            sqltext += "as nexttech ";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(sqltext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                        operations = Convert.ToString(idr["nexttech"]);
                }
            }
            con.Close();


            return operations;

        }

    }
}
