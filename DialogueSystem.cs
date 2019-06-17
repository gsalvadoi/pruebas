using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour {

	public static DialogueSystem Instance { get; set; } 

    //Partes de la UI:
	GameObject dialoguePanel;
	GameObject dialoguePico;
	GameObject dialogueName;
    GameObject dialogueCharBox;
	RectTransform panelrect;
	RectTransform picorect;
	RectTransform namerect;
	Image panelimg;
	Image picoimg;
	Image nameimg;
    TextMeshProUGUI dialogueText;
    TextMeshProUGUI dialogueNameText;

    //Referencias:
    MotherBrain motherbrain;
    Camera camera;
    

    //Contenido:
	string dialogueLine;
	string dialogueLineName;

    GameObject[] listaCaracteres;
    int indiceListaCaracteres;

    //Animacion:
    public int tamany;
    public int posicion;
    float subposicion;

    int acelerar = 1;
    float desplegado = 0;
    public int desplegar = 0;
    Vector3 v3pos_def;
    float current_width = 0;
    float current_height = 0;
    float deca = 1;
    float constant_height = 0;
    int nlinias = 0;

    //Personalizacion:
    int textSize = 1; //0 -> 16; 1 -> 24; 2 -> 36;
    Color textColor;
    int tipo_dialogo = 1; //0 = sin pico; 1 = normal; 2 = leyendo; 3 = mensaje del sistema
    int textShake = 0; //0 = Sin movimiento; 1 = Temblando random;
    int textDance = 0; //0 = Sin movimiento; 1 = Ondulado cantando;
    int textImpact = 0; //0 = Sin impacto; 1 = Impacto al aparecer;

    int[] arrayFontSizes = new int[] {16, 24, 36};


	// Use this for initialization
	void Awake () {



		//Define los componentes de la caja de dialogo:
		dialoguePanel = transform.Find("Button").gameObject;
		dialogueText = dialoguePanel.transform.Find ("Text").GetComponent<TextMeshProUGUI> ();
		dialoguePico = transform.Find("Pico").gameObject;
		dialogueName = transform.Find("CharacterName").gameObject;
		dialogueNameText = dialogueName.transform.Find ("Text").GetComponent<TextMeshProUGUI> ();
        dialogueCharBox = dialogueText.gameObject.transform.Find("ContenedorCaracteres").gameObject;

		panelimg = dialoguePanel.GetComponent<Image>();
		picoimg = dialoguePico.GetComponent<Image>();
		nameimg = dialogueName.GetComponent<Image>();

		panelrect = dialoguePanel.GetComponent<RectTransform>();
		picorect = dialoguePico.GetComponent<RectTransform>();
		namerect = dialogueName.GetComponent<RectTransform>();

        //Define las referencias externas:
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        motherbrain = GameObject.Find("Brain").GetComponent<MotherBrain>();

        //Inicializa la animacion:
		panelimg.color = new Color(1,1,1, 0);
		picoimg.color = new Color(1,1,1, 0);
		nameimg.color = new Color(1,1,1, 0);
        textColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 0);

		panelrect.localScale = new Vector3(0, 0, 0);
		picorect.localScale = new Vector3(0, 0, 0);
		namerect.localScale = new Vector3(1,0,1);

        dialogueText.alpha = 0;

        textImpact = 0;
        textDance = 0;
        textShake = 0;
        textSize = 1;

        listaCaracteres = new GameObject[500];
        indiceListaCaracteres = 0;

		//Contador para avanzar las letras una a una:
		posicion = -1;
		subposicion = 0;
		tamany = 0;

		current_width = 0;
        current_height = 0;
		deca = 1;
        nlinias = 0;

		//Evitar 2 dialogos uno encima del otro:
		if (Instance != null && Instance != this) { Destroy (gameObject); }
		else { Instance = this; }
	}

	public void AddNewDialogue(string line, string name, Vector3 v3pos, int kind)
	{
		indiceListaCaracteres = 0;

        textSize = 1;
		posicion = -1;
		subposicion = 0;
		tamany = 0;
        current_width = 0;
        current_height = 0;
        textColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 0);
		v3pos_def = v3pos;
		dialogueLine = line.Replace("\\n", "\n");
		dialogueLineName = name;
		tipo_dialogo = kind;
        textImpact = 0;
        textDance = 0;
        textShake = 0;
		CreateDialogue();
	}

	public void CreateDialogue()
	{
		indiceListaCaracteres = 0;
		deca = 1;

		//Dialogo transparente y completo para que ocupe toda la caja y coja su tamaño
        if (dialogueLine.ToCharArray()[0] == '>') { textSize = 2; }
        if (dialogueLine.ToCharArray()[0] == '<') { textSize = 0; }

        nlinias = dialogueLine.Split('ª').Length;
        float auxts = 1;
        if (textSize == 0) { auxts = 0.5f; }
        if (textSize == 2) { auxts = 1.7f; }
        current_height += (nlinias - 1) * 15 * auxts;

        string textoAuxiliar = dialogueLine;
        textoAuxiliar = textoAuxiliar.Replace('ª', '\n');
        textoAuxiliar = textoAuxiliar.Replace("{", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace("}", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace("[", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace("]", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace("~", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace("^", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace(">", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace("<", string.Empty);
        textoAuxiliar = textoAuxiliar.Replace("¬", string.Empty);
        dialogueText.text = textoAuxiliar;

        dialogueText.text = "  " + dialogueText.text + "  ";

		dialogueNameText.text = dialogueLineName;
		dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 0f);
        dialogueText.fontSize = arrayFontSizes[textSize];
		tamany = dialogueLine.Length + 1;

		panelimg.color = new Color(1,1,1, 1);
		picoimg.color = new Color(1,1,1, 1);
		nameimg.color = new Color(1,1,1, 0);

		panelrect.localScale = new Vector3(0, 0, 0);
		picorect.localScale = new Vector3(0, 0, 0);
		namerect.localScale = new Vector3(1, 0, 1);

		motherbrain.caja_dialogo_abierta = true;

		//Ajustes segun el tipo de dialogo:
		if (tipo_dialogo == 0) { dialoguePico.SetActive(false); } else { dialoguePico.SetActive(true); }

		if (tipo_dialogo == 3)
		{
			dialoguePico.SetActive(false);
			dialoguePanel.GetComponent<Image>().sprite = Resources.Load<Sprite>("GUI/dialogue_box_spike");
		}
		else
		{
			dialoguePanel.GetComponent<Image>().sprite = Resources.Load<Sprite>("GUI/dialogue_box");
		}

		posicionar_caja();

		//Iniciar la animacion para desplegar el dialogo:
		desplegar = 1;
	}

	public void CloseDialogue() //Mas bien, terminar dialogo
	{
		//Desactivar la caja de dialogo:
		panelimg.color = new Color(1,1,1, 0);
		picoimg.color = new Color(1,1,1, 0);
		nameimg.color = new Color(1,1,1, 0);
		namerect.localScale = new Vector3(1,0,1);
		dialogueText.alpha = 1;

		desplegado = 0;
	

		motherbrain.caja_dialogo_abierta = false;
		dialoguePanel.GetComponent<ContentSizeFitter>().enabled = true;

		panelrect.localScale = new Vector3(0, 0, 0);
		picorect.localScale = new Vector3(0, 0, 0);


		motherbrain.GetComponent<GestorCinematicas>().setDialogueStop(false);
	}

	// Update is called once per frame
	void Update () {

        //Ajuste de la posicion de la caja:
		if (desplegar != 0)
		{
			posicionar_caja();
		}
        
        //////////////////////////////////////////////////////////////////
        ///////////////Etapas de la animacion del dialogo/////////////////
        //////////////////////////////////////////////////////////////////

		if (desplegar == 1) //Abrirse
		{
			desplegado += Time.deltaTime*6;
			if (desplegado >= 1)
            {
                desplegado = 1;
                desplegar = 2;
                //Transparencia del texto real, el que es un solo bloque y esta de fondo:
				dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 0f);
				posicion = -1;
			}
			panelrect.localScale = new Vector3(desplegado, desplegado, desplegado);
			picorect.localScale = new Vector3(desplegado, desplegado, desplegado);

			if (deca > 0) { deca -= Time.deltaTime * 6; }
		}
        else
        if (desplegar == 2) //Mostrar el texto poco a poco
        {
            //Ajustes de la caja:
            if (deca > 0) { deca -= Time.deltaTime * 6; }
            if (deca > 1) { deca = 1; }
            if (deca < 0) { deca = 0; }

            //Ajustes del nombre del globo:
            if (dialogueLineName != "")
            {
                if (namerect.localScale.y < 1) { namerect.localScale = new Vector3(1, namerect.localScale.y + Time.deltaTime * 6, 1); }
                if (namerect.localScale.y > 1) { namerect.localScale = new Vector3(1, 1, 1); }
                nameimg.color = new Color(1, 1, 1, 1);
            }

            //Ajustes de las letras:
            subposicion += 2; //Time.deltaTime*100*acelerar;  //FIXME
			if (subposicion > 1)
            {
                subposicion = 0;
                if (posicion < tamany - 2 && deca == 0)
                {
                    posicion++;

                    string karakter = dialogueLine.Substring(posicion, 1);

                    //Caracteres especiales:
                    if (karakter == "ª") //Salto de linia
                    {
                        current_width = 0;
                        current_height -= constant_height;
                    }
                    else if (karakter == "{" || karakter == "}" || karakter == "[" || karakter == "]") //Cambio de color
                    {
                        if (karakter == "{") { textColor = new Color(0.8962264f, 0.06341223f, 0.06341223f, 0); } //Rojo
                        if (karakter == "[") { textColor = new Color(0.3960784f, 0.3960784f, 0.3960784f, 0); } //Gris
                        if (karakter == "}" || karakter == "]") { textColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 0); } //Negro
                    }
                    else if (karakter == "~") { textShake = 1 - textShake; }
                    else if (karakter == "¬") { textDance = 1 - textDance; }
                    else if (karakter == "^") { textImpact = 1 - textImpact; }
                    else if (karakter == ">") { textSize++; if (textSize > 2) { textSize = 2; } }
                    else if (karakter == "<") { textSize--; if (textSize < 0) { textSize = 0; } }
                    //Caracteres normales:
                    else
                    {

                        //Crear objeto de una nueva letra:
                        GameObject caracter_suelto = (GameObject)Instantiate(Resources.Load("Objetos/obj_IndividualChar"));
                        listaCaracteres[indiceListaCaracteres] = caracter_suelto;
                        indiceListaCaracteres++;

                        TextMeshProUGUI componenteDeTexto = caracter_suelto.GetComponent<TextMeshProUGUI>();

                        componenteDeTexto.text = dialogueLine.Substring(posicion, 1);
                        caracter_suelto.transform.SetParent(dialogueCharBox.transform);


                        //Configurar el tamanyo y color del texto:
                        componenteDeTexto.fontSize = arrayFontSizes[textSize];
                        componenteDeTexto.color = textColor;
                        dialogueText.fontSize = componenteDeTexto.fontSize;

                        //Configurar la animacion del texto:
                        caracter_suelto.GetComponent<scr_individualChar>().shake = textShake;
                        if (textDance > 0) { caracter_suelto.GetComponent<scr_individualChar>().dance = 1 + posicion; }
                        caracter_suelto.GetComponent<scr_individualChar>().impact = textImpact;

                        //Posicionarlo en las coordenadas que tocan:
                        Transform refer = transform.Find("Button").transform.Find("Text").transform;
                        float char_width = caracter_suelto.GetComponent<TextMeshProUGUI>().preferredWidth;
                        float char_height = caracter_suelto.GetComponent<TextMeshProUGUI>().preferredHeight;

                        if (dialogueLine.Substring(posicion, 1) == " ") //Ajustar el tamanyo del espacio
                        {
                            char_width = 5;
                            if (textSize == 0) { char_width--; } if (textSize == 2) { char_width += 5; }
                        }

                        current_width += char_width * 1.025f - Mathf.Max(0,((textSize - 1) * 0.5f));
                        constant_height = char_height;
                        caracter_suelto.transform.position = new Vector3(refer.position.x - refer.GetComponent<RectTransform>().rect.width / 2 + current_width - char_width / 2 - 2.4f,
                                                                         refer.position.y + current_height,
                                                                         refer.position.z);
                    }
                }

            }


        }
        else
		if (desplegar == 3) //Cerrarse
		{
			desplegado -= Time.deltaTime*6;
			if (namerect.localScale.y > 0) { namerect.localScale = new Vector3(1, namerect.localScale.y - Time.deltaTime * 6, 1); }
			if (namerect.localScale.y < 0) { namerect.localScale = new Vector3(1, 0, 1); }
			panelrect.localScale = new Vector3(desplegado, desplegado, desplegado);
			picorect.localScale = new Vector3(desplegado, desplegado, desplegado);

			if (desplegado <= 0) { desplegado = 0; desplegar = 0; CloseDialogue(); }

			if (deca < 1) { deca += Time.deltaTime * 6; }

            //Borrar las letras:
			for (int i = 0; i < indiceListaCaracteres; i++) { Destroy(listaCaracteres[i]); }
		}
			



		//Al pulsar una tecla el dialogo avanza mas rapido
		if (motherbrain.get_pulsa_Z() || motherbrain.get_pulsa_X()) { acelerar = 2; } else { acelerar = 1; }

		//Al pulsar Z, si el dialogo estaba completamente dibujado, termina:
			if (motherbrain.get_pulsa_Z_down() && posicion == tamany - 2 && desplegar == 2)
		{
			dialogueText.text = "";
			desplegar = 3;
		}

	}

	void posicionar_caja()
	{
		float act_height = dialogueLine.Split('\n').Length;

		Vector3 player_head = new Vector3 (v3pos_def.x, v3pos_def.y + get_head_altura() + act_height, v3pos_def.z);
		Vector3 screenPos = camera.WorldToScreenPoint(player_head);

		dialoguePanel.transform.position = new Vector3(screenPos.x, screenPos.y - deca * 50, screenPos.z);
		dialoguePico.transform.position = new Vector3(screenPos.x, screenPos.y + 3 - deca * 50, screenPos.z);
		dialogueName.transform.position = new Vector3(screenPos.x - panelrect.rect.width/2,
													  screenPos.y + panelrect.rect.height,
			                                          screenPos.z);
	}

	float get_head_altura()
	{
		float numero = 4 - (dialogueLine.Split('\n').Length - 1);
		//print("NUM HEADO = " + numero);
		return numero;
	}

}
