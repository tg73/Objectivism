using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Objectivism.ObjectClasses;
using Objectivism.Parameters;
using System.Collections.Generic;
using System.Linq;

namespace Objectivism.Components.Utilities
{
    internal static class ComponentExtensions
    {
        public static GH_RuntimeMessageLevel GetAccessChangedMessageLevel( this IGH_Param param )
            => param is IHasAccessChangedMessageLevel hasAccessChangedMessageLevel
                    ? hasAccessChangedMessageLevel.AccessChangedMessageLevel
                    : GH_RuntimeMessageLevel.Warning;

        public static GH_RuntimeMessageLevel GetAccessChangedMessageLevel( this IGH_Component component, int paramIndex ) 
            => component.Params.Input[paramIndex] is IHasAccessChangedMessageLevel hasAccessChangedMessageLevel
                    ? hasAccessChangedMessageLevel.AccessChangedMessageLevel
                    : GH_RuntimeMessageLevel.Warning;

        public static GH_RuntimeMessageLevel? GetUnanimousAccessChangedMessageLevel( this IGH_Component component )
        {
            var inputParams = component.Params.Input;

            if ( inputParams.Count == 0 )
            {
                return GH_RuntimeMessageLevel.Warning;
            }

            var first = inputParams[0].GetAccessChangedMessageLevel();

            if ( inputParams.Count == 1 )
            {
                return first;
            }

            return inputParams.Skip( 1 ).All( p => p.GetAccessChangedMessageLevel() == first )
                ? (GH_RuntimeMessageLevel?) first
                : null;
        }

        public static (string Name, ObjectProperty Property) GetProperty( this IGH_Component component,
            IGH_DataAccess daObject, int paramIndex )
        {
            var previewOn = true;
            var accessChangedMessageLevel = GH_RuntimeMessageLevel.Warning;
            var param = component.Params.Input[paramIndex];

            if ( param is IHasPreviewToggle hasPreviewToggle )
            {
                previewOn = hasPreviewToggle.PreviewOn;
            }

            if ( param is IHasAccessChangedMessageLevel hasAccessChangedMessageLevel )
            {
                accessChangedMessageLevel = hasAccessChangedMessageLevel.AccessChangedMessageLevel;
            }

            ObjectProperty prop;
            var name = param.NickName;
            if ( param.Access == GH_ParamAccess.item )
            {
                IGH_Goo item = null;
                if ( !daObject.GetData( paramIndex, ref item ) )
                {
                    component.AddRuntimeMessage( GH_RuntimeMessageLevel.Remark,
                        $"{name} has no input and has been assigned null data" );
                }

                prop = new ObjectProperty( item );
            }
            else if ( param.Access == GH_ParamAccess.list )
            {
                var items = new List<IGH_Goo>();
                daObject.GetDataList( paramIndex, items );
                prop = new ObjectProperty( items );
            }
            else //tree access
            {
                daObject.GetDataTree( paramIndex, out GH_Structure<IGH_Goo> itemTree );
                prop = new ObjectProperty( itemTree );
            }

            prop.PreviewOn = previewOn;
            prop.AccessChangedMessageLevel = accessChangedMessageLevel;
            return (name, prop);
        }
    }
}