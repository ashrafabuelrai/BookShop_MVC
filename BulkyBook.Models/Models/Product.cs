﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        [Display(Name ="List Price")]
        [Range(1,1000)]
        public double ListPrice { get; set; }
        [Display(Name = "Price 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }
        [Display(Name = "Price 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }
        [Display(Name = "Price 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }
       
        [ForeignKey("Category")]
        [Display(Name =("Category"))]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }

    }
}
