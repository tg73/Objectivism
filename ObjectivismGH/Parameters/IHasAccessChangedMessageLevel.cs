using Grasshopper.Kernel;

namespace Objectivism.Parameters
{
    internal interface IHasAccessChangedMessageLevel
    {
        GH_RuntimeMessageLevel AccessChangedMessageLevel { get; set; }
    }
}