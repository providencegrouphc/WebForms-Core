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
using System.Collections.Generic;
using Microsoft.Identity.Client;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Data;

namespace PGWebFormsCore.Controllers
{
    public class SharedController : Controller
    {


    }
}
