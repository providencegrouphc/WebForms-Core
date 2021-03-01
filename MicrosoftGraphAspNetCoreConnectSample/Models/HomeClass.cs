using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrosoftGraphAspNetCoreConnectSample.Models
{
    public class Dropdownlist
    {
        public List<Issue_list> issuelist { get; set; }
        public List<Operation_List> operationlist { get; set; }
        public string getGUID { get; set; }
    }

    public class PrePaidObjects
    {
        public List<Operation_List> operationlist { get; set; }
        public List<PrePaidVendors_List> PrePaidVendorslist { get; set; }
    }

    public class LayoutObjects
    {
        public string showGUID { get; set; }
    }

    public class Issue_list
    {
        public string IssueTarget { get; set; }
        public string IssueName { get; set; }
    }
    public class Operation_List
    {
        public string operationName { get; set; }
    }

    public class PrePaidVendors_List
    {
        public string PrePaidVendorsName { get; set; }
    }

    public class getGUID
    {
        public string GUIDName { get; set; }
    }

    public class NavbarItem
    {
        public string controller { get; set; }

    }

    public class prepaidedit
    {
        public string ID { get; set; }
        public string balance { get; set; }
        public string facility { get; set; }
        public string amount { get; set; }
        public string invoiceduedate { get; set; }
        public string paid { get; set; }
        public string expectedreceiptdate { get; set; }
        public string beginamortizationdate { get; set; }
        public string monthsamortized { get; set; }
        public string vendor { get; set; }
        public string typeoflicense { get; set; }
        public string glcode { get; set; }
        public string notes { get; set; }
    }

}
