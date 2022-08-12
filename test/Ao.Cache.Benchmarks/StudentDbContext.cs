using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks
{
    public class StudentDbContext:DbContext
    {
        public StudentDbContext(DbContextOptions options) : base(options)
        {
        }

        protected StudentDbContext()
        {
        }

        public DbSet<Student> Students => Set<Student>();
    }
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
