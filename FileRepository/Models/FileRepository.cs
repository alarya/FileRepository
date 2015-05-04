using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Data.Entity;

namespace FileRepository.Models
{
    [Table("Files")]
    public class File
    {
        [Key]
        public virtual int FileId { get; set; }
        [DisplayName("File Name")]
        public virtual String FileName { get; set; }
        public virtual String Type { get; set; }
        public virtual String Path { get; set; }
        [DisplayName("Owner")]
        public virtual String OwnerName { get; set; }
        public virtual String LastModifiedBy { get; set; }
        public virtual ICollection<Dependency> DependsOn { get; set; }
        public virtual ICollection<Dependency> RequiredBy { get; set; }
    }

    [Table("Dependencies")]
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

    public class FileNavigatorModel
    {
        public ICollection<File> FileForModel { get; set; }
        public ICollection<Boolean> Selected { get; set; }
    }

    public class FileRepositoryDb : DbContext
    {

        public System.Data.Entity.DbSet<FileRepository.Models.File> Files { get; set; }

        public System.Data.Entity.DbSet<FileRepository.Models.Dependency> Dependencies { get; set; }

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
    public class FileRepositoryDbInitializer : DropCreateDatabaseIfModelChanges<FileRepositoryDb>
    {
        protected override void Seed(FileRepositoryDb context)
        {

            //Intializing contents of Files Table
            context.Files.Add(new File
            {
                FileName = "File1.txt",
                Type = "file",
                Path = "root",
                OwnerName = "developer@syr.edu",
                LastModifiedBy = "developer@syr.edu"
            });
            context.Files.Add(new File
            {
                FileName = "File2.txt",
                Type = "file",
                Path = "root",
                OwnerName = "developer@syr.edu",
                LastModifiedBy = "developer@syr.edu"
            });

            context.Files.Add(new File
            {
                FileName = "Folder1",
                Type = "folder",
                Path = "root",
                OwnerName = "developer@syr.edu",
                LastModifiedBy = "developer@syr.edu"
            });
            context.Files.Add(new File
            {
                FileName = "Folder2",
                Type = "folder",
                Path = "root",
                OwnerName = "developer@syr.edu",
                LastModifiedBy = "developer@syr.edu"
            });
            context.Files.Add(new File
            {
                FileName = "File3.txt",
                Type = "file",
                Path = "root/Folder1",
                OwnerName = "developer@syr.edu",
                LastModifiedBy = "developer@syr.edu"
            });
            context.Files.Add(new File
            {
                FileName = "File4.txt",
                Type = "file",
                Path = "root/Folder2",
                OwnerName = "developer@syr.edu",
                LastModifiedBy = "developer@syr.edu"
            });
        }
    }
}