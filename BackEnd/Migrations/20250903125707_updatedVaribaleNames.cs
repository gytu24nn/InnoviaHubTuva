using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class updatedVaribaleNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TimeSlots",
                newName: "TimeSlotsId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ResourceTypes",
                newName: "ResourceTypeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Resources",
                newName: "ResourcesId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bookings",
                newName: "BookingId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "User", "USER" }
                });

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

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "1" },
                    { "2", "2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.RenameColumn(
                name: "TimeSlotsId",
                table: "TimeSlots",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ResourceTypeId",
                table: "ResourceTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ResourcesId",
                table: "Resources",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Bookings",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "varchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "89aa8496-773c-49b7-a5d0-94a8fd3ae0ae", "IdentityUser", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEHbVOavtaAipa9F4Z5lavm3t3W66RMLYBoBN2avMe4IDpcMb8Lx7O0D7YH27BnoxcA==", null, false, "3339f1ab-6177-4eb0-8d3d-c8d9f802c7da", false, "admin" },
                    { "2", 0, "3cacb902-48d2-47a9-88f4-b7fd48539693", "IdentityUser", "user@example.com", true, false, null, "USER@EXAMPLE.COM", "USER", "AQAAAAIAAYagAAAAEKqnLou+I+lqmJ5wsEiKOq22Grko+I9pv029gKZmFM2nxttDFT7mc7QLKpJLdYjtKQ==", null, false, "5df58dcf-f8bf-4fe1-93dc-29750d10a8f3", false, "user" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 1, "role", "admin", "1" });
        }
    }
}
