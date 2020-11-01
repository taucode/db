using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using System.Text;
using TauCode.Extensions;

namespace TauCode.Lab.Db.SqlClient.Tests.DbMigrator
{
    [TestFixture]
    public class SqlJsonMigratorTests : TestBase
    {
        #region Constructor

        [Test]
        [TestCase("dbo")]
        [TestCase(null)]
        public void Constructor_ValidArguments_RunsOk(string schemaName)
        {
            // Arrange

            // Act
            var migrator = new SqlJsonMigratorLab(
                this.Connection,
                schemaName,
                () => "{}",
                () => "{}");

            // Assert
            Assert.That(migrator.Connection, Is.SameAs(this.Connection));
            Assert.That(migrator.Factory, Is.SameAs(SqlUtilityFactoryLab.Instance));
            Assert.That(migrator.SchemaName, Is.EqualTo("dbo"));
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlJsonMigratorLab(
                null,
                "dbo",
                () => "{}",
                () => "{}"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ArgumentException()
        {
            // Arrange
            using var connection = new SqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new SqlJsonMigratorLab(
                connection,
                "dbo",
                () => "{}",
                () => "{}"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
            Assert.That(ex.Message, Does.StartWith("Connection should be opened."));
        }

        [Test]
        public void Constructor_MetadataJsonGetter_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlJsonMigratorLab(
                this.Connection,
                "dbo",
                null,
                () => "{}"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("metadataJsonGetter"));
        }

        [Test]
        public void Constructor_DataJsonGetter_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlJsonMigratorLab(
                this.Connection,
                "dbo",
                () => "{}",
                null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("dataJsonGetter"));
        }

        #endregion

        #region Migrate

        [Test]
        public void Migrate_ValidInput_RunsOk()
        {
            // Arrange
            this.Connection.CreateSchema("zeta");

            var migrator = new SqlJsonMigratorLab(
                this.Connection,
                "zeta",
                () => this.GetType().Assembly.GetResourceText("MigrateMetadataInput.json", true),
                () => this.GetType().Assembly.GetResourceText("MigrateDataCustomInput.json", true),
                x => x != "WorkInfo",
                (tableMold, row) =>
                {
                    if (tableMold.Name == "Person")
                    {
                        var birthday = (string)row.GetValue("Birthday");
                        var birthdayDateTime = DateTime.Parse(birthday.Substring("Month_".Length));
                        row.SetValue("Birthday", birthdayDateTime);

                        var genderString = (string)row.GetValue("Gender");
                        row.SetValue("Gender", (byte)genderString.ToEnum<Gender>());
                    }

                    return row;
                });

            // Act
            migrator.Migrate();

            // Assert

            #region metadata

            var sb = new StringBuilder();



            #endregion

            #region data

            #region Person

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "Person"), Is.EqualTo(2));

            var harvey = TestHelper.LoadRow(this.Connection, "zeta", "Person", 1);
            Assert.That(harvey["Id"], Is.EqualTo(1));
            Assert.That(harvey["Tag"], Is.EqualTo(new Guid("df601c43-fb4c-4a4d-ab05-e6bf5cfa68d1")));
            Assert.That(harvey["IsChecked"], Is.EqualTo(true));
            Assert.That(harvey["Birthday"], Is.EqualTo(DateTime.Parse("1939-05-13")));
            Assert.That(harvey["FirstName"], Is.EqualTo("Harvey"));
            Assert.That(harvey["LastName"], Is.EqualTo("Keitel"));
            Assert.That(harvey["Initials"], Is.EqualTo("HK"));
            Assert.That(harvey["Gender"], Is.EqualTo(100));

            var maria = TestHelper.LoadRow(this.Connection, "zeta", "Person", 2);
            Assert.That(maria["Id"], Is.EqualTo(2));
            Assert.That(maria["Tag"], Is.EqualTo(new Guid("374d413a-6287-448d-a4c1-918067c2312c")));
            Assert.That(maria["IsChecked"], Is.EqualTo(null));
            Assert.That(maria["Birthday"], Is.EqualTo(DateTime.Parse("1965-08-19")));
            Assert.That(maria["FirstName"], Is.EqualTo("Maria"));
            Assert.That(maria["LastName"], Is.EqualTo("Medeiros"));
            Assert.That(maria["Initials"], Is.EqualTo("MM"));
            Assert.That(maria["Gender"], Is.EqualTo(200));

            #endregion

            #region PersonData

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "PersonData"), Is.EqualTo(2));

            var harveyData = TestHelper.LoadRow(this.Connection, "zeta", "PersonData", 101);
            Assert.That(harveyData["Id"], Is.EqualTo(101));
            Assert.That(harveyData["PersonId"], Is.EqualTo(1));
            Assert.That(harveyData["BestAge"], Is.EqualTo(42));
            Assert.That(harveyData["Hash"], Is.EqualTo(791888333));
            Assert.That(harveyData["Height"], Is.EqualTo(175.5));
            Assert.That(harveyData["Weight"], Is.EqualTo(68.9));
            Assert.That(harveyData["UpdatedAt"], Is.EqualTo(DateTime.Parse("1996-11-02T11:12:13")));
            Assert.That(harveyData["Signature"], Is.EqualTo(new byte[] { 0xde, 0xfe, 0xca, 0x77 }));

            var mariaData = TestHelper.LoadRow(this.Connection, "zeta", "PersonData", 201);
            Assert.That(mariaData["Id"], Is.EqualTo(201));
            Assert.That(mariaData["PersonId"], Is.EqualTo(2));
            Assert.That(mariaData["BestAge"], Is.EqualTo(26));
            Assert.That(mariaData["Hash"], Is.EqualTo(901014134123412));
            Assert.That(mariaData["Height"], Is.EqualTo(168.5));
            Assert.That(mariaData["Weight"], Is.EqualTo(54.7));
            Assert.That(mariaData["UpdatedAt"], Is.EqualTo(DateTime.Parse("1994-01-11T16:01:02")));
            Assert.That(mariaData["Signature"], Is.EqualTo(new byte[] { 0x15, 0x99, 0xaa, 0xbb }));

            #endregion

            #region Photo

            // todo: PicMigrationHarvey and so forth

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "Photo"), Is.EqualTo(4));

            var harveyPhoto1 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PH-1");
            Assert.That(harveyPhoto1["Id"], Is.EqualTo("PH-1"));
            Assert.That(harveyPhoto1["PersonDataId"], Is.EqualTo(101));
            Assert.That(harveyPhoto1["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey1.png", true)));
            Assert.That(harveyPhoto1["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey1Thumb.png", true)));
            Assert.That(harveyPhoto1["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("1997-12-12T11:12:13+00:00")));
            Assert.That(harveyPhoto1["ValidUntil"], Is.EqualTo(DateTime.Parse("1998-12-12")));

            var harveyPhoto2 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PH-2");
            Assert.That(harveyPhoto2["Id"], Is.EqualTo("PH-2"));
            Assert.That(harveyPhoto2["PersonDataId"], Is.EqualTo(101));
            Assert.That(harveyPhoto2["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey2.png", true)));
            Assert.That(harveyPhoto2["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey2Thumb.png", true)));
            Assert.That(harveyPhoto2["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("1991-01-01T02:16:17+00:00")));
            Assert.That(harveyPhoto2["ValidUntil"], Is.EqualTo(DateTime.Parse("1993-09-09")));

            var mariaPhoto1 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PM-1");
            Assert.That(mariaPhoto1["Id"], Is.EqualTo("PM-1"));
            Assert.That(mariaPhoto1["PersonDataId"], Is.EqualTo(201));
            Assert.That(mariaPhoto1["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria1.png", true)));
            Assert.That(mariaPhoto1["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria1Thumb.png", true)));
            Assert.That(mariaPhoto1["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("1998-04-05T08:09:22+00:00")));
            Assert.That(mariaPhoto1["ValidUntil"], Is.EqualTo(DateTime.Parse("1999-04-05")));

            var mariaPhoto2 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PM-2");
            Assert.That(mariaPhoto2["Id"], Is.EqualTo("PM-2"));
            Assert.That(mariaPhoto2["PersonDataId"], Is.EqualTo(201));
            Assert.That(mariaPhoto2["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria2.png", true)));
            Assert.That(mariaPhoto2["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria2Thumb.png", true)));
            Assert.That(mariaPhoto2["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("2001-06-01T11:12:19+00:00")));
            Assert.That(mariaPhoto2["ValidUntil"], Is.EqualTo(DateTime.Parse("2002-07-07")));

            #endregion

            #region WorkInfo

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "WorkInfo"), Is.EqualTo(0));

            #endregion

            #endregion
        }

        [Test]
        public void Migrate_TablePredicateIsNull_MigratesAll()
        {
            // Arrange
            this.Connection.CreateSchema("zeta");

            var migrator = new SqlJsonMigratorLab(
                this.Connection,
                "zeta",
                () => this.GetType().Assembly.GetResourceText("MigrateMetadataInput.json", true),
                () => this.GetType().Assembly.GetResourceText("MigrateDataInput.json", true),
                x => x != "WorkInfo",
                (tableMold, row) =>
                {
                    if (tableMold.Name == "Person")
                    {
                        var birthday = (string)row.GetValue("Birthday");
                        var birthdayDateTime = DateTime.Parse(birthday.Substring("Month_".Length));
                        row.SetValue("Birthday", birthdayDateTime);
                    }

                    return row;
                });

            // Act
            migrator.Migrate();

            // Assert

            #region data

            #region Person

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "Person"), Is.EqualTo(2));

            var harvey = TestHelper.LoadRow(this.Connection, "zeta", "Person", 1);
            Assert.That(harvey["Id"], Is.EqualTo(1));
            Assert.That(harvey["Tag"], Is.EqualTo(new Guid("df601c43-fb4c-4a4d-ab05-e6bf5cfa68d1")));
            Assert.That(harvey["IsChecked"], Is.EqualTo(true));
            Assert.That(harvey["Birthday"], Is.EqualTo(DateTime.Parse("1939-05-13")));
            Assert.That(harvey["FirstName"], Is.EqualTo("Harvey"));
            Assert.That(harvey["LastName"], Is.EqualTo("Keitel"));
            Assert.That(harvey["Initials"], Is.EqualTo("HK"));
            Assert.That(harvey["Gender"], Is.EqualTo(100));

            var maria = TestHelper.LoadRow(this.Connection, "zeta", "Person", 2);
            Assert.That(maria["Id"], Is.EqualTo(2));
            Assert.That(maria["Tag"], Is.EqualTo(new Guid("374d413a-6287-448d-a4c1-918067c2312c")));
            Assert.That(maria["IsChecked"], Is.EqualTo(null));
            Assert.That(maria["Birthday"], Is.EqualTo(DateTime.Parse("1965-08-19")));
            Assert.That(maria["FirstName"], Is.EqualTo("Maria"));
            Assert.That(maria["LastName"], Is.EqualTo("Medeiros"));
            Assert.That(maria["Initials"], Is.EqualTo("MM"));
            Assert.That(maria["Gender"], Is.EqualTo(200));

            #endregion

            #region PersonData

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "PersonData"), Is.EqualTo(2));

            var harveyData = TestHelper.LoadRow(this.Connection, "zeta", "PersonData", 101);
            Assert.That(harveyData["Id"], Is.EqualTo(101));
            Assert.That(harveyData["PersonId"], Is.EqualTo(1));
            Assert.That(harveyData["BestAge"], Is.EqualTo(42));
            Assert.That(harveyData["Hash"], Is.EqualTo(791888333));
            Assert.That(harveyData["Height"], Is.EqualTo(175.5));
            Assert.That(harveyData["Weight"], Is.EqualTo(68.9));
            Assert.That(harveyData["UpdatedAt"], Is.EqualTo(DateTime.Parse("1996-11-02T11:12:13")));
            Assert.That(harveyData["Signature"], Is.EqualTo(new byte[] { 0xde, 0xfe, 0xca, 0x77 }));

            var mariaData = TestHelper.LoadRow(this.Connection, "zeta", "PersonData", 201);
            Assert.That(mariaData["Id"], Is.EqualTo(201));
            Assert.That(mariaData["PersonId"], Is.EqualTo(2));
            Assert.That(mariaData["BestAge"], Is.EqualTo(26));
            Assert.That(mariaData["Hash"], Is.EqualTo(901014134123412));
            Assert.That(mariaData["Height"], Is.EqualTo(168.5));
            Assert.That(mariaData["Weight"], Is.EqualTo(54.7));
            Assert.That(mariaData["UpdatedAt"], Is.EqualTo(DateTime.Parse("1994-01-11T16:01:02")));
            Assert.That(mariaData["Signature"], Is.EqualTo(new byte[] { 0x15, 0x99, 0xaa, 0xbb }));

            #endregion

            #region Photo

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "Photo"), Is.EqualTo(4));

            var harveyPhoto1 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PH-1");
            Assert.That(harveyPhoto1["Id"], Is.EqualTo("PH-1"));
            Assert.That(harveyPhoto1["PersonDataId"], Is.EqualTo(101));
            Assert.That(harveyPhoto1["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey1.png", true)));
            Assert.That(harveyPhoto1["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey1Thumb.png", true)));
            Assert.That(harveyPhoto1["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("1997-12-12T11:12:13+00:00")));
            Assert.That(harveyPhoto1["ValidUntil"], Is.EqualTo(DateTime.Parse("1998-12-12")));

            var harveyPhoto2 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PH-2");
            Assert.That(harveyPhoto2["Id"], Is.EqualTo("PH-2"));
            Assert.That(harveyPhoto2["PersonDataId"], Is.EqualTo(101));
            Assert.That(harveyPhoto2["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey2.png", true)));
            Assert.That(harveyPhoto2["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicHarvey2Thumb.png", true)));
            Assert.That(harveyPhoto2["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("1991-01-01T02:16:17+00:00")));
            Assert.That(harveyPhoto2["ValidUntil"], Is.EqualTo(DateTime.Parse("1993-09-09")));

            var mariaPhoto1 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PM-1");
            Assert.That(mariaPhoto1["Id"], Is.EqualTo("PM-1"));
            Assert.That(mariaPhoto1["PersonDataId"], Is.EqualTo(201));
            Assert.That(mariaPhoto1["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria1.png", true)));
            Assert.That(mariaPhoto1["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria1Thumb.png", true)));
            Assert.That(mariaPhoto1["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("1998-04-05T08:09:22+00:00")));
            Assert.That(mariaPhoto1["ValidUntil"], Is.EqualTo(DateTime.Parse("1999-04-05")));

            var mariaPhoto2 = TestHelper.LoadRow(this.Connection, "zeta", "Photo", "PM-2");
            Assert.That(mariaPhoto2["Id"], Is.EqualTo("PM-2"));
            Assert.That(mariaPhoto2["PersonDataId"], Is.EqualTo(201));
            Assert.That(mariaPhoto2["Content"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria2.png", true)));
            Assert.That(mariaPhoto2["ContentThumbnail"],
                Is.EqualTo(this.GetType().Assembly.GetResourceBytes("PicMaria2Thumb.png", true)));
            Assert.That(mariaPhoto2["TakenAt"], Is.EqualTo(DateTimeOffset.Parse("2001-06-01T11:12:19+00:00")));
            Assert.That(mariaPhoto2["ValidUntil"], Is.EqualTo(DateTime.Parse("2002-07-07")));

            #endregion

            #region WorkInfo

            Assert.That(TestHelper.GetTableRowCount(this.Connection, "zeta", "WorkInfo"), Is.EqualTo(0));

            #endregion


            #endregion
        }

        [Test]
        public void Migrate_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert

            throw new NotImplementedException();
        }


        #endregion
    }
}
