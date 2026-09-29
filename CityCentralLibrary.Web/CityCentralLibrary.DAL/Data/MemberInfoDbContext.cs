using CityCentralLibrary.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CityCentralLibrary.DAL.Data
{
    public class MemberInfoDbContext : DbContext
    {
        private readonly string ConnectionString = "Data Source=localhost;database=MemberInfo;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Command Timeout=0";
        public DbSet<MemberInfo> MemberInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
