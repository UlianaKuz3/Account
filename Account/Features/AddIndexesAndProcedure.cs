using Microsoft.EntityFrameworkCore.Migrations;

namespace AccountServices.Features
{
#nullable disable

    // ReSharper disable once UnusedMember.Global Нужно для миграций
    public class AddIndexesAndProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ReSharper disable once StringLiteralTypo Намеренное написание
            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_accounts_ownerid
        ON ""Accounts"" USING hash(""ownerId"");
    ");

            // ReSharper disable once StringLiteralTypo Намеренное написание
            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_transactions_accountid_date
        ON ""Transactions""(""accountId"", ""date"");
    ");

            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_transactions_date_brin
        ON ""Transactions"" USING brin(""date"");
    ");

            // ReSharper disable once StringLiteralTypo Намеренное написание
            migrationBuilder.Sql(@"
        CREATE OR REPLACE PROCEDURE accrue_interest(account_id UUID)
        LANGUAGE plpgsql
        AS $$
        BEGIN
            UPDATE ""Accounts""
            SET ""balance"" = ""balance"" + (""balance"" * 0.05 / 365)
            WHERE ""id"" = account_id;
        END;
        $$;
    ");
            migrationBuilder.Sql("""
        CREATE TABLE IF NOT EXISTS outbox_messages(
            "Id" uuid PRIMARY KEY,
            "CreatedAt" timestamptz NOT NULL,
            "Type" text NOT NULL,
            "RoutingKey" text NOT NULL,
            "Payload" jsonb NOT NULL,
            "CorrelationId" text NULL,
            "CausationId" text NULL,
            "Attempts" int NOT NULL DEFAULT 0,
            "PublishedAt" timestamptz NULL,
            "LastError" text NULL
        );
        CREATE INDEX IF NOT EXISTS ix_outbox_published ON outbox_messages("PublishedAt");
        CREATE INDEX IF NOT EXISTS ix_outbox_pub_attempts ON outbox_messages("PublishedAt","Attempts");
    """);

            migrationBuilder.Sql("""
        CREATE TABLE IF NOT EXISTS inbox_consumed(
            "MessageId" uuid PRIMARY KEY,
            "Handler" text NOT NULL,
            "ProcessedAt" timestamptz NOT NULL
        );
    """);

            migrationBuilder.Sql("""
        CREATE TABLE IF NOT EXISTS inbox_dead_letters(
            "MessageId" uuid PRIMARY KEY,
            "Handler" text NOT NULL,
            "ReceivedAt" timestamptz NOT NULL,
            "Payload" text NOT NULL,
            "Error" text NOT NULL
        );
    """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ReSharper disable once StringLiteralTypo Намеренное написание
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_accounts_ownerid;");
            // ReSharper disable once StringLiteralTypo Намеренное написание
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_transactions_accountid_date;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_transactions_date_brin;");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS accrue_interest(UUID);");

            migrationBuilder.Sql("DROP TABLE IF EXISTS inbox_dead_letters;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS inbox_consumed;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS outbox_messages;");
        }
    }
}
