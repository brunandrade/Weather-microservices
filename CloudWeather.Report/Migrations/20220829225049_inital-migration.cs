using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudWeather.Reports.Migrations
{
    public partial class initalmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AverageHigh = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageLow = table.Column<decimal>(type: "numeric", nullable: false),
                    RainfallTotalInches = table.Column<decimal>(type: "numeric", nullable: false),
                    SnowTotalInches = table.Column<decimal>(type: "numeric", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "report");
        }
    }
}
