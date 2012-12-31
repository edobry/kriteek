using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using kriteek.Models;
using System.Web.Security;

namespace kriteek.Controllers
{
    public class UserController : ApiController
    {
        private Entities db = new Entities();

        // POST api/User/logininfo
        public int PostLogin([FromBody] LoginModel loginInfo)
        {
            if (Membership.ValidateUser(loginInfo.Username, loginInfo.Password))
            {
                FormsAuthentication.SetAuthCookie(loginInfo.Username, true);
                return int.Parse(Membership.GetUser().Comment);
            }
            return 0;
        }

        // GET api/User
        public IEnumerable<Person> GetPeople()
        {
            return db.People.AsEnumerable();
        }

        // GET api/User/5
        public Person GetPerson(int id)
        {
            Person person = db.People.Find(id);
            if (person == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return person;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}