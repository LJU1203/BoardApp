using Board.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Board.Models;
using Board.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Board.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private int pageSize = 10;

        public PostsController(ApplicationDbContext dbContext)
        {
            this.dbContext=dbContext;
        }

        private int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }
        private bool IsLoggedIn()
        {
            return GetCurrentUserId() != 0;
        }
        private async Task<string> GetCurrentUsernameAsync(int id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            return user.Username;
        }


        [HttpGet]
        public async Task<IActionResult> List(int page = 1, string search = "")
        {
            IQueryable<Post> query = dbContext.Posts;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Content.Contains(search) || p.Title.Contains(search));
            }

            var totalPosts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            var posts = await query
                .Join(dbContext.Users,
                      post => post.UserId,
                      user => user.UserId,
                      (post, user) => new ListItem
                      {
                          PostId = post.PostId,
                          Title = post.Title,
                          Username = user.Username,
                          DatePosted = post.DatePosted
                      })
                .OrderByDescending(post => post.DatePosted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var viewModel = new ListViewModel
            {
                PostItems = posts,
                TotalPosts = totalPosts,
                TotalPages = totalPages,
                CurrentPage = page,
                IsLoggedIn = IsLoggedIn()
            };

            return View(viewModel);
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreatePostViewModel viewModel)
        {
            if (GetCurrentUserId() == 0)
            {
                return RedirectToAction("List");
            }

            var post = new Post
            {
                Title = viewModel.Title,
                Content = viewModel.Content,
                UserId = GetCurrentUserId()
            };

            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [HttpGet]

        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();
            var isAuthor = post.UserId == currentUserId;

            var comments = await dbContext.Comments
                .Where(c => c.PostId == id)
                .Join(dbContext.Users,
                      comment => comment.UserId,
                      user => user.UserId,
                      (comment, user) => new CommentViewModel
                      {
                          CommentId = comment.CommentId,
                          Content = comment.Content,
                          Username = user.Username,
                          DateTime = comment.DatePosted,
                          IsAuthor = comment.UserId == GetCurrentUserId()
                      })
                .ToListAsync();

            var postViewModel = new PostViewModel
            {
                Post = post,
                Comments = comments,
                IsAuthor = isAuthor,
                IsLoggedIn = IsLoggedIn()
            };

            return View(postViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (GetCurrentUserId() == 0)
            {
                return RedirectToAction("View", new { id = postId });
            }

            string Author = await GetCurrentUsernameAsync(GetCurrentUserId());

            var comment = new Comment
            {
                PostId = postId,
                UserId = GetCurrentUserId(),
                Content = content,
                DatePosted = DateTime.Now
            };

            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();
            return RedirectToAction("View", new { id = postId });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await upload.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    var image = new Image { Data = imageBytes };
                    dbContext.Images.Add(image);
                    await dbContext.SaveChangesAsync();

                    string imageUrl = Url.Action("GetImage", "Posts", new { id = image.Id }, protocol: Request.Scheme);
                    image.Url = imageUrl;
                    dbContext.Images.Update(image);
                    await dbContext.SaveChangesAsync();

                    return Json(new { uploaded = true, url = imageUrl });
                }
            }

            return Json(new { uploaded = false, error = new { message = "Upload failed" } });
        }

        [HttpGet]
        public IActionResult GetImage(int id)
        {
            var image = dbContext.Images.Find(id);
            if (image != null)
            {
                return File(image.Data, "image/png");
            }
            return NotFound();
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            
            var post = await dbContext.Posts.FindAsync(id);
            
            if (post == null || post.UserId != GetCurrentUserId())
            {
                return NotFound(); 
            }

            EditPostViewModel editModel = new EditPostViewModel { 
                PostId = post.PostId,
                Title = post.Title, 
                Content = post.Content
            };
            return View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostViewModel model)
        {
            var post = dbContext.Posts.Find(model.PostId);

            post.Title = model.Title;
            post.Content = model.Content;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                return View(model); 
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await dbContext.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm(Name = "PostId")] int id)
        {
            var post = await dbContext.Posts.FindAsync(id);
            if (post != null && post.UserId == GetCurrentUserId())
            {
                dbContext.Posts.Remove(post);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await dbContext.Comments.FindAsync(id);
            if (comment != null && comment.UserId == GetCurrentUserId())
            {
                dbContext.Comments.Remove(comment);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("View", new { id = comment.PostId });
        }
    }
}


