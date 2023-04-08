using Newtonsoft.Json;
using TauCode.Db.Collections;
using TauCode.Db.Instructions.Impl;

namespace TauCode.Db;

public class JsonInstructionReader : InstructionReader
{
    public override IEnumerable<IInstruction> ReadInstructions(TextReader textReader)
    {
        var jsonReader = new JsonTextReader(textReader);

        jsonReader.SkipTokenType(JsonToken.StartArray);

        while (true)
        {
            jsonReader.ExpectTokenTypes(JsonToken.StartObject, JsonToken.EndArray);
            if (jsonReader.TokenType == JsonToken.StartObject)
            {
                // got object start
                var instruction = this.ReadInstruction(jsonReader);
                yield return instruction;


                jsonReader.SkipComments();
                // at this point, we gotta be at "EndObject" token which closes the operation.
                jsonReader.ExpectTokenTypes(JsonToken.EndObject); // todo: single-argument overload
                jsonReader.Read();

                continue;
            }

            // got array end
            yield break;
        }
    }

    protected virtual IInstruction ReadInstruction(JsonTextReader jsonReader)
    {
        jsonReader.SkipTokenType(JsonToken.StartObject);

        var operation = jsonReader.ReadExpectedPropertyValue<string>("operation");

        switch (operation)
        {
            case "Insert":
                return this.ReadInsertInstruction(jsonReader);

            default:
                throw new NotImplementedException();
        }
    }

    private IInstruction ReadInsertInstruction(JsonTextReader jsonReader)
    {
        var fullTableName = jsonReader.ReadExpectedPropertyValue<string>("table");
        jsonReader.SkipExpectedPropertyDeclaration("data");

        var rowSet = new VerboseJsonRowSet(jsonReader);

        // todo.
        var parts = fullTableName!.Split('.');
        var schemaName = parts[0];
        var tableName = parts[1];

        IInstruction instruction = new InsertInstruction(schemaName, tableName, rowSet);

        return instruction;
    }
}