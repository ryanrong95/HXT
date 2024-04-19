using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Needs.Underly;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using MvcApp.Buyer.Services.Models;

namespace MvcApp.Buyer.Services.Rates
{
    public class FixedRates : IEnumerable<ExchangeRate>, IXmlSerializable
    {
        IEnumerable<ExchangeRate> source;

        public int Count
        {
            get { return this.source.Count(); }
        }

        protected FixedRates() { }

        public FixedRates(District district, params Currency[] arry)
        {
            if (arry.Length == 0)
            {
                this.source = UnifyRates.Current.FloatRates[district];
            }

            this.source = UnifyRates.Current.FloatRates[district].Where(item => arry.Contains(item.To) || arry.Contains(item.From));
        }

        public FixedRates(IEnumerable<ExchangeRate> source)
        {
            this.source = source;
        }

        public decimal Where(Currency from, Currency to)
        {
            if (from == to)
            {
                return 1m;
            }

            var singler = this.SingleOrDefault(item => item.From == from && item.To == to);

            if (singler == null)
            {
                throw new NotSupportedException($"Do not support the specified currency from {from} to {to}!");
            }

            return singler.Value;
        }

        virtual public IEnumerator<ExchangeRate> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region IXmlSerializable

        virtual public XmlSchema GetSchema()
        {
            return new XmlSchema();
        }

        virtual public void ReadXml(XmlReader reader)
        {
            string xml = reader.ReadOuterXml();
            XElement xele = XElement.Load(new StringReader(xml));
            List<ExchangeRate> source = new List<ExchangeRate>();
            ExchangeRate exampl;
            foreach (var item in xele.Elements(nameof(ExchangeRate)))
            {
                source.Add(new ExchangeRate
                {
                    Value = decimal.Parse(item.Element(nameof(exampl.Value)).Value),
                    District = (District)Enum.Parse(typeof(District), item.Element(nameof(exampl.District)).Value, true),
                    From = (Currency)Enum.Parse(typeof(Currency), item.Element(nameof(exampl.From)).Value, true),
                    To = (Currency)Enum.Parse(typeof(Currency), item.Element(nameof(exampl.To)).Value, true),
                });
            }
            this.source = source;
        }

        virtual public void WriteXml(XmlWriter writer)
        {
            foreach (var item in this)
            {
                writer.WriteStartElement(nameof(ExchangeRate));

                writer.WriteStartElement(nameof(item.District));
                writer.WriteValue(Enum.GetName(item.District.GetType(), item.District));
                writer.WriteEndElement();

                writer.WriteStartElement(nameof(item.From));
                writer.WriteValue(Enum.GetName(item.From.GetType(), item.From));
                writer.WriteEndElement();

                writer.WriteStartElement(nameof(item.To));
                writer.WriteValue(Enum.GetName(item.To.GetType(), item.To));
                writer.WriteEndElement();

                writer.WriteStartElement(nameof(item.Value));
                writer.WriteValue(item.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        void WriteValue(XmlWriter writer, object value)
        {
            if (value is string)
            {
                this.WriteValue(writer, value as string);
            }
            else
            {
                Type type = value.GetType();
                if (type.IsEnum)
                {
                    writer.WriteValue(Enum.GetName(type, value));
                }
                else if (type.IsValueType && value is IConvertible)
                {
                    writer.WriteValue(value);
                }
                else
                {
                    this.WriteValue(writer, value.ToString());
                }
            }
        }

        void WriteValue(XmlWriter writer, string value)
        {
            if (value.Contains('<') || value.Contains('>'))
            {
                writer.WriteCData(value);
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        #endregion
    }
}
