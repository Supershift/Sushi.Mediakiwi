using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    public enum ASyncQueryType
    {
        FindByText = 1,
        SelectOneByID = 2,
        OnSelectCallBack = 3,

    }
    public class ASyncQuery
    {
        public ASyncQuery(HttpContext context)
        {
            _Context = context;
        }
        bool isSet;
        HttpContext _Context;
    
        public string Property { get; set; }
        public string SearchQuery { get; set; }
        public ASyncQueryType SearchType { get; set; }
        public bool IsAsyncCall { get; set; }

        public bool ApplyReset(params string[] property)
        {
            return ApplyReset(true, property);
        }

        internal bool ApplyReset(bool applyContext, params string[] property)
        {
            if (applyContext)
            {
                if (_Context == null)
                    return false;

                _Context.Items["Async.Reset"] = Utility.ConvertToCsvString(property, false);
            }
            else
                _Reset = property;
            return true;
        }

        string[] _Reset;
        internal string[] Reset
        {
            get
            {
                //  Reset without context
                if (_Reset != null)
                    return _Reset;

                if (_Context == null)
                    return null;
                var r = _Context.Items["Async.Reset"];
                if (r == null)
                    return null;

                return r.ToString().Split(',');
            }
        }
    }
}
