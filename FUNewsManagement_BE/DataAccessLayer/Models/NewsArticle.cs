using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Models;

public partial class NewsArticle
{
    public string NewsArticleId { get; set; }

    public string? NewsTitle { get; set; }

    public string Headline { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public string? NewsContent { get; set; }

    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    public bool? NewsStatus { get; set; }

    public short? CreatedById { get; set; }

    public short? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }
    [JsonIgnore]
    public virtual Category? Category { get; set; }
    [JsonIgnore]
    public virtual SystemAccount? CreatedBy { get; set; }
    [JsonIgnore]

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
