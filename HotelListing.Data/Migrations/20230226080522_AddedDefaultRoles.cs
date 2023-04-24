using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelListing.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "52ae86ed-ccab-417c-b22a-3b780c40a95a", "f6d88b3f-0d48-440e-ac49-3c69a961ef80", "User", "USER" },
                    { "e451fc8b-a9dd-4e4c-b45a-c2357ae54531", "82c3efd7-4095-4640-a769-0b8dd6b59469", "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "52ae86ed-ccab-417c-b22a-3b780c40a95a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e451fc8b-a9dd-4e4c-b45a-c2357ae54531");
        }
    }
}
