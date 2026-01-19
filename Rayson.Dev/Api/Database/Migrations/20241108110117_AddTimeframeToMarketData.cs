using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeframeToMarketData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "OpenPrice",
                table: "DailyMarketDatas",
                type: "numeric(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LowPrice",
                table: "DailyMarketDatas",
                type: "numeric(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "HighPrice",
                table: "DailyMarketDatas",
                type: "numeric(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosePrice",
                table: "DailyMarketDatas",
                type: "numeric(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdjustedPrice",
                table: "DailyMarketDatas",
                type: "numeric(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Timeframe",
                table: "DailyMarketDatas",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timeframe",
                table: "DailyMarketDatas");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenPrice",
                table: "DailyMarketDatas",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LowPrice",
                table: "DailyMarketDatas",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "HighPrice",
                table: "DailyMarketDatas",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosePrice",
                table: "DailyMarketDatas",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdjustedPrice",
                table: "DailyMarketDatas",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,9)");
        }
    }
}
