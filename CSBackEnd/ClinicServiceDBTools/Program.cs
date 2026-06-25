using ClinicServiceDBTools.Extensions;
using ClinicServiceDBTools.ForFun;
using CommandLine;
using DbUp;

AsciiArt.GetArt();

var opSystem = Environment.OSVersion;
var osVersion = opSystem.Version;
LogExt.Headline($"Operation system: {opSystem.VersionString} (v.{osVersion})");

try
{
    var arguments = Parser.Default.ParseArguments<Args>(args)
                       .MapResult((Args opts) => opts, (IEnumerable<Error> errors) =>
                       { throw new Exception(string.Join(",", errors.Select(er => er.Tag.ToString()))); });

    var dbTypes = new DBTypes();

    var dbType = dbTypes.DbTypeParse(arguments.Dscs);

    LogExt.Info($"Connect to DB [{arguments.Dscs.Split(";")[1]}] on server [{arguments.Dscs.Split(";")[0]}]...");

    DbUp.Engine.UpgradeEngine upgrader;

    if (dbType == DBType.MSSQL)
    {
        if (arguments.CreateDB)
        {
            LogExt.Info($"Checking DB availability");

            EnsureDatabase.For.SqlDatabase(arguments.Dscs);
        }
    }
    else if (dbType == DBType.PGSQL)
    {
        if (arguments.CreateDB)
        {
            LogExt.Info($"Checking DB availability");

            EnsureDatabase.For.PostgresqlDatabase(arguments.Dscs);
        }
    }
    else
    {
        LogExt.Error("The type of database used is not specified");
        return -1;
    }

    LogExt.Info($"");

}
catch { }

return 0;