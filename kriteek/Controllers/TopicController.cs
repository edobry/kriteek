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
    public class TopicController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Topic
        public IEnumerable<Topic> GetAll()
        {
            return db.Topics.AsEnumerable();
        }

        // GET api/Topic/name
        public IEnumerable<Post> GetAllIn(string id)
        {
            return db.Topics.Include(x => x.Post).Where(x => x.Name == id).Select(x => x.Post);
        }

        // GET api/Topic/?visibleIn=topic
        public IEnumerable<Post> GetAllVisibleIn(string visibleIn)
        {
            var userID = db.People.FirstOrDefault(z => z.Username == User.Identity.Name).ID;
            return db.Topics.Include(x => x.Post).Include("Post.VisibleTo").Where(x =>
                x.Name == visibleIn &&
                x.Post.VisibleTo.Any(y => y.PosterID == userID)
            ).Select(x => x.Post);
        }

        // GET api/topic/?visibleTo=user
        public IEnumerable<Post> GetVisibleTo(string visibleTo)
        {
            var userID = db.People.FirstOrDefault(z => z.Username == visibleTo).ID;
            return db.Posts.Where(x => x.VisibleTo.Any(y => y.PosterID == userID));
        }
        
        // POST api/topic/id/?post=
        public HttpResponseMessage PostAddPost(string id, int post)
        {
            db.Posts.Find(post).Topics.Add(new Topic { Name = id, ID = post });
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/topic/id/?post=
        public HttpResponseMessage DeletePost(string id, int post)
        {
            db.Posts.Include(x => x.Topics).Single(x => x.ID == post).Topics.Remove(db.Topics.Find(post, id));
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