using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using System.IO;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Graph;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using MicrosoftGraphAspNetCoreConnectSample.Services;
using Microsoft.Extensions.Configuration;



namespace PGWebFormsCore.Controllers
{
    [Route("api/pdfcreator")]
    [ApiController]
    public class PdfCreatorController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;
        private IConverter _converter;

        public PdfCreatorController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration, IConverter converter)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
            _converter = converter;
        }

        [HttpGet]
        public IActionResult CreatePDF(string report, string passid, string passid2)
        {
            string htmltext = "";
            string footer = "";
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.Letter,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report"
            };

            if (report == "notes")
            {
                footer = "";
                htmltext = getnotes(passid);

                globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.Letter,
                    Margins = new MarginSettings { Top = 10, Bottom = 50, Left = 10, Right = 10 },
                    DocumentTitle = "PDF Report"
                };
            }

            if (report == "tclist")
            {
                footer = "";
                htmltext = gettclist(passid, passid2);

                globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.Letter,
                    Margins = new MarginSettings { Top = 10, Bottom = 50, Left = 10, Right = 10 },
                    DocumentTitle = "PDF Report"
                };
            }


            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmltext,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 8, Line = true, Center = footer, Spacing = 30, HtmUrl = "https://pacs-technology.com/TripleCheck/pdffooter" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }


        public string getnotes(string passid)
        {
            string htmltext = "";
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select * from TripleCheck_Records where id = '" + passid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();



            if (idr.HasRows)
            {

                while (idr.Read())
                {
                    htmltext += "<h1>" + Convert.ToString(idr["firstname"]) + " " + Convert.ToString(idr["lastname"]) + " -- " + Convert.ToString(idr["medicalid"]) + "</h1>";
                    htmltext += "<h1>" + Convert.ToString(idr["reportmonth"]) + "</h1>";
                    htmltext += "<h3>Notes</h3>";
                    htmltext += Convert.ToString(idr["notes"]);
                }
            }
            con.Close();

            return htmltext;
        }

        public string gettclist(string facilityid, string intmonth)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from TripleCheck_Records r inner join operations o on r.FacilityID = o.operationId where FacilityID = '" + facilityid + "' and INTMonth = '" + intmonth + "' and deleted = 0";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string reportmonth = "";

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%; text-align:left\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
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
                    reportmonth = "<h1>" + Convert.ToString(idr["operationName"]) +  "</h1><h1>" + Convert.ToString(idr["reportmonth"]) + "<h1><hr>";
                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["medicalid"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["firstname"]) + " " + Convert.ToString(idr["lastname"]) + "</td>";

                    DateTime staystart = Convert.ToDateTime(idr["startstay"]);
                    prepaidtable += "<td>" + staystart.ToShortDateString() + "</td>";

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

                    if (Convert.ToString(idr["notes"]) == "")
                    {
                        prepaidtable += "<tr><td colspan=\"11\"><hr /></td></tr>";
                    } else
                    {
                        prepaidtable += "<tr><td colspan=\"11\"><b>Notes:</b><br/>" + Convert.ToString(idr["notes"]) + "<hr/></td></tr>";
                    }
                    
                }
            }
            con.Close();

            prepaidtable += "</tbody></table>";

            return reportmonth + prepaidtable;

        }
    }
}
