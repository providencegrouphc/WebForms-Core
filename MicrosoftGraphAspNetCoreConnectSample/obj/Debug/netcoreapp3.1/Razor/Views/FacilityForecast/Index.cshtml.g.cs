#pragma checksum "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "672f4e9b0147086ffd505d3da3abc9f6b8c6f958"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_FacilityForecast_Index), @"mvc.1.0.view", @"/Views/FacilityForecast/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\_ViewImports.cshtml"
using MicrosoftGraphAspNetCoreConnectSample;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"672f4e9b0147086ffd505d3da3abc9f6b8c6f958", @"/Views/FacilityForecast/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"66db3822b77c774ac4b4a439c502a758348a340e", @"/Views/_ViewImports.cshtml")]
    public class Views_FacilityForecast_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/bootstrap-weekpicker.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("text/javascript"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/jquery.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/select2/js/select2.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n\r\n");
#nullable restore
#line 4 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml"
 if (!User.Identity.IsAuthenticated)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <br />\r\n    <p>Choose <b>Sign in</b> at the top of the page.</p>\r\n");
#nullable restore
#line 8 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 11 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml"
 if (User.Identity.IsAuthenticated)
{
    if (TempData["checkauth"].ToString() == "0")
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <p>You are not authorized to view this page.</p>\r\n");
#nullable restore
#line 16 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml"
    }

    if (TempData["checkauth"].ToString() == "1")
    {

        

#line default
#line hidden
#nullable disable
#nullable restore
#line 21 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml"
   Write(Html.Raw(ViewData["sidebar"]));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <style>
            .next-weekpicker1 {
                visibility: hidden !important;
            }

            .previous-weekpicker1 {
                visibility: hidden !important;
            }

            .form-control {
                border-radius: 5px !important;
                border: 1px solid #aaa;
            }

            .txtbox {
                background-color: #fff;
                border: 1px solid #aaa;
                border-radius: 4px;
                height: 28px;
                width: 100%;
            }

            .txtboxnoheight {
                background-color: #fff;
                border: 1px solid #aaa;
                border-radius: 4px;
                width: 100%;
            }

            .txtlabel {
                margin-top: 15px;
            }

            .float {
                float: left;
                width: 200px;
                margin-top: 15px;
            }

            .subbutton {
                b");
            WriteLiteral(@"ackground-color: #1b1464;
                border-radius: 5px;
                padding-top: 10px;
                padding-bottom: 10px;
                width: 100px;
                color: white;
                text-decoration: none;
                border: none
            }

                .subbutton:hover {
                    background-color: #2b3791
                }

            .validation {
                color: red;
            }

            .tableFixHead {
                overflow-y: auto;
                height: 200px;
            }

                .tableFixHead thead th {
                    position: sticky;
                    top: 0;
                    background-color: #1b1464;
                    color: white;
                }

            table {
                border-collapse: collapse;
                width: 100%;
            }

            th,
            td {
                padding: 8px;
            }

            th {
                bac");
            WriteLiteral(@"kground: #eee;
            }

            td, th {
                border: 1px solid #ddd;
                padding: 5px;
            }

            tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            tr:hover {
                background-color: #ddd;
            }

            .fixed_header {
                width: auto;
                table-layout: fixed;
                border-collapse: collapse;
            }

                .fixed_header td, .fixed_header th {
                    border: 1px solid #ddd;
                    padding: 5px;
                }

                .fixed_header tr:nth-child(even) {
                    background-color: #f2f2f2;
                }

                .fixed_header tr:hover {
                    background-color: #ddd;
                }

                .fixed_header th {
                    padding-top: 12px;
                    padding-bottom: 12px;
                    text-align: left;
   ");
            WriteLiteral(@"                 background-color: #1b1464;
                    color: white;
                }

                .fixed_header tbody {
                    display: block;
                    width: 100%;
                    overflow: auto;
                    max-height: 150px;
                }

                .fixed_header thead tr {
                    display: block;
                }

                .fixed_header thead {
                    background: black;
                    color: #fff;
                }

                .fixed_header th, .fixed_header td {
                    padding: 5px;
                    text-align: left;
                    width: 200px
                }
        </style>
        <h1>Forecast</h1>
");
            WriteLiteral("        <div class=\"txtlabel\">Facility</div>\r\n");
#nullable restore
#line 172 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml"
   Write(Html.Raw(ViewData["forecastoperations"]));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <div id=""validateFacility"" class=""hidden"">Please make a selection.</div>
        <div class=""txtlabel"">* Amount</div>
        <div><input type=""text"" id=""txtAmount"" class=""txtbox"" name=""Amount"" /></div>
        <div id=""validateAmount"" class=""hidden"">Please enter a valid number.</div>
");
            WriteLiteral(@"        <link rel=""stylesheet"" href=""https://netdna.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"">
        <link rel=""stylesheet"" href=""https://cdn.rawgit.com/pingcheng/bootstrap4-datetimepicker/master/build/css/bootstrap-datetimepicker.min.css"">
");
            WriteLiteral("        <div class=\"input-group date align-items-center\" style=\"width:100%\">\r\n            <div id=\"weekpicker1\"></div>\r\n        </div>\r\n");
            WriteLiteral("        <input type=\"button\" class=\"btn btn-primary\" style=\"margin-top:15px;\" value=\"Submit\" onclick=\"validate()\" />\r\n");
            WriteLiteral("        <h3>Projection History</h3>\r\n        <div id=\"forecast\"></div>\r\n");
            WriteLiteral(@"        <script src=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js"" type=""text/javascript""></script>
        <script src=""https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.21.0/moment.min.js"" type=""text/javascript""></script>
        <script src=""https://cdn.rawgit.com/pingcheng/bootstrap4-datetimepicker/master/build/js/bootstrap-datetimepicker.min.js"" type=""text/javascript""></script>
        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "672f4e9b0147086ffd505d3da3abc9f6b8c6f95811928", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            WriteLiteral("        <script type=\"text/javascript\">\r\n\r\n            var weekpicker = $(\"#weekpicker1\").weekpicker();\r\n        </script>\r\n");
            WriteLiteral("        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "672f4e9b0147086ffd505d3da3abc9f6b8c6f95813257", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "672f4e9b0147086ffd505d3da3abc9f6b8c6f95814305", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
        <script type=""text/javascript"">
            $.noConflict();
            jQuery(document).ready(function ($) {
                $.get(""/FacilityForecast/getforcast"", { strFacility: document.getElementById('ddfacility').value }, function (data) { document.getElementById('forecast').innerHTML = data; });
                $(""#ddfacility"").select2();
                $('#ddfacility').on('select2:select', function (e) {
                    $.get(""/FacilityForecast/getforcast"", { strFacility: e.params.data.text }, function (data) { document.getElementById('forecast').innerHTML = data; });
                });
            });


            function validate() {
                var errors = 0
                var selweek = """"
                $("".form-control"").each(function () {
                    selweek = $(this).val();
                });

                if (document.getElementById('ddfacility').value == '') {
                    document.getElementById('validateFacility').className = 'val");
            WriteLiteral(@"idation'
                    errors = 1
                } else {
                    document.getElementById('validateFacility').className = 'hidden'
                }

                if (document.getElementById('txtAmount').value == '') {
                    document.getElementById('validateAmount').className = 'validation'
                    errors = 1
                } else {
                    if (isNaN(document.getElementById('txtAmount').value)) {
                        document.getElementById('validateAmount').className = 'validation'
                        errors = 1
                    } else {
                        document.getElementById('validateAmount').className = 'hidden'
                    }
                }

                if (errors == 0) {
                    $.get(""/FacilityForecast/AddForecast"", { strFacility: document.getElementById('ddfacility').value, strAmount: document.getElementById('txtAmount').value, strWeek: selweek }, function (data) { document.getEl");
            WriteLiteral("ementById(\'forecast\').innerHTML = data });\r\n                }\r\n\r\n\r\n            }\r\n        </script>\r\n");
#nullable restore
#line 249 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\FacilityForecast\Index.cshtml"
    }
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
