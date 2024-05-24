using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.DBEntity.DataModels;

public partial class ChatMessage
{
    [Key]
    public int MessageId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "character varying")]
    public string? Message { get; set; }

    [Column(TypeName = "character varying")]
    public string? SenderName { get; set; }

    [Column(TypeName = "character varying")]
    public string? SenderType { get; set; }

    [Column(TypeName = "character varying")]
    public string? RecipientName { get; set; }

    [Column(TypeName = "character varying")]
    public string? RecipientType { get; set; }

    public int? RequestId { get; set; }

    public int? RecipientId { get; set; }

    public int? SenderId { get; set; }

    [Column(TypeName = "character varying")]
    public string? FilePath { get; set; }
}
