﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.DBEntity.DataModels;

[Table("Category")]
public partial class Category
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string? Name { get; set; }

    [InverseProperty("CategoryNavigation")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
