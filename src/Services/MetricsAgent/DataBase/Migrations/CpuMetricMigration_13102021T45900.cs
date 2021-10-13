namespace MetricsAgent.DataBase.Migrations;
using FluentMigrator;

public class CpuMetricMigration_13102021T45900 : Migration
{
    public override void Up()
    {
        Create.Table(SD.CpuProcessorTimeTotalTableName)
            .WithColumn("Id").AsGuid().PrimaryKey().Identity()
            .WithColumn("RetrieveTime").AsInt64()
            .WithColumn("Value").AsInt32();
    }

    public override void Down()
    {
        Delete.Table(SD.CpuProcessorTimeTotalTableName);
    }
}