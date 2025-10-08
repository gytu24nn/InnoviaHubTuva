using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedChatMessagesDBset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIChatMessages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFromAI = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIChatMessages", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "017eb576-78cb-470b-a0ac-14e597d91ec6", "AQAAAAIAAYagAAAAEOwrBqNdUERpiroWdqBA2zk5UUEqBTmNm0oB19avAuipobOemWxpQ4YiMLtLskDdsg==", "35c385c4-4dd9-43d1-a380-16c9f2f6db74" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3e46ce5d-cb0b-4f7d-b857-2ee559b065a5", "AQAAAAIAAYagAAAAEO6SMKonCOpYPVHGhR3yOCUUqpYWdq4Z/ZW7RtjGoy3/3m8Eeef9ioX3vIkAHce/2g==", "b489eb6b-44bf-4cdf-8001-ff3287cecfc8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIChatMessages");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "30e23344-d385-40aa-88a0-985a4d1776b5", "AQAAAAIAAYagAAAAEDmqIlCk19qmYMJc0Oeiufld6QVFw7YhyJfN0KcfFlVNBJO5SMVKgbxydyHXeE8t7Q==", "fbc451f7-d6e8-4bd7-b2a6-d07c5f15d25b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "199c5f35-ef8c-4134-980c-339004e34b1f", "AQAAAAIAAYagAAAAEMAVMF+BkqgzSaIgoNhoc5QWSJOVHC2L8LSM7ycaNdFhDvqULob80nDjA0sfilQ70A==", "094bce2c-f7d3-4277-be8c-87b5618d2ba6" });
        }
    }
}
