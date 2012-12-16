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

namespace kriteek.Controllers
{
    public class FriendController : ApiController
    {
        private Entities db = new Entities();

        // GET api/friend/id
        public IEnumerable<Person> GetAll(int id)
        {
            return db.People.Include(x => x.Friendtypes).Include("Friendtypes.Members").Single(x => x.ID == id).Friendtypes.SelectMany(x => x.Members).Distinct();
        }

        // GET api/friend/id/?group=
        public IEnumerable<Person> GetInGroup(int id, string group)
        {
            return db.Friendtypes.Include(x => x.Members).Single(x => x.PosterID == id && x.Type == group).Members.ToList();
        }
        
        // POST api/friend/id
        public HttpResponseMessage PostAddToGroup(int id, Friendtype group)
        {
            db.Friendtypes.Attach(group).Members.Add(db.People.Find(id));
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/friend/id
        public HttpResponseMessage DeleteFromGroup(int id, Friendtype group)
        {
            Friendtype ft = db.Friendtypes.Include(x => x.Members).Single(x => x.PosterID == group.PosterID && x.Type == group.Type);
            ft.Members.Remove(db.People.Find(id));
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}