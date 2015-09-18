using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PageNotFoundManager.Models
{
    [DataContract(Name = "pageNotFound")]

    public class PageNotFound
    {
        [DataMember(Name = "parentId")]
        public int ParentId { get; set; }

        [DataMember(Name = "notFoundPageId")]
        public int NotFoundPageId { get; set; }
    }
}