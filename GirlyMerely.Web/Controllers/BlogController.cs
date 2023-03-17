using GirlyMerely.Core.Models;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GirlyMerely.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly ArticlesRepository _articlesRepo;
        private readonly ArticleCategoriesRepository _articleCategoriesRepo;
        public BlogController(ArticlesRepository articlesRepo,
            ArticleCategoriesRepository articleCategoriesRepo)
        {
            _articlesRepo = articlesRepo;
            _articleCategoriesRepo = articleCategoriesRepo;
        }
        // GET: Blog
        public ActionResult Index(int pageNumber = 1, string searchString = null, int? category = null)
        {
            var articles = new List<Article>();
            var take = 3;
            var skip = pageNumber * take - take;
            var count = 0;
            if (category != null)
            {
                articles = _articlesRepo.GetArticlesList(skip, take, category.Value);
                count = _articlesRepo.GetArticlesCount(category.Value);
                var cat = _articleCategoriesRepo.Get(category.Value);
                ViewBag.CategoryId = category;
                ViewBag.Title = $"دسته {cat.Title}";
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                articles = _articlesRepo.GetArticlesList(skip, take, searchString);
                count = _articlesRepo.GetArticlesCount(searchString);
                ViewBag.SearchString = searchString;
                ViewBag.Title = $"جستجو: {searchString}";
            }
            else
            {
                articles = _articlesRepo.GetArticlesList(skip, take);
                count = _articlesRepo.GetArticlesCount();
                ViewBag.Title = "بلاگ";
            }

            var pageCount = (int) Math.Ceiling((double) count / take);
            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = pageNumber;

            var vm = new List<LatestArticlesViewModel>();
            foreach (var item in articles)
                vm.Add(new LatestArticlesViewModel(item));

            return View(vm);
        }
        public ActionResult ArticleCategoriesSection()
        {
            var categories = _articleCategoriesRepo.GetAll();
            return PartialView(categories);
        }
        public ActionResult TopArticlesSection(int take)
        {
            var vm = new List<LatestArticlesViewModel>();
            var articles = _articlesRepo.GetTopArticles(take);
            foreach (var item in articles)
                vm.Add(new LatestArticlesViewModel(item));

            return PartialView(vm);
        }
        [Route("Blog/ArticleDetails/{id}/{title}")]
        [Route("Blog/Article/{id}/{title}")]

        public ActionResult ArticleDetails(int id)
        {
            _articlesRepo.UpdateArticleViewCount(id);
            var article = _articlesRepo.GetArticle(id);
            var articleDetailsVm = new ArticleDetailsViewModel(article);
            var articleComments = _articlesRepo.GetArticleComments(id);
            var articleCommentsVm = new List<ArticleCommentViewModel>();

            foreach (var item in articleComments)
                articleCommentsVm.Add(new ArticleCommentViewModel(item));

            articleDetailsVm.ArticleComments = articleCommentsVm;
            var articleTags = _articlesRepo.GetArticleTags(id);
            articleDetailsVm.Tags = articleTags;
            var articleHeadlines = _articlesRepo.GetArticleHeadlines(id);
            articleDetailsVm.HeadLines = articleHeadlines;

            return View(articleDetailsVm);
        }
        [HttpPost]
        public ActionResult PostComment(CommentFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var comment = new ArticleComment()
                {
                    ArticleId = form.ArticleId.Value,
                    ParentId = form.ParentId,
                    Name = form.Name,
                    Email = form.Email,
                    Message = form.Message,
                    AddedDate = DateTime.Now,
                };
                _articlesRepo.AddComment(comment);
                return RedirectToAction("ContactUsSummary", "Home");
            }
            return RedirectToAction("ArticleDetails", new { id = form.ArticleId });
        }
    }
}