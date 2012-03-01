using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BabySittingCoop.Domain;
using BabySittingCoop.Domain.Views;
using BabySittingCoop.Web.Filters;
using BabySittingCoop.Web.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace BabySittingCoop.Web.Controllers
{
    public class HomeController : Controller
    {
        private ISession _session;
        public HomeController(ISession session)
        {
            _session = session;
        }

        //[Transaction]
        public ActionResult Index()
        {
           return View();
        }

        public ActionResult Recommendations()
        {
            var babysitters =
                _session
                    .QueryOver<BabySitter>()
                    .OrderBy(bs => bs.Name).Asc
                    .List<BabySitter>();

            ViewBag.BabySitters = babysitters;
            return View();
        }

        [HttpPost]
        [Transaction]
        public JsonResult GetBabySittingPoints(string sidx, string sord, int page, int rows)
        {
            var pointsView = _session
                .QueryOver<BabySittingPointsView>()
                .Skip(((page - 1) * rows))
                .Take(rows)
                .UnderlyingCriteria
                .AddOrder(new Order(sidx, sord == "asc"))
                .Future<BabySittingPointsView>();

            var totalCount = _session
                .QueryOver<BabySittingPointsView>()
                .ToRowCountQuery()
                .RowCount();

            var jsonData = new
            {
                total = Math.Ceiling(totalCount / (Double)rows),
                page,
                records = totalCount,

                rows = (from bs in pointsView
                        select new
                        {
                            id = bs.Id,
                            cell = new[]
                                      {
                                          "",bs.Name,bs.ProvidedPoints.ToString(),bs.ReceiverPoints.ToString(),bs.TotalPoints.ToString()
                                      }
                        }).ToArray()
            };
            return Json(jsonData);
        }

        [Transaction]
        [HttpPost]
        public JsonResult BabySitterRecommendations(int id)
        {
            var recommendations = _session
                           .GetNamedQuery("BabySitterRecommendations")
                           .SetResultTransformer(Transformers.AliasToBean(typeof(BabySittingRecommendationsView)))
                           .SetParameter("parentId", id, NHibernateUtil.Int32)
                           .List<BabySittingRecommendationsView>();

            var jsonData = new
            {
                records = recommendations.Count,

                rows = (from bs in recommendations
                        select new
                        {
                            id = bs.BabySitterId,
                            cell = new[]
                                      {
                                          "", bs.Name, bs.ProvidedCount.ToString()
                                      }
                        }).ToArray()
            };
            return Json(jsonData);
        }
    }
}
