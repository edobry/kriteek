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
using Newtonsoft.Json.Linq;
using System.Collections;

namespace kriteek.Controllers
{
    public class PostController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Post
        public IEnumerable<Post> GetAll()
        {
            return db.Posts.AsEnumerable();
        }

        // GET api/Post/5
        public Post GetPost(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return post;
        }

        public IEnumerable<Post> GetAllVisibleToUser(int user)
        {
            return db.Friendtypes.Where(x => x.Members.Select(member => member.ID).Contains(user)).SelectMany(x => x.CanSee);
        }

        public IEnumerable<Post> GetAllVisibleToGroup(Friendtype group)
        {
            return group.CanSee;
        }

        // POST api/Post
        public HttpResponseMessage PostCreate(JObject newPost)
        {
            dynamic json = newPost;
            Post post = json.post.ToObject<Post>();
            int PosterID = json.PosterID;
            IEnumerable<string> groups = json.groups.ToObject<IEnumerable<string>>();

            post.Poster = db.People.Single(x => x.Username == "abees");
            post.VisibleTo = db.Friendtypes.Where(x => x.PosterID == PosterID && groups.Contains(x.Type)).ToList() as ICollection<Friendtype>;
            db.Posts.Add(post);
            db.SaveChanges();
            post.Poster = null;
            post.VisibleTo = null;
            post.Topics = null;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, post);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = post.ID }));
            return response;
        }

        // PUT api/post/id/
        public HttpResponseMessage PutVisibleTo(int id, JObject input)
        {
            dynamic json = input;
            IEnumerable<Friendtype> friendtypes = json.friendtypes.ToObject<IEnumerable<Friendtype>>();
            bool show = json.show;

            Post post = db.Posts.Include(x => x.VisibleTo).Single(x => x.ID == id);
            if (post == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Friendtype _ft;
            foreach (Friendtype ft in friendtypes)
            {
                _ft = db.Friendtypes.Find(ft.PosterID, ft.Type);
                if (show) post.VisibleTo.Add(_ft);
                else post.VisibleTo.Remove(_ft);
            }

            db.SaveChanges();
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //// PUT api/Post/5
        //public HttpResponseMessage PutPost(int id, Post post)
        //{
        //    if (ModelState.IsValid && id == post.ID)
        //    {
        //        db.Entry(post).State = EntityState.Modified;

        //        try
        //        {
        //            db.SaveChanges();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //}

        // POST api/post/id
        public HttpResponseMessage PostRate(int id, Rating rating)
        {
            db.RatePost(id, rating.RaterID, (int)rating.Value);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/Post/5
        public HttpResponseMessage DeletePost(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            } 

            db.Posts.Remove(post);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, post);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}