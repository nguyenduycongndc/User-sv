using KitanoUserService.API.LdapService.Entity;
using KitanoUserService.API.LdapService.Interface;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;

namespace KitanoUserService.API.LdapService.Service
{
    public class LdapService : ILdapService
    {
        private readonly LdapConfiguration _ldapConfig;
        private readonly LdapConnection _ldapConn;

        public LdapService(IOptions<LdapConfiguration> config)
        {
            this._ldapConfig = config.Value;
            this._ldapConn = new LdapConnection()
            {
                SecureSocketLayer = this._ldapConfig.Security
            };
        }

        public LdapUser FindUser(string userName)
        {
            _ldapConn.Connect(_ldapConfig.Url, this._ldapConfig.Security ? LdapConnection.DefaultSslPort : LdapConnection.DefaultPort);
            _ldapConn.Bind(_ldapConfig.BindDn, _ldapConfig.BindCredentials);

            var searchFilter = string.Format(_ldapConfig.SearchFilter, userName);

            var result = _ldapConn.Search(
            _ldapConfig.SearchBase,
            LdapConnection.ScopeSub,
            searchFilter,
            _ldapConfig.Attributes,
            false);

            try
            {
                var user = result.Next();
                if (user != null)
                {
                    _ldapConn.Read(userName);
                    if (_ldapConn.Connected)
                    {
                        var ldapUser = new LdapUser();

                        ldapUser.UserName = userName;

                        foreach (var attr in _ldapConfig.Attributes)
                        {
                            var userAttr = user.GetAttribute(attr);
                            if (userAttr != null)
                            {
                                if (!string.IsNullOrWhiteSpace(userAttr.StringValue))
                                    ldapUser.Properties.Add(attr, userAttr.StringValue);
                            }
                        }

                        return ldapUser;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed.", ex);
            }

            _ldapConn.Disconnect();
            return null;
        }

        public LdapUser Login(string userName, string password)
        {
            _ldapConn.Connect(_ldapConfig.Url, this._ldapConfig.Security ? LdapConnection.DefaultSslPort : LdapConnection.DefaultPort);
            _ldapConn.Bind(_ldapConfig.BindDn + "," + _ldapConfig.SearchBase, _ldapConfig.BindCredentials);

            var searchFilter = string.Format(_ldapConfig.SearchFilter, userName);

            var result = _ldapConn.Search(
            _ldapConfig.SearchBase,
            LdapConnection.ScopeSub,
            searchFilter,
            _ldapConfig.Attributes,
            false);

            try
            {
                var user = result.Next();
                if (user != null)
                {
                    var userDn = "CN=" + userName + "," + _ldapConfig.SearchBase;
                    _ldapConn.Bind(user.Dn, password);
                    if (_ldapConn.Bound)
                    {
                        var ldapUser = new LdapUser();

                        ldapUser.UserName = userName;

                        foreach (var attr in _ldapConfig.Attributes)
                        {
                            var userAttr = user.GetAttribute(attr);
                            if (userAttr != null)
                            {
                                if (!string.IsNullOrWhiteSpace(userAttr.StringValue))
                                    ldapUser.Properties.Add(attr, userAttr.StringValue);
                            }
                        }

                        return ldapUser;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed.", ex);
            }

            _ldapConn.Disconnect();
            return null;
        }
    }
}
