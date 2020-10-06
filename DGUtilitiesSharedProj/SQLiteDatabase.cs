using System;
using System.Linq;
using System.Data.Linq;
using System.Data.SQLite;

namespace DiegoG.Utilities
{
    public class SQLiteDatabase
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public SQLiteConnection Connection { get; private set; }
        public DataContext Context { get; private set; }

        public const string FileExtension = ".db";

        private void chkpswrd() { if (HasPassword) return; throw new InvalidOperationException("This Database does not use a Password"); }
        public bool HasPassword { get; private set; }
        private string _pswrd;
        public string Password
        {
            get
            {
                chkpswrd();
                return _pswrd;
            }
            set
            {
                chkpswrd();
                _pswrd = value;
            }
        }

        public SQLiteDatabase(string name, string adrs) : this(name, adrs, "")
        {
            HasPassword = false;
        }
        public SQLiteDatabase(string name, string adrs, string pass)
        {
            Name = name;
            Address = adrs;
            HasPassword = true;
            Password = pass;
            Connection = new SQLiteConnection($@"Data Source={System.IO.Path.Combine(adrs, name + FileExtension)}");
            Context = new DataContext(Connection);
        }

    }
}
