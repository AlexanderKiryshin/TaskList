using System;
using System.Web;
using System.Web.Routing;

namespace TaskList.Constraints
{
    public class IdConstraint:IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            object value;
          
            if (!values.TryGetValue(parameterName, out value))
            {
                return false;
            }
            int temp;
            if (Int32.TryParse(value.ToString(), out temp))
            {
                return true;
            }
            return false;
        }
    }
}