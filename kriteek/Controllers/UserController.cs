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
    public class UserController : ApiController
    {
        private Entities db = new Entities();

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

        // PUT api/User/5
        public HttpResponseMessage PutPerson(int id, Person person)
        {
            if (ModelState.IsValid && id == person.ID)
            {
                db.Entry(person).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/User
        public HttpResponseMessage PostPerson(Person person)
        {
            if (ModelState.IsValid)
            {
                db.People.Add(person);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, person);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = person.ID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/User/5
        public HttpResponseMessage DeletePerson(int id)
        {
            Person person = db.People.Find(id);
            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.People.Remove(person);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, person);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}