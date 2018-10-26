using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class TrailColorManager : ColorManager
{
    public Color trailColor;

    public override Color ColorForSaberType(Saber.SaberType type)
    {
        return trailColor;
    }
}
