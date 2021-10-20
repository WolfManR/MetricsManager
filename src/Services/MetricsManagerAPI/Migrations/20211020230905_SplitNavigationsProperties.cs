using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetricsManagerAPI.Migrations
{
    public partial class SplitNavigationsProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CpuProcessorTimeTotalMetrics_Agents_AgentId",
                table: "CpuProcessorTimeTotalMetrics");

            migrationBuilder.DropIndex(
                name: "IX_CpuProcessorTimeTotalMetrics_AgentId",
                table: "CpuProcessorTimeTotalMetrics");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CpuProcessorTimeTotalMetrics_AgentId",
                table: "CpuProcessorTimeTotalMetrics",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CpuProcessorTimeTotalMetrics_Agents_AgentId",
                table: "CpuProcessorTimeTotalMetrics",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
