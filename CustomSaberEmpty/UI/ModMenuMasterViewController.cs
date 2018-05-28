using System;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

namespace CustomSaber
{
    /**
     * ModUI Framework implementation ideas:
     *
     * - Mod references BSModUI.dll
     * - Inherits ModGui baseclass
     * - Calls Setup UI base function with name and ver
     * - Toggle is added by default with override for custom page
     * - 
     */

    class ModSelection
    {
    }

    class CustSaber
    {
       
        public string Name { get; set; }
        public string Path { get; set; }
        // TODO: prop for Image
    }

    class ModMenuMasterViewController
    {


        

        public ModsListViewController ModsListViewController;
        public bool ModDetailsPushed = false;

        private ModMenuMasterViewController _modList;




    }
}
