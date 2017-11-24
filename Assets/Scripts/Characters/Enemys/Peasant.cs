using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peasant : Characters
{
    public Peasant()
    {
        force_x = 250;
        force_y = 25;
        mass = 60;

        max_life = 1;
        max_health_point = 2;
        max_ammo = 0;

        life = max_life;
        health_point = max_health_point;
        ammo = max_ammo;
    }
}
