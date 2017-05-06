using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ChessPortal.Data.Entities;
using ChessPortal.Logic.Chess;

namespace ChessPortal.Data.Migrations
{
    [DbContext(typeof(ChessPortalContext))]
    [Migration("20170506132155_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ChessPortal.Data.Entities.ChallengeAcceptEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChallengeId");

                    b.Property<string>("PlayerId");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.HasIndex("PlayerId");

                    b.ToTable("AcceptedChallenges");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.ChallengeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Color");

                    b.Property<int>("DaysPerMove");

                    b.Property<string>("PlayerId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.ChessPlayer", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<int>("NumberOfDrawnGames");

                    b.Property<int>("NumberOfLostGames");

                    b.Property<int>("NumberOfProblemsFailed");

                    b.Property<int>("NumberOfProblemsSolved");

                    b.Property<int>("NumberOfWonGames");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.ChessProblemEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ChessProblemId");

                    b.Property<int>("MoveOffsetNumber");

                    b.Property<string>("PlayerId");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("ChessProblems");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.DrawRequestEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChallengeId");

                    b.Property<string>("PlayerId");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.HasIndex("PlayerId");

                    b.ToTable("DrawRequests");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.MoveEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChallengeId");

                    b.Property<int>("Color");

                    b.Property<int>("FromX");

                    b.Property<int>("FromY");

                    b.Property<DateTime>("MoveDate");

                    b.Property<int>("MoveNumber");

                    b.Property<int>("Piece");

                    b.Property<int?>("PromoteTo");

                    b.Property<int>("ToX");

                    b.Property<int>("ToY");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.ToTable("Moves");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.ChallengeAcceptEntity", b =>
                {
                    b.HasOne("ChessPortal.Data.Entities.ChallengeEntity", "Challenge")
                        .WithMany()
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ChessPortal.Data.Entities.ChessPlayer", "AcceptingPlayer")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.ChallengeEntity", b =>
                {
                    b.HasOne("ChessPortal.Data.Entities.ChessPlayer", "Player")
                        .WithMany("Challenges")
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.ChessProblemEntity", b =>
                {
                    b.HasOne("ChessPortal.Data.Entities.ChessPlayer", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.DrawRequestEntity", b =>
                {
                    b.HasOne("ChessPortal.Data.Entities.ChallengeEntity", "Challenge")
                        .WithMany("DrawRequests")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ChessPortal.Data.Entities.ChessPlayer", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("ChessPortal.Data.Entities.MoveEntity", b =>
                {
                    b.HasOne("ChessPortal.Data.Entities.ChallengeEntity", "Challenge")
                        .WithMany("Moves")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ChessPortal.Data.Entities.ChessPlayer")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ChessPortal.Data.Entities.ChessPlayer")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ChessPortal.Data.Entities.ChessPlayer")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
