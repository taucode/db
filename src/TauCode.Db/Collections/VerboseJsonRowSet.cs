using Newtonsoft.Json;
using System.Collections;

namespace TauCode.Db.Collections;

public class VerboseJsonRowSet : RowSetBase
{
    #region Nested

    private class Enumerator : IEnumerator<IRow>
    {
        #region Fields

        private readonly IDictionary<string, Type> _propertyTypes;

        private readonly VerboseJsonRowSet _host;
        private FixedSchemaRow? _last;

        #endregion

        #region ctor

        internal Enumerator(VerboseJsonRowSet host)
        {
            _host = host;
            _propertyTypes = new Dictionary<string, Type>();
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _host._jsonReader.SkipComments();

            if (_last == null)
            {
                // at the start. current token must be '['
                var tokenType = _host._jsonReader.TokenType;
                if (tokenType != JsonToken.StartArray)
                {
                    throw new NotImplementedException();
                }

                _host._jsonReader.Read();
                _host._jsonReader.SkipComments();

                tokenType = _host._jsonReader.TokenType;
                // must be ']' or '{'

                switch (tokenType)
                {
                    case JsonToken.EndArray:
                        _host._jsonReader.Read(); // skip ']'
                        return false;

                    case JsonToken.StartObject:
                        _host._jsonReader.Read(); // skip '{'
                        _last = this.ReadFirstRow();
                        _host.FieldNames = _last.Schema.Keys.ToList();
                        return true;

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                var tokenType = _host._jsonReader.TokenType;

                switch (tokenType)
                {
                    case JsonToken.EndArray:
                        _host._jsonReader.Read(); // skip ']'
                        return false;

                    case JsonToken.StartObject:
                        _host._jsonReader.Read(); // skip '{'
                        _last = this.ReadNextRow();
                        return true;


                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private FixedSchemaRow ReadFirstRow()
        {
            var fieldNames = new List<string>();
            var fieldValues = new List<object>();

            while (true)
            {
                _host._jsonReader.SkipComments();

                var tokenType = _host._jsonReader.TokenType;

                if (tokenType == JsonToken.EndObject)
                {
                    _host._jsonReader.Read(); // skip '}'
                    break;
                }
                else if (tokenType == JsonToken.PropertyName)
                {
                    var property = _host._jsonReader.ReadProperty();

                    fieldNames.Add(property.Key);
                    fieldValues.Add(property.Value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            // todo: deal with empty fieldNames

            var dictionary = new Dictionary<string, int>();
            for (var i = 0; i < fieldNames.Count; i++)
            {
                dictionary.Add(fieldNames[i], i);
            }

            var row = new FixedSchemaRow(dictionary);
            for (var i = 0; i < fieldNames.Count; i++)
            {
                row[fieldNames[i]] = fieldValues[i];
            }

            return row;
        }

        private FixedSchemaRow ReadNextRow()
        {
            var fieldNames = new List<string>();
            var fieldValues = new List<object>();

            var index = 0;

            while (true)
            {
                _host._jsonReader.SkipComments();

                var tokenType = _host._jsonReader.TokenType;

                if (tokenType == JsonToken.EndObject)
                {
                    _host._jsonReader.Read(); // skip '}'
                    break;
                }
                else if (tokenType == JsonToken.PropertyName)
                {
                    if (index >= _host.FieldNames.Count)
                    {
                        throw new NotImplementedException();
                    }

                    var property = _host._jsonReader.ReadProperty();

                    fieldNames.Add(property.Key);
                    fieldValues.Add(property.Value);
                }
                else
                {
                    throw new NotImplementedException();
                }

                var currentFieldName = fieldNames[index];
                var expectedFieldName = _host.FieldNames[index];

                if (currentFieldName != expectedFieldName)
                {
                    throw new NotImplementedException();
                }

                index++;
            }

            var row = new FixedSchemaRow(_last.Schema);


            //throw new NotImplementedException();



            //// todo: deal with empty fieldNames

            //var dictionary = new Dictionary<string, int>();
            //for (var i = 0; i < fieldNames.Count; i++)
            //{
            //    dictionary.Add(fieldNames[i], i);
            //}

            //var row = new FixedSchemaRow(dictionary);
            for (var i = 0; i < fieldNames.Count; i++)
            {
                row[fieldNames[i]] = fieldValues[i];
            }

            return row;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerator<IRow> Members

        public IRow Current => _last;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // idle
            //throw new NotImplementedException();
        }

        #endregion
    }

    #endregion

    private readonly JsonReader _jsonReader;
    private readonly Enumerator _enumerator;

    public VerboseJsonRowSet(JsonReader jsonReader)
    {
        _jsonReader = jsonReader;
        _enumerator = new Enumerator(this);
    }

    public override IEnumerator<IRow> GetEnumerator()
    {
        return _enumerator;
    }
}