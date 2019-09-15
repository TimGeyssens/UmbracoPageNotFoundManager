using NPoco;
using System.Runtime.Serialization;

namespace PageNotFoundManager.Models
{
    [TableName(TableName)]

    [PrimaryKey("ParentId", AutoIncrement = false)]
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
