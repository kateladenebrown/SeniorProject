﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GameEF
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GameEntities : DbContext
    {
        public GameEntities()
            : base("name=GameEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameState> GameStates { get; set; }
        public virtual DbSet<GameStatusLookup> GameStatusLookups { get; set; }
        public virtual DbSet<GameUser> GameUsers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserStatusLookup> UserStatusLookups { get; set; }
    }
}
