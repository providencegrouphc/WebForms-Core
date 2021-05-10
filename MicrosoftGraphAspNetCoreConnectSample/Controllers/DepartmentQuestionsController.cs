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
    public class DepartmentQuestionsController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public DepartmentQuestionsController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
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
            ViewData["UID"] = getGUID();
            ViewData["department"] = getDepartments();
            return View();
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

        public string getDepartments()
        {
            string operations = "<select id=\"ddDepartment\" onchange=\"typechange(this.value, this.options[this.selectedIndex].text)\" style=\"width: 280px!important\" class=\"txtbox\">";

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from DepartmentQuestions_List ORDER BY department", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            if (idr.HasRows)
            {
                operations += "<option value=\"0\"></option>";
                while (idr.Read())
                {
                    operations += "<option value=\"" + Convert.ToString(idr["departmentemail"]) + "\">" + Convert.ToString(idr["department"]) + "</option>";
                }
            }
            con.Close();

            operations += "</select>";
            return operations;
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
                        picturelist += "<a href=\"https://pgcorestorage.blob.core.windows.net/" + stritem + "/" + actualname + "\">" + actualname + "</a><br/>";
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
            string txtName, string txtEmail, string txtPhone, string strType,
            string strTypeAction, string strNotes, string strUID)
        {
            try
            {
                if (txtName is null){txtName = "";}
                if (txtEmail is null) { txtEmail = ""; }
                if (txtPhone is null) { txtPhone = ""; }

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_DepartmentQuestions_Add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubmitBy", SqlDbType.VarChar).Value = txtName;
                cmd.Parameters.Add("@SubmitEmail", SqlDbType.VarChar).Value = txtEmail;
                cmd.Parameters.Add("@SubmitPhone", SqlDbType.VarChar).Value = txtPhone;
                cmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = strType;
                cmd.Parameters.Add("@Question", SqlDbType.VarChar).Value = strNotes;
                cmd.Parameters.Add("@AttachmentID", SqlDbType.VarChar).Value = strUID;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                var graphClient = _graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity);

                string subject = "Department Question -- " + strType;
                string body = "";

                body += "<b>Name:</b> " + txtName + "<br/>";
                body += "<b>Phone:</b> " + txtPhone + "<br/>";
                body += "<b>Email:</b> " + txtEmail + "<br/>";
                body += "<b>Question:</b><br/>";
                body += strNotes;
                body += "<br/><br/>";
                string getimages = await GetImagesLink(strUID);

                if (getimages != "")
                {
                    body += "<b>Attachments</b><br/>";
                    body += getimages;
                }



                await GraphService.SendEmail(graphClient, _env, "daniel.stump@pacshc.com", HttpContext, subject, body);


                return RedirectToAction("Index", new { strSave = "Success! Your question was submitted." });
            }
            catch (ServiceException se)
            {
                return RedirectToAction("Error", "Home", new { message = "Error: " + se.Error.Message });
            }
        }
    }
}
