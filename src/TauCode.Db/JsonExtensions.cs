using Newtonsoft.Json;
using TauCode.Extensions;

namespace TauCode.Db;

internal static class JsonExtensions
{
    internal static T? ReadExpectedPropertyValue<T>(this JsonReader jsonReader, string propertyName)
    {
        jsonReader.SkipComments();

        var tokenType = jsonReader.TokenType;
        if (tokenType != JsonToken.PropertyName)
        {
            throw new NotImplementedException();
        }

        if (!Equals(propertyName, jsonReader.Value))
        {
            throw new NotImplementedException();
        }

        jsonReader.Read();
        jsonReader.SkipComments();

        var valueTokenType = jsonReader.TokenType;
        JsonToken expectedValueTokenType;

        if (typeof(T) == typeof(string))
        {
            expectedValueTokenType = JsonToken.String;
        }
        else
        {
            throw new NotImplementedException();
        }

        if (valueTokenType != expectedValueTokenType)
        {
            throw new NotImplementedException();
        }

        var value = jsonReader.Value;

        jsonReader.Read(); // move to next token
        return (T?)value;
    }

    internal static KeyValuePair<string, object?> ReadProperty(this JsonReader jsonReader)
    {
        jsonReader.SkipComments();

        var tokenType = jsonReader.TokenType;
        if (tokenType != JsonToken.PropertyName)
        {
            throw new NotImplementedException();
        }

        if (jsonReader.Value is not string propertyName)
        {
            throw new NotImplementedException();
        }

        var wasRead = jsonReader.Read();
        if (!wasRead)
        {
            throw new NotImplementedException();
        }

        tokenType = jsonReader.TokenType;
        if (!IsPropertyValueTokenType(tokenType))
        {
            throw new NotImplementedException();
        }

        var propertyValue = jsonReader.Value;

        var result = new KeyValuePair<string, object?>(propertyName, propertyValue);

        jsonReader.Read(); // move to next token

        return result;
    }

    private static bool IsPropertyValueTokenType(JsonToken tokenType)
    {
        // todo: optimize?

        return tokenType.IsIn(
            JsonToken.Integer,
            JsonToken.String,
            JsonToken.Boolean,
            JsonToken.Float,
            JsonToken.Null
        );
    }

    internal static void SkipExpectedPropertyDeclaration(this JsonReader jsonReader, string propertyName)
    {
        jsonReader.SkipComments();

        var tokenType = jsonReader.TokenType;
        var value = jsonReader.Value;

        var ok = tokenType == JsonToken.PropertyName && Equals(value, propertyName);
        if (!ok)
        {
            throw new NotImplementedException();
        }

        jsonReader.Read();
    }

    internal static void ReadToken(this JsonReader jsonReader)
    {
        throw new NotImplementedException();
    }

    internal static void SkipTokenType(this JsonReader jsonReader, JsonToken expectedTokenType)
    {
        jsonReader.SkipComments();

        if (jsonReader.TokenType != expectedTokenType)
        {
            throw new NotImplementedException();
        }

        jsonReader.Read();
    }

    internal static void ExpectTokenTypes(this JsonReader jsonReader, params JsonToken[] expectedTokenTypes)
    {
        jsonReader.SkipComments();

        if (!jsonReader.TokenType.IsIn(expectedTokenTypes))
        {
            throw new NotImplementedException();
        }
    }

    internal static void SkipComments(this JsonReader jsonReader)
    {
        if (jsonReader.TokenType == JsonToken.None)
        {
            // read method was not called
            jsonReader.Read();
        }

        while (true)
        {
            var tokenType = jsonReader.TokenType;

            if (tokenType == JsonToken.Comment)
            {
                // current token is comment, let's go on if we got space
                var wasRead = jsonReader.Read();
                if (!wasRead)
                {
                    return;
                }

                continue;
            }

            // token type is not comment, let's break
            return;
        }
    }
}
