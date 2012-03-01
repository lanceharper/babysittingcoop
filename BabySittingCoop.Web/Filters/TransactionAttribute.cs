using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;

namespace BabySittingCoop.Web.Filters
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        private ITransaction _currentTransaction;

        public TransactionAttribute()
        {

            //  _currentTransaction = MvcApplication.CurrentSession.Transaction;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _currentTransaction = MvcApplication.CurrentSession.Transaction;
            //if (!filterContext.IsChildAction)
            _currentTransaction.Begin();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (_currentTransaction.IsActive)
            {
                if (filterContext.Exception == null)
                {
                    _currentTransaction.Commit();
                }
                else
                {
                    _currentTransaction.Rollback();
                }
            }
        }
    }
}