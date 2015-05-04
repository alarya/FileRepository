using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Data.Entity;

namespace CodeFirst.Models
{
   
    public class File
    {
        [Key]
        public virtual int FileId { get; set; }
        [DisplayName("File Name")]
        public virtual String FileName { get; set; }
        public virtual String Path { get; set; }
        [DisplayName("Owner")]
        public virtual String OwnerName { get; set; }
        public virtual String OwnerEmail { get; set; }
        public virtual String LastModifiedBy { get; set; }
        public virtual ICollection<Dependency> DependsOn { get; set; }
        public virtual ICollection<Dependency> RequiredBy { get; set; }
    }

    public class Owner
    {
        public virtual String OwnerId { get; set; }
        public virtual String OwnerName { get; set; }
        public virtual String OwnerEmail { get; set; }
    }

    public class Dependency
    {
        [Key]
        public int DependencyId { get; set; }
        [DisplayName("File 1")]
        public virtual int FileId1 { get; set; }
        [DisplayName("File 2")]
        public virtual int FileId2 { get; set; }
        
        public virtual File FileName1 { get; set; }
        
        public virtual File FileName2 { get; set; }
    }

    //Represents the DB Context used for interfacing model classes with the database
    public class FileRepositoryDb : DbContext
    {

        public System.Data.Entity.DbSet<CodeFirst.Models.File> Files { get; set; }

        public System.Data.Entity.DbSet<CodeFirst.Models.Dependency> Dependencies { get; set; }

       // public System.Data.Entity.DbSet<CodeFirst.Models.Owner> Owners { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dependency>()
                        .HasRequired(m => m.FileName1)
                        .WithMany(t => t.DependsOn)
                        .HasForeignKey(m => m.FileId1)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Dependency>()
                        .HasRequired(m => m.FileName2)
                        .WithMany(t => t.RequiredBy)
                        .HasForeignKey(m => m.FileId2)
                        .WillCascadeOnDelete(false);
        }

    }

    //This is the DB Initializer and recreates database whenever model changes
    public class FileRepositoryDbInitializer : DropCreateDatabaseAlways<FileRepositoryDb>
    {
        protected override void Seed(FileRepositoryDb context)
        {
            
            //Intializing contents of Files Table
            context.Files.Add(new File { 
                                    FileName = "File1.txt", Path = ".", OwnerName = "Alok", OwnerEmail = "alarya@syr.edu",
                                    LastModifiedBy = "Alok"
                                    });
            context.Files.Add(new File
            {
                FileName = "File2.txt",
                Path = ".",
                OwnerName = "Aman",
                OwnerEmail = "agupta@syr.edu",
                LastModifiedBy = "Aman"
            });

            context.Files.Add(new File
            {
                FileName = "File3.txt",
                OwnerName = "Sarthak",
                OwnerEmail = "sjain@syr.edu"
            });
            context.Files.Add(new File
            {
                FileName = "File4.txt",
                OwnerName = "Alok",
                OwnerEmail = "alarya@syr.edu"
            });

            //context.Owners.Add(new Owner
            //{
            //    OwnerName = "Alok",
            //    OwnerEmail = "alarya@syr.edu"
            //}
            //    );
            //context.Owners.Add(new Owner
            //{
            //    OwnerName = "Aman",
            //    OwnerEmail = "agupta@syr.edu"
            //}
            //   );
            //context.Owners.Add(new Owner
            //{
            //    OwnerName = "Saurabh",
            //    OwnerEmail = "sgupta@syr.edu"
            //}
            //   );
            //Initializing contents of Dependency table
            //context.Dependencies.Add(new Dependency
            //{
            //    FileName1 = new File{FileName =  "File1"},
            //    FileName2 = new File{FileName = "File3"}
            //});               
        }       
    }
}