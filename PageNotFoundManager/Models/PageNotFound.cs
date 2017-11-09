using System.Runtime.Serialization;
using Umbraco.Core.Persistence;

namespace PageNotFoundManager.Models
{
    [TableName(TableName)]
    [PrimaryKey("ParentId", autoIncrement = false)]
    public class PageNotFound
    {
        [IgnoreDataMember]
        public const string TableName = "pageNotFoundConfig";

        [Column("ParentId")]      
        public int ParentId { get; set; }

        [Column("NotFoundPageId")]
        public int NotFoundPageId { get; set; }
    }
}