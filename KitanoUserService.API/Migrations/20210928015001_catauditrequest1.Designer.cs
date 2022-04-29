﻿// <auto-generated />
using System;
using KitanoUserService.API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KitanoUserService.API.Migrations
{
    [DbContext(typeof(KitanoSqlContext))]
    [Migration("20210928015001_catauditrequest1")]
    partial class catauditrequest1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.Category.CatAuditRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("createdate");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("createdAt");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("createdBy");

                    b.Property<DateTime>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deletedAt");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deletedBy");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modifiedAt");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modifiedBy");

                    b.Property<string>("TypeCode")
                        .HasColumnType("text")
                        .HasColumnName("typecode");

                    b.Property<string>("TypeName")
                        .HasColumnType("text")
                        .HasColumnName("typename");

                    b.HasKey("Id");

                    b.ToTable("CatAuditRequest");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deleted_by");

                    b.Property<int?>("DepartmentTypeID")
                        .HasColumnType("integer")
                        .HasColumnName("department_type_id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modified_by");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer")
                        .HasColumnName("parent_id");

                    b.HasKey("Id");

                    b.ToTable("department");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.UnitType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("createdAt");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("createdBy");

                    b.Property<DateTime>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deletedAt");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deletedBy");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modifiedAt");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modifiedBy");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<bool?>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("UnitType");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Avartar")
                        .HasColumnType("text")
                        .HasColumnName("avartar");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DateOfJoining")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_of_joining");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("integer")
                        .HasColumnName("department_id");

                    b.Property<int?>("DomainId")
                        .HasColumnType("integer")
                        .HasColumnName("domain_Id");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("LastOnline")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_online_at");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<string>("Password")
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<int?>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.Property<string>("SaltKey")
                        .HasColumnType("text")
                        .HasColumnName("salt");

                    b.Property<string>("UserName")
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.Property<int?>("UsersType")
                        .HasColumnType("integer")
                        .HasColumnName("users_type");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.UsersGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deleted_by");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_at");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modified_by");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("users_group");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.UsersGroupMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("group_id")
                        .HasColumnType("integer");

                    b.Property<int?>("users_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("group_id");

                    b.HasIndex("users_id");

                    b.ToTable("users_group_mapping");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.UsersGroupMapping", b =>
                {
                    b.HasOne("KitanoUserService.API.Models.MigrationsModels.UsersGroup", "UsersGroup")
                        .WithMany("UsersGroupMappings")
                        .HasForeignKey("group_id");

                    b.HasOne("KitanoUserService.API.Models.MigrationsModels.Users", "Users")
                        .WithMany("UsersGroupMappings")
                        .HasForeignKey("users_id");

                    b.Navigation("Users");

                    b.Navigation("UsersGroup");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.Users", b =>
                {
                    b.Navigation("UsersGroupMappings");
                });

            modelBuilder.Entity("KitanoUserService.API.Models.MigrationsModels.UsersGroup", b =>
                {
                    b.Navigation("UsersGroupMappings");
                });
#pragma warning restore 612, 618
        }
    }
}
