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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ReSharper disable once StringLiteralTypo Намеренное написание
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_accounts_ownerid;");
            // ReSharper disable once StringLiteralTypo Намеренное написание
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_transactions_accountid_date;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_transactions_date_brin;");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS accrue_interest(UUID);");
        }
    }
}
