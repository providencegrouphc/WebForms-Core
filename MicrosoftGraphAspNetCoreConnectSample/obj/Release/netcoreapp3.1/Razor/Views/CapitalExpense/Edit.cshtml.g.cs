#pragma checksum "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CapitalExpense_Edit), @"mvc.1.0.view", @"/Views/CapitalExpense/Edit.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e", @"/Views/CapitalExpense/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d3ad82abc64eaf4b8b182bed96ff763a59d7fe17", @"/Views/_ViewImports.cshtml")]
    public class Views_CapitalExpense_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/dropzone/dropzone.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/dropzone/dropzone.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("text/css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/jquery-1.12.4.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/UI/jquery-ui.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/UI/jquery-ui.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/jquery.inputmask.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_8 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/moment.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_9 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/bootstrap-maxlength.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_10 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "CapitalExpense", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_11 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "PostExpenseEdit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_12 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_13 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "PostStatusChange", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_14 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/jquery.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_15 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/select2/js/select2.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
 if (!User.Identity.IsAuthenticated)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("<br />\n<p>Choose <b>Sign in</b> at the top of the page.</p>\n");
#nullable restore
#line 5 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\n\n");
#nullable restore
#line 8 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
 if (User.Identity.IsAuthenticated)
{
    if (ViewData["checkauth"].ToString() == "0")
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("<p>You are not authorized to view this page.</p>\n");
#nullable restore
#line 13 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
    }

    if (ViewData["checkauth"].ToString() == "1")
    {


#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
Write(Html.Raw(ViewData["sidebar"]));

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e10300", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e11338", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e12534", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e13572", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e14768", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_6);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e15806", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_7);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e16844", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_8);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e17882", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_9);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            WriteLiteral(@"<style>
    .txtbox {
        background-color: #fff;
        border: 1px solid #aaa;
        border-radius: 4px;
        height: 28px;
        width: 100% !important;
    }

    .txtlabel {
        margin-top: 15px;
    }

    .validation {
        color: red;
    }

    .dropzone {
        border: 1px solid #aaa !important;
        border-radius: 5px !important;
    }

    .imgdiv {
        border: 1px solid #aaa;
        border-radius: 5px;
        margin-top: 10px;
    }

        .imgdiv table {
            width: 280px;
            table-layout: fixed;
        }

    .imgdivtd {
        width: 170px;
        word-wrap: break-word;
        padding-right: 10px
    }
</style>
");
            WriteLiteral(@"<script>
    $(document).ready(function () {

        $('#txtTitle').maxlength()
        $('#txtDepartment').maxlength()
        $('#txtVendor').maxlength()
        $('#txtProblem').maxlength()
        $('#txtRecommended').maxlength()
    });

    $(window).load(function () {
        $.get(""/CapitalExpense/GetImages"", { stritem: document.getElementById('UID').value }, function (data) { document.getElementById('imagelist').innerHTML = data });

        $(""#txtLowBid"").inputmask({
            'alias': 'numeric', 'rightAlign': false, 'groupSeparator': ',', 'digits': 2, 'digitsOptional': false, 'prefix': '$ ', 'placeholder': '0'
        });

        $(""#txtHighBid"").inputmask({
            'alias': 'numeric', 'rightAlign': false, 'groupSeparator': ',', 'digits': 2, 'digitsOptional': false, 'prefix': '$ ', 'placeholder': '0'
        });

        $(""#txtRecBid"").inputmask({
            'alias': 'numeric', 'rightAlign': false, 'groupSeparator': ',', 'digits': 2, 'digitsOptional': false, 'prefix': '$ ', 'placeholder'");
            WriteLiteral(@": '0'
        });

        $(""#txtTotal"").inputmask({
            'alias': 'numeric', 'rightAlign': false, 'groupSeparator': ',', 'digits': 2, 'digitsOptional': false, 'prefix': '$ ', 'placeholder': '0'
        });

        $(""#txtBidCount"").inputmask({
            'alias': 'numeric', 'rightAlign': false, 'groupSeparator': ',', 'digits': 0, 'placeholder': '0'
        });
    });

    $(function () {
        $(""#txtDate"").datepicker();
    });
</script>
");
            WriteLiteral("<h1>Expense Details</h1>\n<hr />\n<a href=\"/CapitalExpense\">Return to Capital Expense Home</a>\n<div style=\"height:15px\"></div>\n");
#nullable restore
#line 114 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
Write(Html.Raw(ViewData["subinfo"]));

#line default
#line hidden
#nullable disable
            WriteLiteral("<h4>Request Information</h4>\n<hr style=\"margin:0px\" />\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e21697", async() => {
                WriteLiteral("\n\n    ");
#nullable restore
#line 119 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
Write(Html.Raw(ViewData["getdetails"]));

#line default
#line hidden
#nullable disable
                WriteLiteral("\n\n    <input type=\"text\" id=\"passid\" name=\"passid\" class=\"hidden\"");
                BeginWriteAttribute("value", " value=\"", 3337, "\"", 3374, 1);
#nullable restore
#line 121 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
WriteAttributeValue("", 3345, Html.Raw(ViewData["passid"]), 3345, 29, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
    <input type=""text"" id=""txtFac"" name=""txtFac"" class=""hidden"" />
    <input type=""text"" id=""strLowBid"" name=""strLowBid"" class=""hidden"" />
    <input type=""text"" id=""strHighBid"" name=""strHighBid"" class=""hidden"" />
    <input type=""text"" id=""strRecBid"" name=""strRecBid"" class=""hidden"" />
    <input type=""text"" id=""strTotal"" name=""strTotal"" class=""hidden"" />
    <input type=""text"" id=""strBidCount"" name=""strBidCount"" class=""hidden"" />
    <input name=""UID"" id=""UID"" type=""text""");
                BeginWriteAttribute("value", " value=\"", 3857, "\"", 3891, 1);
#nullable restore
#line 128 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
WriteAttributeValue("", 3865, Html.Raw(ViewData["UID"]), 3865, 26, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"hidden\" />\n    <input type=\"button\" id=\"subexpense\" class=\"hidden\" onclick=\"submit()\" />\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_10.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_10);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_11.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_11);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_12.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_12);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e25420", async() => {
                WriteLiteral("\n    <input type=\"text\" id=\"strStatusNotes\" name=\"strStatusNotes\" class=\"hidden\" />\n    <input type=\"text\" id=\"strStatus\" name=\"strStatus\" class=\"hidden\" />\n    <input name=\"statusid\" id=\"statusid\" type=\"text\"");
                BeginWriteAttribute("value", " value=\"", 4289, "\"", 4326, 1);
#nullable restore
#line 135 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
WriteAttributeValue("", 4297, Html.Raw(ViewData["passid"]), 4297, 29, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" class=\"hidden\" />\n    <input type=\"button\" id=\"substatus\" class=\"hidden\" onclick=\"submit()\" />\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_10.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_10);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_13.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_13);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_12.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_12);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            WriteLiteral("<div style=\"height:15px\"></div>\n<h4>Attachments</h4>\n<hr style=\"margin:0px\" />\n<div class=\"row\">\n    <div class=\"col-md-4\" style=\"padding-top:35px; max-width:310px;\">\n");
#nullable restore
#line 144 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
         using (Html.BeginForm("UploadFile", "CapitalExpense",
            FormMethod.Post,
            new
            {
                @class = "dropzone",
                id = "dropzone-form",
            }))
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"fallback\">\n            <input name=\"file\" type=\"file\" multiple />\n        </div>\n        <input name=\"fUID\" type=\"text\"");
            BeginWriteAttribute("value", " value=\"", 4965, "\"", 4999, 1);
#nullable restore
#line 155 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
WriteAttributeValue("", 4973, Html.Raw(ViewData["UID"]), 4973, 26, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"hidden\" />");
#nullable restore
#line 155 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
                                                                                           }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\n    <div class=\"col-md-4\">\n        <div id=\"imagelist\"></div>\n    </div>\n</div>\n<div style=\"height:15px\"></div>\n");
            WriteLiteral("<hr style=\"margin:0px\" />\n");
#nullable restore
#line 164 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
Write(Html.Raw(ViewData["subbtn"]));

#line default
#line hidden
#nullable disable
            DefineSection("Scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
        Dropzone.options.dropzoneForm = {
            paramName: ""file"",
            maxFilesize: 20,
            maxFiles: 4,

            dictMaxFilesExceeded: ""Custom max files msg"",
            dictDefaultMessage: '<img src=""/images/uploadimg.png"" /><br /><input type=""button"" class=""btn btn-primary"" style=""margin-bottom:10px;margin-top:15px;"" value=""Choose files to Upload"" /><br /><span style=""color:gray"">or drag and drop them here</span>',
            success: function (file, response) {
                this.removeFile(file);
                $.get(""/CapitalExpense/GetImages"", { stritem: document.getElementById('UID').value }, function (data) { document.getElementById('imagelist').innerHTML = data });
                // This return statement is necessary to remove progress bar after uploading.
                return file.previewElement.classList.add(""dz-success"");
            }
        };


    </script>
");
            }
            );
            WriteLiteral(@"<script>
    function changestatus() {
        document.getElementById('strStatusNotes').value = document.getElementById('txtApprovalNotes').value
        document.getElementById('strStatus').value = document.getElementById('ddstatus').value
        document.getElementById('substatus').click()
    }

    function delblob(blobname) {
        $.get(""/CapitalExpense/DelBlob"", { stritem: document.getElementById('UID').value, strblob: blobname }, function (data) { document.getElementById('imagelist').innerHTML = data });
    }

    function validatesub() {
        var errors = 0

        var LowBid = document.getElementById('txtLowBid').value
        LowBid = LowBid.replace('$', '')
        LowBid = LowBid.replace(' ', '')
        LowBid = LowBid.replace(/,/g, '')

        var HighBid = document.getElementById('txtHighBid').value
        HighBid = HighBid.replace('$', '')
        HighBid = HighBid.replace(' ', '')
        HighBid = HighBid.replace(/,/g, '')

        var RecBid = document.getElementById('txtRecBid'");
            WriteLiteral(@").value
        RecBid = RecBid.replace('$', '')
        RecBid = RecBid.replace(' ', '')
        RecBid = RecBid.replace(/,/g, '')

        var Total = document.getElementById('txtTotal').value
        Total = Total.replace('$', '')
        Total = Total.replace(' ', '')
        Total = Total.replace(/,/g, '')

        var BidCount = document.getElementById('txtBidCount').value
        BidCount = BidCount.replace('$', '')
        BidCount = BidCount.replace(' ', '')
        BidCount = BidCount.replace(/,/g, '')

        if (document.getElementById('ddfacility').value == '') {
            document.getElementById('validateFacility').className = 'validation'
            errors = 1
        } else {
            document.getElementById('txtFac').value = document.getElementById('ddfacility').value
            document.getElementById('validateFacility').className = 'hidden'
        }

        if (document.getElementById('txtDepartment').value == '') {
            document.getElementById('validateDepartment').classNa");
            WriteLiteral(@"me = 'validation'
            errors = 1
        } else {
            document.getElementById('validateDepartment').className = 'hidden'
        }

        if (document.getElementById('txtTitle').value == '') {
            document.getElementById('validateTitle').className = 'validation'
            errors = 1
        } else {
            document.getElementById('validateTitle').className = 'hidden'
        }

        if (document.getElementById('txtProblem').value == '') {
            document.getElementById('validateProblem').className = 'validation'
            errors = 1
        } else {
            document.getElementById('validateProblem').className = 'hidden'
        }

        if (document.getElementById('txtRecommended').value == '') {
            document.getElementById('validateRecommended').className = 'validation'
            errors = 1
        } else {
            document.getElementById('validateRecommended').className = 'hidden'
        }

        if (document.getElementById('txtVendor').value");
            WriteLiteral(@" == '') {
            document.getElementById('validateVendor').className = 'validation'
            errors = 1
        } else {
            document.getElementById('validateVendor').className = 'hidden'
        }

        if (document.getElementById('txtLowBid').value == '') {
            document.getElementById('validateLowBid').className = 'validation'
            errors = 1
        } else {
            document.getElementById('strLowBid').value = LowBid
            document.getElementById('validateLowBid').className = 'hidden'
        }

        if (document.getElementById('txtHighBid').value == '') {
            document.getElementById('validateHighBid').className = 'validation'
            errors = 1
        } else {
            document.getElementById('strHighBid').value = HighBid
            document.getElementById('validateHighBid').className = 'hidden'
        }

        if (document.getElementById('txtRecBid').value == '') {
            document.getElementById('validateRecBid').className = 'validat");
            WriteLiteral(@"ion'
            errors = 1
        } else {
            document.getElementById('strRecBid').value = RecBid
            document.getElementById('validateRecBid').className = 'hidden'
        }

        if (document.getElementById('txtTotal').value == '') {
            document.getElementById('validateTotal').className = 'validation'
            errors = 1
        } else {
            document.getElementById('strTotal').value = Total
            document.getElementById('validateTotal').className = 'hidden'
        }

        if (document.getElementById('txtBidCount').value == '') {
            document.getElementById('validateBidCount').className = 'validation'
            errors = 1
        } else {
            document.getElementById('strBidCount').value = BidCount
            document.getElementById('validateBidCount').className = 'hidden'
        }

        if (moment(document.getElementById('txtDate').value).isValid()) {
            document.getElementById('validateDate').className = 'hidden'
        } e");
            WriteLiteral("lse {\n            document.getElementById(\'validateDate\').className = \'validation\'\n            errors = 1\n        }\n\n        if (errors == 0) {\n            document.getElementById(\'subexpense\').click()\n        }\n    }\n</script>\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e36735", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_14);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "3e2115ca4d1ea17dd5f5c92b89e9dabfc0e25c6e37774", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_15);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n<script type=\"text/javascript\">\n    $.noConflict();\n    jQuery(document).ready(function ($) {\n        $(\"#ddfacility\").select2();\n    });\n\n</script>\n");
#nullable restore
#line 329 "C:\Users\daniel.stump\Documents\GitHub\WebForms-Core\MicrosoftGraphAspNetCoreConnectSample\Views\CapitalExpense\Edit.cshtml"
}}

#line default
#line hidden
#nullable disable
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
