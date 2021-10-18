using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetricsAgent.Migrations
{
    public partial class InitCPUMetrics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CpuProcessorTimeTotalMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RetrieveTime = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpuProcessorTimeTotalMetrics", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CpuProcessorTimeTotalMetrics");
        }
    }
}
