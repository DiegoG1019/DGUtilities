﻿using System;
using System.Linq;
using System.IO;
using System.Data.Linq;
using System.Data.SQLite;

namespace DiegoG.Utilities
{
    public class SQLiteDatabase
    {
        public string Name { get; private set; }
        public string Directory { get; private set; }
        public string Address => Path.Combine(Directory, Name + FileExtension);
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
            Directory = adrs;
            HasPassword = true;
            Password = pass;
            Connection = new SQLiteConnection($@"Data Source={Address}");
            Context = new DataContext(Connection);
        }
        /// <summary>
        /// returns true if database was created
        /// </summary>
        /// <returns></returns>
        public bool CheckAndCreateDatabase()
        {
            if (DatabaseExists)
                return false;
            CreateDatabase();
            return true;
        }
        public void CreateDatabase()
        {
            SQLiteConnection.CreateFile(Address);
        }
        public bool DatabaseExists => File.Exists(Address);

    }
}
