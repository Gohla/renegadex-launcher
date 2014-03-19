using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ReactiveIRC.Interface
{
    public class IdentityMask : IEquatable<IdentityMask>, IComparable<IdentityMask>
    {
        private static readonly Regex PrefixRegex = new Regex("(?:([^!@]*)!)?(?:([^!@]*)@)?([^!@]*)", 
            RegexOptions.Compiled);

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if(value != null)
                    _nameRegex = new Regex(Regex.Escape(value).Replace(@"\*", @"[^!@]*"), RegexOptions.Compiled);
            }
        }
        public String Ident
        {
            get
            {
                return _ident;
            }
            set
            {
                _ident = value;
                if(value != null)
                    _identRegex = new Regex(Regex.Escape(value).Replace(@"\*", @"[^!@]*"), RegexOptions.Compiled);
            }
        }
        public String Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
                if(value != null)
                    _hostRegex = new Regex(Regex.Escape(value).Replace(@"\*", @"[^!@]*"), RegexOptions.Compiled);
            }
        }

        private String _name;
        private Regex _nameRegex;
        private String _ident;
        private Regex _identRegex;
        private String _host;
        private Regex _hostRegex;

        public IdentityMask()
        {

        }

        public IdentityMask(IIdentity identity)
        {
            Name = identity.Name;
            Ident = identity.Ident;
            Host = identity.Host;
        }

        public static IdentityMask Parse(String str)
        {
            Match results = PrefixRegex.Match(str);

            if(!results.Success)
                return null;

            String name = null;
            String ident = null;
            String host = null;

            if(results.Groups[1].Success)
                name = results.Groups[1].Value;
            if(results.Groups[2].Success)
                ident = results.Groups[2].Value;
            if(results.Groups[3].Success)
                host = results.Groups[3].Value;

            return new IdentityMask { Name = name, Ident = ident, Host = host };
        }

        public static bool TryParse(String str, out IdentityMask mask)
        {
            mask = Parse(str);
            if(mask == null)
                return false;
            else
                return true;
        }

        public bool Match(IIdentity identity)
        {
            bool match = true;

            if(_nameRegex != null)
                if(identity.Name.Value != null)
                    match &= _nameRegex.IsMatch(identity.Name.Value);
                else
                    return false;

            if(_identRegex != null)
                if(identity.Ident.Value != null)
                    match &= _identRegex.IsMatch(identity.Ident.Value);
                else
                    return false;

            if(_hostRegex != null)
                if(identity.Host.Value != null)
                    match &= _hostRegex.IsMatch(identity.Host.Value);
                else
                    return false;

            return match;
        }

        public int CompareTo(IdentityMask other)
        {
            if(ReferenceEquals(other, null))
                return 1;

            int result = 0;
            result = this.Name.CompareTo(other.Name);
            if(result == 0)
                result = this.Ident.CompareTo(other.Ident);
            if(result == 0)
                result = this.Host.CompareTo(other.Host);
            return result;
        }

        public override bool Equals(object other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return Equals(other as IdentityMask);
        }

        public bool Equals(IdentityMask other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return
                EqualityComparer<String>.Default.Equals(this.Name, other.Name)
             && EqualityComparer<String>.Default.Equals(this.Ident, other.Ident)
             && EqualityComparer<String>.Default.Equals(this.Host, other.Host)
             ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + EqualityComparer<String>.Default.GetHashCode(this.Name  ?? String.Empty);
                hash = hash * 23 + EqualityComparer<String>.Default.GetHashCode(this.Ident ?? String.Empty);
                hash = hash * 23 + EqualityComparer<String>.Default.GetHashCode(this.Host  ?? String.Empty);
                return hash;
            }
        }

        public override String ToString()
        {
            return String.Concat
            (
                ((Name) ?? String.Empty)
              , (Name != null && (Ident != null || Host != null) ? "!" : String.Empty)
              , ((Ident) ?? String.Empty)
              , (((Name != null || Ident != null) && (Host != null)) ? "@" : String.Empty)
              , ((Host) ?? String.Empty)
            );
        }
    }
}
