using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackHotelBear.Migrations
{
    /// <inheritdoc />
    public partial class isinvoicepayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                table: "Payments");
        }
    }
}
