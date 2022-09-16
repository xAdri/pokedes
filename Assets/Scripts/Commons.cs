using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Commons : MonoBehaviour
{
    public static readonly string basePokeURL = "https://pokeapi.co/api/v2/";
    public static int minIndexPokeNumber = 1;
    public static int maxIndexPokeNumber = 898;
    public static string language = "en";
    public static float cooldown = 0.5f;
    public static float maxStatValue = 300;

    #region pokemon type colors
    public static Color32 rock = new Color32(206, 193, 140, 255);
    public static Color32 steel = new Color32(157, 193, 48, 255);
    public static Color32 ghost = new Color32(105, 112, 197, 255);
    public static Color32 water = new Color32(85, 158, 223, 255);
    public static Color32 grass = new Color32(93, 190, 98, 255);
    public static Color32 psychic = new Color32(248, 124, 122, 255);
    public static Color32 ice = new Color32(126, 212, 201, 255);
    public static Color32 dark = new Color32(95, 96, 109, 255);
    public static Color32 fairy = new Color32(239, 151, 230, 255);
    public static Color32 normal = new Color32(154, 157, 161, 255);
    public static Color32 fighting = new Color32(217, 66, 86, 255);
    public static Color32 flying = new Color32(155, 180, 232, 255);
    public static Color32 poison = new Color32(181, 99, 206, 255);
    public static Color32 ground = new Color32(215, 133, 85, 255);
    public static Color32 bug = new Color32(157, 193, 48, 255);
    public static Color32 fire = new Color32(248, 165, 79, 255);
    public static Color32 electric = new Color32(237, 213, 63, 255);
    public static Color32 dragon = new Color32(7, 115, 199, 255);

    public static Color32 black = new Color32(22, 22, 22, 255);
    public static Color32 green = new Color32(56, 183, 58, 255);
    public static Color32 red = new Color32(183, 56, 69, 255);
    #endregion

    // FUNCTIONS

    public static Color[] PokeTypeWeaknesses()
    {
        Color[] weaknesses = null;

        weaknesses.SetValue(rock, 0);

        return weaknesses;
    }

    public static string CapitalizeFirstLetter(string str)
    {
        // Returns the same string capitalizing the first letter
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    public static float ConvertDecimetersToMeters(float centimeters)
    {
        return centimeters * 0.1f;
    }

    public static float ConvertHectogramsToKilograms(float hectograms)
    {
        return hectograms * 0.1f;
    }

    public static int randomPokeIndex(int minIdNum, int maxIdNum)
    {
        // Returns integer with randon num between min and max int
        return Random.Range(minIdNum, maxIdNum);
    }

    public static void SourceLoader(Text pokeNameText)
    {
        pokeNameText.text = "Loading...";
    }

    public static void SourceLoaderWithError(Sprite pokeball, RawImage pokeImage, RawImage pokeImageShinny, Text pokeNameText, Text pokeDescriptionText, Text pokeHeightText, Text pokeWeightText, Text pokeTypeTextArrayFirst, Text pokeTypeTextArraySecond, Text pokeAbilityNamesText, Text inputFieldText)
    {
        // Load textures and texts
        pokeImage.texture = pokeball.texture;
        pokeImageShinny.texture = pokeball.texture;

        pokeNameText.text = "Pokemon Error #404";
        pokeDescriptionText.text = "Ups, something is wrong here...\nWe couldn't find '" + inputFieldText.text + "' in our pokedex...\nTry again with a valid pokémon name or id!";
        pokeHeightText.text = "0,00 m";
        pokeWeightText.text = "0,00 kg";
        pokeAbilityNamesText.text = "No abilities";

        pokeTypeTextArrayFirst.text = "No types";
        pokeTypeTextArrayFirst.color = black;
        pokeTypeTextArraySecond.text = "No types";
        pokeTypeTextArraySecond.color = black;
    }

    public static void SourceLoaderWithEasterEgg(Sprite adriDraw, Sprite adriPhoto, RawImage pokeImage, RawImage pokeImageShinny, Text pokeNameText, Text pokeDescriptionText, Text pokeHeightText, Text pokeWeightText, Text pokeTypeTextArrayFirst, Text pokeTypeTextArraySecond, Text pokeAbilityNamesText)
    {
        // Load textures and texts
        pokeImage.texture = adriPhoto.texture;
        pokeImage.rectTransform.sizeDelta = new Vector2(480, 480);
        pokeImage.rectTransform.sizeDelta = new Vector2(520, 404);
        pokeImageShinny.texture = adriDraw.texture;

        pokeNameText.text = "Adri Pérez #1996";
        pokeDescriptionText.text = "Congratulations!! You have found this little easter egg, I hope you enjoy the visit to my portfolio" +
            " and feel free to contact me if you want to work or talk something mate! [aperezgfx@gmail.com]";
        pokeHeightText.text = "1,90 m";
        pokeWeightText.text = "95 kg";
        pokeAbilityNamesText.text = "Python, C#,\nUnity Egine,\nIllustrator,\nPhotoshop";

        pokeTypeTextArrayFirst.text = "English";
        pokeTypeTextArrayFirst.color = black;
        pokeTypeTextArraySecond.text = "Spanish";
        pokeTypeTextArraySecond.color = black;
    }

    public static string[] LoadPokeTypeNames(JSONNode pokeInfo)
    {
        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeNames = new string[pokeTypes.Count];

        for (int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }

        return pokeTypeNames;
    }

    public static string LoadPokeAbilityNames(JSONNode pokeInfo)
    {
        JSONNode pokeAbilities = pokeInfo["abilities"];
        List<string> LoadPokeAbilityNames = new List<string>();
        string Abilities = "";

        for (int i = 0; i < pokeAbilities.Count; i++)
        {
            LoadPokeAbilityNames.Add(pokeAbilities[i]["ability"]["name"]);
        }

        for (int i = 0; i < LoadPokeAbilityNames.Count; i++)
        {
            if (i < LoadPokeAbilityNames.Count - 1)
            {
                Abilities += CapitalizeFirstLetter(LoadPokeAbilityNames[i]) + "\n";
            }
            else
            {
                Abilities +=  CapitalizeFirstLetter(LoadPokeAbilityNames[i]);
            }
        }

        return Abilities;
    }

    public static string GetEnPokeDescription(JSONNode pokeDescList)
    {
        string description = "";

        for (int i = 0; i < pokeDescList.Count; i++)
        {
            if (pokeDescList[i]["language"]["name"] == language)
            {
                description = pokeDescList[i]["flavor_text"];
            }
        }

        return description;
    }

    public static Color32 FindPokeTypeColor(string pokeTypeName)
    {
        Color32 typeColor;

        switch (pokeTypeName)
        {
            case "rock":
                typeColor = rock;
                break;
            case "ghost":
                typeColor = ghost;
                break;
            case "steel":
                typeColor = steel;
                break;
            case "water":
                typeColor = water;
                break;
            case "grass":
                typeColor = grass;
                break;
            case "psychic":
                typeColor = psychic;
                break;
            case "ice":
                typeColor = ice;
                break;
            case "dark":
                typeColor = dark;
                break;
            case "fairy":
                typeColor = fairy;
                break;
            case "normal":
                typeColor = normal;
                break;
            case "fighting":
                typeColor = fighting;
                break;
            case "flying":
                typeColor = flying;
                break;
            case "poison":
                typeColor = poison;
                break;
            case "ground":
                typeColor = ground;
                break;
            case "bug":
                typeColor = bug;
                break;
            case "fire":
                typeColor = fire;
                break;
            case "electric":
                typeColor = electric;
                break;
            case "dragon":
                typeColor = dragon;
                break;
            default:
                typeColor = new Color32(0,0,0,0);
                break;
        }

        return typeColor;
    }
}
