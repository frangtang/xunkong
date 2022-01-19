﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunkong.Desktop.Services;

#nullable disable

namespace Xunkong.Desktop.Migrations
{
    [DbContext(typeof(XunkongDbContext))]
    [Migration("20220116120440_FirstInit")]
    partial class FirstInit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("Xunkong.Core.Hoyolab.DailyNoteInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentExpeditionNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentHomeCoin")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentResin")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FinishedTaskNumber")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("HomeCoinRecoveryTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsExtraTaskRewardReceived")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxExpeditionNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxHomeCoin")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxResin")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RemainResinDiscountNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ResinDiscountLimitedNumber")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("ResinRecoveryTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalTaskNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Uid")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("DailyNote_Items");
                });

            modelBuilder.Entity("Xunkong.Core.Hoyolab.UserGameRoleInfo", b =>
                {
                    b.Property<int>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cookie")
                        .HasColumnType("TEXT");

                    b.Property<string>("GameBiz")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsChosen")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsOfficial")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nickname")
                        .HasColumnType("TEXT");

                    b.Property<int>("Region")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RegionName")
                        .HasColumnType("TEXT");

                    b.HasKey("Uid");

                    b.ToTable("Genshin_Users");
                });

            modelBuilder.Entity("Xunkong.Core.Hoyolab.UserInfo", b =>
                {
                    b.Property<int>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Avatar")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Cookie")
                        .HasColumnType("TEXT");

                    b.Property<int>("Gender")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Introduce")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nickname")
                        .HasColumnType("TEXT");

                    b.Property<string>("Pendant")
                        .HasColumnType("TEXT");

                    b.HasKey("Uid");

                    b.ToTable("Hoyolab_Users");
                });

            modelBuilder.Entity("Xunkong.Core.Metadata.CharacterInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Affiliation")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Birthday")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Card")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConstllationName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("Element")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FaceIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("GachaCard")
                        .HasColumnType("TEXT");

                    b.Property<string>("GachaSplash")
                        .HasColumnType("TEXT");

                    b.Property<string>("Gender")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long?>("NameTextMapHash")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Portrait")
                        .HasColumnType("TEXT");

                    b.Property<int>("Rarity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SideIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("WeaponType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Element");

                    b.HasIndex("Gender");

                    b.HasIndex("Name");

                    b.HasIndex("NameTextMapHash");

                    b.HasIndex("Rarity");

                    b.HasIndex("WeaponType");

                    b.ToTable("Info_Character");
                });

            modelBuilder.Entity("Xunkong.Core.Metadata.ConstellationInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Effect")
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CharacterInfoId");

                    b.ToTable("Info_Character_Constellation");
                });

            modelBuilder.Entity("Xunkong.Core.Metadata.WeaponInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AwakenIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("GachaIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long>("NameTextMapHash")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rarity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WeaponType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Info_Weapon");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssAvatar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AvatarId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rarity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SpiralAbyssBattleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AvatarId");

                    b.HasIndex("Level");

                    b.HasIndex("Rarity");

                    b.HasIndex("SpiralAbyssBattleId");

                    b.ToTable("SpiralAbyss_Avatars");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssBattle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SpiralAbyssLevelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SpiralAbyssLevelId");

                    b.HasIndex("Time");

                    b.ToTable("SpiralAbyss_Battles");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssFloor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsUnlock")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxStar")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SettleTime")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("SpiralAbyssInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Star")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Index");

                    b.HasIndex("SpiralAbyssInfoId");

                    b.HasIndex("Star");

                    b.ToTable("SpiralAbyss_Floors");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsUnlock")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MaxFloor")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalBattleCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalStar")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalWinCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Uid")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MaxFloor");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("TotalBattleCount");

                    b.HasIndex("TotalStar");

                    b.HasIndex("TotalWinCount");

                    b.HasIndex("Uid", "ScheduleId")
                        .IsUnique();

                    b.ToTable("SpiralAbyss_Infos");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxStar")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SpiralAbyssFloorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Star")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SpiralAbyssFloorId");

                    b.ToTable("SpiralAbyss_Levels");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssRank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarIcon")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("AvatarId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rarity")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SpiralAbyssInfo_DamageRank")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SpiralAbyssInfo_DefeatRank")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SpiralAbyssInfo_EnergySkillRank")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SpiralAbyssInfo_NormalSkillRank")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SpiralAbyssInfo_RevealRank")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SpiralAbyssInfo_TakeDamageRank")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SpiralAbyssInfo_DamageRank");

                    b.HasIndex("SpiralAbyssInfo_DefeatRank");

                    b.HasIndex("SpiralAbyssInfo_EnergySkillRank");

                    b.HasIndex("SpiralAbyssInfo_NormalSkillRank");

                    b.HasIndex("SpiralAbyssInfo_RevealRank");

                    b.HasIndex("SpiralAbyssInfo_TakeDamageRank");

                    b.HasIndex("Type");

                    b.ToTable("SpiralAbyss_Ranks");
                });

            modelBuilder.Entity("Xunkong.Core.TravelRecord.TravelRecordAwardItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActionName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Month")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Uid")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ActionId");

                    b.HasIndex("Month");

                    b.HasIndex("Time");

                    b.HasIndex("Type");

                    b.HasIndex("Uid");

                    b.HasIndex("Year");

                    b.ToTable("TravelRecord_AwardItems");
                });

            modelBuilder.Entity("Xunkong.Core.TravelRecord.TravelRecordMonthData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentMora")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentPrimogems")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentPrimogemsLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LastMora")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LastPrimogems")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Month")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoraChangeRate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PrimogemsChangeRate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Uid")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Year", "Month");

                    b.HasIndex("Uid", "Year", "Month")
                        .IsUnique();

                    b.ToTable("TravelRecord_MonthDatas");
                });

            modelBuilder.Entity("Xunkong.Core.TravelRecord.TravelRecordPrimogemsMonthGroupStats", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActionName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Month")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Percent")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TravelRecordMonthDataId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Uid")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TravelRecordMonthDataId");

                    b.ToTable("TravelTecord_GroupStats");
                });

            modelBuilder.Entity("Xunkong.Core.Wish.WishEventInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Rank4UpItems")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Rank5UpItems")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("WishType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("_EndTimeString")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("EndTime");

                    b.Property<string>("_StartTimeString")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("WishType");

                    b.HasIndex("WishType", "_StartTimeString")
                        .IsUnique();

                    b.ToTable("Info_WishEvent");
                });

            modelBuilder.Entity("Xunkong.Core.Wish.WishlogAuthkeyItem", b =>
                {
                    b.Property<int>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasMaxLength(4096)
                        .HasColumnType("TEXT");

                    b.HasKey("Uid");

                    b.ToTable("Wishlog_Authkeys");
                });

            modelBuilder.Entity("Xunkong.Core.Wish.WishlogItem", b =>
                {
                    b.Property<int>("Uid")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("QueryType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RankType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("TEXT");

                    b.Property<int>("WishType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Uid", "Id");

                    b.HasIndex("ItemType");

                    b.HasIndex("Name");

                    b.HasIndex("QueryType");

                    b.HasIndex("RankType");

                    b.HasIndex("Time");

                    b.HasIndex("WishType");

                    b.ToTable("Wishlog_Items");
                });

            modelBuilder.Entity("Xunkong.Desktop.Models.UserSettingModel", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("Xunkong.Core.Metadata.ConstellationInfo", b =>
                {
                    b.HasOne("Xunkong.Core.Metadata.CharacterInfo", null)
                        .WithMany("Constellations")
                        .HasForeignKey("CharacterInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssAvatar", b =>
                {
                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssBattle", null)
                        .WithMany("Avatars")
                        .HasForeignKey("SpiralAbyssBattleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssBattle", b =>
                {
                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssLevel", null)
                        .WithMany("Battles")
                        .HasForeignKey("SpiralAbyssLevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssFloor", b =>
                {
                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", null)
                        .WithMany("Floors")
                        .HasForeignKey("SpiralAbyssInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssLevel", b =>
                {
                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssFloor", null)
                        .WithMany("Levels")
                        .HasForeignKey("SpiralAbyssFloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssRank", b =>
                {
                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", null)
                        .WithMany("DamageRank")
                        .HasForeignKey("SpiralAbyssInfo_DamageRank")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", null)
                        .WithMany("DefeatRank")
                        .HasForeignKey("SpiralAbyssInfo_DefeatRank")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", null)
                        .WithMany("EnergySkillRank")
                        .HasForeignKey("SpiralAbyssInfo_EnergySkillRank")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", null)
                        .WithMany("NormalSkillRank")
                        .HasForeignKey("SpiralAbyssInfo_NormalSkillRank")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", null)
                        .WithMany("RevealRank")
                        .HasForeignKey("SpiralAbyssInfo_RevealRank")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", null)
                        .WithMany("TakeDamageRank")
                        .HasForeignKey("SpiralAbyssInfo_TakeDamageRank")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Xunkong.Core.TravelRecord.TravelRecordPrimogemsMonthGroupStats", b =>
                {
                    b.HasOne("Xunkong.Core.TravelRecord.TravelRecordMonthData", null)
                        .WithMany("PrimogemsGroupBy")
                        .HasForeignKey("TravelRecordMonthDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Xunkong.Core.Metadata.CharacterInfo", b =>
                {
                    b.Navigation("Constellations");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssBattle", b =>
                {
                    b.Navigation("Avatars");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssFloor", b =>
                {
                    b.Navigation("Levels");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssInfo", b =>
                {
                    b.Navigation("DamageRank");

                    b.Navigation("DefeatRank");

                    b.Navigation("EnergySkillRank");

                    b.Navigation("Floors");

                    b.Navigation("NormalSkillRank");

                    b.Navigation("RevealRank");

                    b.Navigation("TakeDamageRank");
                });

            modelBuilder.Entity("Xunkong.Core.SpiralAbyss.SpiralAbyssLevel", b =>
                {
                    b.Navigation("Battles");
                });

            modelBuilder.Entity("Xunkong.Core.TravelRecord.TravelRecordMonthData", b =>
                {
                    b.Navigation("PrimogemsGroupBy");
                });
#pragma warning restore 612, 618
        }
    }
}
