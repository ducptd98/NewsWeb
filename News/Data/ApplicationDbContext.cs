using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using News.Models;

namespace ContructorNews.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }

        public new DbSet<User> Users { get; set; }

        public DbSet<Slide> Slides { get; set; }

        public DbSet<ParentComment> ParentComments { get; set; }
        public DbSet<ChildComment> ChildComments { get; set; }
    }
}
