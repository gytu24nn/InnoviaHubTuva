using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c0cbc9b0-250c-4c52-aa4b-4680212ae577", "AQAAAAIAAYagAAAAEEOxKqDLl+oSADwS+aeRF4zls9qEs17hpM9Bv7UhR3qg1XDvS1lQUbQqL4b22Pa1oQ==", "96d85fdb-150f-4866-97d5-ff3a9215e258" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "082a0d89-4d2b-4305-87e8-370c05ec0039", "AQAAAAIAAYagAAAAEGNjlotaYc7+BGwIxI5JMb8NFgUj0WDx5kOAlo1lPmIHO/YABn44M+jWE043GaZNCQ==", "e8664e13-88a0-4ac7-95fa-73de15f7f0df" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
