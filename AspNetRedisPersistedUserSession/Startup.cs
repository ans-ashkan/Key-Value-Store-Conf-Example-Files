using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using AspNetRedisPersistedUserSession.Data;
using AspNetRedisPersistedUserSession.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Owin;

[assembly: OwinStartup(typeof(AspNetRedisPersistedUserSession.Startup))]

namespace AspNetRedisPersistedUserSession
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(AppDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieSecure = CookieSecureOption.SameAsRequest,
                AuthenticationMode = AuthenticationMode.Active,
                CookieHttpOnly = true,
                ExpireTimeSpan = TimeSpan.FromHours(3),
                SlidingExpiration = true,
                CookieName = "LoginCookie",
                //                Provider = new MyCookieAuthenticationProvider(),
                SessionStore = new DictionarySessionStore()
//                SessionStore = new RedisSessionStore()
            });

            /* using (var dbc = new AppDbContext())
             {
                 var um = ApplicationUserManager.Create(dbc);
                 //                dbc.Database.Initialize(true);
                 if (!dbc.Users.Any(t => t.UserName == "ashkan"))
                     um.CreateAsync(new ApplicationUser() { UserName = "ashkan", Email = "ashkan@ans.org" }, "123456").Wait();
                 //                dbc.SaveChanges();
             }*/
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }



        public class RedisSessionStore : IAuthenticationSessionStore
        {
            TicketSerializer _ticketSerializer = new TicketSerializer();


            public async Task<string> StoreAsync(AuthenticationTicket ticket)
            {
                var key = Guid.NewGuid().ToString();
                await RedisCacheManager.Instance.SetAsync(key, _ticketSerializer.Serialize(ticket));
                return key;
            }

            public async Task RenewAsync(string key, AuthenticationTicket ticket)
            {
                await RedisCacheManager.Instance.SetAsync(key, _ticketSerializer.Serialize(ticket));
            }

            public async Task<AuthenticationTicket> RetrieveAsync(string key)
            {
                return _ticketSerializer.Deserialize(await RedisCacheManager.Instance.GetAsync<byte[]>(key));
            }

            public async Task RemoveAsync(string key)
            {
                await RedisCacheManager.Instance.RemoveAsync(key);
            }
        }

        public class DictionarySessionStore : IAuthenticationSessionStore
        {
            public static Dictionary<string, object> Storage = new Dictionary<string, object>(100);

            public Task<string> StoreAsync(AuthenticationTicket ticket)
            {
                string key = Guid.NewGuid().ToString();
                Storage[key + ".Ticket"] = ticket;
                return Task.FromResult(key);
            }

            public Task RenewAsync(string key, AuthenticationTicket ticket)
            {
                Storage[key + ".Ticket"] = ticket;
                return Task.FromResult(0);
            }

            public Task<AuthenticationTicket> RetrieveAsync(string key)
            {
                AuthenticationTicket ticket = null;
                if (Storage.ContainsKey(key + ".Ticket"))
                    ticket = Storage[key + ".Ticket"] as AuthenticationTicket;
                return Task.FromResult(ticket);
            }

            public Task RemoveAsync(string key)
            {
                Storage.Remove(key + ".Ticket");
                return Task.FromResult(0);
            }
        }
    }
}
