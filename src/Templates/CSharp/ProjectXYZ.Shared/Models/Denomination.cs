using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectXYZ.Shared.Models
{
    public class Denomination : Dictionary<string, string>
    {
        public Denomination()
        {

        }

        public Denomination(string nameForAll)
            : this(nameForAll, nameForAll, nameForAll, nameForAll)
        {

        }

        public Denomination(string nl, string fr, string de, string en)
        {
            SetValue(Languages.NL, nl);
            SetValue(Languages.FR, fr);
            SetValue(Languages.DE, de);
            SetValue(Languages.EN, en);
        }
        public string? Nl
        {
            get { return GetValue(Languages.NL); }
            set { SetValue(Languages.NL, value); }
        }
        public string? Fr
        {
            get { return GetValue(Languages.FR); }
            set { SetValue(Languages.FR, value); }
        }
        public string? De
        {
            get { return GetValue(Languages.DE); }
            set { SetValue(Languages.DE, value); }
        }
        public string? En
        {
            get { return GetValue(Languages.EN); }
            set { SetValue(Languages.EN, value); }
        }

        private string GetValue(string language)
        {
            return this.TryGetValue(language, out string outValue) ? outValue : null;
        }

        private void SetValue(string language, string? value)
        {
            if (value is null)
            {
                if (this.ContainsKey(language))
                    this.Remove(language);
            }
            else
            {
                if (this.ContainsKey(language))
                    this[language] = value;
                else
                    this.Add(language, value);
            }
        }

        public bool IsEmpty()
        {
            return this.Count == 0 || this.All(ln => string.IsNullOrWhiteSpace(ln.Value));
        }
    }
}
