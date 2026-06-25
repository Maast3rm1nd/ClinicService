using CommandLine;

namespace ClinicServiceDBTools.Extensions
{
    public class Args
    {
        private string _dscs;

        [Option("dscs", Required = true, HelpText = "Database server connection string")]
        public string Dscs
        {
            get {
                return _dscs;
            }
            set {
                _dscs = value;
            }
        }

        [Option("createdb", Default = false, HelpText = "Create new database")]
        public bool CreateDB { get; set; }

    }
}
