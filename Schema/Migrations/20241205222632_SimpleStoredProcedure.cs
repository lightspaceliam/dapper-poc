using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schema.Migrations
{
    /// <inheritdoc />
    public partial class SimpleStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Drop Stored Procedure if exists.
                IF EXISTS (SELECT   * 
                        FROM    sys.objects 
                        WHERE   [type] = 'P'
                                AND OBJECT_ID = OBJECT_ID('[dbo].[uspGetPeopleByFaculty]'))
                BEGIN
                    DROP PROCEDURE [dbo].[uspGetPeopleByFaculty]
                END

                --  Create Stored Procedure now that we are confident it does not exist.
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE PROCEDURE dbo.uspGetPeopleByFaculty
                (
                    @FacultyName nvarchar(50) = NULL
                )
                AS
                BEGIN
                    -- SET NOCOUNT ON added to prevent extra result sets from
                    -- interfering with SELECT statements.
                    SET NOCOUNT ON

                    -- Insert statements for procedure here
                    SELECT  P.Id
                            , P.FirstName
                            , P.LastName
                            , P.DateOfBirth
                            , P.FacultyId
                            , P.Created
                            , P.Updated
                            , P.PeriodStart
                            , P.PeriodEnd
                    FROM    Faculties AS F
                            INNER JOIN People AS P
                                ON F.Id = P.FacultyId
                    WHERE   F.Name = @FacultyName
                END
                GO
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT   * 
                        FROM    sys.objects 
                        WHERE   [type] = 'P'
                                AND OBJECT_ID = OBJECT_ID('[dbo].[uspGetPeopleByFaculty]'))
                BEGIN
                    DROP PROCEDURE [dbo].[uspGetPeopleByFaculty]
                END
            ");
        }
    }
}
