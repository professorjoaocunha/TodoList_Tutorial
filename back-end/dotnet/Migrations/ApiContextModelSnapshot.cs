﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodosAPI.Data;

namespace TodosAPI.Migrations
{
    [DbContext(typeof(ApiContext))]
    partial class ApiContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("TodosAPI.Models.Todo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Done")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<long?>("TodoListId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TodoListId");

                    b.ToTable("Todos");
                });

            modelBuilder.Entity("TodosAPI.Models.TodoList", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TodoLists");
                });

            modelBuilder.Entity("TodosAPI.Models.Todo", b =>
                {
                    b.HasOne("TodosAPI.Models.TodoList", null)
                        .WithMany("Todos")
                        .HasForeignKey("TodoListId");
                });
#pragma warning restore 612, 618
        }
    }
}
