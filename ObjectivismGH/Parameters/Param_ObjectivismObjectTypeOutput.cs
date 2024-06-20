using System;

namespace Objectivism.Parameters
{
    public class Param_ObjectivismObjectTypeOutput : Param_ObjectivismOutput
    {
        protected override string OutputType => "Type";
        protected override string OutputTypePlural => "Types";
        public override Guid ComponentGuid => new Guid( "55bf273b-08ac-4cd7-a3a2-27bb1228a58c" );
    }
}