using KitanoUserService.API.Models.ExecuteModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Text;

namespace KitanoUserService.API.DataAccess
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public class LdapAuthentication
    {
        private DataSet _ds;
        public List<NguoiDungLDAP> LtsNguoiDungLDAP { get; set; }
        private string _adspath;
        private string _filterAttribute;
        private string _path;

        public LdapAuthentication()
        {
            LtsNguoiDungLDAP = new List<NguoiDungLDAP>();
        }

        public LdapAuthentication(string path)
        {
            _path = path;
            _ds = new DataSet();
            LtsNguoiDungLDAP = new List<NguoiDungLDAP>();
            CreateTable();
        }

        public bool IsAuthenticated(string domain, string type, string username, string pwd)
        {
            //DirectoryEntry Entry = new DirectoryEntry(domain);
            var Entry = new DirectoryEntry(domain, username, pwd);
            try
            {
                Entry.Path = domain + "/" + type;
                Entry.AuthenticationType = AuthenticationTypes.Secure;
                DirectorySearcher Search = new DirectorySearcher(Entry);
                if (username.Contains("@", StringComparison.CurrentCulture))
                    username = username.Substring(0, username.IndexOf("@"));
                Search.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(SAMAccountName=" + username + "))";
                System.DirectoryServices.SearchResult wSearchResult;
                try
                {
                    wSearchResult = Search.FindOne();
                }
                catch (Exception)
                {
                    return false;
                }
                if (wSearchResult == null)
                    return false;
                else
                {
                    _path = wSearchResult.Path;
                    _filterAttribute = wSearchResult.Properties["cn"][0].ToString();
                    _adspath = wSearchResult.Properties["adspath"][0].ToString();
                }

                return true;
            }
            catch (DirectoryServicesCOMException)
            {
                return false;
            }
        }

        private void CreateTable()
        {
            //DataTable for users
            DataTable tbUsers = new DataTable("users");
            //Create Columns for DataTable.
            tbUsers.Columns.Add("FullName", System.Type.GetType("System.String"));
            tbUsers.Columns.Add("Username", System.Type.GetType("System.String"));
            tbUsers.Columns.Add("AdsPath", System.Type.GetType("System.String"));
            tbUsers.Columns.Add("Email", System.Type.GetType("System.String"));
            _ds.Tables.Add(tbUsers);

            //DataTable for properties
            DataTable tbProperties = new DataTable("properties");
            //Create Columns for DataTable.
            tbProperties.Columns.Add("PropertyName", System.Type.GetType("System.String"));
            tbProperties.Columns.Add("PropertyValue", System.Type.GetType("System.String"));
            _ds.Tables.Add(tbProperties);
        }
    }
}
