using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NB.Charts.Utils
{

    public static class Colors
    {
        /// <summary>
        /// Generates a random but nice-looking color
        /// </summary>
        public static Color RandNice()
        {
            return Random.ColorHSV(0, 1, 0.26f, 0.65f, 0.45f, 0.75f);
        }

        static readonly Color32[] C_Pastelles =
        {
            new Color32(253, 127, 111, 255),
            new Color32(178, 224, 97, 255),
            new Color32(126, 176, 213, 255),
            new Color32(189, 126, 190, 255),
            new Color32(255, 181, 90, 255),
            new Color32(255, 238, 101, 255),
            new Color32(190, 185, 219, 255),
            new Color32(253, 204, 229, 255),
            new Color32(139, 211, 199, 255)
        };

        static readonly Color32[] C_Tableau10 =
        {
            new Color32(78, 121, 167, 255),
            new Color32(242, 142, 44, 255),
            new Color32(225, 87, 89, 255),
            new Color32(118, 183, 178, 255),
            new Color32(89, 161, 79, 255),
            new Color32(237, 201, 73, 255),
            new Color32(175, 122, 161, 255),
            new Color32(255, 157, 167, 255),
            new Color32(156, 117, 95, 255),
            new Color32(186, 176, 171, 255)
        };

        // slightly modified solarized palette
        static readonly Color32[] C_Solarized =
        {
            new Color32(220, 50, 47, 255),
            new Color32(33, 153, 0, 255),
            new Color32(38, 139, 210, 255),
            new Color32(253, 246, 227, 255),
            new Color32(42, 161, 152, 255),
            new Color32(108, 113, 196, 255),
            new Color32(203, 75, 22, 255),
            new Color32(211, 54, 130, 255),
            new Color32(181, 137, 0, 255),
        };

        static readonly Color32[] S_Blue2Yellow =
        {
            new Color32(17, 95, 154, 255),
            new Color32(25, 132, 197, 255),
            new Color32(34, 167, 240, 255),
            new Color32(72, 181, 196, 255),
            new Color32(118, 198, 143, 255),
            new Color32(166, 215, 91, 255),
            new Color32(201, 229, 47, 255),
            new Color32(208, 238, 17, 255),
            new Color32(208, 244, 0, 255)
        };

        static readonly Color32[] S_Blue2Red =
        {
            new Color32(215, 225, 238, 255),
            new Color32(203, 214, 228, 255),
            new Color32(191, 203, 219, 255),
            new Color32(179, 191, 209, 255),
            new Color32(164, 162, 168, 255),
            new Color32(223, 136, 121, 255),
            new Color32(200, 101, 88, 255),
            new Color32(176, 66, 56, 255),
            new Color32(153, 31, 23, 255)
        };

        static readonly Color32[] D_Teal2Pink =
{
            new Color32(84, 190, 190, 255),
            new Color32(118, 200, 200, 255),
            new Color32(152, 209, 209, 255),
            new Color32(186, 219, 219, 255),
            new Color32(222, 218, 210, 255),
            new Color32(228, 188, 173, 255),
            new Color32(223, 151, 158, 255),
            new Color32(215, 101, 139, 255),
            new Color32(200, 0, 100, 255)
        };

        /// <summary>
        /// Positive modulo
        /// </summary>
        static int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static Color FromPalette(Palettes palette, int idx)
        {
            switch (palette)
            {
                case Palettes.C_Pastels:
                    return C_Pastelles[mod(idx, C_Pastelles.Length)];
                case Palettes.C_Solarized:
                    return C_Solarized[mod(idx, C_Solarized.Length)];
                case Palettes.C_Tableau10:
                    return C_Tableau10[mod(idx, C_Tableau10.Length)];
                case Palettes.S_Blue2Yellow:
                    return S_Blue2Yellow[mod(idx, S_Blue2Yellow.Length)];
                case Palettes.D_Blue2Red:
                    return S_Blue2Red[mod(idx, S_Blue2Red.Length)];
                case Palettes.D_Teal2Pink:
                    return D_Teal2Pink[mod(idx, D_Teal2Pink.Length)];
                case Palettes.Random:
                    return RandNice();
                default:
                    throw new System.ArgumentException("Unknown color palette!");
            }
        }
    }
}
