#pragma checksum "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2ac84eb4f185caf3a44dc5fb90162bfe90b9c023"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_HRCallLog_Edit), @"mvc.1.0.view", @"/Views/HRCallLog/Edit.cshtml")]
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
#line 1 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\_ViewImports.cshtml"
using MicrosoftGraphAspNetCoreConnectSample;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2ac84eb4f185caf3a44dc5fb90162bfe90b9c023", @"/Views/HRCallLog/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d3ad82abc64eaf4b8b182bed96ff763a59d7fe17", @"/Views/_ViewImports.cshtml")]
    public class Views_HRCallLog_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/dropzone/dropzone.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/dropzone/dropzone.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("text/css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/jquery-1.12.4.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/UI/jquery-ui.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/UI/jquery-ui.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "HRCallLog", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_8 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "PostAddCall", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_9 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_10 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("hidden"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\n");
#nullable restore
#line 2 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
 if (!User.Identity.IsAuthenticated)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("<br />\n<p>Choose <b>Sign in</b> at the top of the page.</p>\n");
#nullable restore
#line 6 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
}

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
 if (User.Identity.IsAuthenticated)
{
    if (ViewData["checkauth"].ToString() == "0")
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("<p>You are not authorized to view this page.</p>\n");
#nullable restore
#line 12 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
    }

    if (ViewData["checkauth"].ToString() == "1")
    {

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2ac84eb4f185caf3a44dc5fb90162bfe90b9c0238153", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "2ac84eb4f185caf3a44dc5fb90162bfe90b9c0239190", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2ac84eb4f185caf3a44dc5fb90162bfe90b9c02310385", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "2ac84eb4f185caf3a44dc5fb90162bfe90b9c02311423", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2ac84eb4f185caf3a44dc5fb90162bfe90b9c02312619", async() => {
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
            WriteLiteral(@"
<style>
    .txtNotes {
        border: 1px solid #aaa;
        width: 300px;
        height: 200px;
        padding: 5px 5px 5px 5px;
        overflow-y: scroll;
        background-color: white;
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

    .collapsible {
        background-color: #337ab7;
        color: white;
        cursor: pointer;
        padding: 5px;
        width: 100%;
        border: 1px solid #2e6da4;
        text-align: left;
        outline: none;
        font-size: 15px;
    }

        .active, .collapsible:hover {
            background-color: #286090;
            border: 1px solid #204d74;
        }

        .collapsible:af");
            WriteLiteral(@"ter {
            content: '\002B';
            color: white;
            font-weight: bold;
            float: right;
            margin-left: 5px;
        }

    .active:after {
        content: ""\2212"";
    }

    .content {
        padding: 0 18px;
        max-height: 0;
        overflow: hidden;
        transition: max-height 0.2s ease-out;
        background-color: #f1f1f1;
    }
</style>
");
#nullable restore
#line 92 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
Write(Html.Raw(ViewData["sidebar"]));

#line default
#line hidden
#nullable disable
            WriteLiteral("<h1>Call Details</h1>\n<hr />\n<a href=\"/HRCallLog\">Return to Call Log Home</a>\n");
            WriteLiteral("<input type=\"text\"");
            BeginWriteAttribute("value", " value=\"", 2146, "\"", 2183, 1);
#nullable restore
#line 98 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
WriteAttributeValue("", 2154, Html.Raw(ViewData["passid"]), 2154, 29, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"hidden\" id=\"passid\" />\n<br />\n<br />\n");
#nullable restore
#line 101 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
Write(Html.Raw(ViewData["loginfo"]));

#line default
#line hidden
#nullable disable
            WriteLiteral("<div class=\"row\">\n    <div class=\"col-md-4\">\n        ");
#nullable restore
#line 104 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
   Write(Html.Raw(ViewData["calldetail"]));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
        <div style=""height:5px;""></div>
        <table style=""width:300px"">
            <tr>
                <td><button type=""submit"" class=""btn btn-primary"" onclick=""showaddnotes()"">Add Notes</button></td>
                <td style=""text-align:right""><button type=""submit"" class=""btn btn-primary"" onclick=""addcall()"">Add Call</button></td>
            </tr>
        </table>

        <div style=""height:10px""></div>
        ");
#nullable restore
#line 114 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
   Write(Html.Raw(ViewData["additionalcalls"]));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2ac84eb4f185caf3a44dc5fb90162bfe90b9c02317278", async() => {
                WriteLiteral("\n            <input type=\"text\" id=\"txtID\" name=\"txtID\" />\n            <input type=\"button\" id=\"subedit\" onclick=\"submit()\" />\n        ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_7.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_7);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_8.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_8);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_9.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_9);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_10);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n\n    </div>\n    <div class=\"col-md-4\">\n\n");
#nullable restore
#line 124 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
         using (Html.BeginForm("UploadFile", "HRCallLog",
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
            WriteLiteral("        <div class=\"fallback\">\n            <input name=\"file\" type=\"file\" multiple />\n\n        </div>\n        <input id=\"fUID\" name=\"fUID\" type=\"text\"");
            BeginWriteAttribute("value", " value=\"", 3397, "\"", 3431, 1);
#nullable restore
#line 136 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
WriteAttributeValue("", 3405, Html.Raw(ViewData["UID"]), 3405, 26, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"hidden\" />");
#nullable restore
#line 136 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
                                                                                                     }

#line default
#line hidden
#nullable disable
            WriteLiteral("\n        <div id=\"imagelist\">\n            ");
#nullable restore
#line 139 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
       Write(Html.Raw(ViewData["uploads"]));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n        </div>\n\n");
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
                        $.get(""/HRCallLog/GetImages"", { stritem: document.getElementById('fUID').value }, function (data) { document.getElementById('imagelist').innerHTML = data });
                        // This return statement is necessary to remove progress bar after uploading.
                        return file.previewElement.classList.add(""dz-success"");
              ");
                WriteLiteral("      }\n                };\n\n\n            </script>\n        ");
            }
            );
            WriteLiteral("\n    </div>\n</div>\n");
            WriteLiteral(@"<script>
    function showaddnotes(type) {
        document.getElementById('txtAddNotes').value = '';
        $(""#showaddnotes"").dialog({
            resizable: false,
            height: ""auto"",
            width: ""420px"",
            modal: true
        });
    };
</script>
");
            WriteLiteral(@"<div style=""visibility:hidden; height:0px; display:none"">
    <div id=""showaddnotes"" title=""Add Notes"">
        <div style=""width:100%; text-align:center"">
            <textarea id=""txtAddNotes"" style=""height:100px; width:300px""></textarea>
        </div>

        <br />

        <div class=""modal-footer"">
            <button type=""submit"" class=""btn btn-primary"" onclick=""addnotes()"">Save</button>
            <button type=""button"" class=""btn btn-secondary"" onclick=""$('#showaddnotes').dialog('close');"">Close</button>
        </div>
    </div>
</div>
");
            WriteLiteral(@"<script>
    function shownewnotes(passid) {
        document.getElementById('txtnewNotes').value = '';
        document.getElementById('txtnewid').value = passid;
        $(""#shownewnotes"").dialog({
            resizable: false,
            height: ""auto"",
            width: ""420px"",
            modal: true
        });
    };
</script>
");
            WriteLiteral(@"<div style=""visibility:hidden; height:0px; display:none"">
    <div id=""shownewnotes"" title=""Add Notes"">
        <div style=""width:100%; text-align:center"">
            <textarea id=""txtnewNotes"" style=""height:100px; width:300px""></textarea>
            <input type=""text"" id=""txtnewid"" class=""hidden"" />
        </div>

        <br />

        <div class=""modal-footer"">
            <button type=""submit"" class=""btn btn-primary"" onclick=""addnewnotes()"">Save</button>
            <button type=""button"" class=""btn btn-secondary"" onclick=""$('#shownewnotes').dialog('close');"">Close</button>
        </div>
    </div>
</div>
");
            WriteLiteral(@"<script>
    $(window).resize(function () {
        $(""#showaddnotes"").dialog(""option"", ""position"", { my: ""center"", at: ""center"", of: window });
    });

    function delblob(blobname) {
        $.get(""/HRCallLog/DelBlob"", { stritem: document.getElementById('fUID').value, strblob: blobname }, function (data) { document.getElementById('imagelist').innerHTML = data });

    }

    function addnotes() {
        $.get(""/HRCallLog/AddNotes"", { stritem: document.getElementById('txtAddNotes').value, strid: document.getElementById('passid').value }, function (data) { document.getElementById('strNotes').innerHTML = data });
        $('#showaddnotes').dialog('close');
    }

    function addnewnotes() {
        $.get(""/HRCallLog/AddNotes"", { stritem: document.getElementById('txtnewNotes').value, strid: document.getElementById('txtnewid').value }, function (data) { document.getElementById(document.getElementById('txtnewid').value).innerHTML = data });
        $('#shownewnotes').dialog('close');
    }

    function addca");
            WriteLiteral(@"ll() {
        document.getElementById('txtID').value = document.getElementById('passid').value
        document.getElementById('subedit').click()
    }

    var coll = document.getElementsByClassName(""collapsible"");
    var i;

    for (i = 0; i < coll.length; i++) {
        coll[i].addEventListener(""click"", function () {
            this.classList.toggle(""active"");
            var content = this.nextElementSibling;
            if (content.style.maxHeight) {
                content.style.maxHeight = null;
            } else {
                content.style.maxHeight = content.scrollHeight + ""px"";
            }
        });
    }
</script>
");
#nullable restore
#line 264 "C:\Users\daniel.stump\Desktop\New Projet\MicrosoftGraphAspNetCoreConnectSample\Views\HRCallLog\Edit.cshtml"
    }
}

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
