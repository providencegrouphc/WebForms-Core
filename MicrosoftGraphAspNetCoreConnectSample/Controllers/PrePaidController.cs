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



namespace MicrosoftGraphAspNetCoreConnectSample.Controllers
{
    public class PrePaidController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly IGraphServiceClientFactory _graphServiceClientFactory;
        private readonly IConfiguration _configuration;

        public PrePaidController(IWebHostEnvironment hostingEnvironment, IGraphServiceClientFactory graphServiceClientFactory, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _graphServiceClientFactory = graphServiceClientFactory;
            _configuration = configuration;
        }


        [Authorize]
        public async Task<IActionResult> Index(string strSave)
        {
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "PrePaid");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            ViewData["prepaidgetyears"] = getyear();
            ViewData["prepaidgetmonths"] = getmonth();
            ViewData["prepaidtable"] = getprepaidtable();
            ViewData["Message"] = strSave;
            return View();
        }

        public async Task<IActionResult> Edit(string prepaidid)
        {
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            if (prepaidid == null)
            {
                return RedirectToAction("Index");
            }
            string[] geteditlist = GetEditList(prepaidid);
            prepaidedit newprepaidedit = new prepaidedit
            {
                ID = geteditlist[0],
                balance = geteditlist[1],
                facility = geteditlist[2],
                amount = geteditlist[3],
                invoiceduedate = geteditlist[4],
                paid = geteditlist[5],
                expectedreceiptdate = geteditlist[6],
                beginamortizationdate = geteditlist[7],
                monthsamortized = totalmonths(prepaidid),
                vendor = geteditlist[9],
                typeoflicense = geteditlist[10],
                glcode = geteditlist[11],
                notes = geteditlist[12]
            };
            ViewData["prepaidpaymentamount"] = getpaymentamount(prepaidid);
            ViewData["prepaidgetpayments"] = getpayments(prepaidid);
            ViewData["prepaidadditions"] = getadditions(prepaidid);
            ViewBag.Message = newprepaidedit;
            return View();
        }

        public string[] GetEditList(string prepaidid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select *, ISNULL(ROUND((SELECT SUM(AMOUNT) FROM PrePaidPaymentSchedule WHERE PREPAIDID = '"+ prepaidid + "' AND PaymentDate > GETDATE()), 3), '0.00') AS BALANCE from prepaid where ID = '"+ prepaidid + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string[] editlist = new string[13];
            
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    editlist[0] = Convert.ToString(idr["ID"]);
                    decimal balance = Convert.ToDecimal(idr["balance"]);
                    editlist[1] = balance.ToString("C3");
                    editlist[2] = Convert.ToString(idr["facility"]);
                    decimal amount = Convert.ToDecimal(idr["amount"]);
                    editlist[3] = amount.ToString("C3");
                    DateTime invoiceduedate = Convert.ToDateTime(idr["invoiceduedate"]);
                    editlist[4] = invoiceduedate.ToShortDateString();
                    editlist[5] = Convert.ToString(idr["paid"]);
                    DateTime expectedreceiptdate = Convert.ToDateTime(idr["expectedreceiptdate"]);
                    editlist[6] = expectedreceiptdate.ToShortDateString();
                    DateTime beginamortizationdate = Convert.ToDateTime(idr["beginamortizationdate"]);
                    editlist[7] = beginamortizationdate.ToShortDateString();
                    editlist[8] = Convert.ToString(idr["howmanymonthsamortized"]);
                    editlist[9] = Convert.ToString(idr["vendor"]);
                    editlist[10] = Convert.ToString(idr["typeoflicense"]);
                    editlist[11] = Convert.ToString(idr["glcode"]);
                    editlist[12] = Convert.ToString(idr["notes"]);
                }
            }
            con.Close();
            return editlist;
        }

        public string getpayments(string prepaidid)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select id, PaymentDate, Amount, isnull(InvoiceNumer, '') as invoicenumber, isnull(checknumber, '') as checknumber, isnull(trackingnumber, '') as trackingnumber from PrePaidPaymentSchedule where PREPAIDID = '" + prepaidid + "' order by PaymentDate";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string paymenttable = "<div class=\"tableFixHead\"><table id=\"paymenttable\"  >";
            paymenttable += "<thead>";
            paymenttable += "<tr>";
            paymenttable += "<th>Payment Date</th>";
            paymenttable += "<th>Amount Paid</th>";
            paymenttable += "<th>Invoice #</th>";
            paymenttable += "<th>Check #</th>";
            paymenttable += "<th>Tracking #</th>";
            paymenttable += "</tr>";
            paymenttable += "</thead>";
            paymenttable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    decimal amount = Convert.ToDecimal(idr["amount"]);
                    DateTime paymentdate = Convert.ToDateTime(idr["Paymentdate"]);
                    paymenttable += "<tr onclick=\"showpayment('" + Convert.ToString(idr["id"]) + "', '"+ amount.ToString("C3").Trim() + "', '"+ paymentdate.ToShortDateString().Trim() + "', '"+ Convert.ToString(idr["invoicenumber"]) + "', '"+ Convert.ToString(idr["checknumber"]) + "', '"+ Convert.ToString(idr["trackingnumber"]) + "');\">";
                    paymenttable += "<td>" + paymentdate.ToShortDateString().Trim() + "</td>";
                    paymenttable += "<td>" + amount.ToString("C3").Trim() + "</td>";
                    paymenttable += "<td>" + Convert.ToString(idr["invoicenumber"]) + "</td>";
                    paymenttable += "<td>" + Convert.ToString(idr["checknumber"]) + "</td>";
                    paymenttable += "<td>" + Convert.ToString(idr["trackingnumber"]) + "</td>";
                    paymenttable += "</tr>";
                }
            }
            con.Close();

            paymenttable += "</tbody></table></div>";

            return paymenttable;
        }

        public string getadditions(string prepaidid)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "select * from PrePaidAdditions where PREPAIDID = '" + prepaidid + "' order by LoggedDate desc";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string paymenttable = "<table id=\"additonstable\" class=\"fixed_header\" style=\"margin-bottom:10px; \">";
            paymenttable += "<thead>";
            paymenttable += "<tr>";
            paymenttable += "<th>Amount</th>";
            paymenttable += "<th>Reason</th>";
            paymenttable += "<th>Logged By</th>";
            paymenttable += "<th>Logged Date</th>";
            paymenttable += "</tr>";
            paymenttable += "</thead>";
            paymenttable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    paymenttable += "<tr>";

                    decimal amount = Convert.ToDecimal(idr["amount"]);
                    paymenttable += "<td>" + amount.ToString("C3") + "</td>";
                    paymenttable += "<td>" + Convert.ToString(idr["reason"]) + "</td>";
                    paymenttable += "<td>" + Convert.ToString(idr["loggedby"]) + "</td>";
                    DateTime paymentdate = Convert.ToDateTime(idr["loggeddate"]);
                    paymenttable += "<td>" + paymentdate.ToShortDateString() + "</td>";
                    paymenttable += "</tr>";
                }
            } else
            {
                paymenttable += "<tr><td>Table has no data</td><tr>";
            }
            con.Close();

            paymenttable += "</tbody></table>";

            return paymenttable;
        }


        public string getpaymentamount(String PREPAIDID)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select distinct amount from PrePaidPaymentSchedule where PREPAIDID = '" + PREPAIDID + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string paymentamounts = "<select id=\"ddpaymentamount\" class=\"txtbox\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    
                    decimal balance = Convert.ToDecimal(idr["amount"]);
                    paymentamounts += "<option>"+balance.ToString("C3") + "</option>";
                }
            }
            con.Close();

            paymentamounts += "</select>";
            return paymentamounts;

            
        }

        public string getmonth()
        {
            int intmonth = DateTime.Now.Month;

            var ddmonth = "<select id=\"ddmonth\">";

            if (intmonth == 1)
            { ddmonth += "<option selected=\"selected\">1</option>"; }
            else
            { ddmonth += "<option>1</option>"; }

            if (intmonth == 2 )
            {ddmonth += "<option selected=\"selected\">2</option>";}
            else
            {ddmonth += "<option>2</option>";}

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
            ddyear += "<option>" + Convert.ToString(intyear + 1) + "</option>";
            ddyear += "<option>" + Convert.ToString(intyear + 2) + "</option>";
            ddyear += "<option>" + Convert.ToString(intyear + 3) + "</option>";
            ddyear += "<option>" + Convert.ToString(intyear + 4) + "</option>";


            ddyear += "</select>";
            return ddyear;
        }

        public string getprepaidtable()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "SELECT ID, Facility, Vendor, TypeOfLicense,";
            sqlcommandtext += "ISNULL(ROUND((SELECT SUM(AMOUNT) FROM PrePaidPaymentSchedule WHERE PREPAIDID = P.ID AND PaymentDate > GETDATE()), 3), '0.00') AS BALANCE,";
            sqlcommandtext += "ISNULL(ROUND((SELECT TOP 1 AMOUNT FROM PrePaidPaymentSchedule WHERE PREPAIDID = P.ID AND PaymentDate > GETDATE()), 3), '0.00') AS MONTHLYDUE,";
            sqlcommandtext += "(SELECT TOP 1 PAYMENTDATE FROM PrePaidPaymentSchedule WHERE PREPAIDID = P.ID ORDER BY PaymentDate DESC) AS ENDDATE,";
            sqlcommandtext += "CASE WHEN(SELECT COUNT(*) FROM PrePaidPaymentSchedule WHERE PREPAIDID = P.ID AND PaymentDate > GETDATE()) > 0 THEN 'YES' ELSE 'NO' END AS 'CURRENT'";
            sqlcommandtext += "FROM PrePaid P where deleted = 0";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();

            string prepaidtable = "<table id=\"example\" class=\"display\" style=\"width:100%\">";
            prepaidtable += "<thead>";
            prepaidtable += "<tr>";
            prepaidtable += "<th>ID</th>";
            prepaidtable += "<th>Facility</th>";
            prepaidtable += "<th>Vendor</th>";
            prepaidtable += "<th>Type Of License</th>";
            prepaidtable += "<th>Balance</th>";
            prepaidtable += "<th>Monthly Due</th>";
            prepaidtable += "<th>End Date</th>";
            prepaidtable += "<th>Current</th>";
            prepaidtable += "</tr>";
            prepaidtable += "</thead>";
            prepaidtable += "<tbody>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    prepaidtable += "<tr>";
                    prepaidtable += "<td>" + Convert.ToString(idr["id"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["facility"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["vendor"]) + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["typeoflicense"]) + "</td>";
                   decimal balance = Convert.ToDecimal(idr["balance"]);
                    prepaidtable += "<td>" + balance.ToString("C3") + "</td>";
                    decimal monthlydue = Convert.ToDecimal(idr["monthlydue"]);
                    prepaidtable += "<td>" + monthlydue.ToString("C3") + "</td>";
                    DateTime enddate = Convert.ToDateTime(idr["enddate"]);
                    prepaidtable += "<td>" + enddate.ToShortDateString() + "</td>";
                    prepaidtable += "<td>" + Convert.ToString(idr["current"]) + "</td>";
                    prepaidtable += "</tr>";
                }
            }
            con.Close();

            prepaidtable += "</tbody></table>";

            return prepaidtable;
        }

        public List<Operation_List> GetOperationList()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("Select operationName From operations order by operationName", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            List<Operation_List> operation = new List<Operation_List>();
            operation.Insert(0, new Operation_List { operationName = "" });
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    operation.Add(new Operation_List
                    {
                        operationName = Convert.ToString(idr["operationName"])
                    });
                }
            }
            con.Close();
            return operation;
        }

        public List<PrePaidVendors_List> GetPrePaidVendorsList()
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT VENDORNAME FROM PREPAIDVENDORS", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            List<PrePaidVendors_List> vendor = new List<PrePaidVendors_List>();
            vendor.Insert(0, new PrePaidVendors_List { PrePaidVendorsName = "" });
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    vendor.Add(new PrePaidVendors_List
                    {
                        PrePaidVendorsName = Convert.ToString(idr["VENDORNAME"])
                    });
                }
            }
            con.Close();
            return vendor;
        }

        public string VendorChange(string stritem)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("SELECT TypeOfLicense FROM PREPAIDVENDORS WHERE VendorName = '"+stritem+"'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            var returndata = "";
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    returndata = returndata + Convert.ToString(idr["TypeOfLicense"]) + ",";
                }
            } else {
                returndata = returndata + ",";
            }
            con.Close();

            return returndata.Remove(returndata.Length -1, 1);
        }

        [HttpPost]
        public IActionResult GenReport(string txtMonth, string txtYear)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            tw.WriteLine("FACILITY,REFERENCE_DESCRIPTION,ACCOUNT,DESCRIPTION,DEBIT,CREDIT,EFFECTIVE_DATE,FISCAL_YEAR,FISCAL_PERIOD");

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);

            var sqlcommandtext = "SELECT Facility, REPLACE(LEFT(GLCode, CHARINDEX(' ', GLCode) - 1), ' ', '') AS ACCOUNT,";
            sqlcommandtext += "LTRIM(REPLACE(SUBSTRING(GLCode, CHARINDEX(' ', GLCode), LEN(GLCode)), GLCode, ' ')) AS 'DESCRIPTION',";
            sqlcommandtext += "S.Amount, PaymentDate, YEAR(PAYMENTDATE) AS 'YEAR', MONTH(PAYMENTDATE) AS 'MONTH'";
            sqlcommandtext += " FROM PrePaid P INNER JOIN PrePaidPaymentSchedule S ON P.ID = S.PREPAIDID";
            sqlcommandtext += " WHERE YEAR(PAYMENTDATE) = '"+txtYear+"' AND MONTH(PAYMENTDATE) = '"+txtMonth+"' and deleted = 0";

            SqlCommand cmd = new SqlCommand(sqlcommandtext, con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            if (idr.HasRows)
            {
                while (idr.Read())
                {
                    var wrline = Convert.ToString(idr["facility"]) + ",";
                    wrline += ",";
                    wrline += Convert.ToString(idr["account"]) + ",";
                    wrline += Convert.ToString(idr["description"]) + ",";
                    var monthlydue = Convert.ToString(idr["amount"]);
                    wrline += monthlydue.Remove(monthlydue.Length - 1, 1) + ",";
                    wrline += ",";
                    DateTime enddate = Convert.ToDateTime(idr["paymentdate"]);
                    wrline += enddate.ToShortDateString() + ",";
                    wrline += Convert.ToString(idr["year"]) + ",";
                    wrline += Convert.ToString(idr["month"]);

                    tw.WriteLine(wrline);
                }
            }
            con.Close();

            tw.Flush();
            tw.Close();

            return File(memoryStream.GetBuffer(), "text/plain", txtMonth + "_" + txtYear + "_" + "PREPAID.csv");
        }

        [Authorize]
        public async Task<IActionResult> New()
        {
            ViewData["checkauth"] = await GraphService.GetAuth(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"), "PrePaid");
            ViewData["sidebar"] = await GraphService.GetSideBar(_graphServiceClientFactory.GetAuthenticatedGraphClient((ClaimsIdentity)User.Identity), User.FindFirst("preferred_username")?.Value, HttpContext, _configuration.GetConnectionString("pgWebForm"));
            PrePaidObjects PassPrePaidObjects = new PrePaidObjects
                {
                operationlist = GetOperationList(),
                PrePaidVendorslist = GetPrePaidVendorsList()
            };

            return View(PassPrePaidObjects);
        }

        [HttpPost]
        public IActionResult PostNew(string txtFacility, string DueDate, string ExpectedReceipt, string BeginAmortization, string txtMonths, string txtVendors, string txtType, string txtGL, string txtNotes, string txtPaid, string txtNewAmount)
        {
            try
            {
                if (txtNotes is null)
                {
                    txtNotes = "";
                }
                var username = User.Identity.Name;

                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_PrePaid_Add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@Facility", SqlDbType.VarChar).Value = txtFacility;
                cmd.Parameters.Add("@InvoiceDueDate", SqlDbType.Date).Value = DueDate;
                cmd.Parameters.Add("@Paid", SqlDbType.Bit).Value = txtPaid;
                cmd.Parameters.Add("@ExpectedReceiptDate", SqlDbType.Date).Value = ExpectedReceipt;
                cmd.Parameters.Add("@BeginAmortizationDate", SqlDbType.Date).Value = BeginAmortization;
                cmd.Parameters.Add("@HowManyMonthsAmortized", SqlDbType.Int).Value = txtMonths;
                cmd.Parameters.Add("@Vendor", SqlDbType.VarChar).Value = txtVendors;
                cmd.Parameters.Add("@TypeOfLicense", SqlDbType.VarChar).Value = txtType;
                cmd.Parameters.Add("@GLCode", SqlDbType.VarChar).Value = txtGL;
                cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = txtNotes;
                cmd.Parameters.Add("@Amount", SqlDbType.VarChar).Value = txtNewAmount;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

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
            
            return RedirectToAction("Edit", new { prepaidid = txtID });

        }

        public string ChangeNotes(string stritem, string strid)
        {
            try
            {
                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_PrePaid_Notes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = strid;
                cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = stritem;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return stritem;
            }
            catch (ServiceException se)
            {
                return se.Error.Message;
            }

        }

        public string AddPayment(string strid, string paymentid, string invoice, string check, string tracking)
        {

            try
            {
                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_PrePaid_Payment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@prepaidID", SqlDbType.VarChar).Value = paymentid;
                cmd.Parameters.Add("@invoice", SqlDbType.VarChar).Value = invoice;
                cmd.Parameters.Add("@check", SqlDbType.VarChar).Value = check;
                cmd.Parameters.Add("@tracking", SqlDbType.VarChar).Value = tracking;
                cmd.Parameters.Add("@loggedby", SqlDbType.VarChar).Value = User.Identity.Name;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                

                return getpayments(strid);
            }
            catch (ServiceException se)
            {
                return se.Message;
            }

            
        }

        public string ChangeMonths(string strid, int strmonths)
        {

            try
            {
                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_PrePaid_ChangeMonths", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@prepaidID", SqlDbType.VarChar).Value = strid;
                cmd.Parameters.Add("@newmonths", SqlDbType.Int).Value = strmonths;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();



                return getpayments(strid);
            }
            catch (ServiceException se)
            {
                return se.Message;
            }


        }

        public string totalmonths(string strid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select count(*) from prepaidpaymentschedule where prepaidid = '"+strid+"'", con);
            con.Open();
            string nummonths = cmd.ExecuteScalar().ToString();
            con.Close();

            return nummonths;
        }

        public string DelRecord(string strid)
        {
            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("update prepaid set deleted = 1 where id = '" + strid + "'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return "";
        }

        public string AddAdditional(string strid, string amountpaid, string reason)
        {

            try
            {
                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("sp_PrePaid_Additions", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@prepaidID", SqlDbType.VarChar).Value = strid;
                cmd.Parameters.Add("@newamount", SqlDbType.Money).Value = amountpaid;
                cmd.Parameters.Add("@reason", SqlDbType.VarChar).Value = reason;
                cmd.Parameters.Add("@loggedby", SqlDbType.VarChar).Value = User.Identity.Name;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                

                return getadditions(strid);
            }
            catch (ServiceException se)
            {
                return se.Message;
            }


        }

        public string getbalance(string PREPAIDID)
        {

            try
            {
                string balance = "Remaining Balance: ";
                var connection = _configuration.GetConnectionString("pgWebForm");
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("select sum(amount) from PrePaidPaymentSchedule where PREPAIDID = '"+ PREPAIDID+"' and PaymentDate > GETDATE()", con);
                con.Open();
                decimal amount = Convert.ToDecimal(cmd.ExecuteScalar());
                balance += amount.ToString("C3");
                con.Close();

                return balance;
            }
            catch (ServiceException se)
            {
                return se.Message;
            }


        }

        public string getnewpaymentamount(String PREPAIDID)
        {

            var connection = _configuration.GetConnectionString("pgWebForm");
            SqlConnection con = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand("select distinct amount from PrePaidPaymentSchedule where PREPAIDID = '" + PREPAIDID + "'", con);
            con.Open();
            SqlDataReader idr = cmd.ExecuteReader();
            string paymentamounts = "<select id=\"ddpaymentamount\" class=\"txtbox\"><option></option>";

            if (idr.HasRows)
            {
                while (idr.Read())
                {

                    decimal balance = Convert.ToDecimal(idr["amount"]);
                    paymentamounts += "<option>" + balance.ToString("C3") + "</option>";
                }
            }
            con.Close();

            paymentamounts += "</select>";
            return paymentamounts;


        }

    }
}
