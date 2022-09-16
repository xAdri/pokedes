using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

public class PokeAPIController : MonoBehaviour
{
    public RawImage pokeImage, pokeImageShinny;
    public Sprite adriPhoto, adriDraw;

    public Sprite pokeballPlaceholder;
    public Text pokeNameText, pokeHeightText, pokeWeightText, pokeAbilityNamesText, pokeDescriptionText, inputFieldText;
    public Text[] pokeTypeTextArray;

    public AudioClip buttonSoundEffect;

    private float cooldownTimer;
    private int pokemonId;
    private float pokeStatPs;
    private float pokeStatAtk;
    private float pokeStatDef;
    private float pokeStatSpA;
    private float pokeStatSpD;
    private float pokeStatSpe;

    public Image pokeStatPsGraphic;
    public Image pokeStatAtkGraphic;
    public Image pokeStatDefGraphic;
    public Image pokeStatSpAGraphic;
    public Image pokeStatSpDGraphic;
    public Image pokeStatSpeGraphic;
    public Image findButton;

    # region getters and setters
    public float CooldownTimer { get => cooldownTimer; set => cooldownTimer = value; }
    public int PokemonId { get => pokemonId; set => pokemonId = value; }
    public float PokeStatPs{ get => pokeStatPs; set => pokeStatPs = value; }
    public float PokeStatAtk { get => pokeStatAtk; set => pokeStatAtk = value; }
    public float PokeStatDef { get => pokeStatDef; set => pokeStatDef = value; }
    public float PokeStatSpA { get => pokeStatSpA; set => pokeStatSpA = value; }
    public float PokeStatSpD { get => pokeStatSpD; set => pokeStatSpD = value; }
    public float PokeStatSpe { get => pokeStatSpe; set => pokeStatSpe = value; }
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        //RandomPokemon();
        FindPokemonWithName("1");
    }

    // Update is called once per frame
    void Update()
    {
        if (CooldownTimer <= Commons.cooldown)
        {
            CooldownTimer += Time.deltaTime;
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.RightArrow) && PokemonId < Commons.maxIndexPokeNumber)
            {
                NextPokemon();
                SoundController.PlaySound(buttonSoundEffect);
                RestartCooldownTimer();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) && PokemonId > Commons.minIndexPokeNumber)
            {
                LastPokemon();
                SoundController.PlaySound(buttonSoundEffect);
                RestartCooldownTimer();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && PokemonId < (Commons.maxIndexPokeNumber - 9))
            {
                PokemonId += 9;
                NextPokemon();
                SoundController.PlaySound(buttonSoundEffect);
                RestartCooldownTimer();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && PokemonId > (Commons.minIndexPokeNumber + 9))
            {
                PokemonId -= 9;
                LastPokemon();
                SoundController.PlaySound(buttonSoundEffect);
                RestartCooldownTimer();
            }
        }        
    }

    void RestartCooldownTimer()
    {
        CooldownTimer = 0f;
    }

    void LastPokemon()
    {
        PokemonId--;
        Commons.SourceLoader(pokeNameText);
        StartCoroutine(GetPokemonAtIndex(PokemonId));
    }

    void NextPokemon()
    {
        PokemonId++;
        Commons.SourceLoader(pokeNameText);
        StartCoroutine(GetPokemonAtIndex(PokemonId));
    }

    void RandomPokemon()
    {
        PokemonId = Commons.randomPokeIndex(Commons.minIndexPokeNumber, Commons.maxIndexPokeNumber);
        StartCoroutine(GetPokemonAtIndex(PokemonId));
    }

    public void FindPokemonWithName(string pokemonName)
    {
        //SoundController.PlaySound(buttonSoundEffect);
        pokemonName = pokemonName.ToLower();

        if (pokemonName != "")
        {
            if (pokemonName == "random")
            {
                RandomPokemon();
            }
            else if (pokemonName.Contains("adri") || pokemonName.Contains("perez"))
            {
                Commons.SourceLoaderWithEasterEgg(adriDraw, adriPhoto, pokeImage, pokeImageShinny, pokeNameText, pokeDescriptionText,
                pokeHeightText, pokeWeightText, pokeTypeTextArray[0], pokeTypeTextArray[1], pokeAbilityNamesText);
                FillPokeStats(280f, 220f, 240f, 200f, 250f, 250f);
            }
            else
            {
                PokemonId = 0;
                Commons.SourceLoader(pokeNameText);
                StartCoroutine(GetPokemonAtIndex(PokemonId, pokemonName));
            }
        }
    }

    IEnumerator GetPokemonAtIndex(int pokemonIndex = default, string pokemonName = default)
    {
        string pokemonURL = "";
        
        if (pokemonName != default )
        {
            pokemonURL = Commons.basePokeURL + "pokemon/" + pokemonName.ToString();
        }

        if (pokemonIndex != default)
        {
            pokemonURL = Commons.basePokeURL + "pokemon/" + pokemonIndex.ToString();
        }

        // Button color
        findButton.color = Commons.green;

        // Get pokeinfo
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);
        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Commons.SourceLoaderWithError(pokeballPlaceholder, pokeImage, pokeImageShinny, pokeNameText, pokeDescriptionText,
                pokeHeightText, pokeWeightText, pokeTypeTextArray[0], pokeTypeTextArray[1], pokeAbilityNamesText, inputFieldText);
            findButton.color = Commons.red;
            FillPokeStats(150f, 150f, 150f, 150f, 150f, 150f);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        UnityWebRequest pokeDescriptionRequest = UnityWebRequest.Get(pokeInfo["species"]["url"]);
        yield return pokeDescriptionRequest.SendWebRequest();

        if (pokeDescriptionRequest.isNetworkError || pokeDescriptionRequest.isHttpError)
        {
            pokeDescriptionText.text = "We are currently unable to load the description of this pokémon, the API is down...";
            findButton.color = Commons.red;
            //Debug.LogError("ERROR:" + pokeDescriptionRequest.error);
        }

        JSONNode pokeDesc = JSON.Parse(pokeDescriptionRequest.downloadHandler.text);

        PokemonId = pokeInfo["id"];
        string pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        string pokeSpriteShinnyURL = pokeInfo["sprites"]["back_default"];
        string pokeHeight = pokeInfo["height"];
        string pokeWeight = pokeInfo["weight"];

        float pokePs = pokeInfo["stats"][0]["base_stat"];
        float pokeAtk = pokeInfo["stats"][1]["base_stat"];
        float pokeDef = pokeInfo["stats"][2]["base_stat"];
        float pokeSpA = pokeInfo["stats"][3]["base_stat"];
        float pokeSpD = pokeInfo["stats"][4]["base_stat"];
        float pokeSpe = pokeInfo["stats"][5]["base_stat"];

        string pokeAbilityNames = Commons.LoadPokeAbilityNames(pokeInfo);
        string[] pokeTypeNames = Commons.LoadPokeTypeNames(pokeInfo);
        JSONNode pokeDescription = pokeDesc["flavor_text_entries"];


        // Get pokesprite
        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);
        UnityWebRequest pokeSpriteShinnyRequest = UnityWebRequestTexture.GetTexture(pokeSpriteShinnyURL);

        yield return pokeSpriteRequest.SendWebRequest();
        yield return pokeSpriteShinnyRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            pokeImageShinny.texture = pokeballPlaceholder.texture;
            findButton.color = Commons.red;
            //Debug.LogError("ERROR:" + pokeSpriteRequest.error);
        }

        if (pokeSpriteShinnyRequest.isNetworkError || pokeSpriteShinnyRequest.isHttpError)
        {
            pokeSpriteShinnyURL = pokeInfo["sprites"]["other"]["official-artwork"]["front_default"];
            pokeSpriteShinnyRequest = UnityWebRequestTexture.GetTexture(pokeSpriteShinnyURL);
            yield return pokeSpriteShinnyRequest.SendWebRequest();
            findButton.color = Commons.red;
            //Debug.LogError("ERROR:" + pokeSpriteShinnyRequest.error);
        }

        // Set UI objects
        pokeImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokeImage.rectTransform.sizeDelta = new Vector2(480, 480);
        pokeImageShinny.texture = DownloadHandlerTexture.GetContent(pokeSpriteShinnyRequest);
        pokeImage.texture.filterMode = FilterMode.Point;
        pokeImageShinny.texture.filterMode = FilterMode.Point;
        pokeDescriptionText.text = Commons.GetEnPokeDescription(pokeDescription);

        pokeNameText.text = Commons.CapitalizeFirstLetter(pokeName) + " #" + PokemonId;
        pokeAbilityNamesText.text = pokeAbilityNames;
        inputFieldText.text = "";

        if (Commons.ConvertDecimetersToMeters(float.Parse(pokeHeight)).ToString().Contains(","))
        {
            pokeHeightText.text = Commons.ConvertDecimetersToMeters(float.Parse(pokeHeight)).ToString() + "0 m";
        }
        else
        {
            pokeHeightText.text = Commons.ConvertDecimetersToMeters(float.Parse(pokeHeight)).ToString() + " m";
        }

        if (Commons.ConvertHectogramsToKilograms(float.Parse(pokeWeight)).ToString().Contains(","))
        {
            pokeWeightText.text = Commons.ConvertHectogramsToKilograms(float.Parse(pokeWeight)).ToString() + "0 kg";
        }
        else
        {
            pokeWeightText.text = Commons.ConvertHectogramsToKilograms(float.Parse(pokeWeight)).ToString() + " kg";
        }

        // Stats
        PokeStatPs = pokePs;
        PokeStatAtk = pokeAtk;
        PokeStatDef = pokeDef;
        PokeStatSpA = pokeSpA;
        PokeStatSpD = pokeSpD;
        PokeStatSpe = pokeSpe;

        FillPokeStats(PokeStatPs, PokeStatAtk, PokeStatDef, PokeStatSpA, PokeStatSpD, PokeStatSpe);

        for (int i = 0; i < pokeTypeNames.Length; i++)
        {
            string pokeTypeName = pokeTypeNames[i];
            Color32 pokeTypeColor = Commons.FindPokeTypeColor(pokeTypeName);
            pokeTypeTextArray[i].gameObject.SetActive(true);

            if (pokeTypeNames.Length == 1)
            {
                pokeTypeTextArray[1].gameObject.SetActive(false);
                pokeTypeTextArray[1].text = "";
                pokeTypeTextArray[1].color = Commons.black;
            }

            pokeTypeTextArray[i].text = Commons.CapitalizeFirstLetter(pokeTypeName);
            pokeTypeTextArray[i].color = pokeTypeColor;
        }

        findButton.color = Commons.red;
    }

    private void FillPokeStats(float PokeStatPs, float PokeStatAtk, float PokeStatDef, float PokeStatSpA, float PokeStatSpD, float PokeStatSpe)
    {
        pokeStatPsGraphic.GetComponent<Image>().fillAmount = PokeStatPs / Commons.maxStatValue;
        pokeStatAtkGraphic.GetComponent<Image>().fillAmount = PokeStatAtk / Commons.maxStatValue;
        pokeStatDefGraphic.GetComponent<Image>().fillAmount = PokeStatDef / Commons.maxStatValue;
        pokeStatSpAGraphic.GetComponent<Image>().fillAmount = PokeStatSpA / Commons.maxStatValue;
        pokeStatSpDGraphic.GetComponent<Image>().fillAmount = PokeStatSpD / Commons.maxStatValue;
        pokeStatSpeGraphic.GetComponent<Image>().fillAmount = PokeStatSpe / Commons.maxStatValue;
    }
}
