﻿using System;
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
            return db.Posts.Include(x => x.Poster).Include(x => x.Ratings).Include(x => x.Topics).AsEnumerable();
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
            return (user != 0
                ? db.Posts.Where(x => x.VisibleTo.Any(y => y.PosterID == user) || x.PosterID == user || x.Type == 0)
                : db.Posts.Where(x => x.Type == 0)).Include(x => x.Poster).Include(x => x.Ratings).Include(x => x.Topics);
        }

        // POST api/Post
        public int PostCreate(JObject newPost)
        {
            dynamic json = newPost;
            Post post = json.post.ToObject<Post>();
            int PosterID = json.PosterID;
            IEnumerable<string> groups = json.groups.ToObject<IEnumerable<string>>();

            post.Poster = db.People.Single(x => x.Username == User.Identity.Name);
            post.Time = DateTime.Now;
            post.VisibleTo = db.Friendtypes.Where(x => x.PosterID == PosterID && groups.Contains(x.Type)).ToList() as ICollection<Friendtype>;
            db.Posts.Add(post);
            db.SaveChanges();
            post.Poster = null;
            post.VisibleTo = null;
            post.Topics = null;
            return post.ID;
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
            if (post.Type == 0) post.Type = 1;
            foreach (Friendtype ft in friendtypes)
            {
                _ft = db.Friendtypes.Find(ft.PosterID, ft.Type);
                if (show) post.VisibleTo.Add(_ft);
                else post.VisibleTo.Remove(_ft);
            }

            db.SaveChanges();
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        
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